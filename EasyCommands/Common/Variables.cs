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
            void setValue(Variable v);
        }

        public class StaticVariable : Variable {
            Primitive primitive;

            public StaticVariable(Primitive primitive) {
                this.primitive = primitive;
            }

            public Primitive GetValue() {
                return primitive;
            }

            public void setValue(Variable v) {
                //static variables cannot be set
            }
        }

        public class LockedVariable : Variable {
            Variable locked;

            public LockedVariable(Variable locked) {
                this.locked = locked;
            }

            public Primitive GetValue() {
                return locked.GetValue();
            }

            public void setValue(Variable v) {
                locked.setValue(v);
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

            public void setValue(Variable v) {
                //void
            }
        }

        public class NotVariable : Variable {
            Variable v;

            public NotVariable(Variable v) {
                this.v = v;
            }

            public Primitive GetValue() {
                return v.GetValue().Not();
            }

            public void setValue(Variable v) {
                v.setValue(new NotVariable(v));
            }
        }

        public class AndVariable : Variable {
            Variable a, b;

            public AndVariable(Variable a, Variable b) {
                this.a = a;
                this.b = b;
            }

            public Primitive GetValue() {
                return new BooleanPrimitive(CastBoolean(a.GetValue()).GetBooleanValue() && CastBoolean(b.GetValue()).GetBooleanValue());
            }

            public void setValue(Variable v) {
                //void
            }
        }

        public class OrVariable : Variable {
            Variable a, b;

            public OrVariable(Variable a, Variable b) {
                this.a = a;
                this.b = b;
            }

            public Primitive GetValue() {
                return new BooleanPrimitive(CastBoolean(a.GetValue()).GetBooleanValue() || CastBoolean(b.GetValue()).GetBooleanValue());
            }

            public void setValue(Variable v) {
                //void
            }
        }

        public class AggregateConditionVariable : Variable {
            AggregationMode aggregationMode;
            BlockCondition blockCondition;
            EntityProvider entityProvider;

            public AggregateConditionVariable(AggregationMode aggregationMode, BlockCondition blockCondition, EntityProvider entityProvider) {
                this.aggregationMode = aggregationMode;
                this.blockCondition = blockCondition;
                this.entityProvider = entityProvider;
            }

            public Primitive GetValue() {
                return new BooleanPrimitive(Evaluate());
            }

            public void setValue(Variable v) {
                //void
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

            public void setValue(Variable v) {
                Program.memoryVariables[variableName] = new StaticVariable(v.GetValue());
            }
        }
    }
}