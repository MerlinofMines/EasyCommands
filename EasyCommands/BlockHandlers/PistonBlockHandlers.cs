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
                AddPropertyHandler(Property.RANGE, new SimpleNumericDirectionPropertyHandler<IMyPistonBase>(GetLimit, SetLimit, Direction.UP));
                AddPropertyHandler(Property.HEIGHT, new PistonHeightHandler());
                AddNumericHandler(Property.VELOCITY, (b) => b.Velocity, (b,v) => b.Velocity = v,1);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.HEIGHT;
                defaultPropertiesByDirection[Direction.UP] = Property.HEIGHT;
                defaultPropertiesByDirection[Direction.DOWN] = Property.HEIGHT;
                defaultDirection = Direction.UP;
            }
        }

        public class PistonHeightHandler : SimpleNumericPropertyHandler<IMyPistonBase> {
            public PistonHeightHandler() : base((b)=>b.CurrentPosition, ExtendPistonToValue, 1) {
                Move = (b, d) => {
                    if (d == Direction.UP) b.Extend();
                    if (d == Direction.DOWN) b.Retract();
                };
                Reverse = (b) => b.Reverse();
            }
        }

        static float GetLimit(IMyPistonBase piston, Direction direction) {
            switch (direction) {
                case Direction.UP:
                case Direction.FORWARD:
                    return piston.MaxLimit;
                case Direction.DOWN:
                case Direction.BACKWARD:
                    return piston.MinLimit;
                default:
                    throw new Exception("Unknown direction: " + direction);
            }
        }

        static void SetLimit(IMyPistonBase piston, Direction direction, float value) {
            switch (direction) {
                case Direction.UP:
                case Direction.FORWARD:
                    piston.MaxLimit = value;
                    break;
                case Direction.DOWN:
                case Direction.BACKWARD:
                    piston.MinLimit = value;
                    break;
                default:
                    throw new Exception("Unsupported direction: " + direction);
            }
        }

        static void ExtendPistonToValue(IMyPistonBase piston, float value) {
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
