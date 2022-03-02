using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class LaserAntennaBlockTests {
        [TestMethod]
        public void GetTheLaserAntennaTarget() {
            using (ScriptTest test = new ScriptTest(@"Print ""Current Target: "" + the ""test laser"" target")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);
                mockLaserAntenna.Setup(b => b.TargetCoords).Returns(new Vector3D(1, 2, 3));

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Current Target: 1:2:3"));
            }
        }

        [TestMethod]
        public void SetTheLaserAntennaTarget() {
            using (ScriptTest test = new ScriptTest(@"set the ""test laser"" target to ""1:2:3""")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);

                test.RunUntilDone();

                mockLaserAntenna.Verify(b => b.SetTargetCoords("GPS:Target:1:2:3:"));
            }
        }

        [TestMethod]
        public void SetTheLaserAntennaCoords() {
            using (ScriptTest test = new ScriptTest(@"set the ""test laser"" coords to ""1:2:3""")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);

                test.RunUntilDone();

                mockLaserAntenna.Verify(b => b.SetTargetCoords("GPS:Target:1:2:3:"));
            }
        }

        [TestMethod]
        public void SetTheLaserAntennaTargetImplicit() {
            using (ScriptTest test = new ScriptTest(@"set the ""test laser"" to 1:2:3")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);

                test.RunUntilDone();

                mockLaserAntenna.Verify(b => b.SetTargetCoords("GPS:Target:1:2:3:"));
            }
        }

        [TestMethod]
        public void IsTheLaserAntennaLocked() {
            using (ScriptTest test = new ScriptTest(@"Print ""Laser Is Locked: "" + the ""test laser"" is locked")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);
                mockLaserAntenna.Setup(b => b.IsPermanent).Returns(true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Laser Is Locked: True"));
            }
        }

        [TestMethod]
        public void LockTheLaserAntenna() {
            using (ScriptTest test = new ScriptTest(@"lock the ""test laser""")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);

                test.RunUntilDone();

                mockLaserAntenna.VerifySet(b => b.IsPermanent = true);
            }
        }

        [TestMethod]
        public void SetTheLaserAntennaToPermanent() {
            using (ScriptTest test = new ScriptTest(@"set the ""test laser"" to permanent")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);

                test.RunUntilDone();

                mockLaserAntenna.VerifySet(b => b.IsPermanent = true);
            }
        }

        [TestMethod]
        public void GetTheLaserAntennaRadius() {
            using (ScriptTest test = new ScriptTest(@"Print ""Radius: "" + ""test laser"" radius")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);
                mockLaserAntenna.Setup(b => b.Range).Returns(2000);

                test.RunUntilDone();

                Assert.AreEqual("Radius: 2000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheLaserAntennaRadius() {
            using (ScriptTest test = new ScriptTest(@"set the ""test laser"" radius to 2000")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);

                test.RunUntilDone();

                mockLaserAntenna.VerifySet(b => b.Range = 2000f);
            }
        }

        [TestMethod]
        public void GetTheLaserAntennaRange() {
            using (ScriptTest test = new ScriptTest(@"Print ""Range: "" + ""test laser"" range")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);
                mockLaserAntenna.Setup(b => b.Range).Returns(2000);

                test.RunUntilDone();

                Assert.AreEqual("Range: 2000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheLaserAntennaRange() {
            using (ScriptTest test = new ScriptTest(@"set the ""test laser"" range to 2000")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);

                test.RunUntilDone();

                mockLaserAntenna.VerifySet(b => b.Range = 2000f);
            }
        }

        [TestMethod]
        public void IsTheLaserAntennaConnected() {
            using (ScriptTest test = new ScriptTest(@"Print ""Laser Antenna Connected: "" + the ""test laser"" is connected")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);
                mockLaserAntenna.Setup(b => b.Status).Returns(MyLaserAntennaStatus.Connected);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Laser Antenna Connected: True"));
            }
        }

        [TestMethod]
        public void ConnectTheLaserAntenna() {
            using (ScriptTest test = new ScriptTest(@"connect the ""test laser""")) {
                Mock<IMyLaserAntenna> mockLaserAntenna = new Mock<IMyLaserAntenna>();
                test.MockBlocksOfType("test laser", mockLaserAntenna);
                test.RunUntilDone();

                mockLaserAntenna.Verify(b => b.Connect());
            }
        }
    }
}
