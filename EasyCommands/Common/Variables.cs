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

        public class StaticVariable : Variable {
            public Primitive primitive;

            public StaticVariable(Primitive prim) {
                primitive = prim;
            }

            public Primitive GetValue() => primitive;
        }

        public class ComparisonVariable : Variable {
            public Variable a, b;
            public PrimitiveComparator comparator;

            public ComparisonVariable(Variable left, Variable right, PrimitiveComparator comp) {
                a = left;
                b = right;
                comparator = comp;
            }

            public Primitive GetValue() => ResolvePrimitive(comparator(a.GetValue(), b.GetValue()));
        }

        public class TernaryConditionVariable : Variable {
            public Variable condition, positiveValue, negativeValue;
            public Primitive GetValue() => CastBoolean(condition.GetValue()) ? positiveValue.GetValue() : negativeValue.GetValue();
        }

        public class VectorVariable : Variable {
            public Variable X, Y, Z;

            public Primitive GetValue() {
                if (NewList(X, Y, Z).All(v => v.GetValue().returnType == Return.NUMERIC))
                    return ResolvePrimitive(Vector(CastNumber(X.GetValue()), CastNumber(Y.GetValue()), CastNumber(Z.GetValue())));
                throw new Exception("Invalid Variable in Vector");
            }
        }

        public class UniOperandVariable : Variable {
            public Variable a;
            public UniOperand operand;

            public UniOperandVariable(UniOperand op, Variable v) {
                operand = op;
                a = v;
            }

            public Primitive GetValue() => PROGRAM.PerformOperation(operand, a.GetValue());
        }

        public class BiOperandVariable : Variable {
            public Variable a, b;
            public BiOperand operand;

            public BiOperandVariable(BiOperand op, Variable left, Variable right) {
                operand = op;
                a = left;
                b = right;
            }

            public Primitive GetValue() => PROGRAM.PerformOperation(operand, a.GetValue(), b.GetValue());
        }

        public class ListAggregateConditionVariable : Variable {
            public AggregationMode aggregationMode;
            public Variable expectedList;
            public PrimitiveComparator comparator;
            public Variable comparisonValue;

            public ListAggregateConditionVariable(AggregationMode aggregation, Variable list, PrimitiveComparator comp, Variable value) {
                aggregationMode = aggregation;
                expectedList = list;
                comparator = comp;
                comparisonValue = value;
            }

            public Primitive GetValue() {
                var list = CastList(expectedList.GetValue()).GetValues();
                return ResolvePrimitive(Evaluate(list.Count, list.Where(v => comparator(v.GetValue(), comparisonValue.GetValue())).Count(), aggregationMode));
            }
        }

        public class AggregateConditionVariable : Variable {
            public AggregationMode aggregationMode;
            public BlockCondition blockCondition;
            public Selector entityProvider;

            public AggregateConditionVariable(AggregationMode aggregation, BlockCondition condition, Selector provider) {
                aggregationMode = aggregation;
                blockCondition = condition;
                entityProvider = provider;
            }

            public Primitive GetValue() {
                var blocks = entityProvider.GetEntities();
                return ResolvePrimitive(Evaluate(blocks.Count, blocks.Count(block => blockCondition(block, entityProvider.GetBlockType())), aggregationMode));
            }
        }

        public static bool Evaluate(int count, int matches, AggregationMode aggregation) {
            switch (aggregation) {
                case AggregationMode.ALL: return count > 0 && matches == count;
                case AggregationMode.ANY: return matches > 0;
                case AggregationMode.NONE: return matches == 0;
                default: throw new Exception("Unsupported Aggregation Mode");
            }
        }

        public class AggregatePropertyVariable : Variable {
            public Aggregator aggregator;
            public Selector entityProvider;
            public PropertySupplier property;

            public AggregatePropertyVariable(Aggregator agg, Selector provider, PropertySupplier p) {
                aggregator = agg;
                entityProvider = provider;
                property = p;
            }

            public Primitive GetValue() {
                IBlockHandler handler = BlockHandlerRegistry.GetBlockHandler(entityProvider.GetBlockType());
                PropertySupplier p = property.Resolve(handler, Return.NUMERIC);
                return aggregator(entityProvider.GetEntities(), b => handler.GetPropertyValue(b, p));
            }
        }

        public class AmbiguousStringVariable : Variable {
            public String value;

            public AmbiguousStringVariable(String v) {
                value = v;
            }

            public Primitive GetValue() {
                try {
                    return PROGRAM.GetVariable(value).GetValue();
                } catch(Exception) {
                    return ResolvePrimitive(value);
                }
            }
        }

        public class ListAggregateVariable : Variable {
            public Variable expectedList;
            public Aggregator aggregator;

            public ListAggregateVariable(Variable list, Aggregator agg) {
                expectedList = list;
                aggregator = agg;
            }

            public Primitive GetValue() => aggregator(CastList(expectedList.GetValue()).GetValues(), v => ((Variable)v).GetValue());
        }

        public class IndexVariable : Variable {
            public Variable expectedIndex;

            public IndexVariable(Variable index) {
                expectedIndex = index;
            }

            public Primitive GetValue() {
                KeyedList list = CastList(expectedIndex.GetValue());
                if (list.GetValues().Count == 1) {
                    Primitive onlyValue = list.GetValue(ResolvePrimitive(0)).GetValue();
                    if (onlyValue.returnType == Return.LIST) list = CastList(onlyValue);
                }
                return ResolvePrimitive(list);
            }
        }

        public class ListIndexVariable : Variable {
            public Variable expectedList;
            public Variable index;

            public ListIndexVariable(Variable list, Variable i) {
                expectedList = list;
                index = new IndexVariable(i);
            }

            public Primitive GetValue() {
                var list = CastList(expectedList.GetValue());
                var values = GetIndexValues()
                    .Select(p => list.GetValue(p))
                    .ToList();
                if (values.Count == 0) return ResolvePrimitive(list);
                return values.Count == 1 ? values[0].GetValue() : ResolvePrimitive(NewKeyedList(values));
            }

            public void SetValue(Variable value) {
                var list = CastList(expectedList.GetValue());
                var indexes = GetIndexValues();
                if (indexes.Count == 0) indexes.AddRange(Enumerable.Range(0, list.GetValues().Count).Select(i => ResolvePrimitive(i)));
                indexes.ForEach(index => list.SetValue(index, value));
            }

            List<Primitive> GetIndexValues() => CastList(index.GetValue()).GetValues().Select(i => i.GetValue()).ToList();
        }

        public class KeyedVariable : Variable, IComparable<KeyedVariable> {
            public Variable Key, Value;

            public KeyedVariable(Variable key, Variable value) {
                Key = key;
                Value = value;
            }

            public bool HasKey() => Key != null;

            public String GetKey() => Key != null ? CastString(Key.GetValue()) : null;
            public Primitive GetValue() => Value.GetValue();

            public String Print() => (HasKey() ? Wrap(GetKey()) + "->" : "") + Wrap(CastString(GetValue()));

            public KeyedVariable DeepCopy() => new KeyedVariable(Key == null ? null : GetStaticVariable(Key.GetValue().DeepCopy().value), GetStaticVariable(Value.GetValue().DeepCopy().value));

            String Wrap(String value) => value.Contains(" ") ? "\"" + value + "\"" : value;

            public override bool Equals(Object variable) => GetKey() == ((KeyedVariable)variable).GetKey() && Value.GetValue().value.Equals(((KeyedVariable)variable).Value.GetValue().value);
            public override int GetHashCode() => base.GetHashCode();

            public int CompareTo(KeyedVariable other) => (int)CastNumber(PROGRAM.PerformOperation(BiOperand.COMPARE, GetValue(), other.GetValue()));
        }

        public static KeyedVariable AsKeyedVariable(Variable variable) => (variable is KeyedVariable) ? (KeyedVariable)variable : new KeyedVariable(null, variable);
    }
}