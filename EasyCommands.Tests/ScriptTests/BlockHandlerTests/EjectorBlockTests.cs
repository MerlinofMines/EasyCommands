using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class EjectorBlockTests : ForceLocale {
        [TestMethod]
        public void TellTheEjectorToDrain() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test ejector"" to drain")) {
                Mock<IMyShipConnector> mockEjector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test ejector", mockEjector);

                test.RunUntilDone();

                mockEjector.VerifySet(b => b.ThrowOut = true);
            }
        }

        [TestMethod]
        public void TellTheEjectorToStopDraining() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test ejector"" to stop draining")) {
                Mock<IMyShipConnector> mockEjector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test ejector", mockEjector);

                test.RunUntilDone();

                mockEjector.VerifySet(b => b.ThrowOut = false);
            }
        }

        [TestMethod]
        public void TellTheEjectorToCollect() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test ejector"" to collect")) {
                Mock<IMyShipConnector> mockEjector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test ejector", mockEjector);

                test.RunUntilDone();

                mockEjector.VerifySet(b => b.CollectAll = true);
            }
        }

        [TestMethod]
        public void TellTheEjectorToStopCollecting() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test ejector"" to stop collecting")) {
                Mock<IMyShipConnector> mockEjector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test ejector", mockEjector);

                test.RunUntilDone();

                mockEjector.VerifySet(b => b.CollectAll = false);
            }
        }
    }
}
