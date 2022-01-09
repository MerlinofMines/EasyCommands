using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class GasTankBlockTests {
        [TestMethod]
        public void TurnOnTheGasTank() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test tank""")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);

                test.RunUntilDone();

                mockTank.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheGasTank() {
            using (ScriptTest test = new ScriptTest(@"turn off the ""test tank""")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);

                test.RunUntilDone();

                mockTank.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void SetTheGasTankToAuto() {
            using (ScriptTest test = new ScriptTest(@"set the ""test tank"" to auto")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);

                test.RunUntilDone();

                mockTank.VerifySet(b => b.AutoRefillBottles = true);
            }
        }

        [TestMethod]
        public void TellTheGasTankToRefill() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test tank"" to refill")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);

                test.RunUntilDone();

                mockTank.VerifySet(b => b.AutoRefillBottles = true);
            }
        }

        [TestMethod]
        public void IsTheGasTankStockpiling() {
            using (ScriptTest test = new ScriptTest(@"Print ""Stockpiling: "" + the ""test tank"" is stockpiling")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);
                mockTank.Setup(b => b.Stockpile).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Stockpiling: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TellTheGasTankToStockpile() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test tank"" to stockpile")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);

                test.RunUntilDone();

                mockTank.VerifySet(b => b.Stockpile = true);
            }
        }

        [TestMethod]
        public void TellTheGasTankToSupply() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test tank"" to supply")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);

                test.RunUntilDone();

                mockTank.VerifySet(b => b.Stockpile = false);
            }
        }

        [TestMethod]
        public void TellTheGasTankToStopStockpiling() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test tank"" to stop stockpiling")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);

                test.RunUntilDone();

                mockTank.VerifySet(b => b.Stockpile = false);
            }
        }

        [TestMethod]
        public void GetTheTankCapacity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Capacity: "" + the ""test tank"" capacity")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);
                mockTank.Setup(b => b.Capacity).Returns(10000f);

                test.RunUntilDone();

                Assert.AreEqual("Capacity: 10000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheTankRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Ratio: "" + the ""test tank"" ratio")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);
                mockTank.Setup(b => b.FilledRatio).Returns(0.4);

                test.RunUntilDone();

                Assert.AreEqual("Ratio: 0.4", test.Logger[0]);
            }
        }
    }
}
