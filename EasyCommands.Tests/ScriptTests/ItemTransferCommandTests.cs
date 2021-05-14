using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game.ModAPI.Ingame;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ItemTransferCommandTests {
        [TestMethod]
        public void SimpleTransferAll() {
            var script = @"transfer ""ore"" from ""source cargo"" to ""destination cargo""";

            Mock<IMyInventory> sourceInventory = new Mock<IMyInventory>();
            Mock<IMyInventory> destinationInventory = new Mock<IMyInventory>();
            Mock<IMyCargoContainer> sourceContainer = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> destinationContainer = new Mock<IMyCargoContainer>();
            MockInventories(sourceContainer, sourceInventory);
            MockInventories(destinationContainer, destinationInventory);
            var ore = MockOre("Iron", 50);

            using (var test = new ScriptTest(script)) {
                test.MockBlocksOfType("source cargo", sourceContainer);
                test.MockBlocksOfType("destination cargo", destinationContainer);

                MockInventoryItems(sourceInventory, ore);
                destinationInventory.Setup(i => i.IsFull).Returns(false);

                MockTransfer(sourceInventory, destinationInventory, ore, MyFixedPoint.MaxValue);

                test.RunUntilDone();

                sourceInventory.Verify(i => i.TransferItemTo(destinationInventory.Object, ore, MyFixedPoint.MaxValue));
            }
        }

        [TestMethod]
        public void SimpleTransferSpecificAmount() {
            var script = @"transfer 50 ""ore"" from ""source cargo"" to ""destination cargo""";

            Mock<IMyInventory> sourceInventory = new Mock<IMyInventory>();
            Mock<IMyInventory> destinationInventory = new Mock<IMyInventory>();
            Mock<IMyCargoContainer> sourceContainer = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> destinationContainer = new Mock<IMyCargoContainer>();
            MockInventories(sourceContainer, sourceInventory);
            MockInventories(destinationContainer, destinationInventory);
            var ore = MockOre("Iron", 100);

            using (var test = new ScriptTest(script)) {
                test.MockBlocksOfType("source cargo", sourceContainer);
                test.MockBlocksOfType("destination cargo", destinationContainer);

                MockInventoryItems(sourceInventory, ore);
                destinationInventory.Setup(i => i.IsFull).Returns(false);

                MockTransfer(sourceInventory, destinationInventory, ore, 50);

                test.RunUntilDone();

                sourceInventory.Verify(i => i.TransferItemTo(destinationInventory.Object, ore, 50));
            }
        }

        [TestMethod]
        public void SimpleTransferAllFromMultipleSources() {
            var script = @"transfer ""ore"" from ""source cargo"" containers to ""destination cargo""";

            Mock<IMyInventory> sourceInventory1 = new Mock<IMyInventory>();
            Mock<IMyInventory> sourceInventory2 = new Mock<IMyInventory>();
            Mock<IMyInventory> destinationInventory = new Mock<IMyInventory>();
            Mock<IMyCargoContainer> sourceContainer1 = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> sourceContainer2 = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> destinationContainer = new Mock<IMyCargoContainer>();
            MockInventories(sourceContainer1, sourceInventory1);
            MockInventories(sourceContainer2, sourceInventory2);
            MockInventories(destinationContainer, destinationInventory);
            var ore = MockOre("Iron", 100);

            using (var test = new ScriptTest(script)) {
                test.MockBlocksInGroup("source cargo", sourceContainer1, sourceContainer2);
                test.MockBlocksOfType("destination cargo", destinationContainer);

                MockInventoryItems(sourceInventory1, ore);
                MockInventoryItems(sourceInventory2, ore);

                MockTransfer(sourceInventory1, destinationInventory, ore, MyFixedPoint.MaxValue, 100);
                MockTransfer(sourceInventory2, destinationInventory, ore, MyFixedPoint.MaxValue - 100, 100);

                destinationInventory.Setup(i => i.IsFull).Returns(false);

                test.RunUntilDone();

                sourceInventory1.Verify(i => i.TransferItemTo(destinationInventory.Object, ore, MyFixedPoint.MaxValue));
                sourceInventory2.Verify(i => i.TransferItemTo(destinationInventory.Object, ore, MyFixedPoint.MaxValue - 100));
            }
        }

        [TestMethod]
        public void SimpleTransferSpecificAmountFromMultipleSources() {
            var script = @"transfer 75 ""ore"" from ""source cargo"" containers to ""destination cargo""";

            Mock<IMyInventory> sourceInventory1 = new Mock<IMyInventory>();
            Mock<IMyInventory> sourceInventory2 = new Mock<IMyInventory>();
            Mock<IMyInventory> destinationInventory = new Mock<IMyInventory>();
            Mock<IMyCargoContainer> sourceContainer1 = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> sourceContainer2 = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> destinationContainer = new Mock<IMyCargoContainer>();
            MockInventories(sourceContainer1, sourceInventory1);
            MockInventories(sourceContainer2, sourceInventory2);
            MockInventories(destinationContainer, destinationInventory);
            var ore = MockOre("Iron", 50);

            using (var test = new ScriptTest(script)) {
                test.MockBlocksInGroup("source cargo", sourceContainer1, sourceContainer2);
                test.MockBlocksOfType("destination cargo", destinationContainer);

                MockInventoryItems(sourceInventory1, ore);
                MockInventoryItems(sourceInventory2, ore);

                MockTransfer(sourceInventory1, destinationInventory, ore, 75, 50);
                MockTransfer(sourceInventory2, destinationInventory, ore, 25);

                destinationInventory.Setup(i => i.IsFull).Returns(false);

                test.RunUntilDone();

                sourceInventory1.Verify(i => i.TransferItemTo(destinationInventory.Object, ore, 75));
                sourceInventory2.Verify(i => i.TransferItemTo(destinationInventory.Object, ore, 25));
            }
        }

        [TestMethod]
        public void ConsolidateInSingleContainerFromSources() {
            var script = @"transfer 75 ""ore"" from ""source cargo"" containers to ""source cargo"" containers @ 0";

            Mock<IMyInventory> sourceInventory1 = new Mock<IMyInventory>();
            Mock<IMyInventory> sourceInventory2 = new Mock<IMyInventory>();
            Mock<IMyCargoContainer> sourceContainer1 = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> sourceContainer2 = new Mock<IMyCargoContainer>();
            MockInventories(sourceContainer1, sourceInventory1);
            MockInventories(sourceContainer2, sourceInventory2);
            var ore = MockOre("Iron", 100);

            using (var test = new ScriptTest(script)) {
                test.MockBlocksInGroup("source cargo", sourceContainer1, sourceContainer2);

                MockInventoryItems(sourceInventory1, ore);
                MockInventoryItems(sourceInventory2, ore);

                MockTransfer(sourceInventory2, sourceInventory1, ore, 75);

                sourceInventory2.Setup(i => i.IsFull).Returns(false);

                test.RunUntilDone();

                sourceInventory1.Verify(i => i.TransferItemTo(sourceInventory1.Object, ore, 75), Times.Never);
                sourceInventory2.Verify(i => i.TransferItemTo(sourceInventory1.Object, ore, 75));
            }
        }

        [TestMethod]
        public void DoNotTransferToSelf() {
            var script = @"transfer 75 ""ore"" from ""source cargo"" to ""source cargo""";

            Mock<IMyInventory> sourceInventory = new Mock<IMyInventory>();
            Mock<IMyCargoContainer> sourceContainer = new Mock<IMyCargoContainer>();
            MockInventories(sourceContainer, sourceInventory);
            var ore = MockOre("Iron", 100);

            using (var test = new ScriptTest(script)) {
                test.MockBlocksOfType("source cargo", sourceContainer);

                MockInventoryItems(sourceInventory, ore);

                test.RunUntilDone();

                sourceInventory.Verify(i => i.TransferItemTo(sourceInventory.Object, ore, 75), Times.Never);
            }
        }

        [TestMethod]
        public void DoNotTransferToFullInventory() {
            var script = @"transfer 75 ""ore"" from ""source cargo"" to ""destination cargo""";

            Mock<IMyInventory> sourceInventory = new Mock<IMyInventory>();
            Mock<IMyInventory> destinationInventory = new Mock<IMyInventory>();
            Mock<IMyCargoContainer> sourceContainer = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> destinationContainer = new Mock<IMyCargoContainer>();
            MockInventories(sourceContainer, sourceInventory);
            MockInventories(destinationContainer, destinationInventory);
            var ore = MockOre("Iron", 100);

            using (var test = new ScriptTest(script)) {
                test.MockBlocksOfType("source cargo", sourceContainer);
                test.MockBlocksOfType("destination cargo", destinationContainer);

                MockInventoryItems(sourceInventory, ore);

                destinationInventory.Setup(i => i.IsFull).Returns(true);

                test.RunUntilDone();

                sourceInventory.Verify(i => i.TransferItemTo(destinationInventory.Object, ore, 75), Times.Never);
            }
        }

        [TestMethod]
        public void SplitRequestedTransferAmountAcrossMultipleContainers() {
            var script = @"transfer 75 ""ore"" from ""source cargo"" to ""destination cargo"" containers";

            Mock<IMyInventory> sourceInventory = new Mock<IMyInventory>();
            Mock<IMyInventory> destinationInventory1 = new Mock<IMyInventory>();
            Mock<IMyInventory> destinationInventory2 = new Mock<IMyInventory>();
            Mock<IMyCargoContainer> sourceContainer = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> destinationContainer1 = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> destinationContainer2 = new Mock<IMyCargoContainer>();
            MockInventories(sourceContainer, sourceInventory);
            MockInventories(destinationContainer1, destinationInventory1);
            MockInventories(destinationContainer2, destinationInventory2);
            var ore = MockOre("Iron", 100);

            using (var test = new ScriptTest(script)) {
                test.MockBlocksOfType("source cargo", sourceContainer);
                test.MockBlocksInGroup("destination cargo", destinationContainer1, destinationContainer2);

                MockInventoryItems(sourceInventory, ore);

                MockTransfer(sourceInventory, destinationInventory1, ore, 75, 50);
                MockTransfer(sourceInventory, destinationInventory2, ore, 25);

                destinationInventory1.Setup(i => i.IsFull).Returns(false);
                destinationInventory2.Setup(i => i.IsFull).Returns(false);

                test.RunUntilDone();

                sourceInventory.Verify(i => i.TransferItemTo(destinationInventory1.Object, ore, 75));
                sourceInventory.Verify(i => i.TransferItemTo(destinationInventory2.Object, ore, 25));
            }
        }

        [TestMethod]
        public void DoNotTransferMoreTimesThanMaxAmount() {
            var script = @"transfer 75 ""ore"" from ""source cargo"" to ""destination cargo"" containers";

            Mock<IMyInventory> sourceInventory = new Mock<IMyInventory>();
            Mock<IMyInventory> destinationInventory1 = new Mock<IMyInventory>();
            Mock<IMyInventory> destinationInventory2 = new Mock<IMyInventory>();
            Mock<IMyCargoContainer> sourceContainer = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> destinationContainer1 = new Mock<IMyCargoContainer>();
            Mock<IMyCargoContainer> destinationContainer2 = new Mock<IMyCargoContainer>();
            MockInventories(sourceContainer, sourceInventory);
            MockInventories(destinationContainer1, destinationInventory1);
            MockInventories(destinationContainer2, destinationInventory2);
            var ore = MockOre("Iron", 100);

            using (var test = new ScriptTest(script)) {
                test.program.maxItemTransfers = 1;
                test.MockBlocksOfType("source cargo", sourceContainer);
                test.MockBlocksInGroup("destination cargo", destinationContainer1, destinationContainer2);

                MockInventoryItems(sourceInventory, ore);

                MockTransfer(sourceInventory, destinationInventory1, ore, 75, 50);
                MockTransfer(sourceInventory, destinationInventory2, ore, 25);

                destinationInventory1.Setup(i => i.IsFull).Returns(false);
                destinationInventory2.Setup(i => i.IsFull).Returns(false);

                test.RunUntilDone();

                sourceInventory.Verify(i => i.TransferItemTo(destinationInventory1.Object, ore, 75));
                sourceInventory.Verify(i => i.TransferItemTo(destinationInventory2.Object, ore, 25), Times.Never);
            }
        }

        private void MockTransfer(Mock<IMyInventory> sourceInventory, Mock<IMyInventory> destinationInventory, MyInventoryItem item, MyFixedPoint transferredAmount) {
            MockTransfer(sourceInventory, destinationInventory, item, transferredAmount, transferredAmount);
        }

        private void MockTransfer(Mock<IMyInventory> sourceInventory, Mock<IMyInventory> destinationInventory, MyInventoryItem item, MyFixedPoint requestedAmount, MyFixedPoint transferredAmount) {
            var currentMass = sourceInventory.Object.CurrentMass;

            sourceInventory.Setup(inventory => inventory.TransferItemTo(destinationInventory.Object, item, (MyFixedPoint?)requestedAmount))
                .Callback<IMyInventory, MyInventoryItem, MyFixedPoint?>((destination, itm, amt) => {
                    sourceInventory.Setup(i => i.CurrentMass).Returns(currentMass - transferredAmount);
                })
                .Returns(true);
        }
    }
}
