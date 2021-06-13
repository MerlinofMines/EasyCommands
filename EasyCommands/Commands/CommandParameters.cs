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
        public interface CommandParameter {
            string Token { get; set; }
        }
        public interface PrimitiveCommandParameter : CommandParameter { }
        public abstract class SimpleCommandParameter : CommandParameter {
            public string Token { get; set; }
        }
        public class IndexCommandParameter : SimpleCommandParameter { }
        public class GroupCommandParameter : SimpleCommandParameter { }
        public class NotCommandParameter : SimpleCommandParameter { }
        public class AndCommandParameter : SimpleCommandParameter { }
        public class OrCommandParameter : SimpleCommandParameter { }
        public class OpenParenthesisCommandParameter : SimpleCommandParameter { }
        public class CloseParenthesisCommandParameter : SimpleCommandParameter { }
        public class OpenBracketCommandParameter : SimpleCommandParameter { }
        public class ListSeparatorCommandParameter : SimpleCommandParameter { }
        public class CloseBracketCommandParameter : SimpleCommandParameter { }
        public class IteratorCommandParameter : SimpleCommandParameter { }
        public class ActionCommandParameter : SimpleCommandParameter { }
        public class ReverseCommandParameter : SimpleCommandParameter { }
        public class RelativeCommandParameter : SimpleCommandParameter { }
        public class WaitCommandParameter : SimpleCommandParameter { }
        public class SendCommandParameter : SimpleCommandParameter { }
        public class ListenCommandParameter : SimpleCommandParameter { }
        public class ElseCommandParameter : SimpleCommandParameter { }
        public class PrintCommandParameter : SimpleCommandParameter { }
        public class SelfCommandParameter : SimpleCommandParameter { }
        public class GlobalCommandParameter : SimpleCommandParameter { }
        public class IgnoreCommandParameter : SimpleCommandParameter { }

        public abstract class ValueCommandParameter<T> : CommandParameter {
            public T value;
            public ValueCommandParameter(T v) { value = v; }

            string CommandParameter.Token {
                get {
                    return value.ToString();
                }
                set { }
            }
        }

        public class QueueCommandParameter : ValueCommandParameter<bool> {
            public QueueCommandParameter(bool async) : base(async) {
            }
        }

        public class UniOperationCommandParameter : ValueCommandParameter<UniOperand> {
            public UniOperationCommandParameter(UniOperand value) : base(value) {
            }
        }

        public class LeftUniOperationCommandParameter : ValueCommandParameter<UniOperand> {
            public LeftUniOperationCommandParameter(UniOperand value) : base(value) {
            }
        }

        public class BiOperandTier1Operand : ValueCommandParameter<BiOperand> {
            public BiOperandTier1Operand(BiOperand value) : base(value) {}
        }

        public class BiOperandTier2Operand : ValueCommandParameter<BiOperand> {
            public BiOperandTier2Operand(BiOperand value) : base(value) {}
        }

        public class BiOperandTier3Operand : ValueCommandParameter<BiOperand> {
            public BiOperandTier3Operand(BiOperand value) : base(value) { }
        }

        public class TransferCommandParameter : ValueCommandParameter<bool> {
            public TransferCommandParameter(bool v) : base(v) {}
        }

        public class AssignmentCommandParameter : SimpleCommandParameter {
            public bool useReference;

            public AssignmentCommandParameter(bool useReference) {
                this.useReference = useReference;
            }
        }

        public class VariableAssignmentCommandParameter : SimpleCommandParameter {
            public string variableName;
            public bool useReference;
            public bool isGlobal;

            public VariableAssignmentCommandParameter(string variableName, bool useReference, bool isGlobal) {
                this.variableName = variableName;
                this.useReference = useReference;
                this.isGlobal = isGlobal;
            }
        }

        public class VariableCommandParameter : ValueCommandParameter<Variable> {
            public VariableCommandParameter(Variable value) : base(value) {}
        }

        public class VariableSelectorCommandParameter : ValueCommandParameter<Variable> {
            public VariableSelectorCommandParameter(Variable value) : base(value) { }
        }

        public class AmbiguiousStringCommandParameter : ValueCommandParameter<String>, PrimitiveCommandParameter {
            public List<CommandParameter> SubTokens = new List<CommandParameter>();
            public bool isImplicit;
            public AmbiguiousStringCommandParameter(String value, bool isImplicit, params CommandParameter[] SubTokens) : base(value) {
                this.SubTokens = SubTokens.ToList();
                this.isImplicit = isImplicit;
            }
        }

        public class StringCommandParameter : ValueCommandParameter<String>, PrimitiveCommandParameter {
            public bool isExplicit;

            public StringCommandParameter(string value, bool isExpl) : base(value) {
                isExplicit = isExpl;
            }
        }

        public class NumericCommandParameter : ValueCommandParameter<float>, PrimitiveCommandParameter {
            public NumericCommandParameter(float value) : base(value) {}
        }

        public class BooleanCommandParameter : ValueCommandParameter<bool>, PrimitiveCommandParameter {
            public BooleanCommandParameter(bool value) : base(value) {}
        }

        public class DirectionCommandParameter : ValueCommandParameter<Direction> {
            public DirectionCommandParameter(Direction value) : base(value) {}
        }

        public class ValuePropertyCommandParameter : ValueCommandParameter<ValueProperty> {
            public ValuePropertyCommandParameter(ValueProperty value) : base(value) {}
        }

        public class PropertyCommandParameter : ValueCommandParameter<PropertySupplier> {
            public PropertyCommandParameter(PropertySupplier value) : base(value) {}
            public PropertyCommandParameter(Property value) : base(new PropertySupplier(value)) { }
        }

        public class UnitCommandParameter : ValueCommandParameter<Unit> {
            public UnitCommandParameter(Unit value) : base(value) {}
        }

        public class ListCommandParameter : ValueCommandParameter<Variable> {
            public ListCommandParameter(Variable v) : base(v) {}
        }

        public class ListIndexCommandParameter : ValueCommandParameter<ListIndexVariable> {
            public ListIndexCommandParameter(ListIndexVariable v) : base(v) {}
        }

        public class IndexSelectorCommandParameter : ValueCommandParameter<Variable> {
            public IndexSelectorCommandParameter(Variable value) : base(value) {}
        }

        public class ControlCommandParameter : ValueCommandParameter<Control> {
            public ControlCommandParameter(Control value) : base(value) {}
        }

        public class FunctionCommandParameter : ValueCommandParameter<Function> {
            public FunctionCommandParameter(Function value) : base(value) {}
        }

        public class FunctionDefinitionCommandParameter : SimpleCommandParameter {
            public Function functionType;
            public FunctionDefinition functionDefinition;

            public FunctionDefinitionCommandParameter(Function functionType, FunctionDefinition functionDefinition) {
                this.functionType = functionType;
                this.functionDefinition = functionDefinition;
            }
        }

        public class WithCommandParameter : SimpleCommandParameter {
            public bool inverseCondition;
            public WithCommandParameter(bool inverseCondition) {
                this.inverseCondition = inverseCondition;
            }
        }

        public class IfCommandParameter : SimpleCommandParameter {
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

        public class PropertyAggregationCommandParameter : ValueCommandParameter<PropertyAggregate> {
            public PropertyAggregationCommandParameter(PropertyAggregate value) : base(value) {
            }
        }

        public class ComparisonCommandParameter : ValueCommandParameter<Comparison> {
            public ComparisonCommandParameter(Comparison value) : base(value) {
            }
        }

        public class SelectorCommandParameter : ValueCommandParameter<EntityProvider> {
            public SelectorCommandParameter(EntityProvider value) : base(value) {
            }
        }

        public class BlockTypeCommandParameter : ValueCommandParameter<Block> {
            public BlockTypeCommandParameter(Block value) : base(value) {}
        }
    }
}
