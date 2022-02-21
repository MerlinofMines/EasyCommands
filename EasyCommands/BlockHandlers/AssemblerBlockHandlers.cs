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
                    Get = (b, p) => ResolvePrimitive(b.Mode == MyAssemblerMode.Assembly && GetProducingAmount(b, p) >= GetRequestedAmount(p)),
                    Set = (b, p, v) => { b.Mode = MyAssemblerMode.Assembly; AddQueueItem(b, p); }
                }) ;
                AddPropertyHandler(ValueProperty.DESTROY, new PropertyHandler<IMyAssembler>() {
                    Get = (b, p) => ResolvePrimitive(b.Mode == MyAssemblerMode.Disassembly && GetProducingAmount(b, p) >= GetRequestedAmount(p)),
                    Set = (b, p, v) => { b.Mode = MyAssemblerMode.Disassembly; AddQueueItem(b, p); }
                });

                AddPropertyHandler(ValueProperty.AMOUNT, new PropertyHandler<IMyAssembler>() {
                    Get = (b, v) => ResolvePrimitive(GetProducingAmount(b, v)),
                    Set = (b, p, v) => AddQueueItem(b, p)
                });

                AddListHandler(Property.TYPES, b => {
                    var currentItems = NewList<MyProductionItem>();
                    b.GetQueue(currentItems);
                    return NewKeyedList(currentItems.Select(item => item.BlueprintId.SubtypeId + "").Distinct().Select(GetStaticVariable));
                });
            }

            float GetRequestedAmount(PropertySupplier p) => CastNumber(NewList(p.attributeValue.GetValue(), (p.propertyValue ?? GetStaticVariable(1)).GetValue()).Find(v => v.returnType == Return.NUMERIC) ?? ResolvePrimitive(1));
            string GetRequestedItemFilter(PropertySupplier p) => CastString(NewList(p.attributeValue.GetValue(), (p.propertyValue ?? GetStaticVariable("*")).GetValue()).Find(v => v.returnType == Return.STRING) ?? ResolvePrimitive("*"));

            float GetProducingAmount(IMyAssembler b, PropertySupplier p) {
                var definitions = PROGRAM.GetItemBluePrints(GetRequestedItemFilter(p));
                var currentItems = NewList<MyProductionItem>();
                b.GetQueue(currentItems);
                MyFixedPoint value = currentItems
                    .Where(item => definitions.Contains(item.BlueprintId))
                    .Select(item => item.Amount)
                    .Aggregate(MyFixedPoint.Zero, (sum, val) => sum + val);
                return (float)value;
            }

            void AddQueueItem(IMyAssembler b, PropertySupplier p) {
                float amount = GetRequestedAmount(p);
                foreach(MyDefinitionId bp in PROGRAM.GetItemBluePrints(GetRequestedItemFilter(p))) {
                    try { b.AddQueueItem(bp, (MyFixedPoint)amount); } catch (Exception) {
                        throw new Exception("Unknown BlueprintId: " + bp.SubtypeId);
                    }
                }
            }
        }
    }
}
