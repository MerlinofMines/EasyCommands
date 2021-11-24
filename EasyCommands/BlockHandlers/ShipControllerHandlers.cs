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
        public class RemoteControlBlockHandler : ShipControllerHandler<IMyRemoteControl> {
            public RemoteControlBlockHandler() : base() {
                var enableHandler = BooleanHandler(b => b.IsAutoPilotEnabled, (b, v) => b.SetAutoPilotEnabled(v));
                AddPropertyHandler(Property.AUTO, enableHandler);
                AddPropertyHandler(Property.RUN, enableHandler);
                AddNumericHandler(Property.RANGE, b => b.SpeedLimit, (b, v) => b.SpeedLimit = v, 10);
                AddPropertyHandler(Property.CONNECTED, TerminalBlockPropertyHandler("DockingMode", true));
                AddVectorHandler(Property.TARGET, b => b.CurrentWaypoint.Coords, (b, v) => {
                    b.ClearWaypoints();
                    b.AddWaypoint(new MyWaypointInfo("Waypoint", v));
                });
                AddListHandler(Property.WAYPOINTS,
                    b => {
                        var waypoints = new List<MyWaypointInfo>();
                        b.GetWaypointInfo(waypoints);
                        return new KeyedList(waypoints.Select(w => new KeyedVariable(w.Name, GetStaticVariable(w.Coords))).ToArray());
                    },
                    (b, v) => {
                        b.ClearWaypoints();
                        for (int i = 0; i < v.keyedValues.Count; i++) {
                            KeyedVariable value = v.keyedValues[i];
                            b.AddWaypoint(new MyWaypointInfo(value.HasKey() ? value.Key : "Waypoint " + i, CastVector(value.GetValue())));
                    }
                });
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.AUTO;
            }
        }

        public class ShipControllerHandler<T> : TerminalBlockHandler<T> where T : class, IMyShipController {
            public ShipControllerHandler() {
                AddBooleanHandler(Property.LOCKED, b => b.HandBrake, (b, v) => b.HandBrake = v);
                AddDirectionHandlers(Property.VELOCITY, Direction.NONE,
                    DirectionalHandler(NumericHandler(b => (float)b.GetShipSpeed()), Direction.NONE),
                    DirectionalHandler(NumericHandler(b => (float)VelocityVector(b).Y), Direction.UP),
                    DirectionalHandler(NumericHandler(b => (float)-VelocityVector(b).Y), Direction.DOWN),
                    DirectionalHandler(NumericHandler(b => (float)-VelocityVector(b).X), Direction.LEFT),
                    DirectionalHandler(NumericHandler(b => (float)VelocityVector(b).X), Direction.RIGHT),
                    DirectionalHandler(NumericHandler(b => (float)-VelocityVector(b).Z), Direction.FORWARD),
                    DirectionalHandler(NumericHandler(b => (float)VelocityVector(b).Z), Direction.BACKWARD));
                AddDirectionHandlers(Property.INPUT, Direction.NONE,
                    DirectionalHandler(NumericHandler(b => b.MoveIndicator.Length()), Direction.NONE),
                    DirectionalHandler(NumericHandler(b => b.MoveIndicator.Y), Direction.UP),
                    DirectionalHandler(NumericHandler(b => -b.MoveIndicator.Y), Direction.DOWN),
                    DirectionalHandler(NumericHandler(b => -b.MoveIndicator.X), Direction.LEFT),
                    DirectionalHandler(NumericHandler(b => b.MoveIndicator.X), Direction.RIGHT),
                    DirectionalHandler(NumericHandler(b => -b.MoveIndicator.Z), Direction.FORWARD),
                    DirectionalHandler(NumericHandler(b => b.MoveIndicator.Z), Direction.BACKWARD));
                AddDirectionHandlers(Property.ROLL_INPUT, Direction.NONE,
                    DirectionalHandler(NumericHandler(b => (float)Math.Sqrt(b.RotationIndicator.LengthSquared() + Math.Pow(b.RollIndicator, 2))), Direction.NONE),
                    DirectionalHandler(NumericHandler(b => -b.RotationIndicator.X), Direction.UP),
                    DirectionalHandler(NumericHandler(b => b.RotationIndicator.X), Direction.DOWN),
                    DirectionalHandler(NumericHandler(b => -b.RotationIndicator.Y), Direction.LEFT),
                    DirectionalHandler(NumericHandler(b => b.RotationIndicator.Y), Direction.RIGHT),
                    DirectionalHandler(NumericHandler(b => -b.RollIndicator), Direction.COUNTERCLOCKWISE),
                    DirectionalHandler(NumericHandler(b => b.RollIndicator), Direction.CLOCKWISE));

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.VELOCITY;
                defaultPropertiesByDirection[Direction.UP] = Property.VELOCITY;
            }

            Vector3D VelocityVector(T block) => Vector3D.TransformNormal(block.GetShipVelocities().LinearVelocity, MatrixD.Transpose(block.WorldMatrix));
        }
    }
}
