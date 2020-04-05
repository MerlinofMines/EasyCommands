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

            //Returns true if the program has finished execution.
            public abstract bool Execute(MyGridProgram program);
        }

        public abstract class HandlerCommand : Command
        {
            private CommandHandler commandHandler;

            public HandlerCommand(MyGridProgram program, List<CommandParameter> commandParameters)
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

        public class WaitCommand : HandlerCommand
        {
            public WaitCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {
            }

            public override void PreParseCommands(List<CommandParameter> commandParameters)
            {
                commandParameters.RemoveAll(param => param is WaitCommandParameter);
                bool unitExists = commandParameters.Exists(param => param is UnitCommandParameter);
                bool timeExists = commandParameters.Exists(param => param is NumericCommandParameter);

                if (!timeExists && !unitExists) {commandParameters.Add(new NumericCommandParameter(1f)); commandParameters.Add(new UnitCommandParameter(UnitType.TICKS));}
            }
            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>
                {
                    new WaitForDurationHandler(),
                    new WaitForDurationUnitHandler()
                };
            }
        }

        public class NullCommand : Command
        {
            public override bool Execute(MyGridProgram program)
            {
                program.Echo("Null Program");
                return true;
            }
        }

        public class BlockHandlerCommand : HandlerCommand
        {
            private BlockHandler blockHandler;
            private IEntityProvider entityProvider;

            public BlockHandlerCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
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
                int boolPropIndex = commandParameters.FindIndex(param => (param is BooleanPropertyCommandParameter));
                int stringPropIndex = commandParameters.FindIndex(param => (param is BooleanPropertyCommandParameter));
                int numericPropIndex = commandParameters.FindIndex(param => (param is NumericPropertyCommandParameter));
                int boolIndex = commandParameters.FindIndex(param => (param is BooleanCommandParameter));
                int stringIndex = commandParameters.FindIndex(param => (param is StringCommandParameter));
                int numericIndex = commandParameters.FindIndex(param => (param is NumericCommandParameter));
                int directionIndex = commandParameters.FindIndex(param => (param is DirectionCommandParameter));

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

                if (numericIndex >= 0)
                {
                    DirectionType direction;
                    if (directionIndex >= 0)
                    {
                        direction = ((DirectionCommandParameter)commandParameters[directionIndex]).GetValue();
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
            }

            public override List<CommandHandler> GetHandlers()
            {
                return new List<CommandHandler>() {
                    //Boolean Handlers
                    new BooleanBlockPropertyCommandHandler(entityProvider, blockHandler),

                    //String Handlers
                    new StringBlockPropertyCommandHandler(entityProvider, blockHandler),

                    //Numeric Handlers
                    new SetNumericPropertyCommandHandler(entityProvider, blockHandler),
                    new SetNumericDirectionPropertyCommandHandler(entityProvider, blockHandler),
                    new IncrementNumericPropertyCommandHandler(entityProvider, blockHandler),
                    new IncrementNumericDirectionPropertyCommandHandler(entityProvider, blockHandler),
                    new MoveNumericDirectionPropertyCommandHandler(entityProvider, blockHandler),
                    new ReverseBlockPropertyCommandHandler(entityProvider, blockHandler),
                    new ReverseBlockCommandHandler(entityProvider, blockHandler)

                    //TODO: GPS Handler?
                };
            }
        }

        public class ConditionalCommand : Command
        {
            private Condition condition;
            private Command conditionMetCommand;
            private Command conditionNotMetCommand;

            public ConditionalCommand(Condition condition, Command conditionMetCommand, Command conditionNotMetCommand)
            {
                this.condition = condition;
                this.conditionMetCommand = conditionMetCommand;
                this.conditionNotMetCommand = conditionNotMetCommand;
            }

            public override bool Execute(MyGridProgram program)
            {
                bool conditionMet = condition.evaluate(program);
                bool commandResult = false;
                if (conditionMet)
                {
                    commandResult = conditionMetCommand.Execute(program);
                } else
                {
                    commandResult = conditionNotMetCommand.Execute(program);
                }

                if (async) { return !conditionMet; } else { return commandResult; }
            }
        }
    }
}
