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

        public interface CommandParameter {}
        public interface PropertyCommandParameter {}
        public interface PrimitiveCommandParameter {}

        public abstract class ValueCommandParameter<T> : CommandParameter
        {
            public T Value;
            public ValueCommandParameter(T value) {this.Value = value;}
        }

        public class StringCommandParameter : ValueCommandParameter<String>, PrimitiveCommandParameter
        {
            public List<CommandParameter> SubTokens = new List<CommandParameter>();
            public StringCommandParameter(string value, params CommandParameter[] SubTokens) : base(value)
            {
                this.SubTokens = SubTokens.ToList();
            }
        }

        public class NumericCommandParameter : ValueCommandParameter<float>, PrimitiveCommandParameter
        {
            public NumericCommandParameter(float value) : base(value)
            {
            }
        }

        public class BooleanCommandParameter : ValueCommandParameter<bool>, PrimitiveCommandParameter
        {
            public BooleanCommandParameter(bool value) : base(value)
            {
            }
        }

        public class DirectionCommandParameter : ValueCommandParameter<DirectionType>
        {
            public DirectionCommandParameter(DirectionType value) : base(value)
            {
            }
        }

        public class BooleanPropertyCommandParameter : ValueCommandParameter<BooleanPropertyType>, PropertyCommandParameter
        {
            public BooleanPropertyCommandParameter(BooleanPropertyType value) : base(value)
            {
            }
        }

        public class StringPropertyCommandParameter : ValueCommandParameter<StringPropertyType>, PropertyCommandParameter
        {
            public StringPropertyCommandParameter(StringPropertyType value) : base(value)
            {
            }
        }

        public class NumericPropertyCommandParameter : ValueCommandParameter<NumericPropertyType>, PropertyCommandParameter
        {
            public NumericPropertyCommandParameter(NumericPropertyType value) : base(value)
            {
            }
        }

        public class GroupCommandParameter : CommandParameter { }
        public class AsyncCommandParameter : CommandParameter { }

        public class ControlCommandParameter : ValueCommandParameter<ControlType>
        {
            public ControlCommandParameter(ControlType value) : base(value)
            {
            }
        }

        public class IfCommandParameter : CommandParameter 
        {
            public bool inverseCondition;
            public bool alwaysEvaluate;
            public bool swapCommands;

            public IfCommandParameter(bool inverseCondition, bool alwaysEvaluate, bool swapCommands)
            {
                this.inverseCondition = inverseCondition;
                this.alwaysEvaluate = alwaysEvaluate;
                this.swapCommands = swapCommands;
            }
        }

        public class ConditionCommandParameter : ValueCommandParameter<Condition>
        {
            public ConditionCommandParameter(Condition value) : base(value) {}
        }

        public class CommandReferenceParameter : ValueCommandParameter<Command>
        {
            public CommandReferenceParameter(Command value) : base(value) {}
        }

        public class NotCommandParameter : CommandParameter { }
        public class AndCommandParameter : CommandParameter { }
        public class OrCommandParameter : CommandParameter { }
        public class OpenParenthesisCommandParameter : CommandParameter { }
        public class CloseParenthesisCommandParameter : CommandParameter { }

        public class AggregationModeCommandParameter : ValueCommandParameter<AggregationMode>
        {
            public AggregationModeCommandParameter(AggregationMode value) : base(value)
            {
            }
        }

        public class ComparisonCommandParameter : ValueCommandParameter<ComparisonType>
        {
            public ComparisonCommandParameter(ComparisonType value) : base(value)
            {
            }
        }

        public class ActionCommandParameter : ValueCommandParameter<List<CommandParameter>>
        {
            public ActionCommandParameter(List<CommandParameter> value) : base(value)
            {
            }
        }

        public class SelectorCommandParameter : CommandParameter
        {
            public BlockType blockType;
            public bool isGroup;
            public String selector;

            public SelectorCommandParameter(BlockType blockType, bool isGroup, string selector)
            {
                this.blockType = blockType;
                this.isGroup = isGroup;
                this.selector = selector;
            }
        }


        public class ElseCommandParameter : CommandParameter { }

        public class BlockTypeCommandParameter : CommandParameter
        {
            private BlockType blockType;

            public BlockTypeCommandParameter(BlockType blockType)
            {
                this.blockType = blockType;
            }

            public BlockType GetBlockType()
            {
                return blockType;
            }
        }

        public class ReverseCommandParameter : CommandParameter
        {

        }

        public class RelativeCommandParameter : CommandParameter
        {

        }

        public class WaitCommandParameter : CommandParameter
        {

        }

        public class UnitCommandParameter : CommandParameter
        {
            UnitType unit;

            public UnitCommandParameter(UnitType unit)
            {
                this.unit = unit;
            }

            public UnitType GetUnit()
            {
                return unit;
            }
        }
    }
}
