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
        //TODO: Add ParameterParserRegistry

        public interface ParameterParser
        {
            bool process(String commandParameter, List<CommandParameter> parameters);
        }

        public class BlockTypeParser : ParameterParser
        {
            private Dictionary<String, BlockType> blockTypeGroupWords = new Dictionary<String, BlockType>() {
                { "pistons", BlockType.PISTON },
                { "lights", BlockType.LIGHT },
                { "rotors", BlockType.ROTOR },
                { "programs", BlockType.PROGRAM },
                { "timers", BlockType.TIMER },
                { "projectors", BlockType.PROJECTOR },
                { "connectors", BlockType.CONNECTOR },
                { "welders", BlockType.WELDER },
                { "grinders", BlockType.GRINDER }
            };

            private Dictionary<String, BlockType> blockTypeWords = new Dictionary<String, BlockType>() {
                { "piston", BlockType.PISTON },
                { "light", BlockType.LIGHT },
                { "rotor", BlockType.ROTOR },
                { "program", BlockType.PROGRAM },
                { "timer", BlockType.TIMER },
                { "projector", BlockType.PROJECTOR },
                { "merge", BlockType.MERGE },
                { "connector", BlockType.CONNECTOR },
                { "welder", BlockType.WELDER },
                { "grinder", BlockType.GRINDER }
            };

            public bool process(String token, List<CommandParameter> parameters)
            {
                BlockType blockType;
                if (blockTypeGroupWords.TryGetValue(token, out blockType))
                {
                    parameters.Add(new BlockTypeCommandParameter(blockType));
                    parameters.Add(new GroupCommandParameter());
                    return true;
                } else if (blockTypeWords.TryGetValue(token, out blockType))
                {
                    parameters.Add(new BlockTypeCommandParameter(blockType));
                    return true;
                }
                return false;
            }
        }
    }
}
