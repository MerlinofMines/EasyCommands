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
            public FunctionType type;
            public FunctionDefinition functionDefinition;
            public Dictionary<String, Variable> inputParameters;

            MultiActionCommand function;

            public FunctionCommand(FunctionType type, FunctionDefinition functionDefinition, Dictionary<string, Variable> inputParameters) {
                this.type = type;
                this.functionDefinition = functionDefinition;
                this.inputParameters = inputParameters;
                function = null;
            }

            public override bool Execute() {
                if (function == null) {
                    function = (MultiActionCommand)FUNCTIONS[functionDefinition.functionName].function.Copy();
                    foreach(string key in inputParameters.Keys) {
                        Program.memoryVariables[key] = new StaticVariable(inputParameters[key].GetValue());
                    }
                }
                STATE = ProgramState.RUNNING;
                switch (type) {
                    case FunctionType.GOSUB:
                        return function.Execute();
                    case FunctionType.GOTO:
                        RUNNING_COMMANDS = function;
                        RUNNING_FUNCTION = functionDefinition.functionName;
                        return false;
                    case FunctionType.SWITCH:
                        RUNNING_COMMANDS = function;
                        RUNNING_FUNCTION = functionDefinition.functionName;
                        STATE = ProgramState.STOPPED;
                        return true;
                    default:
                        throw new Exception("Unsupported Function Type: " + type);
                }
            }
            protected override Command Clone() { return new FunctionCommand(type, functionDefinition, inputParameters); }
        }

        public class VariableAssignmentCommand : Command {
            public String variableName;
            public Variable variable;
            public bool useReference;

            public VariableAssignmentCommand(string variableName, Variable variable, bool useReference) {
                this.variableName = variableName;
                this.variable = variable;
                this.useReference = useReference;
            }

            public override bool Execute() {
                Variable value = useReference ? variable : new StaticVariable(variable.GetValue());
                Program.memoryVariables[variableName] = value;
                return true;
            }

            protected override Command Clone() { return new VariableAssignmentCommand(variableName, variable, useReference); }
        }

        public class ControlCommand : Command {
            Variable loopAmount = new StaticVariable(new NumberPrimitive(1));
            ControlType controlType;
            public ControlCommand(List<CommandParameter> parameters) {
                int controlIndex = parameters.FindIndex(p => p is ControlCommandParameter);
                int loopIndex = parameters.FindIndex(p => p is VariableCommandParameter);
                if (controlIndex < 0) throw new Exception("Control Command must have ControlType");
                controlType = ((ControlCommandParameter)parameters[controlIndex]).Value;
                if (loopIndex >=0) loopAmount = ((VariableCommandParameter)parameters[controlIndex]).Value;
            }

            public ControlCommand(ControlType controlType, Variable loopAmount) {
                this.controlType = controlType;
                this.loopAmount = loopAmount;
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
                        float loops = CastNumber(loopAmount.GetValue()).GetNumericValue();
                        RUNNING_COMMANDS.Loop((int)loops); STATE = ProgramState.RUNNING; break;
                    default: throw new Exception("Unsupported Control Type");
                }
                return true;
            }
            protected override Command Clone() { return new ControlCommand(controlType, loopAmount); }
        }

        public class WaitCommand : Command {
            public Variable waitInterval;
            public UnitType units;
            int ticksLeft = 0;
            public WaitCommand(List<CommandParameter> parameters) {
                int unitIndex = parameters.FindIndex(param => param is UnitCommandParameter);
                int timeIndex = parameters.FindIndex(param => param is VariableCommandParameter);

                if (unitIndex < 0 && timeIndex < 0) {
                    units = UnitType.TICKS;
                } else {
                    units = (unitIndex < 0 ? UnitType.SECONDS : ((UnitCommandParameter)parameters[unitIndex]).Value);
                }
                waitInterval = (timeIndex < 0 ? new StaticVariable(new NumberPrimitive(1)) : ((VariableCommandParameter)parameters[timeIndex]).Value);
            }

            public WaitCommand(Variable waitInterval, UnitType units) {
                this.waitInterval = waitInterval;
                this.units = units;
            }

            protected override Command Clone() { return new WaitCommand(waitInterval,units); }
            public override void Reset() { ticksLeft = 0; }
            public override bool Execute() {
                if (ticksLeft == 0) ticksLeft = getTicks(CastNumber(waitInterval.GetValue()).GetNumericValue(), units);
                Print("Waiting for " + ticksLeft + " ticks");
                ticksLeft--;
                return ticksLeft == 0;
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
            public Variable tag;
            public ListenCommand(List<CommandParameter> commandParameters) {
                int tagIndex = commandParameters.FindIndex(p => p is VariableCommandParameter);
                if (tagIndex < 0) throw new Exception("Tag is required");
                tag = ((VariableCommandParameter)commandParameters[tagIndex]).Value;
            }
            public override bool Execute() {
                PROGRAM.IGC.RegisterBroadcastListener(CastString(tag.GetValue()).GetStringValue());
                return true;
            }
        }

        public class SendCommand : Command {
            public Variable message, tag;
            public SendCommand(List<CommandParameter> commandParameters) {
                int messageIndex = commandParameters.FindIndex(p => p is VariableCommandParameter);
                int tagIndex = commandParameters.FindLastIndex(p => p is VariableCommandParameter);
                if (tagIndex < 0 || messageIndex == tagIndex) throw new Exception("Both Message and Tag must be present");
                message = ((VariableCommandParameter)commandParameters[messageIndex]).Value;
                tag = ((VariableCommandParameter)commandParameters[tagIndex]).Value;
            }
            public override bool Execute() {
                PROGRAM.IGC.SendBroadcastMessage(CastString(tag.GetValue()).GetStringValue(), CastString(message.GetValue()).GetStringValue());
                return true; 
            }
        }

        public class NullCommand : Command { public override bool Execute() { return true; } }

        public class BlockCommand : Command {
            public BlockHandler blockHandler;
            public EntityProvider entityProvider;
            public BlockCommandHandler commandHandler;

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
            }

            public List<BlockCommandHandler> GetHandlers() {
                return new List<BlockCommandHandler>() {
                    //Primitive Handlers
                    new BlockCommandHandler1<VariableCommandParameter>((b,e,v)=>{
                        Primitive result = v.Value.GetValue();
                        PropertyType propertyType = b.GetDefaultProperty(result.GetType());
                        b.SetPropertyValue(e, propertyType, v.Value.GetValue());
                    }),
                    new BlockCommandHandler1<PropertyCommandParameter>((b,e,p)=>{
                        b.SetPropertyValue(e, p.Value, new BooleanPrimitive(true));
                    }),
                    new BlockCommandHandler1<DirectionCommandParameter>((b,e,d)=>{
                        PropertyType propertyType = b.GetDefaultProperty(d.Value);
                        b.MoveNumericPropertyValue(e, propertyType, d.Value);
                    }),
                    new BlockCommandHandler2<PropertyCommandParameter, VariableCommandParameter>((b,e,p,v)=>{
                        b.SetPropertyValue(e, p.Value, v.Value.GetValue());
                    }),
                    new BlockCommandHandler2<DirectionCommandParameter, VariableCommandParameter>((b,e,d,v)=>{
                        PropertyType property = b.GetDefaultProperty(d.Value);
                        b.SetPropertyValue(e, property, v.Value.GetValue());
                    }),
                    new BlockCommandHandler3<PropertyCommandParameter, DirectionCommandParameter, VariableCommandParameter>((b,e,p,d,v)=>{
                        b.SetPropertyValue(e,p.Value,d.Value,v.Value.GetValue());
                    }),
                    new BlockCommandHandler3<PropertyCommandParameter, VariableCommandParameter, RelativeCommandParameter>((b,e,p,v,r)=>{
                        b.IncrementPropertyValue(e,p.Value,v.Value.GetValue());
                    }),
                    new BlockCommandHandler3<DirectionCommandParameter, VariableCommandParameter, RelativeCommandParameter>((b,e,d,v,r)=>{
                        PropertyType property = b.GetDefaultProperty(d.Value);
                        b.IncrementPropertyValue(e,property,d.Value,v.Value.GetValue());
                    }),
                    new BlockCommandHandler4<PropertyCommandParameter, DirectionCommandParameter, VariableCommandParameter, RelativeCommandParameter>((b,e,p,d,v,r)=>{
                        b.IncrementPropertyValue(e,p.Value,d.Value,v.Value.GetValue());
                    }),
                    new BlockCommandHandler2<PropertyCommandParameter, DirectionCommandParameter>((b,e,p,d)=>{
                        b.MoveNumericPropertyValue(e, p.Value, d.Value); }),
                    new BlockCommandHandler2<ReverseCommandParameter, PropertyCommandParameter>((b,e,r,p)=>{
                        b.ReverseNumericPropertyValue(e, p.Value); }),
                    new BlockCommandHandler1<ReverseCommandParameter>((b,e,r)=>{
                        PropertyType property = b.GetDefaultProperty(b.GetDefaultDirection());
                        b.ReverseNumericPropertyValue(e, property); }),
                };
            }
            protected override Command Clone() { return new BlockCommand(blockHandler, entityProvider, commandHandler); }
        }

        public class ConditionalCommand : Command {
            public Variable Condition;
            public bool alwaysEvaluate = false;
            public bool evaluated = false;
            public bool evaluatedValue = false;
            public bool isExecuting = false;
            public Command conditionMetCommand;
            public Command conditionNotMetCommand;

            public ConditionalCommand(Variable condition, Command conditionMetCommand, Command conditionNotMetCommand, bool alwaysEvaluate) {
                this.Condition = condition;
                this.conditionMetCommand = conditionMetCommand;
                this.conditionNotMetCommand = conditionNotMetCommand;
                this.alwaysEvaluate = alwaysEvaluate;
                if (alwaysEvaluate) UpdateAlwaysEvaluate();
            }

            public override bool Execute() {
                Print("Executing Conditional Command");
                Print("Async: " + Async);
                Print("Condition: " + Condition.ToString());
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
            protected override Command Clone() { return new ConditionalCommand(Condition, conditionMetCommand.Copy(), conditionNotMetCommand.Copy(), alwaysEvaluate); }

            private void UpdateAlwaysEvaluate() {
                alwaysEvaluate = true;
                if (conditionMetCommand is ConditionalCommand) ((ConditionalCommand)conditionMetCommand).UpdateAlwaysEvaluate();
                if (conditionNotMetCommand is ConditionalCommand) ((ConditionalCommand)conditionNotMetCommand).UpdateAlwaysEvaluate();
            }

            private bool EvaluateCondition() {
                if ((!isExecuting && alwaysEvaluate) || !evaluated) {
                    Debug("Evaluating Value");
                    evaluatedValue = CastBoolean(Condition.GetValue()).GetBooleanValue(); evaluated = true;
                }
                Print("Evaluated Value: " + evaluatedValue);
                return evaluatedValue;
            }
        }

        public class MultiActionCommand : Command {
            List<Command> commandsToExecute;
            List<Command> currentCommands = null;
            Variable loopCount;
            int loopsLeft;

            public MultiActionCommand(List<Command> commandsToExecute, int loops = 1) : this(commandsToExecute, new StaticVariable(new NumberPrimitive(loops))) {

            }

            public MultiActionCommand(List<Command> commandsToExecute, Variable loops) {
                this.commandsToExecute = commandsToExecute;
                loopCount = loops;
            }

            public override bool Execute() {
                if (currentCommands == null || currentCommands.Count == 0) {
                    currentCommands = commandsToExecute.Select(c => c.Copy()).ToList();//Deep Copy
                    if (loopsLeft == 0) loopsLeft = (int)Math.Round(CastNumber(loopCount.GetValue()).GetNumericValue());
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
