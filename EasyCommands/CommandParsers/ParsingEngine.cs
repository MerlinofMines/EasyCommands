using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript {
    partial class Program {
        public List<ParsingTask> parsingTasks = NewList<ParsingTask>();
        public delegate bool ParsingTask();

        bool ParseCommands() {
            int parsedAmount = 0;
            if (String.IsNullOrWhiteSpace(PROGRAM.Me.CustomData)) {
                Info("Welcome to EasyCommands!");
                Info("Add Commands to Custom Data");
                return false;
            } else if (!PROGRAM.Me.CustomData.Equals(customData)) {
                int implicitMainOffset = 1;

                customData = PROGRAM.Me.CustomData;
                commandStrings = customData
                    .Trim()
                    .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                    .SkipWhile(line => {
                        var skip = string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#");
                        if (skip) implicitMainOffset++;
                        return skip;
                    })
                    .ToList();


                functions.Clear();
                parsingTasks.Clear();

                ClearAllState();

                if (!commandStrings[0].StartsWith(":")) {
                    commandStrings.Insert(0, ":main");
                    implicitMainOffset--;
                }

                var functionIndices = NewList<int>();
                for (int i = commandStrings.Count - 1; i >= 0; i--) {
                    if (commandStrings[i].StartsWith(":")) { functionIndices.Add(i); }
                }

                foreach (int i in functionIndices) {
                    String functionString = commandStrings[i].Remove(0, 1).Trim();
                    List<Token> nameAndParams = Tokenize(functionString);
                    String functionName = nameAndParams[0].original;
                    nameAndParams.RemoveAt(0);
                    FunctionDefinition definition = new FunctionDefinition(functionName, nameAndParams.Select(t => t.original).ToList());
                    functions[functionName] = definition;
                }

                foreach (int i in functionIndices) {
                    int startingLineNumber = i + 1 + implicitMainOffset;
                    String functionString = commandStrings[i].Remove(0, 1).Trim();
                    List<Token> nameAndParams = Tokenize(functionString);
                    String functionName = nameAndParams[0].original;

                    parsingTasks.Add(new ParseCommandLineTask(commandStrings.GetRange(i + 1, commandStrings.Count - (i + 1)).ToList(), startingLineNumber, commandLines => {
                        parsingTasks.Add(new ParseCommmandTask(commandLines, 0, true, command => {
                            if (!(command is MultiActionCommand)) command = new MultiActionCommand(NewList<Command>(command));
                            functions[functionName].function = (MultiActionCommand)command;
                            defaultFunction = functionName;
                        }).GetTask());
                    }).GetTask());
                    commandStrings.RemoveRange(i, commandStrings.Count - i);
                }
                parsedAmount++;
            }

            while (parsedAmount++ < commandParseAmount && parsingTasks.Count > 0 ) {
                if(parsingTasks[0]()) parsingTasks.RemoveAt(0);
            }

            if (parsingTasks.Count > 0) Info("Parsing Script...");

            return parsingTasks.Count == 0;
        }

        public class ParseCommandLineTask {
            int startingIndex;
            List<String> commandStrings;
            List<CommandLine> commandLines = NewList<CommandLine>();
            Action<List<CommandLine>> action;

            public ParseCommandLineTask(List<String> CommandStrings, int StartingIndex, Action<List<CommandLine>> Action) {
                startingIndex = StartingIndex;
                commandStrings = CommandStrings;
                action = Action;
            }

            public ParsingTask GetTask() => () => {
                int i = 0;
                while (i < PROGRAM.commandParameterParseAmount && commandStrings.Count > 0) {
                    CommandLine newLine = new CommandLine(commandStrings[0], startingIndex++);
                    commandStrings.RemoveAt(0);
                    i+=newLine.commandParameters.Count;
                    if(newLine.commandParameters.Count > 0) commandLines.Add(newLine);
                }
                if (commandStrings.Count == 0) {
                    action(commandLines);
                    return true;
                } else return false;
            };
        }

        public class ParseCommmandTask {
            List<Command> resolvedCommands = NewList<Command>();
            List<CommandLine> commandStrings = NewList<CommandLine>();
            int index;
            bool parseSiblings;
            Action<Command> action;

            public ParseCommmandTask(List<CommandLine> CommandStrings, int Index, bool ParseSiblings, Action<Command> Action) {
                commandStrings = CommandStrings;
                index = Index;
                parseSiblings = ParseSiblings;
                action = Action;
            }

            public ParsingTask GetTask() => () => {
                Command command;
                var parsed = ParseCommand(out command);
                if (parsed) action(command);
                return parsed;
            };

            bool ParseCommand(out Command command) {
                command = null;
                while (index < commandStrings.Count - 1) { //Parse Sibling & Child Commands, if any
                    CommandLine current = commandStrings[index];
                    CommandLine next = commandStrings[index + 1];
                    if (current.depth > next.depth) break; //End, break
                    if (current.depth < next.depth) { //I'm a parent of next line
                        PROGRAM.parsingTasks.Insert(0, new ParseCommmandTask(commandStrings, index + 1, true, myCommand => current.commandParameters.Add(new CommandReferenceParameter(myCommand))).GetTask());
                        return false;
                    }
                    if (next.commandParameters.Count > 0 && next.commandParameters[0] is ElseCommandParameter) {//Handle Otherwise
                        current.commandParameters.Add(next.commandParameters[0]);
                        next.commandParameters.RemoveAt(0);
                        PROGRAM.parsingTasks.Insert(0, new ParseCommmandTask(commandStrings, index + 1, false, myCommand => {
                            current.commandParameters.Add(new CommandReferenceParameter(myCommand));
                        }).GetTask());
                        return false;
                    }

                    if (!parseSiblings) break; //Only parsing myself
                    resolvedCommands.Add(PROGRAM.ParseCommand(current.commandParameters, current.lineNumber));
                    commandStrings.RemoveAt(index);
                    return false;
                }

                //Parse Last one, which has become current
                resolvedCommands.Add(PROGRAM.ParseCommand(commandStrings[index].commandParameters, commandStrings[index].lineNumber));
                commandStrings.RemoveAt(index);

                command = resolvedCommands.Count > 1 ? new MultiActionCommand(resolvedCommands) : resolvedCommands[0];
                return true;
            }
        }

        public Command ParseCommand(String commandLine, int lineNumber = 0) =>
            ParseCommand(ParseCommandParameters(Tokenize(commandLine)), lineNumber);

        Command ParseCommand(List<ICommandParameter> parameters, int lineNumber) {
            CommandReferenceParameter command = ParseParameters<CommandReferenceParameter>(parameters);

            if (command == null) throw new Exception("Unable to parse command from command parameters at line number: " + lineNumber);
            return command.value;
        }

        public class CommandLine {
            public int depth, lineNumber;
            public List<ICommandParameter> commandParameters;

            public CommandLine(String command, int line) {
                depth = command.TakeWhile(Char.IsWhiteSpace).Count();
                commandParameters = PROGRAM.ParseCommandParameters(PROGRAM.Tokenize(command));
                lineNumber = line;
            }
        }
    }
}
