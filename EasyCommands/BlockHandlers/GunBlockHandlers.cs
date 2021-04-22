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
        public class TurretBlockHandler<T> : GunBlockHandler<IMyLargeTurretBase> {
            public TurretBlockHandler() : base() {
                AddBooleanHandler(PropertyType.LOCKED, b => b.IsAimed, (b, v) => { if (!v) ResetTarget(b); });
                AddBooleanHandler(PropertyType.AUTO, b => b.EnableIdleRotation, (b, v) => b.EnableIdleRotation = v);
                AddVectorHandler(PropertyType.TARGET, GetTarget, SetTarget);
                AddVectorHandler(PropertyType.TARGET_VELOCITY, b => b.GetTargetedEntity().Velocity, (b, v) => b.TrackTarget(GetTarget(b), v));
                defaultPropertiesByPrimitive[PrimitiveType.VECTOR] = PropertyType.TARGET;
            }

            Vector3D GetTarget(IMyLargeTurretBase turret) {
                Vector3D target = Vector3D.Zero;
                var customTarget = GetCustomProperty(turret, "target");
                if (customTarget == null || !GetVector(customTarget, out target)) {
                    if (turret.HasTarget) {
                        target = GetPosition(turret.GetTargetedEntity());
                    } else {
                        target = Vector3D.Zero;
                    }
                }
                return target;
            }

            void ResetTarget(IMyLargeTurretBase turret) {
                //Idle Movement setting gets reset when calling ResetTargetingToDefault(), so need to re-apply it.
                bool idleMovement = turret.EnableIdleRotation;
                turret.ResetTargetingToDefault();
                DeleteCustomProperty(turret, "target");
                turret.EnableIdleRotation = idleMovement;
            }

            void SetTarget(IMyLargeTurretBase turret, Vector3D target) {
                turret.SetTarget(target);
                SetCustomProperty(turret, "target", VectorToString(target));
            }
        }

        public class GunBlockHandler<T> : FunctionalBlockHandler<T> where T : class, IMyUserControllableGun {
            public GunBlockHandler() {
                AddNumericHandler(PropertyType.RANGE, GetRange, SetRange, 100);
                AddBooleanHandler(PropertyType.TRIGGER, (b) => b.IsShooting, Shoot);
                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.TRIGGER;
                defaultPropertiesByPrimitive[PrimitiveType.NUMERIC] = PropertyType.RANGE;
                defaultPropertiesByDirection[DirectionType.UP] = PropertyType.RANGE;
                defaultDirection = DirectionType.UP;
            }
            void Shoot(IMyUserControllableGun gun, bool b) { if(b) gun.ApplyAction("Shoot_On"); else gun.ApplyAction("Shoot_Off"); }
            float GetRange(IMyUserControllableGun gun) { return gun.GetValueFloat("Range"); }
            void SetRange(IMyUserControllableGun gun, float range) { gun.SetValueFloat("Range", range); }
        }
    }
}
