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
        public class PropertySupplier {
            public String propertyType, propertyWord;
            public IVariable attributeValue, propertyValue;
            public Direction? direction;
            public bool? increment;
            public bool inverse;

            public PropertySupplier() { }

            public PropertySupplier(string property, String word = null) {
                propertyType = property;
                propertyWord = word;
            }

            public PropertySupplier Resolve(IBlockHandler handler, Return? defaultType = null) =>
                (propertyType == ValueProperty.PROPERTY + "") ? ResolveDynamicProperty() : WithPropertyType(ResolvePropertyType(handler, defaultType).propertyType);

            public PropertySupplier ResolveDynamicProperty() {
                PropertySupplier supplier = WithAttributeValue(null);
                var propertyString = CastString(attributeValue.GetValue());

                if(PROGRAM.propertyWords.ContainsKey(propertyString)) {
                    var commandParameters = PROGRAM.propertyWords[propertyString];
                    PropertyCommandParameter property = findLast<PropertyCommandParameter>(commandParameters);
                    BooleanCommandParameter booleanParameter = findLast<BooleanCommandParameter>(commandParameters);
                    if (property != null) supplier = WithPropertyType(property.value+"");
                    if (!booleanParameter?.value ?? false) supplier = supplier.Inverse(true).WithPropertyValue(new UniOperandVariable(UniOperand.REVERSE, propertyValue ?? GetStaticVariable(true)));
                } else {
                    supplier = WithPropertyType(propertyString);
                }
                supplier.propertyWord = propertyString;
                return supplier;
            }

            PropertySupplier ResolvePropertyType(IBlockHandler blockHandler, Return? defaultType = null) {
                if (propertyType != null) return this;
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

            public PropertySupplier WithPropertyType(String propertyType) {
                PropertySupplier copy = Copy();
                copy.propertyType = propertyType;
                return copy;
            }

            public PropertySupplier WithPropertyValue(IVariable propertyValue) {
                PropertySupplier copy = Copy();
                copy.propertyValue = propertyValue;
                return copy;
            }

            public PropertySupplier WithAttributeValue(IVariable attributeValue) {
                PropertySupplier copy = Copy();
                copy.attributeValue = attributeValue;
                return copy;
            }

            public PropertySupplier WithIncrement(bool increment) {
                PropertySupplier copy = Copy();
                copy.increment= increment;
                return copy;
            }

            public PropertySupplier Inverse(bool inverse) {
                PropertySupplier copy = Copy();
                copy.inverse = inverse;
                return copy;
            }

            PropertySupplier Copy() => new PropertySupplier {
                    propertyType = propertyType,
                    propertyWord = propertyWord,
                    attributeValue = attributeValue,
                    propertyValue = propertyValue,
                    direction = direction,
                    increment = increment,
                    inverse = inverse
                };
        }
    }
}
