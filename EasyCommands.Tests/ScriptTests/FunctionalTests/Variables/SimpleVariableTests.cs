using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleVariableTests {
        [TestMethod]
        public void TestImplicitVariableAssignmentAndGetValue() {
            var script = @"
assign a to 1
assign b to a + 1
Print ""b is: "" + b
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("b is: 2"));
            }
        }

        [TestMethod]
        public void TestImplicitStringValue() {
            var script = @"
assign b to a + 1
Print ""b is: "" + b
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("b is: a1"));
            }
        }

        [TestMethod]
        public void AssignVariableWithSet() {
            var script = @"
set b to a + 1
Print ""b is: "" + b
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("b is: a1"));
            }
        }

        [TestMethod]
        public void AssignGlobalVariableWithSet() {
            var script = @"
set global b to a + 1
Print ""b is: "" + b
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("b is: a1"));
            }
        }

        [TestMethod]
        public void piIsAvailable() {
            var script = @"
Print ""Pi is: "" + pi
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Pi is: 3.141593"));
            }
        }

        [TestMethod]
        public void eIsAvailable() {
            var script = @"
Print ""e is: "" + e
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("e is: 2.718282"));
            }
        }

        [TestMethod]
        public void IncrementLocalVariable() {
            var script = @"
assign a to 1
a++
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 2"));
            }
        }

        [TestMethod]
        public void IncrementGlobalVariable() {
            var script = @"
assign global a to 1
a++
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 2"));
            }
        }

        [TestMethod]
        public void IncrementLocalVariableByAmount() {
            var script = @"
assign a to 1
a+=2
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 3"));
            }
        }

        [TestMethod]
        public void IncrementLocalVariableByVariableAmount() {
            var script = @"
assign a to 1
assign b to 2
a+=b
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 3"));
            }
        }

        [TestMethod]
        public void DecrementLocalVariable() {
            var script = @"
assign a to 1
a--
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 0"));
            }
        }

        [TestMethod]
        public void DecrementGlobalVariable() {
            var script = @"
assign global a to 1
a--
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 0"));
            }
        }

        [TestMethod]
        public void DecrementLocalVariableByAmount() {
            var script = @"
assign a to 3
a-=2
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 1"));
            }
        }

        [TestMethod]
        public void DecrementLocalVariableByVariableAmount() {
            var script = @"
assign a to 3
assign b to 2
a-=b
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 1"));
            }
        }

        [TestMethod]
        public void ReverseBoolean() {
            var script = @"
set a to true
Print ""a: "" + reverse a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ReverseNumber() {
            var script = @"
set a to 3
Print ""a: "" + reverse a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: -3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ReverseString() {
            var script = @"
set a to ""myString""
Print ""a: "" + reverse a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: gnirtSym", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ReverseVector() {
            var script = @"
set a to 1:2:3
Print ""a: "" + reverse a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: -1:-2:-3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ReverseColor() {
            var script = @"
set a to #FF00FF
Print ""a: "" + reverse a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: #00FF00", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ReverseList() {
            var script = @"
set a to [""one""-> 1,2,3]
Print ""a: "" + reverse a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: [3,2,one->1]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NegateList() {
            var script = @"
set a to [""one""-> 1,2,3]
Print ""a: "" + -a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: [3,2,one->1]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignNegativeVariable() {
            var script = @"
set a to 2
set b to -a
Print ""b: "" + b
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("b: -2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignNegativeVariablePlusValueNegativeResolvesFirst() {
            var script = @"
set a to 2
set b to -a+5
Print ""b: "" + b
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("b: 3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignNegativeVariableTimesValueNegativeResolvesFirst() {
            var script = @"
set a to 2
set b to -a*5
Print ""b: "" + b
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("b: -10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignNegatationTimesNegation() {
            var script = @"
set a to 2
set b to -a*-a
Print ""b: "" + b
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("b: 4", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignNegativeNumberMinusVariable() {
            var script = @"
set a to 2
set b to -2-a
Print ""b: "" + b
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("b: -4", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignNegativeVariableMinusNumber() {
            var script = @"
set a to 2
set b to -a-2
Print ""b: "" + b
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("b: -4", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignNegativeVariableMinusVariable() {
            var script = @"
set a to 2
set b to -a-a
Print ""b: "" + b
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("b: -4", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignDoubleNegative() {
            var script = @"
set a to 2
set b to 2 - -a
Print ""b: "" + b
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("b: 4", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignParenthesesWrappedSubtractionOperation() {
            var script = @"
set a to 2
set a to (2 - 3) - 1
print ""a: "" + a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: -2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignParenthesesWrappedNegation() {
            var script = @"
set a to 2
set a to (-3) - 1
print ""a: "" + a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: -4", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignParenthesesWrappedNegationAndSubtraction() {
            var script = @"
set a to 2
set a to (2 - -3) - 1
print ""a: "" + a
";

            using (var test = new ScriptTest(script)) {

                test.RunOnce();

                Assert.AreEqual("a: 4", test.Logger[0]);
            }
        }
    }
}
