using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VRageMath;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class SimpleVariableParameterProcessorTests {

        [TestMethod]
        public void AssignVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
        }

        [TestMethod]
        public void AssignVariableWithActionKeyword() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set \"a\" to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
        }

        [TestMethod]
        public void AssignGlobalVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign global \"a\" to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
            Assert.IsTrue(assignCommand.isGlobal);
        }

        [TestMethod]
        public void AssignGlobalVariableWithActionKeyword() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set global \"a\" to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
            Assert.IsTrue(assignCommand.isGlobal);
        }

        [TestMethod]
        public void AssignVariableFromExplicitString() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign 'a' to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
        }

        [TestMethod]
        public void AssignVariableFromInMemoryVariableName() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign {a} to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
        }

        [TestMethod]
        public void AssignVariableToAmbiguousStringValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to b");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual("b", assignCommand.variable.GetValue().value);
            Assert.IsFalse(assignCommand.useReference);
        }

        [TestMethod]
        public void AssignVariableToImplicitVariableReference() {
            var program = MDKFactory.CreateProgram<Program>();
            var thread = new Thread(new NullCommand(), "test", "test");
            program.currentThread = thread;
            program.SetGlobalVariable("b", GetStaticVariable(4));
            var command = program.ParseCommand("assign a to b");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(4, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
        }

        [TestMethod]
        public void AssignVariableCaseIsPreserved() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to {variableName}");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.IsTrue(assignCommand.variable is InMemoryVariable);
            InMemoryVariable memoryVariable = (InMemoryVariable)assignCommand.variable;
            Assert.AreEqual("variableName", memoryVariable.variableName);
        }

        [TestMethod]
        public void LockVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("bind \"a\" to {b} is 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.IsTrue(assignCommand.variable is ComparisonVariable);
            Assert.IsTrue(assignCommand.useReference);

            ComparisonVariable comparison = (ComparisonVariable)assignCommand.variable;
            Assert.IsTrue(comparison.a is InMemoryVariable);
        }

        [TestMethod]
        public void ParseSimpleVector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"53573.9750085028:-26601.8512032533:12058.8229348438\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is StaticVariable);
            StaticVariable variable = (StaticVariable)assignCommand.variable;
            Assert.AreEqual(Return.VECTOR, variable.GetValue().returnType);
            Vector3D vector = CastVector(variable.GetValue());
            Assert.AreEqual(53573.9750085028, vector.X);
            Assert.AreEqual(-26601.8512032533, vector.Y);
            Assert.AreEqual(12058.8229348438, vector.Z);
        }

        [TestMethod]
        public void ParseVectorFromGPSCoordinate() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"GPS:surface:53573.9750085028:-26601.8512032533:12058.8229348438:#FF75C9F1:\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is StaticVariable);
            StaticVariable variable = (StaticVariable)assignCommand.variable;
            Assert.AreEqual(Return.VECTOR, variable.GetValue().returnType);
            Vector3D vector = CastVector(variable.GetValue());
            Assert.AreEqual(53573.9750085028, vector.X);
            Assert.AreEqual(-26601.8512032533, vector.Y);
            Assert.AreEqual(12058.8229348438, vector.Z);
        }

        [TestMethod]
        public void AssignVariableToSelectorProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"vector\" to avg \"Main Cockpit\" position");
            Assert.IsTrue(command is VariableAssignmentCommand);
        }

        [TestMethod]
        public void AssignVariableToMySelectorProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set \"myPosition\" to my position");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
        }

        [TestMethod]
        public void AssignVariableToSelectorString() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"mySelector\" to \"Main Cockpit\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignmentCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("Main Cockpit", assignmentCommand.variable.GetValue().value);
        }
    }
}
