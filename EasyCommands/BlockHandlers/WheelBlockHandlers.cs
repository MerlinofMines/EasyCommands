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
        public class WheelSuspensionBlockHandler : FunctionalBlockHandler<IMyMotorSuspension> {
            public WheelSuspensionBlockHandler() {
                AddNumericHandler(Property.LEVEL, b => b.Height, (b,v)=>b.Height=v,0.1f);
                AddNumericHandler(Property.ANGLE, b => b.MaxSteerAngle, (b, v) => b.MaxSteerAngle = v, 5);
                AddNumericHandler(Property.RATIO, b => b.Power, (b, v) => b.Power = v, 10);
                AddPropertyHandler(Property.VELOCITY, new PropertyValueNumericPropertyHandler<IMyMotorSuspension>("Speed Limit", 5));
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.LEVEL;
                defaultPropertiesByDirection[Direction.UP] = Property.LEVEL;
                defaultPropertiesByDirection[Direction.DOWN] = Property.LEVEL;
                defaultDirection = Direction.UP;
                //TODO: Add Strength?
            }
        }
    }
}
