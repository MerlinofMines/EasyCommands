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
        public delegate object GetTerminalPropertyValue(ITerminalProperty property, IMyTerminalBlock block);
        public delegate void SetTerminalPropertyValue(ITerminalProperty property, IMyTerminalBlock blockd, Primitive value);

        public class TerminalPropertyConverter {
            public GetTerminalPropertyValue GetValue;
            public SetTerminalPropertyValue SetValue;
        }

        public static TerminalPropertyConverter PropertyConverter(GetTerminalPropertyValue GetValue, SetTerminalPropertyValue SetValue) => new TerminalPropertyConverter {
            GetValue = GetValue,
            SetValue = SetValue
        };

        public class TerminalPropertyHandler<T> : SimplePropertyHandler<T> where T : class, IMyTerminalBlock {
            public TerminalPropertyHandler(String propertyId, Primitive delta) : this(new PropertySupplier(propertyId), delta) { }
            public TerminalPropertyHandler(PropertySupplier propertySupplier, Primitive delta) : base(
                (b, p) => { var property = GetTerminalProperty(b, propertySupplier); return ResolvePrimitive(TerminalPropertyConversions[property.TypeName].GetValue(property, b));  },
                (b, p, v) => { var property = GetTerminalProperty(b, propertySupplier); TerminalPropertyConversions[property.TypeName].SetValue(property, b, v); },
                delta) { }

            static Dictionary<String, TerminalPropertyConverter> TerminalPropertyConversions = NewDictionary(
                KeyValuePair("StringBuilder", PropertyConverter((p, b) => p.As<StringBuilder>().GetValue(b).ToString(), (p, b, v) => p.As<StringBuilder>().SetValue(b, new StringBuilder(CastString(v))))),
                KeyValuePair("Boolean", PropertyConverter((p, b) => p.AsBool().GetValue(b), (p, b, v) => p.AsBool().SetValue(b, CastBoolean(v)))),
                KeyValuePair("Single", PropertyConverter((p, b) => p.AsFloat().GetValue(b), (p, b, v) => p.AsFloat().SetValue(b, CastNumber(v)))),
                KeyValuePair("Int64", PropertyConverter((p, b) => (float)p.As<long>().GetValue(b), (p, b, v) => p.As<long>().SetValue(b, (long)CastNumber(v)))),
                KeyValuePair("Color", PropertyConverter((p, b) => p.AsColor().GetValue(b), (p, b, v) => p.AsColor().SetValue(b, CastColor(v))))
            );

            static ITerminalProperty GetTerminalProperty(T block, PropertySupplier propertySupplier) =>
                PROGRAM.propertyCache.GetOrCreate(block.GetType(), propertySupplier.propertyType, s =>{
                        var property = block.GetProperty(s);
                        if (property == null)
                            throw new Exception(block.BlockDefinition.SubtypeName + " does not have property: " + (propertySupplier.propertyWord ?? s));
                        return property;
                    });
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

                AddPropertyHandler(ValueProperty.ACTION, new SimplePropertyHandler<T>(
                    (b, p) => p.attributeValue.GetValue(),
                    (b, p, v) => PROGRAM.actionCache.GetOrCreate(b.GetType(), CastString(p.attributeValue.GetValue()), s => b.GetActionWithName(s)).Apply(b),
                    ResolvePrimitive(0)));

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
                blocks.Where(selector ?? (b => true)).OfType<T>();

            public override PropertyHandler<T> GetPropertyHandler(PropertySupplier property) {
                try {
                    return base.GetPropertyHandler(property);
                } catch (Exception) {
                    return new TerminalPropertyHandler<T>(property, ResolvePrimitive(1));
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

            public PropertyHandler<T> TerminalPropertyHandler(String propertyId, object delta) => new TerminalPropertyHandler<T>(propertyId, ResolvePrimitive(delta));
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
