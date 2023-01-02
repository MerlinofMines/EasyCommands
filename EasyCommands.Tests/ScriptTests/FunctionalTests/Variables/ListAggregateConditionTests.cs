using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ListAggregateConditionTests {

        [TestMethod]
        public void AllOfEmptyListReturnsTrue() {
            using (var test = new ScriptTest(@"
set myList to []
print all of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AllMatchReturnsTrue() {
            using (var test = new ScriptTest(@"
set myList to [2]
print all of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AllWithoutMatchReturnsFalse() {
            using (var test = new ScriptTest(@"
set myList to [2,-1]
print all of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AnyOfEmptyListReturnsFalse() {
            using (var test = new ScriptTest(@"
set myList to []
print any of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AnyMatchReturnsTrue() {
            using (var test = new ScriptTest(@"
set myList to [2, -1]
print any of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AnyWithoutMatchReturnsFalse() {
            using (var test = new ScriptTest(@"
set myList to [-2,-1]
print any of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NoneOfEmptyListReturnsTrue() {
            using (var test = new ScriptTest(@"
set myList to []
print none of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NoneMatchReturnsTrue() {
            using (var test = new ScriptTest(@"
set myList to [-2, -1]
print none of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NoneWithMatchReturnsFalse() {
            using (var test = new ScriptTest(@"
set myList to [2,-1]
print none of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotAnyIsTheSameAsNone() {
            using (var test = new ScriptTest(@"
set myList to [2,-1]
print none of myList[] > 0
print not any of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
                Assert.AreEqual("False", test.Logger[1]);
            }
        }

        [TestMethod]
        public void NotAnyIsTheSameAsNoneForEmptyList() {
            using (var test = new ScriptTest(@"
set myList to []
print none of myList[] > 0
print not any of myList[] > 0
")) {

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
                Assert.AreEqual("True", test.Logger[1]);
            }
        }
    }
}
