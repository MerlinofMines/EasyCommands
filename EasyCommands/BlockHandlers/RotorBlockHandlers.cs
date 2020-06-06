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
        public class RotorBlockHandler : BlockHandler<IMyMotorStator> {
            public RotorBlockHandler() {
                defaultDirection = DirectionType.CLOCKWISE;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.HEIGHT);
                defaultNumericProperties.Add(DirectionType.DOWN, NumericPropertyType.HEIGHT);
                defaultNumericProperties.Add(DirectionType.CLOCKWISE, NumericPropertyType.ANGLE);
                defaultNumericProperties.Add(DirectionType.COUNTERCLOCKWISE, NumericPropertyType.ANGLE);

                numericPropertyGetters.Add(NumericPropertyType.ANGLE, block => (float)(block.Angle * (180 / Math.PI)));
                numericPropertyGetters.Add(NumericPropertyType.VELOCITY, block => block.TargetVelocityRPM);

                numericPropertySetters.Add(NumericPropertyType.ANGLE, new RotorAngleSetter());
                numericPropertySetters.Add(NumericPropertyType.VELOCITY, new RotorVelocitySetter());
            }
        }

        public class RotorAngleSetter : NumericPropertySetter<IMyMotorStator> {
            public RotorAngleSetter() {
                Set = rotateToValue;
                SetDirection = (b, d, v) => rotateToValue(b, v);
                IncrementDirection = (b, d, v) => {
                    if (d == DirectionType.CLOCKWISE) rotateToValue(b, b.Angle + v);
                    if (d == DirectionType.COUNTERCLOCKWISE) rotateToValue(b, b.Angle - v);
                };
                Increment = (b, v) => IncrementDirection(b, DirectionType.CLOCKWISE, v);
                Move = (b, d) => {
                    if (d == DirectionType.CLOCKWISE) b.TargetVelocityRPM = Math.Abs(b.TargetVelocityRPM);
                    if (d == DirectionType.COUNTERCLOCKWISE) b.TargetVelocityRPM = -Math.Abs(b.TargetVelocityRPM);
                };
                Reverse = (b) => b.TargetVelocityRPM *= -1;
            }
        }

        public class RotorVelocitySetter : NumericPropertySetter<IMyMotorStator> {
            public RotorVelocitySetter() {
                Set = (b, v) => b.TargetVelocityRPM = v;
                SetDirection = (b, d, v) => b.TargetVelocityRPM = v;
                IncrementDirection = (b, d, v) => {
                    if (d == DirectionType.UP) b.TargetVelocityRPM += v;
                    if (d == DirectionType.DOWN) b.TargetVelocityRPM -= v;
                };
                Increment = (b, v) => IncrementDirection(b, DirectionType.UP, v);
                Move = (b, d) => {
                    if (d == DirectionType.UP) Increment(b, 1);
                    if (d == DirectionType.DOWN) Increment(b, -1);
                };
                Reverse = (b) => b.TargetVelocityRPM *= -1;
            }
        }

        //TODO: Directions may become important.  Below needs a lot of work
        static void rotateToValue(IMyMotorStator rotor, float value) {
            float newValue = value;

            if (newValue > 360) newValue %= 360;

            if (newValue < -360) {
                newValue = -((-newValue) % 360);
            }

            //TODO: We might find that in some cases, it's faster to go the other way.
            if (rotor.Angle * (180 / Math.PI) < value) {
                rotor.UpperLimitDeg = newValue;
                rotor.TargetVelocityRPM = Math.Abs(rotor.TargetVelocityRPM);
            } else {
                rotor.LowerLimitDeg = newValue;
                rotor.TargetVelocityRPM = -Math.Abs(rotor.TargetVelocityRPM);
            }
        }
    }
}
