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
        public class BeaconBlockHandler : FunctionalBlockHandler<IMyBeacon> {
            public BeaconBlockHandler() {
                stringPropertyGetters.Add(StringPropertyType.TEXT, (b) => b.HudText);
                stringPropertySetters.Add(StringPropertyType.TEXT, (b, v) => b.HudText = v);
                numericPropertyGetters.Add(NumericPropertyType.RANGE, (b) => b.Radius);
                numericPropertySetters.Add(NumericPropertyType.RANGE, new SimpleNumericPropertySetter<IMyBeacon>((b) => b.Radius, (b, v) => b.Radius = v, 3));
                defaultStringProperty = StringPropertyType.TEXT;
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.RANGE);
            }
        }
    }
}
