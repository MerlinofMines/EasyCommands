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

        public interface CommandParameter
        {

        }

        public abstract class ValueCommandParameter<T> : CommandParameter
        {
            private T value;

            public ValueCommandParameter(T value)
            {
                this.value = value;
            }

            public T GetValue()
            {
                return value;
            }
        }

        public class StringCommandParameter : ValueCommandParameter<String>
        {
            public StringCommandParameter(string value) : base(value)
            {
            }
        }

        public class NumericCommandParameter : ValueCommandParameter<float>
        {
            public NumericCommandParameter(float value) : base(value)
            {
            }
        }

        public class BooleanCommandParameter : ValueCommandParameter<bool>
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

        public class BooleanPropertyCommandParameter : ValueCommandParameter<BooleanPropertyType>
        {
            public BooleanPropertyCommandParameter(BooleanPropertyType value) : base(value)
            {
            }
        }

        public class StringPropertyCommandParameter : ValueCommandParameter<StringPropertyType>
        {
            public StringPropertyCommandParameter(StringPropertyType value) : base(value)
            {
            }
        }

        public class NumericPropertyCommandParameter : ValueCommandParameter<NumericPropertyType>
        {
            public NumericPropertyCommandParameter(NumericPropertyType value) : base(value)
            {
            }
        }

        public class GroupCommandParameter : CommandParameter
        {

        }

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
