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
            public String propertyType;
            public Variable attributeValue, propertyValue;
            public Direction? direction;

            public PropertySupplier() { }

            public PropertySupplier(Property property) {
                propertyType = property + "";
            }

            public PropertySupplier Resolve(BlockHandler handler, Return? defaultType = null) {
                //TODO: Deal with PropertyType = "Property" and adjust values based on AttributeValue
                return WithPropertyType(ResolvePropertyType(handler, defaultType).propertyType);
            }

            PropertySupplier ResolvePropertyType(BlockHandler blockHandler, Return? defaultType = null) {
                if (propertyType != null) return this;
                if (direction.HasValue) return blockHandler.GetDefaultProperty(direction.Value);
                if (propertyValue != null) return blockHandler.GetDefaultProperty(propertyValue.GetValue().GetPrimitiveType());
                if (defaultType.HasValue) return blockHandler.GetDefaultProperty(defaultType.Value);
                return blockHandler.GetDefaultProperty(blockHandler.GetDefaultDirection());
            }

            public PropertySupplier WithDirection(Direction? direction) {
                var copy = Copy();
                copy.direction = direction;
                return copy;
            }

            public PropertySupplier WithPropertyType(String propertyType) {
                var copy = Copy();
                copy.propertyType = propertyType;
                return copy;
            }

            public PropertySupplier WithPropertyValue(Variable propertyValue) {
                var copy = Copy();
                copy.propertyValue = propertyValue;
                return copy;
            }

            public PropertySupplier WithAttributeValue(Variable attributeValue) {
                var copy = Copy();
                copy.attributeValue = attributeValue;
                return copy;
            }

            PropertySupplier Copy() {
                var newValue = new PropertySupplier();
                newValue.propertyType = propertyType;
                newValue.attributeValue = attributeValue;
                newValue.propertyValue = propertyValue;
                newValue.direction = direction;
                return newValue;
            }
        }
    }
}
