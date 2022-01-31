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
    public class SimpleCommandProcessorTests : ForceLocale {
        [TestMethod]
        public void PrintCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("print 'Hello World'");
            Assert.IsTrue(command is PrintCommand);
            PrintCommand printCommand = (PrintCommand)command;
            Assert.AreEqual("Hello World", CastString(printCommand.variable.GetValue()));
        }

        [TestMethod]
        public void PrintCommandVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("print a");
            Assert.IsTrue(command is PrintCommand);
            PrintCommand printCommand = (PrintCommand)command;
            Assert.IsTrue(printCommand.variable is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)printCommand.variable;
            Assert.AreEqual("a", variable.value);
        }

        [TestMethod]
        public void ListenCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("listen \"garageDoors\"");
            Assert.IsTrue(command is ListenCommand);
            ListenCommand listenCommand = (ListenCommand)command;
            Assert.AreEqual("garageDoors", CastString(listenCommand.tag.GetValue()));
        }

        [TestMethod]
        public void SendCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("send \"goto openDoors\" to \"garageDoors\"");
            Assert.IsTrue(command is SendCommand);
            SendCommand sendCommand = (SendCommand)command;
            Assert.AreEqual("garageDoors", CastString(sendCommand.tag.GetValue()));
            Assert.AreEqual("goto openDoors", CastString(sendCommand.message.GetValue()));
        }

        [TestMethod]
        public void FunctionCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            program.functions["listen"] = new FunctionDefinition("listen", new List<string>());
            var command = program.ParseCommand("goto \"listen\"");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionName());
            Assert.AreEqual(true, functionCommand.switchExecution);
        }

        [TestMethod]
        public void ImpliciitFunctionCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            program.functions["listen"] = new FunctionDefinition("listen", new List<string>());
            var command = program.ParseCommand("\"listen\"");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionName());
            Assert.AreEqual(false, functionCommand.switchExecution);
        }

        [TestMethod]
        public void FunctionCommandFromExplicitString() {
            var program = MDKFactory.CreateProgram<Program>();
            program.functions["listen"] = new FunctionDefinition("listen", new List<string>());
            var command = program.ParseCommand("goto 'listen'");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionName());
            Assert.AreEqual(true, functionCommand.switchExecution);
        }

        [TestMethod]
        public void FunctionCommandWithParameters() {
            var program = MDKFactory.CreateProgram<Program>();
            program.functions["listen"] = new FunctionDefinition("listen", new List<string>() { "a", "b" });
            var command = program.ParseCommand("goto \"listen\" 2 3");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionName());
            Assert.AreEqual(true, functionCommand.switchExecution);
            Assert.AreEqual(2, CastNumber(functionCommand.inputParameters[0].GetValue()));
            Assert.AreEqual(3, CastNumber(functionCommand.inputParameters[1].GetValue()));
        }

        [TestMethod]
        public void ImplicitFunctionCommandWithParameters() {
            var program = MDKFactory.CreateProgram<Program>();
            program.functions["listen"] = new FunctionDefinition("listen", new List<string>() { "a", "b" });
            var command = program.ParseCommand("\"listen\" 2 3");
            Assert.IsTrue(command is FunctionCommand);
            FunctionCommand functionCommand = (FunctionCommand)command;
            Assert.AreEqual("listen", functionCommand.functionName());
            Assert.AreEqual(false, functionCommand.switchExecution);
            Assert.AreEqual(2, CastNumber(functionCommand.inputParameters[0].GetValue()));
            Assert.AreEqual(3, CastNumber(functionCommand.inputParameters[1].GetValue()));
        }

        [TestMethod]
        public void SetVariableWithSameNameAsFunction() {
            var program = MDKFactory.CreateProgram<Program>();
            program.functions["function"] = new FunctionDefinition("function", new List<string>() { "a", "b" });
            var command = program.ParseCommand("set function to \"myFunction\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("function", assignCommand.variableName);
            Assert.AreEqual("myFunction", assignCommand.variable.GetValue().value);
        }

        [TestMethod]
        public void WaitCommandNoArguments() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("wait");
            Assert.IsTrue(command is WaitCommand);
            WaitCommand waitCommand = (WaitCommand)command;
            Assert.AreEqual(0.01666f, CastNumber(waitCommand.waitInterval.GetValue())); // 1/60 second
        }

        [TestMethod]
        public void WaitCommandInterval() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("wait 3");
            Assert.IsTrue(command is WaitCommand);
            WaitCommand waitCommand = (WaitCommand)command;
            Assert.AreEqual(3, CastNumber(waitCommand.waitInterval.GetValue()));
        }

        [TestMethod]
        public void WaitCommandIntervalTicks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("wait 3 ticks");
            Assert.IsTrue(command is WaitCommand);
            WaitCommand waitCommand = (WaitCommand)command;
            Assert.AreEqual(0.05f, CastNumber(waitCommand.waitInterval.GetValue()));
        }

        [TestMethod]
        public void IterateSimpleCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("print \"hello world\" 3 times");
            Assert.IsTrue(command is MultiActionCommand);
            MultiActionCommand iterateCommand = (MultiActionCommand)command;
            Assert.AreEqual(3f, iterateCommand.loopCount.GetValue().value);
            List<Command> commands = iterateCommand.commandsToExecute;
            Assert.AreEqual(1, commands.Count);
            Assert.IsTrue(commands[0] is PrintCommand);
            PrintCommand printCommand = (PrintCommand)commands[0];
            Assert.AreEqual("hello world", printCommand.variable.GetValue().value);
        }

        [TestMethod]
        public void IterateSimpleCommandAfter() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("for 3 times print \"hello world\"");
            Assert.IsTrue(command is MultiActionCommand);
            MultiActionCommand iterateCommand = (MultiActionCommand)command;
            Assert.AreEqual(3f, iterateCommand.loopCount.GetValue().value);
            List<Command> commands = iterateCommand.commandsToExecute;
            Assert.AreEqual(1, commands.Count);
            Assert.IsTrue(commands[0] is PrintCommand);
            PrintCommand printCommand = (PrintCommand)commands[0];
            Assert.AreEqual("hello world", printCommand.variable.GetValue().value);
        }
    }
}
