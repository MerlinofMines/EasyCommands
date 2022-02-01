using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class WheelBlockTests {
        [TestMethod]
        public void GetTheWheelHeight() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wheel Height: "" + the ""test wheel"" height")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.Height).Returns(5f);
                test.RunUntilDone();

                Assert.AreEqual("Wheel Height: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheWheelHeight() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" height to 5")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                test.RunUntilDone();

                mockWheel.VerifySet(b => b.Height = 5);
            }
        }

        //Note that we inverse the Wheel Angle as defined by the program so that left = -45 and right = 45
        [TestMethod]
        public void GetTheWheelAngle() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wheel Angle: "" + the ""test wheel"" angle")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.SteerAngle).Returns(0.8f);
                test.RunUntilDone();

                Assert.AreEqual("Wheel Angle: -45.83662", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheWheelAngle() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" angle to 30")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = (float)(30 * Math.PI / 144));
                mockWheel.VerifySet(b => b.MaxSteerAngle = 1);
            }
        }

        [TestMethod]
        public void IsTheWheelLocked() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wheel Is Locked: "" + the ""test wheel"" is locked")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.Brake).Returns(true);
                test.RunUntilDone();

                Assert.AreEqual("Wheel Is Locked: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheWheelBrakeOn() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wheel Brake: "" + the ""test wheel"" brake is on")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.Brake).Returns(true);
                test.RunUntilDone();

                Assert.AreEqual("Wheel Brake: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void LockTheWheel() {
            using (ScriptTest test = new ScriptTest(@"lock the ""test wheel""")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.Brake = true);
            }
        }

        [TestMethod]
        public void TellTheWheelToBrake() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test wheel"" to brake")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.Brake = true);
            }
        }

        [TestMethod]
        public void IsTheWheelAttached() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wheel Is Attached: "" + the ""test wheel"" is attached")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.IsAttached).Returns(true);
                test.RunUntilDone();

                Assert.AreEqual("Wheel Is Attached: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AttachTheWheel() {
            using (ScriptTest test = new ScriptTest(@"attach the ""test wheel""")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.Verify(b => b.Attach());
            }
        }

        [TestMethod]
        public void DetachTheWheel() {
            using (ScriptTest test = new ScriptTest(@"detach the ""test wheel""")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.Verify(b => b.Detach());
            }
        }

        [TestMethod]
        public void GetTheWheelLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Speed Limit: "" + the ""test wheel"" limit")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                MockGetProperty(mockWheel, "Speed Limit", 50f);

                test.RunUntilDone();

                Assert.AreEqual("Speed Limit: 50", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheWheelUpperLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Speed Limit: "" + the ""test wheel"" upper limit")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                MockGetProperty(mockWheel, "Speed Limit", 50f);

                test.RunUntilDone();

                Assert.AreEqual("Speed Limit: 50", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheWheelLeftLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Steering Limit: "" + the ""test wheel"" left limit")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.MaxSteerAngle).Returns(20);

                test.RunUntilDone();

                Assert.AreEqual("Steering Limit: 20", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheWheelRightLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Steering Limit: "" + the ""test wheel"" right limit")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.MaxSteerAngle).Returns(20);

                test.RunUntilDone();

                Assert.AreEqual("Steering Limit: 20", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheWheelLimit() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" limit to 50")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                var mockProperty = MockProperty<IMyMotorSuspension, float>(mockWheel, "Speed Limit");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockWheel.Object, 50));
            }
        }

        [TestMethod]
        public void GetTheWheelSpeed() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wheel Speed: "" + the ""test wheel"" speed")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.PropulsionOverride).Returns(0.5f);
                test.RunUntilDone();

                Assert.AreEqual("Wheel Speed: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheWheelSpeed() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" speed to 0.5")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.PropulsionOverride = 0.5f);
            }
        }

        [TestMethod]
        public void GetTheWheelStrength() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wheel Strength: "" + the ""test wheel"" strength")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.Strength).Returns(20);

                test.RunUntilDone();

                Assert.AreEqual("Wheel Strength: 20", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheWheelStrength() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" strength to 20")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.Strength = 20);
            }
        }

        [TestMethod]
        public void GetTheWheelPower() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wheel Power: "" + the ""test wheel"" power")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.Power).Returns(20);

                test.RunUntilDone();

                Assert.AreEqual("Wheel Power: 20", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheWheelPower() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" power to 20")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.Power = 20);
            }
        }

        [TestMethod]
        public void GetTheWheelRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wheel Friction: "" + the ""test wheel"" ratio")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(b => b.Friction).Returns(20);

                test.RunUntilDone();

                Assert.AreEqual("Wheel Friction: 20", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheWheelRatio() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" ratio to 20")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.Friction = 20);
            }
        }
    }
}
