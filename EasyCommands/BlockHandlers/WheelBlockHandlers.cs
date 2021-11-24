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
                AddNumericHandler(Property.LEVEL, b => b.Height, (b, v) => b.Height = v, 0.1f);
                AddNumericHandler(Property.ANGLE, b => (float)(-b.SteerAngle*180/Math.PI), (b, v) => { b.SteeringOverride = (float)(v * Math.PI / 144); b.MaxSteerAngle = 1; }, .1f);
                AddBooleanHandler(Property.LOCKED, (b) => b.Brake, (b, v) => b.Brake = v);
                AddDirectionHandlers(Property.RANGE, Direction.UP,
                    DirectionalHandler(TerminalBlockPropertyHandler("Speed Limit", 5), Direction.UP, Direction.DOWN),
                    DirectionalHandler(NumericHandler(b => b.MaxSteerAngle, (b, v) => b.MaxSteerAngle = v), Direction.LEFT, Direction.RIGHT));

                AddNumericHandler(Property.VELOCITY, b => b.PropulsionOverride, (b,v) => b.PropulsionOverride = v, 0.1f);
                AddNumericHandler(Property.STRENGTH, b => b.Strength, (b, v) => b.Strength = v, 10);
                AddNumericHandler(Property.POWER, b => b.Power, (b, v) => b.Power = v, 10);
                AddNumericHandler(Property.RATIO, b => b.Friction, (b, v) => b.Friction = v, 10);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.LEVEL;
                defaultPropertiesByDirection[Direction.UP] = Property.LEVEL;
                defaultPropertiesByDirection[Direction.DOWN] = Property.LEVEL;
            }
        }
    }
}
