using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class MagnetBlockTests : ForceLocale {
        [TestMethod]
        public void TurnOnTheMagnet() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test magnet""")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);

                test.RunUntilDone();

                mockMagnet.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheMagnet() {
            using (ScriptTest test = new ScriptTest(@"turn off the ""test magnet""")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);

                test.RunUntilDone();

                mockMagnet.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void IsTheMagnetConnected() {
            using (ScriptTest test = new ScriptTest(@"Print ""Connected: "" + ""test magnet"" is connected")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);
                mockMagnet.Setup(b => b.IsLocked).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Connected: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheMagnetLocked() {
            using (ScriptTest test = new ScriptTest(@"Print ""Locked: "" + ""test magnet"" is locked")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);
                mockMagnet.Setup(b => b.IsLocked).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Locked: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ConnectTheMagnet() {
            using (ScriptTest test = new ScriptTest(@"connect the ""test magnet""")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);

                test.RunUntilDone();

                mockMagnet.Verify(b => b.Lock());
            }
        }

        [TestMethod]
        public void LockTheMagnet() {
            using (ScriptTest test = new ScriptTest(@"lock the ""test magnet""")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);

                test.RunUntilDone();

                mockMagnet.Verify(b => b.Lock());
            }
        }

        [TestMethod]
        public void DisconnectTheMagnet() {
            using (ScriptTest test = new ScriptTest(@"disconnect the ""test magnet""")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);

                test.RunUntilDone();

                mockMagnet.Verify(b => b.Unlock());
            }
        }

        [TestMethod]
        public void UnlockTheMagnet() {
            using (ScriptTest test = new ScriptTest(@"unlock the ""test magnet""")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);

                test.RunUntilDone();

                mockMagnet.Verify(b => b.Unlock());
            }
        }

        [TestMethod]
        public void IsMagnetOnAuto() {
            using (ScriptTest test = new ScriptTest(@"Print ""Auto Lock: "" + the ""test magnet"" is on auto")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);
                mockMagnet.Setup(b => b.AutoLock).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Auto Lock: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetMagnetToAuto() {
            using (ScriptTest test = new ScriptTest(@"set the ""test magnet"" to auto")) {
                Mock<IMyLandingGear> mockMagnet = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test magnet", mockMagnet);

                test.RunUntilDone();

                mockMagnet.VerifySet(b => b.AutoLock = true);
            }
        }
    }
}
