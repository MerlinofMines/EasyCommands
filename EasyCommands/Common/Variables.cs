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

        public class NotVariable : Variable {
            public Variable v;

            public NotVariable(Variable v) {
                this.v = v;
            }

            public Primitive GetValue() {
                return v.GetValue().Not();
            }
        }

        public class AndVariable : Variable {
            public Variable a, b;

            public AndVariable(Variable a, Variable b) {
                this.a = a;
                this.b = b;
            }

            public Primitive GetValue() {
                return new BooleanPrimitive(CastBoolean(a.GetValue()).GetBooleanValue() && CastBoolean(b.GetValue()).GetBooleanValue());
            }
        }

        public class OrVariable : Variable {
            public Variable a, b;

            public OrVariable(Variable a, Variable b) {
                this.a = a;
                this.b = b;
            }

            public Primitive GetValue() {
                return new BooleanPrimitive(CastBoolean(a.GetValue()).GetBooleanValue() || CastBoolean(b.GetValue()).GetBooleanValue());
            }
        }

        public class OperandVariable : Variable {
            public Variable a, b;
            public OperandType operand;

            public OperandVariable(Variable a, Variable b, OperandType operand) {
                this.a = a;
                this.b = b;
                this.operand = operand;
            }

            public Primitive GetValue() {
                switch(operand) {
                    case OperandType.ADD: return a.GetValue().Plus(b.GetValue());
                    case OperandType.SUBTACT: return a.GetValue().Minus(b.GetValue());
                    case OperandType.MULTIPLY: return a.GetValue().Multiply(b.GetValue());
                    case OperandType.DIVIDE: return a.GetValue().Divide(b.GetValue());
                    case OperandType.MOD: return a.GetValue().Mod(b.GetValue());
                    default: throw new Exception("Unknown Operand type: " + operand);
                }
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

        public class InMemoryVariable : Variable {
            public String variableName;

            public InMemoryVariable(string variableName) {
                this.variableName = variableName;
            }

            public Primitive GetValue() {
                Variable variable;
                if(!Program.memoryVariables.TryGetValue(variableName, out variable)) throw new Exception("No Variable exists with name: " + variableName);
                return variable.GetValue();
            }
        }
    }
}