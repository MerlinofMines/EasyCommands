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
        public class JumpDriveBlockHandler : FunctionalBlockHandler<IMyJumpDrive> {
            public JumpDriveBlockHandler() {
                AddNumericHandler(Property.POWER, b => b.CurrentStoredPower);
                AddNumericHandler(Property.RATIO, b => b.CurrentStoredPower / b.MaxStoredPower);
                var readyHandler = BooleanHandler(b => b.Status == MyJumpDriveStatus.Ready);
                AddPropertyHandler(Property.COMPLETE, readyHandler);
                AddPropertyHandler(Property.ABLE, readyHandler);
                AddBooleanHandler(Property.SUPPLY, b => !b.Recharge, (b, v) => b.Recharge = !v);

                var jumpDistanceHandler = DirectionalTypedHandler(Direction.NONE,
                    TypeHandler(NumericHandler(b => b.JumpDistanceMeters, (b,v) => b.SetValueFloat("JumpDistance", 100 * (v - b.MinJumpDistanceMeters) / (b.MaxJumpDistanceMeters - b.MinJumpDistanceMeters))), Direction.NONE),
                    TypeHandler(NumericHandler(b => b.MaxJumpDistanceMeters), Direction.UP),
                    TypeHandler(NumericHandler(b => b.MinJumpDistanceMeters), Direction.DOWN));

                AddPropertyHandler(Property.LEVEL, jumpDistanceHandler);
                AddPropertyHandler(Property.RANGE, jumpDistanceHandler);
            }
        }
    }
}
