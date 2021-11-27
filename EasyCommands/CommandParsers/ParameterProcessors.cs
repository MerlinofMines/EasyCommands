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
            ThreeValueRule<VariableSelectorCommandParameter, AmbiguiousStringCommandParameter, BlockTypeCommandParameter, GroupCommandParameter>(
                requiredRight<AmbiguiousStringCommandParameter>(), optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, selector, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null, group.HasValue(), new AmbiguousStringVariable(selector.GetValue().value)))),
            ThreeValueRule<VariableSelectorCommandParameter, VariableCommandParameter, BlockTypeCommandParameter, GroupCommandParameter>(
                requiredRight<VariableCommandParameter>(), optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, selector, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null, group.HasValue(), selector.GetValue().value))),

            //SelectorProcessor
            new BranchingProcessor<AmbiguiousStringCommandParameter>(
                TwoValueRule<AmbiguiousStringCommandParameter, BlockTypeCommandParameter, GroupCommandParameter>(
                        optionalRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                        (p, blockType, group) => {
                            if (!blockType.HasValue()) {
                                BlockTypeCommandParameter type = findLast<BlockTypeCommandParameter>(p.subTokens);
                                if (type != null) blockType.SetValue(type);
                                GroupCommandParameter g = findLast<GroupCommandParameter>(p.subTokens);
                                if (g != null) group.SetValue(g);
                            }
                            return blockType.HasValue();
                        },
                        (p, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.GetValue().value, group.HasValue(), p.isImplicit ? new AmbiguousStringVariable(p.value) : GetStaticVariable(p.value)))),
                NoValueRule<AmbiguiousStringCommandParameter>(
                    name => PROGRAM.functions.ContainsKey(name.value),
                    name => new FunctionDefinitionCommandParameter(Function.GOSUB, PROGRAM.functions[name.value])),
                NoValueRule<AmbiguiousStringCommandParameter>(b => new StringCommandParameter(b.value, false))),

            OneValueRule<ListCommandParameter, StringCommandParameter>(
                requiredLeft<StringCommandParameter>(),
                (list, name) => new ListIndexCommandParameter(new ListIndexVariable(new InMemoryVariable(name.GetValue().value), list.value))),

            OneValueRule<ListIndexCommandParameter, ListCommandParameter>(
                requiredRight<ListCommandParameter>(),
                (index, list) => new ListIndexCommandParameter(new ListIndexVariable(index.value, list.GetValue().value))),

            OneValueRule<ListCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(),
                (list, variable) => new ListIndexCommandParameter(new ListIndexVariable(variable.GetValue().value, list.value))),

            //SelfSelectorProcessor
            OneValueRule<SelfCommandParameter, BlockTypeCommandParameter>(
                optionalRight<BlockTypeCommandParameter>(),
                (p, blockType) => new SelectorCommandParameter(new SelfSelector(blockType.HasValue() ? blockType.GetValue().value : Block.PROGRAM))),

            //VariableSelectorProcessor
            TwoValueRule<VariableCommandParameter, BlockTypeCommandParameter, GroupCommandParameter>(
                requiredRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null, group.HasValue(), p.value))),

            //ListSelectorProcessor
            TwoValueRule<ListIndexCommandParameter, BlockTypeCommandParameter, GroupCommandParameter>(
                requiredRight<BlockTypeCommandParameter>(), optionalRight<GroupCommandParameter>(),
                (p, blockType, group) => new SelectorCommandParameter(new BlockSelector(blockType.HasValue() ? blockType.GetValue().value : (Block?)null, group.HasValue(), p.value))),

            //ImplicitAllSelectorProcessor
            OneValueRule<BlockTypeCommandParameter, GroupCommandParameter>(
                optionalRight<GroupCommandParameter>(),
                (blockType, group) => new SelectorCommandParameter(new BlockTypeSelector(blockType.value))),

            //IndexProcessor
            OneValueRule<IndexCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, var) => new IndexSelectorCommandParameter(var.GetValue().value)),

            //RedundantComparisonProcessor
            //"is not <" => "!<"
            //"is <" => "<"
            //"is not" => !=
            // "not greater than" => <
            OneValueRule<ComparisonCommandParameter, NotCommandParameter>(
                requiredEither<NotCommandParameter>(),
                (p, left) => new ComparisonCommandParameter((a,b) => !p.value(a,b))),
            OneValueRule<ComparisonCommandParameter, ComparisonCommandParameter>(
                requiredRight<ComparisonCommandParameter>(),
                (p, right) => new ComparisonCommandParameter(right.GetValue().value)),

            //IndexSelectorProcessor
            OneValueRule<IndexSelectorCommandParameter, SelectorCommandParameter>(
                requiredLeft<SelectorCommandParameter>(),
                (p, selector) => new SelectorCommandParameter(new IndexSelector(selector.GetValue().value, p.value))),

            //ListProcessors
            OneValueRule<ListCommandParameter, SelectorCommandParameter>(
                requiredLeft<SelectorCommandParameter>(),
                (list, selector) => new SelectorCommandParameter(new IndexSelector(selector.GetValue().value, new IndexVariable(list.value)))),

            new MultiListProcessor(),

            new IgnoreProcessor(),

            //FunctionProcessor
            OneValueRule<StringCommandParameter, FunctionCommandParameter>(
                requiredLeft<FunctionCommandParameter>(),
                (name, function) => new FunctionDefinitionCommandParameter(function.GetValue().value, PROGRAM.functions[name.value])),

            //AssignmentProcessor
            TwoValueRule<AssignmentCommandParameter, GlobalCommandParameter, StringCommandParameter>(
                optionalRight<GlobalCommandParameter>(), requiredRight<StringCommandParameter>(),
                (p, g, name) => new VariableAssignmentCommandParameter(name.GetValue().value, p.useReference, g.HasValue())),
            TwoValueRule<AssignmentCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, list, value) => list.HasValue() && list.GetValue().value is ListIndexVariable && value.HasValue(),
                (p, list, value) => new CommandReferenceParameter(new ListVariableAssignmentCommand((ListIndexVariable)list.GetValue().value, value.GetValue().value, p.useReference))),
            TwoValueRule<AssignmentCommandParameter, GlobalCommandParameter, VariableCommandParameter>(
                optionalRight<GlobalCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, g, name) => name.HasValue() && name.GetValue().value is InMemoryVariable,
                (p, g, name) => new VariableAssignmentCommandParameter(((InMemoryVariable)name.GetValue().value).variableName, p.useReference, g.HasValue())),

            //Primitive Processor
            new PrimitiveProcessor<PrimitiveCommandParameter>(),

            //ValuePropertyProcessor
            //Needs to check left, then right, which is opposite the typical checks.
            OneValueRule<ValuePropertyCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(),
                (p, v) => new PropertyCommandParameter(new PropertySupplier().WithPropertyType(p.value + "").WithAttributeValue(v.GetValue().value))),
            OneValueRule<ValuePropertyCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, v) => new PropertyCommandParameter(new PropertySupplier().WithPropertyType(p.value + "").WithAttributeValue(v.GetValue().value))),

            //UniOperationProcessor
            OneValueRule<UniOperationCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, df) => new VariableCommandParameter(new UniOperandVariable(p.value, df.GetValue().value))),

            //AfterUniOperationProcessor
            OneValueRule<LeftUniOperationCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(),
                (p, df) => new VariableCommandParameter(new UniOperandVariable(p.value, df.GetValue().value))),

            //Tier1OperationProcessor
            TwoValueRule<BiOperandTier1Operand, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, a, b) => new VariableCommandParameter(new BiOperandVariable(p.value, a.GetValue().value, b.GetValue().value))),

            //Tier2OperationProcessor
            TwoValueRule<BiOperandTier2Operand, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, a, b) => new VariableCommandParameter(new BiOperandVariable(p.value, a.GetValue().value, b.GetValue().value))),

            //Tier3OperationProcessor
            TwoValueRule<BiOperandTier3Operand, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, a, b) => new VariableCommandParameter(new BiOperandVariable(p.value, a.GetValue().value, b.GetValue().value))),

            //VariableComparisonProcessor
            TwoValueRule<ComparisonCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, left, right) => new VariableCommandParameter(new ComparisonVariable(left.GetValue().value, right.GetValue().value, p.value))),

            //NotProcessor
            OneValueRule<NotCommandParameter, VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p, right) => new VariableCommandParameter(new UniOperandVariable(UniOperand.NOT, right.GetValue().value))),

            //AndProcessor
            TwoValueRule<AndCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, left, right) => new VariableCommandParameter(new BiOperandVariable(BiOperand.AND, left.GetValue().value, right.GetValue().value))),

            //OrProcessor
            TwoValueRule<OrCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (p, left, right) => new VariableCommandParameter(new BiOperandVariable(BiOperand.OR, left.GetValue().value, right.GetValue().value))),

            //ListPropertyAggregationProcessor
            OneValueRule<ListIndexCommandParameter, PropertyAggregationCommandParameter>(
                requiredLeft<PropertyAggregationCommandParameter>(),
                (list, aggregation) => new VariableCommandParameter(new ListAggregateVariable(list.value, aggregation.GetValue().value))),

            //ListComparisonProcessor
            ThreeValueRule<ListIndexCommandParameter, ComparisonCommandParameter, VariableCommandParameter, AggregationModeCommandParameter>(
                requiredRight<ComparisonCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalLeft<AggregationModeCommandParameter>(),
                (list, comparison, value, aggregation) => {
                    AggregationMode mode = AggregationMode.ALL;
                    if (aggregation.HasValue()) mode = aggregation.GetValue().value;
                    return new VariableCommandParameter(new ListAggregateConditionVariable(mode, list.value, comparison.GetValue().value, value.GetValue().value));
                }),

            //KeyedVariableProcessor
            TwoValueRule<KeyedVariableCommandParameter, VariableCommandParameter, VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (keyed, left, right) => new VariableCommandParameter(new KeyedVariable(left.GetValue().value, right.GetValue().value))),

            //BlockConditionProcessors
            ThreeValueRule<AndCommandParameter,BlockConditionCommandParameter,ThatCommandParameter,BlockConditionCommandParameter>(
                requiredLeft<BlockConditionCommandParameter>(), optionalRight<ThatCommandParameter>(), requiredRight<BlockConditionCommandParameter>(),
                (p,left,with,right) => new BlockConditionCommandParameter((block, blockType) => left.GetValue().value(block, blockType) && right.GetValue().value(block, blockType))),
            ThreeValueRule<OrCommandParameter,BlockConditionCommandParameter,ThatCommandParameter,BlockConditionCommandParameter>(
                requiredLeft<BlockConditionCommandParameter>(), optionalRight<ThatCommandParameter>(), requiredRight<BlockConditionCommandParameter>(),
                (p, left, with, right) => new BlockConditionCommandParameter((block, blockType) => left.GetValue().value(block, blockType) || right.GetValue().value(block, blockType))),

            //ThatBlockConditionProcessor
            FourValueRule<ThatCommandParameter, ComparisonCommandParameter,PropertyCommandParameter,DirectionCommandParameter,VariableCommandParameter>(
                requiredRight<ComparisonCommandParameter>(), optionalRight<PropertyCommandParameter>(),optionalRight<DirectionCommandParameter>(),optionalRight<VariableCommandParameter>(),
                (with,p,prop,dir,var) => p.HasValue() && (var.HasValue() || prop.HasValue()),
                (with,p,prop,dir,var) => {
                    Variable variable = var.HasValue() ? var.GetValue().value : GetStaticVariable(true);
                    PropertySupplier property = null;
                    if(prop.HasValue()) property = prop.GetValue().value;
                    Direction? direction = null;
                    if(dir.HasValue()) direction = dir.GetValue().value;
                    return NewList<CommandParameter>(new ThatCommandParameter(), new BlockConditionCommandParameter(BlockPropertyCondition((property ?? new PropertySupplier()).WithDirection(direction), new PrimitiveComparator(p.GetValue().value), variable)));
                }),

            //ConditionalSelectorProcessor
            TwoValueRule<ThatCommandParameter,SelectorCommandParameter, BlockConditionCommandParameter>(
                requiredLeft<SelectorCommandParameter>(),requiredRight<BlockConditionCommandParameter>(),
                (p,selector,condition) => new SelectorCommandParameter(new ConditionalSelector(selector.GetValue().value, condition.GetValue().value))),

            //PropertyAggregationProcessor
            ThreeValueRule<PropertyAggregationCommandParameter,SelectorCommandParameter,PropertyCommandParameter,DirectionCommandParameter>(
                requiredEither<SelectorCommandParameter>(),optionalEither<PropertyCommandParameter>(),optionalEither<DirectionCommandParameter>(),
                (p,selector,property,direction) => selector.HasValue(),
                (p,selector,prop,dir) => {
                    PropertySupplier property = null;
                    if(prop.HasValue()) property = prop.GetValue().value;
                    Direction? direction = null;
                    if(dir.HasValue()) direction = dir.GetValue().value;
                    return new VariableCommandParameter(new AggregatePropertyVariable(p.value, selector.GetValue().value, (property ?? new PropertySupplier()).WithDirection(direction)));
                }),

            //BlockComparisonProcessor
            ThreeValueRule<ComparisonCommandParameter,PropertyCommandParameter,DirectionCommandParameter,VariableCommandParameter>(
                optionalEither<PropertyCommandParameter>(),optionalEither<DirectionCommandParameter>(),optionalRight<VariableCommandParameter>(),
                (p,prop,dir,var) => var.HasValue() || prop.HasValue(),
                (p,prop,dir,var) => {
                    Variable variable = var.HasValue() ? var.GetValue().value : GetStaticVariable(true);
                    PropertySupplier property = null;
                    if(prop.HasValue()) property = prop.GetValue().value;
                    Direction? direction = null;
                    if(dir.HasValue()) direction = dir.GetValue().value;
                    return new BlockConditionCommandParameter(BlockPropertyCondition((property ?? new PropertySupplier()).WithDirection(direction), new PrimitiveComparator(p.value), variable));
                }),

            //AggregateConditionProcessor
            TwoValueRule<BlockConditionCommandParameter,AggregationModeCommandParameter,SelectorCommandParameter>(
                optionalLeft<AggregationModeCommandParameter>(),requiredLeft<SelectorCommandParameter>(),
                (p,aggregation,selector) => {
                    AggregationMode mode = aggregation.HasValue() ? aggregation.GetValue().value : AggregationMode.ALL;
                    return new VariableCommandParameter(new AggregateConditionVariable(mode, p.value, selector.GetValue().value));
                }),

            //AggregateSelectorProcessor
            OneValueRule<AggregationModeCommandParameter,SelectorCommandParameter>(
                requiredRight<SelectorCommandParameter>(),
                (aggregation, selector) => aggregation.value != AggregationMode.NONE && selector.HasValue(),
                (aggregation, selector) => selector.GetValue()),

            //IteratorProcessor
            OneValueRule<IteratorCommandParameter,VariableCommandParameter>(
                requiredLeft<VariableCommandParameter>(),
                (p,var) => new IterationCommandParameter(var.GetValue().value)),

            //TransferCommandProcessor
            FourValueRule<TransferCommandParameter,SelectorCommandParameter,SelectorCommandParameter,VariableCommandParameter,VariableCommandParameter>(
                requiredLeft<SelectorCommandParameter>(), requiredRight<SelectorCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (t,s1,s2,v1,v2) => new CommandReferenceParameter(new TransferItemCommand((t.value ? s1 : s2).GetValue().value, (t.value ? s2 : s1).GetValue().value, v1.GetValue().value, v2.HasValue() ? v2.GetValue().value : null))),
            FourValueRule<TransferCommandParameter,SelectorCommandParameter,SelectorCommandParameter,VariableCommandParameter,VariableCommandParameter>(
                requiredRight<SelectorCommandParameter>(), requiredRight<SelectorCommandParameter>(), requiredRight<VariableCommandParameter>(), optionalRight<VariableCommandParameter>(),
                (t,s1,s2,v1,v2) => new CommandReferenceParameter(new TransferItemCommand(s1.GetValue().value, s2.GetValue().value, v1.GetValue().value, v2.HasValue() ? v2.GetValue().value : null))),

            //ListIndexAsVariableProcessor
            NoValueRule<ListIndexCommandParameter>(list => new VariableCommandParameter(list.value)),

            //IfProcessor
            OneValueRule<IfCommandParameter,VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p,var) => new ConditionCommandParameter(p.inverseCondition ? new UniOperandVariable(UniOperand.NOT, var.GetValue().value) : var.GetValue().value, p.alwaysEvaluate, p.swapCommands)),

            //AmbiguousSelectorPropertyProcessor
            new BranchingProcessor<SelectorCommandParameter>(
                BlockCommandProcessor(),
                TwoValueRule<SelectorCommandParameter,PropertyCommandParameter,DirectionCommandParameter>(
                    requiredEither<PropertyCommandParameter>(), optionalEither<DirectionCommandParameter>(),
                    (s,p,d) => new VariableCommandParameter(new AggregatePropertyVariable(PROGRAM.SumAggregator, s.value, p.GetValue().value.WithDirection(d.HasValue() ? d.GetValue().value : (Direction?)null)))),
                TwoValueRule<SelectorCommandParameter,PropertyCommandParameter,DirectionCommandParameter>(
                    optionalEither<PropertyCommandParameter>(), optionalEither<DirectionCommandParameter>(),
                    (s,p,d) => p.HasValue() || d.HasValue(),//Must have at least one!
                    (s,p,d) => {
                        PropertySupplier property = p.HasValue() ? p.GetValue().value : new PropertySupplier();
                        Direction? direction = d.HasValue() ? d.GetValue().value : (Direction?)null;
                        if (direction == null) property = property.WithPropertyValue(GetStaticVariable(true));
                        return new CommandReferenceParameter(new BlockCommand(s.value, (b, e) => 
                            b.UpdatePropertyValue(e, property.WithDirection(direction).Resolve(b))));
                    })),

            //PrintCommandProcessor
            OneValueRule<PrintCommandParameter,VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p,var) => new CommandReferenceParameter(new PrintCommand(var.GetValue().value))),

            //WaitProcessor
            TwoValueRule<WaitCommandParameter,VariableCommandParameter,UnitCommandParameter>(
                optionalRight<VariableCommandParameter>(),optionalRight<UnitCommandParameter>(),
                (p,time,unit) => {
                    Unit units = unit.HasValue() ? unit.GetValue().value : time.HasValue() ? Unit.SECONDS : Unit.TICKS;
                    Variable var = time.HasValue() ? time.GetValue().value : new StaticVariable(ResolvePrimitive(1));
                    return new CommandReferenceParameter(new WaitCommand(var, units));
                }),

            //FunctionCallCommandProcessor
            OneValueRule<FunctionDefinitionCommandParameter,VariableCommandParameter>(
                rightList<VariableCommandParameter>(true),
                (p,variables) => ((ListValueDataFetcher<VariableCommandParameter>)variables).GetValues().Count == p.functionDefinition.parameterNames.Count,
                (p,variables) => {
                    List<VariableCommandParameter> parameters = ((ListValueDataFetcher<VariableCommandParameter>)variables).GetValues();
                    var inputParameters = NewDictionary<string, Variable>();
                    for (int i = 0; i < p.functionDefinition.parameterNames.Count; i++) {
                        inputParameters[p.functionDefinition.parameterNames[i]] = parameters[i].value;
                    }
                    Command command = new FunctionCommand(p.functionType, p.functionDefinition, inputParameters);
                    return new CommandReferenceParameter(command);
                }),

            //VariableAssignmentProcessor
            OneValueRule<VariableAssignmentCommandParameter,VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p,var) => new CommandReferenceParameter(new VariableAssignmentCommand(p.variableName, var.GetValue().value, p.useReference, p.isGlobal))),

            //SendCommandProcessor
            //Note: Message to send always comes first: "send <command> to <tag>" is only supported format
            TwoValueRule<SendCommandParameter,VariableCommandParameter,VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),requiredRight<VariableCommandParameter>(),
                (p,message,tag) => new CommandReferenceParameter(new SendCommand(message.GetValue().value, tag.GetValue().value))),

            //ListenCommandProcessor
            OneValueRule<ListenCommandParameter,VariableCommandParameter>(
                requiredRight<VariableCommandParameter>(),
                (p,var) => new CommandReferenceParameter(new ListenCommand(var.GetValue().value))),

            //ControlProcessor 
            NoValueRule<ControlCommandParameter>((p) => new CommandReferenceParameter(new ControlCommand(p.value))),

            //IterationProcessor
            OneValueRule<IterationCommandParameter,CommandReferenceParameter>(
                requiredEither<CommandReferenceParameter>(),
                (p,command) => new CommandReferenceParameter(new MultiActionCommand(NewList(command.GetValue().value), p.value))),

            //QueueProcessor
            OneValueRule<QueueCommandParameter,CommandReferenceParameter>(
                requiredRight<CommandReferenceParameter>(),
                (p,command) => new CommandReferenceParameter(new QueueCommand(command.GetValue().value,p.value))),

            //ConditionalCommandProcessor
            //condition command
            //condition command otherwise command
            ThreeValueRule<ConditionCommandParameter,CommandReferenceParameter,ElseCommandParameter,CommandReferenceParameter>(
                requiredRight<CommandReferenceParameter>(),optionalRight<ElseCommandParameter>(),optionalRight<CommandReferenceParameter>(),
                ConvertConditionalCommand),
            //command condition
            //command condition otherwise command
            ThreeValueRule<ConditionCommandParameter,CommandReferenceParameter,ElseCommandParameter,CommandReferenceParameter>(
                requiredLeft<CommandReferenceParameter>(),optionalRight<ElseCommandParameter>(),optionalRight<CommandReferenceParameter>(),
                ConvertConditionalCommand)
        );

        static CommandParameter ConvertConditionalCommand(ConditionCommandParameter condition, DataFetcher<CommandReferenceParameter> metFetcher,
            DataFetcher<ElseCommandParameter> otherwise, DataFetcher<CommandReferenceParameter> notMetFetcher) {
            Command metCommand = metFetcher.GetValue().value;
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
            var sortedParameterProcessors = NewList<ParameterProcessor>();

            var branches = NewList<List<CommandParameter>>();
            AddProcessors(commandParameters, sortedParameterProcessors);

            int processorIndex = 0;

            Debug(String.Join(" ", commandParameters.Select(p => CommandParameterToString(p)).ToList()));

            while (processorIndex < sortedParameterProcessors.Count) {
                bool revisit = false;
                bool processed = false;
                ParameterProcessor current = sortedParameterProcessors[processorIndex];
                for (int i = commandParameters.Count - 1; i >= 0; i--) {
                    if (current.CanProcess(commandParameters[i])) {
                        List<CommandParameter> finalParameters;
                        if (current.Process(commandParameters, i, out finalParameters, branches)) {
                            AddProcessors(finalParameters, sortedParameterProcessors);
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
                if (!revisit) sortedParameterProcessors.RemoveAt(processorIndex);
                else processorIndex++;
            }

            return branches;
        }

        void AddProcessors(List<CommandParameter> types, List<ParameterProcessor> sortedParameterProcessors) {
            sortedParameterProcessors.AddRange(types.SelectMany(t => parameterProcessorsByParameterType.ContainsKey(t.GetType()) ? parameterProcessorsByParameterType[t.GetType()] : NewList<ParameterProcessor>()).ToList());
            sortedParameterProcessors.Sort();
        }

        delegate bool CanConvert<T>(T t);
        delegate bool OneValueCanConvert<T,U>(T t, DataFetcher<U> a);
        delegate bool TwoValueCanConvert<T,U,V>(T t, DataFetcher<U> a, DataFetcher<V> b);
        delegate bool ThreeValueCanConvert<T, U, V, W>(T t, DataFetcher<U> a, DataFetcher<V> b, DataFetcher<W> c);
        delegate bool FourValueCanConvert<T, U, V, W, X>(T t, DataFetcher<U> a, DataFetcher<V> b, DataFetcher<W> c, DataFetcher<X> d);

        delegate object Convert<T>(T t);
        delegate object OneValueConvert<T, U>(T t, DataFetcher<U> a) where T : CommandParameter;
        delegate object TwoValueConvert<T, U, V>(T t, DataFetcher<U> a, DataFetcher<V> b) where T : CommandParameter;
        delegate object ThreeValueConvert<T, U, V, W>(T t, DataFetcher<U> a, DataFetcher<V> b, DataFetcher<W> c);
        delegate object FourValueConvert<T, U, V, W, X>(T t, DataFetcher<U> a, DataFetcher<V> b, DataFetcher<W> c, DataFetcher<X> d);

        delegate bool Process(CommandParameter p);
        delegate Process ProcessCommandParameter(DataFetcher dataFetcher);

        //Data Fetchers
        interface DataFetcher {
            bool Satisfied();
            bool SetValue(CommandParameter p);
            void Clear();
        }

        interface DataFetcher<T> : DataFetcher {
            bool HasValue();
            T GetValue();
        }

        class ValueDataFetcher<T> : DataFetcher<T> where T : class, CommandParameter {
            T value = null;
            bool required;

            public ValueDataFetcher(bool req) {
                required = req;
            }

            public void Clear() => value = null;
            public bool HasValue() => value != null;
            public T GetValue() => value;
            public bool Satisfied() => !required || value != null;
            public bool SetValue(CommandParameter p) {
                var setValue = p is T && value == null;
                if (setValue) value = (T)p;
                return setValue;
            }
        }

        class ListValueDataFetcher<T> : DataFetcher<T> where T : class, CommandParameter {
            List<T> values = NewList<T>();
            bool required;

            public ListValueDataFetcher(bool req) {
                required = req;
            }

            public void Clear() => values.Clear();

            public T GetValue() => values[0];

            public List<T> GetValues() => values;
            public bool HasValue() => values.Count() > 0;
            public bool Satisfied() => !required || HasValue();
            public bool SetValue(CommandParameter p) {
                if (p is T) values.Add((T)p);
                return p is T;
            }
        }

        //DataProcessors
        class DataProcessor {
            public DataFetcher fetcher;
            public Process left;
            public Process right;
            public DataProcessor(DataFetcher f, ProcessCommandParameter l, ProcessCommandParameter r) {
                fetcher = f;
                left = l(f);
                right = r(f);
            }
        }

        class DataProcessor<U> : DataProcessor {
            public DataFetcher<U> f;

            public DataProcessor(DataFetcher<U> fetcher, ProcessCommandParameter l, ProcessCommandParameter r) : base(fetcher,l,r) {
                f = fetcher;
            }
        }

        static DataProcessor<T> requiredRight<T>() where T : class, CommandParameter => right<T>(true);
        static DataProcessor<T> optionalRight<T>() where T : class, CommandParameter => right<T>(false);

        static DataProcessor<T> requiredLeft<T>() where T : class, CommandParameter => left<T>(true);
        static DataProcessor<T> optionalLeft<T>() where T : class, CommandParameter => left<T>(false);

        static DataProcessor<T> requiredEither<T>() where T : class, CommandParameter => either<T>(true);
        static DataProcessor<T> optionalEither<T>() where T : class, CommandParameter => either<T>(false);

        static DataProcessor<T> right<T>(bool required) where T : class, CommandParameter =>
            new DataProcessor<T>(new ValueDataFetcher<T>(required), df => p => false ,df => p => df.SetValue(p));

        static DataProcessor<T> left<T>(bool required) where T : class, CommandParameter =>
            new DataProcessor<T>(new ValueDataFetcher<T>(required), df => p => df.SetValue(p), df => p => false);

        static DataProcessor<T> either<T>(bool required) where T : class, CommandParameter =>
            new DataProcessor<T>(new ValueDataFetcher<T>(required), df => p => df.SetValue(p), df => p => df.SetValue(p));

        static DataProcessor<T> rightList<T>(bool required) where T: class, CommandParameter =>
            new DataProcessor<T>(new ListValueDataFetcher<T>(required), df => p => false, df => p => df.SetValue(p));

        static DataProcessor<T> leftList<T>(bool required) where T : class, CommandParameter =>
            new DataProcessor<T>(new ListValueDataFetcher<T>(required), df => p => df.SetValue(p), df => p => false);

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
            public override List<Type> GetProcessedTypes() => NewList<Type>(typeof(BooleanCommandParameter), typeof(StringCommandParameter));

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
                Primitive primitive = null;
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
                processors.ForEach(dp => dp.fetcher.Clear());
                int j = i + 1;
                while (j < p.Count) {
                    if (processors.Exists(dp => dp.right(p[j]))) j++;
                    else break;
                }

                int k = i;
                while (k > 0) {
                    if (processors.Exists(dp => dp.left(p[k - 1]))) k--;
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
        static RuleProcessor<T> NoValueRule<T>(Convert<T> convert) where T : class, CommandParameter => NoValueRule((p) => true, convert);

        static RuleProcessor<T> NoValueRule<T>(CanConvert<T> canConvert, Convert<T> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(), canConvert, convert);

        static RuleProcessor<T> OneValueRule<T, U>(DataProcessor<U> u, OneValueConvert<T, U> convert) where T : class, CommandParameter =>
            OneValueRule(u, (p, a) => a.Satisfied(), convert);

        static RuleProcessor<T> OneValueRule<T, U>(DataProcessor<U> u, OneValueCanConvert<T, U> canConvert, OneValueConvert<T, U> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u), (p) => canConvert(p, u.f), (p) => convert(p, u.f));

        static RuleProcessor<T> TwoValueRule<T, U, V>(DataProcessor<U> u, DataProcessor<V> v, TwoValueConvert<T, U, V> convert) where T : class, CommandParameter =>
            TwoValueRule(u, v, (p, a, b) => a.Satisfied() && b.Satisfied(), convert);

        static RuleProcessor<T> TwoValueRule<T, U, V>(DataProcessor<U> u, DataProcessor<V> v, TwoValueCanConvert<T, U, V> canConvert, TwoValueConvert<T, U, V> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v ), (p) => canConvert(p, u.f, v.f), (p) => convert(p, u.f, v.f));

        static RuleProcessor<T> ThreeValueRule<T, U, V, W>(DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, ThreeValueConvert<T, U, V, W> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w ), (p) => u.f.Satisfied() && v.f.Satisfied() && w.f.Satisfied(), (p) => convert(p, u.f, v.f, w.f));

        static RuleProcessor<T> ThreeValueRule<T, U, V, W>(DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, ThreeValueCanConvert<T, U, V, W> canConvert, ThreeValueConvert<T, U, V, W> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w ), (p) => canConvert(p, u.f, v.f, w.f), (p) => convert(p, u.f, v.f, w.f));

        static RuleProcessor<T> FourValueRule<T, U, V, W, X>(DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, DataProcessor<X> x, FourValueConvert<T, U, V, W, X> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w, x ), (p) => u.f.Satisfied() && v.f.Satisfied() && w.f.Satisfied() && x.f.Satisfied(), (p) => convert(p, u.f, v.f, w.f, x.f));

        static RuleProcessor<T> FourValueRule<T, U, V, W, X>(DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, DataProcessor<X> x, FourValueCanConvert<T, U, V, W, X> canConvert, FourValueConvert<T, U, V, W, X> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w, x ), (p) => canConvert(p, u.f, v.f, w.f, x.f), (p) => convert(p, u.f, v.f, w.f, x.f));

        static RuleProcessor<SelectorCommandParameter> BlockCommandProcessor() {
            var assignmentProcessor = requiredEither<AssignmentCommandParameter>();
            var relativeProcessor = requiredEither<RelativeCommandParameter>();
            var variableProcessor = requiredEither<VariableCommandParameter>();
            var propertyProcessor = requiredEither<PropertyCommandParameter>();
            var directionProcessor = requiredEither<DirectionCommandParameter>();
            var reverseProcessor = requiredEither<ReverseCommandParameter>();
            var notProcessor = requiredEither<NotCommandParameter>();
            var processors = NewList<DataProcessor>(
                assignmentProcessor,
                relativeProcessor,
                variableProcessor,
                propertyProcessor,
                directionProcessor,
                reverseProcessor,
                notProcessor);

            CanConvert<SelectorCommandParameter> canConvert = (p) => processors.Exists(x => x.fetcher.Satisfied() && x != directionProcessor && x != propertyProcessor);
            Convert<SelectorCommandParameter> convert = (p) => {
                Action<BlockHandler, Object> blockAction;
                PropertyCommandParameter property = propertyProcessor.f.GetValue();
                DirectionCommandParameter direction = directionProcessor.f.GetValue();
                VariableCommandParameter variable = variableProcessor.f.GetValue();
                bool notValue = notProcessor.f.HasValue();

                PropertySupplier propertySupplier = propertyProcessor.f.HasValue() ? property.value : new PropertySupplier();
                if (directionProcessor.f.HasValue()) propertySupplier = propertySupplier.WithDirection(directionProcessor.f.GetValue().value);

                Variable variableValue = GetStaticVariable(true);
                if (variableProcessor.f.HasValue()) {
                    variableValue = variable.value;
                    propertySupplier = propertySupplier.WithPropertyValue(variableValue);
                }

                if (notValue) {
                    variableValue = new UniOperandVariable(UniOperand.NOT, variableValue);
                    propertySupplier = propertySupplier.WithPropertyValue(variableValue);
                }

                if (AllSatisfied(reverseProcessor)) blockAction = (b, e) => b.ReverseNumericPropertyValue(e, propertySupplier.Resolve(b));
                else if (AllSatisfied(relativeProcessor)) blockAction = (b, e) => b.IncrementPropertyValue(e, propertySupplier.WithPropertyValue(variableValue).Resolve(b));
                else if (AllSatisfied(directionProcessor)) blockAction = (b, e) => b.UpdatePropertyValue(e, propertySupplier.Resolve(b));
                else blockAction = (b, e) => b.UpdatePropertyValue(e, propertySupplier.WithPropertyValue(variableValue).Resolve(b));

                BlockCommand command = new BlockCommand(p.value, blockAction);
                return new CommandReferenceParameter(command);
            };

            return new RuleProcessor<SelectorCommandParameter>(processors, canConvert, convert);
        }

        static bool AllSatisfied(params DataProcessor[] processors) => processors.ToList().TrueForAll(p => p.fetcher.Satisfied());

        static T findLast<T>(List<CommandParameter> parameters) where T : class, CommandParameter => parameters.Where(p => p is T).Select(p => (T)p).LastOrDefault();
    }
}
