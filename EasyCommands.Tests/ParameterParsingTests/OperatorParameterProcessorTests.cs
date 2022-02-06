using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using VRageMath;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class OperatorParameterProcessorTests : ForceLocale {
        [TestMethod]
        public void AssignAbsoluteValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to abs -3 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSin() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to sin 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.SIN, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignCos() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to cos 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.COS, variable.operand);
            Assert.AreEqual((float)Math.Cos(1.5708f), CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignTan() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to tan 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.TAN, variable.operand);
            Assert.AreEqual((float)Math.Tan(1.5708f), CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignASin() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to asin 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.ASIN, variable.operand);
            Assert.AreEqual((float)Math.Asin(1.5708f), CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignACos() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to acos 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.ACOS, variable.operand);
            Assert.AreEqual((float)Math.Acos(1.5708f), CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignATan() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to atan 1.5708");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.ATAN, variable.operand);
            Assert.AreEqual((float)Math.Atan(1.5708f), CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignNaturalLog() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to ln 1.5");
            Assert.IsTrue(command is VariableAssignmentCommand);
            var assignment = command as VariableAssignmentCommand;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            var variable = assignment.variable as UniOperandVariable;
            Assert.AreEqual(UniOperand.LN, variable.operand);
            Assert.AreEqual((float)Math.Log(1.5f), CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSign() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to sign -0.5");
            Assert.IsTrue(command is VariableAssignmentCommand);
            var assignment = command as VariableAssignmentCommand;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            var variable = assignment.variable as UniOperandVariable;
            Assert.AreEqual(UniOperand.SIGN, variable.operand);
            Assert.AreEqual(-1f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignRoundDown() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to round 5.4");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.ROUND, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignRoundUp() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to round 5.6");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.ROUND, variable.operand);
            Assert.AreEqual(6f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignRoundVector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to round (5.6:0:5.4)");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.ROUND, variable.operand);
            Assert.AreEqual(new Vector3D(6f, 0f, 5f), CastVector(variable.GetValue()));
        }

        [TestMethod]
        public void AssignAbsoluteValueVector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to abs 1:0:0 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.ADD, variable.operand);
            Assert.IsTrue(variable.a is UniOperandVariable);
            UniOperandVariable operation = (UniOperandVariable)variable.a;
            Assert.AreEqual(UniOperand.ABS, operation.operand);
            Assert.AreEqual(Return.VECTOR, operation.a.GetValue().returnType);
            Assert.AreEqual(Return.NUMERIC, variable.a.GetValue().returnType);
            Assert.AreEqual(3f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSquareRootValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to sqrt 9 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSquareRootValueVector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to sqrt 9:0:0 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.ADD, variable.operand);
            Assert.IsTrue(variable.a is UniOperandVariable);
            UniOperandVariable operation = (UniOperandVariable)variable.a;
            Assert.AreEqual(UniOperand.SQRT, operation.operand);
            Assert.AreEqual(Return.VECTOR, operation.a.GetValue().returnType);
            Assert.AreEqual(Return.NUMERIC, variable.a.GetValue().returnType);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleAddition() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 3 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.ADD, variable.operand);
            Assert.AreEqual(5f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleSubtraction() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 3 - 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleStringSubtraction() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"test\" - t");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.AreEqual("est", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void AssignNegativeVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to -t");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)assignment.variable;
            Assert.AreEqual(UniOperand.REVERSE, variable.operand);
            Assert.IsTrue(variable.a is AmbiguousStringVariable);
            AmbiguousStringVariable memoryVariable = (AmbiguousStringVariable)variable.a;
            Assert.AreEqual("t", memoryVariable.value);
        }

        [TestMethod]
        public void AssignSimpleStringSubtractionMultipleCharacters() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"second\" - eco");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.AreEqual("snd", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleStringSubtractionEmptyString() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"second\" - \"\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.AreEqual("second", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleStringSubtractionLastCharacter() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"second\" - d");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.AreEqual("secon", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleStringSubtractionDoesNotContain() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"second\" - f");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.AreEqual("second", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleStringSubtractionSubString() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"second\" - 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.AreEqual("seco", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleStringSubtractionSubStringMoreThanLength() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"second\" - 6");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.AreEqual("", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleMultiplication() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 3 * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.MULTIPLY, variable.operand);
            Assert.AreEqual(6f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignVectorNumericAddition() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 1:0:0 + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.ADD, variable.operand);
            Assert.AreEqual(new Vector3D(3, 0, 0), CastVector(variable.GetValue()));
        }

        [TestMethod]
        public void AssignVectorNumericSubtraction() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 1:0:0 - 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.AreEqual(new Vector3D(-1, 0, 0), CastVector(variable.GetValue()));
        }

        [TestMethod]
        public void AssignVectorNumericMultiplication() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 1:0:0 * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.MULTIPLY, variable.operand);
            Assert.AreEqual(new Vector3D(2, 0, 0), CastVector(variable.GetValue()));
        }

        [TestMethod]
        public void AssignVectorNumericDivision() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 2:0:0 / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.DIVIDE, variable.operand);
            Assert.AreEqual(new Vector3D(1, 0, 0), CastVector(variable.GetValue()));
        }

        [TestMethod]
        public void AssignVectorMultiplication() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 0:1:0 * 1:0:0");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.MULTIPLY, variable.operand);
            Assert.AreEqual(new Vector3D(0,0,-1), CastVector(variable.GetValue()));
        }

        [TestMethod]
        public void AssignVectorDotProduct() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 0:1:0 . 1:0:0");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.DOT, variable.operand);
            Assert.AreEqual(0f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignVectorSign() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to sign -0.5:1.5:0");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is UniOperandVariable);
            var variable = assignment.variable as UniOperandVariable;
            Assert.AreEqual(UniOperand.SIGN, variable.operand);
            Assert.AreEqual(new Vector3D(-1,1,0), CastVector(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleDivision() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 6 / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.DIVIDE, variable.operand);
            Assert.AreEqual(3f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleMod() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 5 % 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.MOD, variable.operand);
            Assert.AreEqual(1f, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignStringMod() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to test % t");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.MOD, variable.operand);
            Assert.AreEqual("es", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void AssignVectorMod() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 1:1:0 % 0:1:0");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.MOD, variable.operand);
            Assert.AreEqual(new Vector3D(1,0,0), CastVector(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleExponent() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 2 ^ 4");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.EXPONENT, variable.operand);
            Assert.AreEqual(16, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignBoolExponentAsXOR() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to true ^ false");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.EXPONENT, variable.operand);
            Assert.IsTrue(CastBoolean(variable.GetValue()));
        }

        [TestMethod]
        public void AssignBoolExponentAsXORWhenFalse() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to true xor true");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.EXPONENT, variable.operand);
            Assert.IsFalse(CastBoolean(variable.GetValue()));
        }

        [TestMethod]
        public void AssignVectorExponentAsAngleBetween() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 0:0:1 ^ 1:0:0");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.EXPONENT, variable.operand);
            Assert.AreEqual(90, CastNumber(variable.GetValue()));
        }

        [TestMethod]
        public void AssignSimpleAdditionVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to b + 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.ADD, variable.operand);
            Assert.IsTrue(variable.a is AmbiguousStringVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AssignSimpleSubtractionVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to b - 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.SUBTRACT, variable.operand);
            Assert.IsTrue(variable.a is AmbiguousStringVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AssignSimpleMultiplicationVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to b * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.MULTIPLY, variable.operand);
            Assert.IsTrue(variable.a is AmbiguousStringVariable);
            Assert.IsTrue(variable.b is StaticVariable);
        }

        [TestMethod]
        public void AssignSimpleDivisionVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to b / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.DIVIDE, variable.operand);
            Assert.IsTrue(variable.a is AmbiguousStringVariable);
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
            Assert.AreEqual(11f, CastNumber(addVariable.GetValue()));
            Assert.AreEqual(BiOperand.ADD, addVariable.operand);
            Assert.IsTrue(addVariable.a is BiOperandVariable);
            Assert.IsTrue(addVariable.b is StaticVariable);
            BiOperandVariable multiplyVariable = (BiOperandVariable)addVariable.a;
            Assert.AreEqual(BiOperand.MULTIPLY, multiplyVariable.operand);
        }

        [TestMethod]
        public void DivisionBeforeAddition() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 4 / 2 + 3");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable addVariable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(5f, CastNumber(addVariable.GetValue()));
            Assert.AreEqual(BiOperand.ADD, addVariable.operand);
            Assert.IsTrue(addVariable.a is BiOperandVariable);
            Assert.IsTrue(addVariable.b is StaticVariable);
            BiOperandVariable multiplyVariable = (BiOperandVariable)addVariable.a;
            Assert.AreEqual(BiOperand.DIVIDE, multiplyVariable.operand);
        }

        [TestMethod]
        public void AdditionBeforeVariableComparison() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to b + 1 > 2");
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
            var command = program.ParseCommand("assign a to b + 1 and c");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.AND, variable.operand);
            Assert.IsTrue(variable.a is BiOperandVariable);
            Assert.IsTrue(variable.b is AmbiguousStringVariable);
        }

        [TestMethod]
        public void TernaryOperator() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign e to a > b ? c : d + 1");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is TernaryConditionVariable);
            TernaryConditionVariable variable = (TernaryConditionVariable)assignment.variable;
            Assert.IsTrue(variable.condition is ComparisonVariable);
            ComparisonVariable condition = (ComparisonVariable)variable.condition;
            Assert.IsTrue(condition.a is AmbiguousStringVariable);
            Assert.IsTrue(condition.b is AmbiguousStringVariable);
            Assert.AreEqual("a", ((AmbiguousStringVariable)condition.a).value);
            Assert.AreEqual("b", ((AmbiguousStringVariable)condition.b).value);
            Assert.IsTrue(variable.positiveValue is AmbiguousStringVariable);
            Assert.AreEqual("c", ((AmbiguousStringVariable)variable.positiveValue).value);
            Assert.IsTrue(variable.negativeValue is BiOperandVariable);
        }

        [TestMethod]
        public void ComparisonBeforeBooleanLogic() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign c to a > 0 and b < 1");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignment.variable is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)assignment.variable;
            Assert.AreEqual(BiOperand.AND, variable.operand);
            Assert.IsTrue(variable.a is ComparisonVariable);
            Assert.IsTrue(variable.b is ComparisonVariable);
            ComparisonVariable a = (ComparisonVariable)variable.a;
            Assert.IsTrue(a.a is AmbiguousStringVariable);
            Assert.IsTrue(a.b is StaticVariable);
            ComparisonVariable b = (ComparisonVariable)variable.b;
            Assert.IsTrue(b.a is AmbiguousStringVariable);
            Assert.IsTrue(b.b is StaticVariable);
        }

        [TestMethod]
        public void AdditionUsedAsBlockConditionVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("if the \"rotor\" angle > a + 30 set the \"rotor\" angle to a");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.condition is AggregateConditionVariable);
            AggregateConditionVariable condition = (AggregateConditionVariable)conditionalCommand.condition;
            PropertySupplier property = GetDelegateProperty<PropertySupplier>("property", condition.blockCondition);
            Assert.AreEqual(Property.ANGLE + "", property.propertyType);
            BiOperandVariable comparisonValue = GetDelegateProperty<BiOperandVariable>("comparisonValue", condition.blockCondition);
            Assert.IsTrue(comparisonValue.a is AmbiguousStringVariable);
            Assert.IsTrue(comparisonValue.b is StaticVariable);
            Assert.AreEqual("a", comparisonValue.a.GetValue().value);
            Assert.AreEqual(30f, comparisonValue.b.GetValue().value);
        }

        [TestMethod]
        public void AssignColor() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to red");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(Return.COLOR, primitive.returnType);
            Assert.AreEqual(Color.Red, primitive.value);
        }

        [TestMethod]
        public void AssignColorFromHex() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to #ff0000");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(Return.COLOR, primitive.returnType);
            Assert.AreEqual(Color.Red, primitive.value);
        }

        [TestMethod]
        public void AssignAddedColors() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to #ff0000 + #00ff00");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(Return.COLOR, primitive.returnType);
            Assert.AreEqual(Color.Yellow, primitive.value);
        }

        [TestMethod]
        public void AssignSubtractedColors() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to #ffff00 - #00ff00");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(Return.COLOR, primitive.returnType);
            Assert.AreEqual(Color.Red, primitive.value);
        }

        [TestMethod]
        public void AssignMultipliedColor() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to #112233 * 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(Return.COLOR, primitive.returnType);
            Assert.AreEqual("#224466", CastString(primitive));
        }

        [TestMethod]
        public void AssignDividedColor() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to #224466 / 2");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(Return.COLOR, primitive.returnType);
            Assert.AreEqual("#112233", CastString(primitive));
        }

        [TestMethod]
        public void AssignNotColor() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to not red");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive primitive = assignment.variable.GetValue();
            Assert.AreEqual(Return.COLOR, primitive.returnType);
            Assert.AreEqual(Color.Cyan, primitive.value);
        }

        [TestMethod]
        public void AddNumberToList() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to [0, 1, 2] + 3");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            List<Variable> listValues = CastList(assignment.variable.GetValue()).GetValues();
            Assert.AreEqual(4, listValues.Count);
            Assert.AreEqual(0f, listValues[0].GetValue().value);
            Assert.AreEqual(1f, listValues[1].GetValue().value);
            Assert.AreEqual(2f, listValues[2].GetValue().value);
            Assert.AreEqual(3f, listValues[3].GetValue().value);
        }

        [TestMethod]
        public void AddNumberToListInFront() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to 0 + [1, 2, 3]");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            List<Variable> listValues = CastList(assignment.variable.GetValue()).GetValues();
            Assert.AreEqual(4, listValues.Count);
            Assert.AreEqual(0f, listValues[0].GetValue().value);
            Assert.AreEqual(1f, listValues[1].GetValue().value);
            Assert.AreEqual(2f, listValues[2].GetValue().value);
            Assert.AreEqual(3f, listValues[3].GetValue().value);
        }

        [TestMethod]
        public void AddStringToList() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to [0, 1, 2] + \" three\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.AreEqual("[0,1,2] three", CastString(assignment.variable.GetValue()));
        }

        [TestMethod]
        public void AddStringToListInFront() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"zero \" + [0, 1, 2]");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Assert.AreEqual("zero [0,1,2]", CastString(assignment.variable.GetValue()));
        }

        [TestMethod]
        public void AddTwoLists() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to [0, 1, 2] + [3, 4, 5]");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            List<Variable> listValues = CastList(assignment.variable.GetValue()).GetValues();
            Assert.AreEqual(6, listValues.Count);
            Assert.AreEqual(0f, listValues[0].GetValue().value);
            Assert.AreEqual(1f, listValues[1].GetValue().value);
            Assert.AreEqual(2f, listValues[2].GetValue().value);
            Assert.AreEqual(3f, listValues[3].GetValue().value);
            Assert.AreEqual(4f, listValues[4].GetValue().value);
            Assert.AreEqual(5f, listValues[5].GetValue().value);
        }

        [TestMethod]
        public void CastStringAsVector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to (\"1\" + \":2:\" + \"3\") as \"vector\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive value = assignment.variable.GetValue();
            Assert.AreEqual(Return.VECTOR, value.returnType);
            Assert.AreEqual("1:2:3", CastString(value));
        }

        [TestMethod]
        public void CastStringAsBoolean() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"true\" as \"bool\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive value = assignment.variable.GetValue();
            Assert.AreEqual(Return.BOOLEAN, value.returnType);
            Assert.AreEqual(true, value.value);
        }

        [TestMethod]
        public void SplitStringBySubString() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"My Value\" split \" \"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive value = assignment.variable.GetValue();
            Assert.AreEqual(Return.LIST, value.returnType);
            Assert.AreEqual("[My,Value]", CastString(value));
        }

        [TestMethod]
        public void SplitStringByNewLine() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to \"My\nValue\" split \"\\n\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive value = assignment.variable.GetValue();
            Assert.AreEqual(Return.LIST, value.returnType);
            Assert.AreEqual("[My,Value]", CastString(value));
        }

        [TestMethod]
        public void JoinListByString() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to [1,2,3] joined \", \"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive value = assignment.variable.GetValue();
            Assert.AreEqual(Return.STRING, value.returnType);
            Assert.AreEqual("1, 2, 3", CastString(value));
        }

        [TestMethod]
        public void JoinListByNewLine() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to [1,2,3] joined \"\\n\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignment = (VariableAssignmentCommand)command;
            Primitive value = assignment.variable.GetValue();
            Assert.AreEqual(Return.STRING, value.returnType);
            Assert.AreEqual("1\n2\n3", value.value);
        }
    }
}
