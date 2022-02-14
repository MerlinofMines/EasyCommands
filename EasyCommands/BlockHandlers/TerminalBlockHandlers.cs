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
            public TerminalBlockPropertyHandler(String propertyId, Primitive delta) : this(new PropertySupplier(propertyId), delta) { }
            public TerminalBlockPropertyHandler(PropertySupplier propertySupplier, Primitive delta) : base(
                (b, p) => ResolvePrimitive(GetPropertyConverter(b, propertySupplier).GetValue(b, propertySupplier.propertyType)),
                (b, p, v) => GetPropertyConverter(b, propertySupplier).SetValue(b, propertySupplier.propertyType, v), delta) { }

            static Dictionary<String, TerminalPropertyConverter> TerminalPropertyValueConversion = NewDictionary(
                KeyValuePair("StringBuilder", PropertyConverter((b, p) => b.GetValue<StringBuilder>(p).ToString(), (b, p, v) => b.SetValue(p, new StringBuilder(CastString(v))))),
                KeyValuePair("Boolean", PropertyConverter((b, p) => b.GetValueBool(p), (b, p, v) => b.SetValueBool(p, CastBoolean(v)))),
                KeyValuePair("Single", PropertyConverter((b, p) => b.GetValueFloat(p), (b, p, v) => b.SetValueFloat(p, CastNumber(v)))),
                KeyValuePair("Int64", PropertyConverter((b, p) => (float)b.GetValue<long>(p), (b, p, v) => b.SetValue(p, (long)CastNumber(v)))),
                KeyValuePair("Color", PropertyConverter((b, p) => b.GetValueColor(p), (b, p, v) => b.SetValueColor(p, CastColor(v))))
            );

            static TerminalPropertyConverter GetPropertyConverter(T block, PropertySupplier propertySupplier) {
                var property = block.GetProperty(propertySupplier.propertyType);
                if (property == null) throw new Exception(block.BlockDefinition.SubtypeName + " does not have property: " + (propertySupplier.propertyWord ?? propertySupplier.propertyType));
                return TerminalPropertyValueConversion[property.TypeName];
            }
        }

        public class TerminalBlockHandler<T> : BlockHandler<T> where T : class, IMyTerminalBlock {
            public TerminalBlockHandler() {
                AddVectorHandler(Property.POSITION, block => block.GetPosition());
                AddStringHandler(Property.NAME, b => b.CustomName, (b, v) => b.CustomName = v);
                AddBooleanHandler(Property.SHOW, b => b.ShowInTerminal, (b, v) => b.ShowInTerminal = v);
                AddStringHandler(Property.DATA, b => b.CustomData, (b, v) => b.CustomData = v);
                AddStringHandler(Property.INFO, b => b.DetailedInfo);
                AddListHandler(Property.PROPERTIES, b => {
                    var properties = NewList<ITerminalProperty>();
                    b.GetProperties(properties);
                    return NewKeyedList(properties.Select(p => GetStaticVariable(p.Id)));
                });
                AddListHandler(Property.ACTIONS, b => {
                    var actions = NewList<ITerminalAction>();
                    b.GetActions(actions);
                    return NewKeyedList(actions.Select(p => GetStaticVariable(p.Id)));
                });

                AddPropertyHandler(ValueProperty.ACTION, new SimplePropertyHandler<T>((b, p) => p.attributeValue.GetValue(), (b, p, v) => b.ApplyAction(CastString(p.attributeValue.GetValue())), ResolvePrimitive(0)));

                AddDirectionHandlers(Property.DIRECTION, Direction.FORWARD,
                    TypeHandler(VectorHandler(b => b.WorldMatrix.Forward), Direction.FORWARD),
                    TypeHandler(VectorHandler(b => b.WorldMatrix.Backward), Direction.BACKWARD),
                    TypeHandler(VectorHandler(b => b.WorldMatrix.Up), Direction.UP),
                    TypeHandler(VectorHandler(b => b.WorldMatrix.Down), Direction.DOWN),
                    TypeHandler(VectorHandler(b => b.WorldMatrix.Left), Direction.LEFT),
                    TypeHandler(VectorHandler(b => b.WorldMatrix.Right), Direction.RIGHT));

                defaultPropertiesByPrimitive[Return.VECTOR] = Property.POSITION;
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.ENABLE;
                defaultPropertiesByPrimitive[Return.STRING] = Property.NAME;
            }


            public override IEnumerable<T> SelectBlocksByType<U>(List<U> blocks, Func<U, bool> selector = null) =>
                blocks.Where(b => selector == null || selector(b)).OfType<T>();

            public override PropertyHandler<T> GetPropertyHandler(PropertySupplier property) {
                try {
                    return base.GetPropertyHandler(property);
                } catch (Exception) {
                    return new TerminalBlockPropertyHandler<T>(property, ResolvePrimitive(1));
                }
            }

            public override string Name(T block) => block.CustomName;

            public String GetCustomProperty(T block, String key) => GetCustomData(block).GetValueOrDefault(key, null);

            public void SetCustomProperty(T block, String key, String value) {
                var d = GetCustomData(block);
                d[key] = value;
                SaveCustomData(block, d);
            }

            public void DeleteCustomProperty(T block, String key) {
                var d = GetCustomData(block);
                d.Remove(key);
                SaveCustomData(block, d);
            }

            public void SaveCustomData(T block, Dictionary<String, String> data) =>
                block.CustomData = String.Join("\n", data.Select(p => p.Key + "=" + p.Value));

            public Dictionary<String, String> GetCustomData(T block) =>
                block.CustomData
                    .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Split('='))
                    .ToDictionary(token => token[0], token => token[1]);

            public PropertyHandler<T> TerminalBlockPropertyHandler(String propertyId, object delta) => new TerminalBlockPropertyHandler<T>(propertyId, ResolvePrimitive(delta));
        }
        public class FunctionalBlockHandler<T> : TerminalBlockHandler<T> where T : class, IMyFunctionalBlock {
            public FunctionalBlockHandler() {
                var enableHandler = BooleanHandler(b => b.Enabled, (b, v) => b.Enabled = v);
                AddPropertyHandler(Property.ENABLE, enableHandler);
                AddPropertyHandler(Property.POWER, enableHandler);
            }
        }

        /// <summary>
        ///Provides a consistent way to get the most accurate position for a detected entity.
        ///Not all detected entities will have a HitPosition.  Attempt to use HitPosition first.  If not present, use Position instead.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static Vector3D GetPosition(MyDetectedEntityInfo entity) => entity.HitPosition.GetValueOrDefault(entity.Position);
    }
}
