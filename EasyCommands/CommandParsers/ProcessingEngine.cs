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

        Dictionary<Type, List<ParameterProcessor>> parameterProcessorsByParameterType = NewDictionary<Type, List<ParameterProcessor>>();
        List<ParameterProcessor> parameterProcessors = NewList<ParameterProcessor>(
            new ParenthesisProcessor(),
            new ListProcessor(),

            //SelectorVariableSelectorProcessor
            ThreeValueRule(Type<VariableSelectorCommandParameter>, requiredRight<AmbiguousStringCommandParameter>(), optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, selector, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null, group.HasValue(), new AmbiguousStringVariable(selector.value)))),
            ThreeValueRule(Type<VariableSelectorCommandParameter>, requiredRight<VariableCommandParameter>(), optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, selector, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null, group.HasValue(), selector.value))),

            //SelectorProcessor
            new BranchingProcessor<AmbiguousStringCommandParameter>(
                NoValueRule(Type<AmbiguousStringCommandParameter>,
                    p => p.subTokens.Count > 0 && p.subTokens[0] is AmbiguousCommandParameter,
                    p => p.subTokens),
                TwoValueRule(Type<AmbiguousStringCommandParameter>, optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                        (p, blockType, group) => {
                            if (!blockType.GetValue().HasValue()) {
                                BlockTypeCommandParameter type = findLast<BlockTypeCommandParameter>(p.subTokens);
                                if (type != null) blockType.SetValue(type);
                                GroupCommandParameter g = findLast<GroupCommandParameter>(p.subTokens);
                                if (g != null) group.SetValue(g);
                            }
                            return blockType.GetValue().HasValue();
                        },
                        (p, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.GetValue().value, group.HasValue(), p.isImplicit ? new AmbiguousStringVariable(p.value) : GetStaticVariable(p.value)))),
                NoValueRule(Type<AmbiguousStringCommandParameter>,
                    name => PROGRAM.functions.ContainsKey(name.value),
                    name => new FunctionDefinitionCommandParameter(() => name.value)),
                NoValueRule(Type<AmbiguousStringCommandParameter>, b => new StringCommandParameter(b.value, false))),

            NoValueRule(Type<AmbiguousCommandParameter>, p => p.alternatives.Count > 0, p => p.alternatives),

            OneValueRule(Type<ListCommandParameter>, requiredLeft<StringCommandParameter>(),
                (list, name) => new ListIndexCommandParameter(new ListIndexVariable(new InMemoryVariable(name.value), list.value))),

            OneValueRule(Type<ListIndexCommandParameter>, requiredRight<ListCommandParameter>(),
                (index, list) => new ListIndexCommandParameter(new ListIndexVariable(index.value, list.value))),

            OneValueRule(Type<ListCommandParameter>, requiredLeft<VariableCommandParameter>(),
                (list, variable) => new ListIndexCommandParameter(new ListIndexVariable(variable.value, list.value))),

            //SelfSelectorProcessor
            TwoValueRule(Type<SelfCommandParameter>, optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, blockType, group) => new SelectorCommandParameter(new SelfSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null))),

            //VariableSelectorProcessor
            TwoValueRule(Type<VariableCommandParameter>, requiredRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.value, group.HasValue(), p.value))),

            //ListSelectorProcessor
            TwoValueRule(Type<ListIndexCommandParameter>, requiredRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.value, group.HasValue(), p.value))),

            //ImplicitAllSelectorProcessor
            OneValueRule(Type<BlockTypeCommandParameter>, optionalRight<GroupCommandParameter>(),
                (blockType, group) => new SelectorCommandParameter(new BlockTypeSelector(blockType.value))),

            //IndexProcessor
            OneValueRule(Type<IndexCommandParameter>, requiredRight<VariableCommandParameter>(),
                (p, var) => new IndexSelectorCommandParameter(var.value)),

            //RedundantComparisonProcessor
            //"is not <" => "!<"
            //"is <" => "<"
            //"is not" => !=
            // "not greater than" => <
            OneValueRule(Type<ComparisonCommandParameter>, requiredEither<NotCommandParameter>(),
                (p, left) => new ComparisonCommandParameter((a, b) => !p.value(a, b))),
            OneValueRule(Type<ComparisonCommandParameter>, requiredRight<ComparisonCommandParameter>(),
                (p, right) => new ComparisonCommandParameter(right.value)),

            //IndexSelectorProcessor
            OneValueRule(Type<IndexSelectorCommandParameter>, requiredLeft<SelectorCommandParameter>(),
                (p, selector) => new SelectorCommandParameter(new IndexSelector(selector.value, p.value))),

            //ListProcessors
            OneValueRule(Type<ListCommandParameter>, requiredLeft<SelectorCommandParameter>(),
                (list, selector) => new SelectorCommandParameter(new IndexSelector(selector.value, new IndexVariable(list.value)))),

            new MultiListProcessor(),

            new IgnoreProcessor(),

            //FunctionProcessor
            OneValueRule(Type<StringCommandParameter>, requiredLeft<FunctionCommandParameter>(),
                (name, function) => new FunctionDefinitionCommandParameter(() => CastString(new AmbiguousStringVariable(name.value).GetValue()), function.value)),

            //PropertyProcessor
            NoValueRule(Type<PropertyCommandParameter>, p => new PropertySupplierCommandParameter(new PropertySupplier(p.value + "", p.Token))),

            //ValuePropertyProcessor
            //Needs to check left, then right, which is opposite the typical checks.
            OneValueRule(Type<ValuePropertyCommandParameter>, requiredLeft<StringCommandParameter>(),
                (p, v) => new PropertySupplierCommandParameter(new PropertySupplier(p.value + "", p.Token).WithAttributeValue(new AmbiguousStringVariable(v.value)))),
            OneValueRule(Type<ValuePropertyCommandParameter>, requiredLeft<VariableCommandParameter>(),
                (p, v) => new PropertySupplierCommandParameter(new PropertySupplier(p.value + "", p.Token).WithAttributeValue(v.value))),
            OneValueRule(Type<ValuePropertyCommandParameter>, requiredRight<VariableCommandParameter>(),
                (p, v) => new PropertySupplierCommandParameter(new PropertySupplier(p.value + "", p.Token).WithAttributeValue(v.value))),

            //AssignmentProcessor
            TwoValueRule(Type<AssignmentCommandParameter>, optionalRight<GlobalCommandParameter>(), requiredRight<StringCommandParameter>(),
                (p, g, name) => new VariableAssignmentCommandParameter(name.value, p.value, g.HasValue())),
            TwoValueRule(Type<AssignmentCommandParameter>, requiredRight<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, list, value) => AllSatisfied(list, value) && list.GetValue().value is ListIndexVariable,
                (p, list, value) => new CommandReferenceParameter(new ListVariableAssignmentCommand((ListIndexVariable)list.value, value.value, p.value))),

            //Primitive Processor
            new PrimitiveProcessor<PrimitiveCommandParameter>(),

            //ListPropertyAggregationProcessor
            OneValueRule(Type<ListIndexCommandParameter>, requiredLeft<PropertyAggregationCommandParameter>(),
                (list, aggregation) => new VariableCommandParameter(new ListAggregateVariable(list.value, aggregation.value))),

            //ListComparisonProcessor
            ThreeValueRule(Type<ListIndexCommandParameter>, requiredRight<ComparisonCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalLeft<AggregationModeCommandParameter>(),
                (list, comparison, value, aggregation) => new VariableCommandParameter(new ListAggregateConditionVariable(aggregation.HasValue() ? aggregation.GetValue().value : AggregationMode.ALL, list.value, comparison.value, value.value))),

            //ListIndexAsVariableProcessor
            NoValueRule(Type<ListIndexCommandParameter>, list => new VariableCommandParameter(list.value)),

            //MinusProcessor
            new BranchingProcessor<MinusCommandParameter>(
                NoValueRule(Type<MinusCommandParameter>, minus => new UniOperationCommandParameter(UniOperand.REVERSE)),
                NoValueRule(Type<MinusCommandParameter>, minus => new BiOperandCommandParameter(BiOperand.SUBTRACT, 3))
            ),

            //RoundProcessor
            new BranchingProcessor<RoundCommandParameter>(
                NoValueRule(Type<RoundCommandParameter>, round => new BiOperandCommandParameter(BiOperand.ROUND, 1)),
                NoValueRule(Type<RoundCommandParameter>, round => new LeftUniOperationCommandParameter(UniOperand.ROUND)),
                NoValueRule(Type<RoundCommandParameter>, round => new UniOperationCommandParameter(UniOperand.ROUND))
            ),

            //AfterUniOperationProcessor
            OneValueRule(Type<LeftUniOperationCommandParameter>, requiredLeft<VariableCommandParameter>(),
                (p, df) => new VariableCommandParameter(new UniOperandVariable(p.value, df.value))),

            //UniOperationProcessor
            OneValueRule(Type<UniOperationCommandParameter>, requiredRight<VariableCommandParameter>(),
                (p, df) => new VariableCommandParameter(new UniOperandVariable(p.value, df.value))),

            //VectorProcessor
            FourValueRule(Type<ColonSeparatorParameter>, requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(), requiredRight<ColonSeparatorParameter>(), requiredRight<VariableCommandParameter>(),
                (sep1, x, y, sep2, z) => AllSatisfied(x, y, z) && !(x.GetValue().value is VectorVariable || y.GetValue().value is VectorVariable || z.GetValue().value is VectorVariable),
                (sep1, x, y, sep2, z) => new VariableCommandParameter(new VectorVariable(x.value, y.value, z.value))),

            //Tier0OperationProcessor
            BiOperandProcessor(0),

            //Tier1OperationProcessor
            BiOperandProcessor(1),

            //Tier2OperationProcessor
            BiOperandProcessor(2),

            //Tier3OperationProcessor
            BiOperandProcessor(3),

            //VariableComparisonProcessor
            TwoValueRule(Type<ComparisonCommandParameter>, requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, left, right) => new VariableCommandParameter(new ComparisonVariable(left.value, right.value, p.value))),

            //NotProcessor
            OneValueRule(Type<NotCommandParameter>, requiredRight<VariableCommandParameter>(),
                (p, right) => new VariableCommandParameter(new UniOperandVariable(UniOperand.REVERSE, right.value))),

            //ReverseProcessor
            OneValueRule(Type<ReverseCommandParameter>, requiredRight<VariableCommandParameter>(),
                (p, right) => new VariableCommandParameter(new UniOperandVariable(UniOperand.REVERSE, right.value))),

            //AndProcessor
            TwoValueRule(Type<AndCommandParameter>, requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, left, right) => new VariableCommandParameter(new BiOperandVariable(BiOperand.AND, left.value, right.value))),

            //OrProcessor
            TwoValueRule(Type<OrCommandParameter>, requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, left, right) => new VariableCommandParameter(new BiOperandVariable(BiOperand.OR, left.value, right.value))),

            //Tier4OperationProcessor
            BiOperandProcessor(4),

            //KeyedVariableProcessor
            TwoValueRule(Type<KeyedVariableCommandParameter>, requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (keyed, left, right) => new VariableCommandParameter(new KeyedVariable(left.value, right.value))),

            //BlockConditionProcessors
            ThreeValueRule(Type<AndCommandParameter>, requiredLeft<BlockConditionCommandParameter>(), optionalRight<ThatCommandParameter>(), requiredRight<BlockConditionCommandParameter>(),
                (p, left, with, right) => new BlockConditionCommandParameter(PROGRAM.AndCondition(left.value, right.value))),
            ThreeValueRule(Type<OrCommandParameter>, requiredLeft<BlockConditionCommandParameter>(), optionalRight<ThatCommandParameter>(), requiredRight<BlockConditionCommandParameter>(),
                (p, left, with, right) => new BlockConditionCommandParameter(PROGRAM.OrCondition(left.value, right.value))),

            //ThatBlockConditionProcessor
            FourValueRule(Type<ThatCommandParameter>, requiredRight<ComparisonCommandParameter>(), optionalRight<PropertySupplierCommandParameter>(), optionalRight<DirectionCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (with, p, prop, dir, var) => p.Satisfied() && (var.GetValue().HasValue() || prop.GetValue().HasValue()),
                (with, p, prop, dir, var) => NewList<CommandParameter>(new ThatCommandParameter(), new BlockConditionCommandParameter(BlockPropertyCondition((prop.HasValue() ? prop.GetValue().value : new PropertySupplier()).WithDirection(dir.HasValue() ? dir.GetValue().value : (Direction?)null), new PrimitiveComparator(p.value), var.HasValue() ? var.GetValue().value : GetStaticVariable(true))))),

            //ConditionalSelectorProcessor
            TwoValueRule(Type<ThatCommandParameter>, requiredLeft<SelectorCommandParameter>(), requiredRight<BlockConditionCommandParameter>(),
                (p, selector, condition) => new SelectorCommandParameter(new ConditionalSelector(selector.value, condition.value))),

            //PropertyAggregationProcessor
            ThreeValueRule(Type<PropertyAggregationCommandParameter>, requiredEither<SelectorCommandParameter>(), optionalEither<PropertySupplierCommandParameter>(), optionalEither<DirectionCommandParameter>(),
                (p, selector, prop, dir) => new VariableCommandParameter(new AggregatePropertyVariable(p.value, selector.value, (prop.HasValue() ? prop.GetValue().value : new PropertySupplier()).WithDirection(dir.HasValue() ? dir.GetValue().value : (Direction?)null)))),

            //BlockComparisonProcessor
            ThreeValueRule(Type<ComparisonCommandParameter>, optionalEither<PropertySupplierCommandParameter>(), optionalEither<DirectionCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (p, prop, dir, var) => var.GetValue().HasValue() || prop.GetValue().HasValue(),
                (p, prop, dir, var) => new BlockConditionCommandParameter(BlockPropertyCondition((prop.HasValue() ? prop.GetValue().value : new PropertySupplier()).WithDirection(dir.HasValue() ? dir.GetValue().value : (Direction?)null), new PrimitiveComparator(p.value), var.HasValue() ? var.GetValue().value : GetStaticVariable(true)))),

            //AggregateConditionProcessor
            TwoValueRule(Type<BlockConditionCommandParameter>, optionalLeft<AggregationModeCommandParameter>(), requiredLeft<SelectorCommandParameter>(),
                (p, aggregation, selector) => new VariableCommandParameter(new AggregateConditionVariable(aggregation.HasValue() ? aggregation.GetValue().value : AggregationMode.ALL, p.value, selector.value))),

            //AggregateSelectorProcessor
            OneValueRule(Type<AggregationModeCommandParameter>, requiredRight<SelectorCommandParameter>(),
                (aggregation, selector) => aggregation.value != AggregationMode.NONE && selector.Satisfied(),
                (aggregation, selector) => selector),

            //RepetitionProcessor
            OneValueRule(Type<RepeatCommandParameter>, requiredLeft<VariableCommandParameter>(),
                (p, var) => new RepetitionCommandParameter(var.value)),

            //TransferCommandProcessor
            FourValueRule(Type<TransferCommandParameter>, requiredLeft<SelectorCommandParameter>(), requiredRight<SelectorCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (t, s1, s2, v1, v2) => new CommandReferenceParameter(new TransferItemCommand((t.value ? s1 : s2).value, (t.value ? s2 : s1).value, v1.value, v2.HasValue() ? v2.GetValue().value : null))),
            FourValueRule(Type<TransferCommandParameter>, requiredRight<SelectorCommandParameter>(), requiredRight<SelectorCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (t, s1, s2, v1, v2) => new CommandReferenceParameter(new TransferItemCommand(s1.value, s2.value, v1.value, v2.HasValue() ? v2.GetValue().value : null))),

            //Convert Ambiguous Colon to Ternary Condition Separator
            NoValueRule(Type<ColonSeparatorParameter>, b => new TernaryConditionSeparatorParameter()),

            //TernaryConditionProcessor
            FourValueRule(Type<TernaryConditionIndicatorParameter>, requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(), requiredRight<TernaryConditionSeparatorParameter>(), requiredRight<VariableCommandParameter>(),
                (i, conditionValue, positiveValue, seperator, negativeValue) => new VariableCommandParameter(new TernaryConditionVariable() {
                    condition = conditionValue.value,
                    positiveValue = positiveValue.value,
                    negativeValue = negativeValue.value
                })),

            //IfProcessor
            OneValueRule(Type<IfCommandParameter>, requiredRight<VariableCommandParameter>(),
                (p, var) => new ConditionCommandParameter(p.inverseCondition ? new UniOperandVariable(UniOperand.REVERSE, var.value) : var.value, p.alwaysEvaluate, p.swapCommands)),

            //AmbiguousSelectorPropertyProcessor
            new BranchingProcessor<SelectorCommandParameter>(
                BlockCommandProcessor(),
                TwoValueRule(Type<SelectorCommandParameter>, requiredEither<PropertySupplierCommandParameter>(), optionalEither<DirectionCommandParameter>(),
                    (s, p, d) => new VariableCommandParameter(new AggregatePropertyVariable(PROGRAM.SumAggregator, s.value, p.value.WithDirection(d.HasValue() ? d.GetValue().value : (Direction?)null)))),
                TwoValueRule(Type<SelectorCommandParameter>, optionalEither<PropertySupplierCommandParameter>(), optionalEither<DirectionCommandParameter>(),
                    (s, p, d) => p.GetValue().HasValue() || d.GetValue().HasValue(),//Must have at least one!
                    (s, p, d) => {
                        PropertySupplier property = p.HasValue() ? p.GetValue().value : new PropertySupplier();
                        Direction? direction = d.HasValue() ? d.GetValue().value : (Direction?)null;
                        if (direction == null) property = property.WithPropertyValue(GetStaticVariable(true));
                        return new CommandReferenceParameter(new BlockCommand(s.value, (b, e) =>
                            b.UpdatePropertyValue(e, property.WithDirection(direction).Resolve(b))));
                    })),

            //VariableIncrementCommand
            ThreeValueRule(Type<IncrementCommandParameter>, requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalLeft<IncrementCommandParameter>(),
                (i, v, d, i2) => AllSatisfied(v, d) && v.GetValue().value is AmbiguousStringVariable,
                (i, v, d, i2) => new CommandReferenceParameter(new VariableIncrementCommand(((AmbiguousStringVariable)v.value).value, i.value && (i2.HasValue() ? i2.GetValue().value : true), d.value))),
            OneValueRule(Type<IncrementCommandParameter>, requiredEither<VariableCommandParameter>(),
                (i, v) => v.Satisfied() && v.GetValue().value is AmbiguousStringVariable,
                (i, v) => new CommandReferenceParameter(new VariableIncrementCommand(((AmbiguousStringVariable)v.value).value, i.value, null))),

            //PrintCommandProcessor
            OneValueRule(Type<PrintCommandParameter>, requiredRight<VariableCommandParameter>(),
                (p, var) => new CommandReferenceParameter(new PrintCommand(var.value))),

            //WaitProcessor
            OneValueRule(Type<WaitCommandParameter>, optionalRight<VariableCommandParameter>(),
                (p, time) => new CommandReferenceParameter(new WaitCommand(time.HasValue() ? time.GetValue().value : GetStaticVariable(0.01666f)))),

            //FunctionCallCommandProcessor
            OneValueRule(Type<FunctionDefinitionCommandParameter>, rightList<VariableCommandParameter>(false),
                (p, variables) => new CommandReferenceParameter(new FunctionCommand(p.switchExecution, p.functionDefinition, variables.Select(v => v.value).ToList()))),

            //VariableAssignmentProcessor
            OneValueRule(Type<VariableAssignmentCommandParameter>, requiredRight<VariableCommandParameter>(),
                (p, var) => new CommandReferenceParameter(new VariableAssignmentCommand(p.variableName, var.value, p.useReference, p.isGlobal))),

            //SendCommandProcessor
            //Note: Message to send always comes first: "send <command> to <tag>" is only supported format
            TwoValueRule(Type<SendCommandParameter>, requiredRight<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, message, tag) => new CommandReferenceParameter(new SendCommand(message.value, tag.value))),

            //ListenCommandProcessor
            OneValueRule(Type<ListenCommandParameter>, requiredRight<VariableCommandParameter>(),
                (p, var) => new CommandReferenceParameter(new ListenCommand(var.value, p.value))),

            //IterationProcessor
            OneValueRule(Type<RepetitionCommandParameter>, requiredEither<CommandReferenceParameter>(),
                (p, command) => new CommandReferenceParameter(new MultiActionCommand(NewList(command.value), p.value))),

            //QueueProcessor
            OneValueRule(Type<QueueCommandParameter>, requiredRight<CommandReferenceParameter>(),
                (p, command) => new CommandReferenceParameter(new QueueCommand(command.value, p.value))),

            //IteratorProcessor
            ThreeValueRule(Type<IteratorCommandParameter>, requiredRight<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(), requiredEither<CommandReferenceParameter>(),
                (i, item, list, command) => AllSatisfied(list, command, item) && item.GetValue().value is AmbiguousStringVariable,
                (i, item, list, command) => new CommandReferenceParameter(new ForEachCommand(((AmbiguousStringVariable)item.value).value, list.value, command.value))),

            //ConditionalCommandProcessor
            //condition command
            //condition command otherwise command
            ThreeValueRule(Type<ConditionCommandParameter>, requiredRight<CommandReferenceParameter>(), optionalRight<ElseCommandParameter>(), optionalRight<CommandReferenceParameter>(),
                ConvertConditionalCommand),
            //command condition
            //command condition otherwise command
            ThreeValueRule(Type<ConditionCommandParameter>, requiredLeft<CommandReferenceParameter>(), optionalRight<ElseCommandParameter>(), optionalRight<CommandReferenceParameter>(),
                ConvertConditionalCommand)
        );

        /// <summary>
        /// This method inline processes the given list of command parameters.
        /// Any ambiguous parsing branches which were found during processing are also returned as additional entries.
        /// If the desired result (typically a command) does not result from the returned parse, the returned
        /// branches can be re-processed to see if a correct parse results from the alternate branches.
        /// This can continue until no alternate branches are returned.
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public List<List<CommandParameter>> ProcessParameters(List<CommandParameter> commandParameters) {
            var sortedParameterProcessors = new SortedList<ParameterProcessor, int>();
            var processorRanks = new HashSet<int>();

            var branches = NewList<List<CommandParameter>>();
            AddProcessors(commandParameters, sortedParameterProcessors, processorRanks);

            int processorIndex = 0;

            while (processorIndex < sortedParameterProcessors.Count) {
                bool revisit = false;
                bool processed = false;
                ParameterProcessor current = sortedParameterProcessors.Keys[processorIndex];
                for (int i = commandParameters.Count - 1; i >= 0; i--) {
                    if (current.CanProcess(commandParameters[i])) {
                        List<CommandParameter> finalParameters;
                        if (current.Process(commandParameters, i, out finalParameters, branches)) {
                            AddProcessors(finalParameters, sortedParameterProcessors, processorRanks);
                            processed = true;
                            break;
                        } else revisit = true;
                    }
                }
                if (processed) {
                    processorIndex = 0;
                    continue;
                }
                if (!revisit) {
                    sortedParameterProcessors.Remove(current);
                    processorRanks.Remove(current.Rank);
                } else processorIndex++;
            }

            return branches;
        }

        public void InitializeProcessors() {
            for (int i = 0; i < parameterProcessors.Count; i++) {
                ParameterProcessor processor = parameterProcessors[i];
                processor.Rank = i;

                List<Type> types = processor.GetProcessedTypes();
                foreach (Type t in types) {
                    if (!parameterProcessorsByParameterType.ContainsKey(t)) parameterProcessorsByParameterType[t] = NewList<ParameterProcessor>();
                    parameterProcessorsByParameterType[t].Add(processor);
                }
            }
        }

        public T ParseParameters<T>(List<CommandParameter> parameters) where T : class, CommandParameter {
            var branches = NewList<List<CommandParameter>>();
            branches.Add(parameters);

            //Branches
            while (branches.Count > 0) {
                branches.AddRange(ProcessParameters(branches[0]));
                if (branches[0].Count == 1 && branches[0][0] is T) {
                    return (T)branches[0][0];
                } else {
                    branches.RemoveAt(0);
                }
            }
            return null;
        }

        void AddProcessors(List<CommandParameter> types, SortedList<ParameterProcessor, int> sortedParameterProcessors, HashSet<int> processorRanks) {
            var processors = types.Select(t => t.GetType())
                .SelectMany(t => parameterProcessorsByParameterType.GetValueOrDefault(t, NewList<ParameterProcessor>()))
                .Where(p => !processorRanks.Contains(p.Rank));

            foreach (ParameterProcessor processor in processors) {
                sortedParameterProcessors[processor] = processor.Rank;
                processorRanks.Add(processor.Rank);
            }
        }

        static T findLast<T>(List<CommandParameter> parameters) where T : class, CommandParameter => parameters.Where(p => p is T).Select(p => (T)p).LastOrDefault();
    }
}
