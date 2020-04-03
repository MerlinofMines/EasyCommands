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
    partial class Program
    {
        public static class CommandParserRegistry
        {
            private static List<CommandParser> CommandParsers = new List<CommandParser>();

            public static void RegisterParser(CommandParser commandParser) { CommandParsers.Add(commandParser);}

            public static Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                program.Echo("Parsing Command");
                CommandParser parser = CommandParsers.Find(p => p.CanHandle(commandParameters));
                if (parser == null) throw new Exception("Unsupported Command");
                return parser.ParseCommand(program, commandParameters);
            }
        }
    }
}
