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
                AddStringHandler(PropertyType.TEXT, b => b.GetText(), (b, v) => b.WriteText(v));
                AddColorHandler(PropertyType.COLOR, b => b.FontColor, (b, v) => b.FontColor = v);
                AddPropertyHandler(PropertyType.FONT_SIZE, new SimpleNumericPropertyHandler<IMyTextSurface>((b) => b.FontSize, (b, v) => b.FontSize = v, 1));
                defaultPropertiesByPrimitive[PrimitiveType.STRING] = PropertyType.TEXT;
                defaultPropertiesByPrimitive[PrimitiveType.COLOR] = PropertyType.COLOR;
                defaultPropertiesByDirection[DirectionType.UP] = PropertyType.FONT_SIZE;
                defaultDirection = DirectionType.UP;
            }

            public override List<IMyTextSurface> GetBlocksOfType(Func<IMyTerminalBlock, bool> selector) {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                PROGRAM.GridTerminalSystem.GetBlocksOfType(blocks, selector);

                List<IMyTextSurface> surfaces = new List<IMyTextSurface>();
                blocks.ForEach((b)=>Add(b, surfaces));
                return surfaces;
            }

            public override List<IMyTextSurface> GetBlocksOfTypeInGroup(String groupName) {
                List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();
                PROGRAM.GridTerminalSystem.GetBlockGroups(blockGroups);
                IMyBlockGroup group = blockGroups.Find(g => g.Name == groupName);
                if (group == null) { throw new Exception("Unable to find requested block group: " + groupName); }
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                group.GetBlocksOfType<IMyTerminalBlock>(blocks);
                List<IMyTextSurface> surfaces = new List<IMyTextSurface>();
                blocks.ForEach((b) => Add(b, surfaces));
                return surfaces;
            }

            protected override string Name(IMyTextSurface block) {
                return block.DisplayName;
            }

            private void Add(object b, List<IMyTextSurface> surfaces) {
                if (b is IMyTextSurface) surfaces.Add((IMyTextSurface)b);
                else if (b is IMyTextSurfaceProvider) Add((IMyTextSurfaceProvider)b, surfaces);
            }

            private void Add(IMyTextSurfaceProvider p, List<IMyTextSurface> surfaces) {
                for (int i = 0; i < p.SurfaceCount; i++) surfaces.Add(p.GetSurface(i));
            }
        }
    }
}
