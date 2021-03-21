using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class SimpleCommandProcessorTests {
        [TestMethod]
        public void ListenCommand() {
            var command = ParseCommand("listen \"garageDoors\"");
            Assert.IsTrue(command is ListenCommand);
            ListenCommand listenCommand = (ListenCommand)command;
            Assert.AreEqual("garageDoors", CastString(listenCommand.tag.GetValue()).GetStringValue());
        }

        [TestMethod]
        public void SendCommand() {
            var command = ParseCommand("send \"goto openDoors\" to \"garageDoors\"");
            Assert.IsTrue(command is SendCommand);
            SendCommand sendCommand = (SendCommand)command;
            Assert.AreEqual("garageDoors", CastString(sendCommand.tag.GetValue()).GetStringValue());
            Assert.AreEqual("goto openDoors", CastString(sendCommand.message.GetValue()).GetStringValue());
        }

        [TestMethod]
        public void FunctionCommand() {
            var command = ParseCommand("goto \"listen\"");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionName);
            Assert.AreEqual(FunctionType.GOTO, functionCommand.type);
        }

        [TestMethod]
        public void WaitCommandNoArguments() {
            var command = ParseCommand("wait");
            Assert.IsTrue(command is WaitCommand);
            WaitCommand waitCommand = (WaitCommand)command;
            Assert.AreEqual(1, CastNumber(waitCommand.waitInterval.GetValue()).GetNumericValue());
            Assert.AreEqual(UnitType.TICKS, waitCommand.units);
        }

        [TestMethod]
        public void WaitCommandInterval() {
            var command = ParseCommand("wait 3");
            Assert.IsTrue(command is WaitCommand);
            WaitCommand waitCommand = (WaitCommand)command;
            Assert.AreEqual(3, CastNumber(waitCommand.waitInterval.GetValue()).GetNumericValue());
            Assert.AreEqual(UnitType.SECONDS, waitCommand.units);
        }

        [TestMethod]
        public void WaitCommandIntervalUnits() {
            var command = ParseCommand("wait 3 ticks");
            Assert.IsTrue(command is WaitCommand);
            WaitCommand waitCommand = (WaitCommand)command;
            Assert.AreEqual(3, CastNumber(waitCommand.waitInterval.GetValue()).GetNumericValue());
            Assert.AreEqual(UnitType.TICKS, waitCommand.units);
        }

        [TestMethod]
        public void AssignVariable() {
            var command = ParseCommand("assign \"a\" to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand) command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()).GetNumericValue());
            Assert.IsFalse(assignCommand.useReference);
        }

        [TestMethod]
        public void LockVariable() {
            var command = ParseCommand("bind \"a\" to {b} is 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.IsTrue(assignCommand.variable is ComparisonVariable);
            Assert.IsTrue(assignCommand.useReference);

            ComparisonVariable comparison = (ComparisonVariable)assignCommand.variable;
            Assert.IsTrue(comparison.a is InMemoryVariable);
        }
    }
}
