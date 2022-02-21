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
        public interface ICommandParameter {
            string Token { get; set; }
        }
        public abstract class SimpleCommandParameter : ICommandParameter {
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
        public class ElseCommandParameter : SimpleCommandParameter { }
        public class PrintCommandParameter : SimpleCommandParameter { }
        public class SelfCommandParameter : SimpleCommandParameter { }
        public class GlobalCommandParameter : SimpleCommandParameter { }
        public class IgnoreCommandParameter : SimpleCommandParameter { }
        public class ThatCommandParameter : SimpleCommandParameter { }
        public class KeyedVariableCommandParameter : SimpleCommandParameter { }
        public class TernaryConditionIndicatorParameter : SimpleCommandParameter { }
        public class ColonSeparatorParameter : SimpleCommandParameter { }
        public class TernaryConditionSeparatorParameter : SimpleCommandParameter { }
        public class MinusCommandParameter : SimpleCommandParameter { }
        public class RoundCommandParameter : SimpleCommandParameter { }
        public class CastCommandParameter : SimpleCommandParameter { }
        public class RelativeCommandParameter : SimpleCommandParameter { }

        public abstract class ValueCommandParameter<T> : SimpleCommandParameter {
            public T value;
            public ValueCommandParameter(T v) { value = v; }
        }

        public class ListenCommandParameter : ValueCommandParameter<bool> {
            public ListenCommandParameter(bool v) : base(v) { }
        }

        public class QueueCommandParameter : ValueCommandParameter<bool> {
            public QueueCommandParameter(bool async) : base(async) { }
        }

        public class UniOperationCommandParameter : ValueCommandParameter<UniOperand> {
            public UniOperationCommandParameter(UniOperand value) : base(value) { }
        }

        public class LeftUniOperationCommandParameter : ValueCommandParameter<UniOperand> {
            public LeftUniOperationCommandParameter(UniOperand value) : base(value) { }
        }

        public class BiOperandCommandParameter : ValueCommandParameter<BiOperand> {
            public int tier;
            public BiOperandCommandParameter(BiOperand value, int t) : base(value) {
                tier = t;
            }
        }

        public class TransferCommandParameter : ValueCommandParameter<bool> {
            public TransferCommandParameter(bool v) : base(v) {}
        }

        public class AssignmentCommandParameter : ValueCommandParameter<bool> {
            public AssignmentCommandParameter(bool reference = false) : base(reference) { }
        }

        public class IncreaseCommandParameter : ValueCommandParameter<bool> {
            public IncreaseCommandParameter(bool increase = true) : base(increase) { }
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

        public class VariableIncrementCommandParameter : ValueCommandParameter<bool> {
            public string variableName;
            public VariableIncrementCommandParameter(string variable, bool increase = true) : base(increase) {
                variableName = variable;
            }
        }

        public class VariableCommandParameter : ValueCommandParameter<IVariable> {
            public VariableCommandParameter(IVariable value) : base(value) {}
        }

        public class AmbiguousCommandParameter : SimpleCommandParameter {
            public List<ICommandParameter> alternatives;

            public AmbiguousCommandParameter(params ICommandParameter[] commands) {
                alternatives = commands.ToList();
            }
        }

        public class AmbiguousStringCommandParameter : ValueCommandParameter<String> {
            public List<ICommandParameter> subTokens;
            public bool isImplicit;
            public AmbiguousStringCommandParameter(String value, bool impl, params ICommandParameter[] SubTokens) : base(value) {
                subTokens = SubTokens.ToList();
                isImplicit = impl;
            }
        }

        public class BooleanCommandParameter : ValueCommandParameter<bool> {
            public BooleanCommandParameter(bool value) : base(value) {}
        }

        public class DirectionCommandParameter : ValueCommandParameter<Direction> {
            public DirectionCommandParameter(Direction value) : base(value) {}
        }

        public class ValuePropertyCommandParameter : ValueCommandParameter<ValueProperty> {
            public ValuePropertyCommandParameter(ValueProperty value) : base(value) {}
        }

        public class PropertyCommandParameter : ValueCommandParameter<Property> {
            public PropertyCommandParameter(Property value) : base(value) { }
        }

        public class PropertySupplierCommandParameter : ValueCommandParameter<PropertySupplier> {
            public PropertySupplierCommandParameter(PropertySupplier value) : base(value) {}
        }

        public class ListCommandParameter : ValueCommandParameter<IVariable> {
            public ListCommandParameter(IVariable v) : base(v) {}
        }

        public class ListIndexCommandParameter : ValueCommandParameter<ListIndexVariable> {
            public ListIndexCommandParameter(ListIndexVariable v) : base(v) {}
        }

        public class IndexSelectorCommandParameter : ValueCommandParameter<IVariable> {
            public IndexSelectorCommandParameter(IVariable value) : base(value) {}
        }

        public class FunctionCommandParameter : ValueCommandParameter<bool> {
            public FunctionCommandParameter(bool shouldSwitch) : base(shouldSwitch) {}
        }

        public class FunctionDefinitionCommandParameter : SimpleCommandParameter {
            public bool switchExecution;
            public Supplier<string> functionDefinition;

            public FunctionDefinitionCommandParameter(Supplier<string> definition, bool shouldSwitch = false) {
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

        public class ConditionCommandParameter : ValueCommandParameter<IVariable> {
            public bool alwaysEvaluate;
            public bool swapCommands;

            public ConditionCommandParameter(IVariable value, bool alwaysEval, bool swap) : base(value) {
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

        public class RepetitionCommandParameter : ValueCommandParameter<IVariable> {
            public RepetitionCommandParameter(IVariable value) : base(value) {}
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

        public class SelectorCommandParameter : ValueCommandParameter<ISelector> {
            public SelectorCommandParameter(ISelector value) : base(value) {
            }
        }

        public class BlockTypeCommandParameter : ValueCommandParameter<Block> {
            public BlockTypeCommandParameter(Block value) : base(value) {}
        }
    }
}
