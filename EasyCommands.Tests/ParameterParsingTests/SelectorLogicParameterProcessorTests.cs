using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class SelectorLogicParameterProcessorTests {
        [TestMethod]
        public void BasicSelector() {
            var command = ParseCommand("recharge the \"batteries\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.AreEqual(BlockType.BATTERY, sep.GetBlockType());
            Assert.IsTrue(sep.isGroup);
            Assert.IsTrue(sep.selector is StaticVariable);
            Assert.AreEqual("batteries", CastString(sep.selector.GetValue()).GetStringValue());
        }

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
        public void VariableSelector() {
            var command = ParseCommand("turn on the [a] sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.IsTrue(sep.selector is InMemoryVariable);
            InMemoryVariable variable = (InMemoryVariable)sep.selector;
            Assert.AreEqual("a", variable.variableName);
            Assert.AreEqual(BlockType.SOUND, sep.blockType);
        }
    }
}
