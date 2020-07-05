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
                numericPropertySetters.Add(NumericPropertyType.VELOCITY, new SimpleNumericPropertySetter<IMyRemoteControl>((b) => (float)b.GetShipSpeed(), (b, v) => b.SpeedLimit = v, 10));
                numericPropertySetters.Add(NumericPropertyType.RANGE, new SimpleNumericPropertySetter<IMyRemoteControl>((b) => (float)b.GetShipSpeed(), (b, v) => b.SpeedLimit = v, 10));
                numericPropertyGetters.Add(NumericPropertyType.RANGE, (b) => b.SpeedLimit);
                booleanPropertySetters.Add(BooleanPropertyType.CONNECTED, (b, v) => b.SetDockingMode(v));
                booleanPropertyGetters.Add(BooleanPropertyType.TRIGGER, (b) => b.IsAutoPilotEnabled);
                booleanPropertySetters.Add(BooleanPropertyType.TRIGGER, (b,v) => b.SetAutoPilotEnabled(v));
                booleanPropertyGetters.Add(BooleanPropertyType.AUTO, (b) => b.IsAutoPilotEnabled);
                booleanPropertySetters.Add(BooleanPropertyType.AUTO, (b, v) => b.SetAutoPilotEnabled(v));
            }
        }

        public class ShipControllerHandler<T> : TerminalBlockHandler<T> where T : class, IMyShipController {
            public ShipControllerHandler() {
                booleanPropertyGetters.Add(BooleanPropertyType.LOCKED, (b) => b.HandBrake);
                booleanPropertySetters.Add(BooleanPropertyType.LOCKED, (b, v) => b.HandBrake = v);
                numericPropertyGetters.Add(NumericPropertyType.VELOCITY, (b) => (float)b.GetShipSpeed());
                numericPropertyDirectionGetters.Add(NumericPropertyType.VELOCITY, GetLinearVelocity);
                numericPropertyDirectionGetters.Add(NumericPropertyType.MOVE_INPUT, GetPilotMovementInput);
                numericPropertyDirectionGetters.Add(NumericPropertyType.ROLL_INPUT, GetPilotRollInput);
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.VELOCITY);
            }

            float GetPilotMovementInput(T block, DirectionType direction) {
                var pilotInput = block.MoveIndicator;
                switch(direction) {
                    case DirectionType.UP: return pilotInput.Y;
                    case DirectionType.DOWN: return -pilotInput.Y;
                    case DirectionType.LEFT: return -pilotInput.X;
                    case DirectionType.RIGHT: return pilotInput.X;
                    case DirectionType.FORWARD: return -pilotInput.Z;
                    case DirectionType.BACKWARD: return pilotInput.Z;
                    default: throw new Exception("Unsupported User Input Movement Direction Type: " + direction);
                }
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
