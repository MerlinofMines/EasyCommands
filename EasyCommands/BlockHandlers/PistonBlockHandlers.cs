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
        public class PistonBlockHandler : NumericBaseBlockHandler<IMyPistonBase>
        {
            public PistonBlockHandler()
            {
            }

            public override DirectionType GetDefaultDirection()
            {
                return DirectionType.UP;
            }

            public override NumericPropertyType GetDefaultNumericProperty(DirectionType direction)
            {
                return NumericPropertyType.HEIGHT;
            }

            protected override List<NumericPropertyHandler<IMyPistonBase>> GetNumericPropertyHandlers()
            {
                return new List<NumericPropertyHandler<IMyPistonBase>>()
                {
                    new PistonHeightHandler(),
                    new PistonSpeedHandler()
                };
            }
        }

        public class PistonHeightHandler : NumericPropertyHandler<IMyPistonBase>
        {
            public NumericPropertyType GetHandledPropertyType()
            {
                return NumericPropertyType.HEIGHT;
            }

            public float GetPropertyValue(IMyPistonBase block)
            {
                return block.CurrentPosition;
            }

            public void IncrementPropertyValue(IMyPistonBase block, float deltaValue)
            {
                IncrementPropertyValue(block, DirectionType.UP, deltaValue);
            }

            public void IncrementPropertyValue(IMyPistonBase block, DirectionType direction, float deltaValue)
            {
                if (direction == DirectionType.UP) extendPistonToValue(block, block.CurrentPosition + deltaValue);
                if (direction == DirectionType.DOWN) extendPistonToValue(block, block.CurrentPosition - deltaValue);
            }

            public void MovePropertyValue(IMyPistonBase block, DirectionType direction)
            {
                if (direction == DirectionType.UP) block.Extend();
                if (direction == DirectionType.DOWN) block.Retract();
            }

            public void ReversePropertyValue(IMyPistonBase block)
            {
                block.Reverse();
            }

            public void SetPropertyValue(IMyPistonBase block, DirectionType DirectionType, float value)
            {
                extendPistonToValue(block, value);
            }

            public void SetPropertyValue(IMyPistonBase block, float value)
            {
                extendPistonToValue(block, value);
            }
        }

        public class PistonSpeedHandler : NumericPropertyHandler<IMyPistonBase>
        {
            public NumericPropertyType GetHandledPropertyType()
            {
                return NumericPropertyType.SPEED;
            }

            public float GetPropertyValue(IMyPistonBase block)
            {
                return block.Velocity;
            }

            public void IncrementPropertyValue(IMyPistonBase block, float deltaValue)
            {
                IncrementPropertyValue(block, DirectionType.UP, deltaValue);
            }

            public void IncrementPropertyValue(IMyPistonBase block, DirectionType direction, float deltaValue)
            {
                if (direction == DirectionType.UP) block.Velocity += deltaValue;
                if (direction == DirectionType.DOWN) block.Velocity -= deltaValue;
            }

            public void MovePropertyValue(IMyPistonBase block, DirectionType direction)
            {
                IncrementPropertyValue(block, direction, 1);
            }

            public void ReversePropertyValue(IMyPistonBase block)
            {
                block.Velocity *= -1;
            }

            public void SetPropertyValue(IMyPistonBase block, DirectionType DirectionType, float value)
            {
                block.Velocity = value;
            }

            public void SetPropertyValue(IMyPistonBase block, float value)
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
