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
        public Dictionary<String, FunctionDefinition> functions = new Dictionary<string, FunctionDefinition>();
        public Thread currentThread;

        List<Thread> threadQueue = new List<Thread>();
        List<Thread> asyncThreadQueue = new List<Thread>();
        Dictionary<String, Variable> globalVariables = new Dictionary<string, Variable> {
            { "pi", GetStaticVariable(Math.PI) },
            { "e", GetStaticVariable(Math.E) },
        };

        String defaultFunction;
        String customData = null;
        List<String> commandStrings = new List<String>();

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
            List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
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
            List<int> functionIndices = new List<int>();
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
                Command command = ParseCommand(commandStrings.GetRange(i + 1, commandStrings.Count - (i + 1)).Select(str => new CommandLine(str, this)).ToList(), 0, true, ref startingLineNumber);
                commandStrings.RemoveRange(i, commandStrings.Count - i);
                if (!(command is MultiActionCommand)) { command = new MultiActionCommand(new List<Command> { command }); }
                functions[functionName].function = (MultiActionCommand)command;
                defaultFunction = functionName;
                toParse--;
                if (toParse == 0) break;//Exceeded # of Functions to parse for 1 tick
            }
        }

        Command ParseCommand(List<CommandLine> commandStrings, int index, bool parseSiblings, ref int startingLineNumber) {
            List<Command> resolvedCommands = new List<Command>();
            while (index < commandStrings.Count - 1)//Parse Sibling & Child Commands, if any
            {
                CommandLine current = commandStrings[index];
                CommandLine next = commandStrings[index + 1];
                if (current.Depth > next.Depth) break;//End, break
                if (current.Depth < next.Depth) {//I'm a parent of next line
                    Trace("Parsing Sub Command @ Index: " + (index + 1));
                    startingLineNumber++;
                    current.CommandParameters.Add(new CommandReferenceParameter(ParseCommand(commandStrings, index + 1, true, ref startingLineNumber)));
                    continue;
                }
                if (next.CommandParameters.Count > 0 && next.CommandParameters[0] is ElseCommandParameter) {//Handle Otherwise
                    Trace("Handling Otherwise @ index: " + index);
                    current.CommandParameters.Add(next.CommandParameters[0]);
                    next.CommandParameters.RemoveAt(0);
                    startingLineNumber++;
                    current.CommandParameters.Add(new CommandReferenceParameter(ParseCommand(commandStrings, index + 1, false, ref startingLineNumber)));
                    continue;
                }

                if (!parseSiblings) break;//Only parsing myself
                if (!current.ShouldIgnore()) {
                    Trace("Parsing Sibling Command @ Index: " + index);
                    resolvedCommands.Add(ParseCommand(current.CommandParameters, startingLineNumber));
                }
                commandStrings.RemoveAt(index);
                startingLineNumber++;
            }

            //Parse Last one, which has become current
            if(!commandStrings[index].ShouldIgnore()) {
                Trace("Parsing Final Command @ Index: " + index);
                resolvedCommands.Add(ParseCommand(commandStrings[index].CommandParameters, startingLineNumber));
            }
            commandStrings.RemoveAt(index);

            if (resolvedCommands.Count > 1) return new MultiActionCommand(resolvedCommands); else return resolvedCommands[0];
        }

        public Command ParseCommand(String commandLine, int lineNumber = 0) {
            return ParseCommand(ParseCommandParameters(ParseTokens(commandLine)), lineNumber);
        }

        Command ParseCommand(List<CommandParameter> parameters, int lineNumber) {
            Trace("Parsing Command at line: " + lineNumber);
            Trace("Pre Processed Parameters:");
            parameters.ForEach(param => Trace("Type: " + param.GetType()));

            var branches = new List<List<CommandParameter>>();
            branches.Add(parameters);

            //Branches
            while (branches.Count > 0) {
                branches.AddRange(ProcessParameters(branches[0]));
                if (branches[0].Count == 1 && branches[0][0] is CommandReferenceParameter) {
                    return ((CommandReferenceParameter)branches[0][0]).value;
                } else {
                    branches.RemoveAt(0);
                }
            }

            throw new Exception("Unable to parse command from command parameters at line number: " + lineNumber);
        }

        class CommandLine {
            public int Depth;
            public List<CommandParameter> CommandParameters;
            public String CommandString;

            public CommandLine(String commandString, Program program) {
                Depth = commandString.TakeWhile(Char.IsWhiteSpace).Count();
                CommandParameters = program.ParseCommandParameters(program.ParseTokens(commandString));
                CommandString = commandString;
            }

            public bool ShouldIgnore() {
                return String.IsNullOrWhiteSpace(CommandString) || CommandString.Trim().StartsWith("#");
            }
        }

        public class Thread {
            public Command Command { get; set; }
            public Dictionary<String, Variable> threadVariables = new Dictionary<string, Variable>();
            String prefix;
            String name;

            public Thread(Command command, string prefix, string name) {
                this.Command = command;
                this.prefix = prefix;
                this.name = name;
            }

            public String GetName() => "[" + prefix + "] " + name;
            public void SetName(String s) => name = s;
        }

        public class FunctionDefinition {
            public String functionName;
            public MultiActionCommand function;
            public List<String> parameterNames;

            public FunctionDefinition(string functionName, List<string> parameterNames) {
                this.functionName = functionName;
                this.parameterNames = parameterNames;
                this.function = null;
            }

            public FunctionDefinition(string functionName, MultiActionCommand function, List<string> parameterNames) {
                this.functionName = functionName;
                this.function = function;
                this.parameterNames = parameterNames;
            }
        }
    }
}
