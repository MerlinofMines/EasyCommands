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
        public delegate String GetPropertyType();

        public class PropertySupplier {
            public GetPropertyType propertyType;
            public Variable valueAttribute;

            public PropertySupplier(GetPropertyType property, Variable value = null) {
                propertyType = property;
                valueAttribute = value;
            }

            public PropertySupplier(Property property, Variable value = null) {
                propertyType = () => property + "";
                valueAttribute = value;
            }

            public override string ToString() => propertyType();
        }
    }
}
