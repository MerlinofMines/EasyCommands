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
        public class PistonBlockHandler : BlockHandler<IMyPistonBase>
        {
            public override DirectionType GetDefaultDirection()
            {
                return DirectionType.UP;
            }

            public override NumericPropertyType GetDefaultNumericProperty(DirectionType direction)
            {
                return NumericPropertyType.HEIGHT;
            }

            protected override List<NumericPropertyGetter<IMyPistonBase>> GetNumericPropertyGetters()
            {
                return new List<NumericPropertyGetter<IMyPistonBase>>()
                {
                    new PistonHeightGetter(),
                    new PistonSpeedGetter(),
                };
            }

            protected override List<NumericPropertySetter<IMyPistonBase>> GetNumericPropertySetters()
            {
                return new List<NumericPropertySetter<IMyPistonBase>>()
                {
                    new PistonHeightSetter(),
                    new PistonSpeedSetter(),
                };
            }
        }

        public class PistonHeightGetter : NumericPropertyGetter<IMyPistonBase>
        {
            public PistonHeightGetter() : base(NumericPropertyType.HEIGHT){}

            public override float GetPropertyValue(IMyPistonBase block)
            {
                return block.CurrentPosition;
            }

        }

        public class PistonHeightSetter : NumericPropertySetter<IMyPistonBase>
        {
            public PistonHeightSetter() : base(NumericPropertyType.HEIGHT) { }

            public override void IncrementPropertyValue(IMyPistonBase block, float deltaValue)
            {
                IncrementPropertyValue(block, DirectionType.UP, deltaValue);
            }

            public override void IncrementPropertyValue(IMyPistonBase block, DirectionType direction, float deltaValue)
            {
                if (direction == DirectionType.UP) extendPistonToValue(block, block.CurrentPosition + deltaValue);
                if (direction == DirectionType.DOWN) extendPistonToValue(block, block.CurrentPosition - deltaValue);
            }

            public override void MovePropertyValue(IMyPistonBase block, DirectionType direction)
            {
                if (direction == DirectionType.UP) block.Extend();
                if (direction == DirectionType.DOWN) block.Retract();
            }

            public override void ReversePropertyValue(IMyPistonBase block)
            {
                block.Reverse();
            }

            public override void SetPropertyValue(IMyPistonBase block, DirectionType DirectionType, float value)
            {
                extendPistonToValue(block, value);
            }

            public override void SetPropertyValue(IMyPistonBase block, float value)
            {
                extendPistonToValue(block, value);
            }
        }

        public class PistonSpeedGetter : NumericPropertyGetter<IMyPistonBase>
        {
            public PistonSpeedGetter() : base(NumericPropertyType.SPEED){}

            public override float GetPropertyValue(IMyPistonBase block)
            {
                return block.Velocity;
            }
        }

        public class PistonSpeedSetter : NumericPropertySetter<IMyPistonBase>
        {
            public PistonSpeedSetter() : base(NumericPropertyType.SPEED) { }

            public override void IncrementPropertyValue(IMyPistonBase block, float deltaValue)
            {
                IncrementPropertyValue(block, DirectionType.UP, deltaValue);
            }

            public override void IncrementPropertyValue(IMyPistonBase block, DirectionType direction, float deltaValue)
            {
                if (direction == DirectionType.UP) block.Velocity += deltaValue;
                if (direction == DirectionType.DOWN) block.Velocity -= deltaValue;
            }

            public override void MovePropertyValue(IMyPistonBase block, DirectionType direction)
            {
                IncrementPropertyValue(block, direction, 1);
            }

            public override void ReversePropertyValue(IMyPistonBase block)
            {
                block.Velocity *= -1;
            }

            public override void SetPropertyValue(IMyPistonBase block, DirectionType DirectionType, float value)
            {
                block.Velocity = value;
            }

            public override void SetPropertyValue(IMyPistonBase block, float value)
            {
                block.Velocity = value;
            }
        }

        static void extendPistonToValue(IMyPistonBase piston, float value)
        {
            if (piston.CurrentPosition < value)
            {
                piston.SetValue("UpperLimit", value);
                piston.Extend();
            }
            else
            {
                piston.SetValue("LowerLimit", value);
                piston.Retract();
            }
        }
    }
}
