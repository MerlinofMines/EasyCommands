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
                numericPropertyGetters.Add(NumericPropertyType.HEIGHT, (b) => b.Height);
                numericPropertySetters.Add(NumericPropertyType.HEIGHT, new SimpleNumericPropertySetter<IMyMotorSuspension>((b)=>b.Height, (b,v)=>b.Height=v,0.1f));
                numericPropertyGetters.Add(NumericPropertyType.ANGLE, (b) => b.SteerAngle);
                numericPropertySetters.Add(NumericPropertyType.ANGLE, new SimpleNumericPropertySetter<IMyMotorSuspension>((b) => b.MaxSteerAngle, (b, v) => b.MaxSteerAngle = v, 5));
                numericPropertyGetters.Add(NumericPropertyType.VELOCITY, (b) => b.GetValueFloat("Speed Limit"));
                numericPropertySetters.Add(NumericPropertyType.VELOCITY, new PropertyValueNumericPropertySetter<IMyMotorSuspension>("Speed Limit", 5));
                numericPropertyGetters.Add(NumericPropertyType.RATIO, (b) => b.Power);
                numericPropertySetters.Add(NumericPropertyType.RATIO, new SimpleNumericPropertySetter<IMyMotorSuspension>((b) => b.Power, (b,v)=> b.Power = v, 10));
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.HEIGHT);
                defaultNumericProperties.Add(DirectionType.DOWN, NumericPropertyType.HEIGHT);
                //TODO: Add Strength?
            }
        }
    }
}
