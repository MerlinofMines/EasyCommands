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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("print 'Hello World'");
            Assert.IsTrue(command is PrintCommand);
            PrintCommand printCommand = (PrintCommand)command;
            Assert.AreEqual("Hello World", CastString(printCommand.variable.GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void PrintCommandVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("print {a}");
            Assert.IsTrue(command is PrintCommand);
            PrintCommand printCommand = (PrintCommand)command;
            Assert.IsTrue(printCommand.variable is InMemoryVariable);
            InMemoryVariable variable = (InMemoryVariable)printCommand.variable;
            Assert.AreEqual("a", variable.variableName);
        }

        [TestMethod]
        public void ListenCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("listen \"garageDoors\"");
            Assert.IsTrue(command is ListenCommand);
            ListenCommand listenCommand = (ListenCommand)command;
            Assert.AreEqual("garageDoors", CastString(listenCommand.tag.GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void SendCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("send \"goto openDoors\" to \"garageDoors\"");
            Assert.IsTrue(command is SendCommand);
            SendCommand sendCommand = (SendCommand)command;
            Assert.AreEqual("garageDoors", CastString(sendCommand.tag.GetValue()).GetTypedValue());
            Assert.AreEqual("goto openDoors", CastString(sendCommand.message.GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void FunctionCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            program.functions["listen"] = new FunctionDefinition("listen", new List<string>());
            var command = program.ParseCommand("goto \"listen\"");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionDefinition.functionName);
            Assert.AreEqual(Function.GOTO, functionCommand.type);
        }

        [TestMethod]
        public void FunctionCommandFromExplicitString() {
            var program = MDKFactory.CreateProgram<Program>();
            program.functions["listen"] = new FunctionDefinition("listen", new List<string>());
            var command = program.ParseCommand("goto 'listen'");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionDefinition.functionName);
            Assert.AreEqual(Function.GOTO, functionCommand.type);
        }

        [TestMethod]
        public void FunctionCommandWithParameters() {
            var program = MDKFactory.CreateProgram<Program>();
            program.functions["listen"] = new FunctionDefinition("listen", new List<string>() { "a", "b" });
            var command = program.ParseCommand("goto \"listen\" 2 3");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionDefinition.functionName);
            Assert.AreEqual(Function.GOTO, functionCommand.type);
            Assert.AreEqual(2, CastNumber(functionCommand.inputParameters["a"].GetValue()).GetTypedValue());
            Assert.AreEqual(3, CastNumber(functionCommand.inputParameters["b"].GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void WaitCommandNoArguments() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("wait");
            Assert.IsTrue(command is WaitCommand);
            WaitCommand waitCommand = (WaitCommand)command;
            Assert.AreEqual(1, CastNumber(waitCommand.waitInterval.GetValue()).GetTypedValue());
            Assert.AreEqual(Unit.TICKS, waitCommand.units);
        }

        [TestMethod]
        public void WaitCommandInterval() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("wait 3");
            Assert.IsTrue(command is WaitCommand);
            WaitCommand waitCommand = (WaitCommand)command;
            Assert.AreEqual(3, CastNumber(waitCommand.waitInterval.GetValue()).GetTypedValue());
            Assert.AreEqual(Unit.SECONDS, waitCommand.units);
        }

        [TestMethod]
        public void WaitCommandIntervalUnits() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("wait 3 ticks");
            Assert.IsTrue(command is WaitCommand);
            WaitCommand waitCommand = (WaitCommand)command;
            Assert.AreEqual(3, CastNumber(waitCommand.waitInterval.GetValue()).GetTypedValue());
            Assert.AreEqual(Unit.TICKS, waitCommand.units);
        }

        [TestMethod]
        public void IterateSimpleCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("print \"hello world\" 3 times");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("for 3 times print \"hello world\"");
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
        public void ExitCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("exit");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(Control.STOP, controlCommand.controlType);
        }

        [TestMethod]
        public void RestartCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("restart");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(Control.RESTART, controlCommand.controlType);
        }

        [TestMethod]
        public void RepeatCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("repeat");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(Control.REPEAT, controlCommand.controlType);
        }

        [TestMethod]
        public void PauseCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("pause");
            Assert.IsTrue(command is ControlCommand);
            ControlCommand controlCommand = (ControlCommand)command;
            Assert.AreEqual(Control.PAUSE, controlCommand.controlType);
        }
    }
}
