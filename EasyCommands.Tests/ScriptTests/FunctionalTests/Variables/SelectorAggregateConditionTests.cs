using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SelectorAggregateConditionTests {

        [TestMethod]
        public void AllOfEmptySelectorReturnsTrue() {
            using (var test = new ScriptTest(@"
print all of my pistons height > 5
")) {

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AllMatchReturnsTrue() {
            using (var test = new ScriptTest(@"
print all of my pistons height > 5
")) {
                var mockPiston = new Mock<IMyPistonBase>();
                mockPiston.Setup(b => b.CurrentPosition).Returns(10);
                test.MockBlocksOfType("My Piston", mockPiston);

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AllWithoutMatchReturnsFalse() {
            using (var test = new ScriptTest(@"
print all of my pistons height > 5
")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                mockPiston.Setup(b => b.CurrentPosition).Returns(10);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3);
                test.MockBlocksOfType("My Piston", mockPiston);
                test.MockBlocksOfType("My Piston 2", mockPiston2);

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AnyOfEmptySelectorReturnsFalse() {
            using (var test = new ScriptTest(@"
print any of my pistons height > 5
")) {

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AnyMatchReturnsTrue() {
            using (var test = new ScriptTest(@"
print any of my pistons height > 5
")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                mockPiston.Setup(b => b.CurrentPosition).Returns(10);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3);
                test.MockBlocksOfType("My Piston", mockPiston);
                test.MockBlocksOfType("My Piston 2", mockPiston2);

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AnyWithoutMatchReturnsFalse() {
            using (var test = new ScriptTest(@"
print all of my pistons height > 5
")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                mockPiston.Setup(b => b.CurrentPosition).Returns(4);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3);
                test.MockBlocksOfType("My Piston", mockPiston);
                test.MockBlocksOfType("My Piston 2", mockPiston2);

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NoneOfEmptySelectorReturnsTrue() {
            using (var test = new ScriptTest(@"
print none of my pistons height > 5
")) {

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NoneWithMatchReturnsFalse() {
            using (var test = new ScriptTest(@"
print none of my pistons height > 5
")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                mockPiston.Setup(b => b.CurrentPosition).Returns(10);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3);
                test.MockBlocksOfType("My Piston", mockPiston);
                test.MockBlocksOfType("My Piston 2", mockPiston2);

                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NoneWithoutMatchReturnsTrue() {
            using (var test = new ScriptTest(@"
print none of my pistons height > 5
")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                mockPiston.Setup(b => b.CurrentPosition).Returns(4);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3);
                test.MockBlocksOfType("My Piston", mockPiston);
                test.MockBlocksOfType("My Piston 2", mockPiston2);

                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }
    }
}
