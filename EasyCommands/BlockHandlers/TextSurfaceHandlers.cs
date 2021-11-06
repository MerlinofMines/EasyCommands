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
        public class TextSurfaceHandler : MultiInstanceBlockHandler<IMyTextSurface> {
            public TextSurfaceHandler() {
                AddStringHandler(Property.TEXT, b => b.GetText(), (b, v) => b.WriteText(v));
                AddColorHandler(Property.COLOR, b => b.FontColor, (b, v) => b.FontColor = v);
                AddNumericHandler(Property.FONT_SIZE, b => b.FontSize, (b, v) => b.FontSize = v, 1);
                defaultPropertiesByPrimitive[Return.STRING] = Property.TEXT;
                defaultPropertiesByPrimitive[Return.COLOR] = Property.COLOR;
                defaultPropertiesByDirection[Direction.UP] = Property.FONT_SIZE;
                defaultDirection = Direction.UP;
            }

            public override string Name(IMyTextSurface block) {
                return block.DisplayName;
            }

            public override void GetInstances(IMyTerminalBlock b, List<IMyTextSurface> surfaces) {
                if (b is IMyTextSurface) surfaces.Add((IMyTextSurface)b);
                else if (b is IMyTextSurfaceProvider) Add((IMyTextSurfaceProvider)b, surfaces);
            }

            void Add(IMyTextSurfaceProvider p, List<IMyTextSurface> surfaces) {
                for (int i = 0; i < p.SurfaceCount; i++) surfaces.Add(p.GetSurface(i));
            }
        }
    }
}
