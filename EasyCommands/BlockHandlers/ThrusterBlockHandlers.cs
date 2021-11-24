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
                AddNumericHandler(Property.RANGE, b => b.ThrustOverride, (b, v) => b.ThrustOverride = v, 5000);
                AddNumericHandler(Property.RATIO, b => b.ThrustOverride/b.MaxThrust, (b, v) => b.ThrustOverride = v*b.MaxThrust, 0.1f);
                defaultPropertiesByDirection[Direction.UP] = Property.RANGE;
                //TODO: Better Properties
            }
        }
    }
}
