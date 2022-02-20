using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public class PropertyValue {
            public string propertyType, propertyWord;
            public Variable attributeValue;

            public PropertyValue(string type, string word = null, Variable attribute = null) {
                propertyType = type;
                propertyWord = word ?? type;
                attributeValue = attribute;
            }

            public List<PropertyValue> Resolve() {
                if (propertyType != Property.PROPERTY + "") return NewList(this);
                var resolvedProperty = CastString(attributeValue.GetValue());
                return PROGRAM.propertyWords.ContainsKey(resolvedProperty) ?
                    PROGRAM.propertyWords[resolvedProperty]
                        .OfType<PropertyCommandParameter>()
                        .Select(p => new PropertyValue(p + ""))
                        .ToList() :
                    NewList(new PropertyValue(resolvedProperty, resolvedProperty));
            }
        }

        public class PropertySupplier {
            public List<PropertyValue> properties = NewList<PropertyValue>();
            public Variable propertyValue;
            public bool? increment;

            public PropertySupplier Resolve(BlockHandler handler, Return? defaultType = null) {
                var propertySupplier = properties.Count > 0 ? this : WithProperties(NewList(ResolvePropertyType(handler, defaultType)));
                propertySupplier.properties = propertySupplier.properties.SelectMany(p => p.Resolve()).ToList();
                return propertySupplier;
            }

            PropertyValue ResolvePropertyType(BlockHandler blockHandler, Return? defaultType = null) {
                Return? returnValue = propertyValue?.GetValue()?.returnType ?? defaultType;
                return new PropertyValue("" + (returnValue.HasValue ? blockHandler.GetDefaultProperty(returnValue.Value) : blockHandler.GetDefaultProperty()));
            }

            public PropertySupplier WithProperties(List<PropertyValue> properties) {
                PropertySupplier copy = Copy();
                copy.properties = properties;
                return copy;
            }

            public PropertySupplier WithPropertyValue(Variable propertyValue) {
                PropertySupplier copy = Copy();
                copy.propertyValue = propertyValue;
                return copy;
            }

            public PropertySupplier WithIncrement(bool increment) {
                PropertySupplier copy = Copy();
                copy.increment= increment;
                return copy;
            }

            PropertySupplier Copy() => new PropertySupplier {
                    properties = properties,
                    propertyValue = propertyValue,
                    increment = increment
            };

            public string GetPropertyString() => properties.Aggregate("", (a, b) => a + "," + b.propertyWord);
        }
    }
}
