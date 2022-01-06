using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class AirVentBlockTests {
        [TestMethod]
        public void PressurizeTheAirVent() {
            using (ScriptTest test = new ScriptTest(@"pressurize the ""test airVent""")) {
                Mock<IMyAirVent> mockAirVent= new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);

                test.RunUntilDone();

                mockAirVent.VerifySet(b => b.Depressurize = false);
            }
        }

        [TestMethod]
        public void IsTheAirVentPressurizing() {
            using (ScriptTest test = new ScriptTest(@"print ""Air Vent Pressurizing: "" + the ""test airVent"" is pressurizing")) {
                Mock<IMyAirVent> mockAirVent = new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.Depressurize).Returns(false);

                test.RunUntilDone();

                Assert.AreEqual("Air Vent Pressurizing: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheAirVentDePressurizing() {
            using (ScriptTest test = new ScriptTest(@"print ""Air Vent Depressurizing: "" + the ""test airVent"" is depressurizing")) {
                Mock<IMyAirVent> mockAirVent = new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.Depressurize).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Air Vent Depressurizing: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheAirVentPressurized() {
            using (ScriptTest test = new ScriptTest(@"print ""Air Vent Pressurized: "" + the ""test airVent"" is pressurized")) {
                Mock<IMyAirVent> mockAirVent= new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.Status).Returns(VentStatus.Pressurized);

                test.RunUntilDone();

                Assert.AreEqual("Air Vent Pressurized: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void DepressurizeTheAirVent() {
            using (ScriptTest test = new ScriptTest(@"depressurize the ""test airVent""")) {
                Mock<IMyAirVent> mockAirVent = new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);

                test.RunUntilDone();

                mockAirVent.VerifySet(b => b.Depressurize = true);
            }
        }

        [TestMethod]
        public void IsTheAirVentDepressurized() {
            using (ScriptTest test = new ScriptTest(@"print ""Air Vent Depressurized: "" + the ""test airVent"" is depressurized")) {
                Mock<IMyAirVent> mockAirVent = new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.Status).Returns(VentStatus.Depressurized);

                test.RunUntilDone();

                Assert.AreEqual("Air Vent Depressurized: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AirVentRatioLessThan0001IsDepressurized() {
            using (ScriptTest test = new ScriptTest(@"print ""Air Vent Depressurized: "" + the ""test airVent"" is depressurized")) {
                Mock<IMyAirVent> mockAirVent = new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.Status).Returns(VentStatus.Depressurizing);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0.00001f);

                test.RunUntilDone();

                Assert.AreEqual("Air Vent Depressurized: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheAirVentRunning() {
            using (ScriptTest test = new ScriptTest(@"print ""Air Vent Running: "" + the ""test airVent"" is running")) {
                Mock<IMyAirVent> mockAirVent= new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.Status).Returns(VentStatus.Pressurizing);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0.1f);

                test.RunUntilDone();

                Assert.AreEqual("Air Vent Running: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RunninngAirVentLevelLessThan0001IsNotRunning() {
            using (ScriptTest test = new ScriptTest(@"print ""Air Vent Running: "" + the ""test airVent"" is running")) {
                Mock<IMyAirVent> mockAirVent = new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.Status).Returns(VentStatus.Pressurizing);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0.0001f);
                test.RunUntilDone();

                Assert.AreEqual("Air Vent Running: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheAirVentComplete() {
            using (ScriptTest test = new ScriptTest(@"print ""Air Vent Complete: "" + the ""test airVent"" is complete")) {
                Mock<IMyAirVent> mockAirVent = new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.Status).Returns(VentStatus.Pressurized);

                test.RunUntilDone();

                Assert.AreEqual("Air Vent Complete: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheAirVentLevel() {
            using (ScriptTest test = new ScriptTest(@"Print ""Air Vent Level: "" + the ""test airVent"" level")) {
                Mock<IMyAirVent> mockAirVent= new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0.5f);

                test.RunUntilDone();

                Assert.AreEqual("Air Vent Level: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheAirVentRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Air Vent Ratio: "" + the ""test airVent"" ratio")) {
                Mock<IMyAirVent> mockAirVent = new Mock<IMyAirVent>();
                test.MockBlocksOfType("test airVent", mockAirVent);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0.5f);

                test.RunUntilDone();

                Assert.AreEqual("Air Vent Ratio: 0.5", test.Logger[0]);
            }
        }
    }
}
