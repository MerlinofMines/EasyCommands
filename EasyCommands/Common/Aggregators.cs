using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public delegate Primitive Aggregator(List<Primitive> primitives);

        public static Primitive SumAggregator(List<Primitive> primitives) {
            if (primitives.Count==0) {
                throw new Exception("Cannot sum an empty list");
            }

            Primitive result = primitives[0];

            for (int i = 1; i <primitives.Count;i++) {
                result = result.Plus(primitives[i]);
            }

            return result;
        }

        public static Primitive AverageAggregator(List<Primitive> primitives) {
            return SumAggregator(primitives).Divide(new NumberPrimitive(primitives.Count));
        }

        public static Primitive CountAggregator(List<Primitive> primitives) {
            return new NumberPrimitive(primitives.Count);
        }
    }
}
