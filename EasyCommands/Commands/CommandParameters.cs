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
        public class VariableSelectorCommandParameter : SimpleCommandParameter { }
        public class NotCommandParameter : SimpleCommandParameter { }
        public class AndCommandParameter : SimpleCommandParameter { }
        public class OrCommandParameter : SimpleCommandParameter { }
        public class OpenParenthesisCommandParameter : SimpleCommandParameter { }
        public class CloseParenthesisCommandParameter : SimpleCommandParameter { }
        public class OpenBracketCommandParameter : SimpleCommandParameter { }
        public class ListSeparatorCommandParameter : SimpleCommandParameter { }
        public class CloseBracketCommandParameter : SimpleCommandParameter { }
        public class IteratorCommandParameter : SimpleCommandParameter { }
        public class RepeatCommandParameter : SimpleCommandParameter { }
        public class ReverseCommandParameter : SimpleCommandParameter { }
        public class WaitCommandParameter : SimpleCommandParameter { }
        public class SendCommandParameter : SimpleCommandParameter { }
        public class ListenCommandParameter : SimpleCommandParameter { }
        public class ElseCommandParameter : SimpleCommandParameter { }
        public class PrintCommandParameter : SimpleCommandParameter { }
        public class SelfCommandParameter : SimpleCommandParameter { }
        public class GlobalCommandParameter : SimpleCommandParameter { }
        public class IgnoreCommandParameter : SimpleCommandParameter { }
        public class ThatCommandParameter : SimpleCommandParameter { }
        public class KeyedVariableCommandParameter : SimpleCommandParameter { }
        public class TernaryConditionIndicatorParameter : SimpleCommandParameter { }
        public class TernaryConditionSeparatorParameter : SimpleCommandParameter { }

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

        public class AssignmentCommandParameter : ValueCommandParameter<bool> {
            public AssignmentCommandParameter(bool reference = false) : base(reference) { }
        }

        public class IncrementCommandParameter : ValueCommandParameter<bool> {
            public IncrementCommandParameter(bool increase = true) : base(increase) { }
        }

        public class VariableAssignmentCommandParameter : SimpleCommandParameter {
            public string variableName;
            public bool useReference;
            public bool isGlobal;

            public VariableAssignmentCommandParameter(string variable, bool reference, bool global) {
                variableName = variable;
                useReference = reference;
                isGlobal = global;
            }
        }

        public class VariableCommandParameter : ValueCommandParameter<Variable> {
            public VariableCommandParameter(Variable value) : base(value) {}
        }

        public class AmbiguiousStringCommandParameter : ValueCommandParameter<String>, PrimitiveCommandParameter {
            public List<CommandParameter> subTokens;
            public bool isImplicit;
            public AmbiguiousStringCommandParameter(String value, bool impl, params CommandParameter[] SubTokens) : base(value) {
                subTokens = SubTokens.ToList();
                isImplicit = impl;
            }
        }

        public class StringCommandParameter : ValueCommandParameter<String>, PrimitiveCommandParameter {
            public bool isExplicit;

            public StringCommandParameter(string value, bool isExpl) : base(value) {
                isExplicit = isExpl;
            }
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

        public class FunctionCommandParameter : ValueCommandParameter<bool> {
            public FunctionCommandParameter(bool shouldSwitch) : base(shouldSwitch) {}
        }

        public class FunctionDefinitionCommandParameter : SimpleCommandParameter {
            public bool switchExecution;
            public FunctionDefinition functionDefinition;

            public FunctionDefinitionCommandParameter(FunctionDefinition definition, bool shouldSwitch = false) {
                switchExecution = shouldSwitch;
                functionDefinition = definition;
            }
        }

        public class IfCommandParameter : SimpleCommandParameter {
            public bool inverseCondition;
            public bool alwaysEvaluate;
            public bool swapCommands;

            public IfCommandParameter(bool inverse, bool alwaysEval, bool swap) {
                inverseCondition = inverse;
                alwaysEvaluate = alwaysEval;
                swapCommands = swap;
            }
        }

        public class ConditionCommandParameter : ValueCommandParameter<Variable> {
            public bool alwaysEvaluate;
            public bool swapCommands;

            public ConditionCommandParameter(Variable value, bool alwaysEval, bool swap) : base(value) {
                alwaysEvaluate = alwaysEval;
                swapCommands = swap;
            }
        }

        public class BlockConditionCommandParameter : ValueCommandParameter<BlockCondition> {
            public BlockConditionCommandParameter(BlockCondition value) : base(value) { }
        }

        public class CommandReferenceParameter : ValueCommandParameter<Command> {
            public CommandReferenceParameter(Command value) : base(value) { }
        }

        public class RepetitionCommandParameter : ValueCommandParameter<Variable> {
            public RepetitionCommandParameter(Variable value) : base(value) {}
        }

        public class AggregationModeCommandParameter : ValueCommandParameter<AggregationMode> {
            public AggregationModeCommandParameter(AggregationMode value) : base(value) {
            }
        }

        public class PropertyAggregationCommandParameter : ValueCommandParameter<Aggregator> {
            public PropertyAggregationCommandParameter(Aggregator value) : base(value) {
            }
        }

        public class ComparisonCommandParameter : ValueCommandParameter<PrimitiveComparator> {
            public ComparisonCommandParameter(PrimitiveComparator value) : base(value) {
            }
        }

        public class SelectorCommandParameter : ValueCommandParameter<Selector> {
            public SelectorCommandParameter(Selector value) : base(value) {
            }
        }

        public class BlockTypeCommandParameter : ValueCommandParameter<Block> {
            public BlockTypeCommandParameter(Block value) : base(value) {}
        }
    }
}
