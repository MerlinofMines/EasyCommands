using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using static IngameScript.Program;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class TransferCommandProcessorTests : ForceLocale {
        [TestMethod]
        public void SimpleTransfer() {
            var program = CreateProgram();
            var command = program.ParseCommand("transfer \"gold ingot\" from \"source cargo\" to \"destination cargo\"");
            Assert.IsTrue(command is TransferItemCommand);
            TransferItemCommand transferCommand = (TransferItemCommand)command;
            Assert.IsTrue(transferCommand.from is BlockSelector);
            Assert.IsTrue(transferCommand.to is BlockSelector);
            BlockSelector from = (BlockSelector)transferCommand.from;
            BlockSelector to = (BlockSelector)transferCommand.to;
            Assert.AreEqual("source cargo", from.selector.GetValue().value);
            Assert.AreEqual("destination cargo", to.selector.GetValue().value);
            Assert.AreEqual("gold ingot", transferCommand.first.GetValue().value);
            Assert.IsNull(transferCommand.second);
        }

        [TestMethod]
        public void SimpleTransferWithAmount() {
            var program = CreateProgram();
            var command = program.ParseCommand("transfer 50 \"gold ingot\" from \"source cargo\" to \"destination cargo\"");
            Assert.IsTrue(command is TransferItemCommand);
            TransferItemCommand transferCommand = (TransferItemCommand)command;
            Assert.IsTrue(transferCommand.from is BlockSelector);
            Assert.IsTrue(transferCommand.to is BlockSelector);
            BlockSelector from = (BlockSelector)transferCommand.from;
            BlockSelector to = (BlockSelector)transferCommand.to;
            Assert.AreEqual("source cargo", from.selector.GetValue().value);
            Assert.AreEqual("destination cargo", to.selector.GetValue().value);
            Assert.AreEqual(50f, transferCommand.first.GetValue().value);
            Assert.AreEqual("gold ingot", transferCommand.second.GetValue().value);
        }

        [TestMethod]
        public void TransferAfterSource() {
            var program = CreateProgram();
            var command = program.ParseCommand("\"source cargo\" transfer \"gold ingot\" to \"destination cargo\"");
            Assert.IsTrue(command is TransferItemCommand);
            TransferItemCommand transferCommand = (TransferItemCommand)command;
            Assert.IsTrue(transferCommand.from is BlockSelector);
            Assert.IsTrue(transferCommand.to is BlockSelector);
            BlockSelector from = (BlockSelector)transferCommand.from;
            BlockSelector to = (BlockSelector)transferCommand.to;
            Assert.AreEqual("source cargo", from.selector.GetValue().value);
            Assert.AreEqual("destination cargo", to.selector.GetValue().value);
            Assert.AreEqual("gold ingot", transferCommand.first.GetValue().value);
            Assert.IsNull(transferCommand.second);
        }

        [TestMethod]
        public void TransferAfterSourceWithAmount() {
            var program = CreateProgram();
            var command = program.ParseCommand("\"source cargo\" transfer 50 \"gold ingot\" to \"destination cargo\"");
            Assert.IsTrue(command is TransferItemCommand);
            TransferItemCommand transferCommand = (TransferItemCommand)command;
            Assert.IsTrue(transferCommand.from is BlockSelector);
            Assert.IsTrue(transferCommand.to is BlockSelector);
            BlockSelector from = (BlockSelector)transferCommand.from;
            BlockSelector to = (BlockSelector)transferCommand.to;
            Assert.AreEqual("source cargo", from.selector.GetValue().value);
            Assert.AreEqual("destination cargo", to.selector.GetValue().value);
            Assert.AreEqual(50f, transferCommand.first.GetValue().value);
            Assert.AreEqual("gold ingot", transferCommand.second.GetValue().value);
        }

        [TestMethod]
        public void SimpleTake() {
            var program = CreateProgram();
            var command = program.ParseCommand("take \"gold ingot\" from \"source cargo\" to \"destination cargo\"");
            Assert.IsTrue(command is TransferItemCommand);
            TransferItemCommand transferCommand = (TransferItemCommand)command;
            Assert.IsTrue(transferCommand.from is BlockSelector);
            Assert.IsTrue(transferCommand.to is BlockSelector);
            BlockSelector from = (BlockSelector)transferCommand.from;
            BlockSelector to = (BlockSelector)transferCommand.to;
            Assert.AreEqual("source cargo", from.selector.GetValue().value);
            Assert.AreEqual("destination cargo", to.selector.GetValue().value);
            Assert.AreEqual("gold ingot", transferCommand.first.GetValue().value);
            Assert.IsNull(transferCommand.second);
        }

        [TestMethod]
        public void SimpleTakeWithAmount() {
            var program = CreateProgram();
            var command = program.ParseCommand("take 50 \"gold ingot\" from \"source cargo\" to \"destination cargo\"");
            Assert.IsTrue(command is TransferItemCommand);
            TransferItemCommand transferCommand = (TransferItemCommand)command;
            Assert.IsTrue(transferCommand.from is BlockSelector);
            Assert.IsTrue(transferCommand.to is BlockSelector);
            BlockSelector from = (BlockSelector)transferCommand.from;
            BlockSelector to = (BlockSelector)transferCommand.to;
            Assert.AreEqual("source cargo", from.selector.GetValue().value);
            Assert.AreEqual("destination cargo", to.selector.GetValue().value);
            Assert.AreEqual(50f, transferCommand.first.GetValue().value);
            Assert.AreEqual("gold ingot", transferCommand.second.GetValue().value);
        }

        [TestMethod]
        public void TakeAfterDestination() {
            var program = CreateProgram();
            var command = program.ParseCommand("\"destination cargo\" take \"gold ingot\" from \"source cargo\"");
            Assert.IsTrue(command is TransferItemCommand);
            TransferItemCommand transferCommand = (TransferItemCommand)command;
            Assert.IsTrue(transferCommand.from is BlockSelector);
            Assert.IsTrue(transferCommand.to is BlockSelector);
            BlockSelector from = (BlockSelector)transferCommand.from;
            BlockSelector to = (BlockSelector)transferCommand.to;
            Assert.AreEqual("source cargo", from.selector.GetValue().value);
            Assert.AreEqual("destination cargo", to.selector.GetValue().value);
            Assert.AreEqual("gold ingot", transferCommand.first.GetValue().value);
            Assert.IsNull(transferCommand.second);
        }

        [TestMethod]
        public void TakeAfterDestinationWithAmount() {
            var program = CreateProgram();
            var command = program.ParseCommand("\"destination cargo\" take 50 \"gold ingot\" from \"source cargo\"");
            Assert.IsTrue(command is TransferItemCommand);
            TransferItemCommand transferCommand = (TransferItemCommand)command;
            Assert.IsTrue(transferCommand.from is BlockSelector);
            Assert.IsTrue(transferCommand.to is BlockSelector);
            BlockSelector from = (BlockSelector)transferCommand.from;
            BlockSelector to = (BlockSelector)transferCommand.to;
            Assert.AreEqual("source cargo", from.selector.GetValue().value);
            Assert.AreEqual("destination cargo", to.selector.GetValue().value);
            Assert.AreEqual(50f, transferCommand.first.GetValue().value);
            Assert.AreEqual("gold ingot", transferCommand.second.GetValue().value);
        }
    }
}
