﻿using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public delegate U GetTypedProperty<T, U>(T block);
        public delegate U GetTypedPropertyDirection<T, U>(T block, Direction direction);

        //Setters
        public delegate void SetTypedProperty<T, U>(T block, U value);
        public delegate void SetTypedPropertyDirection<T, U>(T block, Direction direction, U value);

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
            public SimplePropertyHandler(GetProperty<T> Getter, SetProperty<T> Setter, Primitive delta) {
                Get = Getter;
                Set = Setter;
                GetDirection = (b, p, d) => Get(b, p);
                SetDirection = (b, p, d, v) => Set(b, p, v);
                Increment = (b, p, v) => Set(b, p, Get(b, p).Plus(v));
                IncrementDirection = (b, p, d, v) => Increment(b, p, Multiply(v, d));
                Move = (b, p, d) => Set(b, p, Get(b, p).Plus(Multiply(delta, d)));
                Reverse = (b, p) => Set(b, p, Get(b, p).Not());
            }

            Primitive Multiply(Primitive p, Direction d) { return (d == Direction.DOWN) ? p.Not() : p; }
        }

        public class SimpleTypedHandler<T, U> : SimplePropertyHandler<T> {
            public SimpleTypedHandler(GetTypedProperty<T, U> GetValue, SetTypedProperty<T, U> SetValue, Func<Primitive, U> Cast, U incrementValue)
                : base((b, p) => ResolvePrimitive(GetValue(b)), (b, p, v) => SetValue(b, Cast(v)), ResolvePrimitive(incrementValue)) {
            }
        }

        public class SimpleDirectionalTypedHandler<T> : PropertyHandler<T> {
            Dictionary<Direction, PropertyHandler<T>> directionalHandlers;

            public SimpleDirectionalTypedHandler(Dictionary<Direction, PropertyHandler<T>> handlers, Direction defaultDirection) {
                directionalHandlers = handlers;
                GetDirection = (b, p, d) => GetHandler(d, defaultDirection).GetDirection(b, p, d);
                SetDirection = (b, p, d, v) => GetHandler(d, defaultDirection).SetDirection(b, p, d, v);
                Get = (b, p) => GetDirection(b, p, defaultDirection);
                Set = (b, p, v) => SetDirection(b, p, defaultDirection, v);
                IncrementDirection = (b, p, d, v) => GetHandler(d, defaultDirection).IncrementDirection(b, p, d, v);
                Increment = (b, p, v) => IncrementDirection(b, p, defaultDirection, v);
            }

            PropertyHandler<T> GetHandler(Direction d, Direction defaultDirection) => directionalHandlers.ContainsKey(d) ? directionalHandlers[d] : directionalHandlers[defaultDirection];
        }

        public delegate object GetBlockPropertyValue(IMyTerminalBlock block, String propertyId);
        public delegate void SetBlockPropertyValue(IMyTerminalBlock block, String propertyId, Primitive value);

        public class TerminalPropertyConverter {
            public GetBlockPropertyValue GetValue;
            public SetBlockPropertyValue SetValue;
        }

        public static TerminalPropertyConverter PropertyConverter(GetBlockPropertyValue GetValue, SetBlockPropertyValue SetValue) => new TerminalPropertyConverter {
            GetValue = GetValue,
            SetValue = SetValue
        };

        public class TerminalBlockPropertyHandler<T> : SimplePropertyHandler<T> where T : class, IMyTerminalBlock {
            public TerminalBlockPropertyHandler(String propertyId, Primitive delta) : base(
                (b,p) => ResolvePrimitive(GetPropertyConverter(b, propertyId).GetValue(b,propertyId)),
                (b, p, v) => GetPropertyConverter(b, propertyId).SetValue(b, propertyId, v), delta) { }

            static Dictionary<String, TerminalPropertyConverter> TerminalPropertyValueConversion = new Dictionary<String, TerminalPropertyConverter> {
                { "StringBuilder", PropertyConverter((b,p) => b.GetValue<StringBuilder>(p).ToString(), (b,p,v) => b.SetValue(p, new StringBuilder(CastString(v))))},
                { "Boolean", PropertyConverter((b,p) => b.GetValueBool(p), (b,p,v) => b.SetValueBool(p, CastBoolean(v)))},
                { "Single", PropertyConverter((b,p) => b.GetValueFloat(p), (b,p,v) => b.SetValueFloat(p, CastNumber(v)))},
                { "Int64", PropertyConverter((b,p) => (float)b.GetValue<long>(p), (b,p,v) => b.SetValue(p, (long)CastNumber(v)))},
                { "Color", PropertyConverter((b,p) => b.GetValueColor(p), (b,p,v) => b.SetValueColor(p, CastColor(v)))}
            };

            static TerminalPropertyConverter GetPropertyConverter(T block, String propertyId) {
                var property = block.GetProperty(propertyId);
                if (property == null) throw new Exception(block.BlockDefinition.SubtypeName + " does not have property: " + propertyId);
                return TerminalPropertyValueConversion[property.TypeName];
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
                AddStringHandler(Property.NAME, b => b.CustomName, (b, v) => b.CustomName = v);
                AddBooleanHandler(Property.SHOW, b => b.ShowInTerminal, (b, v) => b.ShowInTerminal = v);
                AddListHandler(Property.PROPERTIES, b => {
                    var properties = new List<ITerminalProperty>();
                    b.GetProperties(properties);
                    return new KeyedList(properties.Select(p => GetStaticVariable(p.Id)).ToArray());
                });
                AddListHandler(Property.ACTIONS, b => {
                    var actions = new List<ITerminalAction>();
                    b.GetActions(actions);
                    return new KeyedList(actions.Select(p => GetStaticVariable(p.Id)).ToArray());
                });

                AddPropertyHandler(ValueProperty.ACTION, new SimplePropertyHandler<T>((b,p)=>p.attributeValue.GetValue(), (b,p,v) => b.ApplyAction(CastString(p.attributeValue.GetValue())), ResolvePrimitive(0)));

                AddDirectionHandlers(Property.DIRECTION, Direction.FORWARD,
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Forward)), Direction.FORWARD),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Backward)), Direction.BACKWARD),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Up)), Direction.UP),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Down)), Direction.DOWN),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Left)), Direction.LEFT),
                    DirectionalHandler(VectorHandler(b => Normalize(GetBlock2WorldTransform(b).Right)), Direction.RIGHT));

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
                    return TerminalBlockPropertyHandler(property.propertyType, 1);
                }
            }

            public override string Name(T block) { return block.CustomName; }

            public String GetCustomProperty(T block, String key) { return GetCustomData(block).GetValueOrDefault(key, null); }

            public void SetCustomProperty(T block, String key, String value) {
                Dictionary<String, String> d = GetCustomData(block);
                d[key] = value;SaveCustomData(block, d);
            }

            public void DeleteCustomProperty(T block, String key) {
                Dictionary<String, String> d = GetCustomData(block);
                if(d.ContainsKey(key)) d.Remove(key);
                SaveCustomData(block, d);
            }

            public void SaveCustomData(T block, Dictionary<String, String> data) {
                block.CustomData = String.Join("\n",data.Keys.Select(k => k + "=" + data[k]).ToList());
            }

            public Dictionary<String, String> GetCustomData(T block) {
                List<String> keys = block.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return keys.ToDictionary(k => k.Split('=')[0], v => v.Split('=')[1]);
            }

            public PropertyHandler<T> TerminalBlockPropertyHandler(String propertyId, object delta) => new TerminalBlockPropertyHandler<T>(propertyId, ResolvePrimitive(delta));

            //Taken from https://forum.keenswh.com/threads/how-do-i-get-the-world-position-and-rotation-of-a-ship.7363867/
            MatrixD GetBlock2WorldTransform(IMyCubeBlock blk) {
                Matrix blk2grid;
                blk.Orientation.GetMatrix(out blk2grid);
                return blk2grid *
                       MatrixD.CreateTranslation(new Vector3D(blk.Min + blk.Max) / 2.0) *
                       GetGrid2WorldTransform(blk.CubeGrid);
            }

            MatrixD GetGrid2WorldTransform(IMyCubeGrid grid) {
                Vector3D origin = grid.GridIntegerToWorld(new Vector3I(0, 0, 0));
                Vector3D plusY = grid.GridIntegerToWorld(new Vector3I(0, 1, 0)) - origin;
                Vector3D plusZ = grid.GridIntegerToWorld(new Vector3I(0, 0, 1)) - origin;
                return MatrixD.CreateScale(grid.GridSize) * MatrixD.CreateWorld(origin, -plusZ, plusY);
            }

            Vector3D Normalize(Vector3D vector) => vector / vector.Length();
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

            public void AddBooleanHandler(Property property, GetTypedProperty<T, bool> Get, SetTypedProperty<T, bool> Set = null) {
                AddPropertyHandler(property, BooleanHandler(Get, Set));
            }

            public void AddStringHandler(Property property, GetTypedProperty<T, string> Get, SetTypedProperty<T, string> Set = null) {
                AddPropertyHandler(property, StringHandler(Get, Set));
            }

            public void AddNumericHandler(Property property, GetTypedProperty<T, float> Get, SetTypedProperty<T, float> Set = null, float delta = 0) {
                AddPropertyHandler(property, NumericHandler(Get, Set, delta));
            }

            public void AddVectorHandler(Property property, GetTypedProperty<T, Vector3D> Get, SetTypedProperty<T, Vector3D> Set = null) {
                AddPropertyHandler(property, VectorHandler(Get, Set));
            }

            public void AddColorHandler(Property property, GetTypedProperty<T, Color> Get, SetTypedProperty<T, Color> Set = null) {
                AddPropertyHandler(property, ColorHandler(Get, Set));
            }

            public void AddListHandler(Property property, GetTypedProperty<T, KeyedList> Get, SetTypedProperty<T, KeyedList> Set = null) {
                AddPropertyHandler(property, ListHandler(Get, Set));
            }

            public void AddDirectionHandlers(Property property, Direction defaultDirection, params DirectionHandler<T>[] handlers) {
                AddPropertyHandler(property, DirectionalTypedHandler(defaultDirection, handlers));
            }

            public PropertyHandler<T> DirectionalTypedHandler(Direction defaultDirection, params DirectionHandler<T>[] handlers) {
                Dictionary<Direction, PropertyHandler<T>> directionHandlers = new Dictionary<Direction, PropertyHandler<T>>();
                foreach (DirectionHandler<T> handler in handlers) foreach (Direction direction in handler.supportedDirections) directionHandlers.Add(direction, handler.handler);
                return new SimpleDirectionalTypedHandler<T>(directionHandlers, defaultDirection);
            }

            public DirectionHandler<T> DirectionalHandler(PropertyHandler<T> h, params Direction[] directions) => new DirectionHandler<T> {
                handler = h,
                supportedDirections = directions
            };

            public PropertyHandler<T> BooleanHandler(GetTypedProperty<T, bool> Get, SetTypedProperty<T, bool> Set = null) => TypedPropertyHandler(Get, Set, CastBoolean, true);
            public PropertyHandler<T> StringHandler(GetTypedProperty<T, string> Get, SetTypedProperty<T, string> Set = null) => TypedPropertyHandler(Get, Set, CastString, "");
            public PropertyHandler<T> NumericHandler(GetTypedProperty<T, float> Get, SetTypedProperty<T, float> Set = null, float delta = 0) => TypedPropertyHandler(Get, Set, CastNumber, delta);
            public PropertyHandler<T> VectorHandler(GetTypedProperty<T, Vector3D> Get, SetTypedProperty<T, Vector3D> Set = null) => TypedPropertyHandler(Get, Set, CastVector, Vector3D.Zero);
            public PropertyHandler<T> ColorHandler(GetTypedProperty<T, Color> Get, SetTypedProperty<T, Color> Set = null) => TypedPropertyHandler(Get, Set, CastColor, new Color(10, 10, 10));
            public PropertyHandler<T> ListHandler(GetTypedProperty<T, KeyedList> Get, SetTypedProperty<T, KeyedList> Set = null) => TypedPropertyHandler(Get, Set, CastList, new KeyedList());
            PropertyHandler<T> TypedPropertyHandler<U>(GetTypedProperty<T, U> Get, SetTypedProperty<T, U> Set, Func<Primitive, U> Cast, U delta) => new SimpleTypedHandler<T, U>(Get, Set ?? ((b, v) => { }), Cast, delta);
        }

        public class DirectionHandler<T> {
            public PropertyHandler<T> handler;
            public Direction[] supportedDirections;
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
