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
                AddPropertyHandler(PropertyType.VELOCITY, new RemoteControlVelocityHandler());
                AddPropertyHandler(PropertyType.RANGE, new SimpleNumericPropertyHandler<IMyRemoteControl>((b) => (float)b.GetShipSpeed(), (b, v) => b.SpeedLimit = v, 10));
                AddBooleanHandler(PropertyType.CONNECTED, b => false, (b,v) => b.SetDockingMode(v)); //TODO: Get Docking Mode?
                AddBooleanHandler(PropertyType.TRIGGER, (b) => b.IsAutoPilotEnabled, (b, v) => b.SetAutoPilotEnabled(v));
                AddBooleanHandler(PropertyType.AUTO, (b) => b.IsAutoPilotEnabled, (b, v) => b.SetAutoPilotEnabled(v));
            }
        }

        public class ShipControllerHandler<T> : TerminalBlockHandler<T> where T : class, IMyShipController {
            public ShipControllerHandler() {
                AddBooleanHandler(PropertyType.LOCKED, (b) => b.HandBrake, (b, v) => b.HandBrake = v);
                AddPropertyHandler(PropertyType.VELOCITY, new ShipVelocityHandler<T>());
                AddPropertyHandler(PropertyType.MOVE_INPUT, new ShipMoveInputHandler<T>());
                AddPropertyHandler(PropertyType.ROLL_INPUT, new ShipRollInputHandler<T>());
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, PropertyType.VELOCITY);
            }
        }

        public class RemoteControlVelocityHandler : ShipVelocityHandler<IMyRemoteControl> {
            public RemoteControlVelocityHandler() : base() {
                Set = (b, v) => b.SpeedLimit = v;
                SetDirection = (b, d, v) => Set(b, v);
                Increment = (b, v) => Set(b, b.SpeedLimit + v);
                IncrementDirection = (b, d, v) => Set(b, b.SpeedLimit + Multiply(d) * v);
            }
            private float Multiply(DirectionType d) { return (d == DirectionType.UP) ? 1 : -1; }
        }

        public class ShipVelocityHandler<T> : PropertyHandler<T> where T : class, IMyShipController {
            public ShipVelocityHandler() {
                GetString = (b) => GetNumeric(b).ToString();
                GetBoolean = (b) => GetNumeric(b) > 0;
                GetNumeric = (b) => (float)b.GetShipSpeed();
                GetNumericDirection = GetLinearVelocity;
            }

            float GetLinearVelocity(T block, DirectionType direction) {
                var vRel = Vector3D.TransformNormal(block.GetShipVelocities().LinearVelocity, MatrixD.Transpose(block.WorldMatrix));
                switch (direction) {
                    case DirectionType.UP: return Convert.ToSingle(vRel.Y);
                    case DirectionType.DOWN: return Convert.ToSingle(-vRel.Y);
                    case DirectionType.LEFT: return Convert.ToSingle(-vRel.X);
                    case DirectionType.RIGHT: return Convert.ToSingle(vRel.X);
                    case DirectionType.FORWARD: return Convert.ToSingle(-vRel.Z);
                    case DirectionType.BACKWARD: return Convert.ToSingle(vRel.Z);
                    default: throw new Exception("Unsupported Ship Velocity Direction Type: " + direction);
                }
            }

        }

        public class ShipMoveInputHandler<T> : PropertyHandler<T> where T : class, IMyShipController {
            public ShipMoveInputHandler() {
                GetNumericDirection = GetPilotMovementInput;
                GetNumeric = (b) => b.MoveIndicator.Length();
            }

            float GetPilotMovementInput(T block, DirectionType direction) {
                var pilotInput = block.MoveIndicator;
                switch (direction) {
                    case DirectionType.UP: return pilotInput.Y;
                    case DirectionType.DOWN: return -pilotInput.Y;
                    case DirectionType.LEFT: return -pilotInput.X;
                    case DirectionType.RIGHT: return pilotInput.X;
                    case DirectionType.FORWARD: return -pilotInput.Z;
                    case DirectionType.BACKWARD: return pilotInput.Z;
                    default: throw new Exception("Unsupported User Input Movement Direction Type: " + direction);
                }
            }
        }

        public class ShipRollInputHandler<T> : PropertyHandler<T> where T : class, IMyShipController {
            public ShipRollInputHandler() {
                GetNumericDirection = GetPilotRollInput;
                GetNumeric = (b) => b.RotationIndicator.Length();
            }

            float GetPilotRollInput(T block, DirectionType direction) {
                var rotationInput = block.RotationIndicator;
                var rollInput = block.RollIndicator;
                switch (direction) {
                    case DirectionType.UP: return -rotationInput.X;
                    case DirectionType.DOWN: return rotationInput.X;
                    case DirectionType.LEFT: return -rotationInput.Y;
                    case DirectionType.RIGHT: return rotationInput.Y;
                    case DirectionType.COUNTERCLOCKWISE: return -rollInput;
                    case DirectionType.CLOCKWISE: return rollInput;
                    default: throw new Exception("Unsupported User Input Rotation Direction Type: " + direction);
                }
            }
        }
    }
}
