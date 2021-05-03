﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VRageMath;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class OperatorParameterProcessorTests {

        [TestMethod]
        public void AssignAbsoluteValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to abs -3 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSin() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to sin 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperandType.SIN, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignCos() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to cos 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperandType.COS, variable.operand);
            Assert.AreEqual((float)Math.Cos(1.5708f), CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignTan() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to tan 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperandType.TAN, variable.operand);
            Assert.AreEqual((float)Math.Tan(1.5708f), CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignASin() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to asin 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperandType.ASIN, variable.operand);
            Assert.AreEqual((float)Math.Asin(1.5708f), CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignACos() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to acos 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperandType.ACOS, variable.operand);
            Assert.AreEqual((float)Math.Acos(1.5708f), CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignATan() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to atan 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperandType.ATAN, variable.operand);
            Assert.AreEqual((float)Math.Atan(1.5708f), CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignRoundDown() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to round 5.4");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperandType.ROUND, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignRoundUp() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to round 5.6");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperandType.ROUND, variable.operand);
            Assert.AreEqual(6f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignAbsoluteValueVector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to abs \"1:0:0\" + 2");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to sqrt 9 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSquareRootValueVector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to sqrt \"9:0:0\" + 2");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 3 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleSubtraction() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 3 - 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.SUBTACT, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleMultiplication() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 3 * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.MULTIPLY, variable.operand);
            Assert.AreEqual(6f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignVectorMultiplication() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"0:1:0\" * \"1:0:0\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.MULTIPLY, variable.operand);
            Assert.AreEqual(new Vector3D(0,0,-1), CastVector(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignVectorDotProduct() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"0:1:0\" . \"1:0:0\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.DOT, variable.operand);
            Assert.AreEqual(0f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleDivision() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 6 / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.DIVIDE, variable.operand);
            Assert.AreEqual(3f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleMod() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 5 % 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.MOD, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignVectorMod() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"1:1:0\" % \"0:1:0\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperandType.MOD, variable.operand);
            Assert.AreEqual(new Vector3D(1,0,0), CastVector(variable.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignSimpleAdditionVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to {b} + 2");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to {b} - 2");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to {b} * 2");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to {b} / 2");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 4 * 2 + 3");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 4 / 2 + 3");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to {b} + 1 > 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is ComparisonVariable);
            ComparisonVariable variable = (ComparisonVariable)assignment.variable;
            Assert.IsTrue(variable.a is BiOperandVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AdditionBeforeBooleanLogic() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to {b} + 1 and {c}");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("if the \"rotor\" angle > {a} + 30 set the \"rotor\" angle to {a}");
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
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"red\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Red, primitive.GetValue());
        }

        [TestMethod]
        public void AssignColorFromHex() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"#ff0000\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Red, primitive.GetValue());
        }

        [TestMethod]
        public void AssignAddedColors() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"#ff0000\" + \"#00ff00\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Yellow, primitive.GetValue());
        }

        [TestMethod]
        public void AssignSubtractedColors() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"#ffff00\" - \"#00ff00\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Red, primitive.GetValue());
        }

        [TestMethod]
        public void AssignMultipliedColor() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"#112233\" * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual("#224466", CastString(primitive).GetStringValue());
        }

        [TestMethod]
        public void AssignDividedColor() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"#224466\" / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual("#112233", CastString(primitive).GetStringValue());
        }

        [TestMethod]
        public void AssignNotColor() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to not \"red\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(PrimitiveType.COLOR, primitive.GetPrimitiveType());
            Assert.AreEqual(Color.Cyan, primitive.GetValue());
        }
    }
}
