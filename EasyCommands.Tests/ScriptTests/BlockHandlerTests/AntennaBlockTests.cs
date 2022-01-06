using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class AntennaBlockTests : ForceLocale {
        [TestMethod]
        public void SetTheAntennaText() {
            using (ScriptTest test = new ScriptTest(@"set the ""test antenna"" text to ""Hello World!""")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                test.MockBlocksOfType("test antenna", mockAntenna);

                test.RunUntilDone();

                mockAntenna.VerifySet(b => b.HudText = "Hello World!");
            }
        }

        [TestMethod]
        public void TurnOnTheAntenna() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test antenna""")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                test.MockBlocksOfType("test antenna", mockAntenna);

                test.RunUntilDone();

                mockAntenna.VerifySet(b => b.EnableBroadcasting = true);
            }
        }

        [TestMethod]
        public void TellTheAntennaToBroadcast() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test antenna"" to broadcast")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                test.MockBlocksOfType("test antenna", mockAntenna);

                test.RunUntilDone();

                mockAntenna.VerifySet(b => b.EnableBroadcasting = true);
            }
        }

        [TestMethod]
        public void IsTheAntennaBroadcasting() {
            using (ScriptTest test = new ScriptTest(@"print ""Antenna Broadcasting: "" + ""test antenna"" is broadcasting")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                test.MockBlocksOfType("test antenna", mockAntenna);
                mockAntenna.Setup(b => b.EnableBroadcasting).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Antenna Broadcasting: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TellTheAntennaToShow() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test antenna"" to show")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                test.MockBlocksOfType("test antenna", mockAntenna);

                test.RunUntilDone();

                mockAntenna.VerifySet(b => b.ShowShipName = true);
            }
        }

        [TestMethod]
        public void GetTheAntennaRange() {
            using (ScriptTest test = new ScriptTest(@"Print ""Antenna Range: "" + the ""test antenna"" range")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                test.MockBlocksOfType("test antenna", mockAntenna);
                mockAntenna.Setup(b => b.Radius).Returns(10000f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Antenna Range: 10000"));
            }
        }

        [TestMethod]
        public void SetTheAntennaRange() {
            using (ScriptTest test = new ScriptTest(@"set the ""test antenna"" range to 5000")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                test.MockBlocksOfType("test antenna", mockAntenna);

                test.RunUntilDone();

                mockAntenna.VerifySet(b => b.Radius = 5000f);
            }
        }
    }
}
