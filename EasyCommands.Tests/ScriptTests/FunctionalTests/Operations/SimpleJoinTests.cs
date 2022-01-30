using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleJoinTests {

        [TestMethod]
        public void JoinNonKeyedList() {
            using (var test = new ScriptTest(@"print [1,2,3] joined "", """)) {

                test.RunOnce();

                Assert.AreEqual("1, 2, 3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void JoinKeyedList() {
            using (var test = new ScriptTest(@"print [""one"" -> 1, ""two"" -> 2, ""three"" -> 3] joined "", """)) {
                test.MockNextBoundedRandoms(10, 6);

                test.RunOnce();

                Assert.AreEqual("one->1, two->2, three->3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void JoinKeyedListByNewLine() {
            using (var test = new ScriptTest("print [1,2,3] joined \"\\n\"")) {

                test.RunOnce();

                Assert.AreEqual("1\n2\n3", test.Logger[0]);
            }
        }
    }
}
