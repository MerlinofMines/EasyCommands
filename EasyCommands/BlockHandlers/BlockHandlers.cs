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
               { BlockType.DOOR, new DoorBlockHandler() },
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

            public static List<IMyFunctionalBlock> GetBlocks(BlockType blockType, String customName)
            {
                return blockHandlers[blockType].GetBlocks(customName);
            }

            public static List<IMyFunctionalBlock> GetBlocks(IMyBlockGroup group, BlockType blockType)
            {
                return blockHandlers[blockType].GetBlocks(group);
            }
        }

        //Property Getters
        public delegate String StringPropertyGetter<T>(T block);
        public delegate bool BooleanPropertyGetter<T>(T block);
        public delegate float NumericPropertyGetter<T>(T block);

        //Property Setters
        public delegate void StringPropertySetter<T>(T block, String value);
        public delegate void BooleanPropertySetter<T>(T block, bool value);
        public delegate void SetPropertyValue<T>(T block, float value);
        public delegate void SetPropertyValueDirection<T>(T block, DirectionType DirectionType, float value);
        public delegate void IncrementPropertyValue<T>(T block, float deltaValue);
        public delegate void IncrementPropertyValueDirection<T>(T block, DirectionType direction, float deltaValue);
        public delegate void MovePropertyValue<T>(T block, DirectionType direction);
        public delegate void ReversePropertyValue<T>(T block);

        public class NumericPropertySetter<T>
        {
            public SetPropertyValue<T> Set;
            public SetPropertyValueDirection<T> SetDirection;
            public IncrementPropertyValue<T> Increment;
            public IncrementPropertyValueDirection<T> IncrementDirection;
            public MovePropertyValue<T> Move;
            public ReversePropertyValue<T> Reverse;
        }

        public interface BlockHandler {
            BooleanPropertyType GetDefaultBooleanProperty();
            StringPropertyType GetDefaultStringProperty();
            NumericPropertyType GetDefaultNumericProperty(DirectionType direction);
            DirectionType GetDefaultDirection();
            List<IMyFunctionalBlock> GetBlocks(String name);
            List<IMyFunctionalBlock> GetBlocks(IMyBlockGroup group);
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

            public List<IMyFunctionalBlock> GetBlocks(String name)
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                PROGRAM.GridTerminalSystem.GetBlocksOfType<T>(blocks, block => block.CustomName.ToLower().Equals(name));
                return blocks.Select(block => (IMyFunctionalBlock)block).ToList();
            }

            public List<IMyFunctionalBlock> GetBlocks(IMyBlockGroup group)
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                group.GetBlocksOfType<T>(blocks);
                return blocks.Select(block => (IMyFunctionalBlock)block).ToList();
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
                numericPropertySetters[property].Set((T)block, value);
            }

            public void SetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction, float value)
            {
                Print("Setting " + block.CustomName + " " + property + " to " + value + "in " + direction + "direction");
                numericPropertySetters[property].SetDirection((T)block, direction, value);
            }

            public void IncrementNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, float deltaValue)
            {
                Print("Incrementing " + block.CustomName + " " + property + " by " + deltaValue);
                numericPropertySetters[property].Increment((T)block, deltaValue);
            }

            public void IncrementNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction, float deltaValue)
            {
                Print("Incrementing " + block.CustomName + " " + property + " by " + deltaValue + "in " + direction + "direction");
                numericPropertySetters[property].IncrementDirection((T)block, direction, deltaValue);
            }

            public void MoveNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction)
            {
                Print("Moving " + block.CustomName + " " + property + "in " + direction + "direction");
                numericPropertySetters[property].Move((T)block, direction);
            }

            public void ReverseNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property)
            {
                Print("Reversing " + block.CustomName + " " + property);
                numericPropertySetters[property].Reverse((T)block);
            }
        }
    }
}
