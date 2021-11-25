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
        public class EngineBlockHandler<T> : FunctionalBlockHandler<T> where T : class, IMyPowerProducer {
            Func<T, bool> blockFilter;
            public EngineBlockHandler(String subType = "") {
                blockFilter = b => subType.Length == 0 || b.BlockDefinition.SubtypeId.Contains(subType);
                AddNumericHandler(Property.RATIO, b => b.CurrentOutput / b.MaxOutput);
                AddNumericHandler(Property.RANGE, b => b.MaxOutput);
                AddNumericHandler(Property.VOLUME, b => b.CurrentOutput);
            }

            public override List<T> GetBlocksOfType(Func<IMyTerminalBlock, bool> selector) => base.GetBlocksOfType(selector).Where(blockFilter).ToList();

            public override List<T> GetBlocksOfTypeInGroup(string groupName) => base.GetBlocksOfTypeInGroup(groupName).Where(blockFilter).ToList();
        }
    }
}
