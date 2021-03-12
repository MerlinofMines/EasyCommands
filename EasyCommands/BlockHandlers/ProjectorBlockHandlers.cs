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
        public class ProjectorBlockHandler : FunctionalBlockHandler<IMyProjector> {
            public ProjectorBlockHandler() {
                AddBooleanHandler(PropertyType.COMPLETE, block => block.RemainingBlocks == 0);
                AddNumericHandler(PropertyType.RATIO, block => (block.TotalBlocks - block.RemainingBlocks) / block.TotalBlocks);
            }
        }
    }
}
