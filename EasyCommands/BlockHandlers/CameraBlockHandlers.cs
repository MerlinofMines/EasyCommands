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
        public class CameraBlockHandler : FunctionalBlockHandler<IMyCameraBlock> {
            public CameraBlockHandler() {
                defaultBooleanProperty = BooleanPropertyType.TRIGGER;
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.RANGE);
                booleanPropertySetters.Add(BooleanPropertyType.TRIGGER, (b,v) => b.EnableRaycast = v);
                booleanPropertyGetters.Add(BooleanPropertyType.TRIGGER, (b) => {var range = (double)GetRange(b); return b.CanScan(range)&&!b.Raycast(range).IsEmpty();});
                numericPropertyGetters.Add(NumericPropertyType.RANGE,GetRange);
                numericPropertySetters.Add(NumericPropertyType.RANGE,new SimpleNumericPropertySetter<IMyCameraBlock>(GetRange, (b, v) => SetCustomProperty(b, "Range", ""+v), 100));
            }
            public float GetRange(IMyCameraBlock b) { return float.Parse(GetCustomProperty(b, "Range") ?? "1000"); }
        }
    }
}
