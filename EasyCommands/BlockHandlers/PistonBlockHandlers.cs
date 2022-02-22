﻿using Sandbox.Game.EntityComponents;
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
            public PistonBlockHandler() {
                AddDirectionHandlers(Property.RANGE, Direction.UP,
                    TypeHandler(NumericHandler(b => b.MaxLimit, (b, v) => b.MaxLimit = v, 1), Direction.UP, Direction.FORWARD),
                    TypeHandler(NumericHandler(b => b.MinLimit, (b, v) => b.MinLimit = v, 1), Direction.DOWN, Direction.BACKWARD));
                AddPropertyHandler(Property.LEVEL, new PistonHeightHandler());
                AddNumericHandler(Property.VELOCITY, b => b.Velocity, (b,v) => b.Velocity = v,1);
                AddBooleanHandler(Property.CONNECTED, b => b.IsAttached, (b, v) => { if (v) b.Attach(); else b.Detach(); });
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
                piston.MaxLimit = value;
                piston.Extend();
            } else {
                piston.MinLimit = value;
                piston.Retract();
            }
        }
    }
}
