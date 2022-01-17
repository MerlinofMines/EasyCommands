using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleShuffleTests {

        [TestMethod]
        public void ShuffleList() {
            using (var test = new ScriptTest(@"print ""Shuffled List: "" + shuffle [1,2,3]")) {
                test.MockNextRandoms(3, 1, 2);

                test.RunOnce();

                Assert.AreEqual("Shuffled List: [2,3,1]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ShuffleListDoesNotImpactOriginalList() {
            using (var test = new ScriptTest(@"
set myList to [1,2,3]
print ""Shuffled List: "" + shuffle [1,2,3]
print ""My List: "" + myList
")) {
                test.MockNextRandoms(3, 1, 2);

                test.RunOnce();

                Assert.AreEqual("Shuffled List: [2,3,1]", test.Logger[0]);
                Assert.AreEqual("My List: [1,2,3]", test.Logger[1]);
            }
        }
    }
}
