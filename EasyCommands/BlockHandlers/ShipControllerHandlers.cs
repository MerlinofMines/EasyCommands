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
                AddPropertyHandler(Property.VELOCITY, new RemoteControlVelocityHandler());
                AddPropertyHandler(Property.RANGE, new SimpleNumericPropertyHandler<IMyRemoteControl>((b) => (float)b.GetShipSpeed(), (b, v) => b.SpeedLimit = v, 10));
                AddBooleanHandler(Property.CONNECTED, b => false, (b,v) => b.SetDockingMode(v)); //TODO: Get Docking Mode?
                AddBooleanHandler(Property.TRIGGER, (b) => b.IsAutoPilotEnabled, (b, v) => b.SetAutoPilotEnabled(v));
                AddBooleanHandler(Property.AUTO, (b) => b.IsAutoPilotEnabled, (b, v) => b.SetAutoPilotEnabled(v));
                AddVectorHandler(Property.TARGET, (b) => b.CurrentWaypoint.Coords, (b, v) => {
                    b.ClearWaypoints();
                    b.AddWaypoint(new MyWaypointInfo("target", v));
                });
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.AUTO;
            }
        }

        public class ShipControllerHandler<T> : TerminalBlockHandler<T> where T : class, IMyShipController {
            public ShipControllerHandler() {
                AddBooleanHandler(Property.LOCKED, (b) => b.HandBrake, (b, v) => b.HandBrake = v);
                AddPropertyHandler(Property.VELOCITY, new ShipVelocityHandler<T>());
                AddPropertyHandler(Property.MOVE_INPUT, new ShipMoveInputHandler<T>());
                AddPropertyHandler(Property.ROLL_INPUT, new ShipRollInputHandler<T>());
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.VELOCITY;
//                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.LOCKED;
                defaultPropertiesByDirection[Direction.UP] = Property.VELOCITY;
                defaultDirection = Direction.UP;
            }
        }

        public class RemoteControlVelocityHandler : ShipVelocityHandler<IMyRemoteControl> {
            public RemoteControlVelocityHandler() : base() {
                Set = (b, p, v) => b.SpeedLimit = (float)v.GetValue();
                SetDirection = (b, p, d, v) => Set(b, p, v);
                Increment = (b, p, v) => Set(b, p, v.Plus(new NumberPrimitive(b.SpeedLimit)));
                IncrementDirection = (b, p, d, v) => Set(b, p, Get(b, p).Plus(Multiply(v,d)));
            }
            private Primitive Multiply(Primitive p, Direction d) { return (d == Direction.DOWN) ? p.Not() : p; }
        }

        public class ShipVelocityHandler<T> : PropertyHandler<T> where T : class, IMyShipController {
            public ShipVelocityHandler() {
                Get = (b, p) => new NumberPrimitive((float)b.GetShipSpeed());
                GetDirection = (b, p, d) => new NumberPrimitive(GetLinearVelocity(b, d));
            }

            float GetLinearVelocity(T block, Direction direction) {
                var vRel = Vector3D.TransformNormal(block.GetShipVelocities().LinearVelocity, MatrixD.Transpose(block.WorldMatrix));
                switch (direction) {
                    case Direction.UP: return Convert.ToSingle(vRel.Y);
                    case Direction.DOWN: return Convert.ToSingle(-vRel.Y);
                    case Direction.LEFT: return Convert.ToSingle(-vRel.X);
                    case Direction.RIGHT: return Convert.ToSingle(vRel.X);
                    case Direction.FORWARD: return Convert.ToSingle(-vRel.Z);
                    case Direction.BACKWARD: return Convert.ToSingle(vRel.Z);
                    default: throw new Exception("Unsupported Ship Velocity Direction Type: " + direction);
                }
            }

        }

        public class ShipMoveInputHandler<T> : PropertyHandler<T> where T : class, IMyShipController {
            public ShipMoveInputHandler() {
                Get = (b, p) => new NumberPrimitive(b.MoveIndicator.Length());
                GetDirection = (b, p, d) => new NumberPrimitive(GetPilotMovementInput(b,d));
            }

            float GetPilotMovementInput(T block, Direction direction) {
                var pilotInput = block.MoveIndicator;
                switch (direction) {
                    case Direction.UP: return pilotInput.Y;
                    case Direction.DOWN: return -pilotInput.Y;
                    case Direction.LEFT: return -pilotInput.X;
                    case Direction.RIGHT: return pilotInput.X;
                    case Direction.FORWARD: return -pilotInput.Z;
                    case Direction.BACKWARD: return pilotInput.Z;
                    default: throw new Exception("Unsupported User Input Movement Direction Type: " + direction);
                }
            }
        }

        public class ShipRollInputHandler<T> : PropertyHandler<T> where T : class, IMyShipController {
            public ShipRollInputHandler() {
                Get = (b, p) => new NumberPrimitive(b.RotationIndicator.Length());
                GetDirection = (b, p, d) => new NumberPrimitive(GetPilotRollInput(b, d));
            }

            float GetPilotRollInput(T block, Direction direction) {
                var rotationInput = block.RotationIndicator;
                var rollInput = block.RollIndicator;
                switch (direction) {
                    case Direction.UP: return -rotationInput.X;
                    case Direction.DOWN: return rotationInput.X;
                    case Direction.LEFT: return -rotationInput.Y;
                    case Direction.RIGHT: return rotationInput.Y;
                    case Direction.COUNTERCLOCKWISE: return -rollInput;
                    case Direction.CLOCKWISE: return rollInput;
                    default: throw new Exception("Unsupported User Input Rotation Direction Type: " + direction);
                }
            }
        }
    }
}
