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
                AddDirectionHandlers(Property.COLOR, Direction.UP,
                    TypeHandler(TerminalPropertyHandler("ColorMax", Color.Black), Direction.UP),
                    TypeHandler(TerminalPropertyHandler("ColorMin", Color.Black), Direction.DOWN));
                var radiusHandler = TerminalPropertyHandler("Radius", 1);
                AddPropertyHandler(Property.RANGE, radiusHandler);
                AddPropertyHandler(Property.RADIUS, radiusHandler);
                AddPropertyHandler(Property.VOLUME, TerminalPropertyHandler("Intensity", 1));
                AddPropertyHandler(Property.FALLOFF, TerminalPropertyHandler("Falloff", 0.3));
                AddPropertyHandler(Property.OFFSET, TerminalPropertyHandler("Offset", 0.5));
                AddPropertyHandler(Property.RATIO, TerminalPropertyHandler("PowerDependency", 10));
                defaultPropertiesByPrimitive[Return.COLOR] = Property.COLOR;
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RATIO;
                defaultPropertiesByDirection[Direction.UP] = Property.COLOR;
            }
        }
    }
}
