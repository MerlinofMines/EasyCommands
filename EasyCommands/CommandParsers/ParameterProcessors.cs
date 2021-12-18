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
    partial class Program : MyGridProgram {
        Dictionary<Type, List<ParameterProcessor>> parameterProcessorsByParameterType = NewDictionary<Type, List<ParameterProcessor>>();
        List<ParameterProcessor> parameterProcessors = NewList<ParameterProcessor>(
            new ParenthesisProcessor(),
            new ListProcessor(),

            //SelectorVariableSelectorProcessor
            ThreeValueRule<VariableSelectorCommandParameter, AmbiguiousStringCommandParameter, Optional<BlockTypeCommandParameter>, Optional<GroupCommandParameter>>(
                requiredRight<AmbiguiousStringCommandParameter>(), optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, selector, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null, group.HasValue(), new AmbiguousStringVariable(selector.value)))),
            ThreeValueRule<VariableSelectorCommandParameter, VariableCommandParameter, Optional<BlockTypeCommandParameter>, Optional<GroupCommandParameter>>(
                requiredRight<VariableCommandParameter>(), optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, selector, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null, group.HasValue(), selector.value))),

            //SelectorProcessor
            new BranchingProcessor<AmbiguiousStringCommandParameter>(
                TwoValueRule<AmbiguiousStringCommandParameter, Optional<BlockTypeCommandParameter>, Optional<GroupCommandParameter>>(
                        optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
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
                NoValueRule<AmbiguiousStringCommandParameter>(
                    name => PROGRAM.functions.ContainsKey(name.value),
                    name => new FunctionDefinitionCommandParameter(PROGRAM.functions[name.value])),
                NoValueRule<AmbiguiousStringCommandParameter>(b => new StringCommandParameter(b.value, false))),

            OneValueRule<ListCommandParameter, StringCommandParameter>(
                requiredLeft<StringCommandParameter>(),
                (list, name) => new ListIndexCommandParameter(new ListIndexVariable(new InMemoryVariable(name.value), list.value))),

            OneValueRule<ListIndexCommandParameter, ListCommandParameter>(
                requiredRight<ListCommandParameter>(),
                (index, list) => new ListIndexCommandParameter(new ListIndexVariable(index.value, list.value))),

            OneValueRule<ListCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(),
                (list, variable) => new ListIndexCommandParameter(new ListIndexVariable(variable.value, list.value))),

            //SelfSelectorProcessor
            TwoValueRule<SelfCommandParameter, Optional<BlockTypeCommandParameter>, Optional<GroupCommandParameter>>(
                optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, blockType, group) => new SelectorCommandParameter(new SelfSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null))),

            //VariableSelectorProcessor
            TwoValueRule<VariableCommandParameter, BlockTypeCommandParameter, Optional<GroupCommandParameter>>(
                requiredRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.value, group.HasValue(), p.value))),

            //ListSelectorProcessor
            TwoValueRule<ListIndexCommandParameter, BlockTypeCommandParameter, Optional<GroupCommandParameter>>(
                requiredRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.value, group.HasValue(), p.value))),

            //ImplicitAllSelectorProcessor
            OneValueRule<BlockTypeCommandParameter, Optional<GroupCommandParameter>>(
                optionalRight<GroupCommandParameter>(),
                (blockType, group) => new SelectorCommandParameter(new BlockTypeSelector(blockType.value))),

            //IndexProcessor
            OneValueRule<IndexCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, var) => new IndexSelectorCommandParameter(var.value)),

            //RedundantComparisonProcessor
            //"is not <" => "!<"
            //"is <" => "<"
            //"is not" => !=
            // "not greater than" => <
            OneValueRule<ComparisonCommandParameter, NotCommandParameter>(
                requiredEither<NotCommandParameter>(),
                (p, left) => new ComparisonCommandParameter((a, b) => !p.value(a, b))),
            OneValueRule<ComparisonCommandParameter, ComparisonCommandParameter>(
                requiredRight<ComparisonCommandParameter>(),
                (p, right) => new ComparisonCommandParameter(right.value)),

            //IndexSelectorProcessor
            OneValueRule<IndexSelectorCommandParameter, SelectorCommandParameter>(
                requiredLeft<SelectorCommandParameter>(),
                (p, selector) => new SelectorCommandParameter(new IndexSelector(selector.value, p.value))),

            //ListProcessors
            OneValueRule<ListCommandParameter, SelectorCommandParameter>(
                requiredLeft<SelectorCommandParameter>(),
                (list, selector) => new SelectorCommandParameter(new IndexSelector(selector.value, new IndexVariable(list.value)))),

            new MultiListProcessor(),

            new IgnoreProcessor(),

            //FunctionProcessor
            OneValueRule<StringCommandParameter, FunctionCommandParameter>(
                requiredLeft<FunctionCommandParameter>(),
                (name, function) => new FunctionDefinitionCommandParameter(PROGRAM.functions[name.value], function.value)),

            //AssignmentProcessor
            TwoValueRule<AssignmentCommandParameter, Optional<GlobalCommandParameter>, StringCommandParameter>(
                optionalRight<GlobalCommandParameter>(), requiredRight<StringCommandParameter>(),
                (p, g, name) => new VariableAssignmentCommandParameter(name.value, p.value, g.HasValue())),
            TwoValueRule<AssignmentCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, list, value) => AllSatisfied(list, value) && list.GetValue().value is ListIndexVariable,
                (p, list, value) => new CommandReferenceParameter(new ListVariableAssignmentCommand((ListIndexVariable)list.value, value.value, p.value))),

            //Primitive Processor
            new PrimitiveProcessor<PrimitiveCommandParameter>(),

            //ValuePropertyProcessor
            //Needs to check left, then right, which is opposite the typical checks.
            OneValueRule<ValuePropertyCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(),
                (p, v) => new PropertyCommandParameter(new PropertySupplier().WithPropertyType(p.value + "").WithAttributeValue(v.value))),
            OneValueRule<ValuePropertyCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, v) => new PropertyCommandParameter(new PropertySupplier().WithPropertyType(p.value + "").WithAttributeValue(v.value))),

            //ListPropertyAggregationProcessor
            OneValueRule<ListIndexCommandParameter, PropertyAggregationCommandParameter>(
                requiredLeft<PropertyAggregationCommandParameter>(),
                (list, aggregation) => new VariableCommandParameter(new ListAggregateVariable(list.value, aggregation.value))),

            //ListComparisonProcessor
            ThreeValueRule<ListIndexCommandParameter, ComparisonCommandParameter, VariableCommandParameter, Optional<AggregationModeCommandParameter>>(
                requiredRight<ComparisonCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalLeft<AggregationModeCommandParameter>(),
                (list, comparison, value, aggregation) => new VariableCommandParameter(new ListAggregateConditionVariable(aggregation.HasValue() ? aggregation.GetValue().value : AggregationMode.ALL, list.value, comparison.value, value.value))),

            //ListIndexAsVariableProcessor
            NoValueRule<ListIndexCommandParameter>(list => new VariableCommandParameter(list.value)),

            //AfterUniOperationProcessor
            OneValueRule<LeftUniOperationCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(),
                (p, df) => new VariableCommandParameter(new UniOperandVariable(p.value, df.value))),

            //UniOperationProcessor
            OneValueRule<UniOperationCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, df) => new VariableCommandParameter(new UniOperandVariable(p.value, df.value))),

            //Tier1OperationProcessor
            TwoValueRule<BiOperandTier1Operand, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, a, b) => new VariableCommandParameter(new BiOperandVariable(p.value, a.value, b.value))),

            //Tier2OperationProcessor
            TwoValueRule<BiOperandTier2Operand, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, a, b) => new VariableCommandParameter(new BiOperandVariable(p.value, a.value, b.value))),

            //VariableComparisonProcessor
            TwoValueRule<ComparisonCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, left, right) => new VariableCommandParameter(new ComparisonVariable(left.value, right.value, p.value))),

            //NotProcessor
            OneValueRule<NotCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, right) => new VariableCommandParameter(new UniOperandVariable(UniOperand.NOT, right.value))),

            //AndProcessor
            TwoValueRule<AndCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, left, right) => new VariableCommandParameter(new BiOperandVariable(BiOperand.AND, left.value, right.value))),

            //OrProcessor
            TwoValueRule<OrCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, left, right) => new VariableCommandParameter(new BiOperandVariable(BiOperand.OR, left.value, right.value))),

            //Tier3OperationProcessor
            TwoValueRule<BiOperandTier3Operand, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, a, b) => new VariableCommandParameter(new BiOperandVariable(p.value, a.value, b.value))),

            //KeyedVariableProcessor
            TwoValueRule<KeyedVariableCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (keyed, left, right) => new VariableCommandParameter(new KeyedVariable(left.value, right.value))),

            //BlockConditionProcessors
            ThreeValueRule<AndCommandParameter, BlockConditionCommandParameter, Optional<ThatCommandParameter>, BlockConditionCommandParameter>(
                requiredLeft<BlockConditionCommandParameter>(), optionalRight<ThatCommandParameter>(), requiredRight<BlockConditionCommandParameter>(),
                (p, left, with, right) => new BlockConditionCommandParameter(PROGRAM.AndCondition(left.value, right.value))),
            ThreeValueRule<OrCommandParameter, BlockConditionCommandParameter, Optional<ThatCommandParameter>, BlockConditionCommandParameter>(
                requiredLeft<BlockConditionCommandParameter>(), optionalRight<ThatCommandParameter>(), requiredRight<BlockConditionCommandParameter>(),
                (p, left, with, right) => new BlockConditionCommandParameter(PROGRAM.OrCondition(left.value, right.value))),

            //ThatBlockConditionProcessor
            FourValueRule<ThatCommandParameter, ComparisonCommandParameter, Optional<PropertyCommandParameter>, Optional<DirectionCommandParameter>, Optional<VariableCommandParameter>>(
                requiredRight<ComparisonCommandParameter>(), optionalRight<PropertyCommandParameter>(), optionalRight<DirectionCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (with, p, prop, dir, var) => p.Satisfied() && (var.GetValue().HasValue() || prop.GetValue().HasValue()),
                (with, p, prop, dir, var) => NewList<CommandParameter>(new ThatCommandParameter(), new BlockConditionCommandParameter(BlockPropertyCondition((prop.HasValue() ? prop.GetValue().value : new PropertySupplier()).WithDirection(dir.HasValue() ? dir.GetValue().value : (Direction?)null), new PrimitiveComparator(p.value), var.HasValue() ? var.GetValue().value : GetStaticVariable(true))))),

            //ConditionalSelectorProcessor
            TwoValueRule<ThatCommandParameter, SelectorCommandParameter, BlockConditionCommandParameter>(
                requiredLeft<SelectorCommandParameter>(), requiredRight<BlockConditionCommandParameter>(),
                (p, selector, condition) => new SelectorCommandParameter(new ConditionalSelector(selector.value, condition.value))),

            //PropertyAggregationProcessor
            ThreeValueRule<PropertyAggregationCommandParameter, SelectorCommandParameter, Optional<PropertyCommandParameter>, Optional<DirectionCommandParameter>>(
                requiredEither<SelectorCommandParameter>(), optionalEither<PropertyCommandParameter>(), optionalEither<DirectionCommandParameter>(),
                (p, selector, prop, dir) => new VariableCommandParameter(new AggregatePropertyVariable(p.value, selector.value, (prop.HasValue() ? prop.GetValue().value : new PropertySupplier()).WithDirection(dir.HasValue() ? dir.GetValue().value : (Direction?)null)))),

            //BlockComparisonProcessor
            ThreeValueRule<ComparisonCommandParameter, Optional<PropertyCommandParameter>, Optional<DirectionCommandParameter>, Optional<VariableCommandParameter>>(
                optionalEither<PropertyCommandParameter>(), optionalEither<DirectionCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (p, prop, dir, var) => var.GetValue().HasValue() || prop.GetValue().HasValue(),
                (p, prop, dir, var) => new BlockConditionCommandParameter(BlockPropertyCondition((prop.HasValue() ? prop.GetValue().value : new PropertySupplier()).WithDirection(dir.HasValue() ? dir.GetValue().value : (Direction?)null), new PrimitiveComparator(p.value), var.HasValue() ? var.GetValue().value : GetStaticVariable(true)))),

            //AggregateConditionProcessor
            TwoValueRule<BlockConditionCommandParameter, Optional<AggregationModeCommandParameter>, SelectorCommandParameter>(
                optionalLeft<AggregationModeCommandParameter>(), requiredLeft<SelectorCommandParameter>(),
                (p, aggregation, selector) => new VariableCommandParameter(new AggregateConditionVariable(aggregation.HasValue() ? aggregation.GetValue().value : AggregationMode.ALL, p.value, selector.value))),

            //AggregateSelectorProcessor
            OneValueRule<AggregationModeCommandParameter, SelectorCommandParameter>(
                requiredRight<SelectorCommandParameter>(),
                (aggregation, selector) => aggregation.value != AggregationMode.NONE && selector.Satisfied(),
                (aggregation, selector) => selector),

            //RepetitionProcessor
            OneValueRule<RepeatCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(),
                (p, var) => new RepetitionCommandParameter(var.value)),

            //TransferCommandProcessor
            FourValueRule<TransferCommandParameter, SelectorCommandParameter, SelectorCommandParameter, VariableCommandParameter, Optional<VariableCommandParameter>>(
                requiredLeft<SelectorCommandParameter>(), requiredRight<SelectorCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (t, s1, s2, v1, v2) => new CommandReferenceParameter(new TransferItemCommand((t.value ? s1 : s2).value, (t.value ? s2 : s1).value, v1.value, v2.HasValue() ? v2.GetValue().value : null))),
            FourValueRule<TransferCommandParameter, SelectorCommandParameter, SelectorCommandParameter, VariableCommandParameter, Optional<VariableCommandParameter>>(
                requiredRight<SelectorCommandParameter>(), requiredRight<SelectorCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (t, s1, s2, v1, v2) => new CommandReferenceParameter(new TransferItemCommand(s1.value, s2.value, v1.value, v2.HasValue() ? v2.GetValue().value : null))),

            //TernaryConditionProcessor
            FourValueRule<TernaryConditionIndicatorParameter, VariableCommandParameter, VariableCommandParameter, TernaryConditionSeparatorParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(), requiredRight<TernaryConditionSeparatorParameter>(), requiredRight<VariableCommandParameter>(),
                (i, conditionValue, positiveValue, seperator, negativeValue) => new VariableCommandParameter(new TernaryConditionVariable() {
                    condition = conditionValue.value,
                    positiveValue = positiveValue.value,
                    negativeValue = negativeValue.value
                })),

            //IfProcessor
            OneValueRule<IfCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, var) => new ConditionCommandParameter(p.inverseCondition ? new UniOperandVariable(UniOperand.NOT, var.value) : var.value, p.alwaysEvaluate, p.swapCommands)),

            //AmbiguousSelectorPropertyProcessor
            new BranchingProcessor<SelectorCommandParameter>(
                BlockCommandProcessor(),
                TwoValueRule<SelectorCommandParameter, PropertyCommandParameter, Optional<DirectionCommandParameter>>(
                    requiredEither<PropertyCommandParameter>(), optionalEither<DirectionCommandParameter>(),
                    (s, p, d) => new VariableCommandParameter(new AggregatePropertyVariable(PROGRAM.SumAggregator, s.value, p.value.WithDirection(d.HasValue() ? d.GetValue().value : (Direction?)null)))),
                TwoValueRule<SelectorCommandParameter, Optional<PropertyCommandParameter>, Optional<DirectionCommandParameter>>(
                    optionalEither<PropertyCommandParameter>(), optionalEither<DirectionCommandParameter>(),
                    (s, p, d) => p.GetValue().HasValue() || d.GetValue().HasValue(),//Must have at least one!
                    (s, p, d) => {
                        PropertySupplier property = p.HasValue() ? p.GetValue().value : new PropertySupplier();
                        Direction? direction = d.HasValue() ? d.GetValue().value : (Direction?)null;
                        if (direction == null) property = property.WithPropertyValue(GetStaticVariable(true));
                        return new CommandReferenceParameter(new BlockCommand(s.value, (b, e) =>
                            b.UpdatePropertyValue(e, property.WithDirection(direction).Resolve(b))));
                    })),

            //VariableIncrementCommand
            ThreeValueRule<IncrementCommandParameter, VariableCommandParameter, VariableCommandParameter, Optional<IncrementCommandParameter>>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalLeft<IncrementCommandParameter>(),
                (i, v, d, i2) => AllSatisfied(v, d) && v.GetValue().value is AmbiguousStringVariable,
                (i, v, d, i2) => new CommandReferenceParameter(new VariableIncrementCommand(((AmbiguousStringVariable)v.value).value, i.value && (i2.HasValue() ? i2.GetValue().value : true), d.value))),
            OneValueRule<IncrementCommandParameter, VariableCommandParameter>(
                requiredEither<VariableCommandParameter>(),
                (i, v) => v.Satisfied() && v.GetValue().value is AmbiguousStringVariable,
                (i, v) => new CommandReferenceParameter(new VariableIncrementCommand(((AmbiguousStringVariable)v.value).value, i.value, null))),

            //PrintCommandProcessor
            OneValueRule<PrintCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, var) => new CommandReferenceParameter(new PrintCommand(var.value))),

            //WaitProcessor
            OneValueRule<WaitCommandParameter, Optional<VariableCommandParameter>>(
                optionalRight<VariableCommandParameter>(),
                (p, time) => new CommandReferenceParameter(new WaitCommand(time.HasValue() ? time.GetValue().value : GetStaticVariable(0.0167f)))),

            //FunctionCallCommandProcessor
            OneValueRule<FunctionDefinitionCommandParameter, List<VariableCommandParameter>>(
                rightList<VariableCommandParameter>(true),
                (p, variables) => variables.GetValue().Count >= p.functionDefinition.parameterNames.Count,
                (p, variables) => {
                    List<VariableCommandParameter> parameters = variables;
                    var inputParameters = NewDictionary<string, Variable>();
                    var parameterCount = p.functionDefinition.parameterNames.Count;
                    for (int i = 0; i < parameterCount; i++) {
                        inputParameters[p.functionDefinition.parameterNames[i]] = parameters[i].value;
                    }
                    var results = NewList<CommandParameter>(new CommandReferenceParameter(new FunctionCommand(p.switchExecution, p.functionDefinition, inputParameters)));
                    if (parameters.Count > parameterCount) results.AddRange(parameters.GetRange(parameterCount, parameters.Count - parameterCount));
                    return results;
                }),

            //VariableAssignmentProcessor
            OneValueRule<VariableAssignmentCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, var) => new CommandReferenceParameter(new VariableAssignmentCommand(p.variableName, var.value, p.useReference, p.isGlobal))),

            //SendCommandProcessor
            //Note: Message to send always comes first: "send <command> to <tag>" is only supported format
            TwoValueRule<SendCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, message, tag) => new CommandReferenceParameter(new SendCommand(message.value, tag.value))),

            //ListenCommandProcessor
            OneValueRule<ListenCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, var) => new CommandReferenceParameter(new ListenCommand(var.value))),

            //ControlProcessor 
            NoValueRule<ControlCommandParameter>((p) => new CommandReferenceParameter(new ControlCommand(p.value))),

            //IterationProcessor
            OneValueRule<RepetitionCommandParameter, CommandReferenceParameter>(
                requiredEither<CommandReferenceParameter>(),
                (p, command) => new CommandReferenceParameter(new MultiActionCommand(NewList(command.value), p.value))),

            //QueueProcessor
            OneValueRule<QueueCommandParameter, CommandReferenceParameter>(
                requiredRight<CommandReferenceParameter>(),
                (p, command) => new CommandReferenceParameter(new QueueCommand(command.value, p.value))),

            //IteratorProcessor
            ThreeValueRule<IteratorCommandParameter, VariableCommandParameter, VariableCommandParameter, CommandReferenceParameter>(
                requiredRight<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(), requiredEither<CommandReferenceParameter>(),
                (i, item, list, command) => AllSatisfied(list, command, item) && item.GetValue().value is AmbiguousStringVariable,
                (i, item, list, command) => new CommandReferenceParameter(new ForEachCommand(((AmbiguousStringVariable)item.value).value, list.value, command.value))),

            //ConditionalCommandProcessor
            //condition command
            //condition command otherwise command
            ThreeValueRule<ConditionCommandParameter, CommandReferenceParameter, Optional<ElseCommandParameter>, Optional<CommandReferenceParameter>>(
                requiredRight<CommandReferenceParameter>(), optionalRight<ElseCommandParameter>(), optionalRight<CommandReferenceParameter>(),
                ConvertConditionalCommand),
            //command condition
            //command condition otherwise command
            ThreeValueRule<ConditionCommandParameter, CommandReferenceParameter, Optional<ElseCommandParameter>, Optional<CommandReferenceParameter>>(
                requiredLeft<CommandReferenceParameter>(), optionalRight<ElseCommandParameter>(), optionalRight<CommandReferenceParameter>(),
                ConvertConditionalCommand)
        );

        static CommandParameter ConvertConditionalCommand(ConditionCommandParameter condition, CommandReferenceParameter metFetcher,
            Optional<ElseCommandParameter> otherwise, Optional<CommandReferenceParameter> notMetFetcher) {
            Command metCommand = metFetcher.value;
            Command notMetCommand = otherwise.HasValue() ? notMetFetcher.GetValue().value : new NullCommand();
            if (condition.swapCommands) {
                var temp = metCommand;
                metCommand = notMetCommand;
                notMetCommand = temp;
            }
            Command command = new ConditionalCommand(condition.value, metCommand, notMetCommand, condition.alwaysEvaluate);
            return new CommandReferenceParameter(command);
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

            Debug(String.Join(" ", commandParameters.Select(p => CommandParameterToString(p)).ToList()));

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
                    Debug(String.Join(" ", commandParameters.Select(p => CommandParameterToString(p)).ToList()));
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

        void AddProcessors(List<CommandParameter> types, SortedList<ParameterProcessor, int> sortedParameterProcessors, HashSet<int> processorRanks) {
            var processors = types.Select(t => t.GetType())
                .SelectMany(t => parameterProcessorsByParameterType.GetValueOrDefault(t, NewList<ParameterProcessor>()))
                .Where(p => !processorRanks.Contains(p.Rank));

            foreach (ParameterProcessor processor in processors) {
                sortedParameterProcessors[processor] = processor.Rank;
                processorRanks.Add(processor.Rank);
            }
        }

        //New Data Processors
        interface DataProcessor {
            void Clear();
            bool SetValue(object p);
            bool Satisfied();
            bool Left(object p);
            bool Right(object p);
        }

        //Required Data Processors
        class DataProcessor<T> : DataProcessor {
            T value;
            public bool left, right, required;
            public virtual T GetValue() => value;
            public virtual bool SetValue(object p) {
                var setValue = p is T && value == null;
                if (setValue) value = (T)p;
                return setValue;
            }
            public bool Left(object o) => left && SetValue(o);
            public bool Right(object o) => right && SetValue(o);
            public virtual bool Satisfied() => value != null;
            public virtual void Clear() => value = default(T);
        }

        static DataProcessor<T> requiredRight<T>() => required<T>(false, true);
        static DataProcessor<T> requiredLeft<T>() => required<T>(true, false);
        static DataProcessor<T> requiredEither<T>() => required<T>(true, true);
        static DataProcessor<T> required<T>(bool left, bool right) => new DataProcessor<T> {
            left = left,
            right = right
        };

        //Optional Data Processors
        class OptionalDataProcessor<T> : DataProcessor<Optional<T>> {
            T value;
            public override Optional<T> GetValue() => new Optional<T> { t = value };
            public override bool SetValue(object p) {
                var setValue = p is T && value == null;
                if (setValue) value = (T)p;
                return setValue;
            }
            public override bool Satisfied() => true;
            public override void Clear() => value = default(T);
        }

        public class Optional<T> {
            public T t;
            public bool HasValue() => t != null;
            public T GetValue(T defaultValue = default(T)) => t != null ? t : defaultValue;
        }

        static OptionalDataProcessor<T> optionalRight<T>() => optional<T>(false, true);
        static OptionalDataProcessor<T> optionalLeft<T>() => optional<T>(true, false);
        static OptionalDataProcessor<T> optionalEither<T>() => optional<T>(true, true);
        static OptionalDataProcessor<T> optional<T>(bool left, bool right) => new OptionalDataProcessor<T> {
            left = left,
            right = right
        };

        //ListDataProcessors
        class ListDataProcessor<T> : DataProcessor<List<T>> {
            List<T> values = NewList<T>();
            public override bool SetValue(object p) {
                if (p is T) values.Add((T)p);
                return p is T;
            }
            public override bool Satisfied() => !required || values.Count > 0;
            public override List<T> GetValue() => values;
            public override void Clear() => values.Clear();
        }

        static ListDataProcessor<T> rightList<T>(bool required) => list<T>(false, true, required);
        static ListDataProcessor<T> leftList<T>(bool required) => list<T>(true, false, required);
        static ListDataProcessor<T> eitherList<T>(bool required) => list<T>(true, true, required);
        static ListDataProcessor<T> list<T>(bool left, bool right, bool required) => new ListDataProcessor<T> {
            left = left,
            right = right,
            required = required
        };

        //ParameterProcessors
        public interface ParameterProcessor : IComparable<ParameterProcessor> {
            int Rank { get; set; }
            List<Type> GetProcessedTypes();
            bool CanProcess(CommandParameter p); 
            bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters, List<List<CommandParameter>> branches);
        }

        public abstract class ParameterProcessor<T> : ParameterProcessor where T : class, CommandParameter {
            public int Rank { get; set; }
            public virtual List<Type> GetProcessedTypes() => NewList(typeof(T));
            public int CompareTo(ParameterProcessor other) => Rank.CompareTo(other.Rank);
            public bool CanProcess(CommandParameter p) => p is T;
            public abstract bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters, List<List<CommandParameter>> branches);
        }

        public class IgnoreProcessor : ParameterProcessor<IgnoreCommandParameter> {
            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters, List<List<CommandParameter>> branches) {
                finalParameters = NewList<CommandParameter>();
                p.RemoveAt(i);
                return true;
            }
        }

        public class ParenthesisProcessor : ParameterProcessor<OpenParenthesisCommandParameter> {
            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters, List<List<CommandParameter>> branches) {
                finalParameters = null;
                for(int j = i + 1; j < p.Count; j++) {
                    if (p[j] is OpenParenthesisCommandParameter) return false;
                    else if (p[j] is CloseParenthesisCommandParameter) {
                        finalParameters = p.GetRange(i+1, j - (i+1));
                        var alternateBranches = PROGRAM.ProcessParameters(finalParameters);
                        p.RemoveRange(i, j - i + 1);

                        for (int k = 0; k < alternateBranches.Count; k++) {
                            var copy = new List<CommandParameter>(p);
                            copy.InsertRange(i, alternateBranches[k]);
                            branches.Add(copy);
                        }

                        p.InsertRange(i, finalParameters);
                        return true;
                    }
                }
                throw new Exception("Missing Closing Parenthesis for Command");
            }
        }

        public class ListProcessor : ParameterProcessor<OpenBracketCommandParameter> {
            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters, List<List<CommandParameter>> branches) {
                finalParameters = null;
                var indexValues = NewList<Variable>();
                int startIndex = i;
                for (int j = startIndex + 1; j < p.Count; j++) {
                    if (p[j] is OpenBracketCommandParameter) return false;
                    else if (p[j] is ListSeparatorCommandParameter) {
                        indexValues.Add(ParseVariable(p, startIndex, j));
                        startIndex = j; //set startIndex to next separator
                    }
                    else if (p[j] is CloseBracketCommandParameter) {
                        if (j > i + 1) indexValues.Add(ParseVariable(p, startIndex, j)); //dont try to parse []
                        finalParameters = NewList<CommandParameter>(new ListCommandParameter(GetStaticVariable(new KeyedList(indexValues.ToArray()))));
                        p.RemoveRange(i, j - i + 1);
                        p.InsertRange(i, finalParameters);
                        return true;
                    }
                }
                throw new Exception("Missing Closing Bracket for List");
            }

            Variable ParseVariable(List<CommandParameter> p, int startIndex, int endIndex) {
                var range = p.GetRange(startIndex + 1, endIndex - (startIndex + 1));
                ValueCommandParameter<Variable> variable = PROGRAM.ParseParameters<ValueCommandParameter<Variable>>(range);
                if (variable == null) throw new Exception("List Index Values Must Resolve To a Variable");
                return variable.value;
            }
        }

        public class MultiListProcessor : ParameterProcessor<ListCommandParameter> {
            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters, List<List<CommandParameter>> branches) {
                while (i > 1 && p[i-1] is ListCommandParameter) i--;
                finalParameters = NewList<CommandParameter>(new ListIndexCommandParameter(new ListIndexVariable(((ListCommandParameter)p[i]).value, GetVariables(new KeyedList())[0])));
                p[i] = finalParameters[0];
                return true;
            }
        }

        public class PrimitiveProcessor<T> : ParameterProcessor<T> where T : class, PrimitiveCommandParameter {
            public override List<Type> GetProcessedTypes() => NewList(typeof(BooleanCommandParameter), typeof(StringCommandParameter));

            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters, List<List<CommandParameter>> branches) {
                if (p[i] is StringCommandParameter) {
                    p[i] = GetParameter((StringCommandParameter)p[i]);
                } else if (p[i] is BooleanCommandParameter) {
                    p[i] = new VariableCommandParameter(GetStaticVariable(((BooleanCommandParameter)p[i]).value));
                } else {
                    finalParameters = null;
                    return false;
                }
                finalParameters = NewList(p[i]);
                return true;
            }

            VariableCommandParameter GetParameter(StringCommandParameter value) {
                Primitive primitive;
                ParsePrimitive(value.value, out primitive);

                Variable variable = new AmbiguousStringVariable(value.value);

                if (primitive != null || value.isExplicit) variable = new StaticVariable(primitive ?? ResolvePrimitive(value.value));
                return new VariableCommandParameter(variable);
            }
        }

        class BranchingProcessor<T> : ParameterProcessor<T> where T : class, CommandParameter {
            List<ParameterProcessor<T>> processors;

            public BranchingProcessor(params ParameterProcessor<T>[] p) {
                processors = NewList(p);
            }

            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters, List<List<CommandParameter>> branches) {
                finalParameters = null;
                var eligibleProcessors = processors.Where(processor => processor.CanProcess(p[i])).ToList();
                var copy = new List<CommandParameter>(p);

                bool processed = false;
                foreach(ParameterProcessor processor in eligibleProcessors) { 
                    if (processed) {
                        List<CommandParameter> ignored;
                        var additionalCopy = new List<CommandParameter>(copy);
                        if (processor.Process(additionalCopy, i, out ignored, branches)) {
                            branches.Insert(0, additionalCopy);
                        }
                    } else {
                        processed = processor.Process(p, i, out finalParameters, branches);
                    }
                }
                return processed;
            }
        }

        class RuleProcessor<T> : ParameterProcessor<T> where T : class, CommandParameter {
            List<DataProcessor> processors;
            CanConvert<T> canConvert;
            Convert<T> convert;

            public RuleProcessor(List<DataProcessor> proc, CanConvert<T> canConv, Convert<T> conv) {
                processors = proc;
                canConvert = canConv;
                convert = conv;
            }

            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters, List<List<CommandParameter>> branches) {
                finalParameters = null;
                processors.ForEach(dp => dp.Clear());
                int j = i + 1;
                while (j < p.Count) {
                    if (processors.Exists(dp => dp.Right(p[j]))) j++;
                    else break;
                }

                int k = i;
                while (k > 0) {
                    if (processors.Exists(dp => dp.Left(p[k - 1]))) k--;
                    else break;
                }

                var commandParameters = p.GetRange(k, j - k);

                T hook = (T)p[i];
                if (!canConvert(hook)) return false;
                var converted = convert(hook);

                p.RemoveRange(k, j - k);
                if (converted is CommandParameter) {
                    finalParameters = NewList<CommandParameter>((CommandParameter)converted);
                } else if (converted is List<CommandParameter>) {
                    finalParameters = (List<CommandParameter>)converted;
                } else throw new Exception("Final parameters must be CommandParameter");
                p.InsertRange(k, finalParameters);
                return true;
            }
        }

        //Utility methods efficiently create Rule Processors
        delegate bool CanConvert<T>(T t);
        delegate bool OneValueCanConvert<T, U>(T t, DataProcessor<U> a);
        delegate bool TwoValueCanConvert<T, U, V>(T t, DataProcessor<U> a, DataProcessor<V> b);
        delegate bool ThreeValueCanConvert<T, U, V, W>(T t, DataProcessor<U> a, DataProcessor<V> b, DataProcessor<W> c);
        delegate bool FourValueCanConvert<T, U, V, W, X>(T t, DataProcessor<U> a, DataProcessor<V> b, DataProcessor<W> c, DataProcessor<X> d);

        delegate object Convert<T>(T t);
        delegate object OneValueConvert<T, U>(T t, U a);
        delegate object TwoValueConvert<T, U, V>(T t, U a, V b);
        delegate object ThreeValueConvert<T, U, V, W>(T t, U a, V b, W c);
        delegate object FourValueConvert<T, U, V, W, X>(T t, U a, V b, W c, X d);

        static RuleProcessor<T> NoValueRule<T>(Convert<T> convert) where T : class, CommandParameter => NoValueRule((p) => true, convert);

        static RuleProcessor<T> NoValueRule<T>(CanConvert<T> canConvert, Convert<T> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(), canConvert, convert);

        static RuleProcessor<T> OneValueRule<T, U>(DataProcessor<U> u, OneValueConvert<T, U> convert) where T : class, CommandParameter =>
            OneValueRule(u, (p, a) => a.Satisfied(), convert);

        static RuleProcessor<T> OneValueRule<T, U>(DataProcessor<U> u, OneValueCanConvert<T, U> canConvert, OneValueConvert<T, U> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u), (p) => canConvert(p, u), (p) => convert(p, u.GetValue()));

        static RuleProcessor<T> TwoValueRule<T, U, V>(DataProcessor<U> u, DataProcessor<V> v, TwoValueConvert<T, U, V> convert) where T : class, CommandParameter =>
            TwoValueRule(u, v, (p, a, b) => a.Satisfied() && b.Satisfied(), convert);

        static RuleProcessor<T> TwoValueRule<T, U, V>(DataProcessor<U> u, DataProcessor<V> v, TwoValueCanConvert<T, U, V> canConvert, TwoValueConvert<T, U, V> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v), (p) => canConvert(p, u, v), (p) => convert(p, u.GetValue(), v.GetValue()));

        static RuleProcessor<T> ThreeValueRule<T, U, V, W>(DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, ThreeValueConvert<T, U, V, W> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w), p => AllSatisfied(u, v, w), p => convert(p, u.GetValue(), v.GetValue(), w.GetValue()));

        static RuleProcessor<T> ThreeValueRule<T, U, V, W>(DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, ThreeValueCanConvert<T, U, V, W> canConvert, ThreeValueConvert<T, U, V, W> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w), p => canConvert(p, u, v, w), p => convert(p, u.GetValue(), v.GetValue(), w.GetValue()));

        static RuleProcessor<T> FourValueRule<T, U, V, W, X>(DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, DataProcessor<X> x, FourValueConvert<T, U, V, W, X> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w, x), p => AllSatisfied(u, v, w, x), p => convert(p, u.GetValue(), v.GetValue(), w.GetValue(), x.GetValue()));

        static RuleProcessor<T> FourValueRule<T, U, V, W, X>(DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, DataProcessor<X> x, FourValueCanConvert<T, U, V, W, X> canConvert, FourValueConvert<T, U, V, W, X> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w, x), p => canConvert(p, u, v, w, x), p => convert(p, u.GetValue(), v.GetValue(), w.GetValue(), x.GetValue()));

        static RuleProcessor<SelectorCommandParameter> BlockCommandProcessor() {
            var assignmentProcessor = requiredEither<AssignmentCommandParameter>();
            var incrementProcessor = eitherList<IncrementCommandParameter>(true);
            var variableProcessor = requiredEither<VariableCommandParameter>();
            var propertyProcessor = requiredEither<PropertyCommandParameter>();
            var directionProcessor = requiredEither<DirectionCommandParameter>();
            var reverseProcessor = requiredEither<ReverseCommandParameter>();
            var notProcessor = requiredEither<NotCommandParameter>();
            var processors = NewList<DataProcessor>(
                assignmentProcessor,
                incrementProcessor,
                variableProcessor,
                propertyProcessor,
                directionProcessor,
                reverseProcessor,
                notProcessor);

            CanConvert<SelectorCommandParameter> canConvert = (p) => processors.Exists(x => x.Satisfied() && x != directionProcessor && x != propertyProcessor);
            Convert<SelectorCommandParameter> convert = (p) => {
                Action<BlockHandler, Object> blockAction;
                PropertyCommandParameter property = propertyProcessor.GetValue();
                DirectionCommandParameter direction = directionProcessor.GetValue();
                VariableCommandParameter variable = variableProcessor.GetValue();
                bool notValue = notProcessor.Satisfied();

                PropertySupplier propertySupplier = propertyProcessor.Satisfied() ? property.value : new PropertySupplier();
                if (directionProcessor.Satisfied()) propertySupplier = propertySupplier.WithDirection(directionProcessor.GetValue().value);

                Variable variableValue = GetStaticVariable(true);
                if (variableProcessor.Satisfied()) {
                    variableValue = variable.value;
                    propertySupplier = propertySupplier.WithPropertyValue(variableValue);
                }

                if (notValue) {
                    variableValue = new UniOperandVariable(UniOperand.NOT, variableValue);
                    propertySupplier = propertySupplier.WithPropertyValue(variableValue);
                }

                if (incrementProcessor.Satisfied()) {
                    propertySupplier = propertySupplier.WithIncrement(incrementProcessor.GetValue()
                        .Select(v => v.value)
                        .Aggregate((a, b) => a && b));
                }

                if (AllSatisfied(reverseProcessor)) blockAction = (b, e) => b.ReverseNumericPropertyValue(e, propertySupplier.Resolve(b));
                else if (AllSatisfied(incrementProcessor)) blockAction = (b, e) => b.IncrementPropertyValue(e, propertySupplier.Resolve(b));
                else if (AllSatisfied(directionProcessor)) blockAction = (b, e) => b.UpdatePropertyValue(e, propertySupplier.Resolve(b));
                else blockAction = (b, e) => b.UpdatePropertyValue(e, propertySupplier.WithPropertyValue(variableValue).Resolve(b));

                BlockCommand command = new BlockCommand(p.value, blockAction);
                return new CommandReferenceParameter(command);
            };

            return new RuleProcessor<SelectorCommandParameter>(processors, canConvert, convert);
        }

        static bool AllSatisfied(params DataProcessor[] processors) => processors.ToList().TrueForAll(p => p.Satisfied());

        static T findLast<T>(List<CommandParameter> parameters) where T : class, CommandParameter => parameters.Where(p => p is T).Select(p => (T)p).LastOrDefault();
    }
}
