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

// ڙ (\u0699) is next symbol

namespace IngameScript {
    partial class Program {

        public class ProductionBlockHandler<T> : FunctionalBlockHandler<T> where T : class, IMyProductionBlock {
            public ProductionBlockHandler() {

                AddBooleanHandler(Property.COMPLETE, b => b.IsQueueEmpty, (b, v) => { if (!v) b.ClearQueue(); });
                AddBooleanHandler(Property.BUILD, b => !b.IsQueueEmpty, (b, v) => { if (!v) b.ClearQueue(); });

		AddBooleanHandler(Property.PRODUCING, b => b.IsProducing)

                // TODO: if supplied with no argument, return <IMyProductionBlock>.IsProducing
                AddPropertyHandler(Property.CREATE, new PropertyHandler<IMyProductionBlock>() {
                    Get = (b, p) => ResolvePrimitive(GetProducingAmount(b, p) >= GetRequestedAttributeOrPropertyValue(p, 1f)),
                    Set = (b, p, v) => AddQueueItem(b, p)
                });
                AddPropertyHandler(Property.AMOUNT, new PropertyHandler<IMyProductionBlock>() {
                    Get = (b, v) => ResolvePrimitive(GetProducingAmount(b, v)),
                    Set = (b, p, v) => AddQueueItem(b, p)
                });
                AddListHandler(Property.TYPES, b => {
                    var currentItems = NewList<MyProductionItem>();
                    b.GetQueue(currentItems);
                    return NewKeyedList(currentItems.Select(item => item.BlueprintId.SubtypeId + "").Distinct().Select(GetStaticVariable));
                });
            }

            //Returns the value of first attribute or property variable with the same return type as default value,
            //otherwise the default value. Attributes are checked first.
            protected T GetRequestedAttributeOrPropertyValue<T>(PropertySupplier supplier, T defaultValue) {
                Object value = supplier.propertyValues.Where(p => p.attributeValue != null)
                    .Select(p => p.attributeValue.GetValue())
                    .Concat(NewList(supplier.propertyValue?.GetValue() ?? ResolvePrimitive(defaultValue)))
                    .Concat(NewList(ResolvePrimitive(defaultValue)))
                    .FirstOrDefault(v => v.returnType == ResolvePrimitive(defaultValue).returnType).value;

                return (T)value;
            }

            protected float GetProducingAmount(IMyProductionBlock b, PropertySupplier p) {
                var definitions = PROGRAM.GetItemBluePrints(GetRequestedAttributeOrPropertyValue(p, "*"));
                var currentItems = NewList<MyProductionItem>();
                b.GetQueue(currentItems);
                MyFixedPoint value = currentItems
                    .Where(item => definitions.Contains(item.BlueprintId))
                    .Select(item => item.Amount)
                    .DefaultIfEmpty(MyFixedPoint.Zero)
                    .Aggregate((sum, val) => sum + val);
                return (float)value;
            }

            protected void AddQueueItem(IMyProductionBlock b, PropertySupplier p) {
                float amount = GetRequestedAttributeOrPropertyValue(p, 1f);
                foreach (MyDefinitionId bp in PROGRAM.GetItemBluePrints(GetRequestedAttributeOrPropertyValue(p, "*"))) {
                    try { b.AddQueueItem(bp, (MyFixedPoint)amount); } catch (Exception) {
                        throw new RuntimeException("Unknown BlueprintId: " + bp.SubtypeId);
                    }
                }
            }
		}

        public class AssemblerBlockHandler : ProductionBlockHandler<IMyAssembler> {
            public AssemblerBlockHandler() {
                AddBooleanHandler(Property.SUPPLY, b => b.Mode == MyAssemblerMode.Assembly, (b, v) => b.Mode = v ? MyAssemblerMode.Assembly : MyAssemblerMode.Disassembly);
                AddBooleanHandler(Property.AUTO, b => b.CooperativeMode, (b, v) => b.CooperativeMode = v);
                AddPropertyHandler(Property.CREATE, new PropertyHandler<IMyAssembler>() {
                    Get = (b, p) => ResolvePrimitive(b.Mode == MyAssemblerMode.Assembly && GetProducingAmount(b, p) >= GetRequestedAttributeOrPropertyValue(p, 1f)),
                    Set = (b, p, v) => { b.Mode = MyAssemblerMode.Assembly; AddQueueItem(b, p); }
                });
                AddPropertyHandler(Property.DESTROY, new PropertyHandler<IMyAssembler>() {
                    Get = (b, p) => ResolvePrimitive(b.Mode == MyAssemblerMode.Disassembly && GetProducingAmount(b, p) >= GetRequestedAttributeOrPropertyValue(p, 1f)),
                    Set = (b, p, v) => { b.Mode = MyAssemblerMode.Disassembly; AddQueueItem(b, p); }
                });
            }
        }
    }
}
