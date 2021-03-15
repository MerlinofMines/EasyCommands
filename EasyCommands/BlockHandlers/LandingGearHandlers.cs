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
                AddBooleanHandler(PropertyType.AUTO, (b) => b.AutoLock, (b, v) => b.AutoLock = v);
                AddBooleanHandler(PropertyType.LOCKED, Connected, Connect);
                AddBooleanHandler(PropertyType.CONNECTED, Connected, Connect);
                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.LOCKED;
            }

            static bool Connected(IMyLandingGear gear) {
                return gear.IsLocked;
            }

            static void Connect(IMyLandingGear connector, bool value) {
                if (value) { connector.Lock(); } else { connector.Unlock(); }
            }
        }
    }
}
