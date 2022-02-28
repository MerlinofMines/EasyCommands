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
        public class EngineBlockHandler<T> : SubTypedBlockHandler<T> where T : class, IMyPowerProducer {
            public EngineBlockHandler(string subType = "") : base(IsSubType(subType)) {
                AddNumericHandler(Property.RATIO, b => b.CurrentOutput / b.MaxOutput);
                AddNumericHandler(Property.RANGE, b => b.MaxOutput);
                AddNumericHandler(Property.VOLUME, b => b.CurrentOutput);
            }
        }
    }
}
