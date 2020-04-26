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
        public static class BlockHandlerRegistry
        {
            static readonly Dictionary<BlockType, BlockHandler> blockHandlers = new Dictionary<BlockType, BlockHandler> {
               { BlockType.PISTON, new PistonBlockHandler() },
               { BlockType.LIGHT, new BlockHandler<IMyLightingBlock>() },
               { BlockType.MERGE, new MergeBlockHandler() },
               { BlockType.PROJECTOR, new ProjectorBlockHandler() },
               { BlockType.TIMER, new BlockHandler<IMyTimerBlock>() },
               { BlockType.CONNECTOR, new ConnectorBlockHandler() },
               { BlockType.WELDER, new BlockHandler<IMyShipWelder>() },
               { BlockType.GRINDER, new BlockHandler<IMyShipGrinder>() },
               { BlockType.ROTOR, new RotorBlockHandler() },
               { BlockType.PROGRAM, new ProgramBlockHandler() },
            };

            public static BlockHandler GetBlockHandler(BlockType blockType)
            {
                if (blockHandlers.ContainsKey(blockType))
                {
                    return blockHandlers[blockType];
                }
                else
                {
                    throw new Exception("Unsupported Block Type");
                }
            }

            public static List<IMyFunctionalBlock> getBlocks(BlockType blockType, String customName)
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

                switch (blockType)
                {
                    case BlockType.PISTON: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.ROTOR: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.LIGHT: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.MERGE: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyShipMergeBlock>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.PROJECTOR: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyProjector>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.TIMER: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.CONNECTOR: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.WELDER: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyShipWelder>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.GRINDER: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.PROGRAM: PROGRAM.GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(blocks, block => block.CustomName.ToLower() == customName); break;
                    default: throw new Exception("Unsupported Block Type: " + blockType);
                }
                return blocks.Select(block => (IMyFunctionalBlock)block).ToList();
            }

            public static List<IMyFunctionalBlock> getBlocks(IMyBlockGroup group, BlockType blockType)
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

                switch (blockType)
                {
                    case BlockType.PISTON: group.GetBlocksOfType<IMyPistonBase>(blocks); break;
                    case BlockType.ROTOR: group.GetBlocksOfType<IMyMotorStator>(blocks); break;
                    case BlockType.LIGHT: group.GetBlocksOfType<IMyInteriorLight>(blocks); break;
                    case BlockType.MERGE: group.GetBlocksOfType<IMyShipMergeBlock>(blocks); break;
                    case BlockType.PROJECTOR: group.GetBlocksOfType<IMyProjector>(blocks); break;
                    case BlockType.TIMER: group.GetBlocksOfType<IMyTimerBlock>(blocks); break;
                    case BlockType.CONNECTOR: group.GetBlocksOfType<IMyShipConnector>(blocks); break;
                    case BlockType.WELDER: group.GetBlocksOfType<IMyShipWelder>(blocks); break;
                    case BlockType.GRINDER: group.GetBlocksOfType<IMyShipGrinder>(blocks); break;
                    case BlockType.PROGRAM: group.GetBlocksOfType<IMyProgrammableBlock>(blocks); break;
                    default: throw new Exception("Unsupported Block Type: " + blockType);
                }
                return blocks.Select(block => (IMyFunctionalBlock)block).ToList();
            }
        }

        //Property Getters
        public delegate String StringPropertyGetter<T>(T block);
        public delegate bool BooleanPropertyGetter<T>(T block);
        public delegate float NumericPropertyGetter<T>(T block);

        //Property Setters
        public delegate void StringPropertySetter<T>(T block, String value);
        public delegate void BooleanPropertySetter<T>(T block, bool value);
        public interface NumericPropertySetter<T> {
            void SetPropertyValue(T block, float value);
            void SetPropertyValue(T block, DirectionType DirectionType, float value);
            void IncrementPropertyValue(T block, float deltaValue);
            void IncrementPropertyValue(T block, DirectionType direction, float deltaValue);
            void MovePropertyValue(T block, DirectionType direction);
            void ReversePropertyValue(T block);
        }

        public interface BlockHandler {
            BooleanPropertyType GetDefaultBooleanProperty();
            StringPropertyType GetDefaultStringProperty();
            NumericPropertyType GetDefaultNumericProperty(DirectionType direction);
            DirectionType GetDefaultDirection();
            bool GetBooleanPropertyValue(IMyFunctionalBlock block, BooleanPropertyType property);
            string GetStringPropertyValue(IMyFunctionalBlock block, StringPropertyType property);
            float GetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property);
            void SetBooleanPropertyValue(IMyFunctionalBlock block, BooleanPropertyType property, bool value);
            void SetStringPropertyValue(IMyFunctionalBlock block, StringPropertyType property, String value);
            void SetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, float value);
            void SetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction, float value);
            void IncrementNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, float deltaValue);
            void IncrementNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction, float deltaValue);
            void MoveNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction);
            void ReverseNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property);
        }

        public class BlockHandler<T> : BlockHandler where T : class, IMyFunctionalBlock
        {
            protected Dictionary<BooleanPropertyType, BooleanPropertyGetter<T>> booleanPropertyGetters = new Dictionary<BooleanPropertyType, BooleanPropertyGetter<T>>();
            protected Dictionary<StringPropertyType, StringPropertyGetter<T>> stringPropertyGetters = new Dictionary<StringPropertyType, StringPropertyGetter<T>>();
            protected Dictionary<NumericPropertyType, NumericPropertyGetter<T>> numericPropertyGetters = new Dictionary<NumericPropertyType, NumericPropertyGetter<T>>();

            protected Dictionary<BooleanPropertyType, BooleanPropertySetter<T>> booleanPropertySetters = new Dictionary<BooleanPropertyType, BooleanPropertySetter<T>>();
            protected Dictionary<StringPropertyType, StringPropertySetter<T>> stringPropertySetters = new Dictionary<StringPropertyType, StringPropertySetter<T>>();
            protected Dictionary<NumericPropertyType, NumericPropertySetter<T>> numericPropertySetters = new Dictionary<NumericPropertyType, NumericPropertySetter<T>>();

            protected BooleanPropertyType defaultBooleanProperty = BooleanPropertyType.ON_OFF;
            protected StringPropertyType defaultStringProperty = StringPropertyType.NAME;
            protected Dictionary<DirectionType, NumericPropertyType> defaultNumericProperties = new Dictionary<DirectionType, NumericPropertyType>();
            protected DirectionType? defaultDirection = null;

            public BlockHandler()
            {
                booleanPropertyGetters.Add(BooleanPropertyType.ON_OFF, (block) => block.Enabled);
                booleanPropertySetters.Add(BooleanPropertyType.ON_OFF, (block, enabled) => block.Enabled = enabled); 
            }

            public BooleanPropertyType GetDefaultBooleanProperty()
            {
                return defaultBooleanProperty;
            }

            public StringPropertyType GetDefaultStringProperty()
            {
                return defaultStringProperty;
            }

            public NumericPropertyType GetDefaultNumericProperty(DirectionType direction)
            {
                if(!defaultNumericProperties.ContainsKey(direction))throw new Exception("This Block Does Not Have A Default Numeric Property");
                return defaultNumericProperties[direction];
            }

            public DirectionType GetDefaultDirection()
            {
                if (!defaultDirection.HasValue)throw new Exception("This Block Does Not Have a Default Direction");
                return defaultDirection.Value;
            }

            public bool GetBooleanPropertyValue(IMyFunctionalBlock block, BooleanPropertyType property)
            {
                return booleanPropertyGetters[property]((T)block);
            }
            public string GetStringPropertyValue(IMyFunctionalBlock block, StringPropertyType property)
            {
                return stringPropertyGetters[property]((T)block);
            }
            public float GetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property)
            {
                return numericPropertyGetters[property]((T)block);
            }

            public void SetBooleanPropertyValue(IMyFunctionalBlock block, BooleanPropertyType property, bool value)
            {
                Print("Setting " + block.CustomName + " " + property + " to " + value);
                booleanPropertySetters[property]((T)block, value);
            }

            public void SetStringPropertyValue(IMyFunctionalBlock block, StringPropertyType property, String value)
            {
                Print("Setting " + block.CustomName + " " + property + " to " + value);
                stringPropertySetters[property]((T)block, value);
            }

            public void SetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, float value)
            {
                Print("Setting " + block.CustomName + " " + property + " to " + value);
                numericPropertySetters[property].SetPropertyValue((T)block, value);
            }

            public void SetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction, float value)
            {
                Print("Setting " + block.CustomName + " " + property + " to " + value + "in " + direction + "direction");
                numericPropertySetters[property].SetPropertyValue((T)block, direction, value);
            }

            public void IncrementNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, float deltaValue)
            {
                Print("Incrementing " + block.CustomName + " " + property + " by " + deltaValue);
                numericPropertySetters[property].IncrementPropertyValue((T)block, deltaValue);
            }

            public void IncrementNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction, float deltaValue)
            {
                Print("Incrementing " + block.CustomName + " " + property + " by " + deltaValue + "in " + direction + "direction");
                numericPropertySetters[property].IncrementPropertyValue((T)block, direction, deltaValue);
            }

            public void MoveNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction)
            {
                Print("Moving " + block.CustomName + " " + property + "in " + direction + "direction");
                numericPropertySetters[property].MovePropertyValue((T)block, direction);
            }

            public void ReverseNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property)
            {
                Print("Reversing " + block.CustomName + " " + property);
                numericPropertySetters[property].ReversePropertyValue((T)block);
            }
        }
    }
}
