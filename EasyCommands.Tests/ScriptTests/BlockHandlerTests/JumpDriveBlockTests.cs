using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class JumpDriveBlockTests {

        [TestMethod]
        public void GetJumpDriveLength() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Length: "" + ""test jumpdrive"" length")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.JumpDistanceMeters).Returns(2000f);

                test.RunUntilDone();

                Assert.AreEqual("Jump Drive Length: 2000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetJumpDriveDistance() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Limit: "" + ""test jumpdrive"" distance")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.JumpDistanceMeters).Returns(2000f);

                test.RunUntilDone();

                Assert.AreEqual("Jump Drive Limit: 2000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetJumpDriveDistance() {
            using (ScriptTest test = new ScriptTest(@"set the ""test jumpdrive"" distance to 20000")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                var mockJumpDistance = MockEntityUtility.MockProperty<IMyJumpDrive, float>(mockJumpDrive, "JumpDistance");

                mockJumpDrive.Setup(b => b.MaxJumpDistanceMeters).Returns(10000f);
                mockJumpDrive.Setup(b => b.MinJumpDistanceMeters).Returns(30000f);

                test.RunUntilDone();

                mockJumpDistance.Verify(p => p.SetValue(mockJumpDrive.Object, 50f));
            }
        }

        [TestMethod]
        public void GetJumpDriveUpperLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Upper Limit: "" + ""test jumpdrive"" upper limit")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.MaxJumpDistanceMeters).Returns(6000f);

                test.RunUntilDone();

                Assert.AreEqual("Jump Drive Upper Limit: 6000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetJumpDriveLowerLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Lower Limit: "" + ""test jumpdrive"" lower limit")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.MinJumpDistanceMeters).Returns(1000f);

                test.RunUntilDone();

                Assert.AreEqual("Jump Drive Lower Limit: 1000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetJumpDriveRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Ratio: "" + ""test jumpdrive"" ratio")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.CurrentStoredPower).Returns(20f);
                mockJumpDrive.Setup(b => b.MaxStoredPower).Returns(100f);

                test.RunUntilDone();

                Assert.AreEqual("Jump Drive Ratio: 0.2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsJumpDriveReady() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Ready: "" + ""test jumpdrive"" is ready")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.Status).Returns(MyJumpDriveStatus.Ready);

                test.RunUntilDone();

                Assert.AreEqual("Jump Drive Ready: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsJumpDriveRecharging() {
            using (ScriptTest test = new ScriptTest(@"Print ""Recharging: "" + ""test jumpdrive"" is recharging")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.Recharge).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Recharging: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsJumpDriveRechargingWhenNotRecharging() {
            using (ScriptTest test = new ScriptTest(@"Print ""Recharging: "" + ""test jumpdrive"" is recharging")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.Recharge).Returns(false);

                test.RunUntilDone();

                Assert.AreEqual("Recharging: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RechargeTheJumpDrive() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test jumpdrive"" to recharge")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);

                test.RunUntilDone();

                mockJumpDrive.VerifySet(b => b.Recharge = true);
            }
        }
    }
}
