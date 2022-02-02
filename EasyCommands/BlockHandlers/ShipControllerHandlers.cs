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
        public class CockpitBlockHandler<T> : ShipControllerHandler<T> where T : class, IMyCockpit {
            public CockpitBlockHandler() {
                AddNumericHandler(Property.RATIO, b => b.OxygenFilledRatio);
                AddNumericHandler(Property.RANGE, b => b.OxygenCapacity);
            }
        }

        public class RemoteControlBlockHandler : ShipControllerHandler<IMyRemoteControl> {
            public RemoteControlBlockHandler() {
                var enableHandler = BooleanHandler(b => b.IsAutoPilotEnabled, (b, v) => b.SetAutoPilotEnabled(v));
                AddPropertyHandler(Property.AUTO, enableHandler);
                AddPropertyHandler(Property.RUN, enableHandler);
                AddNumericHandler(Property.RANGE, b => b.SpeedLimit, (b, v) => b.SpeedLimit = v, 10);
                AddPropertyHandler(Property.CONNECTED, TerminalBlockPropertyHandler("DockingMode", true));
                AddVectorHandler(Property.TARGET, b => b.CurrentWaypoint.Coords, (b,v) => SetWaypoints(b, CastList(ResolvePrimitive(v))));
                AddListHandler(Property.WAYPOINTS,
                    b => {
                        var waypoints = NewList<MyWaypointInfo>();
                        b.GetWaypointInfo(waypoints);
                        return NewKeyedList(waypoints.Select(w => new KeyedVariable(GetStaticVariable(w.Name), GetStaticVariable(w.Coords))));
                    },
                    SetWaypoints);
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.AUTO;
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
                defaultPropertiesByPrimitive[Return.LIST] = Property.WAYPOINTS;
            }

            void SetWaypoints(IMyRemoteControl b, KeyedList waypoints) {
                b.ClearWaypoints();
                for (int i = 0; i < waypoints.keyedValues.Count; i++) {
                    KeyedVariable value = waypoints.keyedValues[i];
                    b.AddWaypoint(new MyWaypointInfo(value.HasKey() ? value.GetKey() : "Waypoint " + (i+1), CastVector(value.GetValue())));
                }
            }
        }

        public class ShipControllerHandler<T> : TerminalBlockHandler<T> where T : class, IMyShipController {
            public ShipControllerHandler() {
                var dampenerHandler = BooleanHandler(b => b.DampenersOverride, (b, v) => b.DampenersOverride = v);
                var enableHandler = BooleanHandler(b => b.IsMainCockpit, (b, v) => b.IsMainCockpit = v);
                AddPropertyHandler(Property.ENABLE, enableHandler);
                AddPropertyHandler(Property.POWER, enableHandler);
                AddPropertyHandler(Property.OVERRIDE, dampenerHandler);
                AddPropertyHandler(Property.AUTO, dampenerHandler);
                AddBooleanHandler(Property.LOCKED, b => b.HandBrake, (b, v) => b.HandBrake = v);
                AddVectorHandler(Property.STRENGTH, b => b.GetTotalGravity());
                AddVectorHandler(Property.NATURAL_GRAVITY, b => b.GetNaturalGravity());
                AddVectorHandler(Property.ARTIFICIAL_GRAVITY, b => b.GetArtificialGravity());
                AddNumericHandler(Property.LEVEL, b => b.CalculateShipMass().TotalMass);
                AddBooleanHandler(Property.USE, b => b.IsUnderControl);
                AddDirectionHandlers(Property.VELOCITY, Direction.NONE,
                    TypeHandler(NumericHandler(b => (float)b.GetShipSpeed(), (b,v) => (b as IMyRemoteControl).SpeedLimit = v), Direction.NONE),
                    TypeHandler(NumericHandler(b => (float)VelocityVector(b).Y), Direction.UP),
                    TypeHandler(NumericHandler(b => (float)-VelocityVector(b).Y), Direction.DOWN),
                    TypeHandler(NumericHandler(b => (float)-VelocityVector(b).X), Direction.LEFT),
                    TypeHandler(NumericHandler(b => (float)VelocityVector(b).X), Direction.RIGHT),
                    TypeHandler(NumericHandler(b => (float)-VelocityVector(b).Z), Direction.FORWARD),
                    TypeHandler(NumericHandler(b => (float)VelocityVector(b).Z), Direction.BACKWARD));
                AddDirectionHandlers(Property.INPUT, Direction.NONE,
                    TypeHandler(VectorHandler(b => b.MoveIndicator*new Vector3(1,1,-1)), Direction.NONE),
                    TypeHandler(NumericHandler(b => b.MoveIndicator.Y), Direction.UP),
                    TypeHandler(NumericHandler(b => -b.MoveIndicator.Y), Direction.DOWN),
                    TypeHandler(NumericHandler(b => -b.MoveIndicator.X), Direction.LEFT),
                    TypeHandler(NumericHandler(b => b.MoveIndicator.X), Direction.RIGHT),
                    TypeHandler(NumericHandler(b => -b.MoveIndicator.Z), Direction.FORWARD),
                    TypeHandler(NumericHandler(b => b.MoveIndicator.Z), Direction.BACKWARD));
                AddDirectionHandlers(Property.ROLL_INPUT, Direction.NONE,
                    TypeHandler(VectorHandler(b => new Vector3D(b.RotationIndicator/new Vector2(-9,9), b.RollIndicator)), Direction.NONE),
                    TypeHandler(NumericHandler(b => -b.RotationIndicator.X/9), Direction.UP),
                    TypeHandler(NumericHandler(b => b.RotationIndicator.X/9), Direction.DOWN),
                    TypeHandler(NumericHandler(b => -b.RotationIndicator.Y/9), Direction.LEFT),
                    TypeHandler(NumericHandler(b => b.RotationIndicator.Y/9), Direction.RIGHT),
                    TypeHandler(NumericHandler(b => -b.RollIndicator), Direction.COUNTERCLOCKWISE),
                    TypeHandler(NumericHandler(b => b.RollIndicator), Direction.CLOCKWISE));

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.VELOCITY;
                defaultPropertiesByDirection[Direction.UP] = Property.VELOCITY;
            }

            Vector3D VelocityVector(T block) => Vector3D.TransformNormal(block.GetShipVelocities().LinearVelocity, MatrixD.Transpose(block.WorldMatrix));
        }
    }
}
