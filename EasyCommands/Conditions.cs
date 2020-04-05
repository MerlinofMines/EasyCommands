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
    partial class Program
    {
        public interface Condition
        {
            bool evaluate(MyGridProgram program);
        }

        public enum AggregationMode
        {
            ANY,
            ALL,
            NONE
        }

        /*
        public class RememberedCondition : Condition
        {
            public bool rememberedValue;
            public bool remembered = false;
            public Condition condition;
            public RememberedCondition(Condition condition){this.condition = condition;}
            public bool evaluate() {
                if (!remembered) { rememberedValue = condition.evaluate(); remembered = true;} return rememberedValue;
            }
        }
        */

        public class AggregateCondition<T> : Condition where T : class, IMyFunctionalBlock
        {
            AggregationMode aggregationMode;
            BlockCondition<T> blockCondition;
            IEntityProvider<T> entityProvider;

            public AggregateCondition(AggregationMode aggregationMode, BlockCondition<T> blockCondition, IEntityProvider<T> entityProvider)
            {
                this.aggregationMode = aggregationMode;
                this.blockCondition = blockCondition;
                this.entityProvider = entityProvider;
            }

            public bool evaluate(MyGridProgram program)
            {
                List<T> blocks = entityProvider.GetEntities(program);

                int matches = blocks.Count(block => blockCondition.evaluate(block));

                switch(aggregationMode)
                {
                    case AggregationMode.ALL: return matches == blocks.Count;
                    case AggregationMode.ANY: return matches > 0;
                    case AggregationMode.NONE: return matches == 0;
                    default: throw new Exception("Unsupported Aggregation Mode: " + aggregationMode);
                }
            }
        }

        public interface BlockCondition<T> where T : class, IMyFunctionalBlock
        {
            bool evaluate(T block);
        }

        public class NumericPropertyBlockCondition<T> : BlockCondition<T> where T : class, IMyFunctionalBlock
        {
            BlockHandler<T> blockHandler;
            NumericPropertyType property;
            Comparator<float> comparator;
            float comparisonValue;

            public NumericPropertyBlockCondition(BlockHandler<T> blockHandler, NumericPropertyType property, Comparator<float> comparator, float comparisonValue)
            {
                this.blockHandler = blockHandler;
                this.property = property;
                this.comparator = comparator;
                this.comparisonValue = comparisonValue;
            }

            public bool evaluate(T block)
            {
                return comparator.compare(blockHandler.GetNumericPropertyValue(block, property), comparisonValue);
            }
        }

        public interface Comparator<T>
        {
            bool compare(T a, T b);
        }

        public class GreaterThanNumericComparator : Comparator<float>
        {
            public bool compare(float a, float b)
            {
                return a > b;
            }
        }
    }
}
