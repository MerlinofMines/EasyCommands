using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimplePowerOperatorTests {
        [TestMethod]
        public void BoolPowerAsXOR() {
            using (var test = new ScriptTest(@"print true ^ false")) {

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void BoolPowerAsXORWhenFalse() {
            using (var test = new ScriptTest(@"print false ^ false")) {

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void VectorPowerAsAngleBetween() {
            using (var test = new ScriptTest(@"print 0:0:1 ^ 1:0:0")) {

                test.RunOnce();

                Assert.AreEqual("90", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PowerOperationBeforeMultiplication() {
            using (var test = new ScriptTest(@"print 2 * 3 ^ 2")) {

                test.RunOnce();

                Assert.AreEqual("18", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PowerOperationBeforeDivision() {
            using (var test = new ScriptTest(@"print 100 / 5 ^ 2")) {

                test.RunOnce();

                Assert.AreEqual("4", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PowerOperationBeforeModulus() {
            using (var test = new ScriptTest(@"print 90 % 5 ^ 2")) {

                test.RunOnce();

                Assert.AreEqual("15", test.Logger[0]);
            }
        }
    }
}
