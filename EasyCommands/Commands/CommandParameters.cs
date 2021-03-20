﻿using Sandbox.Game.EntityComponents;
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

        public static T findFirst<T>(List<CommandParameter> parameters) where T : class, CommandParameter {
            var i = parameters.FindIndex(p => p is T);
            if (i < 0) return null;
            T t = (T)parameters[i];
            return t;
        }

        public static T findLast<T>(List<CommandParameter> parameters) where T : class, CommandParameter {
            var i = parameters.FindLastIndex(p => p is T);
            if (i < 0) return null;
            T t = (T)parameters[i];
            return t;
        }

        public static T extractFirst<T>(List<CommandParameter> parameters) where T : class, CommandParameter {
            var i = parameters.FindIndex(p => p is T);
            if (i < 0) return null;
            T t = (T)parameters[i];
            parameters.RemoveAt(i);
            return t;
        }

        public static List<T> extract<T>(List<CommandParameter> parameters) where T : CommandParameter {
            int i = 0;
            List<T> extracted = new List<T>();
            while (i < parameters.Count) {
                if (parameters[i] is T) {
                    extracted.Add((T)parameters[i]);
                    parameters.RemoveAt(i);
                } else { i++; };
            }
            return extracted;
        }

        public interface CommandParameter { }
        public interface PrimitiveCommandParameter : CommandParameter { }
        public class IndexCommandParameter : CommandParameter { }
        public class GroupCommandParameter : CommandParameter { }
        public class AsyncCommandParameter : CommandParameter { }
        public class NotCommandParameter : CommandParameter { }
        public class AndCommandParameter : CommandParameter { }
        public class OrCommandParameter : CommandParameter { }
        public class OpenParenthesisCommandParameter : CommandParameter { }
        public class CloseParenthesisCommandParameter : CommandParameter { }
        public class IteratorCommandParameter : CommandParameter { }
        public class ActionCommandParameter : CommandParameter { }
        public class ReverseCommandParameter : CommandParameter { }
        public class RelativeCommandParameter : CommandParameter { }
        public class WaitCommandParameter : CommandParameter { }
        public class SendCommandParameter : CommandParameter { }
        public class ListenCommandParameter : CommandParameter { }
        public class ElseCommandParameter : CommandParameter { }

        public class VariableCommandParameter : ValueCommandParameter<Variable> {
            public VariableCommandParameter(Variable value) : base(value) {}
        }

        public abstract class ValueCommandParameter<T> : CommandParameter {
            public T Value;
            public ValueCommandParameter(T value) { this.Value = value; }
            public override string ToString() {
                return GetType() + " : " + Value;
            }
        }

        public class StringCommandParameter : ValueCommandParameter<String>, PrimitiveCommandParameter {
            public List<CommandParameter> SubTokens = new List<CommandParameter>();
            public StringCommandParameter(String value, params CommandParameter[] SubTokens) : base(value) {
                this.SubTokens = SubTokens.ToList();
            }
        }

        public class NumericCommandParameter : ValueCommandParameter<float>, PrimitiveCommandParameter {
            public NumericCommandParameter(float value) : base(value) {}
        }

        public class BooleanCommandParameter : ValueCommandParameter<bool>, PrimitiveCommandParameter {
            public BooleanCommandParameter(bool value) : base(value) {}
        }

        public class DirectionCommandParameter : ValueCommandParameter<DirectionType> {
            public DirectionCommandParameter(DirectionType value) : base(value) {}
        }

        public class PropertyCommandParameter : ValueCommandParameter<PropertyType> {
            public PropertyCommandParameter(PropertyType value) : base(value) {}
        }

        public class UnitCommandParameter : ValueCommandParameter<UnitType> {
            public UnitCommandParameter(UnitType value) : base(value) {}
        }

        public class IndexSelectorCommandParameter : ValueCommandParameter<Variable> {
            public IndexSelectorCommandParameter(Variable value) : base(value) {}
        }

        public class ControlCommandParameter : ValueCommandParameter<ControlType> {
            public ControlCommandParameter(ControlType value) : base(value) {}
        }

        public class FunctionCommandParameter : ValueCommandParameter<FunctionType> {
            public FunctionCommandParameter(FunctionType value) : base(value) {}
        }

        public class FunctionCallCommandParameter : CommandParameter {
            public FunctionType function;
            public String functionName;

            public FunctionCallCommandParameter(FunctionType function, string functionName) {
                this.function = function;
                this.functionName = functionName;
            }
        }

        public class WithCommandParameter : CommandParameter {
            public bool inverseCondition;
            public WithCommandParameter(bool inverseCondition) {
                this.inverseCondition = inverseCondition;
            }
        }

        public class IfCommandParameter : CommandParameter {
            public bool inverseCondition;
            public bool alwaysEvaluate;
            public bool swapCommands;

            public IfCommandParameter(bool inverseCondition, bool alwaysEvaluate, bool swapCommands) {
                this.inverseCondition = inverseCondition;
                this.alwaysEvaluate = alwaysEvaluate;
                this.swapCommands = swapCommands;
            }
        }

        public class ConditionCommandParameter : ValueCommandParameter<Variable> {
            public bool alwaysEvaluate;
            public bool swapCommands;

            public ConditionCommandParameter(Variable value, bool alwaysEvaluate, bool swapCommands) : base(value) {
                this.alwaysEvaluate = alwaysEvaluate;
                this.swapCommands = swapCommands;
            }
        }

        public class BlockConditionCommandParameter : ValueCommandParameter<BlockCondition> {
            public BlockConditionCommandParameter(BlockCondition value) : base(value) { }
        }

        public class CommandReferenceParameter : ValueCommandParameter<Command> {
            public CommandReferenceParameter(Command value) : base(value) { }
        }

        public class IterationCommandParameter : ValueCommandParameter<Variable> {
            public IterationCommandParameter(Variable value) : base(value) {}
        }

        public class AggregationModeCommandParameter : ValueCommandParameter<AggregationMode> {
            public AggregationModeCommandParameter(AggregationMode value) : base(value) {
            }
        }

        public class ComparisonCommandParameter : ValueCommandParameter<ComparisonType> {
            public ComparisonCommandParameter(ComparisonType value) : base(value) {
            }
        }

        public class SelectorCommandParameter : ValueCommandParameter<EntityProvider> {
            public SelectorCommandParameter(EntityProvider value) : base(value) {
            }
        }

        public class BlockTypeCommandParameter : ValueCommandParameter<BlockType> {
            public BlockTypeCommandParameter(BlockType value) : base(value) {}
        }
    }
}
