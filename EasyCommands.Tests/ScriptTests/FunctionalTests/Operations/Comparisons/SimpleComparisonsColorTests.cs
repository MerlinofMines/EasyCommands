using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleComparisonsColorTests {

        [TestMethod]
        public void ColorEqualTo() {
            var lines = new List<String> {
                "red == #FF0000",
                "blue == #FF0000" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }

        [TestMethod]
        public void ColorNotEqualTo() {
            var lines = new List<String> {
                "black != #FFFFFF",
                "white != #FFFFFF" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], lines[0]);
                Assert.AreEqual("False", test.Logger[1], lines[1]);
            }
        }
    }
}
