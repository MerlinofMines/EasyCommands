using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IngameScript.Program;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class ComputedVariableTests : ForceLocale {
        class Input : IVariable {
            public float value;
            public int reads;
            public Primitive GetValue() { reads++; return ResolvePrimitive(value); }
        }

        [TestMethod]
        public void ExpressionsStayLazyAndReadChangingInputsOnEveryEvaluation() {
            CreateProgram();
            var a = new Input { value = 2 };
            var b = new Input { value = 3 };
            var unary = UniOperandVariable(UniOperand.REVERSE, a);
            var binary = BiOperandVariable(BiOperand.ADD, a, b);
            var comparison = ComparisonVariable(a, b, (left, right) => left.CompareTo(right) < 0);
            Assert.AreEqual(0, a.reads);
            Assert.AreEqual(0, b.reads);
            Assert.AreEqual(-2f, CastNumber(unary.GetValue()));
            Assert.AreEqual(5f, CastNumber(binary.GetValue()));
            Assert.IsTrue(CastBoolean(comparison.GetValue()));
            a.value = 5;
            Assert.AreEqual(-5f, CastNumber(unary.GetValue()));
            Assert.AreEqual(8f, CastNumber(binary.GetValue()));
            Assert.IsFalse(CastBoolean(comparison.GetValue()));
            Assert.AreEqual(6, a.reads);
            Assert.AreEqual(4, b.reads);
        }

        [TestMethod]
        public void FactoryCallsKeepIndependentCapturedOperands() {
            CreateProgram();
            var firstInput = new Input { value = 2 };
            var secondInput = new Input { value = 10 };
            var first = BiOperandVariable(BiOperand.ADD, firstInput, GetStaticVariable(3));
            var second = BiOperandVariable(BiOperand.MULTIPLY, secondInput, GetStaticVariable(4));
            Assert.AreEqual(5f, CastNumber(first.GetValue()));
            Assert.AreEqual(40f, CastNumber(second.GetValue()));
            firstInput.value = 7;
            Assert.AreEqual(10f, CastNumber(first.GetValue()));
            Assert.AreEqual(40f, CastNumber(second.GetValue()));
        }
    }
}
