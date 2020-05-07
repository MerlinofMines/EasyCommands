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

namespace IngameScript
{
    partial class Program
    {

        public abstract class Command
        {
            public bool Async = false;
            public virtual void Reset() { }
            protected virtual Command Clone() { return this; }
            public Command Copy()
            {
                Command copy = Clone();
                if (Async) copy.Async = true;
                return copy;
            }

            //Returns true if the program has finished execution.
            public abstract bool Execute();
        }

        public abstract class HandlerCommand : Command
        {
            private CommandHandler commandHandler;
            protected List<CommandParameter> commandParameters;

            public HandlerCommand(List<CommandParameter> parameters)
            {
                commandParameters = parameters;
                parameters = new List<CommandParameter>(parameters);

                PreParseCommands(parameters);

                Debug("Command Handler Post Parsed Command Parameters: ");
                parameters.ForEach(param => Debug("" + param.GetType()));

                foreach (CommandHandler handler in GetHandlers())
                {
                    if (handler.CanHandle(parameters)) {
                        commandHandler = handler;
                        return;
                    }
                }

                throw new Exception("Unsupported Command Parameter Combination");
            }

            public override bool Execute()
            {
                return commandHandler.Handle();
            }

            public override void Reset() { commandHandler.Reset();}

            public abstract void PreParseCommands(List<CommandParameter> parameters);

            public abstract List<CommandHandler> GetHandlers();
        }

        public class ControlCommand : Command
        {
            List<CommandParameter> parameters;
            ControlType controlType;
            public ControlCommand(List<CommandParameter> parameters)
            {
                int controlIndex = parameters.FindIndex(p => p is ControlCommandParameter);
                if (controlIndex < 0) throw new Exception("Control Command must have ControlType");
                controlType = ((ControlCommandParameter)parameters[controlIndex]).Value;
                this.parameters = parameters;
            }

            public override bool Execute()
            {
                switch (controlType) {
                    case ControlType.STOP:
                        RUNNING_COMMANDS = null; state = ProgramState.STOPPED; break;
                    case ControlType.PARSE:
                        RUNNING_COMMANDS = null; ParseCommands(); state = ProgramState.STOPPED; break;
                    case ControlType.START:
                        RUNNING_COMMANDS = null; state = ProgramState.RUNNING; break;
                    case ControlType.RESTART:
                        RUNNING_COMMANDS.Reset(); RUNNING_COMMANDS.Loop(1); state = ProgramState.RUNNING; break;
                    case ControlType.PAUSE:
                        state = ProgramState.PAUSED; break;
                    case ControlType.RESUME:
                        state = ProgramState.RUNNING; break;
                    case ControlType.LOOP:
                        int numericIndex = parameters.FindIndex(p => p is NumericCommandParameter);
                        float loopAmount = (numericIndex < 0) ? 1 : ((NumericCommandParameter)parameters[numericIndex]).Value;
                        RUNNING_COMMANDS.Loop((int)loopAmount); state = ProgramState.RUNNING; break;
                    default: throw new Exception("Unsupported Control Type");
                }
                return true;
            }
            protected override Command Clone() { return new ControlCommand(parameters); }
        }

        public class WaitCommand : HandlerCommand
        {
            public WaitCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override void PreParseCommands(List<CommandParameter> parameters) {
                parameters.RemoveAll(param => param is WaitCommandParameter);
                bool unitExists = parameters.Exists(param => param is UnitCommandParameter);
                bool timeExists = parameters.Exists(param => param is NumericCommandParameter);

                if (!timeExists && !unitExists) {parameters.Add(new NumericCommandParameter(1f)); parameters.Add(new UnitCommandParameter(UnitType.TICKS));}
                else if (!unitExists) { parameters.Add(new UnitCommandParameter(UnitType.SECONDS)); }
            }
            public override List<CommandHandler> GetHandlers() {return new List<CommandHandler> {new WaitForDurationUnitHandler()};}
            protected override Command Clone() { return new WaitCommand(commandParameters); }
        }

        public class NullCommand : Command {public override bool Execute() {return true;}}

        public class BlockHandlerCommand : HandlerCommand
        {
            private BlockHandler blockHandler;
            private IEntityProvider entityProvider;

            public BlockHandlerCommand(List<CommandParameter> commandParameters) : base(commandParameters) { }

            public override void PreParseCommands(List<CommandParameter> commandParameters)
            {
                int selectorIndex = commandParameters.FindIndex(param => param is SelectorCommandParameter);

                if (selectorIndex < 0) throw new Exception("SelectorCommandParameter is required for command: " + GetType());

                SelectorCommandParameter selectorParameter = (SelectorCommandParameter)commandParameters[selectorIndex];
                commandParameters.RemoveAt(selectorIndex);

                entityProvider = new SelectorEntityProvider(selectorParameter);
                blockHandler = BlockHandlerRegistry.GetBlockHandler(selectorParameter.blockType);

                //TODO: Move to proper command parameter pre-processor
                int boolPropIndex = commandParameters.FindIndex(param => param is BooleanPropertyCommandParameter);
                int stringPropIndex = commandParameters.FindIndex(param => param is StringPropertyCommandParameter);
                int numericPropIndex = commandParameters.FindIndex(param => param is NumericPropertyCommandParameter);
                int boolIndex = commandParameters.FindIndex(param => param is BooleanCommandParameter);
                int stringIndex = commandParameters.FindIndex(param => param is StringCommandParameter);
                int numericIndex = commandParameters.FindIndex(param => param is NumericCommandParameter);
                int directionIndex = commandParameters.FindIndex(param => param is DirectionCommandParameter);
                int reverseIndex = commandParameters.FindIndex(param => param is ReverseCommandParameter);

                if (boolPropIndex < 0 && boolIndex >= 0)
                {
                    commandParameters.Add(new BooleanPropertyCommandParameter(blockHandler.GetDefaultBooleanProperty()));
                }

                if (boolIndex < 0 && boolPropIndex >= 0)
                {
                    commandParameters.Add(new BooleanCommandParameter(true));
                }

                if (stringPropIndex < 0 && stringIndex >= 0)
                {
                    commandParameters.Add(new StringPropertyCommandParameter(blockHandler.GetDefaultStringProperty()));
                }

                if (stringPropIndex >= 0 && stringIndex < 0)//TODO: Block Default String Value per Property?
                {
                    commandParameters.Add(new StringCommandParameter(""));
                }

                if (numericIndex >= 0)
                {
                    DirectionType direction;
                    if (directionIndex >= 0)
                    {
                        direction = ((DirectionCommandParameter)commandParameters[directionIndex]).Value;
                    } else
                    {
                        direction = blockHandler.GetDefaultDirection();
                        commandParameters.Add(new DirectionCommandParameter(direction));
                    }

                    if (numericPropIndex < 0)
                    {
                        commandParameters.Add(new NumericPropertyCommandParameter(blockHandler.GetDefaultNumericProperty(direction)));
                    }
                }

                if (directionIndex >= 0 && numericPropIndex <= 0)
                {
                    DirectionType direction = ((DirectionCommandParameter)commandParameters[directionIndex]).Value;
                    commandParameters.Add(new NumericPropertyCommandParameter(blockHandler.GetDefaultNumericProperty(direction)));
                }

                if (reverseIndex >=0 && numericPropIndex < 0)
                {
                    commandParameters.Add(new NumericPropertyCommandParameter(blockHandler.GetDefaultNumericProperty(blockHandler.GetDefaultDirection())));
                }
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>() {
                    //Boolean Handlers
                    new TwoParamBlockCommandHandler<BooleanPropertyCommandParameter, BooleanCommandParameter>(entityProvider, blockHandler, (b,e,p,s)=>{  b.SetBooleanPropertyValue(e, p.Value, s.Value); }),

                    //String Handlers
                    new TwoParamBlockCommandHandler<StringPropertyCommandParameter, StringCommandParameter>(entityProvider, blockHandler, (bl,e,p,bo)=>{  bl.SetStringPropertyValue(e, p.Value, bo.Value); }),

                    //Numeric Handlers
                    new TwoParamBlockCommandHandler<NumericPropertyCommandParameter, NumericCommandParameter>(entityProvider, blockHandler, (b,e,p,n)=>{  b.SetNumericPropertyValue(e, p.Value, n.Value); }),
                    new TwoParamBlockCommandHandler<NumericPropertyCommandParameter, DirectionCommandParameter>(entityProvider, blockHandler, (b,e,p,d)=>{  b.MoveNumericPropertyValue(e, p.Value, d.Value); }),
                    new TwoParamBlockCommandHandler<ReverseCommandParameter, NumericPropertyCommandParameter>(entityProvider, blockHandler, (b,e,r,n)=>{  b.ReverseNumericPropertyValue(e, n.Value); }),
                    new ThreeParamBlockCommandHandler<NumericPropertyCommandParameter, DirectionCommandParameter, NumericCommandParameter>(entityProvider, blockHandler, (b,e,p,d,n)=>{ b.SetNumericPropertyValue(e, p.Value, d.Value, n.Value); }),
                    new ThreeParamBlockCommandHandler<NumericPropertyCommandParameter, NumericCommandParameter, RelativeCommandParameter>(entityProvider, blockHandler, (b,e,p,n,r)=>{ b.IncrementNumericPropertyValue(e, p.Value, n.Value); }),
                    new FourParamBlockCommandHandler<NumericPropertyCommandParameter, DirectionCommandParameter, NumericCommandParameter, RelativeCommandParameter>(entityProvider, blockHandler, (b,e,p,d,n,r)=>{ b.IncrementNumericPropertyValue(e, p.Value, d.Value, n.Value); }),

                    //TODO: GPS Handler?
                };
            }
            protected override Command Clone() { return new BlockHandlerCommand(commandParameters); }
        }

        public class ConditionalCommand : Command
        {
            private Condition condition;
            public bool alwaysEvaluate = false;
            private bool evaluated = false;
            private bool evaluatedValue = false;
            private bool isExecuting = false;
            private Command conditionMetCommand;
            private Command conditionNotMetCommand;

            public ConditionalCommand(Condition condition, Command conditionMetCommand, Command conditionNotMetCommand, bool alwaysEvaluate)
            {
                this.condition = condition;
                this.conditionMetCommand = conditionMetCommand;
                this.conditionNotMetCommand = conditionNotMetCommand;
                this.alwaysEvaluate = alwaysEvaluate;
                if (alwaysEvaluate) UpdateAlwaysEvaluate();
            }

            public override bool Execute()
            {
                Print("Executing Conditional Command");
                Print("Async: " + Async);
                Print("Condition: " + condition.ToString());
                Debug("Action Command: " + conditionMetCommand.ToString());
                Debug("Other Command: " + conditionNotMetCommand.ToString());
                Debug("Always Evaluate: " + alwaysEvaluate);
                bool conditionMet = EvaluateCondition();
                bool commandResult = false;

                if (conditionMet)
                {
                    commandResult = conditionMetCommand.Execute();
                } else
                {
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

            public override void Reset()
            {
                conditionMetCommand.Reset();
                conditionNotMetCommand.Reset();
                evaluated = false;
                isExecuting = false;
            }
            protected override Command Clone() { return new ConditionalCommand(condition, conditionMetCommand.Copy(), conditionNotMetCommand.Copy(), alwaysEvaluate); }

            private void UpdateAlwaysEvaluate()
            {
                alwaysEvaluate = true;
                if (conditionMetCommand is ConditionalCommand) ((ConditionalCommand)conditionMetCommand).UpdateAlwaysEvaluate();
                if (conditionNotMetCommand is ConditionalCommand) ((ConditionalCommand)conditionNotMetCommand).UpdateAlwaysEvaluate();
            }

            private bool EvaluateCondition()
            {
                if ((!isExecuting && alwaysEvaluate) || !evaluated)
                {
                    Debug("Evaluating Value");
                    evaluatedValue = condition.Evaluate(); evaluated = true;
                }
                Print("Evaluated Value: " + evaluatedValue);
                return evaluatedValue;
            }
        }

        public class MultiActionCommand : Command
        {
            List<Command> commandsToExecute;
            List<Command> currentCommands = null;
            int loopCount = 0;

            public MultiActionCommand(List<Command> commandsToExecute)
            {
                this.commandsToExecute = commandsToExecute;
            }

            public override bool Execute()
            {
                if (currentCommands == null)
                {
                    currentCommands = commandsToExecute.Select(c => c.Copy()).ToList();//Deep Copy
                    loopCount = Math.Max(0, loopCount - 1);//Decrement and stay at 0.  If 0, execute once and stay at 0.
                }

                Print("Commands left: " + currentCommands.Count);
                Print("Loops Left: " + loopCount);

                int commandIndex = 0;

                while (currentCommands != null && commandIndex < currentCommands.Count)
                {
                    Command nextCommand = currentCommands[commandIndex];

                    bool handled = nextCommand.Execute();
                    if (handled && currentCommands != null) { currentCommands.RemoveAt(commandIndex); } else { commandIndex++; }
                    if (!nextCommand.Async) break;
                    Print("Command is async, continuing to command at index: " + commandIndex);
                }

                if (currentCommands != null && currentCommands.Count > 0) return false;
                if (loopCount == 0) return true;

                Reset();
                return false;
            }
            public override void Reset() { currentCommands = null; }
            protected override Command Clone() { return new MultiActionCommand(commandsToExecute); }
            public void Loop(int times) { loopCount+=times; }
        }
    }
}
