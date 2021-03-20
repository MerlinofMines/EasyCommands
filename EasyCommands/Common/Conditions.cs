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
            bool evaluate(Object block, BlockType blockType);
        }

        public class NotBlockCondition : BlockCondition {
            BlockCondition blockCondition;

            public NotBlockCondition(BlockCondition blockCondition) {
                this.blockCondition = blockCondition;
            }

            public bool evaluate(Object block, BlockType blockType) {
                return !blockCondition.evaluate(block, blockType);
            }
            public override String ToString() {
                return "not " + blockCondition;
            }
        }

        public class AndBlockCondition : BlockCondition {
            BlockCondition conditionA;
            BlockCondition conditionB;

            public AndBlockCondition(BlockCondition conditionA, BlockCondition conditionB) {
                this.conditionA = conditionA;
                this.conditionB = conditionB;
            }

            public bool evaluate(object block, BlockType blockType) {
                return conditionA.evaluate(block, blockType) && conditionB.evaluate(block, blockType);
            }
        }

        public class OrBlockCondition : BlockCondition {
            BlockCondition conditionA;
            BlockCondition conditionB;

            public OrBlockCondition(BlockCondition conditionA, BlockCondition conditionB) {
                this.conditionA = conditionA;
                this.conditionB = conditionB;
            }

            public bool evaluate(object block, BlockType blockType) {
                return conditionA.evaluate(block, blockType) || conditionB.evaluate(block, blockType);
            }
        }

        public class BlockPropertyCondition : BlockCondition {
            protected PropertyType? property;
            protected DirectionType? direction;
            protected PrimitiveComparator comparator;
            protected Variable comparisonValue;

            public BlockPropertyCondition(PropertyType? property, DirectionType? direction, PrimitiveComparator comparator, Variable comparisonValue) {
                this.property = property;
                this.comparator = comparator;
                this.comparisonValue = comparisonValue;
                this.direction = direction;
            }

            public bool evaluate(Object block, BlockType blockType) {
                BlockHandler handler = BlockHandlerRegistry.GetBlockHandler(blockType);
                Primitive value = comparisonValue.GetValue();
                PropertyType prop = property.HasValue ? property.Value : handler.GetDefaultProperty(value.GetType());
                if (direction.HasValue) return comparator.compare(handler.GetPropertyValue(block, prop, direction.Value), value);
                else return comparator.compare(handler.GetPropertyValue(block, prop), value);
            }

            public override String ToString() {
                return property + " " + comparator + " " + (direction.HasValue ? direction + " " : "") + comparisonValue.GetValue();
            }
        }

        public class PrimitiveComparator {
            ComparisonType comparisonType;
            public PrimitiveComparator(ComparisonType comparisonType) {
                this.comparisonType = comparisonType;
            }
            public bool compare(Primitive a, Primitive b) {
                switch (comparisonType) {
                    case ComparisonType.GREATER: return a.Compare(b)>0;
                    case ComparisonType.GREATER_OR_EQUAL: return a.Compare(b) >= 0;
                    case ComparisonType.EQUAL: return a.Compare(b) == 0;
                    case ComparisonType.LESS_OR_EQUAL: return a.Compare(b) <= 0;
                    case ComparisonType.LESS: return a.Compare(b)<0;
                    case ComparisonType.NOT_EQUALS: return a.Compare(b) != 0;
                    default: throw new Exception("Unsupported Comparison Type");
                }
            }
        }

        public static ComparisonType Inverse(ComparisonType comparisonType) {
            switch (comparisonType) {
                case ComparisonType.GREATER: return ComparisonType.LESS_OR_EQUAL;
                case ComparisonType.GREATER_OR_EQUAL: return ComparisonType.LESS;
                case ComparisonType.EQUAL: return ComparisonType.NOT_EQUALS;
                case ComparisonType.LESS_OR_EQUAL: return ComparisonType.GREATER;
                case ComparisonType.LESS: return ComparisonType.GREATER_OR_EQUAL;
                case ComparisonType.NOT_EQUALS: return ComparisonType.EQUAL;
                default: throw new Exception("Unsupported Comparison Type");
            }
        }
    }
}
