using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class SimpleBlockCommandParameterProcessorTests {
        [TestMethod]
        public void SimpleBlockCommandWithProperty() {
            var command = ParseCommand("turn on the \"pistons\"");
            Assert.IsTrue(command is BlockCommand);

            command = ParseCommand("turn the \"pistons\" on");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithValue() {
            var command = ParseCommand("set the \"pistons\" to 2");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithVariableValue() {
            var command = ParseCommand("set the \"pistons\" to {b}");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithDirection() {
            var command = ParseCommand("retract the \"pistons\"");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithReverse()
        {
            var command = ParseCommand("reverse the \"pistons\"");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithPropertyAndVariable() {
            var command = ParseCommand("set the \"pistons\" height to 2");
            Assert.IsTrue(command is BlockCommand);

            command = ParseCommand("set the height of the \"pistons\" to 2");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithDirectionAndVariable() {
            var command = ParseCommand("rotate the \"rotors\" clockwise to 30");
            Assert.IsTrue(command is BlockCommand);

            command = ParseCommand("set the \"rotors\" angle to 30 clockwise");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithDirectionAndPropertyAndVariable() {
            var command = ParseCommand("rotate the \"rotors\" angle clockwise to 30");
            Assert.IsTrue(command is BlockCommand);

            command = ParseCommand("set the \"rotors\" angle to 30 clockwise");
            Assert.IsTrue(command is BlockCommand);
        }

    }
}
