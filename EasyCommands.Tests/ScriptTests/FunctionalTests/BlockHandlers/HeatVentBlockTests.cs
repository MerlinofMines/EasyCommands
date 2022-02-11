using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class HeatVentBlockTests {
        [TestMethod]
        public void TurnOnTheHeatVent() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test heatVent""")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);

                test.RunUntilDone();

                mockHeatVent.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheHeatVent() {
            using (ScriptTest test = new ScriptTest(@"turn off the ""test heatVent""")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);

                test.RunUntilDone();

                mockHeatVent.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void GetTheHeatVentColor() {
            using (ScriptTest test = new ScriptTest(@"Print ""Heat Vent Upper Color: "" + the ""test heatVent"" color")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                MockGetProperty(mockHeatVent, "ColorMax", Color.Red);

                test.RunUntilDone();

                Assert.AreEqual("Heat Vent Upper Color: #FF0000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheHeatVentUpperColor() {
            using (ScriptTest test = new ScriptTest(@"Print ""Heat Vent Upper Color: "" + the ""test heatVent"" upper color")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                MockGetProperty(mockHeatVent, "ColorMax", Color.Red);

                test.RunUntilDone();

                Assert.AreEqual("Heat Vent Upper Color: #FF0000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheHeatVentColor() {
            using (ScriptTest test = new ScriptTest(@"set the ""test heatVent"" color to red")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                var mockProperty = MockProperty<IMyHeatVent, Color>(mockHeatVent, "ColorMax");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockHeatVent.Object, Color.Red));
            }
        }

        [TestMethod]
        public void SetTheHeatVentUpperColor() {
            using (ScriptTest test = new ScriptTest(@"set the ""test heatVent"" upper color to red")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                var mockProperty = MockProperty<IMyHeatVent, Color>(mockHeatVent, "ColorMax");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockHeatVent.Object, Color.Red));
            }
        }

        [TestMethod]
        public void GetTheHeatVentLowerColor() {
            using (ScriptTest test = new ScriptTest(@"Print ""Heat Vent Lower Color: "" + the ""test heatVent"" lower color")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                MockGetProperty(mockHeatVent, "ColorMin", Color.Red);

                test.RunUntilDone();

                Assert.AreEqual("Heat Vent Lower Color: #FF0000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheHeatVentLowerColor() {
            using (ScriptTest test = new ScriptTest(@"set the ""test heatVent"" lower color to red")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                var mockProperty = MockProperty<IMyHeatVent, Color>(mockHeatVent, "ColorMin");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockHeatVent.Object, Color.Red));
            }
        }

        [TestMethod]
        public void GetTheHeatVentRadius() {
            using (ScriptTest test = new ScriptTest(@"Print ""Heat Vent Radius: "" + the ""test heatVent"" radius")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                MockGetProperty(mockHeatVent, "Radius", 10f);

                test.RunUntilDone();

                Assert.AreEqual("Heat Vent Radius: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheHeatVentRadius() {
            using (ScriptTest test = new ScriptTest(@"set the ""test heatVent"" radius to 5")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                var mockProperty = MockProperty<IMyHeatVent, float>(mockHeatVent, "Radius");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockHeatVent.Object, 5f));
            }
        }

        [TestMethod]
        public void GetTheHeatVentIntensity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Heat Vent Intensity: "" + the ""test heatVent"" intensity")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                MockGetProperty(mockHeatVent, "Intensity", 10f);

                test.RunUntilDone();

                Assert.AreEqual("Heat Vent Intensity: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheHeatVentIntensity() {
            using (ScriptTest test = new ScriptTest(@"set the ""test heatVent"" intensity to 5")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                var mockProperty = MockProperty<IMyHeatVent, float>(mockHeatVent, "Intensity");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockHeatVent.Object, 5f));
            }
        }

        [TestMethod]
        public void GetTheHeatVentFalloff() {
            using (ScriptTest test = new ScriptTest(@"Print ""Heat Vent Falloff: "" + the ""test heatVent"" falloff")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                MockGetProperty(mockHeatVent, "Falloff", 10f);

                test.RunUntilDone();

                Assert.AreEqual("Heat Vent Falloff: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheHeatVentFalloff() {
            using (ScriptTest test = new ScriptTest(@"set the ""test heatVent"" falloff to 5")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                var mockProperty = MockProperty<IMyHeatVent, float>(mockHeatVent, "Falloff");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockHeatVent.Object, 5f));
            }
        }

        [TestMethod]
        public void GetTheHeatVentOffset() {
            using (ScriptTest test = new ScriptTest(@"Print ""Heat Vent Offset: "" + the ""test heatVent"" offset")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                MockGetProperty(mockHeatVent, "Offset", 1f);

                test.RunUntilDone();

                Assert.AreEqual("Heat Vent Offset: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheHeatVentOffset() {
            using (ScriptTest test = new ScriptTest(@"set the ""test heatVent"" offset to 0.5")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                var mockProperty = MockProperty<IMyHeatVent, float>(mockHeatVent, "Offset");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockHeatVent.Object, 0.5f));
            }
        }

        [TestMethod]
        public void GetTheHeatVentRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Heat Vent Ratio: "" + the ""test heatVent"" ratio")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                MockGetProperty(mockHeatVent, "PowerDependency", 100f);

                test.RunUntilDone();

                Assert.AreEqual("Heat Vent Ratio: 100", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheHeatVentRatio() {
            using (ScriptTest test = new ScriptTest(@"set the ""test heatVent"" ratio to 50")) {
                Mock<IMyHeatVent> mockHeatVent = new Mock<IMyHeatVent>();
                test.MockBlocksOfType("test heatVent", mockHeatVent);
                var mockProperty = MockProperty<IMyHeatVent, float>(mockHeatVent, "PowerDependency");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockHeatVent.Object, 50f));
            }
        }
    }
}
