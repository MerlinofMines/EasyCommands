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
        public class ThreadBlockHandler : BlockHandler<Thread> {
            public ThreadBlockHandler() {
                AddStringHandler(Property.NAME, t => t.customName ?? t.name, (t,s) => t.customName = s);
                AddBooleanHandler(Property.COMPLETE, t => false, (t, b) => { t.Command = new NullCommand(); if(t==PROGRAM.currentThread) throw new ThreadInterruptException(); });
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.COMPLETE;
            }

            public override string Name(Thread thread) => thread.customName ?? thread.name;
        }
    }
}
