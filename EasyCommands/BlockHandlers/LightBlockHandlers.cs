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
        public class LightBlockHandler : FunctionalBlockHandler<IMyLightingBlock> {
            public LightBlockHandler() {
                AddStringHandler(PropertyType.COLOR, (b) => b.Color.ToString(), (b, v) => b.Color = GetColor(v));
                AddNumericHandler(PropertyType.RANGE, (b) => b.Radius, (b, v) => b.Radius = v, 3);
                defaultStringProperty = PropertyType.COLOR;
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, PropertyType.RANGE);
            }
        }
    }
}
