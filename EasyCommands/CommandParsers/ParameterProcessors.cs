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

        public interface IParameterProcessor : IComparable<IParameterProcessor> {
            int Rank { get; set; }
            Type GetProcessedTypes();
            bool CanProcess(ICommandParameter p);
            bool Process(List<ICommandParameter> p, int i, out List<ICommandParameter> finalParameters, List<List<ICommandParameter>> branches);
        }

        public abstract class ParameterProcessor<T> : IParameterProcessor where T : class, ICommandParameter {
            public int Rank { get; set; }
            public virtual Type GetProcessedTypes() => typeof(T);
            public int CompareTo(IParameterProcessor other) => Rank.CompareTo(other.Rank);
            public bool CanProcess(ICommandParameter p) => p is T;
            public abstract bool Process(List<ICommandParameter> p, int i, out List<ICommandParameter> finalParameters, List<List<ICommandParameter>> branches);
        }

        public class ParenthesisProcessor : ParameterProcessor<OpenParenthesisCommandParameter> {
            public override bool Process(List<ICommandParameter> p, int i, out List<ICommandParameter> finalParameters, List<List<ICommandParameter>> branches) {
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

                        foreach (var branch in alternateBranches) {
                            branch.Insert(0, new OpenParenthesisCommandParameter());
                            branch.Add(new CloseParenthesisCommandParameter());
                            var copy = new List<ICommandParameter>(p);
                            copy.InsertRange(i, branch);
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
            public override bool Process(List<ICommandParameter> p, int i, out List<ICommandParameter> finalParameters, List<List<ICommandParameter>> branches) {
                finalParameters = null;
                var indexValues = NewList<IVariable>();
                int startIndex = i;
                for (int j = startIndex + 1; j < p.Count; j++) {
                    if (p[j] is OpenBracketCommandParameter) return false;
                    else if (p[j] is ListSeparatorCommandParameter) {
                        indexValues.Add(ParseVariable(p, startIndex, j));
                        startIndex = j; //set startIndex to next separator
                    } else if (p[j] is CloseBracketCommandParameter) {
                        if (j > i + 1) indexValues.Add(ParseVariable(p, startIndex, j)); //dont try to parse []
                        finalParameters = NewList<ICommandParameter>(new ListCommandParameter(GetStaticVariable(NewKeyedList(indexValues))));
                        p.RemoveRange(i, j - i + 1);
                        p.InsertRange(i, finalParameters);
                        return true;
                    }
                }
                throw new Exception("Missing Closing Bracket for List");
            }

            IVariable ParseVariable(List<ICommandParameter> p, int startIndex, int endIndex) {
                var range = p.GetRange(startIndex + 1, endIndex - (startIndex + 1));
                var variable = PROGRAM.ParseParameters<ValueCommandParameter<IVariable>>(range);
                if (variable == null) throw new Exception("List Index Values Must Resolve To a Variable");
                return variable.value;
            }
        }

        public class MultiListProcessor : ParameterProcessor<ListCommandParameter> {
            public override bool Process(List<ICommandParameter> p, int i, out List<ICommandParameter> finalParameters, List<List<ICommandParameter>> branches) {
                while (i > 1 && p[i - 1] is ListCommandParameter) i--;
                finalParameters = NewList<ICommandParameter>(new ListIndexCommandParameter(new ListIndexVariable(((ListCommandParameter)p[i]).value, EmptyList())));
                p[i] = finalParameters[0];
                return true;
            }
        }

        class BranchingProcessor<T> : ParameterProcessor<T> where T : class, ICommandParameter {
            List<ParameterProcessor<T>> processors;

            public BranchingProcessor(params ParameterProcessor<T>[] p) {
                processors = NewList(p);
            }

            public override bool Process(List<ICommandParameter> p, int i, out List<ICommandParameter> finalParameters, List<List<ICommandParameter>> branches) {
                finalParameters = null;
                var eligibleProcessors = processors.Where(processor => processor.CanProcess(p[i])).ToList();
                var copy = new List<ICommandParameter>(p);

                bool processed = false;
                foreach (IParameterProcessor processor in eligibleProcessors) {
                    if (processed) {
                        List<ICommandParameter> ignored;
                        var additionalCopy = new List<ICommandParameter>(copy);
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
            var increaseProcessor = requiredLeft<IncreaseCommandParameter>();
            var incrementProcessor = requiredRight<IncrementCommandParameter>();
            var variableProcessor = requiredEither<VariableCommandParameter>();
            var propertyProcessor = requiredEither<PropertySupplierCommandParameter>();
            var directionProcessor = requiredEither<DirectionCommandParameter>();
            var reverseProcessor = requiredEither<ReverseCommandParameter>();
            var notProcessor = requiredEither<NotCommandParameter>();
            var relativeProcessor = requiredRight<RelativeCommandParameter>();
            var processors = NewList<IDataProcessor>(
                assignmentProcessor,
                increaseProcessor,
                incrementProcessor,
                variableProcessor,
                propertyProcessor,
                directionProcessor,
                reverseProcessor,
                notProcessor,
                relativeProcessor);

            CanConvert<SelectorCommandParameter> canConvert = p => processors.Exists(x => x.Satisfied() && x != directionProcessor && x != propertyProcessor);
            Convert<SelectorCommandParameter> convert = p => {
                PropertySupplier propertySupplier = propertyProcessor.Satisfied() ? propertyProcessor.GetValue().value : new PropertySupplier();
                if (directionProcessor.Satisfied()) propertySupplier = propertySupplier.WithDirection(directionProcessor.GetValue().value);

                IVariable variableValue = GetStaticVariable(true);
                if (variableProcessor.Satisfied()) {
                    variableValue = variableProcessor.GetValue().value;
                    propertySupplier = propertySupplier.WithPropertyValue(variableValue);
                }

                if (notProcessor.Satisfied()) {
                    variableValue = new UniOperandVariable(UniOperand.REVERSE, variableValue);
                    propertySupplier = propertySupplier.WithPropertyValue(variableValue);
                }

                if (incrementProcessor.Satisfied()) {
                    propertySupplier = propertySupplier.WithIncrement(incrementProcessor.GetValue().value);
                } else if (increaseProcessor.Satisfied()) {
                    propertySupplier = propertySupplier.WithIncrement(increaseProcessor.GetValue().value);
                } else if (relativeProcessor.Satisfied()) propertySupplier = propertySupplier.WithIncrement(true);

                Action<IBlockHandler, Object> blockAction;
                if (AllSatisfied(reverseProcessor)) blockAction = (b, e) => b.ReverseNumericPropertyValue(e, propertySupplier.Resolve(b));
                else if (increaseProcessor.Satisfied() || incrementProcessor.Satisfied() || relativeProcessor.Satisfied()) blockAction = (b, e) => b.IncrementPropertyValue(e, propertySupplier.Resolve(b));
                else if (AllSatisfied(directionProcessor)) blockAction = (b, e) => b.UpdatePropertyValue(e, propertySupplier.Resolve(b));
                else blockAction = (b, e) => b.UpdatePropertyValue(e, propertySupplier.WithPropertyValue(variableValue).Resolve(b));

                return new CommandReferenceParameter(new BlockCommand(p.value, blockAction));
            };

            return new RuleProcessor<SelectorCommandParameter>(processors, canConvert, convert);
        }

        static ICommandParameter ConvertConditionalCommand(ConditionCommandParameter condition, CommandReferenceParameter metFetcher, ElseCommandParameter otherwise, CommandReferenceParameter notMetFetcher) {
            Command metCommand = metFetcher.value;
            Command notMetCommand = otherwise != null ? notMetFetcher.value : new NullCommand();
            if (condition.swapCommands) {
                var temp = metCommand;
                metCommand = notMetCommand;
                notMetCommand = temp;
            }
            return new CommandReferenceParameter(new ConditionalCommand(condition.value, metCommand, notMetCommand, condition.alwaysEvaluate));
        }

        //Rule Processors
        class RuleProcessor<T> : ParameterProcessor<T> where T : class, ICommandParameter {
            List<IDataProcessor> processors;
            CanConvert<T> canConvert;
            Convert<T> convert;

            public RuleProcessor(List<IDataProcessor> proc, CanConvert<T> canConv, Convert<T> conv) {
                processors = proc;
                canConvert = canConv;
                convert = conv;
            }

            public override bool Process(List<ICommandParameter> p, int i, out List<ICommandParameter> finalParameters, List<List<ICommandParameter>> branches) {
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
                if (converted is ICommandParameter) {
                    finalParameters = NewList((ICommandParameter)converted);
                } else if (converted is List<ICommandParameter>) {
                    finalParameters = (List<ICommandParameter>)converted;
                } else throw new Exception("Final parameters must be CommandParameter");
                p.InsertRange(k, finalParameters);
                return true;
            }
        }

        static RuleProcessor<T> NoValueRule<T>(Supplier<T> type, Convert<T> convert) where T : class, ICommandParameter => NoValueRule(type, p => true, convert);

        static RuleProcessor<T> NoValueRule<T>(Supplier<T> type, CanConvert<T> canConvert, Convert<T> convert) where T : class, ICommandParameter =>
            new RuleProcessor<T>(NewList<IDataProcessor>(), canConvert, convert);

        static RuleProcessor<T> OneValueRule<T, U>(Supplier<T> type, DataProcessor<U> u, OneValueConvert<T, U> convert) where T : class, ICommandParameter =>
            OneValueRule(type, u, (p, a) => a.Satisfied(), convert);

        static RuleProcessor<T> OneValueRule<T, U>(Supplier<T> type, DataProcessor<U> u, OneValueCanConvert<T, U> canConvert, OneValueConvert<T, U> convert) where T : class, ICommandParameter =>
            new RuleProcessor<T>(NewList<IDataProcessor>(u), p => canConvert(p, u), p => convert(p, u.GetValue()));

        static RuleProcessor<T> TwoValueRule<T, U, V>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, TwoValueConvert<T, U, V> convert) where T : class, ICommandParameter =>
            TwoValueRule(type, u, v, (p, a, b) => AllSatisfied(a, b), convert);

        static RuleProcessor<T> TwoValueRule<T, U, V>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, TwoValueCanConvert<T, U, V> canConvert, TwoValueConvert<T, U, V> convert) where T : class, ICommandParameter =>
            new RuleProcessor<T>(NewList<IDataProcessor>(u, v), p => canConvert(p, u, v), p => convert(p, u.GetValue(), v.GetValue()));

        static RuleProcessor<T> ThreeValueRule<T, U, V, W>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, ThreeValueConvert<T, U, V, W> convert) where T : class, ICommandParameter =>
            ThreeValueRule(type, u, v, w, (p, a, b, c) => AllSatisfied(a, b, c), convert);

        static RuleProcessor<T> ThreeValueRule<T, U, V, W>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, ThreeValueCanConvert<T, U, V, W> canConvert, ThreeValueConvert<T, U, V, W> convert) where T : class, ICommandParameter =>
            new RuleProcessor<T>(NewList<IDataProcessor>(u, v, w), p => canConvert(p, u, v, w), p => convert(p, u.GetValue(), v.GetValue(), w.GetValue()));

        static RuleProcessor<T> FourValueRule<T, U, V, W, X>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, DataProcessor<X> x, FourValueConvert<T, U, V, W, X> convert) where T : class, ICommandParameter =>
            FourValueRule(type, u, v, w, x, (p, a, b, c, d) => AllSatisfied(a, b, c, d), convert);

        static RuleProcessor<T> FourValueRule<T, U, V, W, X>(Supplier<T> type, DataProcessor<U> u, DataProcessor<V> v, DataProcessor<W> w, DataProcessor<X> x, FourValueCanConvert<T, U, V, W, X> canConvert, FourValueConvert<T, U, V, W, X> convert) where T : class, ICommandParameter =>
            new RuleProcessor<T>(NewList<IDataProcessor>(u, v, w, x), p => canConvert(p, u, v, w, x), p => convert(p, u.GetValue(), v.GetValue(), w.GetValue(), x.GetValue()));

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
