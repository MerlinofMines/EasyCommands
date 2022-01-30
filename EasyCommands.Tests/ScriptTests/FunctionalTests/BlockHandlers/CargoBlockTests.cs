using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class CargoBlockTests {
        [TestMethod]
        public void getCargoCapacity() {
            String script = @"
assign ""a"" to ""mock cargo"" capacity
print ""Cargo Capacity: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);

                mockInventory.Setup(i => i.MaxVolume).Returns((MyFixedPoint)0.1);

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Cargo Capacity: 100", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getCargoVolume() {
            String script = @"
assign ""a"" to ""mock cargo"" volume
print ""Cargo Volume: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);

                mockInventory.Setup(i => i.CurrentVolume).Returns((MyFixedPoint)0.1);

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Cargo Volume: 100", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getCargoWeight() {
            String script = @"
assign ""a"" to ""mock cargo"" weight
print ""Cargo Weight: "" + a + ""Kg""
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);

                mockInventory.Setup(i => i.CurrentMass).Returns(200);

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Cargo Weight: 200Kg", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getCargoRatio() {
            String script = @"
assign ""a"" to ""mock cargo"" ratio
print ""Cargo Ratio: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);

                mockInventory.Setup(i => i.CurrentVolume).Returns(50);
                mockInventory.Setup(i => i.MaxVolume).Returns(100);

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Cargo Ratio: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getCargoAmount() {
             String script = @"
assign ""a"" to ""mock cargo"" ""ore"" amount
print ""Ore Amount: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);

                MockInventoryItems(mockInventory, MockOre("Iron", 200), MockOre("Stone", 100));

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Ore Amount: 300", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getCargoTypes() {
            String script = @"
print ""Types: "" + ""mock cargo"" types
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);

                MockInventoryItems(mockInventory, MockOre("Iron", 200), MockOre("Stone", 100), MockOre("Iron", 100));

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Types: [MyObjectBuilder_Ore.Iron,MyObjectBuilder_Ore.Stone]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetCargoCustomItemAmount() {
            String script = @"
assign ""a"" to ""mock cargo"" ""CustomItem"" amount
print ""Custom Item Amount: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);

                MockInventoryItems(mockInventory, MockOre("Iron", 200), MockOre("Stone", 100), MockConsumable("CustomItem", 400));

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Custom Item Amount: 400", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetCargoCustomItemTypeAmount() {
            String script = @"
assign ""a"" to ""mock cargo"" ""MyObjectBuilder_Ore."" amount
print ""Custom Type Amount: "" + a
";
            using (var test = new ScriptTest(script)) {
                var ore = MockOre("Iron", 200);

                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);

                MockInventoryItems(mockInventory, MockOre("Iron", 200), MockOre("Stone", 100), MockConsumable("CustomItem", 400));

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Custom Type Amount: 300", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getCargoAmountFromMultipleInventoriesOnSameBlock() {
            String script = @"
assign ""a"" to ""mock cargo"" inventories ""ore"" amount
print ""Ore Amount: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory1 = new Mock<IMyInventory>();
                var mockInventory2 = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory1, mockInventory2);

                MockInventoryItems(mockInventory1, MockOre("Iron", 200), MockOre("Stone", 100));
                MockInventoryItems(mockInventory2, MockOre("Iron", 100), MockOre("Stone", 50));

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Ore Amount: 450", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getCargoAmountFromSpecificInventoryOfBlockWithMultipleInventories() {
            String script = @"
assign ""a"" to ""mock cargo"" inventories[1] ""ore"" amount
print ""Ore Amount: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory1 = new Mock<IMyInventory>();
                var mockInventory2 = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory1, mockInventory2);

                MockInventoryItems(mockInventory1, MockOre("Iron", 200), MockOre("Stone", 100));
                MockInventoryItems(mockInventory2, MockOre("Iron", 100), MockOre("Stone", 50));

                test.MockBlocksOfType("mock cargo", mockContainer);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Ore Amount: 150", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getCargoAmountFromBlockGroupInventories() {
            String script = @"
assign ""a"" to ""mock cargo"" group inventories ""ore"" amount
print ""Ore Amount: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockContainer1 = new Mock<IMyCargoContainer>();
                var mockContainer2 = new Mock<IMyCargoContainer>();
                var mockInventory1 = new Mock<IMyInventory>();
                var mockInventory2 = new Mock<IMyInventory>();
                MockInventories(mockContainer1, mockInventory1);
                MockInventories(mockContainer2, mockInventory2);

                MockInventoryItems(mockInventory1, MockOre("Iron", 200), MockOre("Stone", 100));
                MockInventoryItems(mockInventory2, MockOre("Iron", 100), MockOre("Stone", 50));

                test.MockBlocksInGroup("mock cargo", mockContainer1, mockContainer2);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Ore Amount: 450", test.Logger[0]);
            }
        }

        [TestMethod]
        public void HideCargoInventory() {
            String script = @"
hide the ""mock cargo"" inventory
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);
                test.MockBlocksOfType("mock cargo", mockContainer);

                test.RunUntilDone();

                mockContainer.VerifySet(b => b.ShowInInventory = false);
            }
        }

        [TestMethod]
        public void ShowCargoInventory() {
            String script = @"
show the ""mock cargo"" inventory
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);
                test.MockBlocksOfType("mock cargo", mockContainer);

                test.RunUntilDone();

                mockContainer.VerifySet(b => b.ShowInInventory = true);
            }
        }

        [TestMethod]
        public void SetCargoBlockName() {
            String script = @"
set the ""mock cargo"" name to 'cargo block 1'
";
            using (var test = new ScriptTest(script)) {
                var mockContainer = new Mock<IMyCargoContainer>();
                var mockInventory = new Mock<IMyInventory>();
                MockInventories(mockContainer, mockInventory);
                test.MockBlocksOfType("mock cargo", mockContainer);

                test.RunUntilDone();

                mockContainer.VerifySet(b => b.CustomName = "cargo block 1");
            }
        }
    }
}
