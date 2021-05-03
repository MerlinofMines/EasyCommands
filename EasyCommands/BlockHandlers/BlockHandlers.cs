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
                { Block.BATTERY, new BatteryBlockHandler()},
                { Block.BEACON, new BeaconBlockHandler()},
                { Block.CAMERA, new CameraBlockHandler() },
                { Block.COCKPIT, new ShipControllerHandler<IMyCockpit>()},
                { Block.CONNECTOR, new ConnectorBlockHandler() },
                { Block.DETECTOR, new OreDetectorHandler() },
                { Block.DISPLAY, new TextSurfaceHandler() },
                { Block.DRILL, new FunctionalBlockHandler<IMyShipDrill>() },
                { Block.DOOR, new DoorBlockHandler() },
                { Block.ENGINE, new FunctionalBlockHandler<IMyPowerProducer>() },
                { Block.GEAR, new LandingGearHandler() },
                { Block.GENERATOR, new GasGeneratorHandler()},
                { Block.GRAVITY_GENERATOR, new GravityGeneratorBlockHandler() },
                { Block.GRAVITY_SPHERE, new SphericalGravityGeneratorBlockHandler() },
                { Block.GRINDER, new FunctionalBlockHandler<IMyShipGrinder>() },
                { Block.GUN, new GunBlockHandler<IMyUserControllableGun>() },
                { Block.GYROSCOPE, new GyroscopeBlockHandler() },
                { Block.LIGHT, new LightBlockHandler() },
                { Block.MERGE, new MergeBlockHandler() },
                { Block.PARACHUTE, new ParachuteBlockHandler() },
                { Block.PROGRAM, new ProgramBlockHandler() },
                { Block.PISTON, new PistonBlockHandler() },
                { Block.PROJECTOR, new ProjectorBlockHandler() },
                { Block.REACTOR, new FunctionalBlockHandler<IMyReactor>()},
                { Block.REMOTE, new RemoteControlBlockHandler()},
                { Block.ROTOR, new RotorBlockHandler() },
                { Block.SORTER, new SorterBlockerHandler() },
                { Block.SOUND, new SoundBlockHandler() },
                { Block.SENSOR, new SensorBlockHandler() },
                { Block.SUSPENSION, new WheelSuspensionBlockHandler() },
                { Block.TANK, new GasTankBlockHandler() },
                { Block.TIMER, new FunctionalBlockHandler<IMyTimerBlock>() },
                { Block.THRUSTER, new ThrusterBlockHandler()},
                { Block.TURRET, new TurretBlockHandler<IMyLargeTurretBase>()},
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
        public delegate Primitive GetProperty<T>(T block);
        public delegate Primitive GetPropertyDirection<T>(T block, Direction direction);
        public delegate void SetProperty<T>(T block, Primitive value);
        public delegate void SetPropertyDirection<T>(T block, Direction direction, Primitive value);
        public delegate void IncrementProperty<T>(T block, Primitive deltaValue);
        public delegate void IncrementPropertyDirection<T>(T block, Direction direction, Primitive deltaValue);
        public delegate void MovePropertyValue<T>(T block, Direction direction);
        public delegate void ReversePropertyValue<T>(T block);

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
                SetDirection = (b, d, v) => Set(b, v);
                Increment = (b, v) => Set(b, Get(b).Plus(v));
                IncrementDirection = (b, d, v) => Increment(b, Multiply(v, d));
                Move = (b, d) => Set(b, Get(b).Plus(Multiply(delta,d)));
                Reverse = (b) => Set(b, Get(b).Not());
            }
            private Primitive Multiply(Primitive p, Direction d) { return (d == Direction.DOWN) ? p.Not() : p; }
        }

        public class SimpleNumericPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleNumericPropertyHandler(GetNumericProperty<T> GetValue, SetNumericProperty<T> SetValue, float delta)
                : base((b) => new NumberPrimitive(GetValue(b)), (b,v)=> {
                    SetValue(b, CastNumber(v).GetNumericValue());
                }, new NumberPrimitive(delta)) {
            }
        }

        public class SimpleNumericDirectionPropertyHandler<T> : PropertyHandler<T> {
            public SimpleNumericDirectionPropertyHandler(GetNumericPropertyDirection<T> GetValue, SetNumericPropertyDirection<T> SetValue, Direction defaultDirection) {
                Get = b => GetDirection(b, defaultDirection);
                GetDirection = (b,d) => new NumberPrimitive(GetValue(b,d));
                Set = (b, v) => SetDirection(b, defaultDirection, v);
                SetDirection = (b, d, v) => SetValue(b, d, CastNumber(v).GetNumericValue());
                IncrementDirection = (b, d, v) => SetDirection(b, d, GetDirection(b, d).Plus(v));
                Increment = (b, v) => IncrementDirection(b, defaultDirection, v);
            }
        }


        public class SimpleStringPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleStringPropertyHandler(GetStringProperty<T> GetValue, SetStringProperty<T> SetValue)
                : base((b) => new StringPrimitive(GetValue(b)), (b, v) => {
                    SetValue(b,CastString(v).GetStringValue());
                }, new StringPrimitive("")) {
            }
        }

        public class SimpleBooleanPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleBooleanPropertyHandler(GetBooleanProperty<T> GetValue, SetBooleanProperty<T> SetValue)
                : base((b) => new BooleanPrimitive(GetValue(b)), (b, v) => {
                    SetValue(b, CastBoolean(v).GetBooleanValue());
                }, new BooleanPrimitive(true)) {
            }
        }

        public class SimpleVectorPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleVectorPropertyHandler(GetVectorProperty<T> GetValue, SetVectorProperty<T> SetValue)
                : base((b) => new VectorPrimitive(GetValue(b)), (b, v) => {
                    SetValue(b, CastVector(v).GetVectorValue());
                }, new VectorPrimitive(Vector3D.Zero)) {
            }
        }

        public class SimpleColorPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleColorPropertyHandler(GetColorProperty<T> GetValue, SetColorProperty<T> SetValue)
                : base((b) => new ColorPrimitive(GetValue(b)), (b, v) => {
                    SetValue(b, CastColor(v).GetColorValue());
                }, new ColorPrimitive(new Color(10,10,10))) {
            }
        }

        public class PropertyValueNumericPropertyHandler<T> : SimpleNumericPropertyHandler<T> where T : class, IMyTerminalBlock {
            public PropertyValueNumericPropertyHandler(String propertyName, float delta) : base((b) => b.GetValueFloat(propertyName), (b, v) => b.SetValueFloat(propertyName, v), delta) {
            }
        }

        public class DirectionVectorPropertyHandler<T> : PropertyHandler<T> where T : class, IMyTerminalBlock {
            public DirectionVectorPropertyHandler() {
                Get = (b) => GetDirection(b, Direction.FORWARD);
                GetDirection = (b, d) => {
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
                Set = (b,v) => { };
                SetDirection = (b, d, v) => { };
                Increment = (b, v) => { };
                IncrementDirection = (b, d, v) => { };
                Move = (b, d) => { };
                Reverse = (b) => { };
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
            Property GetDefaultProperty(Return type);
            Property GetDefaultProperty(Direction direction);
            Direction GetDefaultDirection();
            List<Object> GetBlocks(Func<IMyTerminalBlock, bool> selector);
            List<Object> GetBlocksInGroup(String groupName);

            Primitive GetPropertyValue(Object block, Property property);
            Primitive GetPropertyValue(Object block, Property property, Direction direction);

            void SetPropertyValue(Object block, Property property, Primitive value);
            void SetPropertyValue(Object block, Property property, Direction direction, Primitive value);
            void IncrementPropertyValue(Object block, Property property, Primitive value);
            void IncrementPropertyValue(Object block, Property property, Direction direction, Primitive value);
            void MoveNumericPropertyValue(Object block, Property property, Direction direction);
            void ReverseNumericPropertyValue(Object block, Property property);
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
            protected Dictionary<Property, PropertyHandler<T>> propertyHandlers = new Dictionary<Property, PropertyHandler<T>>();
            protected Property defaultBooleanProperty = Property.POWER;
            protected Property defaultStringProperty = Property.NAME;
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
            public Property GetDefaultProperty(Direction direction) {
                if (!defaultPropertiesByDirection.ContainsKey(direction)) throw new Exception(GetType() + " Does Not Have A Default Property for Direction: " + direction);
                return defaultPropertiesByDirection[direction];
            }
            public Property GetDefaultProperty(Return type) {
                if (!defaultPropertiesByPrimitive.ContainsKey(type)) throw new Exception(GetType() + " Does Not Have A Default Property for Primitive: " + type);
                return defaultPropertiesByPrimitive[type];
            }
            public Primitive GetPropertyValue(object block, Property property) {
                return propertyHandlers[property].Get((T)block);
            }
            public Primitive GetPropertyValue(object block, Property property, Direction direction) {
                return propertyHandlers[property].GetDirection((T)block, direction);
            }
            public void SetPropertyValue(Object block, Property property, Primitive value) {
                Debug("Setting " + Name(block) + " " + property + " to " + value.GetValue());
                propertyHandlers[property].Set((T)block, value);
            }
            public void SetPropertyValue(Object block, Property property, Direction direction, Primitive value) {
                Debug("Setting " + Name(block) + " " + property + " to " + value.GetValue() + " in " + direction + " direction");
                propertyHandlers[property].SetDirection((T)block, direction, value);
            }
            public void IncrementPropertyValue(Object block, Property property, Primitive value) {
                Debug("Incrementing " + Name(block) + " " + property + " by " + value.GetValue());
                propertyHandlers[property].Increment((T)block, value);
            }
            public void IncrementPropertyValue(Object block, Property property, Direction direction, Primitive value) {
                Debug("Incrementing " + Name(block) + " " + property + " by " + value.GetValue() + " in " + direction + " direction");
                propertyHandlers[property].IncrementDirection((T)block, direction, value);
            }
            public void MoveNumericPropertyValue(Object block, Property property, Direction direction) {
                Debug("Moving " + Name(block) + " " + property + " in " + direction + " direction");
                propertyHandlers[property].Move((T)block, direction);
            }
            public void ReverseNumericPropertyValue(Object block, Property property) {
                Debug("Reversing " + Name(block) + " " + property);
                propertyHandlers[property].Reverse((T)block);
            }
            private string Name(object block) {
                return Name((T)block);
            }
            protected abstract string Name(T block);

            protected void AddBooleanHandler(Property property, GetBooleanProperty<T> Get) {
                AddBooleanHandler(property, Get, (b, v) => { });
            }

            protected void AddBooleanHandler(Property property, GetBooleanProperty<T> Get, SetBooleanProperty<T> Set) {
                propertyHandlers[property] = new SimpleBooleanPropertyHandler<T>(Get, Set);
            }

            protected void AddPropertyHandler(Property property, PropertyHandler<T> handler) {
                propertyHandlers[property] = handler;
            }

            protected void AddStringHandler(Property property, GetStringProperty<T> Get, SetStringProperty<T> Set) {
                propertyHandlers[property] = new SimpleStringPropertyHandler<T>(Get, Set);
            }

            protected void AddNumericHandler(Property property, GetNumericProperty<T> Get) {
                propertyHandlers[property] = new SimpleNumericPropertyHandler<T>(Get, (b, v) => { }, 0);
            }

            protected void AddNumericHandler(Property property, GetNumericProperty<T> Get, SetNumericProperty<T> Set, float delta) {
                propertyHandlers[property] = new SimpleNumericPropertyHandler<T>(Get, Set, delta);
            }

            protected void AddVectorHandler(Property property, GetVectorProperty<T> Get) {
                propertyHandlers[property] = new SimpleVectorPropertyHandler<T>(Get, (b,v) => { });
            }

            protected void AddVectorHandler(Property property, GetVectorProperty<T> Get, SetVectorProperty<T> Set) {
                propertyHandlers[property] = new SimpleVectorPropertyHandler<T>(Get, Set);
            }

            protected void AddColorHandler(Property property, GetColorProperty<T> Get, SetColorProperty<T> Set) {
                propertyHandlers[property] = new SimpleColorPropertyHandler<T>(Get, Set);
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
