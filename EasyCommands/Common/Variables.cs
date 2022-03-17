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
        public interface IVariable {
            Primitive GetValue();
        }

        public class StaticVariable : IVariable {
            public Primitive primitive;

            public StaticVariable(Primitive prim) {
                primitive = prim;
            }

            public Primitive GetValue() => primitive;
        }

        public class ComparisonVariable : IVariable {
            public IVariable a, b;
            public PrimitiveComparator comparator;

            public ComparisonVariable(IVariable left, IVariable right, PrimitiveComparator comp) {
                a = left;
                b = right;
                comparator = comp;
            }

            public Primitive GetValue() => ResolvePrimitive(comparator(a.GetValue(), b.GetValue()));
        }

        public class TernaryConditionVariable : IVariable {
            public IVariable condition, positiveValue, negativeValue;
            public Primitive GetValue() => CastBoolean(condition.GetValue()) ? positiveValue.GetValue() : negativeValue.GetValue();
        }

        public class VectorVariable : IVariable {
            public IVariable X, Y, Z;

            public Primitive GetValue() {
                if (NewList(X, Y, Z).All(v => v.GetValue().returnType == Return.NUMERIC))
                    return ResolvePrimitive(Vector(CastNumber(X.GetValue()), CastNumber(Y.GetValue()), CastNumber(Z.GetValue())));
                throw new RuntimeException("Invalid Variable in Vector");
            }
        }

        public class UniOperandVariable : IVariable {
            public IVariable a;
            public UniOperand operand;

            public UniOperandVariable(UniOperand op, IVariable v) {
                operand = op;
                a = v;
            }

            public Primitive GetValue() => PerformOperation(operand, a.GetValue());
        }

        public class BiOperandVariable : IVariable {
            public IVariable a, b;
            public BiOperand operand;

            public BiOperandVariable(BiOperand op, IVariable left, IVariable right) {
                operand = op;
                a = left;
                b = right;
            }

            public Primitive GetValue() => PerformOperation(operand, a.GetValue(), b.GetValue());
        }

        public class ListAggregateConditionVariable : IVariable {
            public AggregationMode aggregationMode;
            public IVariable expectedList;
            public PrimitiveComparator comparator;
            public IVariable comparisonValue;

            public ListAggregateConditionVariable(AggregationMode aggregation, IVariable list, PrimitiveComparator comp, IVariable value) {
                aggregationMode = aggregation;
                expectedList = list;
                comparator = comp;
                comparisonValue = value;
            }

            public Primitive GetValue() {
                var list = CastList(expectedList.GetValue());
                return ResolvePrimitive(Evaluate(list.keyedValues.Count, list.keyedValues.Count(v => comparator(v.GetValue(), comparisonValue.GetValue())), aggregationMode));
            }
        }

        public class AggregateConditionVariable : IVariable {
            public AggregationMode aggregationMode;
            public BlockCondition blockCondition;
            public ISelector entityProvider;

            public AggregateConditionVariable(AggregationMode aggregation, BlockCondition condition, ISelector provider) {
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
                default: throw new RuntimeException("Unsupported Aggregation Mode");
            }
        }

        public class AggregatePropertyVariable : IVariable {
            public Aggregator aggregator;
            public ISelector entityProvider;
            public PropertySupplier property;

            public AggregatePropertyVariable(Aggregator agg, ISelector provider, PropertySupplier p) {
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

        public class AmbiguousStringVariable : IVariable {
            public String value;

            public AmbiguousStringVariable(String v) {
                value = v;
            }

            public Primitive GetValue() {
                try {
                    return PROGRAM.GetVariable(value).GetValue();
                } catch(RuntimeException) {
                    return ResolvePrimitive(value);
                }
            }
        }

        public class ListAggregateVariable : IVariable {
            public IVariable expectedList;
            public Aggregator aggregator;

            public ListAggregateVariable(IVariable list, Aggregator agg) {
                expectedList = list;
                aggregator = agg;
            }

            public Primitive GetValue() => aggregator(CastList(expectedList.GetValue()).keyedValues, v => ((IVariable)v).GetValue());
        }

        public class IndexVariable : IVariable {
            public IVariable expectedIndex;

            public IndexVariable(IVariable index) {
                expectedIndex = index;
            }

            public Primitive GetValue() {
                KeyedList list = CastList(expectedIndex.GetValue());
                if (list.keyedValues.Count == 1) {
                    Primitive onlyValue = list.GetValue(ResolvePrimitive(0)).GetValue();
                    if (onlyValue.returnType == Return.LIST) list = CastList(onlyValue);
                }
                return ResolvePrimitive(list);
            }
        }

        public class ListIndexVariable : IVariable {
            public IVariable expectedList, index;

            public ListIndexVariable(IVariable list, IVariable i) {
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

            public void SetValue(IVariable value) {
                var list = CastList(expectedList.GetValue());
                var indexes = GetIndexValues();
                if (indexes.Count == 0) indexes.AddRange(Range(0, list.keyedValues.Count).Select(i => ResolvePrimitive(i)));
                indexes.ForEach(index => list.SetValue(index, value));
            }

            List<Primitive> GetIndexValues() => CastList(index.GetValue()).keyedValues.Select(i => i.GetValue()).ToList();
        }

        public class KeyedVariable : IVariable, IComparable<KeyedVariable>, IEquatable<KeyedVariable> {
            public IVariable Key, Value;

            public KeyedVariable(IVariable key, IVariable value) {
                Key = key;
                Value = value;
            }

            public bool HasKey() => Key != null;

            public String GetKey() => Key != null ? CastString(Key.GetValue()) : null;
            public Primitive GetValue() => Value.GetValue();

            public String Print() => (HasKey() ? Wrap(GetKey()) + "->" : "") + Wrap(CastString(GetValue()));

            public KeyedVariable DeepCopy() => new KeyedVariable(Key == null ? null : GetStaticVariable(Key.GetValue().DeepCopy().value), GetStaticVariable(Value.GetValue().DeepCopy().value));

            String Wrap(String value) => value.Contains(" ") ? "\"" + value + "\"" : value;

            public bool Equals(KeyedVariable variable) => GetKey() == variable.GetKey() && GetValue().value.Equals(variable.GetValue().value);
            public int CompareTo(KeyedVariable other) => GetValue().CompareTo(other.GetValue());
        }

        public static KeyedVariable AsKeyedVariable(IVariable variable) => variable as KeyedVariable ?? new KeyedVariable(null, variable);
    }
}