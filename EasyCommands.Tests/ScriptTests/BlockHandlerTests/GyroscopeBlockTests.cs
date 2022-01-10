using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class GyroscopeBlockTests {
        [TestMethod]
        public void DisableGyroscopeAuto() {
            using (var test = new ScriptTest(@"set the ""test gyro"" auto to false")) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.GyroOverride = true);
            }
        }

        [TestMethod]
        public void TurnOnTheGyroscopeOverrides() {
            using (var test = new ScriptTest(@"turn on the""test gyro"" overrides")) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.GyroOverride = true);
            }
        }

        [TestMethod]
        public void TurnOffTheGyroscopeRotation() {
            using (var test = new ScriptTest(@"turn off the""test gyro"" rotation")) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.GyroOverride = false);
            }
        }

        [TestMethod]
        public void GetGyroscopePower() {
            using (var test = new ScriptTest(@"Print ""Power: "" + the ""test gyro"" power")) {
                var mockGyro = new Mock<IMyGyro>();
                test.MockBlocksOfType("test gyro", mockGyro);
                mockGyro.Setup(b => b.GyroPower).Returns(0.5f);

                test.RunUntilDone();

                Assert.AreEqual("Power: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetGyroscopePower() {
            using (var test = new ScriptTest(@"set the ""test gyro"" power to 0.5")) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.GyroPower = 0.5f);
            }
        }

        [TestMethod]
        public void SetGyroscopeLimit() {
            using (var test = new ScriptTest(@"set the ""test gyro"" limit to 0.5")) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.GyroPower = 0.5f);
            }
        }

        [TestMethod]
        public void GetGyroscopeRotationVector() {
            using (var test = new ScriptTest(@"Print ""Rotation: "" + the ""test gyro"" rotation")) {
                var mockGyro = new Mock<IMyGyro>();
                MockGetProperty(mockGyro, "Pitch", 1f);
                MockGetProperty(mockGyro, "Yaw", 2f);
                MockGetProperty(mockGyro, "Roll", 3f);
                test.MockBlocksOfType("test gyro", mockGyro);

                test.RunUntilDone();

                Assert.AreEqual("Rotation: 1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetGyroscopeRotationVector() {
            using (var test = new ScriptTest(@"set the ""test gyro"" rotation to 1:2:3")) {
                var mockGyro = new Mock<IMyGyro>();
                var mockPitch = MockProperty<IMyGyro, float>(mockGyro, "Pitch");
                var mockYaw = MockProperty<IMyGyro, float>(mockGyro, "Yaw");
                var mockRoll = MockProperty<IMyGyro, float>(mockGyro, "Roll");
                test.MockBlocksOfType("test gyro", mockGyro);

                test.RunUntilDone();

                mockPitch.Verify(p => p.SetValue(mockGyro.Object, 1f));
                mockYaw.Verify(p => p.SetValue(mockGyro.Object, 2f));
                mockRoll.Verify(p => p.SetValue(mockGyro.Object, 3f));
            }
        }

        [TestMethod]
        public void GetGyroscopePitch() {
            using (var test = new ScriptTest(@"Print ""Pitch: "" + the ""test gyro"" upwards rotation")) {
                var mockGyro = new Mock<IMyGyro>();
                MockGetProperty(mockGyro, "Pitch", 5f);
                test.MockBlocksOfType("test gyro", mockGyro);

                test.RunUntilDone();

                Assert.AreEqual("Pitch: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetGyroscopePitch() {
            using (var test = new ScriptTest(@"set the ""test gyro"" upwards rotation to 10")) {
                var mockGyro = new Mock<IMyGyro>();
                var mockPitch = MockProperty<IMyGyro, float>(mockGyro, "Pitch");
                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockPitch.Verify(p => p.SetValue(mockGyro.Object, 10f));
            }
        }

        [TestMethod]
        public void GetGyroscopeInversePitch() {
            using (var test = new ScriptTest(@"Print ""Pitch: "" + the ""test gyro"" downwards rotation")) {
                var mockGyro = new Mock<IMyGyro>();
                MockGetProperty(mockGyro, "Pitch", 5f);
                test.MockBlocksOfType("test gyro", mockGyro);

                test.RunUntilDone();

                Assert.AreEqual("Pitch: -5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetGyroscopeInversePitch() {
            using (var test = new ScriptTest(@"set the ""test gyro"" downwards rotation to 10")) {
                var mockGyro = new Mock<IMyGyro>();
                var mockPitch = MockProperty<IMyGyro, float>(mockGyro, "Pitch");
                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockPitch.Verify(p => p.SetValue(mockGyro.Object, -10f));
            }
        }

        [TestMethod]
        public void GetGyroscopeYaw() {
            using (var test = new ScriptTest(@"Print ""Yaw: "" + the ""test gyro"" right rotation")) {
                var mockGyro = new Mock<IMyGyro>();
                MockGetProperty(mockGyro, "Yaw", 5f);
                test.MockBlocksOfType("test gyro", mockGyro);

                test.RunUntilDone();

                Assert.AreEqual("Yaw: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetGyroscopeYaw() {
            using (var test = new ScriptTest(@"set the ""test gyro"" right rotation to 10")) {
                var mockGyro = new Mock<IMyGyro>();
                var mockYaw = MockProperty<IMyGyro, float>(mockGyro, "Yaw");
                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockYaw.Verify(p => p.SetValue(mockGyro.Object, 10f));
            }
        }

        [TestMethod]
        public void GetGyroscopeInverseYaw() {
            using (var test = new ScriptTest(@"Print ""Yaw: "" + the ""test gyro"" left rotation")) {
                var mockGyro = new Mock<IMyGyro>();
                MockGetProperty(mockGyro, "Yaw", 5f);
                test.MockBlocksOfType("test gyro", mockGyro);

                test.RunUntilDone();

                Assert.AreEqual("Yaw: -5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetGyroscopeInverseYaw() {
            using (var test = new ScriptTest(@"set the ""test gyro"" left rotation to 10")) {
                var mockGyro = new Mock<IMyGyro>();
                var mockYaw = MockProperty<IMyGyro, float>(mockGyro, "Yaw");
                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockYaw.Verify(p => p.SetValue(mockGyro.Object, -10f));
            }
        }

        [TestMethod]
        public void GetGyroscopeRoll() {
            using (var test = new ScriptTest(@"Print ""Roll: "" + the ""test gyro"" clockwise rotation")) {
                var mockGyro = new Mock<IMyGyro>();
                MockGetProperty(mockGyro, "Roll", 5f);
                test.MockBlocksOfType("test gyro", mockGyro);

                test.RunUntilDone();

                Assert.AreEqual("Roll: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetGyroscopeRoll() {
            using (var test = new ScriptTest(@"set the ""test gyro"" clockwise rotation to 10")) {
                var mockGyro = new Mock<IMyGyro>();
                var mockRoll = MockProperty<IMyGyro, float>(mockGyro, "Roll");
                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockRoll.Verify(p => p.SetValue(mockGyro.Object, 10f));
            }
        }

        [TestMethod]
        public void GetGyroscopeInverseRoll() {
            using (var test = new ScriptTest(@"Print ""Roll: "" + the ""test gyro"" counter rotation")) {
                var mockGyro = new Mock<IMyGyro>();
                MockGetProperty(mockGyro, "Roll", 5f);
                test.MockBlocksOfType("test gyro", mockGyro);

                test.RunUntilDone();

                Assert.AreEqual("Roll: -5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetGyroscopeInverseRoll() {
            using (var test = new ScriptTest(@"set the ""test gyro"" counter rotation to 10")) {
                var mockGyro = new Mock<IMyGyro>();
                var mockRoll = MockProperty<IMyGyro, float>(mockGyro, "Roll");
                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockRoll.Verify(p => p.SetValue(mockGyro.Object, -10f));
            }
        }
    }
}
