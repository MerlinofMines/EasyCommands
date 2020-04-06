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
                  new ConditionProcessor(),
                  new ActionProcessor(),
            };

            public static void process(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                program.Echo("Start Parameter Processor ");
                foreach (ParameterProcessor parser in parameterProcessors)
                {
                    program.Echo("Pre Processed Parameters:");
                    commandParameters.ForEach(param => program.Echo("Type: " + param.GetType()));

                    parser.process(program, commandParameters);

                    program.Echo("Post Processed Parameters:");
                    commandParameters.ForEach(param => program.Echo("Type: " + param.GetType()));
                }
                program.Echo("End Parameter Processor ");
            }
        }

        public interface ParameterProcessor
        {
            void process(MyGridProgram program, List<CommandParameter> commandParameters);
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
            public void process(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                program.Echo("Start Selector Processor");
                int index = 0;
                while (index < commandParameters.Count)
                {
                    if (commandParameters[index] is GroupCommandParameter || commandParameters[index] is StringCommandParameter || commandParameters[index] is BlockTypeCommandParameter)
                    {
                        program.Echo("Found Possible Selector at Index: " + index);
                        index+=convertNextSelector(commandParameters, index);
                    } else {
                        index++;
                    }
                }
                program.Echo("End Selector Processor");
            }

            private int convertNextSelector(List<CommandParameter> commandParameters, int index)
            {
                bool isGroup = false;
                String selector = null;
                BlockTypeCommandParameter blockType = null;
                int paramCount = 0;

                while (index + paramCount < commandParameters.Count)
                {
                    CommandParameter param = commandParameters[index + paramCount];

                    if (param is GroupCommandParameter) isGroup = true;
                    else if (param is StringCommandParameter && selector == null) selector = ((StringCommandParameter)param).GetValue();
                    else if (param is BlockTypeCommandParameter && blockType == null) blockType = ((BlockTypeCommandParameter)param);
                    else break;
                    paramCount++;
                }

                if (selector == null) throw new Exception("All selectors must have a string identifier");

                if (blockType == null) {
                    List<String> tokens = parseTokens(selector);
                    List<CommandParameter> p = new List<CommandParameter>();
                    foreach (String token in tokens)
                    {
                        if (new BlockTypeParser().process(token, p)) break;
                    }

                    if (p.Count == 0) return paramCount; //Apparently not a Selector

                    blockType = ((BlockTypeCommandParameter)p[0]);
                    if (p.Count > 1) isGroup = true;
                }

                commandParameters.RemoveRange(index, paramCount);
                commandParameters.Insert(index, new SelectorCommandParameter(blockType.GetBlockType(), isGroup, selector));
                return 1;
            }
        }

        public class ActionProcessor : ParameterProcessor
        {
            public void process(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                program.Echo("Start Action Processor");
                int index = 0;
                while (index < commandParameters.Count)
                {
                    if (isPartOfAction(commandParameters[index]))
                    {
                        convertNextActionParameter(commandParameters, index);
                    }
                    index++;
                }
                program.Echo("End Action Processor");
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
                    parameter is UnitCommandParameter;
            }
        }

        public class ConditionProcessor : ParameterProcessor
        {
            public void process(MyGridProgram program, List<CommandParameter> commandParameters)
            {
                program.Echo("Start Condition Processor");
                int index = 0;
                while (index < commandParameters.Count)
                {
                    if (commandParameters[index] is IfCommandParameter)
                    {
                        index++;
                        convertNextConditionParameter(program, commandParameters, index);
                    }
                    index++;
                }
                program.Echo("End Condition Processor");
            }

            public void convertNextConditionParameter(MyGridProgram program, List<CommandParameter> commandParameters, int index)
            {
                program.Echo("Attempting to parse Condition at index " + index);
                SelectorCommandParameter selector = null;//required
                ComparisonCommandParameter comparator = null;//Can be defaulted
                PrimitiveCommandParameter value = null;//requred? One of primitive or property must be set.
                PropertyCommandParameter property = null;//Can be defaulted
                AggregationModeCommandParameter aggregation = null;
                bool inverseAggregation = false;
                bool inverseBlockCondition = false;

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

                program.Echo("Finished parsing condition params.  Count: " + paramCount);
                AggregationMode aggregationMode = (aggregation == null) ? AggregationMode.ALL : aggregation.GetValue();
                ComparisonType comparison = (comparator == null) ? ComparisonType.EQUAL : comparator.GetValue();
                BlockHandler handler = BlockHandlerRegistry.GetBlockHandler(selector.blockType);
                IEntityProvider provider = new SelectorEntityProvider(selector);

                BlockCondition blockCondition;
                if (value is BooleanCommandParameter || property is BooleanPropertyCommandParameter)
                {
                    program.Echo("Boolean Command");
                    BooleanPropertyType boolProperty = handler.GetDefaultBooleanProperty();
                    if (property != null) boolProperty = ((BooleanPropertyCommandParameter)property).GetValue();
                    bool boolValue = true;  if (value != null) boolValue = ((BooleanCommandParameter)value).GetValue();
                    blockCondition = new BooleanBlockCondition(handler, boolProperty, new BooleanComparator(comparison), boolValue);
                } else if (value is StringCommandParameter || property is StringPropertyCommandParameter)
                {
                    program.Echo("String Command");
                    StringPropertyType stringProperty = handler.GetDefaultStringProperty();
                    if (property != null) stringProperty = ((StringPropertyCommandParameter)property).GetValue();
                    if (value == null) throw new Exception("String Comparison Value Cannot Be Left Blank");
                    String stringValue = ((StringCommandParameter)value).GetValue();
                    blockCondition = new StringBlockCondition(handler, stringProperty, new StringComparator(comparison), stringValue);
                } else if (value is NumericCommandParameter || property is NumericPropertyCommandParameter)
                {
                    program.Echo("Numeric Command");
                    NumericPropertyType numericProperty = handler.GetDefaultNumericProperty(handler.GetDefaultDirection());
                    if (property != null) numericProperty = ((NumericPropertyCommandParameter)property).GetValue();
                    if (value == null) throw new Exception("Numeric Comparison Value Cannot Be Left Blank");
                    float numericValue = ((NumericCommandParameter)value).GetValue();
                    blockCondition = new NumericBlockCondition(handler, numericProperty, new NumericComparator(comparison), numericValue);
                } else
                {
                    throw new Exception("Unsupported Condition Parameters");
                }

                if (inverseBlockCondition) blockCondition = new NotBlockCondition(blockCondition);
                Condition condition = new AggregateCondition(aggregationMode, blockCondition, new SelectorEntityProvider(selector));
                if (inverseAggregation) condition = new NotCondition(condition);

                program.Echo("Removing Range: " + index + ", Params: " + paramCount);
                commandParameters.RemoveRange(index, paramCount);
                commandParameters.Insert(index, new ConditionCommandParameter(condition));
            }
        }
    }
}
