﻿using Sandbox.Game.EntityComponents;
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
        public interface Variable {
            Primitive GetValue();
        }

        public class StaticVariable : Variable {
            public Primitive primitive;

            public StaticVariable(Primitive primitive) {
                this.primitive = primitive;
            }

            public Primitive GetValue() {
                return primitive;
            }
        }

        public class LockedVariable : Variable {
            public Variable locked;

            public LockedVariable(Variable locked) {
                this.locked = locked;
            }

            public Primitive GetValue() {
                return locked.GetValue();
            }
        }

        public class ComparisonVariable : Variable {
            public Variable a, b;
            public PrimitiveComparator comparator;

            public ComparisonVariable(Variable a, Variable b, PrimitiveComparator comparator) {
                this.a = a;
                this.b = b;
                this.comparator = comparator;
            }

            public Primitive GetValue() {
                return new BooleanPrimitive(comparator.compare(a.GetValue(), b.GetValue()));
            }
        }

        public class UniOperandVariable : Variable {
            public Variable a;
            public UniOperand operand;

            public UniOperandVariable(UniOperand operand, Variable a) {
                this.operand = operand;
                this.a = a;
            }

            public Primitive GetValue() {
                return PerformOperation(operand, a.GetValue());
            }
        }

        public class BiOperandVariable : Variable {
            public Variable a, b;
            public BiOperand operand;

            public BiOperandVariable(BiOperand operand, Variable a, Variable b) {
                this.operand = operand;
                this.a = a;
                this.b = b;
            }

            public Primitive GetValue() {
                return PerformOperation(operand, a.GetValue(), b.GetValue());
            }
        }

        public class AggregateConditionVariable : Variable {
            public AggregationMode aggregationMode;
            public BlockCondition blockCondition;
            public EntityProvider entityProvider;

            public AggregateConditionVariable(AggregationMode aggregationMode, BlockCondition blockCondition, EntityProvider entityProvider) {
                this.aggregationMode = aggregationMode;
                this.blockCondition = blockCondition;
                this.entityProvider = entityProvider;
            }

            public Primitive GetValue() {
                return new BooleanPrimitive(Evaluate());
            }

            public bool Evaluate() {
                List<Object> blocks = entityProvider.GetEntities();

                if (blocks.Count == 0) return false; //If there are no blocks, consider this not matching

                int matches = blocks.Count(block => blockCondition.evaluate(block, entityProvider.GetBlockType()));

                switch (aggregationMode) {
                    case AggregationMode.ALL: return matches == blocks.Count;
                    case AggregationMode.ANY: return matches > 0;
                    case AggregationMode.NONE: return matches == 0;
                    default: throw new Exception("Unsupported Aggregation Mode");
                }
            }

            public override String ToString() {
                return getAggregationModeName(aggregationMode) + " of " + entityProvider + " are " + blockCondition;
            }
        }

        public class AggregatePropertyVariable : Variable {
            public PropertyAggregate aggregationType;
            public EntityProvider entityProvider;
            public PropertySupplier property;
            public Direction? direction;

            public AggregatePropertyVariable(PropertyAggregate aggregationType, EntityProvider entityProvider, PropertySupplier property, Direction? direction) {
                this.aggregationType = aggregationType;
                this.entityProvider = entityProvider;
                this.property = property;
                this.direction = direction;
            }

            public Primitive GetValue() {
                List<Object> blocks = entityProvider.GetEntities();

                if(aggregationType == PropertyAggregate.COUNT) {
                    return new NumberPrimitive(blocks.Count);
                }

                BlockHandler handler = BlockHandlerRegistry.GetBlockHandler(entityProvider.GetBlockType());

                PropertySupplier p = property ?? handler.GetDefaultProperty(Return.NUMERIC);

                List<Primitive> propertyValues = blocks.Select(b => {
                    return direction.HasValue ? handler.GetPropertyValue(b, p, direction.Value) : handler.GetPropertyValue(b, p);
                }).ToList();

                switch(aggregationType) {
                    case PropertyAggregate.VALUE:
                        return ValueAggregator(propertyValues);
                    case PropertyAggregate.SUM:
                        return SumAggregator(propertyValues);
                    case PropertyAggregate.AVG:
                        return AverageAggregator(propertyValues);
                    case PropertyAggregate.MIN:
                        return MinAggregator(propertyValues);
                    case PropertyAggregate.MAX:
                        return MaxAggregator(propertyValues);
                    default: throw new Exception("Unknown Aggregation type: " + aggregationType);
                }
            }
        }

        public class AmbiguousStringVariable : Variable {
            public String value;

            public AmbiguousStringVariable(String value) {
                this.value = value;
            }

            public Primitive GetValue() {
                try {
                    return PROGRAM.GetVariable(value).GetValue();
                } catch(Exception) {
                    return new StringPrimitive(value);
                }
            }
        }

        public class InMemoryVariable : Variable {
            public String variableName;

            public InMemoryVariable(string variableName) {
                this.variableName = variableName;
            }

            public Primitive GetValue() => PROGRAM.GetVariable(variableName).GetValue();
        }
    }
}