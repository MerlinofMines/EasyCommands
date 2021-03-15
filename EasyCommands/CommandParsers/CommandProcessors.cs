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
        public static class CommandParserRegistry {
            private static List<CommandProcessor> CommandParsers = new List<CommandProcessor>() {
                new AsyncCommandProcessor(),
                new IterationCommandProcessor(),
                new ConditionalCommandProcessor(),
            };

            public static Command ParseCommand(List<CommandParameter> commandParameters) {
                Debug("Parsing Command");

                bool processing = true;
                while (processing) {
                    //Keep Processing through until all return false
                    processing = CommandParsers.Exists(p => { Debug("Executing " + p.GetType()); bool processed = p.ProcessCommand(commandParameters); Debug("Result: " + processed); return processed; });
                }

                if (commandParameters.Count != 1 || !(commandParameters[0] is CommandReferenceParameter)) throw new Exception("Unable to parse command from command parameters!");
                return ((CommandReferenceParameter)commandParameters[0]).Value;
            }
        }

        public interface CommandProcessor {
            bool ProcessCommand(List<CommandParameter> commandParameters);
        }

        public class AsyncCommandProcessor : CommandProcessor {
            public bool ProcessCommand(List<CommandParameter> commandParameters) {
                bool processed = false;
                for (int i = 0; i < commandParameters.Count - 1; i++) {
                    if (!(commandParameters[i] is AsyncCommandParameter && commandParameters[i + 1] is CommandReferenceParameter)) continue;
                    Command subCommand = ((CommandReferenceParameter)commandParameters[i + 1]).Value;
                    subCommand.Async = true;
                    commandParameters.RemoveRange(i, 2);
                    commandParameters.Insert(i, new CommandReferenceParameter(subCommand));
                    processed = true;
                }
                return processed;
            }
        }

        public class IterationCommandProcessor : CommandProcessor {
            public bool ProcessCommand(List<CommandParameter> commandParameters) {
                bool processed = false;
                for (int i = 0; i < commandParameters.Count - 1; i++) {
                    //First, swap statements (command) (iteration) -> (iteration) (command)
                    if (commandParameters[i + 1] is IterationCommandParameter && commandParameters[i] is CommandReferenceParameter) {
                        CommandParameter temp = commandParameters[i];
                        commandParameters.RemoveAt(i);
                        commandParameters.Insert(i + 1, temp);
                    }
                    if (commandParameters[i] is IterationCommandParameter && commandParameters[i + 1] is CommandReferenceParameter) {
                        MultiActionCommand command = new MultiActionCommand(new List<Command>() { ((CommandReferenceParameter)commandParameters[i + 1]).Value }, ((IterationCommandParameter)commandParameters[i]).Value);
                        commandParameters.RemoveRange(i, 2);
                        commandParameters.Insert(i, new CommandReferenceParameter(command));
                        processed = true;
                    }
                }
                return processed;
            }
        }

        public class ConditionalCommandProcessor : CommandProcessor {
            public bool ProcessCommand(List<CommandParameter> commandParameters) {
                bool processed = false;

                Debug("Pre Processed Parameters:");
                commandParameters.ForEach(param => Debug("Type: " + param.GetType()));

                for (int i = 0; i < commandParameters.Count - 2; i++) {
                    //check for if + condition.  Then look for action (before if or after condition).  If present, keep going
                    //check for otherwise.  if present, if otherwise is followed by command, then process.
                    //if otherwise is not followed by command then do not process (apparently we need to resolve the next thing first, e.g. "otherwise if (condition) (command)
                    //if otherwise is not present then process.  
                    //command if condition and => process command if condition
                    //if condition command and => don't process

                    //First, swap silly misordered statements (command) if (condition)
                    if (commandParameters[i + 1] is IfCommandParameter && commandParameters[i + 2] is ConditionCommandParameter && commandParameters[i] is CommandReferenceParameter) {
                        CommandParameter temp = commandParameters[i];
                        commandParameters.RemoveAt(i);
                        commandParameters.Insert(i + 2, temp);
                    }

                    if (!(commandParameters[i] is IfCommandParameter)) continue;
                    if (!(commandParameters[i + 1] is ConditionCommandParameter)) throw new Exception("Ifs Must Be Followed By A Conditional Variable");
                    if (!(commandParameters[i + 2] is CommandReferenceParameter)) continue; //Not yet ready to parse this one, command not resolved
                    if (i < commandParameters.Count - 3 && commandParameters[i + 3] is AndCommandParameter) continue;  //We want to run more than one command if there is an "and"

                    //Valid condition, sans Else
                    Command otherwiseCommand = new NullCommand();
                    if (i + 3 < commandParameters.Count && commandParameters[i + 3] is ElseCommandParameter) { //Command Contains an Else Block!
                        if (!(commandParameters[i + 4] is CommandReferenceParameter)) continue; //Not ready to parse Otherwise yet!

                        if (i + 5 < commandParameters.Count && commandParameters[i + 5] is AndCommandParameter) continue; //Not ready to parse Otherwise yet, there are more commands to parse!
                        otherwiseCommand = ((CommandReferenceParameter)commandParameters[i + 4]).Value;
                        commandParameters.RemoveRange(i + 3, 2); //Remove Otherwise and Otherwise Command now that we have stored
                    }

                    //Time to Parse!
                    IfCommandParameter ifParameter = (IfCommandParameter)commandParameters[i];
                    Variable condition = ((ConditionCommandParameter)commandParameters[i + 1]).Value;
                    Command actionCommand = ((CommandReferenceParameter)commandParameters[i + 2]).Value;

                    if (ifParameter.swapCommands) {
                        Command temp = actionCommand;
                        actionCommand = otherwiseCommand;
                        otherwiseCommand = temp;
                    }

                    if (ifParameter.inverseCondition) {
                        condition = new NotVariable(condition);
                    }

                    commandParameters.RemoveRange(i, 3);//Remove If, Condition and Command.  Otherwise was already removed, if present.
                    commandParameters.Insert(i, new CommandReferenceParameter(new ConditionalCommand(condition, actionCommand, otherwiseCommand, ifParameter.alwaysEvaluate)));
                    processed = true;
                    i--;//Re-process this one in case this is now (command) if (condition)
                }
                return processed;
            }
        }
    }
}
