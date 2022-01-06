using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class PistonBlockTests : ForceLocale {
        [TestMethod]
        public void getUpperLimit() {
            String script = @"
print the ""piston"" upper limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);

                mockPiston.Setup(b => b.MaxLimit).Returns(5);
                test.RunUntilDone();
                mockPiston.Verify(b => b.MaxLimit);

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void setUpperLimit() {
            String script = @"
set the ""piston"" upper limit to 5
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MaxLimit = 5);
            }
        }

        [TestMethod]
        public void increaseUpperLimit() {
            String script = @"
increase the ""piston"" upper limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);
                mockPiston.Setup(b => b.MaxLimit).Returns(5);
                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MaxLimit = 6);
            }
        }

        [TestMethod]
        public void decreaseUpperLimit() {
            String script = @"
decrease the ""piston"" upper limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);
                mockPiston.Setup(b => b.MaxLimit).Returns(5);
                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MaxLimit = 4);
            }
        }

        [TestMethod]
        public void increaseUpperLimitByAmount() {
            String script = @"
increase the ""piston"" upper limit by 2
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);
                mockPiston.Setup(b => b.MaxLimit).Returns(5);
                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MaxLimit = 7);
            }
        }

        [TestMethod]
        public void decreaseUpperLimitByAmount() {
            String script = @"
decrease the ""piston"" upper limit by 2
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);
                mockPiston.Setup(b => b.MaxLimit).Returns(5);
                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MaxLimit = 3);
            }
        }

        [TestMethod]
        public void getLowerLimit() {
            String script = @"
print the ""piston"" lower limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);

                mockPiston.Setup(b => b.MinLimit).Returns(5);
                test.RunUntilDone();
                mockPiston.Verify(b => b.MinLimit);

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void setLowerLimit() {
            String script = @"
set the ""piston"" lower limit to 5
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MinLimit = 5);
            }
        }

        [TestMethod]
        public void increaseLowerLimit() {
            String script = @"
increase the ""piston"" lower limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);
                mockPiston.Setup(b => b.MinLimit).Returns(5);
                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MinLimit = 6);
            }
        }

        [TestMethod]
        public void decreaseLowerLimit() {
            String script = @"
decrease the ""piston"" lower limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);
                mockPiston.Setup(b => b.MinLimit).Returns(5);
                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MinLimit = 4);
            }
        }

        [TestMethod]
        public void increaseLowerLimitByAmount() {
            String script = @"
increase the ""piston"" lower limit by 2
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);
                mockPiston.Setup(b => b.MinLimit).Returns(5);
                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MinLimit = 7);
            }
        }

        [TestMethod]
        public void decreaseLowerLimitByAmount() {
            String script = @"
decrease the ""piston"" lower limit by 2
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);
                mockPiston.Setup(b => b.MinLimit).Returns(5);
                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MinLimit = 3);
            }
        }

        [TestMethod]
        public void getThePistonHeight() {
            using (ScriptTest test = new ScriptTest(@"Print ""Height: "" + the ""test piston"" height")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                Assert.AreEqual("Height: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void setThePistonHeightRequiresRaising() {
            using (ScriptTest test = new ScriptTest(@"set the ""test piston"" height to 7")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MaxLimit = 7);
                mockPiston.Verify(p => p.Extend());
            }
        }

        [TestMethod]
        public void setThePistonHeightRequiresRetracting() {
            using (ScriptTest test = new ScriptTest(@"set the ""test piston"" height to 3")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MinLimit = 3);
                mockPiston.Verify(p => p.Retract());
            }
        }

        [TestMethod]
        public void increaseThePiston() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test piston""")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MaxLimit = 6);
                mockPiston.Verify(p => p.Extend());
            }
        }

        [TestMethod]
        public void decreaseThePiston() {
            using (ScriptTest test = new ScriptTest(@"decrease the ""test piston""")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MinLimit = 4);
                mockPiston.Verify(p => p.Retract());
            }
        }

        [TestMethod]
        public void increaseThePistonByAmount() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test piston"" by 2")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MaxLimit = 7);
                mockPiston.Verify(p => p.Extend());
            }
        }

        [TestMethod]
        public void decreaseThePistonByAmount() {
            using (ScriptTest test = new ScriptTest(@"decrease the ""test piston"" by 2")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MinLimit = 3);
                mockPiston.Verify(p => p.Retract());
            }
        }

        [TestMethod]
        public void increaseThePistonHeight() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test piston"" height")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MaxLimit = 6);
                mockPiston.Verify(p => p.Extend());
            }
        }

        [TestMethod]
        public void decreaseThePistonHeight() {
            using (ScriptTest test = new ScriptTest(@"decrease the ""test piston"" height")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MinLimit = 4);
                mockPiston.Verify(p => p.Retract());
            }
        }

        [TestMethod]
        public void increaseThePistonHeightByAmount() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test piston"" height by 2")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MaxLimit = 7);
                mockPiston.Verify(p => p.Extend());
            }
        }

        [TestMethod]
        public void decreaseThePistonHeightByAmount() {
            using (ScriptTest test = new ScriptTest(@"decrease the ""test piston"" height by 2")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.CurrentPosition).Returns(5);

                test.RunUntilDone();

                mockPiston.VerifySet(p => p.MinLimit = 3);
                mockPiston.Verify(p => p.Retract());
            }
        }

        [TestMethod]
        public void getThePistonVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Velocity: "" + the ""test piston"" velocity")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.Velocity).Returns(1);

                test.RunUntilDone();

                Assert.AreEqual("Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void setThePistonVelocity() {
            using (ScriptTest test = new ScriptTest(@"set the ""test piston"" velocity to 2")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.Velocity = 2);
            }
        }

        [TestMethod]
        public void increaseThePistonVelocity() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test piston"" velocity")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.Velocity).Returns(1f);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.Velocity = 2);
            }
        }

        [TestMethod]
        public void decreaseThePistonVelocity() {
            using (ScriptTest test = new ScriptTest(@"decrease the ""test piston"" velocity")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.Velocity).Returns(3f);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.Velocity = 2);
            }
        }

        [TestMethod]
        public void increaseThePistonVelocityByAmount() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test piston"" velocity by 2")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.Velocity).Returns(1f);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.Velocity = 3);
            }
        }
 
        [TestMethod]
        public void decreaseThePistonVelocityByAmount() {
            using (ScriptTest test = new ScriptTest(@"decrease the ""test piston"" velocity by 2")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.Velocity).Returns(3f);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.Velocity = 1);
            }
        }

        [TestMethod]
        public void ReverseThePiston() {
            using (ScriptTest test = new ScriptTest(@"reverse the ""test piston""")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);

                test.RunUntilDone();

                mockPiston.Verify(b => b.Reverse());
            }
        }

        [TestMethod]
        public void ReverseThePistonHeight() {
            using (ScriptTest test = new ScriptTest(@"reverse the ""test piston""")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);

                test.RunUntilDone();

                mockPiston.Verify(b => b.Reverse());
            }
        }

        [TestMethod]
        public void ReverseThePistonVelocity() {
            using (ScriptTest test = new ScriptTest(@"reverse the ""test piston"" velocity")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.Velocity).Returns(1);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.Velocity = -1);
            }
        }
    }
}
