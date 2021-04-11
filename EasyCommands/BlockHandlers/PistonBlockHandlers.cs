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
                AddPropertyHandler(PropertyType.RANGE, new SimpleNumericDirectionPropertyHandler<IMyPistonBase>(GetLimit, SetLimit, DirectionType.UP));
                AddPropertyHandler(PropertyType.HEIGHT, new PistonHeightHandler());
                AddNumericHandler(PropertyType.VELOCITY, (b) => b.Velocity, (b,v) => b.Velocity = v,1);
                defaultPropertiesByPrimitive[PrimitiveType.NUMERIC] = PropertyType.HEIGHT;
                defaultPropertiesByDirection[DirectionType.UP] = PropertyType.HEIGHT;
                defaultPropertiesByDirection[DirectionType.DOWN] = PropertyType.HEIGHT;
                defaultDirection = DirectionType.UP;
            }
        }

        public class PistonHeightHandler : SimpleNumericPropertyHandler<IMyPistonBase> {
            public PistonHeightHandler() : base((b)=>b.CurrentPosition, ExtendPistonToValue, 1) {
                Move = (b, d) => {
                    if (d == DirectionType.UP) b.Extend();
                    if (d == DirectionType.DOWN) b.Retract();
                };
                Reverse = (b) => b.Reverse();
            }
        }

        static float GetLimit(IMyPistonBase piston, DirectionType direction) {
            switch (direction) {
                case DirectionType.UP:
                case DirectionType.CLOCKWISE:
                case DirectionType.FORWARD:
                    return piston.MaxLimit;
                case DirectionType.DOWN:
                case DirectionType.COUNTERCLOCKWISE:
                case DirectionType.BACKWARD:
                    return piston.MinLimit;
                default:
                    throw new Exception("Unknown direction: " + direction);
            }
        }

        static void SetLimit(IMyPistonBase piston, DirectionType direction, float value) {
            switch (direction) {
                case DirectionType.UP:
                case DirectionType.FORWARD:
                    piston.MaxLimit = value;
                    break;
                case DirectionType.DOWN:
                case DirectionType.BACKWARD:
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
