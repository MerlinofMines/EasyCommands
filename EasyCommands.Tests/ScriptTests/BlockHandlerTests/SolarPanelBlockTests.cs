using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SolarPanelBlockTests {
        [TestMethod]
        public void GetSolarPanelOutput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Solar Panel Output: "" + ""test solar panel"" output")) {
                Mock<IMyPowerProducer> mockSolarPanel = new Mock<IMyPowerProducer>();
                test.MockBlocksOfType("test solar panel", mockSolarPanel);
                mockSolarPanel.Setup(b => b.CurrentOutput).Returns(20f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Solar Panel Output: 20"));
            }
        }

        [TestMethod]
        public void GetSolarPanelLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Solar Panel Max Output: "" + ""test solar panel"" limit")) {
                Mock<IMyPowerProducer> mockSolarPanel = new Mock<IMyPowerProducer>();
                test.MockBlocksOfType("test solar panel", mockSolarPanel);
                mockSolarPanel.Setup(b => b.MaxOutput).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Solar Panel Max Output: 100"));
            }
        }

        [TestMethod]
        public void GetSolarPanelRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Solar Panel Output: "" + ( 100 * ""test solar panel"" ratio ) + ""%""")) {
                Mock<IMyPowerProducer> mockSolarPanel = new Mock<IMyPowerProducer>();
                test.MockBlocksOfType("test solar panel", mockSolarPanel);
                mockSolarPanel.Setup(b => b.CurrentOutput).Returns(20f);
                mockSolarPanel.Setup(b => b.MaxOutput).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Solar Panel Output: 20%"));
            }
        }
    }
}
