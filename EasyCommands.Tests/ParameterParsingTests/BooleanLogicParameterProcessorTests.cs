using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class BooleanLogicParameterProcessorTests {
        [TestMethod]
        public void AndSimpleVariableCondition() {
            var command = ParseCommand("if true and false turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void AndBlockCondition() {
            var command = ParseCommand("if the \"batteries\" are on but are not recharging turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void AndAggregateConditions() {
            var command = ParseCommand("if the \"batteries\" are on but none of the \"batteries\" is recharging turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void OrSimpleVariableCondition() {
            var command = ParseCommand("if true or false turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void OrBlockCondition() {
            var command = ParseCommand("if any of the \"batteries\" ratio is less than 0.5 or is recharging turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void OrAggregateConditions() {
            var command = ParseCommand("if the \"batteries\" are off or any of the \"batteries\" ratio is less than 0.25 turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void NotVariableCondition() {
            var command = ParseCommand("if not true turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void NotBlockCondition() {
            var command = ParseCommand("if not true turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void NotAggregateBlockCondition() {
            var command = ParseCommand("if not all of the \"batteries\" ratio > 0.75 turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }
    }
}
