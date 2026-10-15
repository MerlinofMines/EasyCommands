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
        public class TurretControlBlockHandler : FunctionalBlockHandler<IMyTurretControlBlock> {
            public TurretControlBlockHandler() {
                AddNumericHandler(Property.RANGE, b => b.Range, (b, v) => b.Range = v, 100f);
                AddPropertyHandler(Property.LOCKED, TerminalPropertyHandler("EnableTargetLocking", true));
                AddPropertyHandler(Property.AUTO, TerminalPropertyHandler("AI", true));
                AddBooleanHandler(Property.USE, b => b.IsUnderControl);
                AddReturnHandlers(Property.TARGET, Return.VECTOR,
                    TypeHandler(VectorHandler(b => b.HasTarget ? GetPosition(b.GetTargetedEntity()) : Vector3D.Zero), Return.VECTOR),
                    TypeHandler(BooleanHandler(b => b.HasTarget), Return.BOOLEAN),
                    TypeHandler(StringHandler(b => b.GetTargetingGroup(), (b, v) => b.SetTargetingGroup(v)), Return.STRING));
                AddPropertyHandler(Property.ANGLE, TerminalPropertyHandler("AngleDeviation", 5f));
                AddBooleanHandler(Property.TRIGGER, b => GetGuns(b).Any(g => g.IsShooting), (b, v) => {
                    foreach (IMyUserControllableGun gun in GetGuns(b)) gun.Shoot = v;
                });
                AddDirectionHandlers(Property.VELOCITY, Direction.RIGHT,
                    TypeHandler(NumericHandler(b => b.VelocityMultiplierAzimuthRpm, (b,v) => b.VelocityMultiplierAzimuthRpm = v), Direction.LEFT, Direction.RIGHT),
                    TypeHandler(NumericHandler(b => b.VelocityMultiplierElevationRpm, (b,v) => b.VelocityMultiplierElevationRpm = v), Direction.UP, Direction.DOWN));
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RANGE;
            }

            IEnumerable<IMyUserControllableGun> GetGuns(IMyTurretControlBlock block) {
                var tools = NewList<IMyFunctionalBlock>();
                block.GetTools(tools);
                return tools.OfType<IMyUserControllableGun>();
            }
        }

        public class TurretBlockHandler<T> : GunBlockHandler<IMyLargeTurretBase> {
            public TurretBlockHandler() {
                AddNumericHandler(Property.RANGE, b => b.Range, (b, v) => b.Range = v, 100f);
                AddPropertyHandler(Property.LOCKED, TerminalPropertyHandler("EnableTargetLocking", true));
                AddBooleanHandler(Property.USE, b => b.IsUnderControl);
                AddBooleanHandler(Property.ROLL_INPUT, b => b.EnableIdleRotation, (b, v) => {
                    b.EnableIdleRotation = v;
                    b.SyncEnableIdleRotation();
                });

                AddReturnHandlers(Property.TARGET, Return.VECTOR,
                    TypeHandler(VectorHandler(GetTarget, SetTarget), Return.VECTOR),
                    TypeHandler(BooleanHandler(b => b.HasTarget, (b, v) => { if (!v) ResetTarget(b); }), Return.BOOLEAN),
                    TypeHandler(StringHandler(b => b.GetTargetingGroup(), (b, v) => b.SetTargetingGroup(v)), Return.STRING));
                AddPropertyHandler(NewList(Property.TARGET, Property.VELOCITY), VectorHandler(b => b.GetTargetedEntity().Velocity, (b, v) => b.TrackTarget(GetTarget(b), v)));
                AddNumericHandler(Property.ANGLE, b => b.Azimuth * RadiansToDegrees, (b, v) => {
                    b.Azimuth = v * DegreesToRadians;
                    b.SyncAzimuth();
                }, 5);
                AddNumericHandler(Property.ALTITUDE, b => b.Elevation * RadiansToDegrees, (b, v) => {
                    b.Elevation = v * DegreesToRadians;
                    b.SyncElevation();
                }, 5);
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RANGE;
            }

            Vector3D GetTarget(IMyLargeTurretBase turret) =>
                GetVector(GetCustomProperty(turret, "target") ?? "") ?? (turret.HasTarget ? GetPosition(turret.GetTargetedEntity()) : Vector3D.Zero);

            void ResetTarget(IMyLargeTurretBase turret) {
                //Idle Movement setting gets reset when calling ResetTargetingToDefault(), so need to re-apply it.
                bool idleMovement = turret.EnableIdleRotation;
                turret.ResetTargetingToDefault();
                DeleteCustomProperty(turret, "target");
                turret.EnableIdleRotation = idleMovement;
                turret.SyncEnableIdleRotation();
            }

            void SetTarget(IMyLargeTurretBase turret, Vector3D target) {
                turret.SetTarget(target);
                SetCustomProperty(turret, "target", VectorToString(target));
            }
        }
    }
}
