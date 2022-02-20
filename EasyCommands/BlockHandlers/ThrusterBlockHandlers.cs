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
        public class ThrusterBlockHandler : FunctionalBlockHandler<IMyThrust> {
            public ThrusterBlockHandler() {
                var currentThrustHandler = NumericHandler(b => b.CurrentThrust, (b, v) => b.ThrustOverride = v, 5000);
                AddPropertyHandler(currentThrustHandler, Property.LEVEL);
                AddPropertyHandler(currentThrustHandler, Property.VOLUME);
                AddNumericHandler(Property.RANGE, b => b.MaxThrust, (b, v) => b.ThrustOverride = v, 5000);
                AddNumericHandler(Property.RATIO, b => b.ThrustOverridePercentage, (b, v) => b.ThrustOverridePercentage = v, 0.1f);
                AddNumericHandler(Property.OVERRIDE, b => b.ThrustOverride, (b, v) => b.ThrustOverride = v, 5000);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.LEVEL;
            }
        }
    }
}
