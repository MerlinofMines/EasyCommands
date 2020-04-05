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

            public static void process(List<CommandParameter> commandParameters)
            {
                foreach (ParameterProcessor parser in parameterProcessors)
                {
                    parser.process(commandParameters);
                }
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
                int index = 0;
                while (index < commandParameters.Count)
                {
                    if (commandParameters[index] is GroupCommandParameter || commandParameters[index] is StringCommandParameter || commandParameters[index] is BlockTypeCommandParameter)
                    {
                        convertNextSelector(commandParameters, index);
                    }
                    index++;
                }
            }

            private void convertNextSelector(List<CommandParameter> commandParameters, int index)
            {
                bool isGroup = false;
                String selector = null;
                BlockTypeCommandParameter blockType = null;
                int paramCount = 0;

                while (index + paramCount <commandParameters.Count)
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

                    if (p.Count == 0) return; //Apparently not a Selector

                    blockType = ((BlockTypeCommandParameter)p[0]);
                    if (p.Count > 1) isGroup = true;
                }

                commandParameters.RemoveRange(index, paramCount);
                commandParameters.Add(new SelectorCommandParameter(blockType.GetBlockType(), isGroup, selector));
            }
        }

        public class ActionProcessor : ParameterProcessor
        {
            public void process(List<CommandParameter> commandParameters)
            {
                int index = 0;
                while (index < commandParameters.Count)
                {
                    if (isPartOfAction(commandParameters[index]))
                    {
                        convertNextActionParameter(commandParameters, index);
                    }
                    index++;
                }
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
            public void process(List<CommandParameter> commandParameters)
            {
                int index = 0;
                while (index < commandParameters.Count)
                {
                    if (commandParameters[index] is ConditionalCommand)
                    {
                        index++;
                        convertNextConditionParameter(commandParameters, index);
                    }
                    index++;
                }
            }

            public void convertNextConditionParameter(List<CommandParameter> commandParameters, int index)
            {
                /*
                SelectorCommandParameter selector = null;//required
                ComparisonCommandParameter comparisonValue = null;//requred
                CommandParameter comparator = null;//Can be defaulted
                CommandParameter property = null;//Can be defaulted
                AggregationModeCommandParameter aggregation = null;
                */

                //TODO: Resolve the above.

                //SelectorCommandParameter
                //BlockType -> BlockHandler
                //Selector -> EntityProvider

                //AggregationMode (default to all)
                //Comparator


                //AggregationMode aggregationMode; (can be inferred)
                //BlockCondition<T> blockCondition;
                // NumericBlockHandler<T> blockHandler; (blockType).
                // NumericPropertyType property; (optional, can be inferred from BlockType).
                // Comparator comparator; (optional)
                // float comparisonValue; (required.  Need to know if Numeric, String, or Bool)
                //IEntityProvider<T> entityProvider; (String selector) (group optional) 
            }

            private Condition CreateCondition(SelectorCommandParameter selector, ComparisonCommandParameter comparison, CommandParameter property, CommandParameter comparisonValue, AggregationModeCommandParameter aggregationMode)
            {
                return null;
            }

            private Condition CreateBlockCondition<T>(SelectorCommandParameter selector, ComparisonCommandParameter comparison, CommandParameter property, CommandParameter comparisonValue, AggregationModeCommandParameter aggregationMode)
                where T : class, IMyFunctionalBlock
            {
                return null;
            }
        }
    }
}
