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
                AddColorHandler(Property.COLOR, (b) => b.Color, (b, v) => b.Color = v);
                AddNumericHandler(Property.RANGE, (b) => b.Radius, (b, v) => b.Radius = v, 3);
                AddNumericHandler(Property.BLINK_INTERVAL, (b) => b.BlinkIntervalSeconds, (b, v) => b.BlinkIntervalSeconds = v, 0.1f);
                AddNumericHandler(Property.BLINK_LENGTH, (b) => b.BlinkLength, (b, v) => b.BlinkLength = v, 0.1f);
                AddNumericHandler(Property.BLINK_OFFSET, (b) => b.BlinkOffset, (b, v) => b.BlinkOffset = v, 0.1f);
                AddNumericHandler(Property.INTENSITY, (b) => b.Intensity, (b, v) => b.Intensity = v, 1f);
                AddNumericHandler(Property.FALLOFF, (b) => b.Falloff, (b, v) => b.Falloff = v, 0.5f);
                defaultPropertiesByPrimitive[Return.COLOR] = Property.COLOR;
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RANGE;
                defaultPropertiesByDirection.Add(Direction.UP, Property.RANGE);
                defaultDirection = Direction.UP;
            }
        }
    }
}
