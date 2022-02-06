using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleStringTests {
        [TestMethod]
        public void AmbiguousString() {
            using (var test = new ScriptTest(@"Print ""String: "" + myString")) {
                test.RunOnce();

                Assert.AreEqual("String: myString", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AmbiguousStringInQuotes() {
            using (var test = new ScriptTest(@"Print ""String: "" + ""myString""")) {
                test.RunOnce();

                Assert.AreEqual("String: myString", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ExplicitStringVectorIsAString() {
            using (var test = new ScriptTest(@"
set a to 4
Print ""String: "" + ""a:b:c""
")) {
                test.RunOnce();

                Assert.AreEqual("String: a:b:c", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ExplicitStringColorIsAString() {
            using (var test = new ScriptTest(@"Print ""String: "" + ""red""")) {
                test.RunOnce();

                Assert.AreEqual("String: red", test.Logger[0]);
            }
        }
    }
}
