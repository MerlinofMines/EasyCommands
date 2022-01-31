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
                for (int j = i + 1; j < p.Count; j++) {
                    if (p[j] is OpenParenthesisCommandParameter) return false;
                    else if (p[j] is CloseParenthesisCommandParameter) {
                        var alternateBranches = NewList(p.GetRange(i + 1, j - (i + 1)));
                        p.RemoveRange(i, j - i + 1);

                        while (alternateBranches.Count > 0) {
                            finalParameters = alternateBranches[0];
                            alternateBranches.AddRange(PROGRAM.ProcessParameters(finalParameters));
                            alternateBranches.RemoveAt(0);
                            if (finalParameters.Count == 1) break;
                        }

                        for (int k = 0; k < alternateBranches.Count; k++) {
                            alternateBranches[k].Insert(0, new OpenParenthesisCommandParameter());
                            alternateBranches[k].Add(new CloseParenthesisCommandParameter());
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
                    } else if (p[j] is CloseBracketCommandParameter) {
                        if (j > i + 1) indexValues.Add(ParseVariable(p, startIndex, j)); //dont try to parse []
                        finalParameters = NewList<CommandParameter>(new ListCommandParameter(GetStaticVariable(NewKeyedList(indexValues))));
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
                while (i > 1 && p[i - 1] is ListCommandParameter) i--;
                finalParameters = NewList<CommandParameter>(new ListIndexCommandParameter(new ListIndexVariable(((ListCommandParameter)p[i]).value, GetVariables(NewKeyedList())[0])));
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

                if (primitive != null || value.isExplicit) {
                    variable = new StaticVariable(primitive ?? ResolvePrimitive(value.value));
                    if (variable.GetValue().returnType == Return.VECTOR)
                        variable = new VectorVariable(variable);
                }
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
                foreach (ParameterProcessor processor in eligibleProcessors) {
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

        static RuleProcessor<BiOperandCommandParameter> BiOperandProcessor(int tier) =>
            TwoValueRule(Type<BiOperandCommandParameter>, requiredLeft<VariableCommandParameter>(), requiredRight<VariableCommandParameter>(),
                (operand, left, right) => operand.tier == tier && AllSatisfied(left, right),
                (operand, left, right) => new VariableCommandParameter(new BiOperandVariable(operand.value, left.value, right.value)));

        static RuleProcessor<SelectorCommandParameter> BlockCommandProcessor() {
            var assignmentProcessor = eitherList<AssignmentCommandParameter>(true);
            var incrementProcessor = eitherList<IncrementCommandParameter>(true);
            var variableProcessor = requiredEither<VariableCommandParameter>();
            var propertyProcessor = requiredEither<PropertySupplierCommandParameter>();
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
                PropertySupplier propertySupplier = propertyProcessor.Satisfied() ? propertyProcessor.GetValue().value : new PropertySupplier();
                if (directionProcessor.Satisfied()) propertySupplier = propertySupplier.WithDirection(directionProcessor.GetValue().value);

                Variable variableValue = GetStaticVariable(true);
                if (variableProcessor.Satisfied()) {
                    variableValue = variableProcessor.GetValue().value;
                    propertySupplier = propertySupplier.WithPropertyValue(variableValue);
                }

                if (notProcessor.Satisfied()) {
                    variableValue = new UniOperandVariable(UniOperand.REVERSE, variableValue);
                    propertySupplier = propertySupplier.WithPropertyValue(variableValue);
                }

                if (incrementProcessor.Satisfied()) {
                    propertySupplier = propertySupplier.WithIncrement(incrementProcessor.GetValue()
                        .Select(v => v.value)
                        .Aggregate((a, b) => a && b));
                }

                Action<BlockHandler, Object> blockAction;
                if (AllSatisfied(reverseProcessor)) blockAction = (b, e) => b.ReverseNumericPropertyValue(e, propertySupplier.Resolve(b));
                else if (AllSatisfied(incrementProcessor)) blockAction = (b, e) => b.IncrementPropertyValue(e, propertySupplier.Resolve(b));
                else if (AllSatisfied(directionProcessor)) blockAction = (b, e) => b.UpdatePropertyValue(e, propertySupplier.Resolve(b));
                else blockAction = (b, e) => b.UpdatePropertyValue(e, propertySupplier.WithPropertyValue(variableValue).Resolve(b));

                return new CommandReferenceParameter(new BlockCommand(p.value, blockAction));
            };

            return new RuleProcessor<SelectorCommandParameter>(processors, canConvert, convert);
        }

        static CommandParameter ConvertConditionalCommand(ConditionCommandParameter condition, CommandReferenceParameter metFetcher,
            Optional<ElseCommandParameter> otherwise, Optional<CommandReferenceParameter> notMetFetcher) {
            Command metCommand = metFetcher.value;
            Command notMetCommand = otherwise.HasValue() ? notMetFetcher.GetValue().value : new NullCommand();
            if (condition.swapCommands) {
                var temp = metCommand;
                metCommand = notMetCommand;
                notMetCommand = temp;
            }
            return new CommandReferenceParameter(new ConditionalCommand(condition.value, metCommand, notMetCommand, condition.alwaysEvaluate));
        }

        //Rule Processors
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

        static RuleProcessor<T> NoValueRule<T>(Supplier<T> type, Convert<T> convert) where T : class, CommandParameter => NoValueRule(type, (p) => true, convert);

        static RuleProcessor<T> NoValueRule<T>(Supplier<T> type, CanConvert<T> canConvert, Convert<T> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(), canConvert, convert);

        static RuleProcessor<T> OneValueRule<T, U>(Supplier<T> type, DataProcessor<U> u, OneValueConvert<T, U> convert) where T : class, CommandParameter =>
            OneValueRule(type, u, (p, a) => a.Satisfied(), convert);

        static RuleProcessor<T> OneValueRule<T, U>(Supplier<T> type, DataProcessor<U> u, OneValueCanConvert<T, U> canConvert, OneValueConvert<T, U> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u), (p) => canConvert(p, u), (p) => convert(p, u.GetValue()));

        static RuleProcessor<T> TwoValueRule<T, U, V>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, TwoValueConvert<T, U, V> convert) where T : class, CommandParameter =>
            TwoValueRule(type, u, v, (p, a, b) => AllSatisfied(a, b), convert);

        static RuleProcessor<T> TwoValueRule<T, U, V>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, TwoValueCanConvert<T, U, V> canConvert, TwoValueConvert<T, U, V> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v), (p) => canConvert(p, u, v), (p) => convert(p, u.GetValue(), v.GetValue()));

        static RuleProcessor<T> ThreeValueRule<T, U, V, W>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, ThreeValueConvert<T, U, V, W> convert) where T : class, CommandParameter =>
            ThreeValueRule(type, u, v, w, (p, a, b, c) => AllSatisfied(a, b, c), convert);

        static RuleProcessor<T> ThreeValueRule<T, U, V, W>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, ThreeValueCanConvert<T, U, V, W> canConvert, ThreeValueConvert<T, U, V, W> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w), p => canConvert(p, u, v, w), p => convert(p, u.GetValue(), v.GetValue(), w.GetValue()));

        static RuleProcessor<T> FourValueRule<T, U, V, W, X>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, DataProcessor<X> x, FourValueConvert<T, U, V, W, X> convert) where T : class, CommandParameter =>
            FourValueRule(type, u, v, w, x, (p, a, b, c, d) => AllSatisfied(a, b, c, d), convert);

        static RuleProcessor<T> FourValueRule<T, U, V, W, X>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, DataProcessor<X> x, FourValueCanConvert<T, U, V, W, X> canConvert, FourValueConvert<T, U, V, W, X> convert) where T : class, CommandParameter =>
            new RuleProcessor<T>(NewList<DataProcessor>(u, v, w, x), p => canConvert(p, u, v, w, x), p => convert(p, u.GetValue(), v.GetValue(), w.GetValue(), x.GetValue()));

        //Utility delegates to efficiently create Rule Processors
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
    }
}
