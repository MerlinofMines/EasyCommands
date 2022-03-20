using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using static IngameScript.Program;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class SimpleBlockCommandParameterProcessorTests : ForceLocale {
        [TestMethod]
        public void SimpleBlockCommandWithProperty() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the \"pistons\"");
            Assert.IsTrue(command is BlockCommand);

            command = program.ParseCommand("turn the \"pistons\" on");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithPropertyUsingAssign() {
            var program = CreateProgram();
            var command = program.ParseCommand("assign the \"pistons\" height to 10");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithValueProperty() {
            var program = CreateProgram();
            var command = program.ParseCommand("set the \"test cargo\" \"gold ingot\" amount to 0");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithValuePropertyBeforeValue() {
            var program = CreateProgram();
            var command = program.ParseCommand("set the amount of \"gold ingot\" in the \"test cargo\" to 0");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithNotProperty() {
            var program = CreateProgram();
            var command = program.ParseCommand("tell the \"rockets\" not to shoot");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithValue() {
            var program = CreateProgram();
            var command = program.ParseCommand("set the \"pistons\" to 2");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithVariableValue() {
            var program = CreateProgram();
            var command = program.ParseCommand("set the \"pistons\" to {b}");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithDirection() {
            var program = CreateProgram();
            var command = program.ParseCommand("retract the \"pistons\"");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithReverse() {
            var program = CreateProgram();
            var command = program.ParseCommand("reverse the \"pistons\"");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithPropertyAndVariable() {
            var program = CreateProgram();
            var command = program.ParseCommand("set the \"pistons\" height to 2");
            Assert.IsTrue(command is BlockCommand);

            command = program.ParseCommand("set the height of the \"pistons\" to 2");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithDirectionAndVariable() {
            var program = CreateProgram();
            var command = program.ParseCommand("rotate the \"rotors\" clockwise to 30");
            Assert.IsTrue(command is BlockCommand);

            command = program.ParseCommand("set the \"rotors\" angle to 30 clockwise");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithDirectionAndPropertyAndVariable() {
            var program = CreateProgram();
            var command = program.ParseCommand("rotate the \"rotors\" angle clockwise to 30");
            Assert.IsTrue(command is BlockCommand);

            command = program.ParseCommand("set the \"rotors\" angle to 30 clockwise");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandUsingImplicitBlockPropertyValue() {
            var program = CreateProgram();
            var command = program.ParseCommand("rotate the \"rotors\" to the \"rotors\" upper limit");
            Assert.IsTrue(command is BlockCommand);
        }
    }
}
