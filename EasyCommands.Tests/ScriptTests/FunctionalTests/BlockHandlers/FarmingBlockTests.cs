using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using VRage.Game;
using VRage.Game.Components.Interfaces;
using static IngameScript.Program;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class FarmingBlockTests {
        [TestMethod]
        public void FoodProcessorExtendsAssemblerWithoutNarrowingParent() {
            using (var test = new ScriptTest(@"turn on the ""Kitchen"" foodprocessor
tell the ""Kitchen"" foodprocessor to create ""kelp crisp""
print ""Kitchen"" foodprocessor progress
print ""Kitchen"" foodprocessor is running")) {
                var food = new Mock<IMyAssembler>();
                var ordinary = new Mock<IMyAssembler>();
                MockBlockDefinition(food, "FoodProcessor");
                MockBlockDefinition(ordinary, "LargeAssembler");
                test.MockBlocksOfType("Kitchen", food, ordinary);
                food.Setup(b => b.CurrentProgress).Returns(.5f);
                food.Setup(b => b.IsProducing).Returns(true);
                test.RunUntilDone();
                food.VerifySet(b => b.Enabled = true);
                ordinary.VerifySet(b => b.Enabled = It.IsAny<bool>(), Times.Never);
                food.Verify(b => b.AddQueueItem(MockBlueprint("Position0030_MealPack_KelpCrisp"), (VRage.MyFixedPoint)1));
                Assert.IsTrue(test.Logger.Contains("0.5"));
                Assert.IsTrue(test.Logger.Contains("True"));
                Assert.AreEqual(2, new AssemblerBlockHandler().SelectBlocks(NewList(food.Object, ordinary.Object)).Count);
                Assert.IsInstanceOfType(new FoodProcessorBlockHandler(), typeof(AssemblerBlockHandler));
            }
        }

        [TestMethod]
        public void IrrigationInheritsGeneratorRefillAndProductionStatus() {
            using (var test = new ScriptTest(@"turn on the ""Water"" irrigation
set ""Water"" irrigation auto to true
print ""Water"" irrigation is running")) {
                var irrigation = new Mock<IMyGasGenerator>();
                var oxygen = new Mock<IMyGasGenerator>();
                MockBlockDefinition(irrigation, "IrrigationSystem");
                MockBlockDefinition(oxygen, "LargeBlockOxygenGenerator");
                test.MockBlocksOfType("Water", irrigation, oxygen);
                irrigation.Setup(b => b.IsProducing).Returns(true);
                test.RunUntilDone();
                irrigation.VerifySet(b => b.AutoRefill = true);
                irrigation.VerifySet(b => b.Enabled = true);
                oxygen.VerifySet(b => b.Enabled = It.IsAny<bool>(), Times.Never);
                Assert.IsTrue(test.Logger.Contains("True"));
                Assert.AreEqual(2, new GasGeneratorHandler().SelectBlocks(NewList(irrigation.Object, oxygen.Object)).Count);
                Assert.IsInstanceOfType(new IrrigationBlockHandler(), typeof(GasGeneratorHandler));
            }
        }

        [DataTestMethod]
        [DataRow(false, false, false, false)]
        [DataRow(true, true, false, false)]
        [DataRow(true, true, true, true)]
        [DataRow(true, false, false, true)]
        public void CropStatesRemainDistinctFromStructuralCompletion(bool planted, bool alive, bool mature, bool harvestable) {
            using (var test = new ScriptTest(@"print ""Bed"" farmplot occupied
print ""Bed"" farmplot crop alive
print ""Bed"" farmplot crop complete
print ""Bed"" farmplot ready
print ""Bed"" farmplot complete
print ""Bed"" farmplot crop")) {
                var plot = new Mock<IMyFunctionalBlock>();
                var crop = new Mock<IMyFarmPlotLogic>();
                var components = new Mock<IMyEntityComponentContainer>();
                var cropObject = crop.Object;
                components.Setup(c => c.TryGet(out cropObject)).Returns(true);
                plot.Setup(b => b.Components).Returns(components.Object);
                MockBlockDefinition(plot, "LargeBlockFarmPlot");
                MockBuildIntegrity(plot, 100, 100, 0);
                crop.Setup(c => c.IsPlantPlanted).Returns(planted);
                crop.Setup(c => c.IsAlive).Returns(alive);
                crop.Setup(c => c.IsPlantFullyGrown).Returns(mature);
                crop.Setup(c => c.IsHarvestable).Returns(harvestable);
                crop.Setup(c => c.OutputItem).Returns(new MyDefinitionId(typeof(MyObjectBuilder_ConsumableItem), "Fruit"));
                test.MockBlocksOfType("Bed", plot);
                test.RunUntilDone();
                CollectionAssert.AreEqual(new[] { planted.ToString(), (planted && alive).ToString(), mature.ToString(), harvestable.ToString(), "True", planted ? "MyObjectBuilder_ConsumableItem.Fruit" : "" }, test.Logger);
            }
        }

        [DataTestMethod]
        [DataRow(0d)]
        [DataRow(.5d)]
        [DataRow(1d)]
        public void WaterUsesStorageComponentAndCannotBeWritten(double ratio) {
            using (var test = new ScriptTest(@"print ""Bed"" farmplot percentage
print ""Bed"" farmplot capacity
print ""Bed"" farmplot level
set ""Bed"" farmplot level to 999
print ""Bed"" farmplot level")) {
                var plot = new Mock<IMyFunctionalBlock>();
                var water = new Mock<IMyResourceStorageComponent>();
                var components = new Mock<IMyEntityComponentContainer>();
                var storage = water.Object;
                components.Setup(c => c.TryGet(out storage)).Returns(true);
                plot.Setup(b => b.Components).Returns(components.Object);
                water.Setup(w => w.IsResourceStorage(It.IsAny<MyDefinitionId>())).Returns(true);
                water.Setup(w => w.ResourceCapacity).Returns(180);
                water.Setup(w => w.FilledRatio).Returns(ratio);
                MockBlockDefinition(plot, "LargeBlockFarmPlot");
                test.MockBlocksOfType("Bed", plot);
                test.RunUntilDone();
                CollectionAssert.AreEqual(new[] { ratio.ToString(), "180", (ratio * 180).ToString(), (ratio * 180).ToString() }, test.Logger);
            }
        }

        [TestMethod]
        public void MissingAndWrongComponentsFailClearly() {
            var handler = new FarmPlotBlockHandler();
            var plot = new Mock<IMyFunctionalBlock>();
            var property = new PropertySupplier(NewList(new PropertyValue(Property.RATIO + "")));
            StringAssert.Contains(Assert.ThrowsException<RuntimeException>(() => handler.GetPropertyValue(plot.Object, property)).Message, "missing component");
            var water = new Mock<IMyResourceStorageComponent>();
            var components = new Mock<IMyEntityComponentContainer>();
            var storage = water.Object;
            components.Setup(c => c.TryGet(out storage)).Returns(true);
            plot.Setup(b => b.Components).Returns(components.Object);
            StringAssert.Contains(Assert.ThrowsException<RuntimeException>(() => handler.GetPropertyValue(plot.Object, property)).Message, "no water storage");
        }

        [DataTestMethod]
        [DataRow("LargeBlockAlgaeFarm")]
        [DataRow("LargeBlockAlgaeFarmReskin")]
        public void AlgaeVariantsShareOutputAndCountdown(string subtype) {
            using (var test = new ScriptTest(@"print ""Farm"" algaefarm output
print ""Farm"" algaefarm rate
print ""Farm"" algaefarm countdown")) {
                var block = new Mock<IMyFunctionalBlock>();
                var food = new Mock<IMySolarFoodGenerator>();
                var components = new Mock<IMyEntityComponentContainer>();
                var generator = food.Object;
                components.Setup(c => c.TryGet(out generator)).Returns(true);
                block.Setup(b => b.Components).Returns(components.Object);
                MockBlockDefinition(block, subtype);
                food.Setup(c => c.ItemsPerMinute).Returns(2);
                food.Setup(c => c.TimeRemainingUntilNextBatch).Returns(15);
                test.MockBlocksOfType("Farm", block);
                test.RunUntilDone();
                CollectionAssert.AreEqual(new[] { "2", "2", "15" }, test.Logger);
            }
        }

        [TestMethod]
        public void GroupSelectionExcludesUnrelatedFunctionalBlocks() {
            using (var test = new ScriptTest(@"turn on the ""Garden"" algaefarms")) {
                var algae = new Mock<IMyFunctionalBlock>();
                var plot = new Mock<IMyFunctionalBlock>();
                MockBlockDefinition(algae, "LargeBlockAlgaeFarmReskin");
                MockBlockDefinition(plot, "LargeBlockFarmPlot");
                test.MockBlocksInGroup("Garden", algae, plot);
                test.RunUntilDone();
                algae.VerifySet(b => b.Enabled = true);
                plot.VerifySet(b => b.Enabled = It.IsAny<bool>(), Times.Never);
            }
        }

        [TestMethod]
        public void OxygenFarmsUseTheirInterfaceWithoutSubtypeRestrictions() {
            using (var test = new ScriptTest(@"print ""Oxygen"" oxygenfarm ready")) {
                var farm = new Mock<IMyOxygenFarm>();
                farm.Setup(b => b.CanProduce).Returns(true);
                test.MockBlocksOfType("Oxygen", farm);
                test.RunUntilDone();
                CollectionAssert.AreEqual(new[] { "True" }, test.Logger);
            }
        }
    }
}
