using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VRageMath;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class OperatorParameterProcessorTests {

        [TestMethod]
        public void AssignAbsoluteValue() {
            var command = ParseCommand("assign a to abs -3 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignAbsoluteValueVector() {
            var command = ParseCommand("assign a to abs \"1:0:0\" + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.ADD, variable.operand);
            Assert.IsTrue(variable.a is UniOperandVariable);
            UniOperandVariable operation = (UniOperandVariable)variable.a;
            Assert.AreEqual(UniOperandType.ABS, operation.operand);
            Assert.AreEqual(PrimitiveType.VECTOR, operation.a.GetValue().GetPrimitiveType());
            Assert.AreEqual(PrimitiveType.NUMERIC, variable.a.GetValue().GetPrimitiveType());
            Assert.AreEqual(3f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSquaeRootValue() {
            var command = ParseCommand("assign a to sqrt 9 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSquareRootValueVector() {
            var command = ParseCommand("assign a to sqrt \"9:0:0\" + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.ADD, variable.operand);
            Assert.IsTrue(variable.a is UniOperandVariable);
            UniOperandVariable operation = (UniOperandVariable)variable.a;
            Assert.AreEqual(UniOperandType.SQRT, operation.operand);
            Assert.AreEqual(PrimitiveType.VECTOR, operation.a.GetValue().GetPrimitiveType());
            Assert.AreEqual(PrimitiveType.NUMERIC, variable.a.GetValue().GetPrimitiveType());
            Assert.AreEqual(5f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleAddition() {
            var command = ParseCommand("assign a to 3 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleSubtraction() {
            var command = ParseCommand("assign a to 3 - 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.SUBTACT, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleMultiplication() {
            var command = ParseCommand("assign a to 3 * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.MULTIPLY, variable.operand);
            Assert.AreEqual(6f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleDivision() {
            var command = ParseCommand("assign a to 6 / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.DIVIDE, variable.operand);
            Assert.AreEqual(3f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleMod()
        {
            var command = ParseCommand("assign a to 5 % 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.MOD, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleAdditionVariable() {
            var command = ParseCommand("assign a to {b} + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.ADD, variable.operand);
            Assert.IsTrue(variable.a is InMemoryVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AssignSimpleSubtractionVariable() {
            var command = ParseCommand("assign a to {b} - 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.SUBTACT, variable.operand);
            Assert.IsTrue(variable.a is InMemoryVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AssignSimpleMultiplicationVariable() {
            var command = ParseCommand("assign a to {b} * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.MULTIPLY, variable.operand);
            Assert.IsTrue(variable.a is InMemoryVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AssignSimpleDivisionVariable() {
            var command = ParseCommand("assign a to {b} / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.DIVIDE, variable.operand);
            Assert.IsTrue(variable.a is InMemoryVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void MultiplicationBeforeAddition() {
            var command = ParseCommand("assign a to 4 * 2 + 3");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable addVariable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(11f, CastNumber(addVariable.GetValue()).GetNumericValue());
            Assert.AreEqual(BiOperandType.ADD, addVariable.operand);
            Assert.IsTrue(addVariable.a is BiOperandVariable);
            Assert.IsTrue(addVariable.b is StaticVariable);
            BiOperandVariable multiplyVariable = (BiOperandVariable)addVariable.a;
            Assert.AreEqual(BiOperandType.MULTIPLY, multiplyVariable.operand);
        }

        [TestMethod]
        public void DivisionBeforeAddition() {
            var command = ParseCommand("assign a to 4 / 2 + 3");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable addVariable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(5f, CastNumber(addVariable.GetValue()).GetNumericValue());
            Assert.AreEqual(BiOperandType.ADD, addVariable.operand);
            Assert.IsTrue(addVariable.a is BiOperandVariable);
            Assert.IsTrue(addVariable.b is StaticVariable);
            BiOperandVariable multiplyVariable = (BiOperandVariable)addVariable.a;
            Assert.AreEqual(BiOperandType.DIVIDE, multiplyVariable.operand);
        }

        [TestMethod]
        public void AdditionBeforeVariableComparison() {
            var command = ParseCommand("assign a to {b} + 1 > 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is ComparisonVariable);
            ComparisonVariable variable = (ComparisonVariable)assignment.variable;
            Assert.IsTrue(variable.a is BiOperandVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AdditionBeforeBooleanLogic() {
            var command = ParseCommand("assign a to {b} + 1 and {c}");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.AND, variable.operand);
            Assert.IsTrue(variable.a is BiOperandVariable);
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
            Assert.IsTrue(blockCondition.comparisonValue is BiOperandVariable);
        }

        [TestMethod]
        public void AssignColor() {
            var command = ParseCommand("assign a to \"red\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Red, primitive.GetValue());
        }

        [TestMethod]
        public void AssignColorFromHex() {
            var command = ParseCommand("assign a to \"#ff0000\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Red, primitive.GetValue());
        }

        [TestMethod]
        public void AssignAddedColors() {
            var command = ParseCommand("assign a to \"#ff0000\" + \"#00ff00\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Yellow, primitive.GetValue());
        }

        [TestMethod]
        public void AssignSubtractedColors() {
            var command = ParseCommand("assign a to \"#ffff00\" - \"#00ff00\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Red, primitive.GetValue());
        }

        [TestMethod]
        public void AssignMultipliedColor() {
            var command = ParseCommand("assign a to \"#112233\" * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual("#224466", CastString(primitive).GetStringValue());
        }

        [TestMethod]
        public void AssignDividedColor() {
            var command = ParseCommand("assign a to \"#224466\" / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual("#112233", CastString(primitive).GetStringValue());
        }

        [TestMethod]
        public void AssignNotColor() {
            var command = ParseCommand("assign a to not \"red\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Cyan, primitive.GetValue());
        }
    }
}
