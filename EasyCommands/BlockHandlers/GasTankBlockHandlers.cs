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
                AddBooleanHandler(PropertyType.PRODUCE, (b) => !b.Stockpile, (b, v) => b.Stockpile = !v);
                AddBooleanHandler(PropertyType.AUTO, (b) => b.AutoRefillBottles, (b, v) => b.AutoRefillBottles = v);
                AddNumericHandler(PropertyType.RANGE, (b) => b.Capacity);
                AddNumericHandler(PropertyType.RATIO, (b) => (float)b.FilledRatio);
                defaultPropertiesByPrimitive[PrimitiveType.NUMERIC] = PropertyType.RATIO;
                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.AUTO;
                defaultPropertiesByDirection.Add(DirectionType.UP, PropertyType.RATIO);
                defaultDirection = DirectionType.UP;
            }
        }
    }
}
