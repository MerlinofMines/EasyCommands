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
                AddBooleanHandler(PropertyType.PRODUCE, (b) => b.ChargeMode != ChargeMode.Recharge, (b, v) => b.ChargeMode = (v ? ChargeMode.Auto : ChargeMode.Recharge));
                AddBooleanHandler(PropertyType.AUTO, (b) => b.ChargeMode == ChargeMode.Auto, (b, v) => b.ChargeMode = (v ? ChargeMode.Auto : ChargeMode.Recharge));
                AddNumericHandler(PropertyType.RANGE, (b) => b.MaxStoredPower);
                AddNumericHandler(PropertyType.RATIO, (b) => b.CurrentStoredPower / b.MaxStoredPower);
                defaultPropertiesByPrimitive[PrimitiveType.NUMERIC] = PropertyType.RATIO;
                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.PRODUCE;
                defaultPropertiesByDirection[DirectionType.UP] = PropertyType.RATIO;
                defaultDirection = DirectionType.UP;
            }
        }
    }
}
