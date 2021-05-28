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
        public class AssemblerBlockHandler : FunctionalBlockHandler<IMyAssembler> {
            public AssemblerBlockHandler() {
                AddBooleanHandler(Property.SUPPLY, b => b.Mode == MyAssemblerMode.Assembly, (b, v) => b.Mode = v ? MyAssemblerMode.Assembly : MyAssemblerMode.Disassembly);
                AddBooleanHandler(Property.COMPLETE, b => b.IsQueueEmpty, (b,v) => { if (!v) b.ClearQueue(); });
                AddBooleanHandler(Property.AUTO, b => b.CooperativeMode, (b, v) => b.CooperativeMode = v);
                AddPropertyHandler(ValueProperty.CREATE, new PropertyHandler<IMyAssembler>() {
                    Get = (b, p) => new BooleanPrimitive(GetProducingAmount(b, p) > 0),
                    Set = (b, p, v) => { b.Mode = MyAssemblerMode.Assembly; AddQueueItem(b, p.valueAttribute.GetValue(), v); }
                }) ;
                AddPropertyHandler(ValueProperty.DESTROY, new PropertyHandler<IMyAssembler>() {
                    Get = (b, p) => new BooleanPrimitive(GetProducingAmount(b, p) > 0),
                    Set = (b, p, v) => { b.Mode = MyAssemblerMode.Disassembly; AddQueueItem(b, p.valueAttribute.GetValue(), v); }
                });

                AddPropertyHandler(ValueProperty.AMOUNT, new PropertyHandler<IMyAssembler>() {
                    Get = (b, v) => new NumberPrimitive(GetProducingAmount(b, v)),
                    Set = (b, p, v) => AddQueueItem(b, p.valueAttribute.GetValue(), v)
                });
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.COMPLETE;
            }

            float GetProducingAmount(IMyAssembler b, PropertySupplier p) {
                var definitions = PROGRAM.GetItemBluePrints(CastString(p.valueAttribute.GetValue()).GetStringValue());
                var currentItems = new List<MyProductionItem>();
                b.GetQueue(currentItems);
                MyFixedPoint value = currentItems.Where(item => definitions.Contains(item.BlueprintId)).Select(item => item.Amount).Aggregate((sum, val) => sum + val);
                return (float)value;
            }

            void AddQueueItem(IMyAssembler b, Primitive v1, Primitive v2) {
                string itemString;
                if (v1.GetPrimitiveType() == Return.STRING) itemString = CastString(v1).GetStringValue();
                else itemString = CastString(v2).GetStringValue();

                float amount = 1f;
                if (v1.GetPrimitiveType() == Return.NUMERIC) {
                    amount = CastNumber(v1).GetNumericValue();
                } else if (v2.GetPrimitiveType() == Return.NUMERIC) {
                    amount = CastNumber(v2).GetNumericValue();
                }

                List<MyDefinitionId> blueprints = PROGRAM.GetItemBluePrints(itemString);

                foreach(MyDefinitionId bp in blueprints) {
                    Print("Found Blueprint: " + bp);
                    b.AddQueueItem(bp, (MyFixedPoint)amount);
                }
            }
        }
    }
}
 