using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;
using VRage.Game.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class MultiWordPropertyBlockCommandTests {
        [TestMethod]
        public void ReverseWithProperty() {
            using (ScriptTest test = new ScriptTest(@"reverse the ""test wheel"" steering override")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(p => p.SteeringOverride).Returns(0.5f);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = -0.5f);
            }
        }

        [TestMethod]
        public void IncrementPropertyOnly() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test wheel"" steering override")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(p => p.SteeringOverride).Returns(0.5f);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.6f);
            }
        }

        [TestMethod]
        public void RaisePropertyDirection() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test wheel"" steering override")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(p => p.SteeringOverride).Returns(0.5f);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.6f);
            }
        }

        [TestMethod]
        public void RaisePropertyDirectionByAmount() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test wheel"" steering override by 0.25")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(p => p.SteeringOverride).Returns(0.5f);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.75f);
            }
        }

        [TestMethod]
        public void DecrementPropertyOnly() {
            using (ScriptTest test = new ScriptTest(@"decrease the ""test wheel"" steering override")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(p => p.SteeringOverride).Returns(0.5f);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.4f);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithActionAndDirection() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test wheel"" steering override by 0.25")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(p => p.SteeringOverride).Returns(0.5f);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.75f);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithDirection() {
            using (ScriptTest test = new ScriptTest(@"up the ""test wheel"" steering override by 0.25")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(p => p.SteeringOverride).Returns(0.5f);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.75f);
            }
        }

        [TestMethod]
        public void SetPropertyWithActionAndDirectionAndValue() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test wheel"" steering override to 0.75")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.75f);
            }
        }

        [TestMethod]
        public void SetPropertyWithDirectionAndValue() {
            using (ScriptTest test = new ScriptTest(@"up the ""test wheel"" steering override to 0.75")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.75f);
            }
        }

        [TestMethod]
        public void MovePropertyWithActionAndDirection() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test wheel"" steering override")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(p => p.SteeringOverride).Returns(0.5f);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.6f);
            }
        }

        [TestMethod]
        public void MovePropertyWithDirection() {
            using (ScriptTest test = new ScriptTest(@"up the ""test wheel"" steering override")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                mockWheel.Setup(p => p.SteeringOverride).Returns(0.5f);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.6f);
            }
        }

        [TestMethod]
        public void SetPropertyWithActionAndValue() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" steering override to 0.75")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.75f);
            }
        }

        [TestMethod]
        public void SetPropertyWithValue() {
            using (ScriptTest test = new ScriptTest(@"""test wheel"" steering override to 0.75")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.75f);
            }
        }

        [TestMethod]
        public void SetPropertyWithNot() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test wheel"" not invert steering")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.InvertSteer = false);
            }
        }

        [TestMethod]
        public void SetPropertyWithAction() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test wheel"" to invert steering")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.InvertSteer = true);
            }
        }

        [TestMethod]
        public void SetPropertyWithPropertyWordsInFront() {
            using (ScriptTest test = new ScriptTest(@"invert the steering of the ""test wheel""")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.InvertSteer = true);
            }
        }

        [TestMethod]
        public void SetPropertyWithPropertyWordsSplit() {
            using (ScriptTest test = new ScriptTest(@"invert the ""test wheel"" steering")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.InvertSteer = true);
            }
        }

        [TestMethod]
        public void SetUnsupportedProperty() {
            using (ScriptTest test = new ScriptTest(@"set the ""test beacon"" angle override to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                MockBlockDefinition(mockBeacon, "IMyBeacon");

                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("IMyBeacon does not have property support for: angle override", test.Logger[1]);
            }
        }

        [TestMethod]
        public void SetUnsupportedPropertyOnNonTerminalBlock() {
            using (ScriptTest test = new ScriptTest(@"set the ""test inventory"" angle override to 200")) {
                Mock<IMyCargoContainer> mockCargo = new Mock<IMyCargoContainer>();
                Mock<IMyInventory> mockInventory = new Mock<IMyInventory>();
                MockInventories(mockCargo, mockInventory);
                test.MockBlocksOfType("test inventory", mockCargo);

                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("IMyInventory does not have property support for: angle override", test.Logger[1]);
            }
        }

        [TestMethod]
        public void SetUnsupportedPropertyPrintOutSuppliedWord() {
            using (ScriptTest test = new ScriptTest(@"invert the steering of the ""test beacon""")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                MockBlockDefinition(mockBeacon, "IMyBeacon");

                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("IMyBeacon does not have property support for: invert steering", test.Logger[1]);
            }
        }

        [TestMethod]
        public void SetDynamicProperty() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" ""steering override"" property to 0.75")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                test.RunUntilDone();

                mockWheel.VerifySet(b => b.SteeringOverride = 0.75f);
            }
        }

        [TestMethod]
        public void SetUnsupportedDynamicProperty() {
            using (ScriptTest test = new ScriptTest(@"set the ""test beacon"" ""angle override"" property to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                MockBlockDefinition(mockBeacon, "IMyBeacon");

                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("IMyBeacon does not have property support for: angle override", test.Logger[1]);
            }
        }

        [TestMethod]
        public void SetUnsupportedDynamicPropertyOnNonTerminalBlock() {
            using (ScriptTest test = new ScriptTest(@"set the ""test inventory"" ""angle override"" property to 200")) {
                Mock<IMyCargoContainer> mockCargo = new Mock<IMyCargoContainer>();
                Mock<IMyInventory> mockInventory = new Mock<IMyInventory>();
                MockInventories(mockCargo, mockInventory);
                test.MockBlocksOfType("test inventory", mockCargo);

                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("IMyInventory does not have property support for: angle override", test.Logger[1]);
            }
        }

        [TestMethod]
        public void SetDynamicMultiWordTerminalBlockProperty() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" ""Random Property"" property to 50")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                var mockProperty = MockProperty<IMyMotorSuspension, float>(mockWheel, "Random Property");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockWheel.Object, 50));
            }
        }
    }
}
