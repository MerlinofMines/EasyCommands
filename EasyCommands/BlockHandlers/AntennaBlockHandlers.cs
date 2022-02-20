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
        public class LaserAntennaBlockHandler : FunctionalBlockHandler<IMyLaserAntenna> {
            public LaserAntennaBlockHandler() {
                AddVectorHandler(Property.TARGET, b => b.TargetCoords, (b, v) => b.SetTargetCoords("GPS:Target:" + VectorToString(v) + ":"));
                AddBooleanHandler(Property.LOCKED, b => b.IsPermanent, (b,v) => b.IsPermanent = v);
                AddNumericHandler(Property.RANGE, b => b.Range, (b, v) => b.Range = v, 1000);
                AddBooleanHandler(Property.CONNECTED, b => b.Status == MyLaserAntennaStatus.Connected, (b, v) => { if (v) b.Connect(); });
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
            }
        }

        public class AntennaBlockHandler : FunctionalBlockHandler<IMyRadioAntenna> {
            public AntennaBlockHandler() {
                AddStringHandler(Property.TEXT, b => b.HudText, (b, v) => b.HudText = v);
                AddBooleanHandler(Property.CONNECTED, b => b.EnableBroadcasting, (b, v) => b.EnableBroadcasting = v);
                AddBooleanHandler(Property.SUPPLY, b => b.EnableBroadcasting, (b, v) => b.EnableBroadcasting = v);
                AddNumericHandler(Property.RANGE, b => b.Radius, (b, v) => b.Radius = v, 1000);
                AddBooleanHandler(Property.SHOW, b => b.ShowShipName, (b, v) => b.ShowShipName = v);
                defaultPropertiesByPrimitive[Return.STRING] = Property.TEXT;
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.CONNECTED;
            }
        }
    }
}
