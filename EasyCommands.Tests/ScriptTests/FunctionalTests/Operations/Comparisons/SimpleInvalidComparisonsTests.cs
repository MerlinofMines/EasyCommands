using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleInvalidComparisonsTests {
        [TestMethod]
        public void InvalidBooleanComparisons() {
            VerifyInvalid("true", "boolean", @"""string""", "string");
            VerifyInvalid("true", "boolean", "123", "number");
            VerifyInvalid("true", "boolean", "1:2:3", "vector");
            VerifyInvalid("true", "boolean", "#FF0000", "color");
            VerifyInvalid("true", "boolean", "[1,2,3]", "list");
        }

        [TestMethod]
        public void InvalidNumberComparisons() {
            VerifyInvalid("123", "number", "true", "boolean");
            VerifyInvalid("123", "number", @"""string""", "string");
            VerifyInvalid("123", "number", "#FF0000", "color");
            VerifyInvalid("123", "number", "[1,2,3]", "list");
        }

        [TestMethod]
        public void InvalidStringComparisons() {
            VerifyInvalid("string", "string", "true", "boolean");
            VerifyInvalid("string", "string", "123", "number");
            VerifyInvalid("string", "string", "1:2:3", "vector");
            VerifyInvalid("string", "string", "#FF0000", "color");
            VerifyInvalid("string", "string", "[1,2,3]", "list");
        }

        [TestMethod]
        public void InvalidVectorComparisons() {
            VerifyInvalid("1:2:3", "vector", "true", "boolean");
            VerifyInvalid("1:2:3", "vector", "string", "string");
            VerifyInvalid("1:2:3", "vector", "#FF0000", "color");
            VerifyInvalid("1:2:3", "vector", "[1,2,3]", "list");
        }

        [TestMethod]
        public void InvalidColorComparisons() {
            VerifyInvalid("#FF0000", "color", "true", "boolean");
            VerifyInvalid("#FF0000", "color", "string", "string");
            VerifyInvalid("#FF0000", "color", "123", "number");
            VerifyInvalid("#FF0000", "color", "1:2:3", "vector");
            VerifyInvalid("#FF0000", "color", "[1,2,3]", "list");
        }

        [TestMethod]
        public void InvalidListComparisons() {
            VerifyInvalid("([1,2,3])", "list", "true", "boolean");
            VerifyInvalid("([1,2,3])", "list", "string", "string");
            VerifyInvalid("([1,2,3])", "list", "123", "number");
            VerifyInvalid("([1,2,3])", "list", "1:2:3", "vector");
            VerifyInvalid("([1,2,3])", "list", "#FF0000", "color");
        }

        private void VerifyInvalid(string value1, string type1, string value2, string type2) {
            using (var test = new ScriptTest("print " + value1 + " = " + value2)) {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot perform operation: compare on types: " + type1 + ", " + type2, test.Logger[1]);
            }

        }
    }
}
