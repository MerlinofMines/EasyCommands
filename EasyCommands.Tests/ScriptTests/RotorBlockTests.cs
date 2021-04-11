using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class RotorBlockTests {
        [TestMethod]
        public void getUpperLimit() {
            String script = @"
print the avg ""rotor"" upper limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(b => b.UpperLimitDeg).Returns(30);
                test.RunUntilDone();
                mockRotor.Verify(b => b.UpperLimitDeg);

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("30", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getLowerLimit() {
            String script = @"
print the avg ""rotor"" lower limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(b => b.LowerLimitDeg).Returns(30);
                test.RunUntilDone();
                mockRotor.Verify(b => b.LowerLimitDeg);

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("30", test.Logger[0]);
            }
        }

        [TestMethod]
        public void setUpperLimit() {
            String script = @"
set the ""rotor"" upper limit to 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                test.RunUntilDone();

                mockRotor.VerifySet(b => b.UpperLimitDeg = 30);
            }
        }

        [TestMethod]
        public void setLowerLimit() {
            String script = @"
set the ""rotor"" lower limit to 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                test.RunUntilDone();

                mockRotor.VerifySet(b => b.LowerLimitDeg = 30);
            }
        }

        [TestMethod]
        public void setRotorAngleGreaterThanMovesClockwise() {
            String script = @"
rotate the ""rotor"" to 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.UpperLimitDeg = 30);
                mockRotor.VerifySet(b => b.TargetVelocityRPM = 1);
            }
        }

        [TestMethod]
        public void setRotorAngleLessThanMovesCounterClockwise() {
            String script = @"
rotate the ""rotor"" to -30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.LowerLimitDeg = -30);
                mockRotor.VerifySet(b => b.TargetVelocityRPM = -1);
            }
        }

        [TestMethod]
        public void increaseRotorAngle() {
            String script = @"
rotate the ""rotor"" clockwise by 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.UpperLimitDeg = 30);
                mockRotor.VerifySet(b => b.TargetVelocityRPM = 1);
            }
        }

        [TestMethod]
        public void increaseRotorAngleWrapsAround() {
            String script = @"
rotate the ""rotor"" clockwise by 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(345 * (float)Math.PI / 180);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.UpperLimitDeg = 15.0000305f); //Oh floats and rounding...
                mockRotor.VerifySet(b => b.TargetVelocityRPM = 1);
            }
        }

        [TestMethod]
        public void decreaseRotorAngle() {
            String script = @"
rotate the ""rotor"" counterclockwise by 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.LowerLimitDeg = -30);
                mockRotor.VerifySet(b => b.TargetVelocityRPM = -1);
            }
        }

        [TestMethod]
        public void decreaseRotorAngleWrapsAround() {
            String script = @"
rotate the ""rotor"" counterclockwise by 30
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(-345 * (float)Math.PI / 180);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.LowerLimitDeg = -15.0000305f); //Oh floats and rounding...
                mockRotor.VerifySet(b => b.TargetVelocityRPM = -1);
            }
        }

        [TestMethod]
        public void setRotorAngleClockwise() {
            String script = @"
rotate the ""rotor"" 30 clockwise
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.UpperLimitDeg = 30);
                mockRotor.VerifySet(b => b.TargetVelocityRPM = 1);
            }
        }

        [TestMethod]
        public void setRotorAngleCounterClockwise() {
            String script = @"
rotate the ""rotor"" -30 counterclockwise
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(0);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.LowerLimitDeg = -30);
                mockRotor.VerifySet(b => b.TargetVelocityRPM = -1);
            }
        }

        [TestMethod]
        public void setRotorAngleClockwiseWrapsAround() {
            String script = @"
rotate the ""rotor"" -60 clockwise
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(-30 * (float)Math.PI / 180);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.UpperLimitDeg = 300);
                mockRotor.VerifySet(b => b.TargetVelocityRPM = 1);
            }
        }

        [TestMethod]
        public void setRotorAngleCounterClockwiseWrapsAround() {
            String script = @"
rotate the ""rotor"" 60 counterclockwise
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRotor = new Mock<IMyMotorStator>();
                test.MockBlocksOfType("rotor", mockRotor);

                mockRotor.Setup(r => r.TargetVelocityRPM).Returns(1);
                mockRotor.Setup(r => r.Angle).Returns(30 * (float)Math.PI / 180);
                test.RunUntilDone();

                mockRotor.VerifySet(b => b.LowerLimitDeg= -300);
                mockRotor.VerifySet(b => b.TargetVelocityRPM = -1);
            }
        }
    }
}
