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
    partial class Program : MyGridProgram 
    {
        //Debug
        private static UpdateFrequency UPDATE_FREQUENCY = UpdateFrequency.Update1;
        private static bool DEBUG_LOG = false;

        static MultiActionCommand RUNNING_COMMANDS;
        static MyGridProgram PROGRAM;

        static ProgramState state = ProgramState.STOPPED;

        public Program() {PROGRAM = this; initParsers();}

        static void Print(String str) { PROGRAM.Echo(str); }
        static void Debug(String str) { if (DEBUG_LOG) PROGRAM.Echo(str); }

        public void Main(string argument, UpdateType updateSource) {
            ParseCommands();
            if (String.IsNullOrEmpty(argument)) {
                if (state == ProgramState.STOPPED || state == ProgramState.COMPLETE) {
                    RUNNING_COMMANDS.Reset();
                    state = ProgramState.RUNNING;
                }
                if (RUNNING_COMMANDS.Execute()) state = ProgramState.COMPLETE;
            }
            else {
                ParseCommand(argument).Execute();
            }
            UpdateState();
        }

        void UpdateState()
        {
            switch(state)
            {
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

        static void ParseCommands() {
            if (RUNNING_COMMANDS == null) {
                Print("Parsing Custom Data");
                List<CommandLine> commandStrings = PROGRAM.Me.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(str => new CommandLine(str))
                    .ToList();

                Command parsedCommand = ParseCommand(commandStrings, 0, true);

                if (parsedCommand is MultiActionCommand) RUNNING_COMMANDS = (MultiActionCommand)parsedCommand;
                else RUNNING_COMMANDS = new MultiActionCommand(new List<Command> { parsedCommand });
            }
        }

        static Command ParseCommand(List<CommandLine> commandStrings, int index, bool parseSiblings) {
            Print("Debug Command at index: " + index);
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

        private static Command ParseCommand(String commandLine) {
            return ParseCommand(ParseCommandParameters(ParseTokens(commandLine)));
        }

        private static Command ParseCommand(List<CommandParameter> parameters) {
            Debug("Pre Processed Parameters:");
            parameters.ForEach(param => Debug("Type: " + param.GetType()));

            ParameterProcessorRegistry.process(parameters);

            Debug("Post Prossessed Parameters:");
            parameters.ForEach(param => Debug("Type: " + param.GetType()));

            return CommandParserRegistry.ParseCommand(parameters);
        }

        class CommandLine {
            public int Depth;
            public List<CommandParameter> CommandParameters;

            public CommandLine(String commandString)
            {
                Depth = commandString.TakeWhile(Char.IsWhiteSpace).Count();
                CommandParameters = ParseCommandParameters(ParseTokens(commandString));
            }
        }
    }
}
