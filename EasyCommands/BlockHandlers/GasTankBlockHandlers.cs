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
                booleanPropertyGetters.Add(BooleanPropertyType.PRODUCE, (b) => !b.Stockpile);
                booleanPropertySetters.Add(BooleanPropertyType.PRODUCE, (b, v) => b.Stockpile = !v);
                booleanPropertyGetters.Add(BooleanPropertyType.AUTO, (b) => b.AutoRefillBottles);
                booleanPropertySetters.Add(BooleanPropertyType.AUTO, (b,v) => b.AutoRefillBottles=v);
                numericPropertyGetters.Add(NumericPropertyType.RANGE, (b) => b.Capacity);
                numericPropertyGetters.Add(NumericPropertyType.RATIO, (b) => (float)b.FilledRatio);
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.RATIO);
            }
        }
    }
}
