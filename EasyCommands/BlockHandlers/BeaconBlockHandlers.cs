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
    using Sandbox.ModAPI.Ingame;
    using SpaceEngineers.Game.ModAPI.Ingame;
    using VRage.Game.ModAPI.Ingame.Utilities;
    using VRage.Game.ModAPI.Ingame;
    partial class Program {
        public class BeaconBlockHandler : FunctionalBlockHandler<IMyBeacon> {
            public BeaconBlockHandler() {
                AddStringHandler(Property.TEXT, b => b.HudText, (b, v) => b.HudText = v);
                var radiusHandler = NumericHandler(b => b.Radius, (b, v) => b.Radius = v, 1000);
                AddPropertyAliases(radiusHandler, Property.RANGE, Property.RADIUS);
                AddBooleanHandler(Property.SUPPLY, b => b.Enabled, (b, v) => b.Enabled = v);
                SetDefaultProperty(Property.TEXT, Return.STRING);
                SetDefaultProperty(Property.RANGE, Return.NUMERIC, Direction.UP);
            }
        }
    }
}
