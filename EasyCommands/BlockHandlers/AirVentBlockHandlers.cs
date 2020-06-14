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
                booleanPropertyGetters.Add(BooleanPropertyType.COMPLETE, (b) => !InProgress(b));
                booleanPropertyGetters.Add(BooleanPropertyType.RUNNING, (b) => InProgress(b));
                booleanPropertyGetters.Add(BooleanPropertyType.PRODUCE, (b) => !InProgress(b));
                booleanPropertySetters.Add(BooleanPropertyType.PRODUCE, (b, v) => b.Depressurize = !v);
                numericPropertyGetters.Add(NumericPropertyType.RATIO, (b) => b.GetOxygenLevel());
            }

            bool InProgress(IMyAirVent b) { return b.Status == VentStatus.Depressurizing || b.Status == VentStatus.Pressurizing; }
        }
    }
}
