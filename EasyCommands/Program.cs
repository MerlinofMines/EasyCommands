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
        private static bool DEBUG_LOG = false;
        private static UpdateFrequency UPDATE_FREQUENCY = UpdateFrequency.Update1;

        static MultiActionCommand RUNNING_COMMANDS;
        static MyGridProgram PROGRAM;

        static bool RUNNING = false;

        public Program() {PROGRAM = this; initParsers();}

        static void Print(String str) { PROGRAM.Echo(str); }
        static void Debug(String str) { if (DEBUG_LOG) PROGRAM.Echo(str); }

        public void Main(string argument, UpdateType updateSource) {
            ParseCommands();
            if (String.IsNullOrEmpty(argument)) {
                if (!RUNNING) RUNNING_COMMANDS.Reset();
                ExecuteCommand(RUNNING_COMMANDS);
            } else {
                ExecuteCommand(ParseCommand(argument));
            }
        }

        void ExecuteCommand(Command command)
        {
            RUNNING = true;
            if (command.Execute()) {
                RUNNING = false;
                Runtime.UpdateFrequency = UpdateFrequency.None;
                Print("Execution Complete");
            } else {
                Runtime.UpdateFrequency = UPDATE_FREQUENCY;
            }
        }

        void ParseCommands() {
            if (RUNNING_COMMANDS == null) {
                Print("Parsing Custom Data");
                String[] commandStrings = Me.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                List<Command> commands = commandStrings
                    .Select(command => ParseCommand(command))
                    .ToList();

                RUNNING_COMMANDS = new MultiActionCommand(commands);
            }
        }

        private Command ParseCommand(String commandLine) {
            return ParseCommand(parseCommandParameters(parseTokens(commandLine)));
        }

        private Command ParseCommand(List<CommandParameter> parameters) {
            Debug("Pre Processed Parameters:");
            parameters.ForEach(param => Debug("Type: " + param.GetType()));

            ParameterProcessorRegistry.process(parameters);

            Debug("Post Prossessed Parameters:");
            parameters.ForEach(param => Debug("Type: " + param.GetType()));

            return CommandParserRegistry.ParseCommand(parameters);
        }
    }
}
