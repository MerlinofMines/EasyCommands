using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleComparisonsBooleanTests {
        [TestMethod]
        public void LessThan() {
            var lines = new List<String>{ "False < True", "True < False", "False < False", "True < True" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
                Assert.AreEqual("False", test.Logger[3], lines[3]);
            }
        }

        [TestMethod]
        public void LessThanOrEqual() {
            var lines = new List<String>{ "False <= True", "True <= False", "False <= False", "True <= True" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("True", test.Logger[2], lines[2]);
                Assert.AreEqual("True", test.Logger[3], lines[3]);
            }
        }

        [TestMethod]
        public void GreaterThan() {
            var lines = new List<String>{ "True > False", "False > True", "False > False", "True > True" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
                Assert.AreEqual("False", test.Logger[3], lines[3]);
            }
        }

        [TestMethod]
        public void GreaterThanOrEqual() {
            var lines = new List<String>{ "True >= False", "False >= True", "False >= False", "True >= True" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("True", test.Logger[2], lines[2]);
                Assert.AreEqual("True", test.Logger[3], lines[3]);
            }
        }

        [TestMethod]
        public void EqualTo() {
            var lines = new List<String>{ "False == False", "True == True", "False == True", "True == False" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("True", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
                Assert.AreEqual("False", test.Logger[3], lines[3]);
            }
        }

        [TestMethod]
        public void NotEqualTo() {
            var lines = new List<String>{ "False != True", "True != False", "False != False", "True != True" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("True", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
                Assert.AreEqual("False", test.Logger[3], lines[3]);
            }
        }
    }
}
