using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {

        public class InterruptException : Exception {
            public ProgramState ProgramState;

            public InterruptException(ProgramState programState) {
                ProgramState = programState;
            }
        }

        public interface IInterruptableCommand {
            void Break();
            void Continue();
        }

        public abstract class Command {
            public virtual void Reset() { }
            public virtual Command Clone() => this;
            public virtual Command SearchCurrentCommand(Func<Command, bool> filter) => filter(this) ? this : null;

            //Returns true if the program has finished execution.
            public abstract bool Execute();
        }

        public class QueueCommand : Command {
            public Command command;
            bool async;

            public QueueCommand(Command Command, bool Async) {
                command = Command;
                async = Async;
            }

            public override bool Execute() {
                Thread thread;
                if (async) {
                    thread = new Thread(command.Clone(), "Async", "Unknown");
                    PROGRAM.QueueAsyncThread(thread);
                } else {
                    thread = new Thread(command.Clone(), "Queued", "Unknown");
                    PROGRAM.QueueThread(thread);
                }
                if (command is FunctionCommand) {
                    thread.SetName(((FunctionCommand)command).functionName());
                }
                thread.threadVariables = new Dictionary<string, IVariable>(PROGRAM.GetCurrentThread().threadVariables);
                return true;
            }
        }

        public class PrintCommand : Command {
            public IVariable variable;

            public PrintCommand(IVariable v) {
                variable = v;
            }

            public override bool Execute() {
                Print(CastString(variable.GetValue()));
                return true;
            }
        }

        public class FunctionCommand : Command {
            public bool switchExecution;
            public Supplier<string> functionName;
            public List<IVariable> inputParameters;

            public Command function;

            public FunctionCommand(bool shouldSwitch, Supplier<string> name, List<IVariable> parameters) {
                switchExecution = shouldSwitch;
                functionName = name;
                inputParameters = parameters;
                function = null;
            }

            public override bool Execute() {
                Thread currentThread = PROGRAM.GetCurrentThread();
                if (function == null) {
                    var name = functionName();
                    FunctionDefinition definition;
                    if (!PROGRAM.functions.TryGetValue(name, out definition)) throw new Exception("Invalid Function Name: " + name);
                    var parameterCount = definition.parameterNames.Count;
                    if (inputParameters.Count != parameterCount) throw new Exception("Function " + name + " expects " + parameterCount + " parameters");

                    function = definition.function.Clone();

                    for (int i = 0; i < parameterCount; i++)
                        currentThread.threadVariables[definition.parameterNames[i]] = new StaticVariable(inputParameters[i].GetValue().DeepCopy());

                    if (switchExecution) {
                        currentThread.Command = function;
                        currentThread.SetName(name);
                    }
                }

                return !switchExecution && function.Execute();
            }
            public override Command Clone() => new FunctionCommand(switchExecution, functionName, inputParameters);
            public override void Reset() => function = null;

            public override Command SearchCurrentCommand(Func<Command, bool> filter) => function.SearchCurrentCommand(filter) ?? base.SearchCurrentCommand(filter);
        }

        public class VariableAssignmentCommand : Command {
            public String variableName;
            public IVariable variable;
            public bool isGlobal, useReference;

            public VariableAssignmentCommand(string name, IVariable var, bool reference, bool global) {
                variableName = name;
                variable = var;
                useReference = reference;
                isGlobal = global;
            }

            public override bool Execute() {
                IVariable value = useReference ? variable : new StaticVariable(variable.GetValue().DeepCopy());
                if (isGlobal) {
                    PROGRAM.SetGlobalVariable(variableName, value);
                } else {
                    PROGRAM.GetCurrentThread().threadVariables[variableName] = value;
                }
                return true;
            }
        }

        public class VariableIncrementCommand : Command {
            public String variableName;
            public bool increment;
            public IVariable variable;

            public VariableIncrementCommand(String VariableName, bool Increment, IVariable Variable) {
                variableName = VariableName;
                increment = Increment;
                variable = Variable;
            }

            public override bool Execute() {
                Primitive delta = variable.GetValue();
                if (!increment) delta = delta.Not();

                IVariable newValue = new StaticVariable(PROGRAM.GetVariable(variableName).GetValue().Plus(delta));

                if (PROGRAM.GetCurrentThread().threadVariables.ContainsKey(variableName)) {
                    PROGRAM.GetCurrentThread().threadVariables[variableName] = newValue;
                } else {
                    PROGRAM.globalVariables[variableName] = newValue;
                }
                return true;
            }
        }

        public class ListVariableAssignmentCommand : Command {
            public ListIndexVariable list;
            public IVariable value;
            public bool useReference;

            public ListVariableAssignmentCommand(ListIndexVariable listVariable, IVariable v, bool reference) {
                list = listVariable;
                value = v;
                useReference = reference;
            }

            public override bool Execute() {
                list.SetValue(useReference ? value : new StaticVariable(value.GetValue().DeepCopy()));
                return true;
            }
        }

        public delegate bool ControlFunction(Thread currentThread);

        public IInterruptableCommand GetInterrupableCommand(string controlStatement) {
            IInterruptableCommand breakCommand = GetCurrentThread().GetCurrentCommand<IInterruptableCommand>(command => (command as ConditionalCommand)?.alwaysEvaluate ?? true);
            if (breakCommand == null) throw new Exception("Invalid use of " + controlStatement + " command");
            return breakCommand;
        }

        public class ControlCommand : Command {
            public ControlFunction controlFunction;
            public override bool Execute() => controlFunction(PROGRAM.currentThread);
        }

        public class WaitCommand : Command {
            public IVariable waitInterval;
            double remainingWaitTime = -1;

            public WaitCommand(IVariable variable) {
                waitInterval = variable;
            }

            public override Command Clone() => new WaitCommand(waitInterval);
            public override void Reset() { remainingWaitTime = -1; }
            public override bool Execute() {
                if (remainingWaitTime < 0) {
                    remainingWaitTime = CastNumber(waitInterval.GetValue()) * 1000;
                    return false;
                }
                remainingWaitTime -= PROGRAM.Runtime.TimeSinceLastRun.TotalMilliseconds;
                return remainingWaitTime <= 5; //if <5ms left to wait, call it.
            }
        }

        public class ListenCommand : Command {
            public IVariable tag;
            bool shouldListen;

            public ListenCommand(IVariable v, bool listen) {
                tag = v;
                shouldListen = listen;
            }

            public override bool Execute() {
                var broadcastTag = CastString(tag.GetValue());
                if (shouldListen)
                    PROGRAM.IGC.RegisterBroadcastListener(broadcastTag);
                else
                    PROGRAM.BroadCastListenerAction(listener => listener.Tag == broadcastTag, listener => PROGRAM.IGC.DisableBroadcastListener(listener));
                return true;
            }
        }

        public class SendCommand : Command {
            public IVariable message, tag;

            public SendCommand(IVariable messageVariable, IVariable tagVariable) {
                message = messageVariable;
                tag = tagVariable;
            }

            public override bool Execute() {
                PROGRAM.IGC.SendBroadcastMessage(CastString(tag.GetValue()), CastString(message.GetValue()));
                return true;
            }
        }

        public class NullCommand : Command { public override bool Execute() => true; }

        public class BlockCommand : Command {
            public ISelector entityProvider;
            public Action<IBlockHandler, Object> blockAction;

            public BlockCommand(ISelector provider, Action<IBlockHandler, Object> action) {
                entityProvider = provider;
                blockAction = action;
            }

            public override bool Execute() {
                IBlockHandler handler = BlockHandlerRegistry.GetBlockHandler(entityProvider.GetBlockType());
                entityProvider.GetEntities().ForEach(e => blockAction(handler, e));
                return true;
            }
        }

        public class TransferItemCommand : Command {
            public ISelector from, to;
            //One of these is an amount (nullable), other must be ItemFilter (non nullable)
            public IVariable first, second;

            public TransferItemCommand(ISelector source, ISelector destination, IVariable firstVariable, IVariable secondVariable) {
                from = source;
                to = destination;
                first = firstVariable;
                second = secondVariable;
            }

            public override bool Execute() {
                if (from.GetBlockType() != Block.CARGO || to.GetBlockType() != Block.CARGO) throw new Exception("Transfers can only be executed on cargo block types");

                var filter = PROGRAM.AnyItem(PROGRAM.GetItemFilters(CastString((second ?? first).GetValue())));
                var items = NewList<MyInventoryItem>();

                var toInventories = to.GetEntities().Cast<IMyInventory>().Where(i => !i.IsFull).ToList();
                var fromInventories = from.GetEntities().Cast<IMyInventory>().Where(i => toInventories.All(to => i.Owner.EntityId != to.Owner.EntityId));

                MyFixedPoint amountLeft = MyFixedPoint.MaxValue;
                if (second != null) amountLeft = (MyFixedPoint)CastNumber(first.GetValue());

                int transfers = 0;

                foreach(IMyInventory fromInventory in fromInventories) {
                    fromInventory.GetItems(items, filter);
                    for(int i = 0; i < toInventories.Count; i++) {
                        foreach (MyInventoryItem item in items) {
                            var destinationInventory = toInventories[i];
                            var startMass = fromInventory.CurrentMass;
                            fromInventory.TransferItemTo(destinationInventory, item, amountLeft);
                            amountLeft -= (startMass - fromInventory.CurrentMass);
                            if (amountLeft <= MyFixedPoint.Zero || ++transfers >= PROGRAM.maxItemTransfers) return true;
                            if (destinationInventory.IsFull) {
                                toInventories.RemoveAt(i--);
                                break;
                            }
                        }
                    }
                }
                return true;
            }
        }

        public class ConditionalCommand : Command, IInterruptableCommand {
            public IVariable condition;
            public bool alwaysEvaluate, evaluated, evaluatedValue, isExecuting, shouldBreak;
            public Command conditionMetCommand, conditionNotMetCommand;

            public ConditionalCommand(IVariable conditionVariable, Command metCommand, Command notMetCommand, bool alwaysEval) {
                condition = conditionVariable;
                conditionMetCommand = metCommand;
                conditionNotMetCommand = notMetCommand;
                alwaysEvaluate = alwaysEval;
            }

            public override bool Execute() {
                if (shouldBreak) {
                    Reset();
                    return true;
                }

                bool conditionMet = EvaluateCondition();
                bool commandResult = conditionMet ? conditionMetCommand.Execute() : conditionNotMetCommand.Execute();

                isExecuting = !commandResult;

                if (isExecuting) return false; //Keep executing subcommand

                //Finished Executing.  Reset Commands
                Reset();

                return alwaysEvaluate ? !conditionMet : commandResult;
            }

            public override void Reset() {
                conditionMetCommand.Reset();
                conditionNotMetCommand.Reset();
                evaluated = false;
                isExecuting = false;
                shouldBreak = false;
            }
            public override Command Clone() => new ConditionalCommand(condition, conditionMetCommand.Clone(), conditionNotMetCommand.Clone(), alwaysEvaluate);

            bool EvaluateCondition() {
                if ((!isExecuting && alwaysEvaluate) || !evaluated) {
                    evaluatedValue = CastBoolean(condition.GetValue());
                    evaluated = true;
                }
                return evaluatedValue;
            }

            public override Command SearchCurrentCommand(Func<Command, bool> filter) =>
                (evaluatedValue ? conditionMetCommand.SearchCurrentCommand(filter) : conditionNotMetCommand.SearchCurrentCommand(filter))
                    ?? base.SearchCurrentCommand(filter);

            public void Continue() {
                Reset();
            }

            public void Break() {
                shouldBreak = true;
            }
        }

        public class MultiActionCommand : Command {
            public List<Command> commandsToExecute, currentCommands = null;
            public IVariable loopCount;
            int loopsLeft;

            public MultiActionCommand(List<Command> commandsToExecute, int loops = 1) : this(commandsToExecute, GetStaticVariable(loops)) {
            }

            public MultiActionCommand(List<Command> commands, IVariable loops) {
                commandsToExecute = commands;
                loopCount = loops;
            }

            public override bool Execute() {
                if ((currentCommands?.Count ?? 0) == 0) {
                    currentCommands = commandsToExecute.Select(c => c.Clone()).ToList();//Deep Copy
                    if (loopsLeft == 0) loopsLeft = (int)Math.Round(CastNumber(loopCount.GetValue()));
                    loopsLeft -= 1;
                }

                while (currentCommands.Count > 0) {
                    if (currentCommands[0].Execute()) {
                        currentCommands.RemoveAt(0);
                    } else {
                        break;
                    }
                }

                if (currentCommands?.Count > 0) return false;
                if (loopsLeft <= 0) return true;

                Reset();
                return false;
            }
            public override void Reset() { currentCommands = null; }
            public override Command Clone() => new MultiActionCommand(commandsToExecute, loopCount);
            public void Loop(int times) { loopsLeft += times; }

            public override Command SearchCurrentCommand(Func<Command, bool> filter) => currentCommands[0].SearchCurrentCommand(filter) ?? base.SearchCurrentCommand(filter);
        }

        public class ForEachCommand : Command, IInterruptableCommand {
            public string iterator;
            public IVariable list;
            public Command command;
            List<IVariable> listElements = null;
            bool executed = true;

            public ForEachCommand(string Iterator, IVariable List, Command Command) {
                iterator = Iterator;
                list = List;
                command = Command;
            }

            public override bool Execute() {
                listElements = listElements ?? CastList(list.GetValue()).GetValues();

                if (executed && listElements.Count == 0) return true;

                if (executed && listElements.Count > 0) {
                    PROGRAM.GetCurrentThread().threadVariables[iterator] = listElements[0];
                    listElements.RemoveAt(0);
                    executed = false;
                }

                if (!executed) executed = command.Execute();
                if (executed) command.Reset();

                return executed && ((listElements?.Count ?? 0) == 0);
            }

            public override void Reset() {
                listElements = null;
                executed = true;
                command.Reset();
            }

            public override Command Clone() => new ForEachCommand(iterator, list, command.Clone());

            public override Command SearchCurrentCommand(Func<Command, bool> filter) => command.SearchCurrentCommand(filter) ?? base.SearchCurrentCommand(filter);

            public void Break() {
                executed = true;
                listElements = NewList<IVariable>();
                command.Reset();
            }

            public void Continue() {
                executed = true;
                command.Reset();
            }
        }
    }
}
