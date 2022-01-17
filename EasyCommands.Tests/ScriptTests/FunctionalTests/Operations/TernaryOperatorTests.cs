using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class TernaryOperatorTests {

        [TestMethod]
        public void TernaryOperator() {
            var lines = new List<String> {
                @"True ? ""Success"" : ""Fail""",
                @"False ? ""Success"" : ""Fail""" };

            using (var test = new SimpleExpressionsTest(lines)) {
                test.RunOnce();

                Assert.AreEqual("Success", test.Logger[0], lines[0]);
                Assert.AreEqual("Fail", test.Logger[1], lines[1]);
            }
        }
    }
}
