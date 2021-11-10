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
                //TODO: Broadcast using Antennas support
                AddPropertyHandler(Property.RANGE, TerminalBlockPropertyHandler("Range", 50));
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RANGE;
                defaultPropertiesByDirection.Add(Direction.UP, Property.RANGE);
                defaultDirection = Direction.UP;
            }
        }
    }
}
