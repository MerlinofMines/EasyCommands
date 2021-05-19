using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public class CargoHandler : MultiInstanceBlockHandler<IMyInventory> {
            public CargoHandler() {
                AddPropertyHandler(ValueProperty.AMOUNT, amountHandler);
                AddNumericHandler(Property.RATIO, i => (float)(i.CurrentVolume.RawValue / (double)i.MaxVolume.RawValue));
                AddNumericHandler(Property.RANGE, i => (float)i.MaxVolume * 1000); //Volumes are returned in kL, convert to L
                AddNumericHandler(Property.VOLUME, i => (float)i.CurrentVolume * 1000); //Volumes are returned in kL, convert to L
            }

            public override void GetInstances(IMyTerminalBlock block, List<IMyInventory> instances) {
                for (int i = 0; i < block.InventoryCount; i++) {
                    instances.Add(block.GetInventory(i));
                }
            }

            protected override string Name(IMyInventory block) => block.Owner.DisplayName;

            PropertyHandler<IMyInventory> amountHandler = new PropertyHandler<IMyInventory> {
                Get = (b, p) => {
                    var itemString = CastString(p.valueAttribute.GetValue()).GetStringValue();
                    var filter = PROGRAM.AnyItem(PROGRAM.GetItemFilters(itemString));
                    double totalAmount = 0;
                    var items = new List<MyInventoryItem>();

                    b.GetItems(items, filter);

                    items.ForEach(item => totalAmount += item.Amount.RawValue);

                    //RawValues are returned in the microUnits, convert to units.
                    return new NumberPrimitive((float)(totalAmount / 1000000));
                }
            };
        }
    }
}
