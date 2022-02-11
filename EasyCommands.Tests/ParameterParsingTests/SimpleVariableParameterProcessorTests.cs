using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VRageMath;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class SimpleVariableParameterProcessorTests :ForceLocale {
        [TestMethod]
        public void AssignVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
        }

        [TestMethod]
        public void AssignVariableWithActionKeyword() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set a to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
        }

        [TestMethod]
        public void IncrementVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("increase i");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual(1f, incrementCommand.variable.GetValue().value);
            Assert.IsTrue(incrementCommand.increment);
        }

        [TestMethod]
        public void IncrementVariablePre() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("++i");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual(1f, incrementCommand.variable.GetValue().value);
            Assert.IsTrue(incrementCommand.increment);
        }

        [TestMethod]
        public void IncrementVariablePost() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("i++");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual(1f, incrementCommand.variable.GetValue().value);
            Assert.IsTrue(incrementCommand.increment);
        }

        [TestMethod]
        public void IncrementVariableValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("increase i by 2");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual(2f, incrementCommand.variable.GetValue().value);
            Assert.IsTrue(incrementCommand.increment);
        }

        [TestMethod]
        public void IncrementVariableByVariableValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("increase i by j");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.IsTrue(incrementCommand.variable is AmbiguousStringVariable);
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual("j", ((AmbiguousStringVariable)incrementCommand.variable).value);
            Assert.IsTrue(incrementCommand.increment);
        }

        [TestMethod]
        public void IncrementVariableValuePost() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("i+=2");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual(2f, incrementCommand.variable.GetValue().value);
            Assert.IsTrue(incrementCommand.increment);
        }

        [TestMethod]
        public void IncrementVariableValuePostVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("i+=j");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.IsTrue(incrementCommand.variable is AmbiguousStringVariable);
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual("j", ((AmbiguousStringVariable)incrementCommand.variable).value);
            Assert.IsTrue(incrementCommand.increment);
        }

        [TestMethod]
        public void DecrementVariablePre() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("--i");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual(1f, incrementCommand.variable.GetValue().value);
            Assert.IsFalse(incrementCommand.increment);
        }

        [TestMethod]
        public void DecrementVariablePost() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("i--");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual(1f, incrementCommand.variable.GetValue().value);
            Assert.IsFalse(incrementCommand.increment);
        }

        [TestMethod]
        public void DecrementVariableValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("decrease i by 2");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual(2f, incrementCommand.variable.GetValue().value);
            Assert.IsFalse(incrementCommand.increment);
        }

        [TestMethod]
        public void DecrementVariableValuePost() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("i-=2");
            Assert.IsTrue(command is VariableIncrementCommand);
            VariableIncrementCommand incrementCommand = (VariableIncrementCommand)command;
            Assert.AreEqual("i", incrementCommand.variableName);
            Assert.AreEqual(2f, incrementCommand.variable.GetValue().value);
            Assert.IsFalse(incrementCommand.increment);
        }

        [TestMethod]
        public void AssignGlobalVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign global a to 2");
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
            var command = program.ParseCommand("set global a to 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.AreEqual(2, CastNumber(assignCommand.variable.GetValue()));
            Assert.IsFalse(assignCommand.useReference);
            Assert.IsTrue(assignCommand.isGlobal);
        }

        [TestMethod]
        public void AssignVariableFromInMemoryVariableName() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 2");
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
            var command = program.ParseCommand("assign a to variableName");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.IsTrue(assignCommand.variable is AmbiguousStringVariable);
            AmbiguousStringVariable memoryVariable = (AmbiguousStringVariable)assignCommand.variable;
            Assert.AreEqual("variableName", memoryVariable.value);
        }

        [TestMethod]
        public void LockVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("bind a to b is 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("a", assignCommand.variableName);
            Assert.IsTrue(assignCommand.variable is ComparisonVariable);
            Assert.IsTrue(assignCommand.useReference);

            ComparisonVariable comparison = (ComparisonVariable)assignCommand.variable;
            Assert.IsTrue(comparison.a is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)comparison.a;
            Assert.AreEqual("b", variable.value);
        }

        [TestMethod]
        public void ParseSimpleVector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 53573.9750085028:-26601.8512032533:12058.8229348438");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            var variable = assignCommand.variable;
            Assert.AreEqual(Return.VECTOR, variable.GetValue().returnType);
            Vector3D vector = CastVector(variable.GetValue());
            Assert.AreEqual(53573.9750085028, vector.X);
            Assert.AreEqual(-26601.8512032533, vector.Y);
            Assert.AreEqual(12058.8229348438, vector.Z);
        }

        [TestMethod]
        public void ParseVectorFromGPSCoordinate() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"GPS:surface:53573.9750085028:-26601.8512032533:12058.8229348438:#FF75C9F1:\" as vector");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            var variable = assignCommand.variable;
            Assert.AreEqual(Return.VECTOR, variable.GetValue().returnType);
            Vector3D vector = CastVector(variable.GetValue());
            Assert.AreEqual(53573.9750085028, vector.X);
            Assert.AreEqual(-26601.8512032533, vector.Y);
            Assert.AreEqual(12058.8229348438, vector.Z);
        }

        [TestMethod]
        public void AssignVariableToSelectorProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign vector to avg \"Main Cockpit\" position");
            Assert.IsTrue(command is VariableAssignmentCommand);
        }

        [TestMethod]
        public void AssignVariableToCountOfGroupSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set myValue to the count of \"My Batteries\" batteries");
            Assert.IsTrue(command is VariableAssignmentCommand);
        }

        [TestMethod]
        public void AssignVariableToCountOfImplicitSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set myValue to the count of \"My Batteries\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
        }

        [TestMethod]
        public void AssignVariableToCountOfAllGroupSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set myValue to the count of my batteries");
            Assert.IsTrue(command is VariableAssignmentCommand);
        }

        [TestMethod]
        public void AssignVariableToMySelectorProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set myPosition to my position");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
        }

        [TestMethod]
        public void AssignVariableToSelectorString() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign mySelector to \"Main Cockpit\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignmentCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("Main Cockpit", assignmentCommand.variable.GetValue().value);
        }
    }
}
