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
        public class WarheadBlockHandler : TerminalBlockHandler<IMyWarhead> {
            public WarheadBlockHandler() {
                AddBooleanHandler(Property.TRIGGER, b => b.IsCountingDown, (b,v) => b.Detonate());
                AddBooleanHandler(Property.ENABLE, b => b.IsArmed, (b, v) => b.IsArmed = v);
                AddNumericHandler(Property.RANGE, b => b.DetonationTime, (b, v) => b.DetonationTime = v, 1);
                AddBooleanHandler(Property.COUNTDOWN, b => b.IsCountingDown, (b, v) => { if (v) b.StartCountdown(); else b.StopCountdown(); });
            }
        }
    }
}
