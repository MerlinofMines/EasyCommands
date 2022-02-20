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
                var maxLimitHandler = NumericHandler(b => b.MaxLimit, (b, v) => b.MaxLimit = v, 1);
                AddPropertyHandler(maxLimitHandler, Property.RANGE);
                AddPropertyHandler(maxLimitHandler, Property.RANGE, Property.UP);
                AddPropertyHandler(NumericHandler(b => b.MinLimit, (b, v) => b.MinLimit = v, 1), Property.RANGE, Property.DOWN);
                
                var reverseHandler = BooleanHandler(b => b.Velocity < 0, (b, v) => { if (v) b.Reverse(); });
                AddPropertyHandler(reverseHandler, Property.REVERSE);
                AddPropertyHandler(reverseHandler, Property.LEVEL, Property.REVERSE);

                var heightHandler = NumericHandler(b => b.CurrentPosition, ExtendPistonToValue, 1);
                AddPropertyHandler(heightHandler, Property.LEVEL);
                AddReturnHandlers(Property.UP, Return.BOOLEAN,
                    TypeHandler(BooleanHandler(b => b.Velocity > 0, (b, v) => b.Velocity = v ? Math.Abs(b.Velocity) : 0), Return.BOOLEAN),
                    TypeHandler(heightHandler, Return.NUMERIC));
                AddReturnHandlers(Property.DOWN, Return.BOOLEAN,
                    TypeHandler(BooleanHandler(b => b.Velocity < 0, (b, v) => b.Velocity = v ? Math.Abs(b.Velocity) : 0), Return.BOOLEAN),
                    TypeHandler(heightHandler, Return.NUMERIC));

                AddBooleanHandler(Property.UP, );
                AddBooleanHandler(Property.DOWN, b => b.Velocity < 0, (b, v) => b.Velocity = v ? -Math.Abs(b.Velocity) : 0);

                AddPropertyHandler(Property.LEVEL, new PistonHeightHandler());

                AddNumericHandler(Property.VELOCITY, (b) => b.Velocity, (b,v) => b.Velocity = v,1);
                AddBooleanHandler(Property.CONNECTED, b => b.IsAttached, (b, v) => { if (v) b.Attach(); else b.Detach(); });
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.LEVEL;
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
