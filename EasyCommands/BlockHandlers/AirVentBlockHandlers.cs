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
        public class AirVentBlockHandler : FunctionalBlockHandler<IMyAirVent> {
            public AirVentBlockHandler() {
                AddBooleanHandler(Property.COMPLETE, (b) => !InProgress(b));
                AddBooleanHandler(Property.RUN, (b) => InProgress(b));
                AddBooleanHandler(Property.SUPPLY, (b) => !InProgress(b), (b, v) => b.Depressurize = !v);
                AddNumericHandler(Property.RATIO, (b) => b.GetOxygenLevel());
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RATIO;
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.SUPPLY;
                defaultPropertiesByDirection[Direction.UP] = Property.RATIO;
                defaultDirection = Direction.UP;
            }

            bool InProgress(IMyAirVent b) { return b.Status == VentStatus.Depressurizing || b.Status == VentStatus.Pressurizing; }
        }
    }
}
