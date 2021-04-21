using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class GyroscopeBlockTests {
        [TestMethod]
        public void DisableGyroscopeAuto() {
            String script = @"
set the ""test gyro"" auto to false
";
            using (var test = new ScriptTest(script)) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.GyroOverride = true);
            }
        }

        [TestMethod]
        public void EnableGyroscopeOverrides() {
            String script = @"
override ""test gyro""
";
            using (var test = new ScriptTest(script)) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.GyroOverride = true);
            }
        }

        [TestMethod]
        public void SetGyroscopePowerLevel() {
            String script = @"
set the ""test gyro"" limit to 0.5
";
            using (var test = new ScriptTest(script)) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.GyroPower = 0.5f);
            }
        }

        [TestMethod]
        public void SetGyroscopePitch() {
            String script = @"
set the ""test gyro"" upwards roll to 0.5
";
            using (var test = new ScriptTest(script)) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.Pitch = 0.5f);
            }
        }

        [TestMethod]
        public void SetGyroscopeInversePitch() {
            String script = @"
set the ""test gyro"" downwards roll to 0.5
";
            using (var test = new ScriptTest(script)) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.Pitch = -0.5f);
            }
        }

        [TestMethod]
        public void SetGyroscopeYaw() {
            String script = @"
set the ""test gyro"" right roll to 0.5
";
            using (var test = new ScriptTest(script)) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.Yaw = 0.5f);
            }
        }

        [TestMethod]
        public void SetGyroscopeInverseYaw() {
            String script = @"
set the ""test gyro"" left roll to 0.5
";
            using (var test = new ScriptTest(script)) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.Yaw = -0.5f);
            }
        }

        [TestMethod]
        public void SetGyroscopeRoll() {
            String script = @"
set the ""test gyro"" clockwise roll to 0.5
";
            using (var test = new ScriptTest(script)) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.Roll = 0.5f);
            }
        }

        [TestMethod]
        public void SetGyroscopeInverseRoll() {
            String script = @"
set the ""test gyro"" counterclock roll to 0.5
";
            using (var test = new ScriptTest(script)) {
                var mockGyro = new Mock<IMyGyro>();

                test.MockBlocksOfType("test gyro", mockGyro);
                test.RunUntilDone();

                mockGyro.VerifySet(b => b.Roll = -0.5f);
            }
        }
    }
}
