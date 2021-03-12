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
               { BlockType.BATTERY, new BatteryBlockHandler()},
               { BlockType.BEACON, new BeaconBlockHandler()},
               { BlockType.CAMERA, new CameraBlockHandler() },
               { BlockType.COCKPIT, new ShipControllerHandler<IMyCockpit>()},
               { BlockType.CONNECTOR, new ConnectorBlockHandler() },
               { BlockType.DETECTOR, new OreDetectorHandler() },
               { BlockType.DISPLAY, new TextSurfaceHandler() },
               { BlockType.DRILL, new FunctionalBlockHandler<IMyShipDrill>() },
               { BlockType.DOOR, new DoorBlockHandler() },
               { BlockType.ENGINE, new FunctionalBlockHandler<IMyPowerProducer>() },
               { BlockType.GEAR, new LandingGearHandler() },
               { BlockType.GENERATOR, new GasGeneratorHandler()},
               { BlockType.GRINDER, new FunctionalBlockHandler<IMyShipGrinder>() },
               { BlockType.GUN, new GunBlockHandler<IMyUserControllableGun>() },
               { BlockType.LIGHT, new LightBlockHandler() },
               { BlockType.MERGE, new MergeBlockHandler() },
               { BlockType.PARACHUTE, new ParachuteBlockHandler() },
               { BlockType.PROGRAM, new ProgramBlockHandler() },
               { BlockType.PISTON, new PistonBlockHandler() },
               { BlockType.PROJECTOR, new ProjectorBlockHandler() },
               { BlockType.REACTOR, new FunctionalBlockHandler<IMyReactor>()},
               { BlockType.REMOTE, new RemoteControlBlockHandler()},
               { BlockType.ROTOR, new RotorBlockHandler() },
               { BlockType.SOUND, new SoundBlockHandler() },
               { BlockType.SENSOR, new SensorBlockHandler() },
               { BlockType.SUSPENSION, new WheelSuspensionBlockHandler() },
               { BlockType.TANK, new GasTankBlockHandler() },
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
        public delegate float NumericPropertyGetterDirection<T>(T block, DirectionType direction);

        //Property Setters
        public delegate void StringPropertySetter<T>(T block, String value);
        public delegate void BooleanPropertySetter<T>(T block, bool value);
        public delegate void SetPropertyValue<T>(T block, float value);
        public delegate void SetPropertyValueDirection<T>(T block, DirectionType direction, float value);
        public delegate void IncrementPropertyValue<T>(T block, float deltaValue);
        public delegate void IncrementPropertyValueDirection<T>(T block, DirectionType direction, float deltaValue);
        public delegate void MovePropertyValue<T>(T block, DirectionType direction);
        public delegate void ReversePropertyValue<T>(T block);

        public class SimpleBooleanPropertyHandler<T> : PropertyHandler<T> {
            public SimpleBooleanPropertyHandler(BooleanPropertyGetter<T> GetValue, BooleanPropertySetter<T> SetValue) {
                GetBoolean = GetValue;
                SetBoolean = SetValue;
                GetString = (b) => GetBoolean(b).ToString().ToLower();
                SetString = (b, v) => SetBoolean(b, Boolean.Parse(v.ToLower()));
                GetNumeric = (b) => GetBoolean(b) ? 1 : 0;
                Set = (b, v) => SetBoolean(b, v > 0);
                SetDirection = (b, d, v) => Set(b, v);
                Reverse = (b) => SetValue(b, !GetValue(b));
            }
        }

        public class SimpleStringPropertyHandler<T> : PropertyHandler<T> {
            public SimpleStringPropertyHandler(StringPropertyGetter<T> GetValue, StringPropertySetter<T> SetValue) {
                GetString = GetValue;
                SetString = SetValue;
                GetNumeric = (b) => GetValue(b).Length;
                Reverse = (b) => SetValue(b, new String(GetValue(b).Reverse().ToArray()));
            }
        }

        public class SimpleNumericPropertyHandler<T> : PropertyHandler<T> {
            public SimpleNumericPropertyHandler(NumericPropertyGetter<T> GetValue, SetPropertyValue<T> SetValue, float delta) {
                GetNumeric = GetValue;
                GetString = (b) => GetValue(b).ToString();
                GetBoolean = (b) => GetValue(b) > 0;
                Set = SetValue;
                SetDirection = (b,d,v) => SetValue(b,v);
                Increment = (b,v) => SetValue(b, GetValue(b) + v);
                IncrementDirection = (b,d,v) => SetValue(b, GetValue(b) + Multiply(d) * v);
                Move = (b, d) => SetValue(b, GetValue(b) + Multiply(d) * delta);
                Reverse = (b) => SetValue(b, -GetValue(b));
            }
            private float Multiply(DirectionType d) { return (d == DirectionType.UP) ? 1 : -1; }
        }

        public class PropertyValueNumericPropertyHandler<T> : SimpleNumericPropertyHandler<T> where T : class, IMyTerminalBlock {
            public PropertyValueNumericPropertyHandler(String propertyName, float delta) : base((b)=>b.GetValueFloat(propertyName), (b,v)=>b.SetValueFloat(propertyName, v), delta) {
            }
        }

        public class PropertyHandler<T> {
            public StringPropertyGetter<T> GetString;
            public StringPropertySetter<T> SetString;
            public BooleanPropertyGetter<T> GetBoolean;
            public BooleanPropertySetter<T> SetBoolean;
            public NumericPropertyGetter<T> GetNumeric;
            public NumericPropertyGetterDirection<T> GetNumericDirection;
            public SetPropertyValue<T> Set;
            public SetPropertyValueDirection<T> SetDirection;
            public IncrementPropertyValue<T> Increment;
            public IncrementPropertyValueDirection<T> IncrementDirection;
            public MovePropertyValue<T> Move;
            public ReversePropertyValue<T> Reverse;
        }

         public interface BlockHandler {
            PropertyType GetDefaultBooleanProperty();
            PropertyType GetDefaultStringProperty();
            PropertyType GetDefaultNumericProperty(DirectionType direction);
            DirectionType GetDefaultDirection();
            List<Object> GetBlocks(String name);
            List<Object> GetBlocksInGroup(String groupName);
            bool GetBooleanPropertyValue(Object block, PropertyType property);
            string GetStringPropertyValue(Object block, PropertyType property);
            float GetNumericPropertyValue(Object block, PropertyType property);
            float GetNumericPropertyValue(Object block, PropertyType property, DirectionType direction);
            void SetBooleanPropertyValue(Object block, PropertyType property, bool value);
            void SetStringPropertyValue(Object block, PropertyType property, String value);
            void SetNumericPropertyValue(Object block, PropertyType property, float value);
            void SetNumericPropertyValue(Object block, PropertyType property, DirectionType direction, float value);
            void IncrementNumericPropertyValue(Object block, PropertyType property, float deltaValue);
            void IncrementNumericPropertyValue(Object block, PropertyType property, DirectionType direction, float deltaValue);
            void MoveNumericPropertyValue(Object block, PropertyType property, DirectionType direction);
            void ReverseNumericPropertyValue(Object block, PropertyType property);
        }

        public class FunctionalBlockHandler<T> : TerminalBlockHandler<T> where T : class, IMyFunctionalBlock {
            public FunctionalBlockHandler() : base() {
                AddPropertyHandler(PropertyType.POWER, new SimpleBooleanPropertyHandler<T>((block) => block.Enabled, (block, enabled) => block.Enabled = enabled));
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
            protected Dictionary<PropertyType, PropertyHandler<T>> propertyHandlers = new Dictionary<PropertyType, PropertyHandler<T>>();
            protected PropertyType defaultBooleanProperty = PropertyType.POWER;
            protected PropertyType defaultStringProperty = PropertyType.NAME;
            protected Dictionary<DirectionType, PropertyType> defaultNumericProperties = new Dictionary<DirectionType, PropertyType>();
            protected DirectionType? defaultDirection = null;

            public List<Object> GetBlocks(String name) { return GetBlocksOfType(name).Select(t => t as object).ToList(); }
            public List<Object> GetBlocksInGroup(String groupName) { return GetBlocksOfTypeInGroup(groupName).Select(t => t as object).ToList(); }

            public abstract List<T> GetBlocksOfType(String name);
            public abstract List<T> GetBlocksOfTypeInGroup(String name);

            public PropertyType GetDefaultBooleanProperty() {
                return defaultBooleanProperty;
            }
            public PropertyType GetDefaultStringProperty() {
                return defaultStringProperty;
            }
            public PropertyType GetDefaultNumericProperty(DirectionType direction) {
                if (!defaultNumericProperties.ContainsKey(direction)) throw new Exception("This Block Does Not Have A Default Numeric Property");
                return defaultNumericProperties[direction];
            }
            public DirectionType GetDefaultDirection() {
                if (!defaultDirection.HasValue) throw new Exception("This Block Does Not Have a Default Direction");
                return defaultDirection.Value;
            }
            public bool GetBooleanPropertyValue(Object block, PropertyType property) {
                return propertyHandlers[property].GetBoolean((T)block);
            }
            public string GetStringPropertyValue(Object block, PropertyType property) {
                return propertyHandlers[property].GetString((T)block);
            }
            public float GetNumericPropertyValue(Object block, PropertyType property) {
                return propertyHandlers[property].GetNumeric((T)block);
            }
            public float GetNumericPropertyValue(Object block, PropertyType property, DirectionType direction) {
                return propertyHandlers[property].GetNumericDirection((T)block, direction);
            }
            public void SetBooleanPropertyValue(Object block, PropertyType property, bool value) {
                Print("Setting " + Name(block) + " " + property + " to " + value);
                propertyHandlers[property].SetBoolean((T)block, value);
            }
            public void SetStringPropertyValue(Object block, PropertyType property, String value) {
                Print("Setting " + Name(block) + " " + property + " to " + value);
                propertyHandlers[property].SetString((T)block, value);
            }
            public void SetNumericPropertyValue(Object block, PropertyType property, float value) {
                Print("Setting " + Name(block) + " " + property + " to " + value);
                propertyHandlers[property].Set((T)block, value);
            }
            public void SetNumericPropertyValue(Object block, PropertyType property, DirectionType direction, float value) {
                Print("Setting " + Name(block) + " " + property + " to " + value + " in " + direction + " direction");
                propertyHandlers[property].SetDirection((T)block, direction, value);
            }
            public void IncrementNumericPropertyValue(Object block, PropertyType property, float deltaValue) {
                Print("Incrementing " + Name(block) + " " + property + " by " + deltaValue);
                propertyHandlers[property].Increment((T)block, deltaValue);
            }
            public void IncrementNumericPropertyValue(Object block, PropertyType property, DirectionType direction, float deltaValue) {
                Print("Incrementing " + Name(block) + " " + property + " by " + deltaValue + " in " + direction + " direction");
                propertyHandlers[property].IncrementDirection((T)block, direction, deltaValue);
            }
            public void MoveNumericPropertyValue(Object block, PropertyType property, DirectionType direction) {
                Print("Moving " + Name(block) + " " + property + " in " + direction + " direction");
                propertyHandlers[property].Move((T)block, direction);
            }
            public void ReverseNumericPropertyValue(Object block, PropertyType property) {
                Print("Reversing " + Name(block) + " " + property);
                propertyHandlers[property].Reverse((T)block);
            }
            private string Name(object block) {
                return Name((T)block);
            }
            protected abstract string Name(T block);

            protected void AddBooleanHandler(PropertyType property, BooleanPropertyGetter<T> Get) {
                AddBooleanHandler(property, Get, (b, v) => { });
            }

            protected void AddBooleanHandler(PropertyType property, BooleanPropertyGetter<T> Get, BooleanPropertySetter<T> Set) {
                propertyHandlers[property] = new SimpleBooleanPropertyHandler<T>(Get, Set);
            }

            protected void AddPropertyHandler(PropertyType property, PropertyHandler<T> handler) {
                propertyHandlers[property] = handler;
            }

            protected void AddStringHandler(PropertyType property, StringPropertyGetter<T> Get, StringPropertySetter<T> Set) {
                propertyHandlers[property] = new SimpleStringPropertyHandler<T>(Get, Set);
            }

            protected void AddNumericHandler(PropertyType property, NumericPropertyGetter<T> Get) {
                propertyHandlers[property] = new SimpleNumericPropertyHandler<T>(Get, (b, v) => { }, 0);
            }

            protected void AddNumericHandler(PropertyType property, NumericPropertyGetter<T> Get, SetPropertyValue<T> Set, float delta) {
                propertyHandlers[property] = new SimpleNumericPropertyHandler<T>(Get, Set, delta);
            }
        }
    }
}
