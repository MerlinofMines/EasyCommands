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
        public class GunBlockHandler<T> : FunctionalBlockHandler<T> where T : class, IMyUserControllableGun {
            public GunBlockHandler() {
                numericPropertyGetters.Add(NumericPropertyType.RANGE, GetRange);
                numericPropertySetters.Add(NumericPropertyType.RANGE, new SimpleNumericPropertySetter<T>(GetRange, SetRange, 100));
                booleanPropertySetters.Add(BooleanPropertyType.TRIGGER, Shoot);
                booleanPropertyGetters.Add(BooleanPropertyType.TRIGGER, (b) => b.IsShooting);
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.RANGE);
            }
            void Shoot(IMyUserControllableGun gun, bool b) { if(b) gun.ApplyAction("Shoot_On"); else gun.ApplyAction("Shoot_Off"); }
            float GetRange(IMyUserControllableGun gun) { return gun.GetValueFloat("Range"); }
            void SetRange(IMyUserControllableGun gun, float range) { gun.SetValueFloat("Range", range); }
        }
    }
}
