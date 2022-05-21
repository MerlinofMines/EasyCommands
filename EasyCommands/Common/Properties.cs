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
            public String propertyType, propertyWord;
            public IVariable attributeValue;
            public bool inverse = false;

            public PropertyValue(string property, string word = null, IVariable attribute = null) {
                propertyType = property;
                propertyWord = word ?? property;
                attributeValue = attribute;
            }

            public PropertyValue Inverse(bool Inverse) {
                inverse = Inverse;
                return this;
            }

            public List<PropertyValue> Resolve() {
                var values = NewList(this);
                if (propertyType == Property.PROPERTY + "") {
                    values = NewList<PropertyValue>();
                    var replaceValues = true;
                    foreach (string s in CastString(attributeValue.GetValue()).ToLower().Split(' ')) {
                        var commandParameters = PROGRAM.propertyWords.GetValueOrDefault(s, NewList<ICommandParameter>());
                        PropertyCommandParameter property = findLast<PropertyCommandParameter>(commandParameters);
                        if (property != null)
                            values.Add(new PropertyValue(property.value + "", property.Token).Inverse(property.inverse));
                        else
                            replaceValues = false;
                    };

                    if (!replaceValues) values = NewList(new PropertyValue(CastString(attributeValue.GetValue())));
                }
                return values;
            }
        }
 
        public class PropertySupplier {
            public List<PropertyValue> propertyValues;
            public IVariable propertyValue;
            public Direction? direction;
            public bool? increment;
            public bool inverse;

            public PropertySupplier(List<PropertyValue> values = null) {
                propertyValues = values ?? NewList<PropertyValue>();
            }

            public PropertySupplier Resolve(IBlockHandler handler, Return? defaultType = null) {
                var resolvedSupplier = WithProperties(ResolvePropertyType(handler, defaultType).propertyValues);
                resolvedSupplier.propertyValues = resolvedSupplier.propertyValues.SelectMany(p => p.Resolve()).ToList();

                if (resolvedSupplier.propertyValues.Any(p => p.inverse)) {
                    resolvedSupplier = resolvedSupplier
                        .WithPropertyValue(new UniOperandVariable(UniOperand.REVERSE, propertyValue ?? GetStaticVariable(true)));
                    resolvedSupplier.inverse = true;
                }
                return resolvedSupplier;
            }

            PropertySupplier ResolvePropertyType(IBlockHandler blockHandler, Return? defaultType = null) {
                if (propertyValues.Count > 0) return this;
                if (direction != null) return blockHandler.GetDefaultProperty(direction.Value);
                var returnType = propertyValue?.GetValue().returnType ?? defaultType;
                return returnType != null
                    ? blockHandler.GetDefaultProperty(returnType.Value)
                    : blockHandler.GetDefaultProperty(blockHandler.GetDefaultDirection());
            }

            public PropertySupplier WithDirection(Direction? direction) {
                PropertySupplier copy = Copy();
                copy.direction = direction;
                return copy;
            }

            public PropertySupplier WithProperties(List<PropertyValue> properties) {
                PropertySupplier copy = Copy();
                copy.propertyValues = properties;
                return copy;
            }

            public PropertySupplier WithPropertyValue(IVariable propertyValue) {
                PropertySupplier copy = Copy();
                copy.propertyValue = propertyValue;
                return copy;
            }

            public PropertySupplier WithIncrement(bool increment) {
                PropertySupplier copy = Copy();
                copy.increment= increment;
                return copy;
            }

            public PropertySupplier And(PropertySupplier supplier) {
                PropertySupplier copy = Copy();
                copy.propertyValues.AddRange(supplier.propertyValues);
                copy.direction = copy.direction ?? supplier.direction;
                return copy;
            }

            PropertySupplier Copy() => new PropertySupplier {
                    propertyValues = propertyValues,
                    propertyValue = propertyValue,
                    direction = direction,
                    increment = increment,
                    inverse = inverse
                };

            public string GetPropertyString() => string.Join(" ", propertyValues.Select(p => p.propertyWord));
        }
    }
}
