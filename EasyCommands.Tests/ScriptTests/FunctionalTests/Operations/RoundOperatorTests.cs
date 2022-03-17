using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class RoundOperatorTests {

        #region numbers
        [TestMethod]
        public void RoundNumberUniOperandBeforeValue() {
            using (var test = new ScriptTest(@"print ""My Number: "" + round 9.81 + "" rounded""")) {

                test.RunOnce();

                Assert.AreEqual("My Number: 10 rounded", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RoundNumberUniOperandAfterValue() {
            using (var test = new ScriptTest(@"print ""My Number: "" + 9.81 rounded + "" rounded""")) {

                test.RunOnce();

                Assert.AreEqual("My Number: 10 rounded", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RoundNumberBiOperand() {
            using (var test = new ScriptTest(@"print ""My Number: "" + 3.14157 rounded to 2 + "" rounded""")) {

                test.RunOnce();

                Assert.AreEqual("My Number: 3.14 rounded", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RoundNumberBiOperandWithDigits() {
            using (var test = new ScriptTest(@"print ""My Number: "" + 3.14157 rounded to 3 digits + "" rounded""")) {

                test.RunOnce();

                Assert.AreEqual("My Number: 3.142 rounded", test.Logger[0]);
            }
        }
        #endregion

        #region vectors
        [TestMethod]
        public void RoundVectorUniOperandBeforeValue() {
            using (var test = new ScriptTest(@"print ""My Number: "" + round 1.2:2.3:3.4 + "" rounded""")) {

                test.RunOnce();

                Assert.AreEqual("My Number: 1:2:3 rounded", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RoundVectorUniOperandAfterValue() {
            using (var test = new ScriptTest(@"print ""My Number: "" + 1.2:2.3:3.4 rounded + "" rounded""")) {

                test.RunOnce();

                Assert.AreEqual("My Number: 1:2:3 rounded", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RoundVectorBiOperand() {
            using (var test = new ScriptTest(@"print ""My Number: "" + 1.23:2.34:3.45 rounded to 1 + "" rounded""")) {

                test.RunOnce();

                Assert.AreEqual("My Number: 1.2:2.3:3.4 rounded", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RoundVectorBiOperandWithDigits() {
            using (var test = new ScriptTest(@"print ""My Number: "" + 1.23:2.34:3.45 rounded to 1 digit + "" rounded""")) {

                test.RunOnce();

                Assert.AreEqual("My Number: 1.2:2.3:3.4 rounded", test.Logger[0]);
            }
        }
        #endregion

        #region InvalidRounding
        [TestMethod]
        public void CannotRoundString() {
            using (var test = new ScriptTest(@"print ""myString"" rounded")) {

                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot perform operation: round on type: string", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CannotRoundStringToDigits() {
            using (var test = new ScriptTest(@"print ""myString"" rounded to 2 digits")) {

                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot perform operation: round on types: string, number", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CannotRoundNumberToStringDigits() {
            using (var test = new ScriptTest(@"print 123 rounded to ""myString"" digits")) {

                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot perform operation: round on types: number, string", test.Logger[1]);
            }
        }
        #endregion
    }
}
