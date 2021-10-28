using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class EngineBlockTests {
        [TestMethod]
        public void GetEngineOutput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Engine Output: "" + ""test engine"" output")) {
                Mock<IMyPowerProducer> mockEngine = new Mock<IMyPowerProducer>();
                test.MockBlocksOfType("test engine", mockEngine);
                MockBlockDefinition(mockEngine, "HydrogenEngine");
                mockEngine.Setup(b => b.CurrentOutput).Returns(20f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Engine Output: 20"));
            }
        }

        [TestMethod]
        public void GetEngineLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Engine Max Output: "" + ""test engine"" limit")) {
                Mock<IMyPowerProducer> mockEngine = new Mock<IMyPowerProducer>();
                test.MockBlocksOfType("test engine", mockEngine);
                MockBlockDefinition(mockEngine, "HydrogenEngine");
                mockEngine.Setup(b => b.MaxOutput).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Engine Max Output: 100"));
            }
        }

        [TestMethod]
        public void GetEngineRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Engine Output: "" + ( 100 * ""test engine"" ratio ) + ""%""")) {
                Mock<IMyPowerProducer> mockEngine = new Mock<IMyPowerProducer>();
                test.MockBlocksOfType("test engine", mockEngine);
                MockBlockDefinition(mockEngine, "HydrogenEngine");
                mockEngine.Setup(b => b.CurrentOutput).Returns(20f);
                mockEngine.Setup(b => b.MaxOutput).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Engine Output: 20%"));
            }
        }
    }
}
