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
        public delegate Primitive Aggregator(IEnumerable<object> blocks, Func<object,Primitive> primitiveSupplier);
        public Aggregator SumAggregator = (blocks, primitiveSupplier) => blocks.Select(primitiveSupplier).Aggregate((Primitive)null, (a, b) => a?.Plus(b) ?? b) ?? ResolvePrimitive(0);
        public Aggregator DefaultAggregator = (blocks, primitiveSupplier) => {
            var blockValues = blocks
                .Select(primitiveSupplier)
                .DefaultIfEmpty(ResolvePrimitive(NewKeyedList()));

            return (blockValues.Count() == 1) ? blockValues.First() :
                blockValues.All(v => v.returnType == Return.NUMERIC) ? blockValues.Aggregate((a, b) => a.Plus(b)) :
                ResolvePrimitive(NewKeyedList(blockValues.Select(v => new StaticVariable(v))));
        };

        public delegate bool AggregateCondition(int count, int matches);
        public AggregateCondition AllCondition = (count, matches) => count == matches;
        public AggregateCondition NoneCondition = (count, matches) => matches == 0;
    }
}
