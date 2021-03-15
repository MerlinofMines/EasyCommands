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
            private static List<ParameterProcessor> parameterProcessors = new List<ParameterProcessor>
            {
                  new SelectorProcessor(),
                  new ConditionalSelectorProcessor(),
                  new RunArgumentProcessor(),
                  new SimpleVariableProcessor(),
                  new IfProcessor(),
                  new IterationProcessor(),
                  new FunctionProcessor(),
                  new ActionProcessor(),
            };

            public static void process(List<CommandParameter> commandParameters) {
                foreach (ParameterProcessor parser in parameterProcessors) {
                    Debug("Start Parameter Processor: " + parser.GetType());
                    Debug("Pre Processed Parameters:");
                    commandParameters.ForEach(param => Debug("Type: " + param.GetType()));
                    parser.Process(commandParameters);
                    Debug("End Parameter Processor: " + parser.GetType());
                    Debug("Post Processed Parameters:");
                    commandParameters.ForEach(param => Debug("Type: " + param.GetType()));
                }
            }
        }

        public abstract class ParameterProcessor {
            protected abstract bool ShouldProcess(CommandParameter p);
            protected abstract void ConvertNext(List<CommandParameter> p, ref int i);
            public void Process(List<CommandParameter> p) { for (int i = 0; i < p.Count; i++) { if (ShouldProcess(p[i])) ConvertNext(p, ref i); } }
        }

        public class SimpleVariableProcessor : ParameterProcessor {
            protected override bool ShouldProcess(CommandParameter p) {
                return p is StringCommandParameter ||
                    p is NumericCommandParameter ||
                    p is BooleanCommandParameter;
            }
            protected override void ConvertNext(List<CommandParameter> p, ref int i) {
                if (p[i] is StringCommandParameter) {
                    p[i] = new VariableCommandParameter(new StaticVariable(new StringPrimitive(((StringCommandParameter)p[i]).Original)));
                } else if (p[i] is NumericCommandParameter) {
                    p[i] = new VariableCommandParameter(new StaticVariable(new NumberPrimitive(((NumericCommandParameter)p[i]).Value)));
                } else if (p[i] is BooleanCommandParameter) {
                    p[i] = new VariableCommandParameter(new StaticVariable(new BooleanPrimitive(((BooleanCommandParameter)p[i]).Value)));
                }
            }
        }

        public class SelectorProcessor : ParameterProcessor {
            protected override bool ShouldProcess(CommandParameter p) {
                return p is GroupCommandParameter || p is StringCommandParameter || p is BlockTypeCommandParameter || p is IndexCommandParameter;
            }
            protected override void ConvertNext(List<CommandParameter> p, ref int i) {
                bool isGroup = false;
                StringCommandParameter selector = null;
                BlockTypeCommandParameter blockType = null;
                Variable index = null;
                int paramCount = 0;

                while (i + paramCount < p.Count) {
                    CommandParameter param = p[i + paramCount];

                    if (param is GroupCommandParameter) isGroup = true;
                    else if (param is StringCommandParameter && selector == null) selector = ((StringCommandParameter)param);
                    else if (param is BlockTypeCommandParameter && blockType == null) blockType = ((BlockTypeCommandParameter)param);
                    else if (param is IndexCommandParameter) index = ((IndexCommandParameter)param).Value;
                    else break;
                    paramCount++;
                }

                if (selector == null) throw new Exception("All selectors must have a string identifier");

                if (blockType == null) {
                    int blockTypeIndex = selector.SubTokens.FindLastIndex(s => s is BlockTypeCommandParameter);//Use Last Block Type
                    if (blockTypeIndex < 0) return; //Apparently not a Selector
                    if (selector.SubTokens.Exists(s => s is GroupCommandParameter)) isGroup = true;
                    blockType = ((BlockTypeCommandParameter)selector.SubTokens[blockTypeIndex]);
                }

                Debug("Converted String at index: " + i + " to SelectorCommandParamter");
                p.RemoveRange(i, paramCount);

                EntityProvider entityProvider = new SelectorEntityProvider(blockType.GetBlockType(), isGroup, selector.Value);
                if (index != null) entityProvider = new IndexEntityProvider(entityProvider, index);

                p.Insert(i, new SelectorCommandParameter(entityProvider));
            }
        }

        //TODO: Take all following VariableCommandParameters as function parameters?  What if they need to be resolved?
        public class FunctionProcessor : ParameterProcessor {
            protected override bool ShouldProcess(CommandParameter p) { return p is FunctionCommandParameter; }
            protected override void ConvertNext(List<CommandParameter> p, ref int i) {
                if (i == p.Count - 1 || !(p[i + 1] is VariableCommandParameter)) throw new Exception("Function Name Required after Function Call Keywords");
                FunctionCommand function = new FunctionCommand(p.GetRange(i, 2));
                p.RemoveRange(i, 2);
                p.Insert(i, new CommandReferenceParameter(function));
            }
        }

        public class RunArgumentProcessor : ParameterProcessor {
            protected override bool ShouldProcess(CommandParameter p) { return p is StringCommandParameter; }
            protected override void ConvertNext(List<CommandParameter> p, ref int i) {
                StringCommandParameter param = (StringCommandParameter)p[i];
                if (param.SubTokens.Count == 0 || !(param.SubTokens[0] is PropertyCommandParameter)) return;
                if (((PropertyCommandParameter)param.SubTokens[0]).Value != PropertyType.RUN) return;
                Debug("Found Run Keyword!");
                List<Token> values = ParseTokens(param.Value);
                p.RemoveAt(i);
                values.RemoveAt(0);
                Debug("Arguments: (" + String.Join(" ", values) + ")");
                p.Insert(i, new PropertyCommandParameter(PropertyType.RUN));
                p.Insert(i + 1, new StringCommandParameter(String.Join(" ", values)));
            }
        }

        public class ActionProcessor : ParameterProcessor {
            protected override bool ShouldProcess(CommandParameter p) {
                return p is SelectorCommandParameter ||
                    p is DirectionCommandParameter ||
                    p is VariableCommandParameter ||
                    p is PropertyCommandParameter ||
                    p is ReverseCommandParameter ||
                    p is RelativeCommandParameter ||
                    p is WaitCommandParameter ||
                    p is UnitCommandParameter ||
                    p is ControlCommandParameter ||
                    p is ListenCommandParameter ||
                    p is SendCommandParameter ||
                    p is ActionCommandParameter;
            }

            protected override void ConvertNext(List<CommandParameter> p, ref int i) {
                int paramCount = 0;
                while (i + paramCount < p.Count) {
                    if (!ShouldProcess(p[i + paramCount])) break;
                    paramCount++;
                }
                List<CommandParameter> action = p.GetRange(i, paramCount);

                Command command;
                if (action.Exists(a => a is ListenCommandParameter)) command = new ListenCommand(action);
                else if (action.Exists(a => a is SendCommandParameter)) command = new SendCommand(action);
                else if (action.Exists(a => a is ControlCommandParameter)) command = new ControlCommand(action);
                else if (action.Exists(a => a is WaitCommandParameter)) command = new WaitCommand(action);
                else if (action.Exists(a => a is SelectorCommandParameter)) command = new BlockCommand(action);
                else throw new Exception("Unknown Command Reference Type.");
                p.RemoveRange(i, paramCount);
                p.Insert(i, new CommandReferenceParameter(command));
            }
        }

        public class IterationProcessor : ParameterProcessor {
            protected override void ConvertNext(List<CommandParameter> p, ref int i) {
                IterationCommandParameter icp = (IterationCommandParameter)p[i];
                if (i == 0 || !(p[i - 1] is VariableCommandParameter)) throw new Exception("Iteration must be preceded by a variable");
                icp.Value = ((VariableCommandParameter)p[i - 1]).Value;
                p.RemoveAt(i - 1);
            }

            protected override bool ShouldProcess(CommandParameter p) {
                return p is IterationCommandParameter;
            }
        }

        public class IfProcessor : ParameterProcessor {
            protected override bool ShouldProcess(CommandParameter p) {
                return p is IfCommandParameter;
            }

            protected override void ConvertNext(List<CommandParameter> p, ref int i) {
                i++;
                int length = 0;
                while (i+length < p.Count && IsVariable(p[i+length])) {
                    length++;
                }
                Print("Length: " + length);
                Variable condition = ConvertToVariable(p.GetRange(i, length));
                p.RemoveRange(i, length);
                p.Insert(i, new ConditionCommandParameter(condition));
            }
        }

        static bool IsVariable(CommandParameter p) {
            return p is OpenParenthesisCommandParameter ||
                p is CloseParenthesisCommandParameter ||
                p is AndCommandParameter ||
                p is OrCommandParameter ||
                p is NotCommandParameter ||
                p is AggregationModeCommandParameter ||
                p is ComparisonCommandParameter ||
                p is SelectorCommandParameter ||
                p is PropertyCommandParameter ||
                p is DirectionCommandParameter ||
                p is VariableCommandParameter;

            //TODO: Add AggregateProperty
            //TODO: Add Operation
        }

        //Attempts to convert entire input list into 1 VariableCommandParameter
        static Variable ConvertToVariable(List<CommandParameter> parameters) {

            foreach(var p in parameters) {
                Print("Parameter:  " + p);
            }

            int openPars = -1;
            int closedPars = -1;
            int ors = -1;
            int ands = -1;
            int aggConditions = -1;
            int comparisions = -1;
            int selectors = -1;
            int nots = -1;

            for (int i = 0; i < parameters.Count; i++) {
                if (parameters[i] is OpenParenthesisCommandParameter) { openPars = i; }
                if (parameters[i] is CloseParenthesisCommandParameter && closedPars < 0) { closedPars = i; }
                if (parameters[i] is AndCommandParameter) { ands = i; }
                if (parameters[i] is OrCommandParameter) { ors = i; }
                if (parameters[i] is NotCommandParameter) { nots = i; }
                if (parameters[i] is AggregationModeCommandParameter) { aggConditions = i; }
                if (parameters[i] is ComparisonCommandParameter) { comparisions = i; }
                if (parameters[i] is SelectorCommandParameter) { selectors = i; }
            }

            if (openPars >= 0) { //Parentheses first
                if (closedPars <= openPars + 1) throw new Exception("Mismatched Parenthesis!");
                List<CommandParameter> resolved = parameters.GetRange(openPars + 1, (closedPars - 1) - (openPars + 1));
                Variable converted = ConvertToVariable(resolved);
                parameters.RemoveRange(openPars, closedPars - openPars);
                parameters.Insert(openPars, new VariableCommandParameter(converted));
            } else if (closedPars >= 0) { throw new Exception("Mismatched Parentehsis!"); } else if (ands > 0) { //And Second
                Variable leftOperand = ConvertToVariable(parameters.GetRange(0, ands));
                Variable rightOperand = ConvertToVariable(parameters.GetRange(ands + 1, parameters.Count() - (ands + 1)));
                return new AndVariable(leftOperand, rightOperand);
            } else if (ors >= 0) {//Ors Third
                Variable leftOperand = ConvertToVariable(parameters.GetRange(0, ors));
                Variable rightOperand = ConvertToVariable(parameters.GetRange(ors + 1, parameters.Count() - (ors + 1)));
                return new OrVariable(leftOperand, rightOperand);
            } else if (aggConditions >= 0) {//Aggregate condition
                Print("Found Aggregator.  Parsing Aggregate Condition");
                Variable aggregateCondition = ParseAggregateCondition(parameters.GetRange(aggConditions, parameters.Count() - aggConditions));
                parameters.RemoveRange(aggConditions, parameters.Count() - aggConditions);
                parameters.Insert(aggConditions, new VariableCommandParameter(aggregateCondition));
            } else if (selectors >= 0 && selectors < comparisions) {
                Print("Parsing Implied Aggregate Condition");
                Variable aggregateCondition = ParseAggregateCondition(parameters.GetRange(selectors, parameters.Count() - selectors));
                parameters.RemoveRange(selectors, parameters.Count() - selectors);
                parameters.Insert(selectors, new VariableCommandParameter(aggregateCondition));
            } else if (nots >= 0) {
                Variable condition = new NotVariable(ConvertToVariable(parameters.GetRange(nots + 1, parameters.Count() - (nots + 1))));
                parameters.RemoveRange(nots, parameters.Count() - nots);
                parameters.Insert(nots, new VariableCommandParameter(condition));
            } else if (parameters.Count() == 1 && parameters[0] is VariableCommandParameter) {
                //Base Case
                return ((VariableCommandParameter)parameters[0]).Value;
            } else {
                Print("Ended");
                foreach (var p in parameters) {
                    Print("Parameter:  " + p);
                }
                throw new Exception("Unable to Variable from provided input");
            }
            //Recurse if not finished resolving
            return ConvertToVariable(parameters);
        }

        public static Variable ParseAggregateCondition(List<CommandParameter> commandParameters) {
            EntityProvider selector = null;//required
            ComparisonType? comparison = null;//Can be defaulted
            DirectionType? direction = null;//Optional
            Variable value = null;//requred? One of primitive or property must be set.
            PropertyType? property = null;//Can be defaulted
            AggregationMode? aggregation = null;
            bool inverseBlockCondition = false;

            int paramCount = 0;
            while (paramCount < commandParameters.Count) {
                CommandParameter param = commandParameters[paramCount];

                if (param is SelectorCommandParameter && selector == null) selector = ((SelectorCommandParameter)param).Value;
                else if (param is ComparisonCommandParameter) comparison = ((ComparisonCommandParameter)param).Value;
                else if (param is DirectionCommandParameter && direction == null) direction = ((DirectionCommandParameter)param).Value;
                else if (param is AggregationModeCommandParameter && aggregation == null) aggregation = ((AggregationModeCommandParameter)param).Value;
                else if (param is PropertyCommandParameter && property == null) property = ((PropertyCommandParameter)param).Value;
                else if (param is VariableCommandParameter && value == null) value = ((VariableCommandParameter)param).Value;
                else if (param is NotCommandParameter) { inverseBlockCondition = !inverseBlockCondition;
                } else throw new Exception("Unknown parameter passed to AggregateCondition");
                paramCount++;
            }

            if (selector == null) throw new Exception("All conditions must have a selector");
            if (value == null && property == null) throw new Exception("All conditions must have either a property or a value");

            Debug("Finished parsing condition params.  Count: " + paramCount);
            PrimitiveComparator comparator = new PrimitiveComparator(comparison.GetValueOrDefault(ComparisonType.EQUAL));
            BlockHandler handler = BlockHandlerRegistry.GetBlockHandler(selector.GetBlockType());

            if (value == null) {
                value = new StaticVariable(new BooleanPrimitive(true));
            }

            BlockCondition blockCondition;

            if (direction.HasValue) {
                blockCondition = new BlockDirectionPropertyCondition(handler, property, direction.Value, comparator, value);
            } else {
                blockCondition = new BlockPropertyCondition(handler, property, comparator, value);
            }

            Debug("Inverse Block Condition: " + inverseBlockCondition);

            if (inverseBlockCondition) blockCondition = new NotBlockCondition(blockCondition);
            AggregateConditionVariable condition = new AggregateConditionVariable(aggregation.GetValueOrDefault(AggregationMode.ALL),blockCondition, selector);

            return condition;
        }

        public class ConditionalSelectorProcessor : ParameterProcessor {
            protected override bool ShouldProcess(CommandParameter p) { return p is WithCommandParameter; }
            protected override void ConvertNext(List<CommandParameter> p, ref int i) {
                //TODO: Handle AggregationParameter and ignore ("any", "all")
                Debug("Attempting to parse Block Condition at index " + i);
                if (i == 0) throw new Exception("Block Condition must come after a selector");
                SelectorCommandParameter selector = p[i - 1] as SelectorCommandParameter;
                if (selector == null) throw new Exception("Block Condition must come after a selector");
                BlockHandler handler = BlockHandlerRegistry.GetBlockHandler(selector.Value.GetBlockType());
                parseNextBlockConditionTokens(p, i, handler);
                resolveNextBlockCondition(p, i, handler);
                BlockConditionCommandParameter blockCondition = p[i] as BlockConditionCommandParameter;
                if (blockCondition == null) throw new Exception("Unable to parse Block Condition Parameters");
                EntityProvider provider = new ConditionalEntityProvider(selector.Value, blockCondition.Value);
                p.RemoveRange(i - 1, 2);
                p.Insert(i - 1, new SelectorCommandParameter(provider));
            }

            public void parseNextBlockConditionTokens(List<CommandParameter> commandParameters, int index, BlockHandler handler) {
                Debug("Attempting to parse Condition Tokens at index " + index);
                ComparisonType? comparison = null;//Can be defaulted
                Variable value = null;//requred? One of primitive or property must be set.
                PropertyType? property = null;//Can be defaulted
                DirectionType? direction = null;//Optional

                bool inverseBlockCondition = false;

                if (commandParameters[index] is WithCommandParameter || commandParameters[index] is NotCommandParameter || commandParameters[index] is OpenParenthesisCommandParameter) {
                    Debug("Token is " + commandParameters[index].GetType() + ", continuing");
                    parseNextBlockConditionTokens(commandParameters, index + 1, handler);
                    return;
                }

                int paramCount = 0;
                while (index + paramCount < commandParameters.Count) {
                    CommandParameter param = commandParameters[index + paramCount];

                    if (param is ComparisonCommandParameter && comparison == null) comparison = ((ComparisonCommandParameter)param).Value;
                    else if (param is DirectionCommandParameter && direction == null) direction = ((DirectionCommandParameter)param).Value;
                    else if (param is PropertyCommandParameter && property == null) property = ((PropertyCommandParameter)param).Value;
                    else if (param is VariableCommandParameter && value == null) value = ((VariableCommandParameter)param).Value;
                    else if (param is NotCommandParameter) {
                        inverseBlockCondition = !inverseBlockCondition;
                    } else break;
                    paramCount++;
                }

                if (value == null && property == null) throw new Exception("All conditions must have either a property or a value");

                Debug("Finished parsing condition params.  Count: " + paramCount);
                PrimitiveComparator comparator = new PrimitiveComparator(comparison.GetValueOrDefault(ComparisonType.EQUAL));

                if (value == null) {
                    value = new StaticVariable(new BooleanPrimitive(true));
                }

                BlockCondition blockCondition;

                if (direction.HasValue) {
                    blockCondition = new BlockDirectionPropertyCondition(handler, property, direction.Value, comparator, value);
                } else {
                    blockCondition = new BlockPropertyCondition(handler, property, comparator, value);
                }

                Debug("Inverse Block Condition: " + inverseBlockCondition);

                if (inverseBlockCondition) blockCondition = new NotBlockCondition(blockCondition);

                Debug("Removing Range: " + index + ", Params: " + paramCount);

                commandParameters.RemoveRange(index, paramCount);
                commandParameters.Insert(index, new BlockConditionCommandParameter(blockCondition));

                if (commandParameters.Count == index + 1) return;

                //TODO: There's definitely bugs with this..
                int newIndex = index + 1;
                while (newIndex < commandParameters.Count - 1 && commandParameters[newIndex] is CloseParenthesisCommandParameter) { newIndex++; }
                if (commandParameters.Count == newIndex + 1) return;
                if (commandParameters[newIndex] is AndCommandParameter || commandParameters[newIndex] is OrCommandParameter || commandParameters[newIndex] is WithCommandParameter) {
                    Debug("Next Token After Processing Condition is " + commandParameters[newIndex].GetType() + ", continuing");
                    parseNextBlockConditionTokens(commandParameters, newIndex + 1, handler);
                }
                Debug("Finished Processing Condition Tokens at index: " + index);
            }

            public void resolveNextBlockCondition(List<CommandParameter> commandParameters, int index, BlockHandler handler) {
                Debug("Attempting to resolve Condition at index " + index);
                //TODO: Add ThatCommandParameter Support here for handling That w/ inversion ("without").  
                if (commandParameters[index] is WithCommandParameter) { // Handle "That"
                    bool inverseBlockCondition = ((WithCommandParameter)commandParameters[index]).inverseCondition;
                    commandParameters.RemoveAt(index); // Remove That
                    if (inverseBlockCondition) commandParameters.Insert(index, new NotCommandParameter());//If invert, insert not
                    resolveNextBlockCondition(commandParameters, index, handler);
                } else if (commandParameters[index] is NotCommandParameter) { // Handle Nots
                    commandParameters.RemoveAt(index); // Remove Not
                    BlockCondition notCondition = new NotBlockCondition(getNextBlockCondition(commandParameters, index, handler));
                    commandParameters.RemoveAt(index);
                    commandParameters.Insert(index, new BlockConditionCommandParameter(notCondition));
                } else if (commandParameters[index] is OpenParenthesisCommandParameter) { //Handle Parenthesis Next
                    resolveNextBlockCondition(commandParameters, index + 1, handler);
                    if (!(commandParameters[index + 2] is CloseParenthesisCommandParameter)) throw new Exception("Mismatched Parenthesis!");
                    commandParameters.RemoveAt(index); //Remove Open Parenthesis
                    commandParameters.RemoveAt(index + 1); //Remove Close Parenthesis
                }

                if (!(commandParameters[index] is BlockConditionCommandParameter)) throw new Exception("Invalid Token Inside Condition: " + commandParameters[index].GetType());
                BlockCondition conditionA = ((BlockConditionCommandParameter)commandParameters[index]).Value;
                while (commandParameters.Count > index + 2) //Look for And/Or + more conditions
                {
                    if (commandParameters[index + 1] is AndCommandParameter) {//Handle Ands before Ors
                        Debug("Found And Parameter at index: " + (index + 1));
                        AndBlockCondition andCondition = new AndBlockCondition(conditionA, getNextBlockCondition(commandParameters, index + 2, handler));
                        commandParameters.RemoveRange(index, 3);
                        commandParameters.Insert(index, new BlockConditionCommandParameter(andCondition));
                    } else if (commandParameters[index + 1] is OrCommandParameter) {
                        Debug("Found Or Parameter at index: " + (index + 1));
                        resolveNextBlockCondition(commandParameters, index + 2, handler);
                        OrBlockCondition orCondition = new OrBlockCondition(conditionA, getNextBlockCondition(commandParameters, index + 2, handler));
                        commandParameters.RemoveRange(index, 3);
                        commandParameters.Insert(index, new BlockConditionCommandParameter(orCondition));
                    } else if (commandParameters[index + 1] is WithCommandParameter) { // That x that y = And
                        Debug("Found That Parameter at index, assuming And: " + (index + 1));
                        AndBlockCondition andCondition = new AndBlockCondition(conditionA, getNextBlockCondition(commandParameters, index + 1, handler));
                        commandParameters.RemoveRange(index, 2);
                        commandParameters.Insert(index, new BlockConditionCommandParameter(andCondition));
                    } else {
                        break;
                    }
                }
            }

            private BlockCondition getNextBlockCondition(List<CommandParameter> commandParameters, int index, BlockHandler handler) {
                if (!(commandParameters[index] is BlockConditionCommandParameter)) ///Resolve if not a simple condition (more parentheses, for example)
                {
                    Debug("In getNextCondition.  Next Condition at index " + index + " is not simple, resolving");
                    resolveNextBlockCondition(commandParameters, index, handler);
                }
                return ((BlockConditionCommandParameter)commandParameters[index]).Value;
            }
        }
    }
}
