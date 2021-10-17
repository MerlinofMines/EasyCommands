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
        public static Primitive SumAggregator(List<Primitive> primitives) => primitives.DefaultIfEmpty(ResolvePrimitive(0)).Aggregate((a, b) => a.Plus(b));

        public static Primitive AverageAggregator(List<Primitive> primitives) => SumAggregator(primitives).Divide(ResolvePrimitive(Math.Max(1, primitives.Count)));

        public static Primitive MinAggregator(List<Primitive> primitives) => primitives.DefaultIfEmpty(ResolvePrimitive(0)).Aggregate((a, b) => (a.Compare(b) < 0 ? a : b));

        public static Primitive MaxAggregator(List<Primitive> primitives) => primitives.DefaultIfEmpty(ResolvePrimitive(0)).Aggregate((a, b) => (a.Compare(b) > 0 ? a : b));
    }
}
