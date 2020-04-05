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

        public abstract class EntityHandlerCommand<E> : HandlerCommand where E : class, IMyTerminalBlock
        {
            protected IEntityProvider<E> entityProvider;

            public EntityHandlerCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {
            }

            public override void PreParseCommands(List<CommandParameter> commandParameters)
            {
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

        public class BlockHandlerCommand<T> : EntityHandlerCommand<T> where T : class, IMyFunctionalBlock
        {
            private BlockHandler<T> blockHandler;

            public BlockHandlerCommand(MyGridProgram program, List<CommandParameter> commandParameters) : base(program, commandParameters)
            {

            }

            public override void PreParseCommands(List<CommandParameter> commandParameters)
            {
                int selectorIndex = commandParameters.FindIndex(param => param is SelectorCommandParameter);

                if (selectorIndex < 0) throw new Exception("SelectorCommandParameter is required for command: " + GetType());

                SelectorCommandParameter selectorParameter = (SelectorCommandParameter)commandParameters[selectorIndex];
                commandParameters.RemoveAt(selectorIndex);

                if (selectorParameter.isGroup)
                { 
                    entityProvider = new SelectorGroupEntityProvider<T>(selectorParameter);
                }
                else
                {
                    entityProvider = new SelectorEntityProvider<T>(selectorParameter);
                }

                blockHandler = BlockHandlerRegistry.GetBlockHandler<T>(selectorParameter.blockType);

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
                    new BooleanBlockPropertyCommandHandler<T>(entityProvider, blockHandler),

                    //String Handlers
                    new StringBlockPropertyCommandHandler<T>(entityProvider, blockHandler),

                    //Numeric Handlers
                    new SetNumericPropertyCommandHandler<T>(entityProvider, blockHandler),
                    new SetNumericDirectionPropertyCommandHandler<T>(entityProvider, blockHandler),
                    new IncrementNumericPropertyCommandHandler<T>(entityProvider, blockHandler),
                    new IncrementNumericDirectionPropertyCommandHandler<T>(entityProvider, blockHandler),
                    new MoveNumericDirectionPropertyCommandHandler<T>(entityProvider, blockHandler),
                    new ReverseBlockPropertyCommandHandler<T>(entityProvider, blockHandler),
                    new ReverseBlockCommandHandler<T>(entityProvider, blockHandler)

                    //TODO: GPS Handler?
                };
            }
        }

        public class RunAfterConditionCommand : Command
        {
            private Condition condition;
            private Command command;

            public RunAfterConditionCommand(Condition condition, Command command)
            {
                this.condition = condition;
                this.command = command;
            }

            //If Condition Not Met, run command.  
            public override bool Execute(MyGridProgram program)
            {
                if (!condition.evaluate())
                {
                    return false;
                }

                command.Execute(program);
                return true;
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
                bool conditionMet = condition.evaluate();
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
