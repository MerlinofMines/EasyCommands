using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public class GyroscopeBlockHandler : FunctionalBlockHandler<IMyGyro> {
            public GyroscopeBlockHandler() {
                AddBooleanHandler(PropertyType.AUTO, b => !b.GyroOverride, (b, v) => b.GyroOverride = !v);
                AddBooleanHandler(PropertyType.OVERRIDE, b => b.GyroOverride, (b, v) => b.GyroOverride = v);
                AddNumericHandler(PropertyType.RANGE, b => b.GyroPower, (b, v) => b.GyroPower = v, 0.1f);
                var rollHandler = new SimpleNumericDirectionPropertyHandler<IMyGyro>(GetOverrideInput, SetOverrideInput, DirectionType.UP);
                AddPropertyHandler(PropertyType.ROLL_INPUT, rollHandler);
                AddPropertyHandler(PropertyType.MOVE_INPUT, rollHandler);
            }

            float GetOverrideInput(IMyGyro gyro, DirectionType direction) {
                switch(direction) {
                    case DirectionType.UP:
                        return gyro.Pitch;
                    case DirectionType.DOWN:
                        return -gyro.Pitch;
                    case DirectionType.LEFT:
                        return -gyro.Yaw;
                    case DirectionType.RIGHT:
                        return gyro.Yaw;
                    case DirectionType.CLOCKWISE:
                        return gyro.Roll;
                    case DirectionType.COUNTERCLOCKWISE:
                        return -gyro.Roll;
                    default:
                        throw new Exception("Unknown direction: " + direction);
                }
            }

            void SetOverrideInput(IMyGyro gyro, DirectionType direction, float value) {
                switch (direction) {
                    case DirectionType.UP:
                        gyro.Pitch = value; break;
                    case DirectionType.DOWN:
                        gyro.Pitch = -value; break;
                    case DirectionType.LEFT:
                        gyro.Yaw = -value; break;
                    case DirectionType.RIGHT:
                        gyro.Yaw = value; break;
                    case DirectionType.CLOCKWISE:
                        gyro.Roll = value; break;
                    case DirectionType.COUNTERCLOCKWISE:
                        gyro.Roll = -value; break;
                    default:
                        throw new Exception("Unknown direction: " + direction);
                }
            }
        }
    }
}
