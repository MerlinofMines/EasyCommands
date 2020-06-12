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
        public class TextSurfaceHandler : BlockHandler<IMyTextSurface> {
            public TextSurfaceHandler() {
                stringPropertySetters.Add(StringPropertyType.TEXT, (b, v) => b.WriteText(v));
                stringPropertySetters.Add(StringPropertyType.COLOR, (b, v) => b.FontColor=GetColor(v));
                numericPropertySetters.Add(NumericPropertyType.FONT_SIZE, new SimpleNumericPropertySetter<IMyTextSurface>((b) => b.FontSize, (b, v) => b.FontSize = v, 1));
                defaultStringProperty = StringPropertyType.TEXT;
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP,NumericPropertyType.FONT_SIZE);
            }

            public override List<object> GetBlocks(String name) {
                List<IMyFunctionalBlock> blocks = new List<IMyFunctionalBlock>();
                PROGRAM.GridTerminalSystem.GetBlocksOfType(blocks, block => block.CustomName.ToLower().Equals(name));

                List<object> surfaces = new List<object>();
                blocks.ForEach((b)=>Add(b, surfaces));
                return surfaces;
            }

            public override List<object> GetBlocksInGroup(String groupName) {
                List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();
                PROGRAM.GridTerminalSystem.GetBlockGroups(blockGroups);
                IMyBlockGroup group = blockGroups.Find(g => g.Name.ToLower() == groupName);
                if (group == null) { throw new Exception("Unable to find requested block group: " + groupName); }
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                group.GetBlocksOfType<IMyTerminalBlock>(blocks);
                List<object> surfaces = new List<object>();
                blocks.ForEach((b) => Add(b, surfaces));
                return surfaces;
            }

            protected override string Name(object block) {
                return ((IMyTextSurface)block).DisplayName;
            }

            private void Add(object b, List<object> surfaces) {
                if (b is IMyTextSurface) surfaces.Add((IMyTextSurface)b);
                else if (b is IMyTextSurfaceProvider) Add((IMyTextSurfaceProvider)b, surfaces);
            }

            private void Add(IMyTextSurfaceProvider p, List<object> surfaces) {
                for (int i = 0; i < p.SurfaceCount; i++) surfaces.Add(p.GetSurface(i));
            }
        }
    }
}
