using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class JumpDriveBlockTests : ForceLocale {
        [TestMethod]
        public void GetJumpDriveLevel() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Level: "" + ""test jumpdrive"" level")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.CurrentStoredPower).Returns(20f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Jump Drive Level: 20"));
            }
        }

        [TestMethod]
        public void GetJumpDriveLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Max Power: "" + ""test jumpdrive"" limit")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.MaxStoredPower).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Jump Drive Max Power: 100"));
            }
        }

        [TestMethod]
        public void GetJumpDriveRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Ratio: "" + ( 100 * ""test jumpdrive"" ratio ) + ""%""")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.CurrentStoredPower).Returns(20f);
                mockJumpDrive.Setup(b => b.MaxStoredPower).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Jump Drive Ratio: 20%"));
            }
        }

        [TestMethod]
        public void IsJumpDriveReady() {
            using (ScriptTest test = new ScriptTest(@"Print ""Jump Drive Ready: "" + ""test jumpdrive"" is ready")) {
                Mock<IMyJumpDrive> mockJumpDrive = new Mock<IMyJumpDrive>();
                test.MockBlocksOfType("test jumpdrive", mockJumpDrive);
                mockJumpDrive.Setup(b => b.Status).Returns(MyJumpDriveStatus.Ready);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Jump Drive Ready: True"));
            }
        }
    }
}
