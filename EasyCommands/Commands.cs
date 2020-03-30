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
        }

        public abstract class HandlerCommand : Command
        {
            private CommandHandler commandHandler;

            public HandlerCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(commandParameters)
            {
                PreParseCommands(commandParameters);

                program.Echo("Post Parsed Command Parameters: ");
                commandParameters.ForEach(param => program.Echo(""+param.GetType()));

                foreach (CommandHandler handler in GetHandlers())
                {
                    if (handler.CanHandle(commandParameters)) {
                        commandHandler = handler;
                        return;
                    }
                }

                throw new Exception("Unsupported Command Parameter Combination");
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

            public EntityHandlerCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
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
                    && ((IMyTerminalBlock)entity).CustomName.ToLower() == selector.GetSelector());
            }

            protected List<E> GetGroupEntities(MyGridProgram program, SelectorCommandParameter selector)
            {
                IMyBlockGroup group = program.GridTerminalSystem.GetBlockGroupWithName(selector.GetSelector());

                if (group == null)
                {
                    program.Echo("Unable to find requested block group: " + selector.GetSelector());
                    throw new Exception();
                }

                List<E> entities = new List<E>();
                group.GetBlocksOfType<E>(entities);
                return entities;
            }

            protected void handleActivation(MyGridProgram program, List<E> functionalBlocks, ActivationCommandParameter activationParameter)
            {
                bool activation = activationParameter.IsActivate();

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

        public class WaitCommand : HandlerCommand
        {
            public WaitCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {
            }

            public override void PreParseCommands(List<CommandParameter> commandParameters)
            {
                commandParameters.RemoveAll(param => param is WaitCommandParameter);
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>
                {
                    new WaitForDurationHandler(),
                    new WaitForDurationUnitHandler()
                    //TODO: Add Conditional Wait Handlers
                };
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

        public class PistonCommand : EntityHandlerCommand<IMyPistonBase>
        {
            public PistonCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {

            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>()
                {
                    new PistonRelativeVelocityHandler(entityProvider),
                    new PistonIncrementalAbsoluteVelocityHandler(entityProvider),
                    new PistonAbsoluteVelocityHandler(entityProvider),
                    new PistonRelativePositionHandler(entityProvider),
                    new PistonIncrementalAbsolutePositionHandler(entityProvider),
                    new PistonAbsolutePositionHandler(entityProvider),
                    new PistonIncrementalPositionHandler(entityProvider),
                    new ReverseHandler<IMyPistonBase>(entityProvider),
                    new ActivationHandler<IMyPistonBase>(entityProvider)
                };
            }
         }

        public class RotorCommand : EntityHandlerCommand<IMyMotorStator>
        {
            public RotorCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>()
                {
                    new RotorRelativeVelocityHandler(entityProvider),
                    new RotorIncrementalAbsoluteVelocityHandler(entityProvider),
                    new RotorAbsoluteVelocityHandler(entityProvider),
                    new RotorRelativePositionHandler(entityProvider),
                    new RotorIncrementalAbsolutePositionHandler(entityProvider),
                    new RotorAbsolutePositionHandler(entityProvider),
                    new RotorIncrementalPositionHandler(entityProvider),
                    new ReverseHandler<IMyMotorStator>(entityProvider),
                    new ActivationHandler<IMyMotorStator>(entityProvider)
                };
            }
        }

        public class LightCommand : EntityHandlerCommand<IMyLightingBlock>
        {
            public LightCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>()
                {
                    new ActivationHandler<IMyLightingBlock>(entityProvider)
                };
            }
        }

        public class ProgramCommand : EntityHandlerCommand<IMyProgrammableBlock>
        {
            public ProgramCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {
            }

            public override List<CommandHandler> GetHandlers()
            {
                //TODO: Add More Handlers
                return new List<CommandHandler>()
                {
                    new ActivationHandler<IMyProgrammableBlock>(entityProvider)
                };
            }
        }

        public class TimerBlockCommand : EntityHandlerCommand<IMyTimerBlock>
        {
            public TimerBlockCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {
            }

            public override List<CommandHandler> GetHandlers()
            {
                //TODO: Add More Handlers
                return new List<CommandHandler>()
                {
                    new ActivationHandler<IMyTimerBlock>(entityProvider)
                };
            }
        }

        public class MergeBlockCommand : EntityHandlerCommand<IMyShipMergeBlock>
        {
            public MergeBlockCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
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
            public ProjectorCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
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
            public ConnectorCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>
                {
                    new ConnectorLockHandler(entityProvider),
                    new ConnectorConnectHandler(entityProvider),
                    new ActivationHandler<IMyShipConnector>(entityProvider)
                };
            }
        }
    }
}
