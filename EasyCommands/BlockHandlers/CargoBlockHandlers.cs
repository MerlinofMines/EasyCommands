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
        public class CargoHandler : MultiInstanceBlockHandler<IMyInventory> {
            public CargoHandler() {
            }

            public override void Add(IMyTerminalBlock block, List<IMyInventory> instances) {
                for (int i = 0; i < block.InventoryCount; i++) {
                    instances.Add(block.GetInventory(i));
                }
            }

            protected override string Name(IMyInventory block) => block.Owner.DisplayName;
        }
    }
}
