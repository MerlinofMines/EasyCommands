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

namespace IngameScript {
    partial class Program {
        public class RotorBlockHandler : FunctionalBlockHandler<IMyMotorStator> {
            public RotorBlockHandler() {
                AddPropertyHandler(PropertyType.ANGLE, new RotorAngleHandler());
                AddNumericHandler(PropertyType.VELOCITY, (b) => b.TargetVelocityRPM, (b, v) => b.TargetVelocityRPM = v, 1);
                AddNumericHandler(PropertyType.HEIGHT, (b) => b.Displacement, (b, v) => b.Displacement = v, 0.1f);
                defaultPropertiesByPrimitive[PrimitiveType.NUMERIC] = PropertyType.ANGLE;
                defaultPropertiesByDirection.Add(DirectionType.UP, PropertyType.HEIGHT);
                defaultPropertiesByDirection.Add(DirectionType.DOWN, PropertyType.HEIGHT);
                defaultPropertiesByDirection.Add(DirectionType.CLOCKWISE, PropertyType.ANGLE);
                defaultPropertiesByDirection.Add(DirectionType.COUNTERCLOCKWISE, PropertyType.ANGLE);
                defaultDirection = DirectionType.CLOCKWISE;
            }
        }

        public class RotorAngleHandler : PropertyHandler<IMyMotorStator> {
            public RotorAngleHandler() {
                Get = block => new NumberPrimitive(block.Angle * (float)(180 / Math.PI));
                GetDirection = (b, d) => Get(b);
                Set = RotateToValue;
                SetDirection = (b, d, v) => RotateToValue(b, v, d);
                IncrementDirection = (b, d, v) => {
                    if (d == DirectionType.CLOCKWISE || d == DirectionType.UP) RotateToValue(b, Get(b).Plus(v), d);
                    if (d == DirectionType.COUNTERCLOCKWISE || d == DirectionType.DOWN) RotateToValue(b, Get(b).Minus(v), d);
                };
                Increment = (b, v) => IncrementDirection(b, DirectionType.CLOCKWISE, v);
                Move = (b, d) => {
                    if (d == DirectionType.CLOCKWISE) b.TargetVelocityRPM = Math.Abs(b.TargetVelocityRPM);
                    if (d == DirectionType.COUNTERCLOCKWISE) b.TargetVelocityRPM = -Math.Abs(b.TargetVelocityRPM);
                };
                Reverse = (b) => b.TargetVelocityRPM *= -1;
            }
        }

        static void RotateToValue(IMyMotorStator rotor, Primitive primitive) {
            if(primitive.GetPrimitiveType()!=PrimitiveType.NUMERIC) {
                throw new Exception("Cannot rotate rotor to non-numeric value: " + primitive);
            }

            float value = (float)primitive.GetValue();
            float newValue = GetCorrectedAngle(value);

            //TODO: We might find that in some cases, it's faster to go the other way.
            if (rotor.Angle * (180 / Math.PI) < value) {
                rotor.UpperLimitDeg = newValue;
                rotor.TargetVelocityRPM = Math.Abs(rotor.TargetVelocityRPM);
            } else {
                rotor.LowerLimitDeg = newValue;
                rotor.TargetVelocityRPM = -Math.Abs(rotor.TargetVelocityRPM);
            }
        }

        static void RotateToValue(IMyMotorStator rotor, Primitive primitive, DirectionType direction) {
            if (primitive.GetPrimitiveType() != PrimitiveType.NUMERIC) {
                throw new Exception("Cannot rotate rotor to non-numeric value: " + primitive);
            }

            float value = GetCorrectedAngle((float)primitive.GetValue());
            float currentAngle = rotor.Angle * (180 / (float)Math.PI);

            switch (direction) {
                case DirectionType.CLOCKWISE:
                    if (value < currentAngle) value = GetCorrectedAngle(value + 360);
                    rotor.UpperLimitDeg = value;
                    rotor.TargetVelocityRPM = Math.Abs(rotor.TargetVelocityRPM);
                    break;
                case DirectionType.COUNTERCLOCKWISE:
                    if (value > currentAngle) value = GetCorrectedAngle(value - 360);
                    rotor.LowerLimitDeg = value;
                    rotor.TargetVelocityRPM = -Math.Abs(rotor.TargetVelocityRPM);
                    break;
                default:
                    RotateToValue(rotor, primitive);
                    break;
            }
        }

        static float GetCorrectedAngle(float angle) {
            float newAngle = angle;
            if (newAngle > 360) newAngle %= 360;
            else if (newAngle < -360) {
                newAngle = -((-newAngle) % 360);
            }
            return newAngle;
        }
    }
}
