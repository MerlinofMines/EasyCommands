using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleRandomTests {

        [TestMethod]
        public void RandomNumber() {
            using (var test = new ScriptTest(@"print ""Next Random: "" + rand 10")) {
                test.MockNextBoundedRandoms(10, 3);

                test.RunOnce();

                Assert.AreEqual("Next Random: 3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RandNumber() {
            using (var test = new ScriptTest(@"print ""Next Random: "" + rand 10")) {
                test.MockNextBoundedRandoms(10, 6);

                test.RunOnce();

                Assert.AreEqual("Next Random: 6", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RandomItemFromList() {
            using (var test = new ScriptTest(@"print ""Next Random: "" + rand [one, two, three, four, five]")) {
                test.MockNextBoundedRandoms(5, 3);

                test.RunOnce();

                Assert.AreEqual("Next Random: four", test.Logger[0]);
            }
        }
    }
}
