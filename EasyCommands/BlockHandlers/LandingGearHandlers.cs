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
        public class LandingGearHandler : FunctionalBlockHandler<IMyLandingGear> {
            public LandingGearHandler() {
                AddBooleanHandler(Property.AUTO, b => b.AutoLock, (b, v) => b.AutoLock = v);
                var lockHandler = BooleanHandler(b => b.IsLocked, (b, v) => { if (v) b.Lock(); else b.Unlock(); });
                AddPropertyHandler(Property.LOCKED, lockHandler);
                AddPropertyHandler(Property.CONNECTED, lockHandler);
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.LOCKED;
            }
        }
    }
}
