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
                commandParameters.ForEach(param => program.Echo("" + param.GetType()));

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
                Boolean isGroup = (commandParameters.RemoveAll(param => param is GroupCommandParameter) > 0);//Remove all group, set true if at least 1 removed

                //TODO: Smarter resolution of which Selector, if more than 1.
                int selectorIndex = commandParameters.FindIndex(param => param is StringCommandParameter);

                if (selectorIndex < 0) throw new Exception("SelectorCommandParameter is required for command: " + GetType());

                StringCommandParameter selector = (StringCommandParameter)commandParameters[selectorIndex];
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

        public abstract class EntityCommand<E> : Command where E : class, IMyTerminalBlock
        {
            public EntityCommand(List<CommandParameter> commandParameters) : base(commandParameters)
            {
            }

            protected List<E> GetSelectorEntities(MyGridProgram program)
            {
                bool isGroup = commandParameters.Exists(parameter => (parameter is GroupCommandParameter));
                //TODO: Need Smart Selection by finding the closest Selector to either a Group or a BlockType Parameter.  Other param may be something else.
                StringCommandParameter selectorParameter = (StringCommandParameter)commandParameters.Find(parameter => (parameter is StringCommandParameter));

                if (isGroup)
                {
                    return GetGroupEntities(program, selectorParameter);
                } else
                {
                    return GetEntities(program, selectorParameter);
                }
            }

            protected List<E> GetEntities(MyGridProgram program, StringCommandParameter selector)
            {
                List<E> entities = new List<E>();
                program.GridTerminalSystem.GetBlocksOfType<E>(entities);

                return entities.FindAll(entity => (entity is IMyTerminalBlock)
                    && ((IMyTerminalBlock)entity).CustomName.ToLower() == selector.GetValue());
            }

            protected List<E> GetGroupEntities(MyGridProgram program, StringCommandParameter selector)
            {
                IMyBlockGroup group = program.GridTerminalSystem.GetBlockGroupWithName(selector.GetValue());

                if (group == null)
                {
                    program.Echo("Unable to find requested block group: " + selector.GetValue());
                    throw new Exception();
                }

                List<E> entities = new List<E>();
                group.GetBlocksOfType<E>(entities);
                return entities;
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

        public class BlockHandlerCommand<T> : EntityHandlerCommand<T> where T : class, IMyFunctionalBlock
        {
            private BooleanBlockHandler<T> booleanBlockHandler;
            private StringBlockHandler<T> stringBlockHandler;
            private NumericBlockHandler<T> numericBlockHandler;

            public BlockHandlerCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {

            }

            public override void PreParseCommands(List<CommandParameter> commandParameters)
            {
                base.PreParseCommands(commandParameters);

                int blockTypeIndex = commandParameters.FindIndex(param => param is BlockTypeCommandParameter);

                if (blockTypeIndex < 0) throw new Exception("BlockTypeCommandParameter is required for command: " + GetType());

                BlockTypeCommandParameter blockTypeParameter = (BlockTypeCommandParameter) commandParameters[blockTypeIndex];
                commandParameters.RemoveAt(blockTypeIndex);

                booleanBlockHandler = BlockHandlerRegistry.GetBooleanBlockHandler<T>(blockTypeParameter.GetBlockType());
                stringBlockHandler = BlockHandlerRegistry.GetStringBlockHandler<T>(blockTypeParameter.GetBlockType());
                numericBlockHandler = BlockHandlerRegistry.GetNumericBlockHandler<T>(blockTypeParameter.GetBlockType());

                //TODO: Move to proper command parameter pre-processor
                int boolPropIndex = commandParameters.FindIndex(param => (param is BooleanPropertyCommandParameter));
                int stringPropIndex = commandParameters.FindIndex(param => (param is BooleanPropertyCommandParameter));
                int numericPropIndex = commandParameters.FindIndex(param => (param is NumericPropertyCommandParameter));
                int boolIndex = commandParameters.FindIndex(param => (param is BooleanCommandParameter));
                int stringIndex = commandParameters.FindIndex(param => (param is StringCommandParameter));
                int numericIndex = commandParameters.FindIndex(param => (param is NumericCommandParameter));
                int directionIndex = commandParameters.FindIndex(param => (param is DirectionCommandParameter));

                if (boolPropIndex < 0 && boolIndex >= 0)
                {
                    commandParameters.Add(new BooleanPropertyCommandParameter(booleanBlockHandler.GetDefaultBooleanProperty()));
                }

                if (boolIndex < 0 && boolPropIndex >= 0)
                {
                    commandParameters.Add(new BooleanCommandParameter(true));
                }

                if (stringPropIndex < 0 && stringIndex >= 0)
                {
                    commandParameters.Add(new StringPropertyCommandParameter(stringBlockHandler.GetDefaultStringProperty()));
                }

                if (numericIndex >= 0)
                {
                    DirectionType direction;
                    if (directionIndex >= 0)
                    {
                        direction = ((DirectionCommandParameter)commandParameters[directionIndex]).GetValue();
                    } else
                    {
                        direction = numericBlockHandler.GetDefaultDirection();
                        commandParameters.Add(new DirectionCommandParameter(direction));
                    }

                    if (numericPropIndex < 0)
                    {
                        commandParameters.Add(new NumericPropertyCommandParameter(numericBlockHandler.GetDefaultNumericProperty(direction)));
                    }
                }
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>() {
                    //Boolean Handlers
                    new BooleanBlockPropertyCommandHandler<T>(entityProvider, booleanBlockHandler),

                    //String Handlers
                    new StringBlockPropertyCommandHandler<T>(entityProvider, stringBlockHandler),

                    //Numeric Handlers
                    new SetNumericPropertyCommandHandler<T>(entityProvider, numericBlockHandler),
                    new SetNumericDirectionPropertyCommandHandler<T>(entityProvider, numericBlockHandler),
                    new IncrementNumericPropertyCommandHandler<T>(entityProvider, numericBlockHandler),
                    new IncrementNumericDirectionPropertyCommandHandler<T>(entityProvider, numericBlockHandler),
                    new MoveNumericDirectionPropertyCommandHandler<T>(entityProvider, numericBlockHandler),
                    new ReverseBlockPropertyCommandHandler<T>(entityProvider, numericBlockHandler),
                    new ReverseBlockCommandHandler<T>(entityProvider, numericBlockHandler)

                    //TODO: GPS Handler?
                };
            }
        }
    }
}
