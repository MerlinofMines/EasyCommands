﻿using Sandbox.Game.EntityComponents;
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
        static UpdateFrequency UPDATE_FREQUENCY = UpdateFrequency.Update1;
        static bool DEBUG_LOG = false;
        static int PARSE_AMOUNT = 1;

        static MultiActionCommand RUNNING_COMMANDS;
        public static Dictionary<String, FunctionDefinition> FUNCTIONS = new Dictionary<string, FunctionDefinition>();
        static String DEFAULT_FUNCTION;
        static String RUNNING_FUNCTION;
        static String CUSTOM_DATA;
        static String ARGUMENT;
        static List<String> COMMAND_STRINGS = new List<String>();
        static MyGridProgram PROGRAM;
        static ProgramState STATE = ProgramState.STOPPED;
        public delegate String CustomDataProvider(MyGridProgram program);
        public delegate List<MyIGCMessage> BroadcastMessageProvider(MyGridProgram program);
        static public CustomDataProvider CUSTOM_DATA_PROVIDER = (p) => p.Me.CustomData;
        static public BroadcastMessageProvider BROADCAST_MESSAGE_PROVIDER = provideMessages;
        static Dictionary<String, Variable> memoryVariables = new Dictionary<string, Variable>();

        static List<MyIGCMessage> provideMessages(MyGridProgram program)
        {
            List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
            program.IGC.GetBroadcastListeners(listeners);
            return listeners.Where(l => l.HasPendingMessage).Select(l => l.AcceptMessage()).ToList();
        }

        public Program() {
            PROGRAM = this;
            InitializeParsers();
            ParameterProcessorRegistry.InitializeProcessors();
            Runtime.UpdateFrequency = UPDATE_FREQUENCY;
        }

        static void Print(String str) { PROGRAM.Echo(str); }
        static void Debug(String str) { if (DEBUG_LOG) PROGRAM.Echo(str); }

        public void Main(string argument) {
            if (!String.IsNullOrEmpty(argument)) { ARGUMENT = argument; }

            try {
                if (!ParseCommands()) {
                    Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                    return;
                }
            } catch (Exception e) {
                Print("Exception Occurred During Parsing: ");
                Print(e.Message);
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }

            Echo("Functions: " + FUNCTIONS.Count);
            Echo("Running Function: " + RUNNING_FUNCTION);
            Echo("Argument: " + ARGUMENT);

            List<MyIGCMessage> messages = BROADCAST_MESSAGE_PROVIDER(PROGRAM);

            try {
                if (messages.Count > 0) {
                    ParseCommand((String)messages[0].Data).Execute();
                } else if (String.IsNullOrEmpty(ARGUMENT)) {
                    if (STATE == ProgramState.STOPPED || STATE == ProgramState.COMPLETE) {
                        RUNNING_COMMANDS.Reset();
                        STATE = ProgramState.RUNNING;
                    }
                    if (RUNNING_COMMANDS.Execute()) STATE = ProgramState.COMPLETE;
                } else { ParseCommand(ARGUMENT).Execute(); ARGUMENT = null; }
                UpdateState();
            } catch(Exception e) {
                Print("Exception Occurred: ");
                Print(e.Message);
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }
        }

        void UpdateState() {
            switch (STATE) {
                case ProgramState.RUNNING:
                    Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                    Print("Running");
                    break;
                case ProgramState.PAUSED:
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    Print("Paused");
                    break;
                case ProgramState.STOPPED:
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    Print("Stopped");
                    break;
                case ProgramState.COMPLETE:
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                    Print("Complete");
                    break;
                default:
                    throw new Exception("Unknown Program State");
            }
        }

        static bool ParseCommands() {
            if ((RUNNING_COMMANDS == null && COMMAND_STRINGS.Count==0) || !CUSTOM_DATA.Equals(CUSTOM_DATA_PROVIDER(PROGRAM))) {
                CUSTOM_DATA = CUSTOM_DATA_PROVIDER(PROGRAM);
                COMMAND_STRINGS = CUSTOM_DATA.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (COMMAND_STRINGS.Count == 0) {
                    Print("Welcome to EasyCommands!");
                    Print("Add Commands to Custom Data");
                    return false;
                }
                Print("Parsing Custom Data");
                FUNCTIONS.Clear();
            }

            if (COMMAND_STRINGS.Count == 0) return true;
            Print("Parsing Commands.  Lines Left: " + COMMAND_STRINGS.Count);
            ParseFunctions(COMMAND_STRINGS);

            if (COMMAND_STRINGS.Count > 0) return false;
            RUNNING_COMMANDS = (MultiActionCommand)(FUNCTIONS[DEFAULT_FUNCTION].function).Copy();
            return true;
        }

        static void ParseFunctions(List<String> commandStrings) {
            List<int> functionIndices = new List<int>();
            if (!commandStrings[0].StartsWith(":")) { commandStrings.Insert(0, ":main"); }

            for (int i = commandStrings.Count - 1; i >= 0; i--) {
                Debug("Command String: " + commandStrings[i]);
                if (commandStrings[i].StartsWith(":")) { functionIndices.Add(i); }
            }
            Debug("Function Indices: ");
            Debug(String.Join(" | ", functionIndices));

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
            int toParse = PARSE_AMOUNT;
            foreach (int i in functionIndices) {
                String functionString = commandStrings[i].Remove(0, 1).Trim();
                List<Token> nameAndParams = ParseTokens(functionString);
                String functionName = nameAndParams[0].original;
                Print("Parsing Function: " + functionName);
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
                    Debug("Parsing Sub Command @ Index: " + (index + 1));
                    current.CommandParameters.Add(new CommandReferenceParameter(ParseCommand(commandStrings, index + 1, true)));
                    continue;
                }
                if (next.CommandParameters[0] is ElseCommandParameter) {//Handle Otherwise
                    Debug("Handling Otherwise @ index: " + index);
                    current.CommandParameters.Add(next.CommandParameters[0]);
                    next.CommandParameters.RemoveAt(0);
                    current.CommandParameters.Add(new CommandReferenceParameter(ParseCommand(commandStrings, index + 1, false)));
                    continue;
                }

                if (!parseSiblings) break;//Only parsing myself
                Debug("Parsing Sibling Command @ Index: " + index);
                resolvedCommands.Add(ParseCommand(current.CommandParameters));
                commandStrings.RemoveAt(index);
            }

            //Parse Last one, which has become current
            Debug("Parsing Final Command @ Index: " + index);
            resolvedCommands.Add(ParseCommand(commandStrings[index].CommandParameters));
            commandStrings.RemoveAt(index);

            if (resolvedCommands.Count > 1) return new MultiActionCommand(resolvedCommands); else return resolvedCommands[0];
        }

        public static Command ParseCommand(String commandLine) {
            return ParseCommand(ParseCommandParameters(ParseTokens(commandLine)));
        }

        private static Command ParseCommand(List<CommandParameter> parameters) {
            Debug("Parsing Command");
            Debug("Pre Processed Parameters:");
            parameters.ForEach(param => Debug("Type: " + param.GetType()));

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
