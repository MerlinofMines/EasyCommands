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
print ""Cargo Capacity: "" + {a}
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
print ""Cargo Volume: "" + {a}
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
        public void getCargoRatio() {
            String script = @"
assign ""a"" to ""mock cargo"" ratio
print ""Cargo Ratio: "" + {a}
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
print ""Ore Amount: "" + {a}
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
