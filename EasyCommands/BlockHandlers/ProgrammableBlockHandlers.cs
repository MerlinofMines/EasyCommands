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
        public class ProgramBlockHandler : FunctionalBlockHandler<IMyProgrammableBlock> {
            public ProgramBlockHandler() : base() {
                AddBooleanHandler(Property.RUNNING, block => block.DetailedInfo.EndsWith("Running\n"));
                AddBooleanHandler(Property.STOPPED, block => block.DetailedInfo.EndsWith("Stopped\n"));
                AddBooleanHandler(Property.PAUSED, block => block.DetailedInfo.EndsWith("Paused\n"));
                AddBooleanHandler(Property.COMPLETE, block => block.DetailedInfo.EndsWith("Complete\n"));
                AddStringHandler(Property.RUN, (block) => "", (block, value) => block.TryRun(value));
                defaultPropertiesByPrimitive[Return.STRING] = Property.RUN;
            }
        }
    }
}
