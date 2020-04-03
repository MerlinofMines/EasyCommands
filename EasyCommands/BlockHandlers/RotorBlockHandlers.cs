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
        public class RotorBlockHandler : NumericBaseBlockHandler<IMyMotorStator>
        {
            public override DirectionType GetDefaultDirection()
            {
                return DirectionType.CLOCKWISE;
            }

            public override NumericPropertyType GetDefaultNumericProperty(DirectionType direction)
            {
                switch(direction)
                {
                    case DirectionType.UP:
                    case DirectionType.DOWN:
                        return NumericPropertyType.HEIGHT;
                    default:
                        return NumericPropertyType.ANGLE;
                }
            }

            protected override List<NumericPropertyHandler<IMyMotorStator>> GetNumericPropertyHandlers()
            {
                return new List<NumericPropertyHandler<IMyMotorStator>>()
                {
                    new RotorAngleHandler(),
                    new RotorSpeedHandler()
                };
            }
        }

        public class RotorAngleHandler : NumericPropertyHandler<IMyMotorStator>
        {
            public NumericPropertyType GetHandledPropertyType()
            {
                return NumericPropertyType.ANGLE;
            }

            public float GetPropertyValue(IMyMotorStator block)
            {
                return block.Angle;
            }

            public void IncrementPropertyValue(IMyMotorStator block, float deltaValue)
            {
                IncrementPropertyValue(block, DirectionType.CLOCKWISE, deltaValue);
            }

            public void IncrementPropertyValue(IMyMotorStator block, DirectionType direction, float deltaValue)
            {
                if (direction == DirectionType.CLOCKWISE) rotateToValue(block, block.Angle + deltaValue);
                if (direction == DirectionType.COUNTERCLOCKWISE) rotateToValue(block, block.Angle - deltaValue);
            }

            public void MovePropertyValue(IMyMotorStator block, DirectionType direction)
            {
                if (direction == DirectionType.CLOCKWISE) block.TargetVelocityRPM = Math.Abs(block.TargetVelocityRPM);
                if (direction == DirectionType.COUNTERCLOCKWISE) block.TargetVelocityRPM = -Math.Abs(block.TargetVelocityRPM);
            }

            public void ReversePropertyValue(IMyMotorStator block)
            {
                block.TargetVelocityRPM *= -1;
            }

            public void SetPropertyValue(IMyMotorStator block, DirectionType DirectionType, float value)
            {
                //TODO: Add Direction Support
                rotateToValue(block, value);
            }

            public void SetPropertyValue(IMyMotorStator block, float value)
            {
                rotateToValue(block, value);
            }
        }

        public class RotorSpeedHandler : NumericPropertyHandler<IMyMotorStator>
        {
            public NumericPropertyType GetHandledPropertyType()
            {
                return NumericPropertyType.SPEED;
            }

            public float GetPropertyValue(IMyMotorStator block)
            {
                return block.TargetVelocityRPM;
            }

            public void IncrementPropertyValue(IMyMotorStator block, float deltaValue)
            {
                IncrementPropertyValue(block, DirectionType.UP, deltaValue);
            }

            public void IncrementPropertyValue(IMyMotorStator block, DirectionType direction, float deltaValue)
            {
                if (direction == DirectionType.UP) block.TargetVelocityRPM += deltaValue;
                if (direction == DirectionType.DOWN) block.TargetVelocityRPM -= deltaValue;
            }

            public void MovePropertyValue(IMyMotorStator block, DirectionType direction)
            {
                IncrementPropertyValue(block, direction, 1);
            }

            public void ReversePropertyValue(IMyMotorStator block)
            {
                block.TargetVelocityRPM *= -1;
            }

            public void SetPropertyValue(IMyMotorStator block, DirectionType DirectionType, float value)
            {
                block.TargetVelocityRPM = value;
            }

            public void SetPropertyValue(IMyMotorStator block, float value)
            {
                block.TargetVelocityRPM = value;
            }
        }

        //TODO: Directions may become important.  Below needs a lot of work
        static void rotateToValue(IMyMotorStator rotor, float value)
        {
            float newValue = value;

            if (newValue > 360) newValue %= 360;

            if (newValue < -360)
            {
                newValue = -((-newValue) % 360);
            }

            //TODO: We might find that in some cases, it's faster to go the other way.

            if (rotor.Angle < value)
            {
                rotor.UpperLimitDeg = newValue;
                rotor.TargetVelocityRPM = Math.Abs(rotor.TargetVelocityRPM);
            }
            else
            {
                rotor.LowerLimitDeg = newValue;
                rotor.TargetVelocityRPM = -Math.Abs(rotor.TargetVelocityRPM);
            }
        }
    }
}
