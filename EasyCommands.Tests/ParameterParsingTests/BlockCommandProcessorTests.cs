﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class SimpleBlockCommandParameterProcessorTests {
        [TestMethod]
        public void SimpleBlockCommandWithProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the \"pistons\"");
            Assert.IsTrue(command is BlockCommand);

            command = program.ParseCommand("turn the \"pistons\" on");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithNotProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("tell the \"rockets\" not to shoot");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set the \"pistons\" to 2");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithVariableValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set the \"pistons\" to {b}");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithDirection() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("retract the \"pistons\"");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithReverse() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("reverse the \"pistons\"");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithPropertyAndVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set the \"pistons\" height to 2");
            Assert.IsTrue(command is BlockCommand);

            command = program.ParseCommand("set the height of the \"pistons\" to 2");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithDirectionAndVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("rotate the \"rotors\" clockwise to 30");
            Assert.IsTrue(command is BlockCommand);

            command = program.ParseCommand("set the \"rotors\" angle to 30 clockwise");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void SimpleBlockCommandWithDirectionAndPropertyAndVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("rotate the \"rotors\" angle clockwise to 30");
            Assert.IsTrue(command is BlockCommand);

            command = program.ParseCommand("set the \"rotors\" angle to 30 clockwise");
            Assert.IsTrue(command is BlockCommand);
        }
    }
}
