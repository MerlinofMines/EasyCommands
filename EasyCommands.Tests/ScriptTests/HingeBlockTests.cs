using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class HingeBlockTests {
        [TestMethod]
        public void getUpperLimit() {
            String script = @"
print the avg ""hinge"" upper limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                mockHinge.Setup(b => b.UpperLimitDeg).Returns(30);
                test.RunUntilDone();
                mockHinge.Verify(b => b.UpperLimitDeg);

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("30", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getLowerLimit() {
            String script = @"
print the avg ""hinge"" lower limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                mockHinge.Setup(b => b.LowerLimitDeg).Returns(30);
                test.RunUntilDone();
                mockHinge.Verify(b => b.LowerLimitDeg);

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("30", test.Logger[0]);
            }
        }

        [TestMethod]
        public void setUpperLimit() {
            String script = @"
set the ""hinge"" upper limit to 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                test.RunUntilDone();

                mockHinge.VerifySet(b => b.UpperLimitDeg = 30);
            }
        }

        [TestMethod]
        public void setLowerLimit() {
            String script = @"
set the ""hinge"" lower limit to 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                test.RunUntilDone();

                mockHinge.VerifySet(b => b.LowerLimitDeg = 30);
            }
        }

        [TestMethod]
        public void setHingeAngleGreaterThanMovesClockwise() {
            String script = @"
rotate the ""hinge"" to 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                mockHinge.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockHinge.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockHinge.VerifySet(b => b.UpperLimitDeg = 30);
                mockHinge.VerifySet(b => b.TargetVelocityRPM = 1);
            }
        }

        [TestMethod]
        public void setHingeAngleLessThanMovesCounterClockwise() {
            String script = @"
rotate the ""hinge"" to -30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                mockHinge.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockHinge.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockHinge.VerifySet(b => b.LowerLimitDeg = -30);
                mockHinge.VerifySet(b => b.TargetVelocityRPM = -1);
            }
        }

        [TestMethod]
        public void increaseHingeAngle() {
            String script = @"
rotate the ""hinge"" clockwise by 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                mockHinge.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockHinge.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockHinge.VerifySet(b => b.UpperLimitDeg = 30);
                mockHinge.VerifySet(b => b.TargetVelocityRPM = 1);
            }
        }

        [TestMethod]
        public void decreaseHingeAngle() {
            String script = @"
rotate the ""hinge"" counterclockwise by 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                mockHinge.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockHinge.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockHinge.VerifySet(b => b.LowerLimitDeg = -30);
                mockHinge.VerifySet(b => b.TargetVelocityRPM = -1);
            }
        }

        [TestMethod]
        public void attachTheRotor() {
            using (ScriptTest test = new ScriptTest(@"attach the ""hinge""")) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                test.RunUntilDone();

                mockHinge.Verify(b => b.Attach());
            }
        }

        [TestMethod]
        public void detachTheRotor() {
            using (ScriptTest test = new ScriptTest(@"detach the ""hinge""")) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                test.RunUntilDone();

                mockHinge.Verify(b => b.Detach());
            }
        }

        [TestMethod]
        public void lockTheRotor() {
            using (ScriptTest test = new ScriptTest(@"lock the ""hinge""")) {
                var mockHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockHinge);
                MockBlockDefinition(mockHinge, "LargeHinge");

                test.RunUntilDone();

                mockHinge.VerifySet(b => b.RotorLock = true);
            }
        }

        [TestMethod]
        public void unlockTheRotor() {
            using (ScriptTest test = new ScriptTest(@"unlock the ""hinge""")) {
                var morkHinge = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", morkHinge);
                MockBlockDefinition(morkHinge, "LargeHinge");

                test.RunUntilDone();

                morkHinge.VerifySet(b => b.RotorLock = false);
            }
        }

        [TestMethod]
        public void setTheHingeTorque() {
            using (ScriptTest test = new ScriptTest(@"set the ""hinge"" torque to 2000")) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("hinge", mockRotor);
                MockBlockDefinition(mockRotor, "LargeHinge");

                test.RunUntilDone();

                mockRotor.VerifySet(b => b.Torque = 2000f);
            }
        }
    }
}
