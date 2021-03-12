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
        public class PistonBlockHandler : FunctionalBlockHandler<IMyPistonBase> {
            public PistonBlockHandler() : base() {
                AddPropertyHandler(PropertyType.HEIGHT, new PistonHeightHandler());
                AddPropertyHandler(PropertyType.VELOCITY, new PistonVelocityHandler());
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, PropertyType.HEIGHT);
                defaultNumericProperties.Add(DirectionType.DOWN, PropertyType.HEIGHT);
            }
        }

        public class PistonHeightHandler : PropertyHandler<IMyPistonBase> {
            public PistonHeightHandler() {
                GetNumeric = (b) => b.CurrentPosition;
                Set = extendPistonToValue;
                SetDirection = (b, d, v) => extendPistonToValue(b, v);
                IncrementDirection = (b, d, v) => {
                    if (d == DirectionType.UP) extendPistonToValue(b, b.CurrentPosition + v);
                    if (d == DirectionType.DOWN) extendPistonToValue(b, b.CurrentPosition - v);
                };
                Increment = (b, v) => IncrementDirection(b, DirectionType.UP, v);
                Move = (b, d) => {
                    if (d == DirectionType.UP) b.Extend();
                    if (d == DirectionType.DOWN) b.Retract();
                };
                Reverse = (b) => b.Reverse();
            }
        }

        public class PistonVelocityHandler : PropertyHandler<IMyPistonBase> {
            public PistonVelocityHandler() {
                GetNumeric = (b) => b.Velocity;
                Set = (b, v) => b.Velocity = v;
                SetDirection = (b, d, v) => b.Velocity = v;
                IncrementDirection = (b, d, v) => {
                    if (d == DirectionType.UP) b.Velocity += v;
                    if (d == DirectionType.DOWN) b.Velocity -= v;
                };
                Increment = (b, v) => IncrementDirection(b, DirectionType.UP, v);
                Move = (b, d) => {
                    if (d == DirectionType.UP) Increment(b, 1);
                    if (d == DirectionType.DOWN) Increment(b, -1);
                };
                Reverse = (b) => b.Velocity *= -1;
            }
        }

        static void extendPistonToValue(IMyPistonBase piston, float value) {
            if (piston.CurrentPosition < value) {
                piston.SetValue("UpperLimit", value);
                piston.Extend();
            } else {
                piston.SetValue("LowerLimit", value);
                piston.Retract();
            }
        }
    }
}
