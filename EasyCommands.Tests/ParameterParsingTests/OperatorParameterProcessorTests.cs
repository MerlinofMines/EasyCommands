using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class OperatorParameterProcessorTests {

        [TestMethod]
        public void AssignSimpleAddition() {
            var command = ParseCommand("assign a to 3 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable variable = (OperandVariable)assignment.variable;
            Assert.AreEqual(OperandType.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleSubtraction() {
            var command = ParseCommand("assign a to 3 - 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable variable = (OperandVariable)assignment.variable;
            Assert.AreEqual(OperandType.SUBTACT, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleMultiplication() {
            var command = ParseCommand("assign a to 3 * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable variable = (OperandVariable)assignment.variable;
            Assert.AreEqual(OperandType.MULTIPLY, variable.operand);
            Assert.AreEqual(6f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleDivision() {
            var command = ParseCommand("assign a to 6 / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable variable = (OperandVariable)assignment.variable;
            Assert.AreEqual(OperandType.DIVIDE, variable.operand);
            Assert.AreEqual(3f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleMod()
        {
            var command = ParseCommand("assign a to 5 % 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable variable = (OperandVariable)assignment.variable;
            Assert.AreEqual(OperandType.MOD, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleAdditionVariable() {
            var command = ParseCommand("assign a to {b} + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable variable = (OperandVariable)assignment.variable;
            Assert.AreEqual(OperandType.ADD, variable.operand);
            Assert.IsTrue(variable.a is InMemoryVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AssignSimpleSubtractionVariable() {
            var command = ParseCommand("assign a to {b} - 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable variable = (OperandVariable)assignment.variable;
            Assert.AreEqual(OperandType.SUBTACT, variable.operand);
            Assert.IsTrue(variable.a is InMemoryVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AssignSimpleMultiplicationVariable() {
            var command = ParseCommand("assign a to {b} * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable variable = (OperandVariable)assignment.variable;
            Assert.AreEqual(OperandType.MULTIPLY, variable.operand);
            Assert.IsTrue(variable.a is InMemoryVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AssignSimpleDivisionVariable() {
            var command = ParseCommand("assign a to {b} / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable variable = (OperandVariable)assignment.variable;
            Assert.AreEqual(OperandType.DIVIDE, variable.operand);
            Assert.IsTrue(variable.a is InMemoryVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void MultiplicationBeforeAddition() {
            var command = ParseCommand("assign a to 4 * 2 + 3");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable addVariable = (OperandVariable)assignment.variable;
            Assert.AreEqual(11f, CastNumber(addVariable.GetValue()).GetNumericValue());
            Assert.AreEqual(OperandType.ADD, addVariable.operand);
            Assert.IsTrue(addVariable.a is OperandVariable);
            Assert.IsTrue(addVariable.b is StaticVariable);
            OperandVariable multiplyVariable = (OperandVariable)addVariable.a;
            Assert.AreEqual(OperandType.MULTIPLY, multiplyVariable.operand);
        }

        [TestMethod]
        public void DivisionBeforeAddition() {
            var command = ParseCommand("assign a to 4 / 2 + 3");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is OperandVariable);
            OperandVariable addVariable = (OperandVariable)assignment.variable;
            Assert.AreEqual(5f, CastNumber(addVariable.GetValue()).GetNumericValue());
            Assert.AreEqual(OperandType.ADD, addVariable.operand);
            Assert.IsTrue(addVariable.a is OperandVariable);
            Assert.IsTrue(addVariable.b is StaticVariable);
            OperandVariable multiplyVariable = (OperandVariable)addVariable.a;
            Assert.AreEqual(OperandType.DIVIDE, multiplyVariable.operand);
        }

        [TestMethod]
        public void AdditionBeforeVariableComparison() {
            var command = ParseCommand("assign a to {b} + 1 > 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is ComparisonVariable);
            ComparisonVariable variable = (ComparisonVariable)assignment.variable;
            Assert.IsTrue(variable.a is OperandVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AdditionBeforeBooleanLogic() {
            var command = ParseCommand("assign a to {b} + 1 and {c}");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is AndVariable);
            AndVariable variable = (AndVariable)assignment.variable;
            Assert.IsTrue(variable.a is OperandVariable);
            Assert.IsTrue(variable.b is InMemoryVariable);
        }

        [TestMethod]
        public void AdditionUsedAsBlockConditionVariable() {
            var command = ParseCommand("if the \"rotor\" angle > {a} + 30 set the \"rotor\" angle to {a}");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand assignment = (ConditionalCommand)command;
            Assert.IsTrue(assignment.Condition is AggregateConditionVariable);
            AggregateConditionVariable condition = (AggregateConditionVariable)assignment.Condition;
            Assert.IsTrue(condition.blockCondition is BlockPropertyCondition);
            BlockPropertyCondition blockCondition = (BlockPropertyCondition)condition.blockCondition;
            Assert.IsTrue(blockCondition.comparisonValue is OperandVariable);
        }
    }
}
