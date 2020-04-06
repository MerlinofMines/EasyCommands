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
        public static class CommandParserRegistry
        {
            private static List<CommandParser> CommandParsers = new List<CommandParser>()
            {
                new AsyncCommandParser(),
                new ConditionalCommandParser(),
                new ActionCommandParser(),
            };

            public static void RegisterParser(CommandParser commandParser) { CommandParsers.Add(commandParser); }

            public static Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                program.Echo("Parsing Command");

                CommandParser parser = CommandParsers.Find(p => p.CanHandle(commandParameters));
                if (parser == null) throw new Exception("Unsupported Command");
                return parser.ParseCommand(program, commandParameters);
            }
        }

        public interface CommandParser
        {
            bool CanHandle(List<CommandParameter> commandParameters);
            Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters);
        }

        public class AsyncCommandParser : CommandParser
        {
            public bool CanHandle(List<CommandParameter> commandParameters)
            {
                return commandParameters[0] is AsyncCommandParameter;
            }

            public Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                commandParameters.RemoveAt(0);
                Command subCommand = CommandParserRegistry.ParseCommand(program, commandParameters);
                subCommand.SetAsync(true);
                return subCommand;
            }
        }

        public class ConditionalCommandParser : CommandParser
        {
            public bool CanHandle(List<CommandParameter> commandParameters)
            {
                return commandParameters.Exists(param => param is IfCommandParameter);
            }

            public Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                int ifIndex = commandParameters.FindIndex(param => param is IfCommandParameter);
                if (!(commandParameters[ifIndex+1] is ConditionCommandParameter)) throw new Exception("Ifs Must Be Followed By A Condition");
                IfCommandParameter ifParameter = (IfCommandParameter)commandParameters[ifIndex];
                ConditionCommandParameter conditionParameter = (ConditionCommandParameter)commandParameters[ifIndex + 1];
                commandParameters.RemoveRange(ifIndex, 2);
                Command actionCommand = CommandParserRegistry.ParseCommand(program, commandParameters);
                Condition condition = conditionParameter.GetValue();

                //Handle Otherwise
                Command otherwiseCommand = new NullCommand();
                int otherwiseIndex = commandParameters.FindIndex(param => param is ElseCommandParameter);
                if (otherwiseIndex>=0)
                {
                    commandParameters.RemoveAt(otherwiseIndex);
                    otherwiseCommand = CommandParserRegistry.ParseCommand(program, commandParameters);
                }

                if (ifParameter.swapCommands)
                {
                    Command temp = actionCommand;
                    actionCommand = otherwiseCommand;
                    otherwiseCommand = temp;
                }

                if (ifParameter.inverseCondition)
                {
                    condition = new NotCondition(condition);
                }

                return new ConditionalCommand(condition, actionCommand, otherwiseCommand, ifParameter.alwaysEvaluate);
            }
        }

        public class ActionCommandParser : CommandParser
        {
            private List<CommandParser> actionCommandParsers = new List<CommandParser>()
            {
                new WaitCommandParser(),
                new BlockHandlerCommandParser()
            };

            public bool CanHandle(List<CommandParameter> commandParameters)
            {
                return commandParameters.Exists(param => param is ActionCommandParameter);
            }

            public Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                int index = commandParameters.FindIndex(param => param is ActionCommandParameter);
                ActionCommandParameter action = (ActionCommandParameter)commandParameters[index];
                commandParameters.RemoveAt(index);

                foreach (CommandParser parser in actionCommandParsers)
                {
                    if (parser.CanHandle(action.GetValue()))
                    {
                        return parser.ParseCommand(program, action.GetValue());
                    }
                }

                throw new Exception("Unknown Action Type. ");
            }
        }

        public class WaitCommandParser : CommandParser
        {
            public bool CanHandle(List<CommandParameter> commandParameters)
            {
                return commandParameters.Exists(param => param is WaitCommandParameter);
            }

            public Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                return new WaitCommand(program, commandParameters);
            }
        }

        public class BlockHandlerCommandParser : CommandParser
        {
            public bool CanHandle(List<CommandParameter> commandParameters)
            {
                return commandParameters.FindIndex(param => param is SelectorCommandParameter) >= 0;
            }

            public Command ParseCommand(MyGridProgram program, List<CommandParameter> parameters)
            {
                return new BlockHandlerCommand(program, parameters);
            }
        }
    }
}
