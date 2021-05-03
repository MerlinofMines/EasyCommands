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
        public class GasTankBlockHandler : FunctionalBlockHandler<IMyGasTank> {
            public GasTankBlockHandler() {
                AddBooleanHandler(Property.PRODUCE, (b) => !b.Stockpile, (b, v) => b.Stockpile = !v);
                AddBooleanHandler(Property.AUTO, (b) => b.AutoRefillBottles, (b, v) => b.AutoRefillBottles = v);
                AddNumericHandler(Property.RANGE, (b) => b.Capacity);
                AddNumericHandler(Property.RATIO, (b) => (float)b.FilledRatio);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RATIO;
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.AUTO;
                defaultPropertiesByDirection.Add(Direction.UP, Property.RATIO);
                defaultDirection = Direction.UP;
            }
        }
    }
}
