using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class WindTurbineBlockTests {
        [TestMethod]
        public void GetWindTurbineOutput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wind Turbine Output: "" + ""test wind turbine"" output")) {
                Mock<IMyPowerProducer> mockTurbine = new Mock<IMyPowerProducer>();
                test.MockBlocksOfType("test wind turbine", mockTurbine);
                MockBlockDefinition(mockTurbine, "WindTurbine");
                mockTurbine.Setup(b => b.CurrentOutput).Returns(20f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Wind Turbine Output: 20"));
            }
        }

        [TestMethod]
        public void GetWindTurbineLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wind Turbine Max Output: "" + ""test wind turbine"" limit")) {
                Mock<IMyPowerProducer> mockTurbine = new Mock<IMyPowerProducer>();
                test.MockBlocksOfType("test wind turbine", mockTurbine);
                MockBlockDefinition(mockTurbine, "WindTurbine");
                mockTurbine.Setup(b => b.MaxOutput).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Wind Turbine Max Output: 100"));
            }
        }

        [TestMethod]
        public void GetWindTurbineRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Wind Turbine Output: "" + ( 100 * ""test wind turbine"" ratio ) + ""%""")) {
                Mock<IMyPowerProducer> mockTurbine = new Mock<IMyPowerProducer>();
                test.MockBlocksOfType("test wind turbine", mockTurbine);
                MockBlockDefinition(mockTurbine, "WindTurbine");
                mockTurbine.Setup(b => b.CurrentOutput).Returns(20f);
                mockTurbine.Setup(b => b.MaxOutput).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Wind Turbine Output: 20%"));
            }
        }
    }
}
