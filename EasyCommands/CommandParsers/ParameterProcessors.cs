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
        public static class ParameterProcessorRegistry {
            private static bool initialized = false;

            private static List<ParameterProcessor> parameterProcessors = new List<ParameterProcessor>
            {
                  new ParenthesisProcessor(),
                  new FunctionProcessor(),
                  new AssignmentProcessor(),
                  new RunArgumentProcessor(),
                  new SelectorProcessor(),
                  new VariableSelectorProcessor(),
                  new PrimitiveProcessor(),
                  new RedundantComparisonProcessor(),
                  new MultiplyProcessor(),
                  new AddProcessor(),
                  new AndProcessor(),
                  new OrProcessor(),
                  new NotProcessor(),
                  new VariableComparisonProcessor(),
                  new BlockComparisonProcessor(),
                  new IndexProcessor(),
                  new ConditionalSelectorProcessor(),
                  new IndexSelectorProcessor(),
                  new PropertyAggregationProcessor(),
                  new AggregationProcessor(),
                  new IteratorProcessor(),
                  new IfProcessor(),
                  new IterationProcessor(),
                  new ActionProcessor(),
                  new PrintCommandProcessor(),
                  new WaitProcessor(),
                  new FunctionCallCommandProcessor(),
                  new VariableAssignmentProcesor(),
                  new SendCommandProcessor(),
                  new ListenCommandProcessor(),
                  new ControlProcessor(),
                  new IterationProcessor(),
                  new ConditionalCommandProcessor(),
                  new AsyncCommandProcessor()
            };

            static Dictionary<Type, List<ParameterProcessor>> parameterProcessorsByParameterType = new Dictionary<Type, List<ParameterProcessor>>();
            static Dictionary<Type, int> priorityByParameterProcessor = new Dictionary<Type, int>();

            public static void InitializeProcessors() {
                if (initialized) return;

                for(int i = 0; i < parameterProcessors.Count; i++) {
                    ParameterProcessor processor = parameterProcessors[i];

                    List<Type> types = processor.GetProcessedTypes();
                    foreach (Type t in types) {
                        if (!parameterProcessorsByParameterType.ContainsKey(t)) parameterProcessorsByParameterType[t] = new List<ParameterProcessor>();
                        parameterProcessorsByParameterType[t].Add(processor);
                    }
                    priorityByParameterProcessor[processor.GetType()] = i;
                    initialized = true;
                }
            }

            public static void Process(List<CommandParameter> commandParameters) {
                InitializeProcessors();

                List<ParameterProcessor> sortedParameterProcessors = new List<ParameterProcessor>();

                AddProcessors(commandParameters, sortedParameterProcessors);

                int processorIndex = 0;

                while(processorIndex < sortedParameterProcessors.Count) {
                    bool revisit = false;
                    bool processed = false;
                    ParameterProcessor current = sortedParameterProcessors[processorIndex];
                    for(int i = 0;i<commandParameters.Count;i++) {
                        if(current.CanProcess(commandParameters[i])) {
                            List<CommandParameter> finalParameters;
                            if (current.Process(commandParameters, i, out finalParameters)) {
                                AddProcessors(finalParameters, sortedParameterProcessors);
                                processed = true;
                                //break; TODO: -Not sure if this may be needed! But much faster processing w/o this.
                            } else revisit = true;
                        }
                    }
                    if (processed) { processorIndex = 0; continue; }
                    if (!revisit) sortedParameterProcessors.RemoveAt(processorIndex);
                    else processorIndex++;
                }
            }

            static void AddProcessors(List<CommandParameter> types, List<ParameterProcessor> sortedParameterProcessors) {
                //how awefully inefficient
                for (int i = 0; i < types.Count; i++) {
                    List<ParameterProcessor> processors;
                    if (!parameterProcessorsByParameterType.TryGetValue(types[i].GetType(), out processors)) continue;
                    foreach (ParameterProcessor processor in processors) {
                        if(!sortedParameterProcessors.Contains(processor)) InsertProcessor(processor, sortedParameterProcessors);
                     }
                }
            }

            static void InsertProcessor(ParameterProcessor processor, List<ParameterProcessor> parameterProcessors) {
                int priority = priorityByParameterProcessor[processor.GetType()];
                int i = 0;
                while (i < parameterProcessors.Count() && priorityByParameterProcessor[parameterProcessors[i].GetType()] < priority) i++;
                parameterProcessors.Insert(i, processor);
            }
        }

        public interface ParameterProcessor {
            List<Type> GetProcessedTypes();
            bool CanProcess(CommandParameter p);
            bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters);
        }

        public abstract class ParameterProcessor<T> : ParameterProcessor where T : class, CommandParameter {
            public virtual List<Type> GetProcessedTypes() { return new List<Type>() { typeof(T) }; }
            public bool CanProcess(CommandParameter p) {
                return p is T;
            }
            public abstract bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters);
        }

        public abstract class SimpleParameterProcessor<T> : ParameterProcessor<T> where T : class, CommandParameter {
            public abstract void Initialize();
            public abstract bool ProcessLeft(CommandParameter p);
            public abstract bool ProcessRight(CommandParameter p);
            public abstract bool CanConvert(List<CommandParameter> p);
            public abstract CommandParameter Convert(List<CommandParameter> p);

            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters) {
                finalParameters = null;
                Initialize();
                int j = i + 1;
                while (j < p.Count) {
                    if (ProcessRight(p[j])) j++;
                    else break;
                }

                int k = i;
                while (k > 0) {
                    if (ProcessLeft(p[k - 1])) k--;
                    else break;
                }

                List<CommandParameter> commandParameters = p.GetRange(k, j - k);

                if (!CanConvert(commandParameters)) return false;

                CommandParameter param = Convert(commandParameters);
                p.RemoveRange(k, j - k);
                p.Insert(k, param);
                finalParameters = new List<CommandParameter>() { p[k] };
                return true;
            }
        }

        public abstract class SimpleCommandProcessor<T> : ParameterProcessor<T> where T : class, CommandParameter {
            public abstract void Initialize();
            public abstract bool CanConvert();
            public abstract bool ProcessParameterArgument(CommandParameter p);
            public abstract Command GetCommand(List<CommandParameter> commandParameters);
            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters) {
                finalParameters = null;
                Initialize();
                int j = i + 1;
                while (j < p.Count) {
                    if (ProcessParameterArgument(p[j])) j++;
                    else break;
                }

                List<CommandParameter> commandParameters = p.GetRange(i, j - i);
                Command command = GetCommand(commandParameters);
                p.RemoveRange(i, j - i);
                p.Insert(i, new CommandReferenceParameter(command));
                finalParameters = new List<CommandParameter>() { p[i] };
                return true;
            }
        }

        public class ParenthesisProcessor : ParameterProcessor<OpenParenthesisCommandParameter> {
            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters) {
                finalParameters = null;
                for(int j = i + 1; j < p.Count; j++) {
                    if (p[j] is OpenParenthesisCommandParameter) return false;
                    else if (p[j] is CloseParenthesisCommandParameter) {
                        finalParameters = p.GetRange(i+1, j - (i+1));
                        ParameterProcessorRegistry.Process(finalParameters);
                        p.RemoveRange(i, j - i + 1);
                        p.InsertRange(i, finalParameters);
                        return true;
                    }
                }
                throw new Exception("Missing Closing Parenthesis for Command");
            }
        }

        public class PrimitiveProcessor : ParameterProcessor<PrimitiveCommandParameter> {
            public override List<Type> GetProcessedTypes() {
                return new List<Type>() { typeof(StringCommandParameter), typeof(NumericCommandParameter), typeof(BooleanCommandParameter) };
            }

            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters) {
                if (p[i] is StringCommandParameter) {
                    String value = ((StringCommandParameter)p[i]).Value;
                    Primitive primitive = new StringPrimitive(value);
                    Vector3D vector;
                    if (GetVector(value, out vector)) primitive = new VectorPrimitive(vector);
                    //TODO Handle Color
                    p[i] = new VariableCommandParameter(new StaticVariable(primitive));
                } else if (p[i] is NumericCommandParameter) {
                    p[i] = new VariableCommandParameter(new StaticVariable(new NumberPrimitive(((NumericCommandParameter)p[i]).Value)));
                } else if (p[i] is BooleanCommandParameter) {
                    p[i] = new VariableCommandParameter(new StaticVariable(new BooleanPrimitive(((BooleanCommandParameter)p[i]).Value)));
                } else {
                    finalParameters = null;
                    return false;
                }
                finalParameters = new List<CommandParameter>() { p[i] };
                return true;
            }
        }

        public class MultiplyProcessor : SimpleParameterProcessor<MultiplyCommandParameter> {
            Variable a, b;
            public override bool CanConvert(List<CommandParameter> p) {
                return a != null & b != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                OperandType operand = findFirst<MultiplyCommandParameter>(p).Value;
                return new VariableCommandParameter(new OperandVariable(a, b, operand));
            }

            public override void Initialize() {
                a = null;
                b = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is VariableCommandParameter && a == null) {
                    a = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is VariableCommandParameter && b == null) {
                    b = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }
        }

        public class AddProcessor : SimpleParameterProcessor<AddCommandParameter> {
            Variable a, b;
            public override bool CanConvert(List<CommandParameter> p) {
                return a != null & b != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                OperandType operand = findFirst<AddCommandParameter>(p).Value;
                return new VariableCommandParameter(new OperandVariable(a, b, operand));
            }

            public override void Initialize() {
                a = null;
                b = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is VariableCommandParameter && a == null) {
                    a = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is VariableCommandParameter && b == null) {
                    b = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }
        }

        public class AndProcessor : SimpleParameterProcessor<AndCommandParameter> {
            CommandParameter left, right;

            public override bool CanConvert(List<CommandParameter> p) {
                if (left == null || right == null) return false;
                if (left.GetType() != right.GetType()) return false;
                return (left is BlockConditionCommandParameter ||
                    left is VariableCommandParameter);
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                if (left is BlockConditionCommandParameter) {
                    BlockCondition a = ((BlockConditionCommandParameter)left).Value;
                    BlockCondition b = ((BlockConditionCommandParameter)right).Value;
                    return new BlockConditionCommandParameter(new AndBlockCondition(a, b));
                } else {
                    Variable a = ((VariableCommandParameter)left).Value;
                    Variable b = ((VariableCommandParameter)right).Value;
                    return new VariableCommandParameter(new AndVariable(a, b));
                }
            }

            public override void Initialize() {
                left = null;
                right = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (left != null) return false;
                left = p;
                return true;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (right != null) return false;
                right = p;
                return true;
            }
        }

        public class OrProcessor : SimpleParameterProcessor<OrCommandParameter> {
            CommandParameter left, right;

            public override bool CanConvert(List<CommandParameter> p) {
                if (left == null || right == null) return false;
                if (left.GetType() != right.GetType()) return false;
                return (left is BlockConditionCommandParameter ||
                    left is VariableCommandParameter);
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                if (left is BlockConditionCommandParameter) {
                    BlockCondition a = ((BlockConditionCommandParameter)left).Value;
                    BlockCondition b = ((BlockConditionCommandParameter)right).Value;
                    return new BlockConditionCommandParameter(new OrBlockCondition(a, b));
                } else {
                    Variable a = ((VariableCommandParameter)left).Value;
                    Variable b = ((VariableCommandParameter)right).Value;
                    return new VariableCommandParameter(new OrVariable(a, b));
                }
            }

            public override void Initialize() {
                left = null;
                right = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (left != null) return false;
                left = p;
                return true;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (right != null) return false;
                right = p;
                return true;
            }
        }

        public class NotProcessor : SimpleParameterProcessor<NotCommandParameter> {
            Variable variable;

            public override bool CanConvert(List<CommandParameter> p) {
                return variable != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                return new VariableCommandParameter(new NotVariable(variable));
            }

            public override void Initialize() {
                variable = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is VariableCommandParameter && variable == null) {
                    variable = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }
        }

        public class ConditionalSelectorProcessor : SimpleParameterProcessor<WithCommandParameter> {
            EntityProvider entityProvider;
            BlockCondition blockCondition;

            public override bool CanConvert(List<CommandParameter> p) {
                return entityProvider != null && blockCondition != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                ConditionalEntityProvider provider = new ConditionalEntityProvider(entityProvider, blockCondition);
                return new SelectorCommandParameter(provider);
            }

            public override void Initialize() {
                entityProvider = null;
                blockCondition = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is SelectorCommandParameter && entityProvider == null) {
                    entityProvider = ((SelectorCommandParameter)p).Value;
                    return true;
                } else return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is BlockConditionCommandParameter && blockCondition == null) {
                    blockCondition = ((BlockConditionCommandParameter)p).Value;
                    return true;
                } else return false;
            }
        }

        public class IndexProcessor : SimpleParameterProcessor<IndexCommandParameter> {
            Variable indexSelector;
            public override bool CanConvert(List<CommandParameter> p) {
                return indexSelector != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                return new IndexSelectorCommandParameter(indexSelector);
            }

            public override void Initialize() {
                indexSelector = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is VariableCommandParameter && indexSelector == null) {
                    indexSelector = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }
        }

        public class IndexSelectorProcessor : SimpleParameterProcessor<IndexSelectorCommandParameter> {
            EntityProvider entityProvider;

            public override bool CanConvert(List<CommandParameter> p) {
                return entityProvider != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                Variable indexSpecifier = findFirst<IndexSelectorCommandParameter>(p).Value;
                EntityProvider provider = new IndexEntityProvider(entityProvider, indexSpecifier);
                return new SelectorCommandParameter(provider);
            }

            public override void Initialize() {
                entityProvider = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is SelectorCommandParameter && entityProvider == null) {
                    entityProvider = ((SelectorCommandParameter)p).Value;
                    return true;
                } else return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                return false;
            }
        }

        public class PropertyAggregationProcessor : SimpleParameterProcessor<PropertyAggregationCommandParameter> {
            EntityProvider entityProvider;
            PropertyType? property;
            DirectionType? direction;

            public override bool CanConvert(List<CommandParameter> p) {
                return entityProvider != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                PropertyAggregatorType aggregator = findFirst<PropertyAggregationCommandParameter>(p).Value;
                return new VariableCommandParameter(new AggregatePropertyVariable(aggregator, entityProvider, property, direction));
            }

            public override void Initialize() {
                entityProvider = null;
                property = null;
                direction = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return Process(p);
            }

            public override bool ProcessRight(CommandParameter p) {
                return Process(p);
            }

            bool Process(CommandParameter p) {
                if (p is SelectorCommandParameter && entityProvider == null) entityProvider = ((SelectorCommandParameter)p).Value;
                else if (p is PropertyCommandParameter && !property.HasValue) property = ((PropertyCommandParameter)p).Value;
                else if (p is DirectionCommandParameter && !direction.HasValue) direction = ((DirectionCommandParameter)p).Value;
                else return false;
                return true;
            }
        }

        public class AggregationProcessor : SimpleParameterProcessor<BlockConditionCommandParameter> {
            AggregationMode? aggegration;
            EntityProvider entityProvider;

            public override bool CanConvert(List<CommandParameter> p) {
                return entityProvider != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                BlockCondition blockCondition = findFirst<BlockConditionCommandParameter>(p).Value;
                Variable variable = new AggregateConditionVariable(aggegration.GetValueOrDefault(AggregationMode.ALL), blockCondition, entityProvider);
                return new VariableCommandParameter(variable);
            }

            public override void Initialize() {
                entityProvider = null;
                aggegration = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is SelectorCommandParameter && entityProvider == null) {
                    entityProvider = ((SelectorCommandParameter)p).Value;
                    return true;
                } else if (p is AggregationModeCommandParameter && !aggegration.HasValue) {
                    aggegration = ((AggregationModeCommandParameter)p).Value;
                    return true;
                } else return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                return false;
            }
        }

        //"is not <" => "!<"
        //"is <" => "<"
        //"is not" => !=
        // "not greater than" => <
        public class RedundantComparisonProcessor : SimpleParameterProcessor<ComparisonCommandParameter> {
            ComparisonType? comparison;
            bool invertComparison;

            public override bool CanConvert(List<CommandParameter> p) {
                ComparisonType implicitType = extractFirst<ComparisonCommandParameter>(p).Value;
                return implicitType==ComparisonType.EQUAL && (comparison.HasValue || invertComparison); 
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                ComparisonType comp = comparison.GetValueOrDefault(ComparisonType.EQUAL);
                if (invertComparison) comp = Inverse(comp);
                return new ComparisonCommandParameter(comp);
            }

            public override void Initialize() {
                comparison = null;
                invertComparison = false;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is NotCommandParameter && !invertComparison && comparison == null) {
                    invertComparison = true;
                    return true;
                } else return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is NotCommandParameter && !invertComparison && comparison == null) {
                    invertComparison = true;
                    return true;
                } else if (p is ComparisonCommandParameter && comparison == null) {
                    comparison = ((ComparisonCommandParameter)p).Value;
                    return true;
                } else return false;
            }
        }

        public class BlockComparisonProcessor : SimpleParameterProcessor<ComparisonCommandParameter> {
            Variable comparisonValue;
            PropertyType? property;
            DirectionType? direction;

            public override bool CanConvert(List<CommandParameter> p) {
                return comparisonValue != null || (property.HasValue);
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                ComparisonType comparison = findFirst<ComparisonCommandParameter>(p).Value;

                PrimitiveComparator comparator = new PrimitiveComparator(comparison);

                if (comparisonValue == null) {
                    comparisonValue = new StaticVariable(new BooleanPrimitive(true));
                }

                BlockCondition blockCondition = new BlockPropertyCondition(property, direction, comparator, comparisonValue);

                return new BlockConditionCommandParameter(blockCondition);
            }

            public override void Initialize() {
                comparisonValue = null;
                property = null;
                direction = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is PropertyCommandParameter && property == null) {
                    property = ((PropertyCommandParameter)p).Value;
                } else if (p is DirectionCommandParameter && direction == null) {
                    direction = ((DirectionCommandParameter)p).Value;
                } else return false;
                return true;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is PropertyCommandParameter && property == null) {
                    property = ((PropertyCommandParameter)p).Value;
                } else if (p is DirectionCommandParameter && direction == null) {
                    direction = ((DirectionCommandParameter)p).Value;
                } else if (p is VariableCommandParameter && comparisonValue == null) {
                    comparisonValue = ((VariableCommandParameter)p).Value;
                } else return false;
                return true;
            }
        }

        public class VariableComparisonProcessor : SimpleParameterProcessor<ComparisonCommandParameter> {
            Variable a, b;
            public override bool CanConvert(List<CommandParameter> p) {
                return a != null & b != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                ComparisonType comparison = extractFirst<ComparisonCommandParameter>(p).Value;
                Variable v = new ComparisonVariable(a, b, new PrimitiveComparator(comparison));
                return new VariableCommandParameter(v);
            }

            public override void Initialize() {
                a = null;
                b = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is VariableCommandParameter && a == null) {
                    a = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is VariableCommandParameter && b == null) {
                    b = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }
        }

        public class SelectorProcessor : SimpleParameterProcessor<StringCommandParameter> {
            BlockType? blockType = null;
            bool isGroup = false;

            public override bool CanConvert(List<CommandParameter> p) {
                StringCommandParameter selector = findFirst<StringCommandParameter>(p);
                if (blockType == null) {
                    BlockTypeCommandParameter type = findLast<BlockTypeCommandParameter>(selector.SubTokens);
                    if (type != null) blockType = type.Value;
                    if (selector.SubTokens.Exists(s => s is GroupCommandParameter)) isGroup = true;
                }
                return blockType != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                StringCommandParameter selector = findFirst<StringCommandParameter>(p);
                Variable variable = new StaticVariable(new StringPrimitive(selector.Value));
                return new SelectorCommandParameter(new SelectorEntityProvider(blockType.Value, isGroup, variable));
            }

            public override void Initialize() {
                blockType = null;
                isGroup = false;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is BlockTypeCommandParameter && blockType == null) blockType = ((BlockTypeCommandParameter)p).Value;
                else if (p is GroupCommandParameter && !isGroup) isGroup = true;
                else return false;
                return true;
            }
        }

        public class VariableSelectorProcessor : SimpleParameterProcessor<VariableSelectorCommandParameter> {
            BlockType? blockType = null;
            bool isGroup = false;

            public override bool CanConvert(List<CommandParameter> p) {
                return blockType != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                VariableSelectorCommandParameter selector = findFirst<VariableSelectorCommandParameter>(p);
                Variable variable = selector.Value;
                return new SelectorCommandParameter(new SelectorEntityProvider(blockType.Value, isGroup, variable));
            }

            public override void Initialize() {
                blockType = null;
                isGroup = false;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is BlockTypeCommandParameter && blockType == null) blockType = ((BlockTypeCommandParameter)p).Value;
                else if (p is GroupCommandParameter && !isGroup) isGroup = true;
                else return false;
                return true;
            }
        }

        public class PrintCommandProcessor : SimpleParameterProcessor<PrintCommandParameter> {
            Variable variable;

            public override bool CanConvert(List<CommandParameter> p) {
                return variable != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                return new CommandReferenceParameter(new PrintCommand(variable));
            }

            public override void Initialize() {
                variable = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is VariableCommandParameter && variable == null) {
                    variable = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }
        }

        public class AssignmentProcessor : ParameterProcessor<AssignmentCommandParameter> {
            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters) {
                finalParameters = null;
                if (i == p.Count - 1 || !(p[i + 1] is StringCommandParameter)) return false;
                AssignmentCommandParameter assignment = (AssignmentCommandParameter)p[i];
                StringCommandParameter variableName = (StringCommandParameter)p[i + 1];
                p.RemoveRange(i, 2);
                p.Insert(i, new VariableAssignmentCommandParameter(variableName.Value, assignment.useReference));
                finalParameters = new List<CommandParameter>();
                finalParameters.Add(p[i]);
                return true;
            }
        }

        public class FunctionProcessor : ParameterProcessor<FunctionCommandParameter> {
            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters) {
                finalParameters = null;
                if (i == p.Count - 1 || !(p[i + 1] is StringCommandParameter)) return false;
                FunctionType functionType = ((FunctionCommandParameter)p[i]).Value;
                StringCommandParameter functionName = (StringCommandParameter)p[i + 1];
                FunctionDefinition definition;
                if (!FUNCTIONS.TryGetValue(functionName.Value, out definition)) throw new Exception("Unknown function: " + functionName.Value);
                p.RemoveRange(i, 2);
                p.Insert(i, new FunctionDefinitionCommandParameter(functionType, definition));
                finalParameters = new List<CommandParameter>();
                finalParameters.Add(p[i]);
                return true;
            }
        }

        public class RunArgumentProcessor : ParameterProcessor<StringCommandParameter> {
            public override bool Process(List<CommandParameter> p, int i, out List<CommandParameter> finalParameters) {
                finalParameters = null;
                StringCommandParameter param = (StringCommandParameter)p[i];
                if (param.SubTokens.Count == 0 || !(param.SubTokens[0] is PropertyCommandParameter)) return false;
                if (((PropertyCommandParameter)param.SubTokens[0]).Value != PropertyType.RUN) return false;
                Trace("Found Run Keyword!");
                List<Token> values = ParseTokens(param.Value);
                p.RemoveAt(i);
                values.RemoveAt(0);
                Trace("Arguments: (" + String.Join(" ", values) + ")");
                p.Insert(i, new PropertyCommandParameter(PropertyType.RUN));
                p.Insert(i + 1, new VariableCommandParameter(new StaticVariable(new StringPrimitive(String.Join(" ", values)))));
                finalParameters = p.GetRange(i, 2);
                return true;
            }
        }

        public class WaitProcessor : SimpleCommandProcessor<WaitCommandParameter> {
            bool unitIndex, timeIndex;

            public override Command GetCommand(List<CommandParameter> commandParameters) {
                return new WaitCommand(commandParameters);
            }

            public override void Initialize() {
                unitIndex = false;
                timeIndex = false;
            }

            public override bool CanConvert() {
                return true;
            }

            public override bool ProcessParameterArgument(CommandParameter p) {
                if (p is UnitCommandParameter && !unitIndex) unitIndex = true;
                else if (p is VariableCommandParameter && !timeIndex) timeIndex = true;
                else return false;
                return true;
            }
        }

        public class ControlProcessor : SimpleCommandProcessor<ControlCommandParameter> {
            bool loopIndex;

            public override void Initialize() {
                loopIndex = false;
            }

            public override Command GetCommand(List<CommandParameter> commandParameters) {
                return new ControlCommand(commandParameters);
            }

            public override bool CanConvert() {
                return true;
            }

            public override bool ProcessParameterArgument(CommandParameter p) {
                if (p is VariableCommandParameter && !loopIndex) loopIndex = true;
                else return false;
                return true;
            }
        }

        public class SendCommandProcessor : SimpleCommandProcessor<SendCommandParameter> {
            bool messageIndex, tagIndex;

            public override Command GetCommand(List<CommandParameter> commandParameters) {
                return new SendCommand(commandParameters);
            }

            public override void Initialize() {
                tagIndex = false;
                messageIndex = false;
            }

            public override bool CanConvert() {
                return tagIndex && messageIndex;
            }

            public override bool ProcessParameterArgument(CommandParameter p) {
                if (p is VariableCommandParameter && !messageIndex) messageIndex = true;
                else if (p is VariableCommandParameter && !tagIndex) tagIndex = true;
                else return false;
                return true;
            }
        }

        public class ListenCommandProcessor : SimpleCommandProcessor<ListenCommandParameter> {
            bool tagIndex;

            public override Command GetCommand(List<CommandParameter> commandParameters) {
                return new ListenCommand(commandParameters);
            }

            public override void Initialize() {
                tagIndex = false;
            }

            public override bool CanConvert() {
                return tagIndex;
            }

            public override bool ProcessParameterArgument(CommandParameter p) {
                if (p is VariableCommandParameter && !tagIndex) tagIndex = true;
                else return false;
                return true;
            }
        }

        public class VariableAssignmentProcesor : SimpleParameterProcessor<VariableAssignmentCommandParameter> {
            Variable variable;

            public override bool CanConvert(List<CommandParameter> p) {
                return variable != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                VariableAssignmentCommandParameter assignment = findFirst<VariableAssignmentCommandParameter>(p);
                Command command = new VariableAssignmentCommand(assignment.variableName, variable, assignment.useReference);
                return new CommandReferenceParameter(command);
            }

            public override void Initialize() {
                variable = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is VariableCommandParameter && variable == null) {
                    variable = ((VariableCommandParameter)p).Value;
                    return true;
                } else return false;
            }
        }

        public class FunctionCallCommandProcessor : SimpleParameterProcessor<FunctionDefinitionCommandParameter> {
            List<Variable> variables;

            public override bool CanConvert(List<CommandParameter> p) {
                FunctionDefinition function = findFirst<FunctionDefinitionCommandParameter>(p).functionDefinition;
                return function.parameterNames.Count() == variables.Count;

            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                FunctionDefinitionCommandParameter function = findFirst<FunctionDefinitionCommandParameter>(p);
                FunctionDefinition definition = function.functionDefinition;
                FunctionType functionType = function.functionType;
                Dictionary<string, Variable> inputParameters = new Dictionary<string, Variable>();
                for (int i = 0; i < definition.parameterNames.Count(); i++) {
                    inputParameters[definition.parameterNames[i]] = variables[i];
                }
                Command command = new FunctionCommand(functionType, definition, inputParameters);
                return new CommandReferenceParameter(command);
            }

            public override void Initialize() {
                variables = new List<Variable>();
            }

            public override bool ProcessLeft(CommandParameter p) {
                return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is VariableCommandParameter) {
                    variables.Add(((VariableCommandParameter)p).Value);
                    return true;
                } else return false;
            }
        }

        public class AsyncCommandProcessor : SimpleCommandProcessor<AsyncCommandParameter> {
            Command command = null;

            public override Command GetCommand(List<CommandParameter> commandParameters) {
                command.Async = true;
                return command;
            }

            public override void Initialize() {
                command = null;
            }

            public override bool CanConvert() {
                return command != null;
            }

            public override bool ProcessParameterArgument(CommandParameter p) {
                if (p is CommandReferenceParameter && command == null) {
                    command = ((CommandReferenceParameter)p).Value;
                    return true;
                }
                return false;
            }
        }

        public class ActionProcessor : SimpleParameterProcessor<SelectorCommandParameter> {
            bool hasDirection, hasVariable, hasProperty, hasReverse, hasRelative, hasAction;

            public override void Initialize() {
                hasAction = false;
                hasProperty = false;
                hasVariable = false;
                hasDirection = false;
                hasRelative = false;
                hasDirection = false;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return Process(p);
            }

            public override bool ProcessRight(CommandParameter p) {
                return Process(p);
            }

            private bool Process(CommandParameter p) {
                if (p is DirectionCommandParameter && !hasDirection) hasDirection = true;
                else if (p is VariableCommandParameter && !hasVariable) hasVariable = true;
                else if (p is PropertyCommandParameter && !hasProperty) hasProperty = true;
                else if (p is ReverseCommandParameter && !hasReverse) hasReverse = true;
                else if (p is RelativeCommandParameter && !hasRelative) hasRelative = true;
                else if ((p is ActionCommandParameter) && !hasAction) hasAction = true;
                else return false;
                return true;
            }

            public override bool CanConvert(List<CommandParameter> p) {
                return hasVariable || hasProperty || hasDirection || hasReverse;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                BlockCommand command = new BlockCommand(p);
                return new CommandReferenceParameter(command);
            }
        }

        public class IteratorProcessor : SimpleParameterProcessor<IteratorCommandParameter> {
            Variable iterations;
            public override bool CanConvert(List<CommandParameter> p) {
                return iterations != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                return new IterationCommandParameter(iterations);
            }

            public override void Initialize() {
                iterations = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is VariableCommandParameter && iterations == null) {
                    iterations = ((VariableCommandParameter)p).Value;
                    return true;
                }
                return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                return false;
            }
        }

        public class IterationProcessor : SimpleParameterProcessor<IterationCommandParameter> {
            Command command = null;

            public override bool CanConvert(List<CommandParameter> p) {
                return command != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                IterationCommandParameter iterations = extractFirst<IterationCommandParameter>(p);
                MultiActionCommand multiCommand = new MultiActionCommand(new List<Command>() { command }, iterations.Value);
                return new CommandReferenceParameter(multiCommand);
            }

            public override void Initialize() {
                command = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return Process(p);
            }

            public override bool ProcessRight(CommandParameter p) {
                return Process(p);
            }

            private bool Process(CommandParameter p) {
                if (p is CommandReferenceParameter && command == null) {
                    command = ((CommandReferenceParameter)p).Value;
                    return true;
                }
                return false;
            }
        }

        public class IfProcessor : SimpleParameterProcessor<IfCommandParameter> {
            Variable condition;
            public override bool CanConvert(List<CommandParameter> p) {
                return condition != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                IfCommandParameter parameter = extractFirst<IfCommandParameter>(p);
                if (parameter.inverseCondition) condition = new NotVariable(condition);

                return new ConditionCommandParameter(condition, parameter.alwaysEvaluate, parameter.swapCommands);
            }

            public override void Initialize() {
                condition = null;
            }

            public override bool ProcessLeft(CommandParameter p) {
                return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if(p is VariableCommandParameter && condition == null) {
                    condition = ((VariableCommandParameter)p).Value;
                    return true;
                }
                return false;
            }
        }

        public class ConditionalCommandProcessor : SimpleParameterProcessor<ConditionCommandParameter> {
            Command conditionMetCommand, conditionNotMetCommand;
            bool otherwise;

            public override bool CanConvert(List<CommandParameter> p) {
                return conditionMetCommand != null;
            }

            public override CommandParameter Convert(List<CommandParameter> p) {
                ConditionCommandParameter condition = extractFirst<ConditionCommandParameter>(p);
                if (conditionNotMetCommand == null) conditionNotMetCommand = new NullCommand(); 

                if (condition.swapCommands) {
                    var temp = conditionMetCommand;
                    conditionMetCommand = conditionNotMetCommand;
                    conditionNotMetCommand = temp;
                }

                Command command = new ConditionalCommand(condition.Value, conditionMetCommand, conditionNotMetCommand, condition.alwaysEvaluate);
                return new CommandReferenceParameter(command);
            }

            public override void Initialize() {
                conditionMetCommand = null;
                conditionNotMetCommand = null;
                otherwise = false;
            }

            public override bool ProcessLeft(CommandParameter p) {
                if (p is CommandReferenceParameter && conditionMetCommand == null) {
                    conditionMetCommand = ((CommandReferenceParameter)p).Value;
                    return true;
                } else return false;
            }

            public override bool ProcessRight(CommandParameter p) {
                if (p is CommandReferenceParameter) {
                    if (otherwise) {
                        if (conditionNotMetCommand == null) {
                            conditionNotMetCommand = ((CommandReferenceParameter)p).Value;
                            return true;
                        } else return false;
                    } else {
                        if (conditionMetCommand == null) {
                            conditionMetCommand = ((CommandReferenceParameter)p).Value;
                            return true;
                        } else return false;
                    }
                } else if (p is ElseCommandParameter && !otherwise) {
                    otherwise = true;
                    return true;
                } else return false;
            }
        }

    }
}
