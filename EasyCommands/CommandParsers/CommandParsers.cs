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
        public interface CommandParser
        {
            bool CanHandle(List<CommandParameter> commandParameters);
            Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters);
        }

        public class WaitCommandParser : CommandParser
        {
            public bool CanHandle(List<CommandParameter> commandParameters)
            {
                return commandParameters.Exists(param => param is WaitCommandParameter);
            }

            public Command ParseCommand(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                return new WaitCommand(program, commandParameters);
            }
        }

        public class BlockHandlerCommandParser : CommandParser
        {
            public bool CanHandle(List<CommandParameter> commandParameters)
            {
                return commandParameters.FindIndex(param => param is BlockTypeCommandParameter) >= 0;
            }

            public Command ParseCommand(MyGridProgram program, List<CommandParameter> parameters)
            {
                CommandParameter blockTypeParameter = parameters.Find(param => param is BlockTypeCommandParameter);

                BlockType blockType = ((BlockTypeCommandParameter)blockTypeParameter).GetBlockType();

                switch (blockType)
                {
                    case BlockType.PISTON:
                        return new BlockHandlerCommand<IMyPistonBase>(program, parameters);
                    case BlockType.ROTOR:
                        return new BlockHandlerCommand<IMyMotorStator>(program, parameters);
                    case BlockType.LIGHT:
                        return new BlockHandlerCommand<IMyLightingBlock>(program, parameters);
                    case BlockType.PROGRAM:
                        return new BlockHandlerCommand<IMyProgrammableBlock>(program, parameters);
                    case BlockType.TIMER:
                        return new BlockHandlerCommand<IMyTimerBlock>(program, parameters);
                    case BlockType.PROJECTOR:
                        return new BlockHandlerCommand<IMyProjector>(program, parameters);
                    case BlockType.MERGE:
                        return new BlockHandlerCommand<IMyShipMergeBlock>(program, parameters);
                    case BlockType.CONNECTOR:
                        return new BlockHandlerCommand<IMyShipConnector>(program, parameters);
                    case BlockType.WELDER:
                        return new BlockHandlerCommand<IMyShipWelder>(program, parameters);
                    case BlockType.GRINDER:
                        return new BlockHandlerCommand<IMyShipGrinder>(program, parameters);
                    default:
                        throw new Exception("Unsupported Block Type Command: " + blockType);
                }
            }
        }
    }
}
