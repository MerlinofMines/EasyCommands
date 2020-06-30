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
        public class BatteryBlockHandler : FunctionalBlockHandler<IMyBatteryBlock> {
            public BatteryBlockHandler() {
                booleanPropertyGetters.Add(BooleanPropertyType.PRODUCE, (b) => b.ChargeMode != ChargeMode.Recharge);
                booleanPropertyGetters.Add(BooleanPropertyType.AUTO, (b) => b.ChargeMode == ChargeMode.Auto);
                booleanPropertySetters.Add(BooleanPropertyType.PRODUCE, (b, v) => b.ChargeMode = (v ? ChargeMode.Auto : ChargeMode.Recharge));
                booleanPropertySetters.Add(BooleanPropertyType.AUTO, (b, v) => b.ChargeMode = (v ? ChargeMode.Auto : ChargeMode.Recharge));
                numericPropertyGetters.Add(NumericPropertyType.RANGE, (b) => b.MaxStoredPower);
                numericPropertyGetters.Add(NumericPropertyType.RATIO, (b) => b.CurrentStoredPower / b.MaxStoredPower);
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.RATIO);
                defaultDirection = DirectionType.UP;
            }
        }
    }
}
