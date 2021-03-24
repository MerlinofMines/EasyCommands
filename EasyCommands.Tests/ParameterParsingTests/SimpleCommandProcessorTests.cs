using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class SimpleCommandProcessorTests {
        [TestMethod]
        public void PrintCommand() {
            var command = ParseCommand("print 'Hello World'");
            Assert.IsTrue(command is PrintCommand);
            PrintCommand printCommand = (PrintCommand)command;
            Assert.AreEqual("Hello World", CastString(printCommand.variable.GetValue()).GetStringValue());
        }

        [TestMethod]
        public void PrintCommandVariable() {
            var command = ParseCommand("print {a}");
            Assert.IsTrue(command is PrintCommand);
            PrintCommand printCommand = (PrintCommand)command;
            Assert.IsTrue(printCommand.variable is InMemoryVariable);
            InMemoryVariable variable = (InMemoryVariable)printCommand.variable;
            Assert.AreEqual("a", variable.variableName);
        }

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
            FUNCTIONS["listen"] = new FunctionDefinition("listen", new List<string>());
            var command = ParseCommand("goto \"listen\"");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionDefinition.functionName);
            Assert.AreEqual(FunctionType.GOTO, functionCommand.type);
        }

        [TestMethod]
        public void FunctionCommandWithParameters() {
            FUNCTIONS["listen"] = new FunctionDefinition("listen", new List<string>() { "a", "b" });
            var command = ParseCommand("goto \"listen\" 2 3");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionDefinition.functionName);
            Assert.AreEqual(FunctionType.GOTO, functionCommand.type);
            Assert.AreEqual(2, CastNumber(functionCommand.inputParameters["a"].GetValue()).GetNumericValue());
            Assert.AreEqual(3, CastNumber(functionCommand.inputParameters["b"].GetValue()).GetNumericValue());
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
        public void AssignVariableCaseIsPreserved() {
            var command = ParseCommand("assign \"a\" to {variableName}");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.IsTrue(assignCommand.variable is InMemoryVariable);
            InMemoryVariable memoryVariable = (InMemoryVariable)assignCommand.variable;
            Assert.AreEqual("variableName", memoryVariable.variableName);
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
