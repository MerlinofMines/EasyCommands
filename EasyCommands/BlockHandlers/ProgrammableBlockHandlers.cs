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
                AddBooleanHandler(Property.COMPLETE, block => !block.IsRunning);
                AddStringHandler(Property.TEXT, block => block.TerminalRunArgument);
                AddReturnHandlers(Property.RUN, Return.STRING,
                    TypeHandler(StringHandler(b => b.IsRunning.ToString(), (b, v) => b.TryRun(v)), Return.STRING),
                    TypeHandler(BooleanHandler(b => b.IsRunning, (b, v) => { if (v) b.TryRun(b.TerminalRunArgument); else b.Enabled = false; }), Return.BOOLEAN));

                defaultPropertiesByPrimitive[Return.STRING] = Property.RUN;
            }
        }
    }
}
