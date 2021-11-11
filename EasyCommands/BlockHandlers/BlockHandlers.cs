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
                { Block.ENGINE, new EngineBlockHandler<IMyPowerProducer>("Engine") },
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
                { Block.REACTOR, new EngineBlockHandler<IMyReactor>() },
                { Block.REMOTE, new RemoteControlBlockHandler()},
                { Block.ROTOR, new RotorBlockHandler(b => !IsHinge(b)) },
                { Block.SOLAR_PANEL, new EngineBlockHandler<IMySolarPanel>() },
                { Block.SORTER, new SorterBlockerHandler() },
                { Block.SOUND, new SoundBlockHandler() },
                { Block.SENSOR, new SensorBlockHandler() },
                { Block.SUSPENSION, new WheelSuspensionBlockHandler() },
                { Block.TANK, new GasTankBlockHandler() },
                { Block.TERMINAL, new TerminalBlockHandler<IMyTerminalBlock>() },
                { Block.TIMER, new TimerBlockHandler() },
                { Block.THRUSTER, new ThrusterBlockHandler()},
                { Block.TURBINE, new EngineBlockHandler<IMyPowerProducer>("WindTurbine") },
                { Block.TURRET, new TurretBlockHandler<IMyLargeTurretBase>()},
                { Block.WARHEAD, new WarheadBlockHandler() },
                { Block.WELDER, new FunctionalBlockHandler<IMyShipWelder>() },
                { Block.REFINERY, new FunctionalBlockHandler<IMyRefinery>() }
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
        public delegate U GetTypedProperty<T,U>(T block);
        public delegate float GetNumericPropertyDirection<T>(T block, Direction direction);

        //Setters
        public delegate void SetTypedProperty<T,U>(T block, U value);
        public delegate void SetNumericPropertyDirection<T>(T block, Direction direction, float value);

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

        public class SimpleTypedHandler<T, U> : SimplePropertyHandler<T> {
            public SimpleTypedHandler(GetTypedProperty<T, U> GetValue, SetTypedProperty<T, U> SetValue, Func<Primitive, U> Cast, U incrementValue)
                : base((b, p) => ResolvePrimitive(GetValue(b)), (b, p, v) => SetValue(b, Cast(v)), ResolvePrimitive(incrementValue)) {
            }
        }

        public class SimpleNumericDirectionPropertyHandler<T> : PropertyHandler<T> {
            public SimpleNumericDirectionPropertyHandler(GetNumericPropertyDirection<T> GetValue, SetNumericPropertyDirection<T> SetValue, Direction defaultDirection) {
                Get = (b, p) => GetDirection(b, p, defaultDirection);
                GetDirection = (b, p, d) => ResolvePrimitive(GetValue(b,d));
                Set = (b, p, v) => SetDirection(b, p, defaultDirection, v);
                SetDirection = (b, p, d, v) => SetValue(b, d, CastNumber(v));
                IncrementDirection = (b, p, d, v) => SetDirection(b, p, d, GetDirection(b, p, d).Plus(v));
                Increment = (b, p, v) => IncrementDirection(b, p, defaultDirection, v);
            }
        }

        public class TerminalBlockPropertyHandler<T> : SimplePropertyHandler<T> where T : class, IMyTerminalBlock {
            public TerminalBlockPropertyHandler(String propertyId, Primitive delta) : base((b,p) => GetPrimitive(b, propertyId), (b,p,v) => SetPrimitiveValue(b,propertyId,v), delta) { }
        }

        public static Primitive GetPrimitive<T>(T block, String propertyId) where T : class, IMyTerminalBlock {
            var property = block.GetProperty(propertyId);
            if (property == null) throw new Exception(typeof(T) + block.BlockDefinition.SubtypeName + " does not have property: " + propertyId);
            object value;
            if (property.TypeName == "bool") value = block.GetValueBool(propertyId);
            else if (property.TypeName == "color") value = block.GetValueColor(propertyId);
            else value = block.GetValueFloat(propertyId);
            return ResolvePrimitive(value);
        }

        public static void SetPrimitiveValue(IMyTerminalBlock block, String propertyId, Primitive value) {
            Return type = value.returnType;
            if (type == Return.BOOLEAN) block.SetValueBool(propertyId, CastBoolean(value));
            else if (type == Return.COLOR) block.SetValueColor(propertyId, CastColor(value));
            else block.SetValueFloat(propertyId, CastNumber(value));
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
                    return ResolvePrimitive(vector/vector.Length());//Return normalized vector
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
            String GetName(Object block);

            Primitive GetPropertyValue(Object block, PropertySupplier property);
            void UpdatePropertyValue(Object block, PropertySupplier property);
            void IncrementPropertyValue(Object block, PropertySupplier property);
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
                List<T> instances = new List<T>();
                if (group == null) return instances;
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                group.GetBlocksOfType<IMyTerminalBlock>(blocks);
                blocks.ForEach((b) => GetInstances(b, instances));
                return instances;
            }

            public abstract void GetInstances(IMyTerminalBlock block, List<T> instances);
        }

        public class FunctionalBlockHandler<T> : TerminalBlockHandler<T> where T : class, IMyFunctionalBlock {
            public FunctionalBlockHandler() : base() {
                AddBooleanHandler(Property.ENABLE, b => b.Enabled, (b, v) => b.Enabled = v);
                AddBooleanHandler(Property.POWER, b => b.Enabled, (b, v) => b.Enabled = v);
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.POWER;
            }
        }

        public class TerminalBlockHandler<T> : BlockHandler<T> where T : class, IMyTerminalBlock {
            public TerminalBlockHandler() {
                AddVectorHandler(Property.POSITION, block => block.GetPosition());
                AddPropertyHandler(Property.DIRECTION, new DirectionVectorPropertyHandler<T>());
                AddStringHandler(Property.NAME, b => b.CustomName, (b, v) => b.CustomName = v);
                AddBooleanHandler(Property.SHOW, b => b.ShowInTerminal, (b, v) => b.ShowInTerminal = v);
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
                List<T> blocks = new List<T>();
                if (group == null) return blocks;
                group.GetBlocksOfType<T>(blocks);
                return blocks;
            }

            public override PropertyHandler<T> GetPropertyHandler(PropertySupplier property) {
                try {
                    return base.GetPropertyHandler(property);
                } catch (Exception) {
                    return new TerminalBlockPropertyHandler<T>(property.propertyType, ResolvePrimitive(1f));
                }
            }

            public override string Name(T block) { return block.CustomName; }

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
            public string GetName(object block) => Name((T)block);

            public virtual PropertyHandler<T> GetPropertyHandler(PropertySupplier property) {
                if(propertyHandlers.ContainsKey(property.propertyType)) return propertyHandlers[property.propertyType];
                throw new Exception("Unsupported Property: " + property.propertyType);
            }

            public abstract List<T> GetBlocksOfType(Func<IMyTerminalBlock, bool> selector);
            public abstract List<T> GetBlocksOfTypeInGroup(String name);
            public abstract string Name(T block);

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
                Primitive value = (property.direction.HasValue ? GetPropertyHandler(property).GetDirection((T)block, property, property.direction.Value) :
                GetPropertyHandler(property).Get((T)block, property));
                if (property.propertyValue != null && !CastBoolean(property.propertyValue.GetValue())) value = value.Not();
                return value;
            }

            public void UpdatePropertyValue(Object block, PropertySupplier property) {
                if(property.propertyValue != null) {
                    Primitive value = property.propertyValue.GetValue();
                    if(property.direction != null) {
                        Debug("Setting " + GetName(block) + " " + property.propertyType + " to " + value + " in " + property.direction + " direction");
                        GetPropertyHandler(property).SetDirection((T)block, property, property.direction.Value, value);
                    } else {
                        Debug("Setting " + GetName(block) + " " + property.propertyType + " to " + value);
                        GetPropertyHandler(property).Set((T)block, property, value);
                    }
                } else {
                    Debug("Moving " + GetName(block) + " " + property.propertyType + " in " + property.direction + " direction");
                    GetPropertyHandler(property).Move((T)block, property, property.direction.Value);
                }
            }

            public void IncrementPropertyValue(Object block, PropertySupplier property) {
                Primitive value = property.propertyValue.GetValue();
                if(property.direction != null) {
                    Debug("Incrementing " + GetName(block) + " " + property.propertyType + " by " + value + " in " + property.direction + " direction");
                    GetPropertyHandler(property).IncrementDirection((T)block, property, property.direction.Value, value);
                } else {
                    Debug("Incrementing " + GetName(block) + " " + property.propertyType + " by " + value);
                    GetPropertyHandler(property).Increment((T)block, property, value);
                }
            }

            public void ReverseNumericPropertyValue(Object block, PropertySupplier property) {
                Debug("Reversing " + GetName(block) + " " + property.propertyType);
                GetPropertyHandler(property).Reverse((T)block, property);
            }

            public void AddPropertyHandler(Property property, PropertyHandler<T> handler) {
                propertyHandlers[property + ""] = handler;
            }

            public void AddPropertyHandler(ValueProperty property, PropertyHandler<T> handler) {
                propertyHandlers[property + ""] = handler;
            }

            public void AddBooleanHandler(Property property, GetTypedProperty<T, bool> Get) {
                AddBooleanHandler(property, Get, (b, v) => { });
            }

            public void AddBooleanHandler(Property property, GetTypedProperty<T, bool> Get, SetTypedProperty<T, bool> Set) {
                AddTypedPropertyHandler(property, Get, Set, CastBoolean, true);
            }

            public void AddStringHandler(Property property, GetTypedProperty<T, string> Get, SetTypedProperty<T, string> Set) {
                AddTypedPropertyHandler(property, Get, Set, CastString, "");
            }

            public void AddNumericHandler(Property property, GetTypedProperty<T, float> Get) {
                AddNumericHandler(property, Get, (b, v) => { }, 0);
            }

            public void AddNumericHandler(Property property, GetTypedProperty<T, float> Get, SetTypedProperty<T, float> Set, float delta) {
                AddTypedPropertyHandler(property, Get, Set, CastNumber, delta);
            }

            public void AddVectorHandler(Property property, GetTypedProperty<T, Vector3D> Get) {
                AddVectorHandler(property, Get, (b, v) => { });
            }

            public void AddVectorHandler(Property property, GetTypedProperty<T, Vector3D> Get, SetTypedProperty<T, Vector3D> Set) {
                AddTypedPropertyHandler(property, Get, Set, CastVector, Vector3D.Zero);
            }

            public void AddColorHandler(Property property, GetTypedProperty<T, Color> Get, SetTypedProperty<T, Color> Set) {
                AddTypedPropertyHandler(property, Get, Set, p => CastColor(p), new Color(10, 10, 10));
            }

            void AddTypedPropertyHandler<U>(Property property, GetTypedProperty<T, U> Get, SetTypedProperty<T, U> Set, Func<Primitive, U> Cast, U delta) {
                AddPropertyHandler(property, new SimpleTypedHandler<T, U>(Get, Set, Cast, delta));
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
