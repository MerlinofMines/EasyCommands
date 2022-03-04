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
        public class SearchlightHandler : SubTypedBlockHandler<IMyFunctionalBlock> {
            public SearchlightHandler() : base(IsSubType("Searchlight")) {
                AddPropertyHandler(Property.COLOR, TerminalPropertyHandler("Color", Color.Black));
                AddPropertyHandler(Property.RADIUS, TerminalPropertyHandler("Radius", 10));
                AddPropertyHandler(Property.RANGE, TerminalPropertyHandler("Range", 100));
                AddPropertyHandler(Property.INTERVAL, TerminalPropertyHandler("Blink Interval", 0.1f));
                AddPropertyHandler(Property.LEVEL, TerminalPropertyHandler("Blink Lenght", 0.1f));//Intentionally Typod
                AddPropertyHandler(Property.OFFSET, TerminalPropertyHandler("Offset", 0.1f));
                AddPropertyHandler(Property.VOLUME, TerminalPropertyHandler("Intensity", 1f));
                AddPropertyHandler(Property.FALLOFF, TerminalPropertyHandler("Falloff", 0.5f));
                AddPropertyHandler(Property.ROLL_INPUT, TerminalPropertyHandler("EnableIdleMovement", true));
                AddPropertyHandler(Property.LOCKED, TerminalPropertyHandler("EnableTargetLocking", true));
                //TODO: Add Blink Offset w/ Multi-Property Support
                //TODO: Add Target Options support
                defaultPropertiesByPrimitive[Return.COLOR] = Property.COLOR;
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RANGE;
                defaultPropertiesByDirection.Add(Direction.UP, Property.RANGE);
            }
        }

        public class LightBlockHandler : FunctionalBlockHandler<IMyLightingBlock> {
            public LightBlockHandler() {

                AddColorHandler(Property.COLOR, b => b.Color, (b, v) => b.Color = v);
                var radiusHandler = NumericHandler(b => b.Radius, (b, v) => b.Radius = v, 3);
                AddPropertyHandler(Property.RANGE, radiusHandler);
                AddPropertyHandler(Property.RADIUS, radiusHandler);
                AddNumericHandler(Property.INTERVAL, b => b.BlinkIntervalSeconds, (b, v) => b.BlinkIntervalSeconds = v, 0.1f);
                AddNumericHandler(Property.LEVEL, b => b.BlinkLength, (b, v) => b.BlinkLength = v, 0.1f);
                AddNumericHandler(Property.OFFSET, b => b.BlinkOffset, (b, v) => b.BlinkOffset = v, 0.1f);
                AddNumericHandler(Property.VOLUME, b => b.Intensity, (b, v) => b.Intensity = v, 1f);
                AddNumericHandler(Property.FALLOFF, b => b.Falloff, (b, v) => b.Falloff = v, 0.5f);
                defaultPropertiesByPrimitive[Return.COLOR] = Property.COLOR;
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RANGE;
                defaultPropertiesByDirection.Add(Direction.UP, Property.RANGE);
            }
        }
    }
}
