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

        public enum AggregationMode {
            ANY,
            ALL,
            NONE
        }

        public static String getAggregationModeName(AggregationMode mode) {
            switch (mode) {
                case AggregationMode.ALL: return "All";
                case AggregationMode.ANY: return "Any";
                case AggregationMode.NONE: return "None";
                default: throw new Exception("Unsupported Aggregation Mode");
            }
        }

        public interface BlockCondition {
            bool evaluate(Object block, Block blockType);
        }

        public class NotBlockCondition : BlockCondition {
            public BlockCondition blockCondition;

            public NotBlockCondition(BlockCondition condition) {
                blockCondition = condition;
            }

            public bool evaluate(Object block, Block blockType) => !blockCondition.evaluate(block, blockType);

            public override String ToString() => "not " + blockCondition;
        }

        public class AndBlockCondition : BlockCondition {
            public BlockCondition conditionA;
            public BlockCondition conditionB;

            public AndBlockCondition(BlockCondition condA, BlockCondition condB) {
                conditionA = condA;
                conditionB = condB;
            }

            public bool evaluate(object block, Block blockType) => conditionA.evaluate(block, blockType) && conditionB.evaluate(block, blockType);
        }

        public class OrBlockCondition : BlockCondition {
            public BlockCondition conditionA;
            public BlockCondition conditionB;

            public OrBlockCondition(BlockCondition condA, BlockCondition condB) {
                conditionA = condA;
                conditionB = condB;
            }

            public bool evaluate(object block, Block blockType) => conditionA.evaluate(block, blockType) || conditionB.evaluate(block, blockType);
        }

        public class BlockPropertyCondition : BlockCondition {
            public PropertySupplier property;
            public PrimitiveComparator comparator;
            public Variable comparisonValue;

            public BlockPropertyCondition(PropertySupplier prop, PrimitiveComparator comp, Variable value) {
                property = prop;
                comparator = comp;
                comparisonValue = value;
            }

            public bool evaluate(Object block, Block blockType) {
                BlockHandler handler = BlockHandlerRegistry.GetBlockHandler(blockType);
                Primitive value = comparisonValue.GetValue();
                PropertySupplier prop = property.Resolve(handler, value.returnType);
                return comparator(handler.GetPropertyValue(block, prop), value);
            }

            public override String ToString() => property + " " + comparator + " " + comparisonValue.GetValue();
        }

        public delegate bool PrimitiveComparator(Primitive a, Primitive b);
    }
}
