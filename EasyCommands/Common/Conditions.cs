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
            bool evaluate(Object block);
        }

        public class NotBlockCondition : BlockCondition {
            BlockCondition blockCondition;

            public NotBlockCondition(BlockCondition blockCondition) {
                this.blockCondition = blockCondition;
            }

            public bool evaluate(Object block) {
                return !blockCondition.evaluate(block);
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

            public bool evaluate(object block) {
                return conditionA.evaluate(block) && conditionB.evaluate(block);
            }
        }

        public class OrBlockCondition : BlockCondition {
            BlockCondition conditionA;
            BlockCondition conditionB;

            public OrBlockCondition(BlockCondition conditionA, BlockCondition conditionB) {
                this.conditionA = conditionA;
                this.conditionB = conditionB;
            }

            public bool evaluate(object block) {
                return conditionA.evaluate(block) || conditionB.evaluate(block);
            }
        }

        public class BlockPropertyCondition : BlockCondition {
            protected BlockHandler blockHandler;
            protected PropertyType? property;
            protected PrimitiveComparator comparator;
            protected Variable comparisonValue;

            public BlockPropertyCondition(BlockHandler blockHandler, PropertyType? property, PrimitiveComparator comparator, Variable comparisonValue) {
                this.blockHandler = blockHandler;
                this.property = property;
                this.comparator = comparator;
                this.comparisonValue = comparisonValue;
            }
            public bool evaluate(Object block) {
                Primitive value = comparisonValue.GetValue();
                PropertyType prop = ( property.HasValue ) ? property.Value : blockHandler.GetDefaultProperty(value.GetType());
                return comparator.compare(blockHandler.GetPropertyValue(block, prop), value);
            }

            public override String ToString() {
                return property + " " + comparator + " " + comparisonValue.GetValue();
            }
        }

        public class BlockDirectionPropertyCondition : BlockCondition {
            protected BlockHandler blockHandler;
            protected PropertyType? property;
            protected DirectionType direction;
            protected PrimitiveComparator comparator;
            protected Variable comparisonValue;

            public BlockDirectionPropertyCondition(BlockHandler blockHandler, PropertyType? property, DirectionType direction, PrimitiveComparator comparator, Variable comparisonValue) {
                this.blockHandler = blockHandler;
                this.property = property;
                this.comparator = comparator;
                this.comparisonValue = comparisonValue;
                this.direction = direction;
            }

            public bool evaluate(Object block) {
                Primitive value = comparisonValue.GetValue();
                PropertyType prop = property.GetValueOrDefault(blockHandler.GetDefaultProperty(value.GetType()));
                return comparator.compare(blockHandler.GetPropertyValue(block, prop, direction), value);
            }

            public override String ToString() {
                return property + " " + comparator + " " + comparisonValue.GetValue();
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
                    case ComparisonType.LESS_OR_EQUAL: return a.Compare(b)<=0;
                    case ComparisonType.LESS: return a.Compare(b)<0;
                    default: throw new Exception("Unsupported Comparison Type");
                }
            }
        }
    }
}
