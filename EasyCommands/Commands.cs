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
            protected bool async = false;

            public bool IsAsync()
            {
                return async;
            }

            public void SetAsync(bool async)
            {
                this.async = async;
            }

            public virtual void Reset() { }

            //Returns true if the program has finished execution.
            public abstract bool Execute();
        }

        public abstract class HandlerCommand : Command
        {
            private CommandHandler commandHandler;

            public HandlerCommand(List<CommandParameter> commandParameters)
            {
                PreParseCommands(commandParameters);

                Print("Post Parsed Command Parameters: ");
                commandParameters.ForEach(param => Print("" + param.GetType()));

                foreach (CommandHandler handler in GetHandlers())
                {
                    if (handler.CanHandle(commandParameters)) {
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

            public override void Reset()
            {
                commandHandler.Reset();
            }

            public abstract void PreParseCommands(List<CommandParameter> commandParameters);

            public abstract List<CommandHandler> GetHandlers();
        }

        public class RestartCommand : Command
        {
            public override bool Execute()
            {
                RUNNING_COMMANDS.Clear();//Clear any existing commands
                RUNNING_COMMANDS.Loop(1);//Repeat Running Commands 1 iteration ("restart")
                return false;
            }
        }

        public class LoopCommand : HandlerCommand
        {
            public LoopCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>() { new LoopCommandHandler() };
            }

            public override void PreParseCommands(List<CommandParameter> commandParameters)
            {
                int numericIndex = commandParameters.FindIndex(p => p is NumericCommandParameter);
                if (numericIndex < 0) commandParameters.Add(new NumericCommandParameter(1));
                commandParameters.RemoveAll(p => p is LoopCommandParameter);
            }
        }

        public class LoopCommandHandler : OneParameterCommandHandler<NumericCommandParameter>
        {
            public override bool Handle()
            {
                RUNNING_COMMANDS.Loop((int)parameter.Value);
                return true;
            }
        }

        public class WaitCommand : HandlerCommand
        {
            public WaitCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override void PreParseCommands(List<CommandParameter> commandParameters)
            {
                commandParameters.RemoveAll(param => param is WaitCommandParameter);
                bool unitExists = commandParameters.Exists(param => param is UnitCommandParameter);
                bool timeExists = commandParameters.Exists(param => param is NumericCommandParameter);

                if (!timeExists && !unitExists) {commandParameters.Add(new NumericCommandParameter(1f)); commandParameters.Add(new UnitCommandParameter(UnitType.TICKS));}
                else if (!unitExists) { commandParameters.Add(new UnitCommandParameter(UnitType.SECONDS)); }
            }
            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>
                {
                    new WaitForDurationUnitHandler()
                };
            }
        }

        public class NullCommand : Command
        {
            public override bool Execute()
            {
                Print("Null Program");
                return true;
            }
        }

        public class BlockHandlerCommand : HandlerCommand
        {
            private BlockHandler blockHandler;
            private IEntityProvider entityProvider;

            public BlockHandlerCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {

            }

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
                if (alwaysEvaluate) updateAlwaysEvaluate();
            }

            public override bool Execute()
            {
                Print("Executing Conditional Command");
                Print("Async: " + async);
                Print("Condition: " + condition.ToString());
                Print("Action Command: " + conditionMetCommand.ToString());
                Print("Other Command: " + conditionNotMetCommand.ToString());
                Print("Always Evaluate: " + alwaysEvaluate);
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

            private void updateAlwaysEvaluate()
            {
                alwaysEvaluate = true;
                if (conditionMetCommand is ConditionalCommand) ((ConditionalCommand)conditionMetCommand).updateAlwaysEvaluate();
                if (conditionNotMetCommand is ConditionalCommand) ((ConditionalCommand)conditionNotMetCommand).updateAlwaysEvaluate();
            }

            private bool EvaluateCondition()
            {
                if ((!isExecuting && alwaysEvaluate) || !evaluated)
                {
                    Print("Evaluating Value");
                    evaluatedValue = condition.Evaluate(); evaluated = true;
                }
                Print("Evaluated Value: " + evaluatedValue);
                return evaluatedValue;
            }
        }

        public class MultiActionCommand : Command
        {
            List<Command> commandsToExecute;
            List<Command> currentCommands;
            int loopCount = 0;

            public MultiActionCommand(List<Command> commandsToExecute)
            {
                this.commandsToExecute = commandsToExecute;
                this.currentCommands = new List<Command>(commandsToExecute);
            }

            public override bool Execute()
            {
                Print("Executing All Commands.  Commands left: " + currentCommands.Count);
                Print("Loops Left: " + loopCount);
                if (currentCommands.Count == 0) return true;

                int commandIndex = 0;

                while (commandIndex < currentCommands.Count)
                {
                    Command nextCommand = currentCommands[commandIndex];

                    bool handled = nextCommand.Execute();
                    if (handled) { currentCommands.RemoveAt(commandIndex); } else { commandIndex++; }
                    if (!nextCommand.IsAsync()) break;
                    Print("Command is async, continuing to command at index: " + commandIndex);
                }

                if (currentCommands.Count > 0) return false;
                if (loopCount == 0) return true;

                Reset();
                loopCount--;
                return false;
            }

            public override void Reset()
            {
                currentCommands = new List<Command>(commandsToExecute);
                foreach (Command command in currentCommands) { command.Reset(); }
            }

            public void Clear()
            {
                currentCommands.Clear();
            }

            public void Loop(int times)
            {
                loopCount+=times;
            }
        }
    }
}
