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
        public class GyroscopeBlockHandler<T> : FunctionalBlockHandler<T> where T : class, IMyGyro {
            public GyroscopeBlockHandler() {
                var powerHandler = NumericHandler(b => b.GyroPower, (b, v) => b.GyroPower = v, 0.1f);
                AddPropertyHandler(Property.RANGE, powerHandler);
                AddPropertyHandler(Property.POWER, powerHandler);

                AddBooleanHandler(Property.AUTO, b => !b.GyroOverride, (b, v) => b.GyroOverride = !v);

                var overrideHandler = DirectionalTypedHandler(Direction.NONE,
                        TypeHandler(ReturnTypedHandler(Return.VECTOR,
                            TypeHandler(VectorHandler(b => Vector(-b.Pitch * RadiansPerSecToRPM, b.Yaw * RadiansPerSecToRPM, b.Roll * RadiansPerSecToRPM), (b, v) => {
                                b.Pitch = RPMToRadiansPerSec * (float)-v.X;
                                b.Yaw = RPMToRadiansPerSec * (float)v.Y;
                                b.Roll = RPMToRadiansPerSec * (float)v.Z;
                            }), Return.VECTOR),
                            TypeHandler(BooleanHandler(b => b.GyroOverride, (b, v) => b.GyroOverride = v), Return.BOOLEAN))
                        , Direction.NONE),
                        TypeHandler(NumericHandler(b => RadiansPerSecToRPM * -b.Pitch, (b, v) => b.Pitch = RPMToRadiansPerSec * -v, 5), Direction.UP),
                        TypeHandler(NumericHandler(b => RadiansPerSecToRPM * b.Pitch, (b, v) => b.Pitch = RPMToRadiansPerSec * v, 5), Direction.DOWN),
                        TypeHandler(NumericHandler(b => RadiansPerSecToRPM * -b.Yaw, (b, v) => b.Yaw = RPMToRadiansPerSec * -v, 5), Direction.LEFT),
                        TypeHandler(NumericHandler(b => RadiansPerSecToRPM * b.Yaw, (b, v) => b.Yaw = RPMToRadiansPerSec * v, 5), Direction.RIGHT),
                        TypeHandler(NumericHandler(b => RadiansPerSecToRPM * b.Roll, (b, v) => b.Roll = RPMToRadiansPerSec * v, 5), Direction.CLOCKWISE),
                        TypeHandler(NumericHandler(b => RadiansPerSecToRPM * -b.Roll, (b, v) => b.Roll = RPMToRadiansPerSec * -v, 5), Direction.COUNTERCLOCKWISE));

                AddPropertyHandler(Property.OVERRIDE, overrideHandler);
                AddPropertyHandler(Property.ROLL_INPUT, overrideHandler);
                AddPropertyHandler(Property.INPUT, overrideHandler);

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.POWER;
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.OVERRIDE;
            }
        }
    }
}
