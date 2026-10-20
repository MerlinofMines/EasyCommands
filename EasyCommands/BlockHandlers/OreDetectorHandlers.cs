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
        public class OreDetectorHandler : FunctionalBlockHandler<IMyOreDetector> {
            public OreDetectorHandler() {
                var rangeHandler = TerminalPropertyHandler("Range", 50);
                AddPropertyAliases(rangeHandler, Property.RANGE, Property.RADIUS);
                AddBooleanHandler(Property.SUPPLY, b => b.BroadcastUsingAntennas, (b, v) => b.BroadcastUsingAntennas = v);
                SetDefaultProperty(Property.RANGE, Return.NUMERIC, Direction.UP);
            }
        }
    }
}
