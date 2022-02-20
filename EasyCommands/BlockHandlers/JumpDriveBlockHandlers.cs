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
                AddBooleanHandler(Property.COMPLETE, b => b.Status == MyJumpDriveStatus.Ready);
                AddBooleanHandler(Property.SUPPLY, b => !b.Recharge, (b, v) => b.Recharge = !v);

                var jumpDistanceHandler = NumericHandler(b => b.JumpDistanceMeters, (b, v) => b.SetValueFloat("JumpDistance", 100 * (v - b.MinJumpDistanceMeters) / (b.MaxJumpDistanceMeters - b.MinJumpDistanceMeters)));
                AddPropertyHandler(jumpDistanceHandler, Property.RANGE);
                AddPropertyHandler(jumpDistanceHandler, Property.LEVEL);

                var maxJumpDistanceHandler = NumericHandler(b => b.MaxJumpDistanceMeters);
                AddPropertyHandler(maxJumpDistanceHandler, Property.RANGE, Property.UP);
                AddPropertyHandler(maxJumpDistanceHandler, Property.LEVEL, Property.UP);

                var minJumpDistanceHandler = NumericHandler(b => b.MaxJumpDistanceMeters);
                AddPropertyHandler(minJumpDistanceHandler, Property.RANGE, Property.DOWN);
                AddPropertyHandler(minJumpDistanceHandler, Property.LEVEL, Property.DOWN);
            }
        }
    }
}
