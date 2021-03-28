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
        public class TurretBlockHandler<T> : GunBlockHandler<T> where T : class, IMyLargeTurretBase {
            public TurretBlockHandler() : base() {
                AddVectorHandler(PropertyType.TARGET, b => b.GetTargetedEntity().Position, (b, v) => b.SetTarget(v));
                defaultPropertiesByPrimitive[PrimitiveType.VECTOR] = PropertyType.TARGET;
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
