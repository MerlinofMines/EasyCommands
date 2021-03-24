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
                AddNumericHandler(PropertyType.BLINK_INTERVAL, (b) => b.BlinkIntervalSeconds, (b, v) => b.BlinkIntervalSeconds = v, 0.1f);
                AddNumericHandler(PropertyType.BLINK_LENGTH, (b) => b.BlinkLength, (b, v) => b.BlinkLength = v, 0.1f);
                AddNumericHandler(PropertyType.BLINK_OFFSET, (b) => b.BlinkOffset, (b, v) => b.BlinkOffset = v, 0.1f);
                AddNumericHandler(PropertyType.INTENSITY, (b) => b.Intensity, (b, v) => b.Intensity = v, 1f);
                AddNumericHandler(PropertyType.FALLOFF, (b) => b.Falloff, (b, v) => b.Falloff = v, 0.5f);
                defaultPropertiesByPrimitive[PrimitiveType.STRING] = PropertyType.COLOR;
                defaultPropertiesByPrimitive[PrimitiveType.NUMERIC] = PropertyType.RANGE;
                defaultPropertiesByDirection.Add(DirectionType.UP, PropertyType.RANGE);
                defaultDirection = DirectionType.UP;
            }
        }
    }
}
