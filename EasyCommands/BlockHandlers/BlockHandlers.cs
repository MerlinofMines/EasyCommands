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
        //Property Getters
        public delegate Primitive GetProperty<T>(T block, PropertySupplier property);
        public delegate Primitive GetPropertyDirection<T>(T block, PropertySupplier property, Direction direction);
        public delegate void SetProperty<T>(T block, PropertySupplier property, Primitive value);
        public delegate void SetPropertyDirection<T>(T block, PropertySupplier property, Direction direction, Primitive value);
        public delegate void IncrementProperty<T>(T block, PropertySupplier property);
        public delegate void IncrementPropertyValue<T>(T block, PropertySupplier property, Primitive deltaValue);
        public delegate void IncrementPropertyValueDirection<T>(T block, PropertySupplier property, Direction direction, Primitive deltaValue);
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
            public IncrementPropertyValue<T> IncrementValue;
            public IncrementPropertyValueDirection<T> IncrementValueDirection;
            public MovePropertyValue<T> Move;
            public ReversePropertyValue<T> Reverse;
        }

        public class SimplePropertyHandler<T> : PropertyHandler<T> {
            public SimplePropertyHandler(GetProperty<T> Getter, SetProperty<T> Setter, Primitive delta) {
                Get = Getter;
                Set = Setter;
                GetDirection = (b, p, d) => Get(b, p);
                SetDirection = (b, p, d, v) => Set(b, p, v);
                Increment = (b, p) => IncrementValue(b, p, delta);
                IncrementValue = (b, p, v) => Set(b, p, Get(b, p).Plus(Multiply(v, p)));
                IncrementValueDirection = (b, p, d, v) => SetDirection(b, p, d, GetDirection(b, p, d).Plus(Multiply(v, p)));
                Move = (b, p, d) => SetDirection(b, p, d, GetDirection(b, p, d).Plus(Multiply(delta, p)));
                Reverse = (b, p) => Set(b, p, Get(b, p).Not());
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
                GetDirection = (b, p, d) => GetHandler(p).GetDirection(b, p, d);
                SetDirection = (b, p, d, v) => GetHandler(p).SetDirection(b, p, d, v);
                IncrementValueDirection = (b, p, d, v) => GetHandler(p).IncrementValueDirection(b, p, d, v);
                IncrementValue = (b, p, v) => GetHandler(p).IncrementValue(b, p, v);
                Increment = (b, p) => GetHandler(p).Increment(b, p);
                Move = (b, p, d) => GetHandler(p).Move(b, p, d);
            }
        }

        public class TypeHandler<T, U> {
            public PropertyHandler<T> handler;
            public U[] supportedTypes;
        }

        public interface IBlockHandler {
            PropertySupplier GetDefaultProperty(Return type);
            PropertySupplier GetDefaultProperty(Direction direction);
            Direction GetDefaultDirection();
            List<Object> SelectBlocks<U>(List<U> blocks, Func<U, bool> selector = null) where U : IMyTerminalBlock;
            String GetName(Object block);

            Primitive GetPropertyValue(Object block, PropertySupplier property);
            void UpdatePropertyValue(Object block, PropertySupplier property);
            void IncrementPropertyValue(Object block, PropertySupplier property);
            void ReverseNumericPropertyValue(Object block, PropertySupplier property);
        }

        public abstract class BlockHandler<T> : IBlockHandler where T : class {
            public Dictionary<String, PropertyHandler<T>> propertyHandlers = NewDictionary<String, PropertyHandler<T>>();
            public Dictionary<Return, Property> defaultPropertiesByPrimitive = NewDictionary<Return, Property>();
            public Dictionary<Direction, Property> defaultPropertiesByDirection = NewDictionary<Direction, Property>();
            public Direction defaultDirection = Direction.UP;

            public BlockHandler() {
                AddListHandler(Property.NAMES, b => CastList(ResolvePrimitive(Name(b))));
                defaultPropertiesByPrimitive[Return.STRING] = Property.NAMES;
            }

            public string GetName(object block) => Name((T)block);

            public virtual PropertyHandler<T> GetPropertyHandler(PropertySupplier property) {
                if (propertyHandlers.ContainsKey(property.propertyType)) return propertyHandlers[property.propertyType];
                throw new Exception(typeof(T).Name + " does not have property: " + (property.propertyWord ?? property.propertyType));
            }

            public List<Object> SelectBlocks<U>(List<U> blocks, Func<U, bool> selector = null) where U : IMyTerminalBlock =>
                SelectBlocksByType(blocks, selector).OfType<Object>().ToList();

            public abstract IEnumerable<T> SelectBlocksByType<U>(List<U> blocks, Func<U, bool> selector = null) where U : IMyTerminalBlock;
            public abstract string Name(T block);

            public Direction GetDefaultDirection() => defaultDirection;

            public PropertySupplier GetDefaultProperty(Direction direction) =>
                new PropertySupplier(defaultPropertiesByDirection.GetValueOrDefault(direction, defaultPropertiesByDirection[defaultDirection]) + "");

            public PropertySupplier GetDefaultProperty(Return type) =>
                new PropertySupplier(defaultPropertiesByPrimitive.GetValueOrDefault(type, defaultPropertiesByPrimitive[Return.STRING]) + "");

            public Primitive GetPropertyValue(object block, PropertySupplier property) {
                Primitive value = property.direction.HasValue ?
                    GetPropertyHandler(property).GetDirection((T)block, property, property.direction.Value) :
                    GetPropertyHandler(property).Get((T)block, property);
                return property.inverse ? value.Not() : value;
            }

            public void UpdatePropertyValue(Object block, PropertySupplier property) {
                if (property.propertyValue != null) {
                    Primitive value = property.propertyValue.GetValue();
                    if (property.direction != null) {
                        GetPropertyHandler(property).SetDirection((T)block, property, property.direction.Value, value);
                    } else {
                        GetPropertyHandler(property).Set((T)block, property, value);
                    }
                } else {
                    GetPropertyHandler(property).Move((T)block, property, property.direction ?? defaultDirection);
                }
            }

            public void IncrementPropertyValue(Object block, PropertySupplier property) {
                if (property.propertyValue == null) {
                    GetPropertyHandler(property).Increment((T)block, property);
                } else {
                    Primitive value = property.propertyValue.GetValue();
                    if (property.direction != null) {
                        GetPropertyHandler(property).IncrementValueDirection((T)block, property, property.direction.Value, value);
                    } else {
                        GetPropertyHandler(property).IncrementValue((T)block, property, value);
                    }
                }
            }

            public void ReverseNumericPropertyValue(Object block, PropertySupplier property) => GetPropertyHandler(property).Reverse((T)block, property);
            public void AddPropertyHandler(Property property, PropertyHandler<T> handler) => propertyHandlers[property + ""] = handler;
            public void AddPropertyHandler(ValueProperty property, PropertyHandler<T> handler) => propertyHandlers[property + ""] = handler;
            public void AddBooleanHandler(Property property, GetTypedProperty<T, bool> Get, SetTypedProperty<T, bool> Set = null) =>
                AddPropertyHandler(property, BooleanHandler(Get, Set));
            public void AddStringHandler(Property property, GetTypedProperty<T, string> Get, SetTypedProperty<T, string> Set = null) =>
                AddPropertyHandler(property, StringHandler(Get, Set));
            public void AddNumericHandler(Property property, GetTypedProperty<T, float> Get, SetTypedProperty<T, float> Set = null, float delta = 0) =>
                AddPropertyHandler(property, NumericHandler(Get, Set, delta));
            public void AddVectorHandler(Property property, GetTypedProperty<T, Vector3D> Get, SetTypedProperty<T, Vector3D> Set = null) =>
                AddPropertyHandler(property, VectorHandler(Get, Set));
            public void AddColorHandler(Property property, GetTypedProperty<T, Color> Get, SetTypedProperty<T, Color> Set = null) =>
                AddPropertyHandler(property, ColorHandler(Get, Set));
            public void AddListHandler(Property property, GetTypedProperty<T, KeyedList> Get, SetTypedProperty<T, KeyedList> Set = null) =>
                AddPropertyHandler(property, ListHandler(Get, Set));
            public void AddDirectionHandlers(Property property, Direction defaultDirection, params TypeHandler<T, Direction>[] handlers) =>
                AddPropertyHandler(property, DirectionalTypedHandler(defaultDirection, handlers));
            public void AddReturnHandlers(Property property, Return defaultReturn, params TypeHandler<T, Return>[] handlers) =>
                AddPropertyHandler(property, ReturnTypedHandler(defaultReturn, handlers));

            public PropertyHandler<T> DirectionalTypedHandler(Direction defaultDirection, params TypeHandler<T, Direction>[] handlers) =>
                TypedHandler(defaultDirection, p => p.direction ?? defaultDirection, handlers);
            public PropertyHandler<T> ReturnTypedHandler(Return defaultReturn, params TypeHandler<T, Return>[] handlers) =>
                TypedHandler(defaultReturn, p => p.propertyValue?.GetValue().returnType ?? Return.DEFAULT, handlers);

            public TypeHandler<T, U> TypeHandler<U>(PropertyHandler<T> h, params U[] values) => new TypeHandler<T, U> {
                handler = h,
                supportedTypes = values
            };

            PropertyHandler<T> TypedHandler<U>(U defaultType, Func<PropertySupplier, U> Resolver, params TypeHandler<T, U>[] handlers) {
                var typeHandlers = NewDictionary<U, PropertyHandler<T>>();
                foreach (TypeHandler<T, U> handler in handlers) foreach (U type in handler.supportedTypes) typeHandlers.Add(type, handler.handler);
                return new SimplePropertySupplierBasedHandler<T>(p => typeHandlers.GetValueOrDefault(Resolver(p), typeHandlers[defaultType]));
            }

            public PropertyHandler<T> BooleanHandler(GetTypedProperty<T, bool> Get, SetTypedProperty<T, bool> Set = null) => TypedPropertyHandler(Get, Set, CastBoolean, true);
            public PropertyHandler<T> StringHandler(GetTypedProperty<T, string> Get, SetTypedProperty<T, string> Set = null) => TypedPropertyHandler(Get, Set, CastString, "");
            public PropertyHandler<T> NumericHandler(GetTypedProperty<T, float> Get, SetTypedProperty<T, float> Set = null, float delta = 0) => TypedPropertyHandler(Get, Set, CastNumber, delta);
            public PropertyHandler<T> VectorHandler(GetTypedProperty<T, Vector3D> Get, SetTypedProperty<T, Vector3D> Set = null) => TypedPropertyHandler(Get, Set, CastVector, Vector3D.Zero);
            public PropertyHandler<T> ColorHandler(GetTypedProperty<T, Color> Get, SetTypedProperty<T, Color> Set = null) => TypedPropertyHandler(Get, Set, CastColor, new Color(10, 10, 10));
            public PropertyHandler<T> ListHandler(GetTypedProperty<T, KeyedList> Get, SetTypedProperty<T, KeyedList> Set = null) => TypedPropertyHandler(Get, Set, CastList, NewKeyedList());
            PropertyHandler<T> TypedPropertyHandler<U>(GetTypedProperty<T, U> Get, SetTypedProperty<T, U> Set, Func<Primitive, U> Cast, U delta) => new SimpleTypedHandler<T, U>(Get, Set ?? ((b, v) => { }), Cast, delta);
        }

        public abstract class MultiInstanceBlockHandler<T> : BlockHandler<T> where T : class {
            public override IEnumerable<T> SelectBlocksByType<U>(List<U> blocks, Func<U, bool> selector = null) =>
                blocks.Where(selector ?? (b => true)).SelectMany(b => GetInstances(b));
            public abstract IEnumerable<T> GetInstances(IMyTerminalBlock block);
        }
    }
}
