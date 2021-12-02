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

                var rollHandler = DirectionalTypedHandler(Direction.UP,
                    TypeHandler(NumericHandler(b => b.Pitch, (b,v) => b.Pitch = v), Direction.UP),
                    TypeHandler(NumericHandler(b => -b.Pitch, (b, v) => b.Pitch = -v), Direction.DOWN),
                    TypeHandler(NumericHandler(b => -b.Yaw, (b, v) => b.Yaw = -v), Direction.LEFT),
                    TypeHandler(NumericHandler(b => b.Yaw, (b, v) => b.Yaw = v), Direction.RIGHT),
                    TypeHandler(NumericHandler(b => b.Roll, (b, v) => b.Roll = v), Direction.CLOCKWISE),
                    TypeHandler(NumericHandler(b => -b.Roll, (b, v) => b.Roll = -v), Direction.COUNTERCLOCKWISE));

                AddPropertyHandler(Property.ROLL_INPUT, rollHandler);
                AddPropertyHandler(Property.INPUT, rollHandler);
            }
        }
    }
}
