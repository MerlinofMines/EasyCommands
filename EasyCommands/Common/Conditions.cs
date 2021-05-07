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

            public NotBlockCondition(BlockCondition blockCondition) {
                this.blockCondition = blockCondition;
            }

            public bool evaluate(Object block, Block blockType) {
                return !blockCondition.evaluate(block, blockType);
            }
            public override String ToString() {
                return "not " + blockCondition;
            }
        }

        public class AndBlockCondition : BlockCondition {
            public BlockCondition conditionA;
            public BlockCondition conditionB;

            public AndBlockCondition(BlockCondition conditionA, BlockCondition conditionB) {
                this.conditionA = conditionA;
                this.conditionB = conditionB;
            }

            public bool evaluate(object block, Block blockType) {
                return conditionA.evaluate(block, blockType) && conditionB.evaluate(block, blockType);
            }
        }

        public class OrBlockCondition : BlockCondition {
            public BlockCondition conditionA;
            public BlockCondition conditionB;

            public OrBlockCondition(BlockCondition conditionA, BlockCondition conditionB) {
                this.conditionA = conditionA;
                this.conditionB = conditionB;
            }

            public bool evaluate(object block, Block blockType) {
                return conditionA.evaluate(block, blockType) || conditionB.evaluate(block, blockType);
            }
        }

        public class BlockPropertyCondition : BlockCondition {
            public PropertySupplier property;
            public Direction? direction;
            public PrimitiveComparator comparator;
            public Variable comparisonValue;

            public BlockPropertyCondition(PropertySupplier property, Direction? direction, PrimitiveComparator comparator, Variable comparisonValue) {
                this.property = property;
                this.comparator = comparator;
                this.comparisonValue = comparisonValue;
                this.direction = direction;
            }

            public bool evaluate(Object block, Block blockType) {
                BlockHandler handler = BlockHandlerRegistry.GetBlockHandler(blockType);
                Primitive value = comparisonValue.GetValue();
                PropertySupplier prop = property ?? handler.GetDefaultProperty(value.GetPrimitiveType());
                if (direction.HasValue) return comparator.compare(handler.GetPropertyValue(block, prop, direction.Value), value);
                else return comparator.compare(handler.GetPropertyValue(block, prop), value);
            }

            public override String ToString() {
                return property + " " + comparator + " " + (direction.HasValue ? direction + " " : "") + comparisonValue.GetValue();
            }
        }

        public class PrimitiveComparator {
            public Comparison comparisonType;
            public PrimitiveComparator(Comparison comparisonType) {
                this.comparisonType = comparisonType;
            }
            public bool compare(Primitive a, Primitive b) {
                switch (comparisonType) {
                    case Comparison.GREATER: return a.Compare(b)>0;
                    case Comparison.GREATER_OR_EQUAL: return a.Compare(b) >= 0;
                    case Comparison.EQUAL: return a.Compare(b) == 0;
                    case Comparison.LESS_OR_EQUAL: return a.Compare(b) <= 0;
                    case Comparison.LESS: return a.Compare(b)<0;
                    case Comparison.NOT_EQUALS: return a.Compare(b) != 0;
                    default: throw new Exception("Unsupported Comparison Type");
                }
            }
        }

        public static Comparison Inverse(Comparison comparisonType) {
            switch (comparisonType) {
                case Comparison.GREATER: return Comparison.LESS_OR_EQUAL;
                case Comparison.GREATER_OR_EQUAL: return Comparison.LESS;
                case Comparison.EQUAL: return Comparison.NOT_EQUALS;
                case Comparison.LESS_OR_EQUAL: return Comparison.GREATER;
                case Comparison.LESS: return Comparison.GREATER_OR_EQUAL;
                case Comparison.NOT_EQUALS: return Comparison.EQUAL;
                default: throw new Exception("Unsupported Comparison Type");
            }
        }
    }
}
