using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class ConditionalParameterProcessorTests {
        [TestInitialize]
        public void InitializeTestClass() {
          System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");          
        }

        [TestMethod]
        public void SimpleBooleanCondition() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("if true set the \"rotors\" height to 5");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.condition is StaticVariable);
            StaticVariable variable = (StaticVariable)conditionalCommand.condition;
            Assert.AreEqual(Return.BOOLEAN, variable.GetValue().returnType);
            Assert.IsTrue(CastBoolean(variable.GetValue()));
        }

        [TestMethod]
        public void SimpleVariableCondition() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("if a set the \"rotors\" height to 5");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.condition is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)conditionalCommand.condition;
            Assert.AreEqual("a", variable.value);
        }

        [TestMethod]
        public void VariableComparisonCondition() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("if 3 > 2 set the \"rotors\" height to 5");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.condition is ComparisonVariable);
            ComparisonVariable variable = (ComparisonVariable)conditionalCommand.condition;
            Assert.AreEqual(Return.BOOLEAN, variable.GetValue().returnType);
            Assert.IsTrue(CastBoolean(variable.GetValue()));
            Assert.IsTrue(variable.a is StaticVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void BlockConditionWithProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("if the \"batteries\" are recharging set the \"rotors\" height to 5");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.condition is AggregateConditionVariable);
        }
    }
}
