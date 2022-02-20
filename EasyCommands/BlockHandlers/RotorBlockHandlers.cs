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
        static Func<IMyMotorStator, bool> IsHinge = b => b.BlockDefinition.SubtypeId.Contains("Hinge");

        public class RotorBlockHandler : FunctionalBlockHandler<IMyMotorStator> {
            Func<IMyMotorStator, bool> blockFilter;
            public RotorBlockHandler(Func<IMyMotorStator, bool> filter) {
                AddNumericHandler(Property.VELOCITY, (b) => b.TargetVelocityRPM, (b, v) => b.TargetVelocityRPM = v, 1);
                AddNumericHandler(Property.LEVEL, (b) => b.Displacement, (b, v) => b.Displacement = v, 0.1f);
                AddBooleanHandler(Property.CONNECTED, b => b.IsAttached, (b, v) => { if (v) b.Attach(); else b.Detach(); });
                AddBooleanHandler(Property.LOCKED, b => b.RotorLock, (b, v) => b.RotorLock = v);
                AddNumericHandler(Property.STRENGTH, b => b.Torque, (b,v) => b.Torque = v, 1000);

                var upperLimitHandler = NumericHandler(b => b.UpperLimitDeg, (b, v) => b.UpperLimitDeg = v, 10);
                AddPropertyHandler(upperLimitHandler, Property.RANGE);
                AddPropertyHandler(upperLimitHandler, Property.RANGE, Property.UP);
                AddPropertyHandler(upperLimitHandler, Property.RANGE, Property.CLOCKWISE);

                var lowerLimitHandler = NumericHandler(b => b.UpperLimitDeg, (b, v) => b.UpperLimitDeg = v, 10);
                AddPropertyHandler(lowerLimitHandler, Property.RANGE, Property.DOWN);
                AddPropertyHandler(lowerLimitHandler, Property.RANGE, Property.COUNTERCLOCKWISE);

                GetTypedProperty<IMyMotorStator, float> GetAngle = b => b.Angle * (float)(180 / Math.PI);

                AddNumericHandler(Property.ANGLE, GetAngle, RotateToValue);
                AddBooleanHandler(Property.REVERSE, b => b.TargetVelocityRPM < 0, (b, v) => b.TargetVelocityRPM = v ? -b.TargetVelocityRPM : 0);

                var clockwiseAngleHandler = NumericHandler(GetAngle, (b, v) => RotateToValue(b, v, Property.CLOCKWISE));
                AddPropertyHandler(clockwiseAngleHandler,
                    Property.ANGLE, Property.CLOCKWISE);
                AddPropertyHandler(clockwiseAngleHandler,
                    Property.ANGLE, Property.UP);

                var counterClockwiseAngleHandler = NumericHandler(GetAngle, (b, v) => RotateToValue(b, v, Property.COUNTERCLOCKWISE));
                AddPropertyHandler(counterClockwiseAngleHandler,
                    Property.ANGLE, Property.COUNTERCLOCKWISE);
                AddPropertyHandler(counterClockwiseAngleHandler,
                    Property.ANGLE, Property.DOWN);

                var clockwiseRotateHandler = BooleanHandler(b => b.TargetVelocityRPM > 0, (b, v) => b.TargetVelocityRPM = v ? Math.Abs(b.TargetVelocityRPM) : 0);
                AddReturnHandlers(Property.CLOCKWISE, Return.BOOLEAN,
                    TypeHandler(clockwiseRotateHandler, Return.BOOLEAN),
                    TypeHandler(NumericHandler(b => b.TargetVelocityRPM, (b, v) => RotateToValue(b, v, Property.CLOCKWISE)), Return.NUMERIC));

                var counterClockwiseRotateHandler = BooleanHandler(b => b.TargetVelocityRPM < 0, (b, v) => b.TargetVelocityRPM = v ? -Math.Abs(b.TargetVelocityRPM) : 0);
                AddReturnHandlers(Property.CLOCKWISE, Return.BOOLEAN,
                    TypeHandler(clockwiseRotateHandler, Return.BOOLEAN),
                    TypeHandler(NumericHandler(b => b.TargetVelocityRPM, (b, v) => RotateToValue(b, v, Property.COUNTERCLOCKWISE)), Return.NUMERIC));

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.ANGLE;
                defaultProperty = Property.CLOCKWISE;
                blockFilter = filter;
            }

            public override IEnumerable<IMyMotorStator> SelectBlocksByType<U>(List<U> blocks, Func<U, bool> selector = null) =>
                base.SelectBlocksByType(blocks, selector).Where(blockFilter);
        }

        static void RotateToValue(IMyMotorStator rotor, float value) {
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

        static void RotateToValue(IMyMotorStator rotor, float angle, Property direction) {
            float value = GetCorrectedAngle(angle);
            float currentAngle = rotor.Angle * (180 / (float)Math.PI);

            switch (direction) {
                case Property.CLOCKWISE:
                    if (value < currentAngle) value = GetCorrectedAngle(value + 360);
                    rotor.UpperLimitDeg = value;
                    rotor.TargetVelocityRPM = Math.Abs(rotor.TargetVelocityRPM);
                    break;
                case Property.COUNTERCLOCKWISE:
                    if (value > currentAngle) value = GetCorrectedAngle(value - 360);
                    rotor.LowerLimitDeg = value;
                    rotor.TargetVelocityRPM = -Math.Abs(rotor.TargetVelocityRPM);
                    break;
                default:
                    RotateToValue(rotor, angle);
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
