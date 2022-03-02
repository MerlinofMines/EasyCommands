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
        public class OreDetectorHandler : FunctionalBlockHandler<IMyOreDetector> {
            public OreDetectorHandler() {
                var rangeHandler = TerminalPropertyHandler("Range", 50);
                AddPropertyHandler(Property.RANGE, rangeHandler);
                AddPropertyHandler(Property.RADIUS, rangeHandler);
                AddBooleanHandler(Property.SUPPLY, b => b.BroadcastUsingAntennas, (b, v) => b.BroadcastUsingAntennas = v);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RANGE;
                defaultPropertiesByDirection.Add(Direction.UP, Property.RANGE);
            }
        }
    }
}
