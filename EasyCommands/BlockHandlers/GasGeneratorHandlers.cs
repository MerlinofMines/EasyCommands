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
        public class GasGeneratorHandler : FunctionalBlockHandler<IMyGasGenerator> {
            public GasGeneratorHandler() {
                AddBooleanHandler(PropertyType.AUTO, (b) => b.AutoRefill, (b, v) => b.AutoRefill = v);
                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.AUTO;
            }
        }
    }
}
