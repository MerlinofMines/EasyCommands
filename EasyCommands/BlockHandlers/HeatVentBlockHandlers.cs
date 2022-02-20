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
        public class HeatVentBlockHandler : FunctionalBlockHandler<IMyHeatVent> {
            public HeatVentBlockHandler() {
                var maxColorHandler = TerminalPropertyHandler("ColorMax", Color.Black);
                AddPropertyHandler(maxColorHandler, Property.COLOR);
                AddPropertyHandler(maxColorHandler, Property.COLOR, Property.UP);
                AddPropertyHandler(TerminalPropertyHandler("ColorMin", Color.Black), Property.COLOR, Property.DOWN);
                AddPropertyHandler(TerminalPropertyHandler("Radius", 1), Property.RANGE);
                AddPropertyHandler(TerminalPropertyHandler("Intensity", 1), Property.VOLUME);
                AddPropertyHandler(TerminalPropertyHandler("Falloff", 0.3), Property.FALLOFF);
                AddPropertyHandler(TerminalPropertyHandler("Offset", 0.5), Property.OFFSET);
                AddPropertyHandler(TerminalPropertyHandler("PowerDependency", 10), Property.RATIO);
                defaultPropertiesByPrimitive[Return.COLOR] = Property.COLOR;
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RATIO;
            }
        }
    }
}
