using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleComparisonsStringTests {
        [TestMethod]
        public void LessThan() {
            var lines = new List<String> {
                @"""cat"" is less than ""dog""",
                @"""dog"" is less than ""cat""",
                @"""cat"" is less than ""cat""" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void GreaterThan() {
            var lines = new List<String> {
                @"""dog"" is greater than ""cat""",
                @"""cat"" is greater than ""dog""",
                @"""cat"" is greater than ""cat""" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
                Assert.AreEqual("False", test.Logger[2], lines[2]);
            }
        }

        [TestMethod]
        public void EqualTo() {
            var lines = new List<String> {
                @"""cat"" is equal to ""cat""",
                @"""dog"" is equal to ""cat""" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }

        [TestMethod]
        public void NotEqualTo() {
            var lines = new List<String> {
                @"""cat"" is not equal to ""dog""",
                @"""cat"" is not equal to ""cat""" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }

        [TestMethod]
        public void Contains() {
            var lines = new List<String> {
                @"""cat in the hat"" contains ""cat""",
                @"""cat in the hat"" contains ""dog""" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }

        [TestMethod]
        public void DoesNotContain() {
            var lines = new List<String> {
                @"""cat in the hat"" does not contain ""dog""",
                @"""cat in the hat"" does not contain ""cat""" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }
    }
}
