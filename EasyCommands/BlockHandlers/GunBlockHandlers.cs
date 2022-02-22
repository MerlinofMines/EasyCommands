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
            public TurretBlockHandler() {
                AddBooleanHandler(Property.LOCKED, b => b.IsAimed, (b, v) => { if (!v) ResetTarget(b); });
                AddBooleanHandler(Property.AUTO, b => b.EnableIdleRotation, (b, v) => b.EnableIdleRotation = v);
                AddVectorHandler(Property.TARGET, GetTarget, SetTarget);
                AddVectorHandler(Property.TARGET_VELOCITY, b => b.GetTargetedEntity().Velocity, (b, v) => b.TrackTarget(GetTarget(b), v));
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
            }

            Vector3D GetTarget(IMyLargeTurretBase turret) =>
                GetVector(GetCustomProperty(turret, "target") ?? "") ?? (turret.HasTarget ? GetPosition(turret.GetTargetedEntity()) : Vector3D.Zero);

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
                AddPropertyHandler(Property.RANGE, TerminalPropertyHandler("Range", 100));
                AddBooleanHandler(Property.TRIGGER, b => b.IsShooting, Shoot);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RANGE;
                defaultPropertiesByDirection[Direction.UP] = Property.RANGE;
            }
            void Shoot(IMyUserControllableGun gun, bool b) =>
                PROGRAM.actionCache.GetOrCreate(gun.GetType(), b ? "Shoot_On" : "Shoot_Off", s => gun.GetActionWithName(s)).Apply(gun);
        }
    }
}