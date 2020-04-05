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
//                new AsyncCommandParser(),
//                new ConditionalComandParser(),
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

        public class ConditionalComandParser : CommandParser
        {
            public bool CanHandle(List<CommandParameter> commandParameters)
            {
                return commandParameters.Exists(param => param is ConditionalComandParser);
            }

            public Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                int conditionalIndex = commandParameters.FindIndex(param => param is ConditionCommandParameter);
                int otherwiseIndex = commandParameters.FindIndex(param => param is ElseCommandParameter);

                Command otherwiseCommand = new NullCommand();
                Command ifCommand = new NullCommand();

                //Base case, no other otherwise. Return simple command
                if (otherwiseIndex<0)
                {
                    return ParseConditionalCommand(program, commandParameters, new NullCommand());
                }

                //Convert first otherwise to first otherwiseCommand
                commandParameters.RemoveAt(otherwiseIndex);
                otherwiseCommand = CommandParserRegistry.ParseCommand(program, commandParameters.GetRange(otherwiseIndex, commandParameters.Count - otherwiseIndex));
                commandParameters.RemoveRange(otherwiseIndex, commandParameters.Count - otherwiseIndex);

                if (otherwiseIndex<conditionalIndex)
                {
                    throw new Exception("Otherwise should never be before a conditional");
                }

                if (otherwiseIndex>conditionalIndex)
                {
                } else
                {

                }

                return null;
            }

            //May have more than 1 if clause, but no otherwise clauses
            private Command ParseConditionalCommand(MyGridProgram program, List<CommandParameter> commandParameters, Command otherwiseCommand)
            {
                int lastConditionalIndex = commandParameters.FindLastIndex(param => param is ConditionCommandParameter);

                return null;
            }

            //Assumes no ifs or elses remain
            private Command ParseSimpleConditionalCommand(MyGridProgram program, List<CommandParameter> commandParameters, Command otherwiseCommand)
            {
                List<CommandParameter> trueCommandParameters = new List<CommandParameter>();
//                Condition condition = new .parseCondition(commandParameters, out trueCommandParameters);
                Command actionCommand = CommandParserRegistry.ParseCommand(program, trueCommandParameters);

                //                return new ConditionalCommand(condition, actionCommand, otherwiseCommand);
                return null;
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
                    if(parser.CanHandle(action.GetValue()))
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
                CommandParameter selectorParameter = parameters.Find(param => param is SelectorCommandParameter);

                BlockType blockType = ((SelectorCommandParameter)selectorParameter).blockType;

                switch (blockType)
                {
                    case BlockType.PISTON:
                        return new BlockHandlerCommand<IMyPistonBase>(program, parameters);
                    case BlockType.ROTOR:
                        return new BlockHandlerCommand<IMyMotorStator>(program, parameters);
                    case BlockType.LIGHT:
                        return new BlockHandlerCommand<IMyLightingBlock>(program, parameters);
                    case BlockType.PROGRAM:
                        return new BlockHandlerCommand<IMyProgrammableBlock>(program, parameters);
                    case BlockType.TIMER:
                        return new BlockHandlerCommand<IMyTimerBlock>(program, parameters);
                    case BlockType.PROJECTOR:
                        return new BlockHandlerCommand<IMyProjector>(program, parameters);
                    case BlockType.MERGE:
                        return new BlockHandlerCommand<IMyShipMergeBlock>(program, parameters);
                    case BlockType.CONNECTOR:
                        return new BlockHandlerCommand<IMyShipConnector>(program, parameters);
                    case BlockType.WELDER:
                        return new BlockHandlerCommand<IMyShipWelder>(program, parameters);
                    case BlockType.GRINDER:
                        return new BlockHandlerCommand<IMyShipGrinder>(program, parameters);
                    default:
                        throw new Exception("Unsupported Block Type Command: " + blockType);
                }
            }
        }
    }
}
