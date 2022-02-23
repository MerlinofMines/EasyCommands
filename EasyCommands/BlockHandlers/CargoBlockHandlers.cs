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
                AddNumericHandler(Property.WEIGHT, i => (float)i.CurrentMass);//Mass, in Kg
                AddStringHandler(Property.NAME, b => GetOwner(b).CustomName, (b, v) => GetOwner(b).CustomName = v);
                AddBooleanHandler(Property.SHOW, b => GetOwner(b).ShowInInventory, (b, v) => GetOwner(b).ShowInInventory = v);
                AddListHandler(Property.TYPES, b => {
                    var inventoryItems = NewList<MyInventoryItem>();
                    b.GetItems(inventoryItems);
                    return NewKeyedList(inventoryItems.Select(type => type.Type.TypeId + "." + type.Type.SubtypeId).Distinct().Select(GetStaticVariable));
                });

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.RATIO;
                defaultPropertiesByPrimitive[Return.STRING] = Property.NAME;
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.SHOW;
            }

            public override string Name(IMyInventory block) => GetOwner(block).CustomName;
            public override IEnumerable<IMyInventory> GetInstances(IMyTerminalBlock block) => Range(0, block.InventoryCount).Select(block.GetInventory);

            PropertyHandler<IMyInventory> amountHandler = new PropertyHandler<IMyInventory> {
                Get = (b, p) => {
                    var itemString = CastString(p.attributeValue.GetValue());
                    var filter = PROGRAM.AnyItem(PROGRAM.GetItemFilters(itemString));
                    double totalAmount = 0;
                    var items = NewList<MyInventoryItem>();

                    b.GetItems(items, filter);

                    items.ForEach(item => totalAmount += item.Amount.RawValue);

                    //RawValues are returned in the microUnits, convert to units.
                    return ResolvePrimitive((float)(totalAmount / 1000000));
                }
            };

            IMyTerminalBlock GetOwner(IMyInventory inventory) => (IMyTerminalBlock)inventory.Owner;
        }
    }
}
