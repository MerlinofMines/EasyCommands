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
                AddPropertyHandler(Property.TARGET, TerminalPropertyHandler("TargetLocking", true));
            }
        }

        public class RemoteControlBlockHandler : ShipControllerHandler<IMyRemoteControl> {
            public RemoteControlBlockHandler() {
                var enableHandler = BooleanHandler(b => b.IsAutoPilotEnabled, (b, v) => b.SetAutoPilotEnabled(v));
                AddPropertyAliases(enableHandler, Property.AUTO, Property.RUN);
                AddNumericHandler(Property.RANGE, b => b.SpeedLimit, (b, v) => b.SpeedLimit = v, 10);
                AddPropertyHandler(Property.CONNECTED, TerminalPropertyHandler("DockingMode", true));
                AddVectorHandler(Property.TARGET, b => b.CurrentWaypoint.Coords, (b,v) => SetWaypoints(b, CastList(ResolvePrimitive(v))));
                AddListHandler(Property.WAYPOINTS,
                    b => {
                        var waypoints = NewList<MyWaypointInfo>();
                        b.GetWaypointInfo(waypoints);
                        return NewKeyedList(waypoints.Select(w => new KeyedVariable(GetStaticVariable(w.Name), GetStaticVariable(w.Coords))));
                    },
                    SetWaypoints);
                SetDefaultProperty(Property.AUTO, Return.BOOLEAN);
                SetDefaultProperty(Property.TARGET, Return.VECTOR);
                SetDefaultProperty(Property.WAYPOINTS, Return.LIST);
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
                AddPropertyAliases(enableHandler, Property.ENABLE, Property.POWER);
                AddPropertyAliases(dampenerHandler, Property.OVERRIDE, Property.AUTO);
                AddBooleanHandler(Property.LOCKED, b => b.HandBrake, (b, v) => b.HandBrake = v);
                AddVectorHandler(Property.STRENGTH, b => b.GetTotalGravity());
                AddPropertyHandler(NewList(Property.NATURAL, Property.STRENGTH), VectorHandler(b => b.GetNaturalGravity()));
                AddPropertyHandler(NewList(Property.ARTIFICIAL, Property.STRENGTH), VectorHandler(b => b.GetArtificialGravity()));
                AddNumericHandler(Property.WEIGHT, b => b.CalculateShipMass().TotalMass);
                AddNumericHandler(Property.LEVEL, b => GetPlanetElevation(b, MyPlanetElevation.Surface));
                AddNumericHandler(Property.ALTITUDE, b => GetPlanetElevation(b, MyPlanetElevation.Sealevel));
                AddNumericHandler(Property.WEIGHT, b => b.CalculateShipMass().TotalMass);
                AddBooleanHandler(Property.USE, b => b.IsUnderControl);
                AddDirectionHandlers(Property.VELOCITY, Direction.NONE,
                    TypeHandler(ReturnTypedHandler(Return.VECTOR, 
                        TypeHandler(VectorHandler(b => VelocityVector(b)), Return.VECTOR),
                        TypeHandler(NumericHandler(b => (float)VelocityVector(b).Length(), (b, v) => (b as IMyRemoteControl).SpeedLimit = v), Return.NUMERIC)), Direction.NONE),
                    SignedAxis(b => (float)VelocityVector(b).Y, null, Direction.UP, Direction.DOWN),
                    SignedAxis(b => (float)-VelocityVector(b).X, null, Direction.LEFT, Direction.RIGHT),
                    SignedAxis(b => (float)-VelocityVector(b).Z, null, Direction.FORWARD, Direction.BACKWARD));
                AddDirectionHandlers(Property.INPUT, Direction.NONE,
                    TypeHandler(VectorHandler(b => b.MoveIndicator*new Vector3(1,1,-1)), Direction.NONE),
                    SignedAxis(b => b.MoveIndicator.Y, null, Direction.UP, Direction.DOWN),
                    SignedAxis(b => -b.MoveIndicator.X, null, Direction.LEFT, Direction.RIGHT),
                    SignedAxis(b => -b.MoveIndicator.Z, null, Direction.FORWARD, Direction.BACKWARD));
                AddDirectionHandlers(Property.ROLL_INPUT, Direction.NONE,
                    TypeHandler(VectorHandler(b => new Vector3D(b.RotationIndicator/new Vector2(-9,9), b.RollIndicator)), Direction.NONE),
                    SignedAxis(b => -b.RotationIndicator.X/9, null, Direction.UP, Direction.DOWN),
                    SignedAxis(b => -b.RotationIndicator.Y/9, null, Direction.LEFT, Direction.RIGHT),
                    SignedAxis(b => -b.RollIndicator, null, Direction.COUNTERCLOCKWISE, Direction.CLOCKWISE));

                SetDefaultProperty(Property.VELOCITY, Return.NUMERIC, Direction.UP);
            }

            Vector3D VelocityVector(T block) => Vector3D.TransformNormal(block.GetShipVelocities().LinearVelocity, MatrixD.Transpose(block.WorldMatrix));

            float GetPlanetElevation(T block, MyPlanetElevation type) {
                double height;
                return (float)(block.TryGetPlanetElevation(type, out height) ? height : -1);
            }
        }
    }
}
