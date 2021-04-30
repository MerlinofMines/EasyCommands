using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
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
            MDKFactory.CreateProgram<Program>();
            PROGRAM.functions["listen"] = new FunctionDefinition("listen", new List<string>());
            var command = ParseCommand("goto \"listen\"");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionDefinition.functionName);
            Assert.AreEqual(FunctionType.GOTO, functionCommand.type);
        }

        [TestMethod]
        public void FunctionCommandFromExplicitString() {
            MDKFactory.CreateProgram<Program>();
            PROGRAM.functions["listen"] = new FunctionDefinition("listen", new List<string>());
            var command = ParseCommand("goto 'listen'");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionDefinition.functionName);
            Assert.AreEqual(FunctionType.GOTO, functionCommand.type);
        }

        [TestMethod]
        public void FunctionCommandWithParameters() {
            MDKFactory.CreateProgram<Program>();
            PROGRAM.functions["listen"] = new FunctionDefinition("listen", new List<string>() { "a", "b" });
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
        public void IterateSimpleCommand() {
            var command = ParseCommand("print \"hello world\" 3 times");
            Assert.IsTrue(command is MultiActionCommand);
            MultiActionCommand iterateCommand = (MultiActionCommand)command;
            Assert.AreEqual(3f, iterateCommand.loopCount.GetValue().GetValue());
            List<Command> commands = iterateCommand.commandsToExecute;
            Assert.AreEqual(1, commands.Count);
            Assert.IsTrue(commands[0] is PrintCommand);
            PrintCommand printCommand = (PrintCommand)commands[0];
            Assert.AreEqual("hello world", printCommand.variable.GetValue().GetValue());
        }

        [TestMethod]
        public void IterateSimpleCommandAfter() {
            var command = ParseCommand("for 3 times print \"hello world\"");
            Assert.IsTrue(command is MultiActionCommand);
            MultiActionCommand iterateCommand = (MultiActionCommand)command;
            Assert.AreEqual(3f, iterateCommand.loopCount.GetValue().GetValue());
            List<Command> commands = iterateCommand.commandsToExecute;
            Assert.AreEqual(1, commands.Count);
            Assert.IsTrue(commands[0] is PrintCommand);
            PrintCommand printCommand = (PrintCommand)commands[0];
            Assert.AreEqual("hello world", printCommand.variable.GetValue().GetValue());
        }

        [TestMethod]
        public void StopCommand() {
            var command = ParseCommand("stop");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(ControlType.STOP, controlCommand.controlType);
        }

        [TestMethod]
        public void StartCommand() {
            var command = ParseCommand("start");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(ControlType.START, controlCommand.controlType);
        }

        [TestMethod]
        public void RestartCommand() {
            var command = ParseCommand("restart");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(ControlType.RESTART, controlCommand.controlType);
        }

        [TestMethod]
        public void RepeatCommand() {
            var command = ParseCommand("repeat");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(ControlType.REPEAT, controlCommand.controlType);
        }

        [TestMethod]
        public void PauseCommand() {
            var command = ParseCommand("pause");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(ControlType.PAUSE, controlCommand.controlType);
        }

        [TestMethod]
        public void ResumeCommand() {
            var command = ParseCommand("resume");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(ControlType.START, controlCommand.controlType);
        }
    }
}
