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
                AddBooleanHandler(Property.LOCKED, b => b.IsAimed, (b, v) => { if (!v) ResetTarget(b); });
                AddBooleanHandler(Property.AUTO, b => b.EnableIdleRotation, (b, v) => b.EnableIdleRotation = v);
                AddVectorHandler(Property.TARGET, GetTarget, SetTarget);
                AddVectorHandler(Property.TARGET_VELOCITY, b => b.GetTargetedEntity().Velocity, (b, v) => b.TrackTarget(GetTarget(b), v));
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
            }

            Vector3D GetTarget(IMyLargeTurretBase turret) {
                Vector3D? target = Vector3D.Zero;
                var customTarget = GetCustomProperty(turret, "target");
                if (customTarget != null) target = GetVector(customTarget);
                else if (turret.HasTarget) target = GetPosition(turret.GetTargetedEntity());
                return target.Value;
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
                AddPropertyHandler(Property.RANGE, TerminalBlockPropertyHandler("Range", 100));
                AddBooleanHandler(Property.TRIGGER, (b) => b.IsShooting, Shoot);
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.TRIGGER;
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RANGE;
                defaultPropertiesByDirection[Direction.UP] = Property.RANGE;

            }
            void Shoot(IMyUserControllableGun gun, bool b) { if(b) gun.ApplyAction("Shoot_On"); else gun.ApplyAction("Shoot_Off"); }
        }
    }
}
