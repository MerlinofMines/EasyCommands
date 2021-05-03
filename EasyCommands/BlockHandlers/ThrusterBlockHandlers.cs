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
                AddPropertyHandler(Property.RANGE, new SimpleNumericPropertyHandler<IMyThrust>((b) => b.ThrustOverride, (b, v) => b.ThrustOverride = v, 5000));
                defaultPropertiesByDirection[Direction.UP] = Property.RANGE;
                defaultDirection = Direction.UP;
                //TODO: Better Properties
            }
        }
    }
}
