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

        public abstract class Command {
            public bool Async = false;
            public virtual void Reset() { }
            protected virtual Command Clone() { return this; }
            public Command Copy() {
                Command copy = Clone();
                if (Async) copy.Async = true;
                return copy;
            }

            //Returns true if the program has finished execution.
            public abstract bool Execute();
        }

        public class FunctionCommand : Command {
            MultiActionCommand function;
            String functionName;
            List<CommandParameter> parameters;
            FunctionType type;

            public FunctionCommand(List<CommandParameter> parameters) {
                this.parameters = parameters;
                int typeIndex = parameters.FindIndex(p => p is FunctionCommandParameter);
                if (typeIndex < 0) throw new Exception("Function Type is required for Function Command");
                int functionindex = parameters.FindIndex(p => p is StringCommandParameter);
                if (functionindex < 0) throw new Exception("Function name required for Function Command");
                functionName = ((StringCommandParameter)parameters[functionindex]).Value;
                type = ((FunctionCommandParameter)parameters[typeIndex]).Value;
            }

            public override bool Execute() {
                if (function == null) {
                    if (!FUNCTIONS.ContainsKey(functionName)) {
                        throw new Exception("Undefined Function Name: " + functionName);
                    }
                    function = (MultiActionCommand)FUNCTIONS[functionName].Copy();
                }
                STATE = ProgramState.RUNNING;
                switch (type) {
                    case FunctionType.GOSUB:
                        return function.Execute();
                    case FunctionType.GOTO:
                        RUNNING_COMMANDS = function;
                        RUNNING_FUNCTION = functionName;
                        return false;
                    case FunctionType.SWITCH:
                        RUNNING_COMMANDS = function;
                        RUNNING_FUNCTION = functionName;
                        STATE = ProgramState.STOPPED;
                        return true;
                    default:
                        throw new Exception("Unsupported Function Type: " + type);
                }
            }
            protected override Command Clone() { return new FunctionCommand(parameters); }
        }


        public class ControlCommand : Command {
            List<CommandParameter> parameters;
            ControlType controlType;
            public ControlCommand(List<CommandParameter> parameters) {
                int controlIndex = parameters.FindIndex(p => p is ControlCommandParameter);
                if (controlIndex < 0) throw new Exception("Control Command must have ControlType");
                controlType = ((ControlCommandParameter)parameters[controlIndex]).Value;
                this.parameters = parameters;
            }

            public override bool Execute() {
                switch (controlType) {
                    case ControlType.STOP:
                        RUNNING_COMMANDS = null; STATE = ProgramState.STOPPED; break;
                    case ControlType.PARSE:
                        RUNNING_COMMANDS = null; ParseCommands(); STATE = ProgramState.STOPPED; break;
                    case ControlType.START:
                        RUNNING_COMMANDS = null; STATE = ProgramState.RUNNING; break;
                    case ControlType.RESTART:
                        RUNNING_COMMANDS.Reset(); RUNNING_COMMANDS.Loop(1); STATE = ProgramState.RUNNING; break;
                    case ControlType.PAUSE:
                        STATE = ProgramState.PAUSED; break;
                    case ControlType.RESUME:
                        STATE = ProgramState.RUNNING; break;
                    case ControlType.LOOP:
                        int numericIndex = parameters.FindIndex(p => p is NumericCommandParameter);
                        float loopAmount = (numericIndex < 0) ? 1 : ((NumericCommandParameter)parameters[numericIndex]).Value;
                        RUNNING_COMMANDS.Loop((int)loopAmount); STATE = ProgramState.RUNNING; break;
                    default: throw new Exception("Unsupported Control Type");
                }
                return true;
            }
            protected override Command Clone() { return new ControlCommand(parameters); }
        }

        public class WaitCommand : Command {
            int waitTicks, ticks = 0;
            public WaitCommand(List<CommandParameter> parameters) {
                int unitIndex = parameters.FindIndex(param => param is UnitCommandParameter);
                int timeIndex = parameters.FindIndex(param => param is NumericCommandParameter);
                if (unitIndex < 0 && timeIndex < 0) {
                    waitTicks = 1;
                } else {
                    waitTicks = getTicks(timeIndex < 0 ? 1 : ((NumericCommandParameter)parameters[timeIndex]).Value, unitIndex < 0 ? UnitType.SECONDS : ((UnitCommandParameter)parameters[unitIndex]).Value);
                }
            }
            public WaitCommand(int ticks) { waitTicks = ticks; }
            protected override Command Clone() { return new WaitCommand(waitTicks); }
            public override void Reset() { ticks = 0; }
            public override bool Execute() {
                ticks++;
                Print("Waited for " + ticks + " ticks");
                return ticks >= waitTicks;
            }

            int getTicks(float numeric, UnitType unitType) {
                switch (unitType) {
                    case UnitType.SECONDS:
                        return (int)(numeric * 60);//Assume 60 ticks / second
                    case UnitType.TICKS:
                        return (int)numeric;
                    default:
                        throw new Exception("Unsupported Unit Type: " + unitType);
                }
            }
        }

        public class ListenCommand : Command {
            String tag;
            public ListenCommand(List<CommandParameter> commandParameters) {
                int tagIndex = commandParameters.FindIndex(p => p is StringCommandParameter);
                if (tagIndex < 0) throw new Exception("Tag is required");
                tag = ((StringCommandParameter)commandParameters[tagIndex]).Value;
            }
            public override bool Execute() { PROGRAM.IGC.RegisterBroadcastListener(tag); return true; }
        }

        public class SendCommand : Command {
            String message, tag;
            public SendCommand(List<CommandParameter> commandParameters) {
                int messageIndex = commandParameters.FindIndex(p => p is StringCommandParameter);
                int tagIndex = commandParameters.FindLastIndex(p => p is StringCommandParameter);
                if (messageIndex == tagIndex) throw new Exception("Both Message and Tag must be present");
                message = ((StringCommandParameter)commandParameters[messageIndex]).Value;
                tag = ((StringCommandParameter)commandParameters[tagIndex]).Value;
            }
            public override bool Execute() { PROGRAM.IGC.SendBroadcastMessage(tag, message); return true; }
        }

        public class NullCommand : Command { public override bool Execute() { return true; } }

        public class BlockCommand : Command {
            private BlockHandler blockHandler;
            private EntityProvider entityProvider;
            private BlockCommandHandler commandHandler;

            public BlockCommand(BlockHandler blockHandler, EntityProvider entityProvider, BlockCommandHandler commandHandler) {
                this.blockHandler = blockHandler;
                this.entityProvider = entityProvider;
                this.commandHandler = commandHandler;
            }
            public BlockCommand(List<CommandParameter> parameters) {
                parameters = new List<CommandParameter>(parameters);
                PreParseCommands(parameters);

                Debug("Command Handler Post Parsed Command Parameters: ");
                parameters.ForEach(param => Debug("" + param.GetType()));
                foreach (BlockCommandHandler handler in GetHandlers()) {
                    if (handler.canHandle(parameters)) {
                        commandHandler = handler;
                        commandHandler.b = blockHandler;
                        commandHandler.e = entityProvider;
                        return;
                    }
                }

                parameters.ForEach(param => Print("" + param.GetType()));
                throw new Exception("Unsupported Command Parameter Combination");
            }

            public override bool Execute() {
                commandHandler.Execute();
                return true;
            }

            public void PreParseCommands(List<CommandParameter> commandParameters) {
                extract<ActionCommandParameter>(commandParameters);//Extract and ignore
                SelectorCommandParameter selector = extractFirst<SelectorCommandParameter>(commandParameters);
                if (selector == null) throw new Exception("SelectorCommandParameter is required for command: " + GetType());
                blockHandler = BlockHandlerRegistry.GetBlockHandler(selector.Value.GetBlockType());
                entityProvider = selector.Value;

                var boolVals = extract<BooleanCommandParameter>(commandParameters);
                var notVals = extract<NotCommandParameter>(commandParameters);
                var boolVal = (notVals.Count % 2 == 1) ? false : true;
                boolVals.ForEach(v => boolVal = !(boolVal ^ v.Value));
                if (boolVals.Count > 0 || notVals.Count > 0) commandParameters.Add(new BooleanCommandParameter(boolVal));

                //TODO: Move to proper command parameter pre-processor
                int boolPropIndex = commandParameters.FindIndex(param => param is BooleanPropertyCommandParameter);
                int stringPropIndex = commandParameters.FindIndex(param => param is StringPropertyCommandParameter);
                int numericPropIndex = commandParameters.FindIndex(param => param is NumericPropertyCommandParameter);
                int boolIndex = commandParameters.FindIndex(param => param is BooleanCommandParameter);
                int stringIndex = commandParameters.FindIndex(param => param is StringCommandParameter);
                int numericIndex = commandParameters.FindIndex(param => param is NumericCommandParameter);
                int directionIndex = commandParameters.FindIndex(param => param is DirectionCommandParameter);
                int reverseIndex = commandParameters.FindIndex(param => param is ReverseCommandParameter);

                if (boolPropIndex < 0 && boolIndex >= 0) {
                    commandParameters.Add(new BooleanPropertyCommandParameter(blockHandler.GetDefaultBooleanProperty()));
                }

                if (boolIndex < 0 && boolPropIndex >= 0) {
                    commandParameters.Add(new BooleanCommandParameter(boolVal));
                }

                if (stringPropIndex < 0 && stringIndex >= 0) {
                    commandParameters.Add(new StringPropertyCommandParameter(blockHandler.GetDefaultStringProperty()));
                }

                if (stringPropIndex >= 0 && stringIndex < 0)//TODO: Block Default String Value per Property?
                {
                    commandParameters.Add(new StringCommandParameter(""));
                }

                if (numericIndex >= 0) {
                    DirectionType direction;
                    if (directionIndex >= 0) {
                        direction = ((DirectionCommandParameter)commandParameters[directionIndex]).Value;
                    } else {
                        direction = blockHandler.GetDefaultDirection();
                        directionIndex = commandParameters.Count;
                        commandParameters.Add(new DirectionCommandParameter(direction));
                    }

                    if (numericPropIndex < 0) {
                        numericPropIndex = commandParameters.Count;
                        commandParameters.Add(new NumericPropertyCommandParameter(blockHandler.GetDefaultNumericProperty(direction)));
                    }
                }

                if (directionIndex >= 0 && numericPropIndex < 0) {
                    DirectionType direction = ((DirectionCommandParameter)commandParameters[directionIndex]).Value;
                    numericPropIndex = commandParameters.Count;
                    commandParameters.Add(new NumericPropertyCommandParameter(blockHandler.GetDefaultNumericProperty(direction)));
                }

                if (reverseIndex >= 0 && numericPropIndex < 0) {
                    numericPropIndex = commandParameters.Count;
                    commandParameters.Add(new NumericPropertyCommandParameter(blockHandler.GetDefaultNumericProperty(blockHandler.GetDefaultDirection())));
                }
            }

            public List<BlockCommandHandler> GetHandlers() {
                return new List<BlockCommandHandler>() {
                    //Boolean Handlers
                    new BlockCommandHandler2<BooleanPropertyCommandParameter, BooleanCommandParameter>((b,e,p,s)=>{b.SetBooleanPropertyValue(e, p.Value, s.Value);}),

                    //String Handlers
                    new BlockCommandHandler2<StringPropertyCommandParameter, StringCommandParameter>((bl,e,p,bo)=>{  bl.SetStringPropertyValue(e, p.Value, bo.original); }),

                    //Numeric Handlers
                    new BlockCommandHandler2<NumericPropertyCommandParameter, NumericCommandParameter>((b,e,p,n)=>{  b.SetNumericPropertyValue(e, p.Value, n.Value); }),
                    new BlockCommandHandler2<NumericPropertyCommandParameter, DirectionCommandParameter>((b,e,p,d)=>{  b.MoveNumericPropertyValue(e, p.Value, d.Value); }),
                    new BlockCommandHandler2<ReverseCommandParameter, NumericPropertyCommandParameter>((b,e,r,n)=>{  b.ReverseNumericPropertyValue(e, n.Value); }),
                    new BlockCommandHandler3<NumericPropertyCommandParameter, DirectionCommandParameter, NumericCommandParameter>((b,e,p,d,n)=>{ b.SetNumericPropertyValue(e, p.Value, d.Value, n.Value); }),
                    new BlockCommandHandler3<NumericPropertyCommandParameter, NumericCommandParameter, RelativeCommandParameter>((b,e,p,n,r)=>{ b.IncrementNumericPropertyValue(e, p.Value, n.Value); }),
                    new BlockCommandHandler4<NumericPropertyCommandParameter, DirectionCommandParameter, NumericCommandParameter, RelativeCommandParameter>((b,e,p,d,n,r)=>{ b.IncrementNumericPropertyValue(e, p.Value, d.Value, n.Value); }),

                    //TODO: GPS Handler?
                };
            }
            protected override Command Clone() { return new BlockCommand(blockHandler, entityProvider, commandHandler); }
        }

        public class ConditionalCommand : Command {
            private Condition condition;
            public bool alwaysEvaluate = false;
            private bool evaluated = false;
            private bool evaluatedValue = false;
            private bool isExecuting = false;
            private Command conditionMetCommand;
            private Command conditionNotMetCommand;

            public ConditionalCommand(Condition condition, Command conditionMetCommand, Command conditionNotMetCommand, bool alwaysEvaluate) {
                this.condition = condition;
                this.conditionMetCommand = conditionMetCommand;
                this.conditionNotMetCommand = conditionNotMetCommand;
                this.alwaysEvaluate = alwaysEvaluate;
                if (alwaysEvaluate) UpdateAlwaysEvaluate();
            }

            public override bool Execute() {
                Print("Executing Conditional Command");
                Print("Async: " + Async);
                Print("Condition: " + condition.ToString());
                Debug("Action Command: " + conditionMetCommand.ToString());
                Debug("Other Command: " + conditionNotMetCommand.ToString());
                Debug("Always Evaluate: " + alwaysEvaluate);
                bool conditionMet = EvaluateCondition();
                bool commandResult = false;

                if (conditionMet) {
                    commandResult = conditionMetCommand.Execute();
                } else {
                    commandResult = conditionNotMetCommand.Execute();
                }

                isExecuting = !commandResult;

                if (isExecuting) return false; //Keep executing subcommand

                //Finished Executing.  Reset Commands
                conditionMetCommand.Reset();
                conditionNotMetCommand.Reset();

                //throw new Exception("Stop!");
                if (alwaysEvaluate) { return !conditionMet; } else { return commandResult; }
            }

            public override void Reset() {
                conditionMetCommand.Reset();
                conditionNotMetCommand.Reset();
                evaluated = false;
                isExecuting = false;
            }
            protected override Command Clone() { return new ConditionalCommand(condition, conditionMetCommand.Copy(), conditionNotMetCommand.Copy(), alwaysEvaluate); }

            private void UpdateAlwaysEvaluate() {
                alwaysEvaluate = true;
                if (conditionMetCommand is ConditionalCommand) ((ConditionalCommand)conditionMetCommand).UpdateAlwaysEvaluate();
                if (conditionNotMetCommand is ConditionalCommand) ((ConditionalCommand)conditionNotMetCommand).UpdateAlwaysEvaluate();
            }

            private bool EvaluateCondition() {
                if ((!isExecuting && alwaysEvaluate) || !evaluated) {
                    Debug("Evaluating Value");
                    evaluatedValue = condition.Evaluate(); evaluated = true;
                }
                Print("Evaluated Value: " + evaluatedValue);
                return evaluatedValue;
            }
        }

        public class MultiActionCommand : Command {
            List<Command> commandsToExecute;
            List<Command> currentCommands = null;
            int loopCount;
            int loopsLeft;
            public MultiActionCommand(List<Command> commandsToExecute, int loops = 1) {
                this.commandsToExecute = commandsToExecute;
                loopCount = loops;
            }

            public override bool Execute() {
                if (currentCommands == null || currentCommands.Count == 0) {
                    currentCommands = commandsToExecute.Select(c => c.Copy()).ToList();//Deep Copy
                    if (loopsLeft == 0) loopsLeft = loopCount;
                    loopsLeft -= 1;
                }

                Print("Commands left: " + currentCommands.Count);
                Print("Loops Left: " + loopsLeft);

                int commandIndex = 0;

                while (currentCommands != null && commandIndex < currentCommands.Count) {
                    Command nextCommand = currentCommands[commandIndex];

                    bool handled = nextCommand.Execute();
                    if (handled && currentCommands != null) { currentCommands.RemoveAt(commandIndex); } else { commandIndex++; }
                    if (!nextCommand.Async) break;
                    Print("Command is async, continuing to command at index: " + commandIndex);
                }

                if (currentCommands != null && currentCommands.Count > 0) return false;
                if (loopsLeft == 0) return true;

                Reset();
                return false;
            }
            public override void Reset() { currentCommands = null; }
            protected override Command Clone() { return new MultiActionCommand(commandsToExecute, loopCount); }
            public void Loop(int times) { loopsLeft += times; }
        }
    }
}
