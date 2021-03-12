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
                AddPropertyHandler(PropertyType.HEIGHT, new SimpleNumericPropertyHandler<IMyMotorSuspension>((b)=>b.Height, (b,v)=>b.Height=v,0.1f));
                AddPropertyHandler(PropertyType.ANGLE, new SimpleNumericPropertyHandler<IMyMotorSuspension>((b) => b.MaxSteerAngle, (b, v) => b.MaxSteerAngle = v, 5));
                AddPropertyHandler(PropertyType.VELOCITY, new PropertyValueNumericPropertyHandler<IMyMotorSuspension>("Speed Limit", 5));
                AddPropertyHandler(PropertyType.RATIO, new SimpleNumericPropertyHandler<IMyMotorSuspension>((b) => b.Power, (b,v)=> b.Power = v, 10));
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, PropertyType.HEIGHT);
                defaultNumericProperties.Add(DirectionType.DOWN, PropertyType.HEIGHT);
                //TODO: Add Strength?
            }
        }
    }
}
