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

namespace IngameScript {
    partial class Program {
        public static class BlockHandlerRegistry {
            static readonly Dictionary<BlockType, BlockHandler> blockHandlers = new Dictionary<BlockType, BlockHandler> {
               { BlockType.AIRVENT, new AirVentBlockHandler()},
               { BlockType.ANTENNA, new AntennaBlockHandler()},
               { BlockType.BEACON, new BeaconBlockHandler()},
               { BlockType.CAMERA, new CameraBlockHandler() },
               { BlockType.COCKPIT, new ShipControllerHandler<IMyCockpit>()},
               { BlockType.CONNECTOR, new ConnectorBlockHandler() },
               { BlockType.DISPLAY, new TextSurfaceHandler() },
               { BlockType.DOOR, new DoorBlockHandler() },
               { BlockType.GRINDER, new FunctionalBlockHandler<IMyShipGrinder>() },
               { BlockType.GUN, new GunBlockHandler<IMyUserControllableGun>() },
               { BlockType.LIGHT, new LightBlockHandler() },
               { BlockType.MERGE, new MergeBlockHandler() },
               { BlockType.PROGRAM, new ProgramBlockHandler() },
               { BlockType.PISTON, new PistonBlockHandler() },
               { BlockType.PROJECTOR, new ProjectorBlockHandler() },
               { BlockType.REMOTE, new RemoteControlBlockHandler()},
               { BlockType.ROTOR, new RotorBlockHandler() },
               { BlockType.SOUND, new SoundBlockHandler() },
               { BlockType.SENSOR, new SensorBlockHandler() },
               { BlockType.TIMER, new FunctionalBlockHandler<IMyTimerBlock>() },
               { BlockType.THRUSTER, new ThrusterBlockHandler()},
               { BlockType.WELDER, new FunctionalBlockHandler<IMyShipWelder>() },
            };

            public static BlockHandler GetBlockHandler(BlockType blockType) {
                if (!blockHandlers.ContainsKey(blockType)) throw new Exception("Unsupported Block Type: " + blockType);
                return blockHandlers[blockType];
            }
            public static List<Object> GetBlocks(BlockType blockType, String customName) {
                return blockHandlers[blockType].GetBlocks(customName);
            }
            public static List<Object> GetBlocksInGroup(BlockType blockType, String groupName) {
                return blockHandlers[blockType].GetBlocksInGroup(groupName);
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

        public class SimpleNumericPropertySetter<T> : NumericPropertySetter<T> {
            public SimpleNumericPropertySetter(NumericPropertyGetter<T> GetValue, SetPropertyValue<T> SetValue, float delta) {
                Set = SetValue;
                SetDirection = (b,d,v) => SetValue(b,v);
                Increment = (b,v) => SetValue(b, GetValue(b) + v);
                IncrementDirection = (b,d,v) => SetValue(b, GetValue(b) + Multiply(d) * v);
                Move = (b, d) => SetValue(b, GetValue(b) + Multiply(d) * delta);
                Reverse = (b) => SetValue(b, -GetValue(b));
            }
            private float Multiply(DirectionType d) { return (d == DirectionType.UP) ? 1 : -1; }
        }

        public class NumericPropertySetter<T> {
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
            List<Object> GetBlocks(String name);
            List<Object> GetBlocksInGroup(String groupName);
            bool GetBooleanPropertyValue(Object block, BooleanPropertyType property);
            string GetStringPropertyValue(Object block, StringPropertyType property);
            float GetNumericPropertyValue(Object block, NumericPropertyType property);
            void SetBooleanPropertyValue(Object block, BooleanPropertyType property, bool value);
            void SetStringPropertyValue(Object block, StringPropertyType property, String value);
            void SetNumericPropertyValue(Object block, NumericPropertyType property, float value);
            void SetNumericPropertyValue(Object block, NumericPropertyType property, DirectionType direction, float value);
            void IncrementNumericPropertyValue(Object block, NumericPropertyType property, float deltaValue);
            void IncrementNumericPropertyValue(Object block, NumericPropertyType property, DirectionType direction, float deltaValue);
            void MoveNumericPropertyValue(Object block, NumericPropertyType property, DirectionType direction);
            void ReverseNumericPropertyValue(Object block, NumericPropertyType property);
        }

        public class FunctionalBlockHandler<T> : TerminalBlockHandler<T> where T : class, IMyFunctionalBlock {
            public FunctionalBlockHandler() : base() {
                booleanPropertyGetters.Add(BooleanPropertyType.POWER, (block) => block.Enabled);
                booleanPropertySetters.Add(BooleanPropertyType.POWER, (block, enabled) => block.Enabled = enabled);
            }
        }

        public abstract class TerminalBlockHandler<T> : BlockHandler<T> where T : class, IMyTerminalBlock {
            public override List<T> GetBlocksOfType(String name) {
                List<T> blocks = new List<T>();
                PROGRAM.GridTerminalSystem.GetBlocksOfType<T>(blocks, block => block.CustomName.ToLower().Equals(name));
                return blocks;
            }

            public override List<T> GetBlocksOfTypeInGroup(String groupName) {
                List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();
                PROGRAM.GridTerminalSystem.GetBlockGroups(blockGroups);
                IMyBlockGroup group = blockGroups.Find(g => g.Name.ToLower() == groupName);
                if (group == null) { throw new Exception("Unable to find requested block group: " + groupName); }
                List<T> blocks = new List<T>();
                group.GetBlocksOfType<T>(blocks);
                return blocks;
            }

            protected override string Name(T block) { return block.CustomName; }

            protected String GetCustomProperty(T block, String key) { return GetCustomData(block).GetValueOrDefault(key); }
            protected void SetCustomProperty(T block, String key, String value) {
                Dictionary<String, String> d = GetCustomData(block);
                d[key] = value;SaveCustomData(block, d);
            }
            protected void SaveCustomData(T block, Dictionary<String, String> data) {
                block.CustomData = String.Join("\n",data.Keys.Select(k => k + "=" + data[k] + '\n').ToList());
            }
            protected Dictionary<String, String> GetCustomData(T block) {
                List<String> keys = block.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return keys.ToDictionary(k => k.Split('=')[0], v => v.Split('=')[1]);
            }
        }

        public abstract class BlockHandler<T> : BlockHandler where T : class {
            protected Dictionary<BooleanPropertyType, BooleanPropertyGetter<T>> booleanPropertyGetters = new Dictionary<BooleanPropertyType, BooleanPropertyGetter<T>>();
            protected Dictionary<StringPropertyType, StringPropertyGetter<T>> stringPropertyGetters = new Dictionary<StringPropertyType, StringPropertyGetter<T>>();
            protected Dictionary<NumericPropertyType, NumericPropertyGetter<T>> numericPropertyGetters = new Dictionary<NumericPropertyType, NumericPropertyGetter<T>>();
            protected Dictionary<BooleanPropertyType, BooleanPropertySetter<T>> booleanPropertySetters = new Dictionary<BooleanPropertyType, BooleanPropertySetter<T>>();
            protected Dictionary<StringPropertyType, StringPropertySetter<T>> stringPropertySetters = new Dictionary<StringPropertyType, StringPropertySetter<T>>();
            protected Dictionary<NumericPropertyType, NumericPropertySetter<T>> numericPropertySetters = new Dictionary<NumericPropertyType, NumericPropertySetter<T>>();
            protected BooleanPropertyType defaultBooleanProperty = BooleanPropertyType.POWER;
            protected StringPropertyType defaultStringProperty = StringPropertyType.NAME;
            protected Dictionary<DirectionType, NumericPropertyType> defaultNumericProperties = new Dictionary<DirectionType, NumericPropertyType>();
            protected DirectionType? defaultDirection = null;

            public List<Object> GetBlocks(String name) { return GetBlocksOfType(name).Select(t => t as object).ToList(); }
            public List<Object> GetBlocksInGroup(String groupName) { return GetBlocksOfTypeInGroup(groupName).Select(t => t as object).ToList(); }

            public abstract List<T> GetBlocksOfType(String name);
            public abstract List<T> GetBlocksOfTypeInGroup(String name);

            public BooleanPropertyType GetDefaultBooleanProperty() {
                return defaultBooleanProperty;
            }
            public StringPropertyType GetDefaultStringProperty() {
                return defaultStringProperty;
            }
            public NumericPropertyType GetDefaultNumericProperty(DirectionType direction) {
                if (!defaultNumericProperties.ContainsKey(direction)) throw new Exception("This Block Does Not Have A Default Numeric Property");
                return defaultNumericProperties[direction];
            }
            public DirectionType GetDefaultDirection() {
                if (!defaultDirection.HasValue) throw new Exception("This Block Does Not Have a Default Direction");
                return defaultDirection.Value;
            }
            public bool GetBooleanPropertyValue(Object block, BooleanPropertyType property) {
                return booleanPropertyGetters[property]((T)block);
            }
            public string GetStringPropertyValue(Object block, StringPropertyType property) {
                return stringPropertyGetters[property]((T)block);
            }
            public float GetNumericPropertyValue(Object block, NumericPropertyType property) {
                return numericPropertyGetters[property]((T)block);
            }
            public void SetBooleanPropertyValue(Object block, BooleanPropertyType property, bool value) {
                Print("Setting " + Name(block) + " " + property + " to " + value);
                booleanPropertySetters[property]((T)block, value);
            }
            public void SetStringPropertyValue(Object block, StringPropertyType property, String value) {
                Print("Setting " + Name(block) + " " + property + " to " + value);
                stringPropertySetters[property]((T)block, value);
            }
            public void SetNumericPropertyValue(Object block, NumericPropertyType property, float value) {
                Print("Setting " + Name(block) + " " + property + " to " + value);
                numericPropertySetters[property].Set((T)block, value);
            }
            public void SetNumericPropertyValue(Object block, NumericPropertyType property, DirectionType direction, float value) {
                Print("Setting " + Name(block) + " " + property + " to " + value + " in " + direction + " direction");
                numericPropertySetters[property].SetDirection((T)block, direction, value);
            }
            public void IncrementNumericPropertyValue(Object block, NumericPropertyType property, float deltaValue) {
                Print("Incrementing " + Name(block) + " " + property + " by " + deltaValue);
                numericPropertySetters[property].Increment((T)block, deltaValue);
            }
            public void IncrementNumericPropertyValue(Object block, NumericPropertyType property, DirectionType direction, float deltaValue) {
                Print("Incrementing " + Name(block) + " " + property + " by " + deltaValue + " in " + direction + " direction");
                numericPropertySetters[property].IncrementDirection((T)block, direction, deltaValue);
            }
            public void MoveNumericPropertyValue(Object block, NumericPropertyType property, DirectionType direction) {
                Print("Moving " + Name(block) + " " + property + " in " + direction + " direction");
                numericPropertySetters[property].Move((T)block, direction);
            }
            public void ReverseNumericPropertyValue(Object block, NumericPropertyType property) {
                Print("Reversing " + Name(block) + " " + property);
                numericPropertySetters[property].Reverse((T)block);
            }
            private string Name(object block) {
                return Name((T)block);
            }
            protected abstract string Name(T block);
        }
    }
}
