using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class BeaconBlockTests : ForceLocale {
        [TestMethod]
        public void SetTheBeaconText() {
            using (ScriptTest test = new ScriptTest(@"set the ""test beacon"" text to ""Hello World!""")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.HudText = "Hello World!");
            }
        }

        [TestMethod]
        public void TurnOnTheBeacon() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test beacon""")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TellTheBeaconToBroadcast() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test beacon"" to broadcast")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void IsTheBeaconBroadcasting() {
            using (ScriptTest test = new ScriptTest(@"print ""Beacon Broadcasting: "" + ""test beacon"" is broadcasting")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(b => b.Enabled).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Beacon Broadcasting: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheBeaconRange() {
            using (ScriptTest test = new ScriptTest(@"Print ""Beacon Range: "" + the ""test beacon"" range")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(b => b.Radius).Returns(10000f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Beacon Range: 10000"));
            }
        }

        [TestMethod]
        public void SetTheBeaconRange() {
            using (ScriptTest test = new ScriptTest(@"set the ""test beacon"" range to 5000")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 5000f);
            }
        }
    }
}
