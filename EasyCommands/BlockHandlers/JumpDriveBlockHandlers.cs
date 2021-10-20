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
                AddNumericHandler(Property.RATIO, b => b.CurrentStoredPower / b.MaxStoredPower);
                AddNumericHandler(Property.RANGE, b => b.MaxStoredPower);
                AddNumericHandler(Property.LEVEL, b => b.CurrentStoredPower);
                AddBooleanHandler(Property.COMPLETE, b => b.Status == MyJumpDriveStatus.Ready);
            }
        }
    }
}
