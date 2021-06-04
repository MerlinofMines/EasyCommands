using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public static class BlockHandlerRegistry {
            static readonly Dictionary<Block, BlockHandler> blockHandlers = new Dictionary<Block, BlockHandler> {
                { Block.AIRVENT, new AirVentBlockHandler()},
                { Block.ANTENNA, new AntennaBlockHandler()},
                { Block.ASSEMBLER, new AssemblerBlockHandler()},
                { Block.BATTERY, new BatteryBlockHandler()},
                { Block.BEACON, new BeaconBlockHandler()},
                { Block.CAMERA, new CameraBlockHandler() },
                { Block.CARGO, new CargoHandler() },
                { Block.COCKPIT, new ShipControllerHandler<IMyCockpit>() },
                { Block.COLLECTOR, new FunctionalBlockHandler<IMyCollector>() },
                { Block.CONNECTOR, new ConnectorBlockHandler() },
                { Block.DECOY, new FunctionalBlockHandler<IMyDecoy>() },
                { Block.DETECTOR, new OreDetectorHandler() },
                { Block.DISPLAY, new TextSurfaceHandler() },
                { Block.DRILL, new FunctionalBlockHandler<IMyShipDrill>() },
                { Block.DOOR, new DoorBlockHandler() },
                { Block.EJECTOR, new EjectorBlockHandler() },
                { Block.ENGINE, new EngineBlockHandler() },
                { Block.GEAR, new LandingGearHandler() },
                { Block.GENERATOR, new GasGeneratorHandler()},
                { Block.GRAVITY_GENERATOR, new GravityGeneratorBlockHandler() },
                { Block.GRAVITY_SPHERE, new SphericalGravityGeneratorBlockHandler() },
                { Block.GRINDER, new FunctionalBlockHandler<IMyShipGrinder>() },
                { Block.GUN, new GunBlockHandler<IMyUserControllableGun>() },
                { Block.GYROSCOPE, new GyroscopeBlockHandler() },
                { Block.HINGE, new RotorBlockHandler(IsHinge) },
                { Block.JUMPDRIVE, new JumpDriveBlockHandler() },
                { Block.LASER_ANTENNA, new LaserAntennaBlockHandler() },
                { Block.LIGHT, new LightBlockHandler() },
                { Block.MERGE, new MergeBlockHandler() },
                { Block.PARACHUTE, new ParachuteBlockHandler() },
                { Block.PROGRAM, new ProgramBlockHandler() },
                { Block.PISTON, new PistonBlockHandler() },
                { Block.PROJECTOR, new ProjectorBlockHandler() },
                { Block.REACTOR, new FunctionalBlockHandler<IMyReactor>()},
                { Block.REMOTE, new RemoteControlBlockHandler()},
                { Block.ROTOR, new RotorBlockHandler(b => !IsHinge(b)) },
                { Block.SORTER, new SorterBlockerHandler() },
                { Block.SOUND, new SoundBlockHandler() },
                { Block.SENSOR, new SensorBlockHandler() },
                { Block.SUSPENSION, new WheelSuspensionBlockHandler() },
                { Block.TANK, new GasTankBlockHandler() },
                { Block.TIMER, new TimerBlockHandler() },
                { Block.THRUSTER, new ThrusterBlockHandler()},
                { Block.TURRET, new TurretBlockHandler<IMyLargeTurretBase>()},
                { Block.WARHEAD, new WarheadBlockHandler() },
                { Block.WELDER, new FunctionalBlockHandler<IMyShipWelder>() },
            };

            public static BlockHandler GetBlockHandler(Block blockType) {
                if (!blockHandlers.ContainsKey(blockType)) throw new Exception("Unsupported Block Type: " + blockType);
                return blockHandlers[blockType];
            }

            public static List<Object> GetBlocks(Block blockType, Func<IMyTerminalBlock, bool> selector) {
                return blockHandlers[blockType].GetBlocks(selector);
            }

            public static List<Object> GetBlocksInGroup(Block blockType, String groupName) {
                return blockHandlers[blockType].GetBlocksInGroup(groupName);
            }
        }

        //Property Getters
        public delegate Primitive GetProperty<T>(T block, PropertySupplier property);
        public delegate Primitive GetPropertyDirection<T>(T block, PropertySupplier property, Direction direction);
        public delegate void SetProperty<T>(T block, PropertySupplier property, Primitive value);
        public delegate void SetPropertyDirection<T>(T block, PropertySupplier property, Direction direction, Primitive value);
        public delegate void IncrementProperty<T>(T block, PropertySupplier property, Primitive deltaValue);
        public delegate void IncrementPropertyDirection<T>(T block, PropertySupplier property, Direction direction, Primitive deltaValue);
        public delegate void MovePropertyValue<T>(T block, PropertySupplier property, Direction direction);
        public delegate void ReversePropertyValue<T>(T block, PropertySupplier property);

        //Getters
        public delegate bool GetBooleanProperty<T>(T block);
        public delegate string GetStringProperty<T>(T block);
        public delegate float GetNumericProperty<T>(T block);
        public delegate float GetNumericPropertyDirection<T>(T block, Direction direction);
        public delegate Vector3D GetVectorProperty<T>(T block);
        public delegate Color GetColorProperty<T>(T block);

        //Setters
        public delegate void SetBooleanProperty<T>(T block, bool value);
        public delegate void SetStringProperty<T>(T block, String value);
        public delegate void SetNumericProperty<T>(T block, float value);
        public delegate void SetNumericPropertyDirection<T>(T block, Direction direction, float value);
        public delegate void SetVectorProperty<T>(T block, Vector3D value);
        public delegate void SetColorProperty<T>(T block, Color value);

        public class PropertyHandler<T> {
            public GetProperty<T> Get;
            public GetPropertyDirection<T> GetDirection;
            public SetProperty<T> Set;
            public SetPropertyDirection<T> SetDirection;
            public IncrementProperty<T> Increment;
            public IncrementPropertyDirection<T> IncrementDirection;
            public MovePropertyValue<T> Move;
            public ReversePropertyValue<T> Reverse;
        }

        public class SimplePropertyHandler<T> : PropertyHandler<T> {
            public SimplePropertyHandler(GetProperty<T> Get, SetProperty<T> Set, Primitive delta) {
                this.Get = Get;
                this.Set = Set;
                SetDirection = (b, p, d, v) => Set(b, p, v);
                Increment = (b, p, v) => Set(b, p, Get(b, p).Plus(v));
                IncrementDirection = (b, p, d, v) => Increment(b, p, Multiply(v, d));
                Move = (b, p, d) => Set(b, p, Get(b, p).Plus(Multiply(delta,d)));
                Reverse = (b, p) => Set(b, p, Get(b, p).Not());
            }
            private Primitive Multiply(Primitive p, Direction d) { return (d == Direction.DOWN) ? p.Not() : p; }
        }

        public class SimpleNumericPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleNumericPropertyHandler(GetNumericProperty<T> GetValue, SetNumericProperty<T> SetValue, float delta)
                : base((b, p) => new NumberPrimitive(GetValue(b)), (b, p, v)=> {
                    SetValue(b, CastNumber(v).GetTypedValue());
                }, new NumberPrimitive(delta)) {
            }
        }

        public class SimpleNumericDirectionPropertyHandler<T> : PropertyHandler<T> {
            public SimpleNumericDirectionPropertyHandler(GetNumericPropertyDirection<T> GetValue, SetNumericPropertyDirection<T> SetValue, Direction defaultDirection) {
                Get = (b, p) => GetDirection(b, p, defaultDirection);
                GetDirection = (b, p, d) => new NumberPrimitive(GetValue(b,d));
                Set = (b, p, v) => SetDirection(b, p, defaultDirection, v);
                SetDirection = (b, p, d, v) => SetValue(b, d, CastNumber(v).GetTypedValue());
                IncrementDirection = (b, p, d, v) => SetDirection(b, p, d, GetDirection(b, p, d).Plus(v));
                Increment = (b, p, v) => IncrementDirection(b, p, defaultDirection, v);
            }
        }


        public class SimpleStringPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleStringPropertyHandler(GetStringProperty<T> GetValue, SetStringProperty<T> SetValue)
                : base((b, p) => new StringPrimitive(GetValue(b)), (b, p, v) => {
                    SetValue(b,CastString(v).GetTypedValue());
                }, new StringPrimitive("")) {
            }
        }

        public class SimpleBooleanPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleBooleanPropertyHandler(GetBooleanProperty<T> GetValue, SetBooleanProperty<T> SetValue)
                : base((b, p) => new BooleanPrimitive(GetValue(b)), (b, p, v) => {
                    SetValue(b, CastBoolean(v).GetTypedValue());
                }, new BooleanPrimitive(true)) {
            }
        }

        public class SimpleVectorPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleVectorPropertyHandler(GetVectorProperty<T> GetValue, SetVectorProperty<T> SetValue)
                : base((b, p) => new VectorPrimitive(GetValue(b)), (b, p, v) => {
                    SetValue(b, CastVector(v).GetTypedValue());
                }, new VectorPrimitive(Vector3D.Zero)) {
            }
        }

        public class SimpleColorPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleColorPropertyHandler(GetColorProperty<T> GetValue, SetColorProperty<T> SetValue)
                : base((b, p) => new ColorPrimitive(GetValue(b)), (b, p, v) => {
                    SetValue(b, CastColor(v).GetTypedValue());
                }, new ColorPrimitive(new Color(10,10,10))) {
            }
        }

        public class PropertyValueNumericPropertyHandler<T> : SimpleNumericPropertyHandler<T> where T : class, IMyTerminalBlock {
            public PropertyValueNumericPropertyHandler(String propertyName, float delta) : base((b) => b.GetValueFloat(propertyName), (b, v) => b.SetValueFloat(propertyName, v), delta) {
            }
        }

        public class DirectionVectorPropertyHandler<T> : PropertyHandler<T> where T : class, IMyTerminalBlock {
            public DirectionVectorPropertyHandler() {
                Get = (b, p) => GetDirection(b, p, Direction.FORWARD);
                GetDirection = (b, p, d) => {
                    Vector3D vector;
                    MatrixD b2w = GetBlock2WorldTransform(b);
                    switch (d) {
                        case Direction.FORWARD:
                            vector = b2w.Forward;
                            break;
                        case Direction.BACKWARD:
                            vector = b2w.Backward;
                            break;
                        case Direction.UP:
                            vector = b2w.Up;
                            break;
                        case Direction.DOWN:
                            vector = b2w.Down;
                            break;
                        case Direction.LEFT:
                            vector = b2w.Left;
                            break;
                        case Direction.RIGHT:
                            vector = b2w.Right;
                            break;
                        default: throw new Exception("Cannot get direction value for direction: " + d);
                    }
                    return new VectorPrimitive(vector/vector.Length());//Return normalized vector
                };
                Set = (b, p, v) => { };
                SetDirection = (b, p, d, v) => { };
                Increment = (b, p, v) => { };
                IncrementDirection = (b, p, d, v) => { };
                Move = (b, p, d) => { };
                Reverse = (b, p) => { };
            }

             //Taken from https://forum.keenswh.com/threads/how-do-i-get-the-world-position-and-rotation-of-a-ship.7363867/
            MatrixD GetGrid2WorldTransform(IMyCubeGrid grid) {
                Vector3D origin = grid.GridIntegerToWorld(new Vector3I(0, 0, 0));
                Vector3D plusY = grid.GridIntegerToWorld(new Vector3I(0, 1, 0)) - origin;
                Vector3D plusZ = grid.GridIntegerToWorld(new Vector3I(0, 0, 1)) - origin;
                return MatrixD.CreateScale(grid.GridSize) * MatrixD.CreateWorld(origin, -plusZ, plusY);
            }

            MatrixD GetBlock2WorldTransform(IMyCubeBlock blk) {
                Matrix blk2grid;
                blk.Orientation.GetMatrix(out blk2grid);
                return blk2grid *
                       MatrixD.CreateTranslation(new Vector3D(blk.Min + blk.Max) / 2.0) *
                       GetGrid2WorldTransform(blk.CubeGrid);
            }
        }

        public interface BlockHandler {
            PropertySupplier GetDefaultProperty(Return type);
            PropertySupplier GetDefaultProperty(Direction direction);
            Direction GetDefaultDirection();
            List<Object> GetBlocks(Func<IMyTerminalBlock, bool> selector);
            List<Object> GetBlocksInGroup(String groupName);

            Primitive GetPropertyValue(Object block, PropertySupplier property);
            Primitive GetPropertyValue(Object block, PropertySupplier property, Direction direction);

            void SetPropertyValue(Object block, PropertySupplier property, Primitive value);
            void SetPropertyValue(Object block, PropertySupplier property, Direction direction, Primitive value);
            void IncrementPropertyValue(Object block, PropertySupplier property, Primitive value);
            void IncrementPropertyValue(Object block, PropertySupplier property, Direction direction, Primitive value);
            void MoveNumericPropertyValue(Object block, PropertySupplier property, Direction direction);
            void ReverseNumericPropertyValue(Object block, PropertySupplier property);
        }

        public abstract class MultiInstanceBlockHandler<T> : BlockHandler<T> where T : class {
            public override List<T> GetBlocksOfType(Func<IMyTerminalBlock, bool> selector) {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                PROGRAM.GridTerminalSystem.GetBlocksOfType(blocks, selector);

                List<T> instances = new List<T>();
                blocks.ForEach((b) => GetInstances(b, instances));
                return instances;
            }

            public override List<T> GetBlocksOfTypeInGroup(String groupName) {
                List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();
                PROGRAM.GridTerminalSystem.GetBlockGroups(blockGroups);
                IMyBlockGroup group = blockGroups.Find(g => g.Name == groupName);
                if (group == null) { throw new Exception("Unable to find requested block group: " + groupName); }
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                group.GetBlocksOfType<IMyTerminalBlock>(blocks);
                List<T> instances = new List<T>();
                blocks.ForEach((b) => GetInstances(b, instances));
                return instances;
            }

            public abstract void GetInstances(IMyTerminalBlock block, List<T> instances);
        }

        public class FunctionalBlockHandler<T> : TerminalBlockHandler<T> where T : class, IMyFunctionalBlock {
            public FunctionalBlockHandler() : base() {
                AddPropertyHandler(Property.POWER, new SimpleBooleanPropertyHandler<T>((block) => block.Enabled, (block, enabled) => block.Enabled = enabled));
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.POWER;
            }
        }

        public abstract class TerminalBlockHandler<T> : BlockHandler<T> where T : class, IMyTerminalBlock {

            public TerminalBlockHandler() {
                AddPropertyHandler(Property.POSITION, new SimpleVectorPropertyHandler<T>((block) => block.GetPosition(), (block, position) => { }));
                AddPropertyHandler(Property.DIRECTION, new DirectionVectorPropertyHandler<T>());
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.POSITION;
            }

            public override List<T> GetBlocksOfType(Func<IMyTerminalBlock, bool> selector) {
                List<T> blocks = new List<T>();
                PROGRAM.GridTerminalSystem.GetBlocksOfType<T>(blocks, selector);
                return blocks;
            }

            public override List<T> GetBlocksOfTypeInGroup(String groupName) {
                List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();
                PROGRAM.GridTerminalSystem.GetBlockGroups(blockGroups);
                IMyBlockGroup group = blockGroups.Find(g => g.Name.Equals(groupName));
                if (group == null) { throw new Exception("Unable to find requested block group: " + groupName); }
                List<T> blocks = new List<T>();
                group.GetBlocksOfType<T>(blocks);
                return blocks;
            }

            protected override string Name(T block) { return block.CustomName; }

            protected String GetCustomProperty(T block, String key) { return GetCustomData(block).GetValueOrDefault(key, null); }
            protected void SetCustomProperty(T block, String key, String value) {
                Dictionary<String, String> d = GetCustomData(block);
                d[key] = value;SaveCustomData(block, d);
            }
            protected void DeleteCustomProperty(T block, String key) {
                Dictionary<String, String> d = GetCustomData(block);
                if(d.ContainsKey(key)) d.Remove(key);
                SaveCustomData(block, d);
            }
            protected void SaveCustomData(T block, Dictionary<String, String> data) {
                block.CustomData = String.Join("\n",data.Keys.Select(k => k + "=" + data[k]).ToList());
            }
            protected Dictionary<String, String> GetCustomData(T block) {
                List<String> keys = block.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return keys.ToDictionary(k => k.Split('=')[0], v => v.Split('=')[1]);
            }
        }

        public abstract class BlockHandler<T> : BlockHandler where T : class {
            protected Dictionary<String, PropertyHandler<T>> propertyHandlers = new Dictionary<String, PropertyHandler<T>>();
            protected Dictionary<Return, Property> defaultPropertiesByPrimitive = new Dictionary<Return, Property>();
            protected Dictionary<Direction, Property> defaultPropertiesByDirection = new Dictionary<Direction, Property>();
            protected Direction? defaultDirection = null;

            public List<Object> GetBlocks(Func<IMyTerminalBlock, bool> selector) { return GetBlocksOfType(selector).Select(t => t as object).ToList(); }
            public List<Object> GetBlocksInGroup(String groupName) { return GetBlocksOfTypeInGroup(groupName).Select(t => t as object).ToList(); }

            public abstract List<T> GetBlocksOfType(Func<IMyTerminalBlock, bool> selector);
            public abstract List<T> GetBlocksOfTypeInGroup(String name);

            public Direction GetDefaultDirection() {
                if (!defaultDirection.HasValue) throw new Exception(GetType() + " Does Not Have a Default Direction");
                return defaultDirection.Value;
            }
            public PropertySupplier GetDefaultProperty(Direction direction) {
                if (!defaultPropertiesByDirection.ContainsKey(direction)) throw new Exception(GetType() + " Does Not Have A Default Property for Direction: " + direction);
                return new PropertySupplier(defaultPropertiesByDirection[direction]);
            }
            public PropertySupplier GetDefaultProperty(Return type) {
                if (!defaultPropertiesByPrimitive.ContainsKey(type)) throw new Exception(GetType() + " Does Not Have A Default Property for Primitive: " + type);
                return new PropertySupplier(defaultPropertiesByPrimitive[type]);
            }
            public Primitive GetPropertyValue(object block, PropertySupplier property) {
                return propertyHandlers[property.propertyType()].Get((T)block, property);
            }
            public Primitive GetPropertyValue(object block, PropertySupplier property, Direction direction) {
                return propertyHandlers[property.propertyType()].GetDirection((T)block, property, direction);
            }
            public void SetPropertyValue(Object block, PropertySupplier property, Primitive value) {
                Debug("Setting " + Name(block) + " " + property + " to " + value.GetValue());
                propertyHandlers[property.propertyType()].Set((T)block, property, value);
            }
            public void SetPropertyValue(Object block, PropertySupplier property, Direction direction, Primitive value) {
                Debug("Setting " + Name(block) + " " + property + " to " + value.GetValue() + " in " + direction + " direction");
                propertyHandlers[property.propertyType()].SetDirection((T)block, property, direction, value);
            }
            public void IncrementPropertyValue(Object block, PropertySupplier property, Primitive value) {
                Debug("Incrementing " + Name(block) + " " + property + " by " + value.GetValue());
                propertyHandlers[property.propertyType()].Increment((T)block, property, value);
            }
            public void IncrementPropertyValue(Object block, PropertySupplier property, Direction direction, Primitive value) {
                Debug("Incrementing " + Name(block) + " " + property + " by " + value.GetValue() + " in " + direction + " direction");
                propertyHandlers[property.propertyType()].IncrementDirection((T)block, property, direction, value);
            }
            public void MoveNumericPropertyValue(Object block, PropertySupplier property, Direction direction) {
                Debug("Moving " + Name(block) + " " + property + " in " + direction + " direction");
                propertyHandlers[property.propertyType()].Move((T)block, property, direction);
            }
            public void ReverseNumericPropertyValue(Object block, PropertySupplier property) {
                Debug("Reversing " + Name(block) + " " + property);
                propertyHandlers[property.propertyType()].Reverse((T)block, property);
            }

            private string Name(object block) {
                return Name((T)block);
            }
            protected abstract string Name(T block);

            protected void AddBooleanHandler(Property property, GetBooleanProperty<T> Get) {
                AddBooleanHandler(property, Get, (b, v) => { });
            }

            protected void AddBooleanHandler(Property property, GetBooleanProperty<T> Get, SetBooleanProperty<T> Set) {
                propertyHandlers[property + ""] = new SimpleBooleanPropertyHandler<T>(Get, Set);
            }

            protected void AddPropertyHandler(Property property, PropertyHandler<T> handler) {
                propertyHandlers[property + ""] = handler;
            }

            protected void AddPropertyHandler(ValueProperty property, PropertyHandler<T> handler) {
                propertyHandlers[property + ""] = handler;
            }

            protected void AddStringHandler(Property property, GetStringProperty<T> Get, SetStringProperty<T> Set) {
                propertyHandlers[property + ""] = new SimpleStringPropertyHandler<T>(Get, Set);
            }

            protected void AddNumericHandler(Property property, GetNumericProperty<T> Get) {
                propertyHandlers[property + ""] = new SimpleNumericPropertyHandler<T>(Get, (b, v) => { }, 0);
            }

            protected void AddNumericHandler(Property property, GetNumericProperty<T> Get, SetNumericProperty<T> Set, float delta) {
                propertyHandlers[property + ""] = new SimpleNumericPropertyHandler<T>(Get, Set, delta);
            }

            protected void AddVectorHandler(Property property, GetVectorProperty<T> Get) {
                propertyHandlers[property + ""] = new SimpleVectorPropertyHandler<T>(Get, (b,v) => { });
            }

            protected void AddVectorHandler(Property property, GetVectorProperty<T> Get, SetVectorProperty<T> Set) {
                propertyHandlers[property + ""] = new SimpleVectorPropertyHandler<T>(Get, Set);
            }

            protected void AddColorHandler(Property property, GetColorProperty<T> Get, SetColorProperty<T> Set) {
                propertyHandlers[property + ""] = new SimpleColorPropertyHandler<T>(Get, Set);
            }
        }

        /// <summary>
        ///Provides a consistent way to get the most accurate position for a detected entity.
        ///Not all detected entities will have a HitPosition.  Attempt to use HitPosition first.  If not present, use Position instead.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static Vector3D GetPosition(MyDetectedEntityInfo entity) {
            return entity.HitPosition.GetValueOrDefault(entity.Position);
        }
    }
}
