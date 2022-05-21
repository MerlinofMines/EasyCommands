using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ConnectorBlockTests {
        [TestMethod]
        public void TellTheConnectorToDrain() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test connector"" to drain")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.VerifySet(b => b.ThrowOut = true);
            }
        }

        [TestMethod]
        public void TellTheConnectorToStopDraining() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test connector"" to stop draining")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.VerifySet(b => b.ThrowOut = false);
            }
        }

        [TestMethod]
        public void TellTheConnectorToCollect() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test connector"" to collect")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.VerifySet(b => b.CollectAll = true);
            }
        }

        [TestMethod]
        public void TellTheConnectorToStopCollecting() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test connector"" to stop collecting")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.VerifySet(b => b.CollectAll = false);
            }
        }

        [TestMethod]
        public void SetTheConnectorStrength() {
            using (ScriptTest test = new ScriptTest(@"set the ""test connector"" strength to 0.2")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.VerifySet(b => b.PullStrength = 0.2f);
            }
        }

        [TestMethod]
        public void SetTheConnectorStrengthImplicit() {
            using (ScriptTest test = new ScriptTest(@"set the ""test connector"" to 0.2")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.VerifySet(b => b.PullStrength = 0.2f);
            }
        }

        [TestMethod]
        public void IsTheConnectorConnected() {
            using (ScriptTest test = new ScriptTest(@"Print ""Connected: "" + ""test connector"" is connected")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);
                mockConnector.Setup(b => b.Status).Returns(MyShipConnectorStatus.Connected);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Connected: True"));
            }
        }

        [TestMethod]
        public void IsTheConnectorLocked() {
            using (ScriptTest test = new ScriptTest(@"Print ""Locked: "" + ""test connector"" is locked")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);
                mockConnector.Setup(b => b.Status).Returns(MyShipConnectorStatus.Connected);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Locked: True"));
            }
        }

        [TestMethod]
        public void ConnectTheConnector() {
            using (ScriptTest test = new ScriptTest(@"connect the ""test connector""")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.Verify(b => b.Connect());
            }
        }

        [TestMethod]
        public void LockTheConnector() {
            using (ScriptTest test = new ScriptTest(@"lock the ""test connector""")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.Verify(b => b.Connect());
            }
        }

        [TestMethod]
        public void DisconnectTheConnector() {
            using (ScriptTest test = new ScriptTest(@"disconnect the ""test connector""")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.Verify(b => b.Disconnect());
            }
        }

        [TestMethod]
        public void UnlockTheConnector() {
            using (ScriptTest test = new ScriptTest(@"unlock the ""test connector""")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);

                test.RunUntilDone();

                mockConnector.Verify(b => b.Disconnect());
            }
        }

        [TestMethod]
        public void IsTheConnectorReady() {
            using (ScriptTest test = new ScriptTest(@"Print ""Ready: "" + ""test connector"" is ready")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);
                mockConnector.Setup(b => b.Status).Returns(MyShipConnectorStatus.Connectable);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Ready: True"));
            }
        }

        [TestMethod]
        public void IsTheConnectorReadyWhenNotReady() {
            using (ScriptTest test = new ScriptTest(@"Print ""Ready: "" + ""test connector"" is ready")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);
                mockConnector.Setup(b => b.Status).Returns(MyShipConnectorStatus.Connected);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Ready: False"));
            }
        }

        [TestMethod]
        public void IsTheConnectorReadyToConnect() {
            using (ScriptTest test = new ScriptTest(@"Print ""Ready: "" + ""test connector"" is able to be connected")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);
                mockConnector.Setup(b => b.Status).Returns(MyShipConnectorStatus.Connectable);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Ready: True"));
            }
        }

        [TestMethod]
        public void TheConnectorCanBeLocked() {
            using (ScriptTest test = new ScriptTest(@"Print ""Ready: "" + ""test connector"" can be locked")) {
                Mock<IMyShipConnector> mockConnector = new Mock<IMyShipConnector>();
                test.MockBlocksOfType("test connector", mockConnector);
                mockConnector.Setup(b => b.Status).Returns(MyShipConnectorStatus.Connectable);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Ready: True"));
            }
        }

    }
}
