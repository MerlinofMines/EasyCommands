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
        private static bool DEBUG_LOG = true;

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
                String[] commandStrings = PROGRAM.Me.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                List<Command> commands = commandStrings
                    .Select(command => ParseCommand(command))
                    .ToList();

                RUNNING_COMMANDS = new MultiActionCommand(commands);
            }
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
    }
}
