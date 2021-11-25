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
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EasyCommands.Tests")]
namespace IngameScript {
    public partial class Program : MyGridProgram {
        //Debug
        #region mdk preserve
        public UpdateFrequency updateFrequency = UpdateFrequency.Update1;
        public LogLevel logLevel = LogLevel.INFO;
        public int functionParseAmount = 1;
        public int maxAsyncThreads = 50;
        public int maxQueuedThreads = 50;
        public int maxItemTransfers = 10;
        #endregion

        public delegate List<MyIGCMessage> BroadcastMessageProvider();

        public static Program PROGRAM;
        public BroadcastMessageProvider broadcastMessageProvider;
        public ProgramState state = ProgramState.STOPPED;
        public Dictionary<String, FunctionDefinition> functions = NewDictionary<string, FunctionDefinition>();
        public Thread currentThread;

        List<Thread> threadQueue = NewList<Thread>();
        List<Thread> asyncThreadQueue = NewList<Thread>();
        Dictionary<String, Variable> globalVariables = new Dictionary<string, Variable> {
            { "pi", GetStaticVariable(Math.PI) },
            { "e", GetStaticVariable(Math.E) },
            { "empty", GetStaticVariable(new KeyedList()) }
        };

        String defaultFunction;
        String customData = null;
        List<String> commandStrings = NewList<String>();

        public void ClearAllThreads() {
            asyncThreadQueue.Clear();
            threadQueue.Clear();
        }

        public Thread GetCurrentThread() => currentThread;

        public void QueueThread(Thread thread) {
            threadQueue.Add(thread);
            if (threadQueue.Count > maxQueuedThreads) throw new Exception("Stack Overflow Exception! Cannot have more than " + maxQueuedThreads + " queued commands");
        }

        public void QueueAsyncThread(Thread thread) {
            asyncThreadQueue.Add(thread);
            if (asyncThreadQueue.Count > maxAsyncThreads) throw new Exception("Stack Overflow Exception! Cannot have more than " + maxAsyncThreads + "concurrent async commands");
        }

        public void SetGlobalVariable(String variableName, Variable variable) {
            globalVariables[variableName] = variable;
        }

        public Variable GetVariable(String variableName) {
            Thread currentThread = GetCurrentThread();
            if(currentThread.threadVariables.ContainsKey(variableName)) {
                return currentThread.threadVariables[variableName];
            } else if (globalVariables.ContainsKey(variableName)) {
                return globalVariables[variableName];
            } else {
                throw new Exception("No Variable Exists for name: " + variableName);
            }
        }

        List<MyIGCMessage> provideMessages()
        {
            var listeners = NewList<IMyBroadcastListener>();
            IGC.GetBroadcastListeners(listeners);
            return listeners.Where(l => l.HasPendingMessage).Select(l => l.AcceptMessage()).ToList();
        }

        public Program() {
            PROGRAM = this;
            InitializeParsers();
            InitializeProcessors();
            InitializeOperators();
            InitializeItems();
            Runtime.UpdateFrequency = updateFrequency;
            broadcastMessageProvider = provideMessages;
        }

        static void Print(String str) { PROGRAM.Echo(str); }
        static void Info(String str) { if (PROGRAM.logLevel != LogLevel.SCRIPT_ONLY) PROGRAM.Echo(str); }
        static void Debug(String str) { if (PROGRAM.logLevel == LogLevel.DEBUG || PROGRAM.logLevel == LogLevel.TRACE) PROGRAM.Echo(str); }
        static void Trace(String str) { if (PROGRAM.logLevel == LogLevel.TRACE) PROGRAM.Echo(str); }

        public void Main(string argument) {
            try {
                if (!ParseCommands()) {
                    Runtime.UpdateFrequency = updateFrequency;
                    return;
                }
            } catch (Exception e) {
                Info("Exception Occurred During Parsing: ");
                Info(e.Message);
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }

            Debug("Functions: " + functions.Count);
            Debug("Argument: " + argument);

            List<MyIGCMessage> messages = broadcastMessageProvider();

            try {
                if (messages.Count > 0) {
                    List<Thread> messageCommands = messages.Select(message => new Thread(ParseCommand((String)message.Data), "Message", message.Tag)).ToList();
                    threadQueue.InsertRange(0, messageCommands);
                }
                if (!String.IsNullOrEmpty(argument)) { threadQueue.Insert(0, new Thread(ParseCommand(argument), "Request", argument)); }

                RunThreads();
                UpdateState();
            } catch(Exception e) {
                Info("Exception Occurred: ");
                Info(e.Message);
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }
        }

        void RunThreads() {
            try {
                //If no current commands, we've been asked to restart.  start at the top.
                if(threadQueue.Count == 0 && asyncThreadQueue.Count == 0) {
                    FunctionDefinition defaultFunctionDefinition = functions[defaultFunction];
                    threadQueue.Add(new Thread(defaultFunctionDefinition.function.Clone(), "Main", defaultFunctionDefinition.functionName));
                }

                Info("Queued Threads: " + threadQueue.Count());
                Info("Async Threads: " + asyncThreadQueue.Count());
                //Run first command in the queue.  Could be from a message, program run request, or request to start the main program.
                if (threadQueue.Count > 0 ) {
                    currentThread = threadQueue[0];
                    Info(currentThread.GetName());
                    if(currentThread.Command.Execute()) {
                        threadQueue.RemoveAt(0);
                    }
                }

                //Process 1 iteration of all async commands, removing from queue if processed.
                int asyncThreadQueueIndex = 0;

                while (asyncThreadQueueIndex < asyncThreadQueue.Count) {
                    currentThread = asyncThreadQueue[asyncThreadQueueIndex];
                    Info(currentThread.GetName());
                    if (currentThread.Command.Execute()) {
                        asyncThreadQueue.RemoveAt(asyncThreadQueueIndex);
                    } else {
                        asyncThreadQueueIndex++;
                    }
                }

                if(threadQueue.Count == 0 && asyncThreadQueue.Count == 0) {
                    state = ProgramState.COMPLETE;
                } else {
                    state = ProgramState.RUNNING;
                }
            //InterruptException is thrown by control commands to interrupt execution (stop, pause, restart).
            //The command itself has set the correct state of the command queues, we just need to set the program state.
            } catch(InterruptException interrupt) {
                Debug("Script interrupted!");
                state = interrupt.ProgramState;
            }
        }

        void UpdateState() {
            switch (state) {
                case ProgramState.RUNNING:
                    Runtime.UpdateFrequency = updateFrequency;
                    Info("Running");
                    break;
                case ProgramState.PAUSED:
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    Info("Paused");
                    break;
                case ProgramState.STOPPED:
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    Info("Stopped");
                    break;
                case ProgramState.COMPLETE:
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    Info("Complete");
                    break;
                default:
                    throw new Exception("Unknown Program State");
            }
        }

        bool ParseCommands() {
            if (String.IsNullOrWhiteSpace(PROGRAM.Me.CustomData)) {
                Info("Welcome to EasyCommands!");
                Info("Add Commands to Custom Data");
                return false;
            } else if (!PROGRAM.Me.CustomData.Equals(customData)) {
                customData = PROGRAM.Me.CustomData;
                commandStrings = customData.Trim().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                    .ToList();

                Info("Parsing Custom Data");
                functions.Clear();
                ClearAllThreads();
            }

            if (commandStrings.Count == 0) return true;
            Info("Parsing Commands.  Lines Left: " + commandStrings.Count);
            ParseFunctions(commandStrings);

            return commandStrings.Count == 0;
        }

        void ParseFunctions(List<String> commandStrings) {
            var functionIndices = NewList<int>();
            int implicitMainOffset = 1;
            if (!commandStrings[0].StartsWith(":")) {
                commandStrings.Insert(0, ":main");
                implicitMainOffset--;
            }

            for (int i = commandStrings.Count - 1; i >= 0; i--) {
                Trace("Command String: " + commandStrings[i]);
                if (commandStrings[i].StartsWith(":")) { functionIndices.Add(i); }
            }
            Trace("Function Indices: ");
            Trace(String.Join(" | ", functionIndices));

            //Parse Function Definitions
            foreach (int i in functionIndices) {
                String functionString = commandStrings[i].Remove(0, 1).Trim();
                List<Token> nameAndParams = ParseTokens(functionString);
                String functionName = nameAndParams[0].original;
                nameAndParams.RemoveAt(0);
                FunctionDefinition definition = new FunctionDefinition(functionName, nameAndParams.Select(t => t.original).ToList());
                functions[functionName] = definition;
            }

            //Parse Function Commands and add to Definitions
            int toParse = functionParseAmount;
            foreach (int i in functionIndices) {
                int startingLineNumber = i + 1 + implicitMainOffset;
                String functionString = commandStrings[i].Remove(0, 1).Trim();
                List<Token> nameAndParams = ParseTokens(functionString);
                String functionName = nameAndParams[0].original;
                Info("Parsing Function: " + functionName);
                Command command = ParseCommand(commandStrings
                    .GetRange(i + 1, commandStrings.Count - (i + 1))
                    .Select(str => new CommandLine(str, startingLineNumber++))
                    .Where(line => line.commandParameters.Count > 0)
                    .ToList(), 0, true);
                commandStrings.RemoveRange(i, commandStrings.Count - i);
                if (!(command is MultiActionCommand)) command = new MultiActionCommand(NewList<Command>(command));
                functions[functionName].function = (MultiActionCommand)command;
                defaultFunction = functionName;
                toParse--;
                if (toParse == 0) break;//Exceeded # of Functions to parse for 1 tick
            }
        }

        Command ParseCommand(List<CommandLine> commandStrings, int index, bool parseSiblings) {
            var resolvedCommands = NewList<Command>();
            while (index < commandStrings.Count - 1)//Parse Sibling & Child Commands, if any
            {
                CommandLine current = commandStrings[index];
                CommandLine next = commandStrings[index + 1];
                if (current.depth > next.depth) break;//End, break
                if (current.depth < next.depth) {//I'm a parent of next line
                    Trace("Parsing Sub Command @ Index: " + (index + 1));
                    current.commandParameters.Add(new CommandReferenceParameter(ParseCommand(commandStrings, index + 1, true)));
                    continue;
                }
                if (next.commandParameters.Count > 0 && next.commandParameters[0] is ElseCommandParameter) {//Handle Otherwise
                    Trace("Handling Otherwise @ index: " + index);
                    current.commandParameters.Add(next.commandParameters[0]);
                    next.commandParameters.RemoveAt(0);
                    current.commandParameters.Add(new CommandReferenceParameter(ParseCommand(commandStrings, index + 1, false)));
                    continue;
                }

                if (!parseSiblings) break;//Only parsing myself
                Trace("Parsing Sibling Command @ Index: " + index);
                resolvedCommands.Add(ParseCommand(current.commandParameters, current.lineNumber));
                commandStrings.RemoveAt(index);
            }

            //Parse Last one, which has become current
            Trace("Parsing Final Command @ Index: " + index);
            resolvedCommands.Add(ParseCommand(commandStrings[index].commandParameters, commandStrings[index].lineNumber));
            commandStrings.RemoveAt(index);

            return resolvedCommands.Count > 1 ? new MultiActionCommand(resolvedCommands) : resolvedCommands[0];
        }

        public Command ParseCommand(String commandLine, int lineNumber = 0) =>
            ParseCommand(ParseCommandParameters(ParseTokens(commandLine)), lineNumber);

        Command ParseCommand(List<CommandParameter> parameters, int lineNumber) {
            Trace("Parsing Command at line: " + lineNumber);
            Trace("Pre Processed Parameters:");
            parameters.ForEach(param => Trace("Type: " + param.GetType()));

            CommandReferenceParameter command = ParseParameters<CommandReferenceParameter>(parameters);

            if (command == null) throw new Exception("Unable to parse command from command parameters at line number: " + lineNumber);
            return command.value;
        }
        class CommandLine {
            public int depth, lineNumber;
            public List<CommandParameter> commandParameters;

            public CommandLine(String command, int line) {
                depth = command.TakeWhile(Char.IsWhiteSpace).Count();
                commandParameters = PROGRAM.ParseCommandParameters(PROGRAM.ParseTokens(command));
                lineNumber = line;
            }
        }

        public class Thread {
            public Command Command { get; set; }
            public Dictionary<String, Variable> threadVariables = NewDictionary<string, Variable>();
            String prefix;
            String name;

            public Thread(Command command, string p, string n) {
                Command = command;
                prefix = p;
                name = n;
            }

            public String GetName() => "[" + prefix + "] " + name;
            public void SetName(String s) => name = s;
        }

        public class FunctionDefinition {
            public String functionName;
            public MultiActionCommand function = null;
            public List<String> parameterNames;

            public FunctionDefinition(string function, List<string> parameters) {
                functionName = function;
                parameterNames = parameters;
            }
        }
    }
}
