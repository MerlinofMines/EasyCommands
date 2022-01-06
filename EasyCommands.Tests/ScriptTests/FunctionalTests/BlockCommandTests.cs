using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class BlockCommandTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void ReverseWithProperty() {
            using (ScriptTest test = new ScriptTest(@"reverse the ""test piston"" velocity")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(p => p.Velocity).Returns(1);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.Velocity = -1);
            }
        }

        [TestMethod]
        public void ReverseWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"reverse the ""test piston""")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);

                test.RunUntilDone();

                mockPiston.Verify(b => b.Reverse());
            }
        }

        [TestMethod]
        public void IncrementPropertyOnly() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test beacon"" range")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(1000);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 2000);
            }
        }

        [TestMethod]
        public void RaisePropertyDirection() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test beacon"" range")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(1000);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 2000);
            }
        }

        [TestMethod]
        public void RaisePropertyDirectionByAmount() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test beacon"" range by 100")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(1000);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 1100);
            }
        }

        [TestMethod]
        public void DecrementPropertyOnly() {
            using (ScriptTest test = new ScriptTest(@"decrease the ""test beacon"" range")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(2000);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 1000);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithActionAndDirection() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test beacon"" range by 100")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(100);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithActionAndDirectionWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"increase the ""test beacon"" by 100")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(100);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithDirection() {
            using (ScriptTest test = new ScriptTest(@"up the ""test beacon"" range by 100")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(100);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithDirectionWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"up the ""test beacon"" by 100")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(100);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithAction() {
            using (ScriptTest test = new ScriptTest(@"set the ""test beacon"" range by 100")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(100);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithActionWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"set the ""test beacon"" by 100")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(100);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithoutAction() {
            using (ScriptTest test = new ScriptTest(@"""test beacon"" range by 100")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(100);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void IncrementPropertyWithoutActionAndProperty() {
            using (ScriptTest test = new ScriptTest(@"""test beacon"" by 100")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(p => p.Radius).Returns(100);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void SetPropertyWithActionAndDirectionAndValue() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test beacon"" range to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void SetPropertyWithActionAndDirectionAndValueWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test beacon"" to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void SetPropertyWithDirectionAndValue() {
            using (ScriptTest test = new ScriptTest(@"up the ""test beacon"" range to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void SetPropertyWithDirectionAndValueWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"up the ""test beacon"" to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void MovePropertyWithActionAndDirection() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test piston"" height")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);

                test.RunUntilDone();

                mockPiston.Verify(b => b.Extend());
            }
        }

        [TestMethod]
        public void MovePropertyWithActionAndDirectionWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"raise the ""test piston""")) {
                Mock<IMyPistonBase> mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);

                test.RunUntilDone();

                mockPiston.Verify(b => b.Extend());
            }
        }

        [TestMethod]
        public void MovePropertyWithDirection() {
            using (ScriptTest test = new ScriptTest(@"up the ""test beacon"" range")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(b => b.Radius).Returns(1000);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 2000);
            }
        }

        [TestMethod]
        public void MovePropertyWithDirectionWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"up the ""test beacon""")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);
                mockBeacon.Setup(b => b.Radius).Returns(1000);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 2000);
            }
        }

        [TestMethod]
        public void SetPropertyWithActionAndValue() {
            using (ScriptTest test = new ScriptTest(@"set the ""test beacon"" range to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void SetPropertyWithActionAndValueWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"set the ""test beacon"" to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void SetPropertyWithValue() {
            using (ScriptTest test = new ScriptTest(@"""test beacon"" range to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void SetPropertyWithValueWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"""test beacon"" to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        [TestMethod]
        public void SetPropertyWithNot() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test vent"" not to pressurize")) {
                Mock<IMyAirVent> mockVent = new Mock<IMyAirVent>();
                test.MockBlocksOfType("test vent", mockVent);

                test.RunUntilDone();

                mockVent.VerifySet(b => b.Depressurize = true);
            }
        }

        [TestMethod]
        public void SetPropertyWithNotWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"stop the ""test assembler""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.Verify(b => b.ClearQueue());
            }
        }

        [TestMethod]
        public void SetPropertyWithAction() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test bomb"" to detonate")) {
                Mock<IMyWarhead> mockBomb = new Mock<IMyWarhead>();
                test.MockBlocksOfType("test bomb", mockBomb);

                test.RunUntilDone();

                mockBomb.Verify(b => b.Detonate());
            }
        }

        [TestMethod]
        public void SetPropertyWithOnlyProperty() {
            using (ScriptTest test = new ScriptTest(@"detonate the ""test bomb""")) {
                Mock<IMyWarhead> mockBomb = new Mock<IMyWarhead>();
                test.MockBlocksOfType("test bomb", mockBomb);

                test.RunUntilDone();

                mockBomb.Verify(b => b.Detonate());
            }
        }

        [TestMethod]
        public void SetDynamicProperty() {
            using (ScriptTest test = new ScriptTest(@"set the ""test beacon"" ""range"" property to 200")) {
                Mock<IMyBeacon> mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 200);
            }
        }

        //Under the hood, "stockpile" actually means to set supply to false.  This test confirms that we properly
        //negate the value to set based on this fact.
        [TestMethod]
        public void SetDynamicPropertyWithImplicitNegation() {
            using (ScriptTest test = new ScriptTest(@"set the ""test tank"" ""stockpile"" property to true")) {
                Mock<IMyGasTank> mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);

                test.RunUntilDone();

                mockTank.VerifySet(b => b.Stockpile = true);
            }
        }

        [TestMethod]
        public void SetDynamicTerminalBlockProperty() {
            using (ScriptTest test = new ScriptTest(@"set the ""test wheel"" ""Speed Limit"" property to 50")) {
                Mock<IMyMotorSuspension> mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                var mockProperty = MockProperty<IMyMotorSuspension, float>(mockWheel, "Speed Limit");

                test.RunUntilDone();

                mockProperty.Verify(p => p.SetValue(mockWheel.Object, 50));
            }
        }
    }
}
