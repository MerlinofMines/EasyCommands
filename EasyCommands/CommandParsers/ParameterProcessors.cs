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

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public static class ParameterProcessorRegistry
        {
            private static List<ParameterProcessor> parameterProcessors = new List<ParameterProcessor>
            {
                  new SelectorProcessor(),
                  new RunArgumentProcessor(),
                  new ConditionProcessor(),
                  new ActionProcessor(),
            };

            public static void process(List<CommandParameter> commandParameters)
            {
                Print("Start Parameter Processor ");
                foreach (ParameterProcessor parser in parameterProcessors)
                {
                    Print("Pre Processed Parameters:");
                    commandParameters.ForEach(param => Print("Type: " + param.GetType()));

                    parser.process(commandParameters);

                    Print("Post Processed Parameters:");
                    commandParameters.ForEach(param => Print("Type: " + param.GetType()));
                }
                Print("End Parameter Processor ");
            }
        }

        public interface ParameterProcessor
        {
            void process(List<CommandParameter> commandParameters);
        }

        //Taken shamelessly from https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        public static List<String> parseTokens(String commandString)
        {
            return commandString.Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                                    : new string[] { element })  // Keep the entire item
                .SelectMany(element => element)
                .Select(element => element.ToLower())
                .ToList();
        }

        public class SelectorProcessor : ParameterProcessor
        {
            public void process(List<CommandParameter> commandParameters)
            {
                Print("Start Selector Processor");
                int index = 0;
                while (index < commandParameters.Count)
                {
                    if (commandParameters[index] is GroupCommandParameter || commandParameters[index] is StringCommandParameter || commandParameters[index] is BlockTypeCommandParameter)
                    {
                        Print("Found Possible Selector at Index: " + index);
                        index+=convertNextSelector(commandParameters, index);
                    } else {
                        index++;
                    }
                }
                Print("End Selector Processor");
            }

            private int convertNextSelector(List<CommandParameter> commandParameters, int index)
            {
                bool isGroup = false;
                StringCommandParameter selector = null;
                BlockTypeCommandParameter blockType = null;
                int paramCount = 0;

                while (index + paramCount < commandParameters.Count)
                {
                    CommandParameter param = commandParameters[index + paramCount];

                    if (param is GroupCommandParameter) isGroup = true;
                    else if (param is StringCommandParameter && selector == null) selector = ((StringCommandParameter)param);
                    else if (param is BlockTypeCommandParameter && blockType == null) blockType = ((BlockTypeCommandParameter)param);
                    else break;
                    paramCount++;
                }

                if (selector == null) throw new Exception("All selectors must have a string identifier");

                if (blockType == null) {
                    int blockTypeIndex = selector.SubTokens.FindLastIndex(p => p is BlockTypeCommandParameter);//Use Last Block Type
                    if (blockTypeIndex<0) return paramCount; //Apparently not a Selector
                    if (selector.SubTokens.Exists(p => p is GroupCommandParameter)) isGroup = true;
                    blockType = ((BlockTypeCommandParameter)selector.SubTokens[blockTypeIndex]);
                }

                Print("Converted String at index: " + index + " to SelectorCommandParamter");
                commandParameters.RemoveRange(index, paramCount);
                commandParameters.Insert(index, new SelectorCommandParameter(blockType.GetBlockType(), isGroup, selector.Value));
                return 1;
            }
        }

        public class RunArgumentProcessor : ParameterProcessor
        {
            public void process(List<CommandParameter> commandParameters)
            {
                Print("Start Run Argument Processor");
                for (int i = 0; i < commandParameters.Count; i++)
                {
                    if (commandParameters[i] is StringCommandParameter)
                    {
                        StringCommandParameter param = (StringCommandParameter)commandParameters[i];
                        if (param.SubTokens.Count == 0 || !(param.SubTokens[0] is StringPropertyCommandParameter)) continue;
                        if (((StringPropertyCommandParameter)param.SubTokens[0]).Value != StringPropertyType.RUN) continue;

                        Print("Found Run Keyword!");
                        List<String> values = parseTokens(param.Value);
                        commandParameters.RemoveAt(i);
                        values.RemoveAt(0);
                        Print("Arguments: (" + String.Join(" ", values) + ")");
                        commandParameters.Insert(i, new StringPropertyCommandParameter(StringPropertyType.RUN));
                        commandParameters.Insert(i + 1, new StringCommandParameter(String.Join(" ", values)));
                        i++;
                    }
                }
                Print("End Run Argument Processor");
            }
        }

        public class ActionProcessor : ParameterProcessor
        {
            public void process(List<CommandParameter> commandParameters)
            {
                Print("Start Action Processor");
                int index = 0;
                while (index < commandParameters.Count)
                {
                    if (isPartOfAction(commandParameters[index]))
                    {
                        convertNextActionParameter(commandParameters, index);
                    }
                    index++;
                }
                Print("End Action Processor");
            }

            private void convertNextActionParameter(List<CommandParameter> commandParameters, int index)
            {
                int paramCount = 0;
                while (index + paramCount < commandParameters.Count)
                {
                    CommandParameter param = commandParameters[index + paramCount];

                    if (!isPartOfAction(param)) break;
                    paramCount++;
                }

                List<CommandParameter> actionParameters = commandParameters.GetRange(index, paramCount);

                ActionCommandParameter actionParameter = new ActionCommandParameter(actionParameters);

                commandParameters.RemoveRange(index, paramCount);
                commandParameters.Insert(index, actionParameter);
            }

            private bool isPartOfAction(CommandParameter parameter)
            {
                return parameter is SelectorCommandParameter ||
                    parameter is DirectionCommandParameter ||
                    parameter is NumericCommandParameter ||
                    parameter is StringCommandParameter ||
                    parameter is BooleanCommandParameter ||
                    parameter is NumericPropertyCommandParameter ||
                    parameter is BooleanPropertyCommandParameter ||
                    parameter is StringPropertyCommandParameter ||
                    parameter is ReverseCommandParameter ||
                    parameter is RelativeCommandParameter ||
                    parameter is WaitCommandParameter ||
                    parameter is UnitCommandParameter ||
                    parameter is ControlCommandParameter;
            }
        }

        public class ConditionProcessor : ParameterProcessor
        {
            public void process(List<CommandParameter> commandParameters)
            {
                Print("Start Condition Processor");
                int index = 0;
                while (index < commandParameters.Count)
                {
                    if (commandParameters[index] is IfCommandParameter)
                    {
                        index++;
                        convertNextConditionParameter(commandParameters, index);
                    }
                    index++;
                }
                Print("End Condition Processor");
            }

            public void convertNextConditionParameter(List<CommandParameter> commandParameters, int index)
            {
                Print("Attempting to parse Condition at index " + index);
                parseNextConditionTokens(commandParameters, index);
                resolveNextCondition(commandParameters, index);
            }

            public void parseNextConditionTokens(List<CommandParameter> commandParameters, int index) {
                Print("Attempting to parse Condition Tokens at index " + index);
                SelectorCommandParameter selector = null;//required
                ComparisonCommandParameter comparator = null;//Can be defaulted
                PrimitiveCommandParameter value = null;//requred? One of primitive or property must be set.
                PropertyCommandParameter property = null;//Can be defaulted
                AggregationModeCommandParameter aggregation = null;
                bool inverseAggregation = false;
                bool inverseBlockCondition = false;

                if (commandParameters[index] is NotCommandParameter || commandParameters[index] is OpenParenthesisCommandParameter)
                {
                    Print("Token is " + commandParameters[index].GetType() + ", continuing");
                    parseNextConditionTokens(commandParameters, index+1);
                    return;
                }

                int paramCount = 0;
                while (index + paramCount < commandParameters.Count)
                {
                    CommandParameter param = commandParameters[index + paramCount];

                    if (param is SelectorCommandParameter && selector == null) selector = (SelectorCommandParameter) param;
                    else if (param is ComparisonCommandParameter && comparator == null) comparator = (ComparisonCommandParameter)param;
                    else if (param is AggregationModeCommandParameter && aggregation == null) aggregation = (AggregationModeCommandParameter)param;
                    else if (param is PropertyCommandParameter && property == null) property = (PropertyCommandParameter) param;
                    else if (param is PrimitiveCommandParameter && value == null) value = ((PrimitiveCommandParameter)param);
                    else if (param is NotCommandParameter)
                    {
                        if (aggregation!=null || selector!=null) {
                            inverseBlockCondition = !inverseBlockCondition;
                        } else {
                            inverseAggregation = !inverseAggregation;
                        }
                    }
                    else break;
                    paramCount++;
                }

                if (selector == null) throw new Exception("All conditions must have a selector");
                if (value == null && property == null) throw new Exception("All conditions must have either a property or a value");

                Print("Finished parsing condition params.  Count: " + paramCount);
                AggregationMode aggregationMode = (aggregation == null) ? AggregationMode.ALL : aggregation.Value;
                ComparisonType comparison = (comparator == null) ? ComparisonType.EQUAL : comparator.Value;
                BlockHandler handler = BlockHandlerRegistry.GetBlockHandler(selector.blockType);
                IEntityProvider provider = new SelectorEntityProvider(selector);

                BlockCondition blockCondition;
                if (value is BooleanCommandParameter || property is BooleanPropertyCommandParameter)
                {
                    Print("Boolean Command");
                    BooleanPropertyType boolProperty = handler.GetDefaultBooleanProperty();
                    if (property != null) boolProperty = ((BooleanPropertyCommandParameter)property).Value;
                    bool boolValue = true;  if (value != null) boolValue = ((BooleanCommandParameter)value).Value;
                    blockCondition = new BooleanBlockCondition(handler, boolProperty, new BooleanComparator(comparison), boolValue);
                } else if (value is StringCommandParameter || property is StringPropertyCommandParameter)
                {
                    Print("String Command");
                    StringPropertyType stringProperty = handler.GetDefaultStringProperty();
                    if (property != null) stringProperty = ((StringPropertyCommandParameter)property).Value;
                    if (value == null) throw new Exception("String Comparison Value Cannot Be Left Blank");
                    String stringValue = ((StringCommandParameter)value).Value;
                    blockCondition = new StringBlockCondition(handler, stringProperty, new StringComparator(comparison), stringValue);
                } else if (value is NumericCommandParameter || property is NumericPropertyCommandParameter)
                {
                    Print("Numeric Command");
                    NumericPropertyType numericProperty = handler.GetDefaultNumericProperty(handler.GetDefaultDirection());
                    if (property != null) numericProperty = ((NumericPropertyCommandParameter)property).Value;
                    if (value == null) throw new Exception("Numeric Comparison Value Cannot Be Left Blank");
                    float numericValue = ((NumericCommandParameter)value).Value;
                    blockCondition = new NumericBlockCondition(handler, numericProperty, new NumericComparator(comparison), numericValue);
                } else
                {
                    throw new Exception("Unsupported Condition Parameters");
                }

                Print("Inverse Block Condition: " + inverseBlockCondition);
                Print("Inverse Aggregation: " + inverseAggregation);

                if (inverseBlockCondition) blockCondition = new NotBlockCondition(blockCondition);
                Condition condition = new AggregateCondition(aggregationMode, blockCondition, new SelectorEntityProvider(selector));
                if (inverseAggregation) condition = new NotCondition(condition);

                Print("Removing Range: " + index + ", Params: " + paramCount);
                commandParameters.RemoveRange(index, paramCount);
                commandParameters.Insert(index, new ConditionCommandParameter(condition));

                if (commandParameters.Count == index+1) return;

                //TODO: There's definitely bugs with this..
                int newIndex = index + 1;
                while (commandParameters[newIndex] is CloseParenthesisCommandParameter) { newIndex++; }
                if (commandParameters.Count == newIndex + 1) return;
                if (commandParameters[newIndex] is AndCommandParameter || commandParameters[newIndex] is OrCommandParameter)
                {
                    Print("Next Token After Processing Condition is " + commandParameters[newIndex].GetType() + ", continuing");
                    parseNextConditionTokens(commandParameters, newIndex + 1);
                }
                Print("Finished Processing Condition Tokens at index: " + index);
            }

            public void resolveNextCondition(List<CommandParameter> commandParameters, int index)
            {
                Print("Attempting to resolve Condition at index " + index);
                if (commandParameters[index] is NotCommandParameter) // Handle Nots first
                {
                    commandParameters.RemoveAt(index); // Remove Not
                    Condition notCondition = new NotCondition(getNextCondition(commandParameters, index));
                    commandParameters.RemoveAt(index);
                    commandParameters.Insert(index, new ConditionCommandParameter(notCondition));
                }
                else if (commandParameters[index] is OpenParenthesisCommandParameter) { //Handle Parenthesis Next
                    resolveNextCondition(commandParameters, index + 1);
                    if (!(commandParameters[index + 2] is CloseParenthesisCommandParameter)) throw new Exception("Mismatched Parenthesis!");
                    commandParameters.RemoveAt(index); //Remove Open Parenthesis
                    commandParameters.RemoveAt(index + 1); //Remove Close Parenthesis
                }

                if (!(commandParameters[index] is ConditionCommandParameter)) throw new Exception("Invalid Token Inside Condition: " + commandParameters[index].GetType());
                Condition conditionA = ((ConditionCommandParameter)commandParameters[index]).Value;
                while (commandParameters.Count > index+2) //Look for And/Or + more conditions
                {
                    if (commandParameters[index + 1] is AndCommandParameter) {//Handle Ands before Ors
                        Print("Found And Parameter at index: " + (index + 1));
                        AndCondition andCondition = new AndCondition(conditionA, getNextCondition(commandParameters, index+2));
                        commandParameters.RemoveRange(index,3);
                        commandParameters.Insert(index, new ConditionCommandParameter(andCondition));
                    }
                    else if (commandParameters[index + 1] is OrCommandParameter)
                    {
                        Print("Found Or Parameter at index: " + (index + 1));
                        resolveNextCondition(commandParameters, index + 2);
                        OrCondition orCondition = new OrCondition(conditionA, getNextCondition(commandParameters, index+2));
                        commandParameters.RemoveRange(index,3);
                        commandParameters.Insert(index, new ConditionCommandParameter(orCondition));
                    } else
                    {
                        break;
                    }
                }
            }

            private Condition getNextCondition(List<CommandParameter> commandParameters, int index)
            {
                if (!(commandParameters[index] is ConditionCommandParameter)) ///Resolve if not a simple condition (more parentheses, for example)
                {
                    Print("In getNextCondition.  Next Condition at index " + index + " is not simple, resolving");
                    resolveNextCondition(commandParameters, index);
                }
                return ((ConditionCommandParameter)commandParameters[index]).Value;
            }
        }
    }
}
