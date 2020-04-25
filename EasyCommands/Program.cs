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

        static MultiActionCommand RUNNING_COMMANDS;
        static MyGridProgram PROGRAM;

        static bool isRunning = false;

        public Program()
        {
            PROGRAM = this;
            initParsers();
        }

        static void Print(String str)
        {
            PROGRAM.Echo(str);
        }

        void validateParsed()
        {
            if (RUNNING_COMMANDS == null) {
                RUNNING_COMMANDS = new MultiActionCommand(ParseCommands());
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (String.IsNullOrEmpty(argument))
            {
                validateParsed();
                if (!isRunning) RUNNING_COMMANDS.Reset();
                isRunning = true;
                if (RUNNING_COMMANDS.Execute())
                {
                    Print("Execution Complete");
                    isRunning = false;
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                }
                else
                {
                    Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                }
            }
            else if (argument.ToLower() == "restart") //Restart execution of existing commands
            {
                validateParsed();
                Print("Restarting Commands");
                RUNNING_COMMANDS.Reset();
                Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                isRunning = true;
            }
            else if (argument.ToLower() == "loop") //Loop execution of existing commands.  TODO: "Loop 3"
            {
                Print("Looping Commands");
                validateParsed();
                if (!isRunning) {
                    RUNNING_COMMANDS.Reset();
                }
                else {
                    RUNNING_COMMANDS.Loop(1);
                }
                Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                isRunning = true;
            }
            else if (argument.ToLower() == "start") //Parse custom data and run
            {
                Print("Starting Commands");
                validateParsed();
                Runtime.UpdateFrequency = UPDATE_FREQUENCY;
            }
            else if (argument.ToLower() == "parse") // Parse Custom Data only.  Useful for debugging.
            {
                Print("Parsing Custom Data");
                ParseCommands();
                isRunning = false;
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
            else if (argument.ToLower() == "stop") //Stop execution
            {
                Print("Stopping Command Execution");
                Runtime.UpdateFrequency = UpdateFrequency.None;
                isRunning = false;
                RUNNING_COMMANDS = null;
            }
        }

        private List<Command> ParseCommands()
        {
            String[] commandList = Me.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            return commandList
                .Select(command => parseTokens(command))
                .Select(tokens => parseCommandParameters(tokens))
                .Select(parameters => parseCommand(parameters))
                .ToList();
        }

        private Command parseCommand(List<CommandParameter> parameters)
        {
            Print("Pre Processed Parameters:");
            parameters.ForEach(param => Print("Type: " + param.GetType()));

            ParameterProcessorRegistry.process(parameters);

            Print("Post Prossessed Parameters:");
            parameters.ForEach(param => Print("Type: " + param.GetType()));

            return CommandParserRegistry.ParseCommand(parameters);
        }
    }
}
