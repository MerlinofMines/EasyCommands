using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public class GridBlockHandler : BlockHandler<IMyCubeGrid> {
            public GridBlockHandler() {
                AddStringHandler(Property.NAME, b => b.CustomName, (b,v) => b.CustomName = v);
                AddBooleanHandler(Property.LOCKED, b => b.IsStatic);
                AddStringHandler(Property.LEVEL, b => b.GridSizeEnum == MyCubeSize.Large ? "large" : "small");
            }

            public override string Name(IMyCubeGrid block) => block.CustomName;
            public override IEnumerable<IMyCubeGrid> SelectBlocksByType<U>(List<U> blocks, Func<U, bool> selector = null) =>
                blocks.Where(selector ?? (b => true)).Select(b => ((IMyTerminalBlock)b).CubeGrid).Distinct();
        }
    }
}
