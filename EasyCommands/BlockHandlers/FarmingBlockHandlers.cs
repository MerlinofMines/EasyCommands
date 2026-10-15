using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game;

namespace IngameScript {
    partial class Program {
        // Keep the block as the selected object: crop/storage interfaces are components.
        static T RequireComponent<T>(IMyTerminalBlock block) {
            T component;
            if (block.Components != null && block.Components.TryGet(out component)) return component;
            throw new RuntimeException(block.CustomName + " is missing component " + typeof(T).Name);
        }

        public class FoodProcessorBlockHandler : AssemblerBlockHandler {
            public override IEnumerable<IMyAssembler> SelectBlocksByType<U>(List<U> blocks, Func<U, bool> selector = null) =>
                base.SelectBlocksByType(blocks, selector).Where(b => b.BlockDefinition.SubtypeId == "FoodProcessor");
        }

        public class IrrigationBlockHandler : GasGeneratorHandler {
            public override IEnumerable<IMyGasGenerator> SelectBlocksByType<U>(List<U> blocks, Func<U, bool> selector = null) =>
                base.SelectBlocksByType(blocks, selector).Where(b => b.BlockDefinition.SubtypeId == "IrrigationSystem");
        }

        public class FarmPlotBlockHandler : SubTypedBlockHandler<IMyFunctionalBlock> {
            public FarmPlotBlockHandler() : base(b => b.BlockDefinition.SubtypeId == "LargeBlockFarmPlot") {
                AddBooleanHandler(Property.USE, b => Crop(b).IsPlantPlanted);
                AddBooleanHandler(Property.ABLE, b => Crop(b).IsHarvestable);
                AddPropertyHandler(NewList(Property.CROP, Property.COMPLETE), BooleanHandler(b => Crop(b).IsPlantFullyGrown));
                AddPropertyHandler(NewList(Property.CROP, Property.ALIVE), BooleanHandler(b => Crop(b).IsPlantPlanted && Crop(b).IsAlive));
                AddStringHandler(Property.CROP, b => {
                    var crop = Crop(b);
                    return crop.IsPlantPlanted ? crop.OutputItem.TypeId + "." + crop.OutputItem.SubtypeId : "";
                });
                AddNumericHandler(Property.RATIO, b => (float)Water(b).FilledRatio);
                AddNumericHandler(Property.RANGE, b => Water(b).ResourceCapacity);
                AddNumericHandler(Property.LEVEL, b => (float)Water(b).FilledRatio * Water(b).ResourceCapacity);
                SetDefaultProperty(Property.RATIO, Return.NUMERIC, Direction.UP);
            }

            IMyFarmPlotLogic Crop(IMyFunctionalBlock block) => RequireComponent<IMyFarmPlotLogic>(block);
            IMyResourceStorageComponent Water(IMyFunctionalBlock block) {
                var water = RequireComponent<IMyResourceStorageComponent>(block);
                if (!water.IsResourceStorage(new MyDefinitionId(typeof(VRage.Game.ObjectBuilders.Definitions.MyObjectBuilder_GasProperties), "Water")))
                    throw new RuntimeException(block.CustomName + " has no water storage");
                return water;
            }
        }

        public class AlgaeFarmBlockHandler : SubTypedBlockHandler<IMyFunctionalBlock> {
            public AlgaeFarmBlockHandler() : base(b => b.BlockDefinition.SubtypeId == "LargeBlockAlgaeFarm" || b.BlockDefinition.SubtypeId == "LargeBlockAlgaeFarmReskin") {
                AddPropertyAliases(NumericHandler(b => RequireComponent<IMySolarFoodGenerator>(b).ItemsPerMinute), Property.VOLUME, Property.VELOCITY);
                AddNumericHandler(Property.COUNTDOWN, b => RequireComponent<IMySolarFoodGenerator>(b).TimeRemainingUntilNextBatch);
                SetDefaultProperty(Property.VOLUME, Return.NUMERIC, Direction.UP);
            }
        }

        public class OxygenFarmBlockHandler : FunctionalBlockHandler<IMyOxygenFarm> {
            public OxygenFarmBlockHandler() {
                AddBooleanHandler(Property.ABLE, b => b.CanProduce);
            }
        }
    }
}
