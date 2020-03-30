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
            protected List<CommandParameter> commandParameters;

            public Command(List<CommandParameter> commandParameters)
            {
                this.commandParameters = commandParameters;
            }

            //Returns true if the program has finished execution.
            public abstract bool Execute(MyGridProgram program);

            public bool ParameterExists<T>() where T: CommandParameter
            {
                List<T> list;
                return TryGetParameters<T>(out list);
            }

            public bool TryGetParameters<T>(out List<T> matchingParameters) where T : CommandParameter
            {
                matchingParameters = new List<T>();

                foreach (CommandParameter param in commandParameters)
                {
                    if (param is T)
                    {
                        matchingParameters.Add((T)param);
                    }
                }

                return matchingParameters.Count > 0;
            }
        }

        public abstract class HandlerCommand : Command
        {
            private CommandHandler commandHandler;

            public HandlerCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
                PreParseCommands(commandParameters);

                foreach (CommandHandler handler in GetHandlers())
                {
                    if (handler.CanHandle(commandParameters)) {
                        commandHandler = handler;
                        return;
                    }
                }

                throw new Exception("Unsupported Command Parameter Combination: " + commandParameters);
            }

            public override bool Execute(MyGridProgram program)
            {
                return commandHandler.Handle(program);
            }

            public abstract void PreParseCommands(List<CommandParameter> commandParameters);

            public abstract List<CommandHandler> GetHandlers(); 
        }

        public abstract class EntityHandlerCommand<E> : HandlerCommand where E : class, IMyTerminalBlock
        {
            protected IEntityProvider<E> entityProvider;

            public EntityHandlerCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override void PreParseCommands(List<CommandParameter> commandParameters)
            {
                //TODO: Smarter resolution of which Selector, if more than 1.

                commandParameters.RemoveAll(param => param is BlockTypeCommandParameter); //Ignore these, no one needs them

                Boolean isGroup = (commandParameters.RemoveAll(param => param is GroupCommandParameter) > 0);//Remove all group, set true if at least 1 removed

                int selectorIndex = commandParameters.FindIndex(param => param is SelectorCommandParameter);

                if (selectorIndex < 0) throw new Exception("SelectorCommandParameter is required for command: " + GetType());

                SelectorCommandParameter selector = (SelectorCommandParameter) commandParameters[selectorIndex];
                commandParameters.RemoveAt(selectorIndex);

                if (isGroup)
                {
                    entityProvider = new SelectorGroupEntityProvider<E>(selector);
                } else
                {
                    entityProvider = new SelectorEntityProvider<E>(selector);
                }
            }
        }

        public abstract class EntityCommand <E> : Command where E : class, IMyTerminalBlock
        {
            public EntityCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            protected List<E> GetSelectorEntities(MyGridProgram program)
            {
                bool isGroup = commandParameters.Exists(parameter => (parameter is GroupCommandParameter));
                //TODO: Need Smart Selection by finding the closest Selector to either a Group or a BlockType Parameter.  Other param may be something else.
                SelectorCommandParameter selectorParameter = (SelectorCommandParameter) commandParameters.Find(parameter => (parameter is SelectorCommandParameter));

                if (isGroup)
                {
                    return GetGroupEntities(program, selectorParameter);
                } else
                {
                    return GetEntities(program, selectorParameter);
                }
            }

            protected List<E> GetEntities(MyGridProgram program, SelectorCommandParameter selector)
            {
                List<E> entities = new List<E>();
                program.GridTerminalSystem.GetBlocksOfType<E>(entities);

                return entities.FindAll(entity => (entity is IMyTerminalBlock) 
                    && ((IMyTerminalBlock)entity).CustomName.ToLower() == selector.getSelector());
            }

            protected List<E> GetGroupEntities(MyGridProgram program, SelectorCommandParameter selector)
            {
                IMyBlockGroup group = program.GridTerminalSystem.GetBlockGroupWithName(selector.getSelector());

                if (group == null)
                {
                    program.Echo("Unable to find requested block group: " + selector.getSelector());
                    throw new Exception();
                }

                List<E> entities = new List<E>();
                group.GetBlocksOfType<E>(entities);
                return entities;
            }

            protected void handleActivation(MyGridProgram program, List<E> functionalBlocks, ActivationCommandParameter activationParameter)
            {
                bool activation = activationParameter.isActivate();

                if (activation)
                {
                    functionalBlocks.ForEach(block => block.ApplyAction("OnOff_On"));
                }
                else
                {
                    functionalBlocks.ForEach(block => block.ApplyAction("OnOff_Off"));
                }
            }
        }

        public class WaitCommand : Command
        {
            private int ticks;

            public WaitCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
                List<NumericCommandParameter> numericParameter;
                List<UnitCommandParameter> unitParameter;

                bool isNumeric = TryGetParameters<NumericCommandParameter>(out numericParameter);
                bool isUnit = TryGetParameters<UnitCommandParameter>(out unitParameter);

                float numeric = (isNumeric) ? numericParameter[0].getValue() : 0;
                UnitType unitType = (isUnit) ? unitParameter[0].getUnit() : UnitType.SECONDS;//default to seconds
        
                if (isNumeric)
                {
                    switch (unitType)
                    {
                        case UnitType.SECONDS:
                            ticks = (int)(numeric * 60);
                            break;
                        case UnitType.TICKS:
                            ticks = (int)numeric;
                            break;
                        default:
                            throw new Exception("Unsupported Unit Type: " + unitType);
                    }
                }
                
                //TODO: parse command parameters to calculate ticks.
            }

            public override bool Execute(MyGridProgram program)
            {
                program.Echo("Waiting " + ticks + " ticks");
                ticks--;

                return ticks <= 0;
            }
        }

        public class NullCommand : Command
        {
            public NullCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override bool Execute(MyGridProgram program)
            {
                program.Echo("Null Program");
                return true;
            }
        }

        public class PistonCommand : EntityCommand<IMyPistonBase>
        {
            public PistonCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {

            }

            public override bool Execute(MyGridProgram program)
            {
                program.Echo("Controlling Pistons");

                List<IMyPistonBase> pistons = GetSelectorEntities(program);

                if (pistons.Count == 0)
                {
                    program.Echo("Could not find pistons for selector.  Returning");
                    return true;
                }

                List<NumericCommandParameter> numericParameter;
                List<IncrementCommandParameter> incrementalParameter;
                List<ActivationCommandParameter> activationParameter;

                bool isVelocity = ParameterExists<VelocityCommandParameter>();
                bool isRelative = ParameterExists<RelativeCommandParameter>();
                bool isReverse = ParameterExists<ReverseCommandParameter>();
                bool isNumeric = TryGetParameters<NumericCommandParameter>(out numericParameter);
                bool isIncremental = TryGetParameters<IncrementCommandParameter>(out incrementalParameter);
                bool isActivation = TryGetParameters<ActivationCommandParameter>(out activationParameter);

                bool incremental = (isIncremental) ? incrementalParameter[0].isIncrement() : false;
                bool activation = (isActivation) ? activationParameter[0].isActivate() : false;
                float numeric = (isNumeric) ? numericParameter[0].getValue() : 0;
                float deltaValue = numeric * (incremental ? 1 : -1);

                if (isReverse) //Handle Reverse
                {
                    pistons.ForEach(piston => piston.Reverse());
                }
                else if (isNumeric && isVelocity) //Handle Velocity Changes
                {
                    if (isIncremental && isRelative)
                    {
                        pistons.ForEach(piston => piston.Velocity = piston.Velocity + deltaValue);
                    } else
                    {
                        pistons.ForEach(piston => piston.Velocity = numericParameter[0].getValue());
                    }
                }
                else if (isNumeric) //Handle Extension Changes With Specific Value
                {
                    float desiredPosition = numeric;
                    if (isIncremental && isRelative)
                    {
                        pistons.ForEach(piston => extendPistonToValue(piston, piston.CurrentPosition + deltaValue));
                    }
                    else
                    {
                        pistons.ForEach(piston => extendPistonToValue(piston, numeric));
                    }
                }
                else if (isIncremental) //Handle Extension Changes With No Value
                {
                    if (incremental)
                    {
                        pistons.ForEach(piston => piston.Extend());
                    } else
                    {
                        pistons.ForEach(piston => piston.Retract());
                    }
                } else if (isActivation) //Handle Activations
                {
                    if (activation)
                    {
                        pistons.ForEach(piston => piston.ApplyAction("OnOff_On"));
                    } else
                    {
                        pistons.ForEach(piston => piston.ApplyAction("OnOff_Off"));
                    }
                }

                return true;
            }

            private void extendPistonToValue(IMyPistonBase piston, float value)
            {
                if (piston.CurrentPosition < value)
                {
                    piston.SetValue("UpperLimit", value);
                    piston.Extend();
                }
                else
                {
                    piston.SetValue("LowerLimit", value);
                    piston.Retract();
                }
            }
        }

        public class RotorCommand : EntityCommand<IMyMotorStator>
        {
            public RotorCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override bool Execute(MyGridProgram program)
            {
                program.Echo("Controlling Rotors");

                List<IMyMotorStator> rotors = GetSelectorEntities(program);

                if (rotors.Count == 0)
                {
                    program.Echo("Could not find rotors for selector.  Returning");
                    return true;
                }

                List<NumericCommandParameter> numericParameter;
                List<IncrementCommandParameter> incrementalParameter;
                List<ActivationCommandParameter> activationParameter;
                List<UnitCommandParameter> unitParameter;

                bool isVelocity = ParameterExists<VelocityCommandParameter>();
                bool isRelative = ParameterExists<RelativeCommandParameter>();
                bool isReverse = ParameterExists<ReverseCommandParameter>();
                bool isNumeric = TryGetParameters<NumericCommandParameter>(out numericParameter);
                bool isIncremental = TryGetParameters<IncrementCommandParameter>(out incrementalParameter);
                bool isActivation = TryGetParameters<ActivationCommandParameter>(out activationParameter);
                bool isUnit = TryGetParameters<UnitCommandParameter>(out unitParameter);

                bool incremental = (isIncremental) ? incrementalParameter[0].isIncrement() : false;
                bool activation = (isActivation) ? activationParameter[0].isActivate() : false;
                UnitType unitType = (isUnit) ? unitParameter[0].getUnit() : UnitType.DEGREES;
                float numeric = (isNumeric) ? numericParameter[0].getValue() : 0;
                float deltaValue = numeric * (incremental ? 1 : -1);
                //TODO: Handle Radians

                if (isReverse) //Handle Reverse
                {
                    rotors.ForEach(rotor => rotor.TargetVelocityRPM = -rotor.TargetVelocityRPM);
                }
                else if (isNumeric && isVelocity) //Handle Velocity Changes
                {
                    if (isIncremental && isRelative)
                    {
                        rotors.ForEach(piston => piston.TargetVelocityRPM += deltaValue);
                    }
                    else
                    {
                        rotors.ForEach(piston => piston.TargetVelocityRPM = numericParameter[0].getValue());
                    }
                }
                else if (isNumeric) //Handle Extension Changes With Specific Value
                {
                    float desiredPosition = numeric;
                    if (isIncremental && isRelative)
                    {
                        rotors.ForEach(piston => rotateToValue(piston, piston.Angle + deltaValue));
                    }
                    else
                    {
                        rotors.ForEach(piston => rotateToValue(piston, numeric));
                    }
                }
                else if (isIncremental) //Handle Extension Changes With No Value
                {
                    if (incremental)
                    {
                        rotors.ForEach(piston => piston.TargetVelocityRPM = Math.Abs(piston.TargetVelocityRPM));
                    }
                    else
                    {
                        rotors.ForEach(piston => piston.TargetVelocityRPM = -Math.Abs(piston.TargetVelocityRPM));
                    }
                }
                else if (isActivation) //Handle Activations
                {
                    if (activation)
                    {
                        rotors.ForEach(piston => piston.ApplyAction("OnOff_On"));
                    }
                    else
                    {
                        rotors.ForEach(piston => piston.ApplyAction("OnOff_Off"));
                    }
                }

                return true;
            }

            //TODO: Directions may become important
            private void rotateToValue(IMyMotorStator rotor, float value)
            {
                float newValue = value;

                if (newValue > 360) newValue %= 360;

                if (newValue < -360)
                {
                    newValue = -((-newValue) % 360);
                }

                //TODO: We might find that in some cases, it's faster to go the other way.

                if (rotor.Angle < value) 
                {
                    rotor.UpperLimitDeg = newValue;
                    rotor.TargetVelocityRPM = Math.Abs(rotor.TargetVelocityRPM);
                }
                else
                {
                    rotor.LowerLimitDeg = newValue;
                    rotor.TargetVelocityRPM = -Math.Abs(rotor.TargetVelocityRPM);
                }
            }
        }

        public class LightCommand : EntityCommand<IMyLightingBlock>
        {
            public LightCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override bool Execute(MyGridProgram program)
            {
                program.Echo("Controlling Lights");
                return true;
            }
        }

        public class ProgramCommand : EntityCommand<IMyProgrammableBlock>
        {
            public ProgramCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override bool Execute(MyGridProgram program)
            {
                program.Echo("Controlling Program");
                return true;
            }
        }

        public class TimerBlockCommand : EntityCommand<IMyProgrammableBlock>
        {
            public TimerBlockCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override bool Execute(MyGridProgram program)
            {
                program.Echo("Controlling Timer");
                return true;
            }
        }

        public class MergeBlockCommand : EntityHandlerCommand<IMyShipMergeBlock>
        {
            public MergeBlockCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>
                {
                    new ActivationHandler<IMyShipMergeBlock>(entityProvider)
                };
            }
        }

        public class ProjectorCommand : EntityHandlerCommand<IMyProjector>
        {
            public ProjectorCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>
                {
                    new ActivationHandler<IMyProjector>(entityProvider)
                };
            }
        }

        public class ConnectorCommand : EntityHandlerCommand<IMyShipConnector>
        {
            public ConnectorCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>
                {
                    new ActivationHandler<IMyShipConnector>(entityProvider)
                };
            }
        }
    }
}
