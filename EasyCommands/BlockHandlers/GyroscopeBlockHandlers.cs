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
                AddBooleanHandler(Property.AUTO, b => !b.GyroOverride, (b, v) => b.GyroOverride = !v);
                AddBooleanHandler(Property.OVERRIDE, b => b.GyroOverride, (b, v) => b.GyroOverride = v);
                AddNumericHandler(Property.RANGE, b => b.GyroPower, (b, v) => b.GyroPower = v, 0.1f);
                var rollHandler = new SimpleNumericDirectionPropertyHandler<IMyGyro>(GetOverrideInput, SetOverrideInput, Direction.UP);
                AddPropertyHandler(Property.ROLL_INPUT, rollHandler);
                AddPropertyHandler(Property.MOVE_INPUT, rollHandler);
            }

            float GetOverrideInput(IMyGyro gyro, Direction direction) {
                switch(direction) {
                    case Direction.UP:
                        return gyro.Pitch;
                    case Direction.DOWN:
                        return -gyro.Pitch;
                    case Direction.LEFT:
                        return -gyro.Yaw;
                    case Direction.RIGHT:
                        return gyro.Yaw;
                    case Direction.CLOCKWISE:
                        return gyro.Roll;
                    case Direction.COUNTERCLOCKWISE:
                        return -gyro.Roll;
                    default:
                        throw new Exception("Unknown direction: " + direction);
                }
            }

            void SetOverrideInput(IMyGyro gyro, Direction direction, float value) {
                switch (direction) {
                    case Direction.UP:
                        gyro.Pitch = value; break;
                    case Direction.DOWN:
                        gyro.Pitch = -value; break;
                    case Direction.LEFT:
                        gyro.Yaw = -value; break;
                    case Direction.RIGHT:
                        gyro.Yaw = value; break;
                    case Direction.CLOCKWISE:
                        gyro.Roll = value; break;
                    case Direction.COUNTERCLOCKWISE:
                        gyro.Roll = -value; break;
                    default:
                        throw new Exception("Unknown direction: " + direction);
                }
            }
        }
    }
}
