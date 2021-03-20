using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Test {
    [TestClass]
    public class SelectorLogicParameterProcessorTests {
        [TestMethod]
        public void ConditionalSelector() {
            var command = ParseCommand("recharge \"batteries\" whose ratio < 0.5");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is ConditionalEntityProvider);
        }

        [TestMethod]
        public void IndexSelector() {
            var command = ParseCommand("turn on \"batteries\" @ 0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
        }

        [TestMethod]
        public void InLineIndexSelector() {
            var command = ParseCommand("turn on \"batteries\" @0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
        }

        [TestMethod]
        public void ConditionalIndexSelector() {
            var command = ParseCommand("set the \"batteries\" whose ratio < 0.5 @0 to recharge");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            Assert.IsTrue(iep.provider is ConditionalEntityProvider);
        }

        [TestMethod]
        public void LastBlockTypeImpliedIsUsed() {
            var command = ParseCommand("turn on the \"Boom Door Program\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.AreEqual(BlockType.PROGRAM, sep.GetBlockType());
        }

        [TestMethod]
        public void Test() {
            var command = ParseCommand("set the \"Crush Program\" display @0 text to \"Ready\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            Assert.AreEqual(BlockType.DISPLAY, bc.entityProvider.GetBlockType());
            IndexEntityProvider sep = (IndexEntityProvider)bc.entityProvider;
            Assert.AreEqual(0, CastNumber(sep.index.GetValue()).GetNumericValue());
        }

        //TODO: VariableSelector
    }
}
