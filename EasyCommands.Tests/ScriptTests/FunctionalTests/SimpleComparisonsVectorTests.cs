using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleComparisonsVectorTests {
        [TestMethod]
        public void EqualTo() {
            var lines = new List<String> {
                "x == 1:0:0",
                "y == 1:0:0" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }

        [TestMethod]
        public void NotEqualTo() {
            var lines = new List<String> {
                "x != 0:1:0",
                "y != 0:1:0" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }

        [TestMethod]
        public void LessThanNumber() {
            var lines = new List<String> {
                "x < 2",
                "0 < x",
                "x < 1"
            };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("True", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void GreaterThanNumber() {
            var lines = new List<String> {
                "x > 0",
                "2 > x",
                "x > 1"
            };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("True", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void EqualToNumber() {
            var lines = new List<String> {
                "x == 1",
                "1 == x",
                "x == 0"
            };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("True", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void NotEqualToNumber() {
            var lines = new List<String> {
                "x != 2",
                "0 != x",
                "x != 1"
            };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("True", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }
    }
}
