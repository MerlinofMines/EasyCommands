using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class ConditionalParameterProcessorTests {
        [TestMethod]
        public void SimpleBooleanCondition() {
            var command = ParseCommand("if true set the \"rotors\" height to 5");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.Condition is StaticVariable);
            StaticVariable variable = (StaticVariable)conditionalCommand.Condition;
            Assert.IsTrue(variable.GetValue() is BooleanPrimitive);
            BooleanPrimitive boolean = (BooleanPrimitive)variable.GetValue();
            Assert.IsTrue(boolean.GetBooleanValue());
        }

        [TestMethod]
        public void VariableComparisonCondition() {
            var command = ParseCommand("if 3 > 2 set the \"rotors\" height to 5");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.Condition is ComparisonVariable);
            ComparisonVariable variable = (ComparisonVariable)conditionalCommand.Condition;
            Assert.IsTrue(variable.GetValue() is BooleanPrimitive);
            BooleanPrimitive boolean = (BooleanPrimitive)variable.GetValue();
            Assert.IsTrue(boolean.GetBooleanValue());
            Assert.IsTrue(variable.a is StaticVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void BlockConditionWithProperty() {
            var command = ParseCommand("if the \"batteries\" are recharging set the \"rotors\" height to 5");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.Condition is AggregateConditionVariable);
        }
    }
}
