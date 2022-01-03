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
        public class TimerBlockHandler : FunctionalBlockHandler<IMyTimerBlock> {
            public TimerBlockHandler() {
                AddBooleanHandler(Property.TRIGGER, b => b.IsCountingDown, (b, v) => b.Trigger());
                AddBooleanHandler(Property.MEDIA, b => !b.Silent, (b, v) => b.Silent = !v);
                AddNumericHandler(Property.RANGE, b => b.TriggerDelay, (b, v) => b.TriggerDelay = v, 1);
                AddBooleanHandler(Property.COUNTDOWN, b => b.IsCountingDown, (b, v) => { if (v) b.StartCountdown(); else b.StopCountdown(); });
            }
        }
    }
}
