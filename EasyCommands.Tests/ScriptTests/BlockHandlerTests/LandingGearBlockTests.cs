using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class LandingGearBlockTests {
        [TestMethod]
        public void TurnOnTheLandingGear() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test landing gear""")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);

                test.RunUntilDone();

                mockLandingGear.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheLandingGear() {
            using (ScriptTest test = new ScriptTest(@"turn off the ""test landing gear""")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);

                test.RunUntilDone();

                mockLandingGear.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void IsTheLandingGearConnected() {
            using (ScriptTest test = new ScriptTest(@"Print ""Connected: "" + ""test landing gear"" is connected")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);
                mockLandingGear.Setup(b => b.IsLocked).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Connected: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheLandingGearLocked() {
            using (ScriptTest test = new ScriptTest(@"Print ""Locked: "" + ""test landing gear"" is locked")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);
                mockLandingGear.Setup(b => b.IsLocked).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Locked: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ConnectTheLandingGear() {
            using (ScriptTest test = new ScriptTest(@"connect the ""test landing gear""")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);

                test.RunUntilDone();

                mockLandingGear.Verify(b => b.Lock());
            }
        }

        [TestMethod]
        public void LockTheLandingGear() {
            using (ScriptTest test = new ScriptTest(@"lock the ""test landing gear""")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);

                test.RunUntilDone();

                mockLandingGear.Verify(b => b.Lock());
            }
        }

        [TestMethod]
        public void DisconnectTheLandingGear() {
            using (ScriptTest test = new ScriptTest(@"disconnect the ""test landing gear""")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);

                test.RunUntilDone();

                mockLandingGear.Verify(b => b.Unlock());
            }
        }

        [TestMethod]
        public void UnlockTheLandingGear() {
            using (ScriptTest test = new ScriptTest(@"unlock the ""test landing gear""")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);

                test.RunUntilDone();

                mockLandingGear.Verify(b => b.Unlock());
            }
        }

        [TestMethod]
        public void IsLandingGearOnAuto() {
            using (ScriptTest test = new ScriptTest(@"Print ""Auto Lock: "" + the ""test landing gear"" is on auto")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);
                mockLandingGear.Setup(b => b.AutoLock).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Auto Lock: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetLandingGearToAuto() {
            using (ScriptTest test = new ScriptTest(@"set the ""test landing gear"" to auto")) {
                Mock<IMyLandingGear> mockLandingGear = new Mock<IMyLandingGear>();
                test.MockBlocksOfType("test landing gear", mockLandingGear);

                test.RunUntilDone();

                mockLandingGear.VerifySet(b => b.AutoLock = true);
            }
        }
    }
}
