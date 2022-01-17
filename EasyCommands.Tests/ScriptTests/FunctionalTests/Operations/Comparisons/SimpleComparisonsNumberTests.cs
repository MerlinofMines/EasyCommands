using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleComparisonsNumberTests {
        [TestMethod]
        public void LessThan() {
            var lines = new List<String>{"2 < 3", "3 < 2", "2 < 2" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void NotLessThan() {
            var lines = new List<String>{ "3 not less than 3", "2 not less than 3" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }

        [TestMethod]
        public void LessThanOrEqual() {
            var lines = new List<String>{ "2 <= 3", "3 <= 2", "2 <= 2" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("True", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void NotLessThanOrEqual() {
            var lines = new List<String>{ "3 !<= 2", "2 !<= 3" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }

        [TestMethod]
        public void EqualTo() {
            var lines = new List<String>{ "2 == 2", "3 == 2", "2 == 3" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void NotEqualTo() {
            var lines = new List<String>{ "2 != 3", "2 is not equal to 3", "2 != 2" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("True", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void GreaterThan() {
            var lines = new List<String>{ "3 > 2", "2 > 3", "2 > 2" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void NotGreaterThan() {
            var lines = new List<String>{ "2 not greater than 3", "3 not greater than 2" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }

        [TestMethod]
        public void GreaterThanOrEqual() {
            var lines = new List<String>{ "3 >= 2", "2 >= 3", "2 >= 2" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("True", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void NotGreaterThanOrEqual() {
            var lines = new List<String>{ "2 !>= 3", "3 !>= 2" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }
    }
}
