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
        public class ComputedVariable : IVariable {
            Func<Primitive> evaluate;

            public ComputedVariable(Func<Primitive> value) { evaluate = value; }
            public Primitive GetValue() => evaluate();
        }

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

        public static IVariable ComparisonVariable(IVariable a, IVariable b, PrimitiveComparator comparator) => new ComputedVariable(() => ResolvePrimitive(comparator(a.GetValue(), b.GetValue())));

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

        public static IVariable UniOperandVariable(UniOperand operand, IVariable a) => new ComputedVariable(() => PerformOperation(operand, a.GetValue()));

        public static IVariable BiOperandVariable(BiOperand operand, IVariable a, IVariable b) => new ComputedVariable(() => PerformOperation(operand, a.GetValue(), b.GetValue()));

        public static IVariable ListAggregateConditionVariable(AggregateCondition aggregateCondition, IVariable expectedList, PrimitiveComparator comparator, IVariable comparisonValue) => new ComputedVariable(() => {
                var list = CastList(expectedList.GetValue());
                return ResolvePrimitive(aggregateCondition(list.keyedValues.Count, list.keyedValues.Count(v => comparator(v.GetValue(), comparisonValue.GetValue()))));
            });

        public static IVariable AggregateConditionVariable(AggregateCondition aggregateCondition, BlockCondition blockCondition, ISelector entityProvider) => new ComputedVariable(() => {
                var blocks = entityProvider.GetEntities();
                return ResolvePrimitive(aggregateCondition(blocks.Count, blocks.Count(block => blockCondition(block, entityProvider.GetBlockType()))));
            });

        public static IVariable AggregatePropertyVariable(Aggregator aggregator, ISelector entityProvider, PropertySupplier property) => new ComputedVariable(() => {
                IBlockHandler handler = BlockHandlerRegistry.GetBlockHandler(entityProvider.GetBlockType());
                if (property.propertyValues.Count == 0) property = property.WithProperties(NewList(new PropertyValue(Property.NAME + "")));
                PropertySupplier p = property.Resolve(handler, Return.STRING);
                return aggregator(entityProvider.GetEntities(), b => handler.GetPropertyValue(b, p));
            });

        public class AmbiguousStringVariable : IVariable {
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

        public static IVariable ListAggregateVariable(IVariable expectedList, Aggregator aggregator) => new ComputedVariable(() => aggregator(CastList(expectedList.GetValue()).keyedValues, v => ((IVariable)v).GetValue()));

        public static IVariable IndexVariable(IVariable expectedIndex) => new ComputedVariable(() => {
                KeyedList list = CastList(expectedIndex.GetValue());
                if (list.keyedValues.Count == 1) {
                    Primitive onlyValue = list.GetValue(ResolvePrimitive(0)).GetValue();
                    if (onlyValue.returnType == Return.LIST) list = CastList(onlyValue);
                }
                return ResolvePrimitive(list);
            });

        public class ListIndexVariable : IVariable {
            public IVariable expectedList, index;

            public ListIndexVariable(IVariable list, IVariable i) {
                expectedList = list;
                index = IndexVariable(i);
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

            String Wrap(String value) => value.Contains(" ") ? $"\"{value}\"" : value;

            public bool Equals(KeyedVariable variable) => GetKey() == variable.GetKey() && GetValue().value.Equals(variable.GetValue().value);
            public int CompareTo(KeyedVariable other) => GetValue().CompareTo(other.GetValue());
        }

        public static KeyedVariable AsKeyedVariable(IVariable variable) => variable as KeyedVariable ?? new KeyedVariable(null, variable);
    }
}