﻿using Sandbox.Game.EntityComponents;
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

        public class NotCondition : Condition
        {
            Condition condition;

            public NotCondition(Condition condition)
            {
                this.condition = condition;
            }

            public bool evaluate(MyGridProgram program)
            {
                return !condition.evaluate(program);
            }
        }

        public class AggregateCondition : Condition
        {
            AggregationMode aggregationMode;
            BlockCondition blockCondition;
            IEntityProvider entityProvider;

            public AggregateCondition(AggregationMode aggregationMode, BlockCondition blockCondition, IEntityProvider entityProvider)
            {
                this.aggregationMode = aggregationMode;
                this.blockCondition = blockCondition;
                this.entityProvider = entityProvider;
            }

            public bool evaluate(MyGridProgram program)
            {
                List<IMyFunctionalBlock> blocks = entityProvider.GetEntities(program);

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

        public interface BlockCondition
        {
            bool evaluate(IMyFunctionalBlock block);
        }

        public class NotBlockCondition : BlockCondition
        {
            BlockCondition blockCondition;

            public NotBlockCondition(BlockCondition blockCondition)
            {
                this.blockCondition = blockCondition;
            }

            public bool evaluate(IMyFunctionalBlock block)
            {
                return !blockCondition.evaluate(block);
            }
        }

        public abstract class BlockCondition<T,U> : BlockCondition
        {
            protected BlockHandler blockHandler;
            protected T property;
            protected Comparator<U> comparator;
            protected U comparisonValue;

            protected BlockCondition(BlockHandler blockHandler, T property, Comparator<U> comparator, U comparisonValue)
            {
                this.blockHandler = blockHandler;
                this.property = property;
                this.comparator = comparator;
                this.comparisonValue = comparisonValue;
            }

            public abstract bool evaluate(IMyFunctionalBlock block);
        }

        public class BooleanBlockCondition : BlockCondition<BooleanPropertyType, bool>
        {
            public BooleanBlockCondition(BlockHandler blockHandler, BooleanPropertyType property, Comparator<bool> comparator, bool comparisonValue) : base(blockHandler, property, comparator, comparisonValue){}
            public override bool evaluate(IMyFunctionalBlock block) {return comparator.compare(blockHandler.GetBooleanPropertyValue(block, property), comparisonValue);}
        }

        public class StringBlockCondition : BlockCondition<StringPropertyType, String>
        {
            public StringBlockCondition(BlockHandler blockHandler, StringPropertyType property, Comparator<String> comparator, String comparisonValue) : base(blockHandler, property, comparator, comparisonValue) { }
            public override bool evaluate(IMyFunctionalBlock block) { return comparator.compare(blockHandler.GetStringPropertyValue(block, property), comparisonValue); }
        }

        public class NumericBlockCondition : BlockCondition<NumericPropertyType, float>
        {
            public NumericBlockCondition(BlockHandler blockHandler, NumericPropertyType property, Comparator<float> comparator, float comparisonValue) : base(blockHandler, property, comparator, comparisonValue) { }
            public override bool evaluate(IMyFunctionalBlock block) { return comparator.compare(blockHandler.GetNumericPropertyValue(block, property), comparisonValue); }
        }

        public abstract class Comparator<T>
        {
            protected ComparisonType comparisonType;

            protected Comparator(ComparisonType comparisonType)
            {
                this.comparisonType = comparisonType;
            }

            public abstract bool compare(T a, T b);
        }

        public class BooleanComparator : Comparator<bool>
        {
            public BooleanComparator(ComparisonType comparisonType) : base(comparisonType) { }

            public override bool compare(bool a, bool b)
            {
                if (ComparisonType.EQUAL == comparisonType) return a == b;
                else throw new Exception("Boolean Comparisons Only Support Equality");
            }
        }

        public class StringComparator : Comparator<String>
        {
            public StringComparator(ComparisonType comparisonType) : base(comparisonType) { }

            public override bool compare(string a, string b)
            {
                if (ComparisonType.EQUAL == comparisonType) return a == b;
                else throw new Exception("Boolean Comparisons Only Support Equality");
                //TODO: More Comparison Types?? 
            }
        }

        public class NumericComparator : Comparator<float>
        {
            public NumericComparator(ComparisonType comparisonType) : base(comparisonType){}

            public override bool compare(float a, float b)
            {
                switch (comparisonType)
                {
                    case ComparisonType.GREATER: return a > b;
                    case ComparisonType.GREATER_OR_EQUAL: return a >= b;
                    case ComparisonType.EQUAL: return a == b;
                    case ComparisonType.LESS_OR_EQUAL: return a <= b;
                    case ComparisonType.LESS: return a < b;
                    default: throw new Exception("Unsupported Comparison Type");
                }
            }
        }
    }
}