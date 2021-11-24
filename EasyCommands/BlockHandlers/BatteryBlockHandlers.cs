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
                AddBooleanHandler(Property.SUPPLY, b => b.ChargeMode != ChargeMode.Recharge, (b, v) => b.ChargeMode = (v ? ChargeMode.Auto : ChargeMode.Recharge));
                AddBooleanHandler(Property.AUTO, b => b.ChargeMode == ChargeMode.Auto, (b, v) => b.ChargeMode = (v ? ChargeMode.Auto : ChargeMode.Recharge));
                AddNumericHandler(Property.RANGE, b => b.MaxStoredPower);
                AddNumericHandler(Property.RATIO, b => b.CurrentStoredPower / b.MaxStoredPower);
                AddNumericHandler(Property.INPUT, b => b.CurrentInput);
                AddNumericHandler(Property.VOLUME, b => b.CurrentOutput);
                AddNumericHandler(Property.LEVEL, b => b.CurrentStoredPower);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RATIO;
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.SUPPLY;
                defaultPropertiesByDirection[Direction.UP] = Property.RATIO;
             }
        }
    }
}
