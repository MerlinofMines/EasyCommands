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
        public interface Variable {
            Primitive GetValue();
        }

        public static Variable GetStaticVariable(object o) => new StaticVariable(ResolvePrimitive(o));
        public static Variable EmptyList() => new StaticVariable(ResolvePrimitive(GetVariables()));

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
                return PROGRAM.PerformOperation(operand, a.GetValue());
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
                return PROGRAM.PerformOperation(operand, a.GetValue(), b.GetValue());
            }
        }

        public class ListAggregateConditionVariable : Variable {
            public AggregationMode aggregationMode;
            public Variable expectedList;
            public PrimitiveComparator comparator;
            public Variable comparisonValue;

            public ListAggregateConditionVariable(AggregationMode aggregation, Variable list, Comparison comparison, Variable value) {
                aggregationMode = aggregation;
                expectedList = list;
                comparator = new PrimitiveComparator(comparison);
                comparisonValue = value;
            }

            public Primitive GetValue() {
                Primitive comparison = comparisonValue.GetValue();
                List<Variable> list = CastList(expectedList.GetValue()).GetTypedValue();
                return new BooleanPrimitive(Evaluate(list.Count, list.Where(v => comparator.compare(v.GetValue(), comparison)).Count(), aggregationMode));
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
                var blocks = entityProvider.GetEntities();
                return new BooleanPrimitive(Evaluate(blocks.Count, blocks.Count(block => blockCondition.evaluate(block, entityProvider.GetBlockType())), aggregationMode));
            }

            public override String ToString() {
                return getAggregationModeName(aggregationMode) + " of " + entityProvider + " are " + blockCondition;
            }
        }

        public static bool Evaluate(int count, int matches, AggregationMode aggregation) {
            if (count == 0) return false; //If there are none, consider this not matching

            switch (aggregation) {
                case AggregationMode.ALL: return matches == count;
                case AggregationMode.ANY: return matches > 0;
                case AggregationMode.NONE: return matches == 0;
                default: throw new Exception("Unsupported Aggregation Mode");
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

                return Aggregate(propertyValues, aggregationType);
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

        public class ListAggregateVariable : Variable {
            public Variable expectedList;
            public PropertyAggregate aggregation;

            public ListAggregateVariable(Variable list, PropertyAggregate agg) {
                expectedList = list;
                aggregation = agg;
            }

            public Primitive GetValue() => Aggregate(CastList(expectedList.GetValue()).GetTypedValue().Select(v => v.GetValue()).ToList(), aggregation);
        }

        public class ListIndexVariable : Variable {
            public Variable expectedList;
            public Variable index;

            public ListIndexVariable(Variable list, Variable i) {
                expectedList = list;
                index = i;
            }

            //TODO: Add Lookup by string support (Dictionary)
            //TODO: Add List support!  If index is list then return list containing at all requested indexes.  if empty, return expectedList
            public Primitive GetValue() {
                var list = GetList();
                var values = GetIndexValues()
                    .Where(i => i.GetPrimitiveType() == Return.NUMERIC)
                    .Select(p => list[(int)CastNumber(p).GetTypedValue()])
                    .ToList();
                if (values.Count == 0) return expectedList.GetValue();
                return values.Count == 1 ? values[0].GetValue() : new ListPrimitive(values);
            }

            //TODO: Support String indexes?
            public void SetValue(Variable value) {
                var list = CastList(expectedList.GetValue()).GetTypedValue();
                var indexes = GetIndexValues();
                if (indexes.Count == 0) indexes.AddRange(Enumerable.Range(0, list.Count).Select(i => ResolvePrimitive(i)));
                indexes.Where(i => i.GetPrimitiveType() == Return.NUMERIC)
                  .Select(p =>CastNumber(p).GetTypedValue())
                  .ForEach(n => list[(int)n] = value);
            }

            List<Variable> GetList() {
                Primitive list = expectedList.GetValue();
                return list.GetPrimitiveType() == Return.LIST ? CastList(list).GetTypedValue() : new List<Variable> { expectedList };
            }

            List<Primitive> GetIndexValues() {
                var primitives = new List<Primitive>();
                Primitive indexValue = index.GetValue();
                if (indexValue.GetPrimitiveType() == Return.LIST) {
                    primitives.AddRange(CastList(indexValue).GetTypedValue().Select(i => i.GetValue()).ToList());
                } else primitives.Add(indexValue);
                return primitives;
            }
        }

        public static Primitive Aggregate(List<Primitive> propertyValues, PropertyAggregate aggregationType) {
            switch (aggregationType) {
                case PropertyAggregate.COUNT:
                    return ResolvePrimitive(propertyValues.Count);
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
}