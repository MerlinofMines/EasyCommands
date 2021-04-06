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
        public static UpdateFrequency UPDATE_FREQUENCY = UpdateFrequency.Update1;
        public static LogLevel LOG_LEVEL = LogLevel.INFO;
        public static int FUNCTION_PARSE_AMOUNT = 1;
        public static int MAX_ASYNC_THREADS = 50;
        public static int MAX_QUEUED_THREADS = 50;
        #endregion

        int AsyncThreadQueueIndex = 0;
        bool InAsyncThreadQueue = false;
        List<Thread> ThreadQueue = new List<Thread>();
        List<Thread> AsyncThreadQueue = new List<Thread>();

        public void ClearAllThreads() {
            AsyncThreadQueue.Clear();
            ThreadQueue.Clear();
        }

        public Thread GetCurrentThread() {
            if (InAsyncThreadQueue) {
                return AsyncThreadQueue[AsyncThreadQueueIndex];
            } else {
                return ThreadQueue[0];
            }
        }

        public void QueueThread(Thread thread) {
            ThreadQueue.Add(thread);
            if (ThreadQueue.Count > MAX_QUEUED_THREADS) throw new Exception("Stack Overflow Exception! Cannot have more than " + MAX_QUEUED_THREADS + " queued commands");
        }

        public void QueueAsyncThread(Thread thread) {
            AsyncThreadQueue.Add(thread);
            if (AsyncThreadQueue.Count > MAX_ASYNC_THREADS) throw new Exception("Stack Overflow Exception! Cannot have more than " + MAX_ASYNC_THREADS + "concurrent async commands");
        }

        public static Dictionary<String, FunctionDefinition> FUNCTIONS = new Dictionary<string, FunctionDefinition>();
        static String DEFAULT_FUNCTION;
        static String CUSTOM_DATA = null;
        static List<String> COMMAND_STRINGS = new List<String>();
        static Program PROGRAM;
        public static ProgramState STATE = ProgramState.STOPPED;
        public delegate String CustomDataProvider(MyGridProgram program);
        public delegate List<MyIGCMessage> BroadcastMessageProvider();
        public BroadcastMessageProvider broadcastMessageProvider;
        static Dictionary<String, Variable> memoryVariables = new Dictionary<string, Variable>();

        List<MyIGCMessage> provideMessages()
        {
            List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
            IGC.GetBroadcastListeners(listeners);
            return listeners.Where(l => l.HasPendingMessage).Select(l => l.AcceptMessage()).ToList();
        }

        public Program() {
            PROGRAM = this;
            InitializeParsers();
            ParameterProcessorRegistry.InitializeProcessors();
            InitializeOperators();
            Runtime.UpdateFrequency = UPDATE_FREQUENCY;
            broadcastMessageProvider = provideMessages;
        }

        static void Print(String str) { PROGRAM.Echo(str); }
        static void Info(String str) { if (LOG_LEVEL != LogLevel.SCRIPT_ONLY) PROGRAM.Echo(str); }
        static void Debug(String str) { if (LOG_LEVEL == LogLevel.DEBUG || LOG_LEVEL == LogLevel.TRACE) PROGRAM.Echo(str); }
        static void Trace(String str) { if (LOG_LEVEL == LogLevel.TRACE) PROGRAM.Echo(str); }

        public void Main(string argument) {
            try {
                if (!ParseCommands()) {
                    Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                    return;
                }
            } catch (Exception e) {
                Info("Exception Occurred During Parsing: ");
                Info(e.Message);
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }

            Debug("Functions: " + FUNCTIONS.Count);
            Debug("Argument: " + argument);

            List<MyIGCMessage> messages = broadcastMessageProvider();

            try {
                if (messages.Count > 0) {
                    List<Thread> messageCommands = messages.Select(message => new Thread(ParseCommand((String)message.Data),"Message", message.Tag)).ToList();
                    ThreadQueue.InsertRange(0, messageCommands);
                }
                if (!String.IsNullOrEmpty(argument)) { ThreadQueue.Insert(0, new Thread(ParseCommand(argument),"Request", argument)); }

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
            InAsyncThreadQueue = false;
            try {
                //If no current commands, we've been asked to restart.  start at the top.
                if(ThreadQueue.Count == 0 && AsyncThreadQueue.Count == 0) {
                    FunctionDefinition defaultFunction = FUNCTIONS[DEFAULT_FUNCTION];
                    ThreadQueue.Add(new Thread(defaultFunction.function.Clone(), "Main", defaultFunction.functionName));
                }

                Info("Queued Threads: " + ThreadQueue.Count());
                Info("Async Threads: " + AsyncThreadQueue.Count());
                //Run first command in the queue.  Could be from a message, program run request, or request to start the main program.
                if (ThreadQueue.Count > 0 ) {
                    Thread currentThread = ThreadQueue[0];
                    Info("Current Thread: " + currentThread.GetName());
                    if(currentThread.Command.Execute()) {
                        ThreadQueue.RemoveAt(0);
                    }
                }

                //Process 1 iteration of all async commands, removing from queue if processed.
                InAsyncThreadQueue = true;
                AsyncThreadQueueIndex = 0;

                while (AsyncThreadQueueIndex < AsyncThreadQueue.Count) {
                    Thread currentThread = AsyncThreadQueue[AsyncThreadQueueIndex];
                    Info("Async Thread: " + currentThread.GetName());
                    if (currentThread.Command.Execute()) {
                        AsyncThreadQueue.RemoveAt(AsyncThreadQueueIndex);
                    } else {
                        AsyncThreadQueueIndex++;
                    }
                }
                if(ThreadQueue.Count == 0 && AsyncThreadQueue.Count == 0) {
                    STATE = ProgramState.COMPLETE;
                } else {
                    STATE = ProgramState.RUNNING;
                }
            //InterruptException is thrown by control commands to interrupt execution (stop, pause, restart).
            //The command itself has set the correct state of the command queues, we just need to set the program state.
            } catch(InterruptException interrupt) {
                Debug("Script interrupted!");
                STATE = interrupt.ProgramState;
            }
        }

        void UpdateState() {
            switch (STATE) {
                case ProgramState.RUNNING:
                    Runtime.UpdateFrequency = UPDATE_FREQUENCY;
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

        static bool ParseCommands() {
            if (String.IsNullOrWhiteSpace(PROGRAM.Me.CustomData) || !PROGRAM.Me.CustomData.Equals(CUSTOM_DATA)) {
                CUSTOM_DATA = PROGRAM.Me.CustomData;
                COMMAND_STRINGS = CUSTOM_DATA.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(line => !line.StartsWith("#"))
                    .ToList();
                if (COMMAND_STRINGS.Count == 0) {
                    Info("Welcome to EasyCommands!");
                    Info("Add Commands to Custom Data");
                    return false;
                }
                Info("Parsing Custom Data");
                FUNCTIONS.Clear();
                PROGRAM.ClearAllThreads();
            }

            if (COMMAND_STRINGS.Count == 0) return true;
            Info("Parsing Commands.  Lines Left: " + COMMAND_STRINGS.Count);
            ParseFunctions(COMMAND_STRINGS);

            return COMMAND_STRINGS.Count == 0;
        }

        static void ParseFunctions(List<String> commandStrings) {
            List<int> functionIndices = new List<int>();
            if (!commandStrings[0].StartsWith(":")) { commandStrings.Insert(0, ":main"); }

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
                FUNCTIONS[functionName] = definition;
            }

            //Parse Function Commands and add to Definitions
            int toParse = FUNCTION_PARSE_AMOUNT;
            foreach (int i in functionIndices) {
                String functionString = commandStrings[i].Remove(0, 1).Trim();
                List<Token> nameAndParams = ParseTokens(functionString);
                String functionName = nameAndParams[0].original;
                Info("Parsing Function: " + functionName);
                Command command = ParseCommand(commandStrings.GetRange(i + 1, commandStrings.Count - (i + 1)).Select(str => new CommandLine(str)).ToList(), 0, true);
                commandStrings.RemoveRange(i, commandStrings.Count - i);
                if (!(command is MultiActionCommand)) { command = new MultiActionCommand(new List<Command> { command }); }
                FUNCTIONS[functionName].function = (MultiActionCommand)command;
                DEFAULT_FUNCTION = functionName;
                toParse--;
                if (toParse == 0) break;//Exceeded # of Functions to parse for 1 tick
            }
        }

        static Command ParseCommand(List<CommandLine> commandStrings, int index, bool parseSiblings) {
            List<Command> resolvedCommands = new List<Command>();
            while (index < commandStrings.Count - 1)//Parse Sibling & Child Commands, if any
            {
                CommandLine current = commandStrings[index];
                CommandLine next = commandStrings[index + 1];
                if (current.Depth > next.Depth) break;//End, break
                if (current.Depth < next.Depth) {//I'm a parent of next line
                    Trace("Parsing Sub Command @ Index: " + (index + 1));
                    current.CommandParameters.Add(new CommandReferenceParameter(ParseCommand(commandStrings, index + 1, true)));
                    continue;
                }
                if (next.CommandParameters[0] is ElseCommandParameter) {//Handle Otherwise
                    Trace("Handling Otherwise @ index: " + index);
                    current.CommandParameters.Add(next.CommandParameters[0]);
                    next.CommandParameters.RemoveAt(0);
                    current.CommandParameters.Add(new CommandReferenceParameter(ParseCommand(commandStrings, index + 1, false)));
                    continue;
                }

                if (!parseSiblings) break;//Only parsing myself
                Trace("Parsing Sibling Command @ Index: " + index);
                resolvedCommands.Add(ParseCommand(current.CommandParameters));
                commandStrings.RemoveAt(index);
            }

            //Parse Last one, which has become current
            Trace("Parsing Final Command @ Index: " + index);
            resolvedCommands.Add(ParseCommand(commandStrings[index].CommandParameters));
            commandStrings.RemoveAt(index);

            if (resolvedCommands.Count > 1) return new MultiActionCommand(resolvedCommands); else return resolvedCommands[0];
        }

        public static Command ParseCommand(String commandLine) {
            return ParseCommand(ParseCommandParameters(ParseTokens(commandLine)));
        }

        private static Command ParseCommand(List<CommandParameter> parameters) {
            Trace("Parsing Command");
            Trace("Pre Processed Parameters:");
            parameters.ForEach(param => Trace("Type: " + param.GetType()));

            ParameterProcessorRegistry.Process(parameters);

            if (parameters.Count != 1 || !(parameters[0] is CommandReferenceParameter)) throw new Exception("Unable to parse command from command parameters!");
            return ((CommandReferenceParameter)parameters[0]).Value;
        }

        class CommandLine {
            public int Depth;
            public List<CommandParameter> CommandParameters;
            public String CommandString;

            public CommandLine(String commandString) {
                Depth = commandString.TakeWhile(Char.IsWhiteSpace).Count();
                CommandParameters = ParseCommandParameters(ParseTokens(commandString));
                CommandString = commandString;
            }
        }

        public class Thread {
            public Command Command { get; set; }
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
