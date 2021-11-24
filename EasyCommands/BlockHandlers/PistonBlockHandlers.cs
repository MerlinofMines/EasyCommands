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
                AddDirectionHandlers(Property.RANGE, Direction.UP,
                    DirectionalHandler(NumericHandler(b => b.MaxLimit, (b, v) => b.MaxLimit = v, 1), Direction.UP, Direction.FORWARD),
                    DirectionalHandler(NumericHandler(b => b.MinLimit, (b, v) => b.MinLimit = v, 1), Direction.DOWN, Direction.BACKWARD));
                AddPropertyHandler(Property.LEVEL, new PistonHeightHandler());
                AddNumericHandler(Property.VELOCITY, (b) => b.Velocity, (b,v) => b.Velocity = v,1);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.LEVEL;
                defaultPropertiesByDirection[Direction.UP] = Property.LEVEL;
                defaultPropertiesByDirection[Direction.DOWN] = Property.LEVEL;
            }
        }

        public class PistonHeightHandler : SimpleTypedHandler<IMyPistonBase, float> {
            public PistonHeightHandler() : base(b=>b.CurrentPosition, ExtendPistonToValue, CastNumber, 1) {
                Move = (b, p, d) => {
                    if (d == Direction.UP) b.Extend();
                    if (d == Direction.DOWN) b.Retract();
                };
                Reverse = (b, p) => b.Reverse();
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
