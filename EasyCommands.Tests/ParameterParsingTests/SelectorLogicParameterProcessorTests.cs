using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
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

        [TestMethod]
        public void ImplicitVariableSelector() {
            var command = ParseCommand("turn on the [a]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.IsTrue(sep.selector is InMemoryVariable);
            InMemoryVariable variable = (InMemoryVariable)sep.selector;
            Assert.AreEqual("a", variable.variableName);
        }

        [TestMethod]
        public void MySelector() {
            var command = ParseCommand("assign a to my average location");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable variable = (AggregatePropertyVariable)assignCommand.variable;
            Assert.IsTrue(variable.entityProvider is SelfEntityProvider);
            Assert.AreEqual(BlockType.PROGRAM, variable.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void MySelectorWithBlockType() {
            var command = ParseCommand("set my display @0 text to \"hello world\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            Assert.IsTrue(iep.provider is SelfEntityProvider);
            Assert.AreEqual(BlockType.DISPLAY, iep.provider.GetBlockType());
        }

        [TestMethod]
        public void AllSelector() {
            var command = ParseCommand("set all piston height to 0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)bc.entityProvider;
            Assert.AreEqual(BlockType.PISTON, aep.GetBlockType());
        }

        [TestMethod]
        public void AllSelectorGroup() {
            var command = ParseCommand("set the height of all pistons to 0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)bc.entityProvider;
            Assert.AreEqual(BlockType.PISTON, aep.GetBlockType());
        }

        [TestMethod]
        public void AllSelectorGroupWithCondition() {
            var command = ParseCommand("recharge all batteries whose ratio < 0.25");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is ConditionalEntityProvider);
            ConditionalEntityProvider cep = (ConditionalEntityProvider)bc.entityProvider;
            Assert.IsTrue(cep.provider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)cep.provider;
            Assert.AreEqual(BlockType.BATTERY, aep.GetBlockType());
        }

        [TestMethod]
        public void ImplicitAllSelector() {
            var command = ParseCommand("turn on the light");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)bc.entityProvider;
            Assert.AreEqual(BlockType.LIGHT, aep.GetBlockType());
        }

        [TestMethod]
        public void ImplicitAllGroupSelector() {
            var command = ParseCommand("turn on the lights");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)bc.entityProvider;
            Assert.AreEqual(BlockType.LIGHT, aep.GetBlockType());
        }
    }
}
