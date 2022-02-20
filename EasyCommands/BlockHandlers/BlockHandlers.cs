using Sandbox.ModAPI.Ingame;
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
        public List<IMyTerminalBlock> blockCache = NewList<IMyTerminalBlock>();
        public Cache<Block, List<Object>> selectorCache = new Cache<Block, List<Object>>();
        public Cache<Block, List<Object>> groupCache = new Cache<Block, List<Object>>();
        public Cache<Type, ITerminalAction> actionCache = new Cache<Type, ITerminalAction>();
        public Cache<Type, ITerminalProperty> propertyCache = new Cache<Type, ITerminalProperty>();

        public static class BlockHandlerRegistry {
            static Dictionary<Block, BlockHandler> blockHandlers = new Dictionary<Block, BlockHandler> {
                { Block.AIRVENT, new AirVentBlockHandler()},
                { Block.ANTENNA, new AntennaBlockHandler()},
                { Block.ASSEMBLER, new AssemblerBlockHandler()},
                { Block.BATTERY, new BatteryBlockHandler()},
                { Block.BEACON, new BeaconBlockHandler()},
                { Block.CAMERA, new CameraBlockHandler() },
                { Block.CARGO, new CargoHandler() },
                { Block.COCKPIT, new CockpitBlockHandler<IMyCockpit>() },
                { Block.COLLECTOR, new FunctionalBlockHandler<IMyCollector>() },
                { Block.CONNECTOR, new ConnectorBlockHandler() },
                { Block.CRYO_CHAMBER, new CockpitBlockHandler<IMyCryoChamber>() },
                { Block.DECOY, new FunctionalBlockHandler<IMyDecoy>() },
                { Block.DETECTOR, new OreDetectorHandler() },
                { Block.DISPLAY, new TextSurfaceHandler() },
                { Block.DOOR, new DoorBlockHandler() },
                { Block.DRILL, new FunctionalBlockHandler<IMyShipDrill>() },
                { Block.EJECTOR, new EjectorBlockHandler() },
                { Block.ENGINE, new EngineBlockHandler<IMyPowerProducer>("Engine") },
                { Block.GENERATOR, new GasGeneratorHandler()},
                { Block.GRAVITY_GENERATOR, new GravityGeneratorBlockHandler() },
                { Block.GRAVITY_SPHERE, new SphericalGravityGeneratorBlockHandler() },
                { Block.GRINDER, new FunctionalBlockHandler<IMyShipGrinder>() },
                { Block.GUN, new GunBlockHandler<IMyUserControllableGun>() },
                { Block.GYROSCOPE, new GyroscopeBlockHandler<IMyGyro>() },
                { Block.HEAT_VENT, new HeatVentBlockHandler() },
                { Block.HINGE, new RotorBlockHandler(IsHinge) },
                { Block.JUMPDRIVE, new JumpDriveBlockHandler() },
                { Block.LASER_ANTENNA, new LaserAntennaBlockHandler() },
                { Block.LIGHT, new LightBlockHandler() },
                { Block.MAGNET, new LandingGearHandler() },
                { Block.MERGE, new MergeBlockHandler() },
                { Block.PARACHUTE, new ParachuteBlockHandler() },
                { Block.PROGRAM, new ProgramBlockHandler() },
                { Block.PISTON, new PistonBlockHandler() },
                { Block.PROJECTOR, new ProjectorBlockHandler() },
                { Block.REACTOR, new EngineBlockHandler<IMyReactor>() },
                { Block.REMOTE, new RemoteControlBlockHandler()},
                { Block.REFINERY, new FunctionalBlockHandler<IMyRefinery>() },
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
                { Block.WELDER, new FunctionalBlockHandler<IMyShipWelder>() }
            };

            public static BlockHandler GetBlockHandler(Block blockType) {
                if (!blockHandlers.ContainsKey(blockType)) throw new Exception("Unsupported Block Type: " + blockType);
                return blockHandlers[blockType];
            }

            public static List<Object> GetSelf(Block? blockType) =>
                blockType == null || blockType == Block.DISPLAY
                    ? blockHandlers[blockType ?? Block.PROGRAM].SelectBlocks(NewList(PROGRAM.Me))
                    : null;

            public static List<Object> GetBlocks(Block blockType, string selector = null) {
                if (PROGRAM.blockCache.Count == 0)
                    PROGRAM.GridTerminalSystem.GetBlocks(PROGRAM.blockCache);

                return PROGRAM.selectorCache.GetOrCreate(blockType, selector, s =>
                    blockHandlers[blockType].SelectBlocks(PROGRAM.blockCache, b => s?.Equals(b.CustomName) ?? true));
            }

            public static List<Object> GetBlocksInGroup(Block blockType, String groupName) =>
                PROGRAM.groupCache.GetOrCreate(blockType, groupName, s => {
                    var blocks = NewList<IMyTerminalBlock>();
                    PROGRAM.GridTerminalSystem.GetBlockGroupWithName(s)?.GetBlocks(blocks);
                    return blockHandlers[blockType].SelectBlocks(blocks);
                });
        }

        //Property Getters
        public delegate Primitive GetProperty<T>(T block, PropertySupplier property);
        public delegate void SetProperty<T>(T block, PropertySupplier property, Primitive value);
        public delegate void IncrementProperty<T>(T block, PropertySupplier property);
        public delegate void IncrementPropertyValue<T>(T block, PropertySupplier property, Primitive deltaValue);

        //Getters
        public delegate U GetTypedProperty<T, U>(T block);

        //Setters
        public delegate void SetTypedProperty<T, U>(T block, U value);

        public class PropertyHandler<T> {
            public GetProperty<T> Get;
            public SetProperty<T> Set;
            public IncrementProperty<T> Increment;
            public IncrementPropertyValue<T> IncrementValue;
        }

        public class SimplePropertyHandler<T> : PropertyHandler<T> {
            public SimplePropertyHandler(GetProperty<T> Getter, SetProperty<T> Setter, Primitive delta) {
                Get = Getter;
                Set = Setter;
                Increment = (b, p) => IncrementValue(b, p, delta);
                IncrementValue = (b, p, v) => Set(b, p, Get(b, p).Plus(Multiply(v, p)));
                //TODO: Remove
                //IncrementValueDirection = (b, p, d, v) => SetDirection(b, p, d, GetDirection(b, p, d).Plus(Multiply(v, p)));
                //Move = (b, p, d) => SetDirection(b, p, d, GetDirection(b, p, d).Plus(Multiply(delta, p)));
                //Reverse = (b, p) => Set(b, p, Get(b, p).Not());
            }

            Primitive Multiply(Primitive p, PropertySupplier property) => property.increment ?? true ? p : p.Not();
        }

        public class SimpleTypedHandler<T, U> : SimplePropertyHandler<T> {
            public SimpleTypedHandler(GetTypedProperty<T, U> GetValue, SetTypedProperty<T, U> SetValue, Func<Primitive, U> Cast, U incrementValue)
                : base((b, p) => ResolvePrimitive(GetValue(b)), (b, p, v) => SetValue(b, Cast(v)), ResolvePrimitive(incrementValue)) {
            }
        }

        public delegate PropertyHandler<T> GetPropertyHandler<T>(PropertySupplier supplier);

        public class SimplePropertySupplierBasedHandler<T> : PropertyHandler<T> {
            public SimplePropertySupplierBasedHandler(GetPropertyHandler<T> GetHandler) {
                Get = (b, p) => GetHandler(p).Get(b, p);
                Set = (b, p, v) => GetHandler(p).Set(b, p, v);
                IncrementValue = (b, p, v) => GetHandler(p).IncrementValue(b, p, v);
                Increment = (b, p) => GetHandler(p).Increment(b, p);

                //TODO: Remove
                //GetDirection = (b, p, d) => GetHandler(p).GetDirection(b, p, d);
                //SetDirection = (b, p, d, v) => GetHandler(p).SetDirection(b, p, d, v);
                //IncrementValueDirection = (b, p, d, v) => GetHandler(p).IncrementValueDirection(b, p, d, v);
                //Move = (b, p, d) => GetHandler(p).Move(b, p, d);
            }
        }

        public class TypeHandler<T, U> {
            public PropertyHandler<T> handler;
            public U[] supportedTypes;
        }

        public interface BlockHandler {
            Property GetDefaultProperty(Return type);
            Property GetDefaultProperty();
            List<Object> SelectBlocks<U>(List<U> blocks, Func<U, bool> selector = null) where U : IMyTerminalBlock;
            String GetName(Object block);

            Primitive GetPropertyValue(Object block, PropertySupplier property);
            void UpdatePropertyValue(Object block, PropertySupplier property);
            void IncrementPropertyValue(Object block, PropertySupplier property);
        }

        public abstract class BlockHandler<T> : BlockHandler where T : class {
            public Dictionary<int, PropertyHandler<T>> propertyHandlers = NewDictionary<int, PropertyHandler<T>>();
            public Dictionary<Return, Property> defaultPropertiesByPrimitive = NewDictionary<Return, Property>();
            public Property defaultProperty = Property.NAMES;

            public BlockHandler() {
                AddListHandler(Property.NAMES, b => CastList(ResolvePrimitive(Name(b))));
                defaultPropertiesByPrimitive[Return.STRING] = Property.NAMES;
            }

            public string GetName(object block) => Name((T)block);

            public virtual PropertyHandler<T> GetPropertyHandler(PropertySupplier property) {
                int hashCode = property.GetHashCode();
                if (propertyHandlers.ContainsKey(hashCode)) return propertyHandlers[hashCode];
                throw new Exception(typeof(T).Name + " does not have property support for: " + property.GetPropertyString());
            }

            public List<Object> SelectBlocks<U>(List<U> blocks, Func<U, bool> selector = null) where U : IMyTerminalBlock =>
                SelectBlocksByType(blocks, selector).OfType<Object>().ToList();

            public abstract IEnumerable<T> SelectBlocksByType<U>(List<U> blocks, Func<U, bool> selector = null) where U : IMyTerminalBlock;
            public abstract string Name(T block);

            public Property GetDefaultProperty() => defaultProperty;
            public Property GetDefaultProperty(Return type) => defaultPropertiesByPrimitive.GetValueOrDefault(type, defaultProperty);
            public Primitive GetPropertyValue(object block, PropertySupplier property) =>
                GetPropertyHandler(property).Get((T)block, property);

            public void UpdatePropertyValue(object block, PropertySupplier property) =>
                GetPropertyHandler(property).Set((T)block, property, property.propertyValue?.GetValue() ?? ResolvePrimitive(true));

            public void IncrementPropertyValue(object block, PropertySupplier property) {
                if (property.propertyValue == null) {
                    GetPropertyHandler(property).Increment((T)block, property);
                } else {
                    GetPropertyHandler(property).IncrementValue((T)block, property, property.propertyValue.GetValue());
                }
            }

            public void AddPropertyHandler(PropertyHandler<T> handler, params Property[] properties) {
                propertyHandlers[GetPropertiesHashCode(properties.Select(p => p + ""))] = handler;
                if (!properties.Contains(Property.REVERSE))
                    propertyHandlers[GetPropertiesHashCode(properties.Concat(NewList(Property.REVERSE)).Select(p => p + ""))] = ReverseHandler(handler);
            }

            public void AddBooleanHandler(Property property, GetTypedProperty<T, bool> Get, SetTypedProperty<T, bool> Set = null) =>
                AddPropertyHandler(BooleanHandler(Get, Set), NewArray(property));
            public void AddStringHandler(Property property, GetTypedProperty<T, string> Get, SetTypedProperty<T, string> Set = null) =>
                AddPropertyHandler(StringHandler(Get, Set), NewArray(property));
            public void AddNumericHandler(Property property, GetTypedProperty<T, float> Get, SetTypedProperty<T, float> Set = null, float delta = 0) =>
                AddPropertyHandler(NumericHandler(Get, Set, delta), NewArray(property));
            public void AddVectorHandler(Property property, GetTypedProperty<T, Vector3D> Get, SetTypedProperty<T, Vector3D> Set = null) =>
                AddPropertyHandler(VectorHandler(Get, Set), NewArray(property));
            public void AddColorHandler(Property property, GetTypedProperty<T, Color> Get, SetTypedProperty<T, Color> Set = null) =>
                AddPropertyHandler(ColorHandler(Get, Set), NewArray(property));
            public void AddListHandler(Property property, GetTypedProperty<T, KeyedList> Get, SetTypedProperty<T, KeyedList> Set = null) =>
                AddPropertyHandler(ListHandler(Get, Set), NewArray(property));
            public void AddReturnHandlers(Property property, Return defaultReturn, params TypeHandler<T, Return>[] handlers) =>
                AddPropertyHandler(ReturnTypedHandler(defaultReturn, handlers), NewArray(property));

            public PropertyHandler<T> ReturnTypedHandler(Return defaultReturn, params TypeHandler<T, Return>[] handlers) =>
                TypedHandler(defaultReturn, p => p.propertyValue != null ? p.propertyValue.GetValue().returnType : Return.DEFAULT, handlers);

            //This function generates the same hashcode for a given property set regardless of ordering
            int GetPropertiesHashCode(IEnumerable<string> properties) => properties.Aggregate(0, (a, b) => a.GetHashCode() ^ b.GetHashCode());

            public TypeHandler<T, U> TypeHandler<U>(PropertyHandler<T> h, params U[] values) => new TypeHandler<T, U> {
                handler = h,
                supportedTypes = values
            };

            PropertyHandler<T> TypedHandler<U>(U defaultType, Func<PropertySupplier, U> Resolver, params TypeHandler<T, U>[] handlers) {
                var typeHandlers = handlers.SelectMany(handler => handler.supportedTypes, (handler, type) => new { handler.handler, type })
                    .ToDictionary(o => o.type, o => o.handler);
                return new SimplePropertySupplierBasedHandler<T>(p => typeHandlers.GetValueOrDefault(Resolver(p), typeHandlers[defaultType]));
            }

            public PropertyHandler<T> BooleanHandler(GetTypedProperty<T, bool> Get, SetTypedProperty<T, bool> Set = null) => TypedPropertyHandler(Get, Set, CastBoolean, true);
            public PropertyHandler<T> StringHandler(GetTypedProperty<T, string> Get, SetTypedProperty<T, string> Set = null) => TypedPropertyHandler(Get, Set, CastString, "");
            public PropertyHandler<T> NumericHandler(GetTypedProperty<T, float> Get, SetTypedProperty<T, float> Set = null, float delta = 0) => TypedPropertyHandler(Get, Set, CastNumber, delta);
            public PropertyHandler<T> VectorHandler(GetTypedProperty<T, Vector3D> Get, SetTypedProperty<T, Vector3D> Set = null) => TypedPropertyHandler(Get, Set, CastVector, Vector3D.Zero);
            public PropertyHandler<T> ColorHandler(GetTypedProperty<T, Color> Get, SetTypedProperty<T, Color> Set = null) => TypedPropertyHandler(Get, Set, CastColor, new Color(10, 10, 10));
            public PropertyHandler<T> ListHandler(GetTypedProperty<T, KeyedList> Get, SetTypedProperty<T, KeyedList> Set = null) => TypedPropertyHandler(Get, Set, CastList, NewKeyedList());
            PropertyHandler<T> TypedPropertyHandler<U>(GetTypedProperty<T, U> Get, SetTypedProperty<T, U> Set, Func<Primitive, U> Cast, U delta) => new SimpleTypedHandler<T, U>(Get, Set ?? ((b, v) => { }), Cast, delta);
            PropertyHandler<T> ReverseHandler(PropertyHandler<T> handler) => new SimplePropertyHandler<T>((b,p) => handler.Get(b,p).Not(), (b,p,v) => handler.Set(b,p,v.Not()), ResolvePrimitive(false));
        }

        public abstract class MultiInstanceBlockHandler<T> : BlockHandler<T> where T : class {
            public override IEnumerable<T> SelectBlocksByType<U>(List<U> blocks, Func<U, bool> selector = null) =>
                blocks.Where(b => selector == null || selector(b)).SelectMany(b => GetInstances(b));
            public abstract IEnumerable<T> GetInstances(IMyTerminalBlock block);
        }
    }
}
