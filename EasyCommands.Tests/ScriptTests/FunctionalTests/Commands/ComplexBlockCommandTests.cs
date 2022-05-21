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
    public class ComplexBlockCommandTests {
        [TestMethod]
        public void SetPropertyToPropertyOfOther() {
            using (ScriptTest test = new ScriptTest(@"set ""My Beacon"" range to the limit of ""My Beacon 2""")) {
                var mockBeacon = new Mock<IMyBeacon>();
                var mockBeacon2 = new Mock<IMyBeacon>();
                test.MockBlocksOfType("My Beacon", mockBeacon);
                test.MockBlocksOfType("My Beacon 2", mockBeacon2);

                mockBeacon2.Setup(b => b.Radius).Returns(2000);

                test.RunUntilDone();

                mockBeacon.VerifySet(b => b.Radius = 2000);
            }
        }

        [TestMethod]
        public void SetDirectionalPropertyToDirectionalPropertyOfOther() {
            using (ScriptTest test = new ScriptTest(@"set ""My Piston"" upper limit to the upper limit of ""My Piston 2""")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("My Piston", mockPiston);
                test.MockBlocksOfType("My Piston 2", mockPiston2);

                mockPiston2.Setup(b => b.MaxLimit).Returns(6);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MaxLimit = 6f);
            }
        }

        [TestMethod]
        public void SetPropertyToAggregateBlockProperty() {
            using (ScriptTest test = new ScriptTest(@"set ""My Pistons"" height to the avg ""My Pistons"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("My Pistons", mockPiston, mockPiston2);

                mockPiston.Setup(b => b.CurrentPosition).Returns(4);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(6);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MaxLimit = 5f);
                mockPiston.Verify(b => b.Extend());
                mockPiston2.VerifySet(b => b.MinLimit = 5f);
                mockPiston2.Verify(b => b.Retract());
            }
        }

        [TestMethod]
        public void MultiActionPropertyToAggregateBlockProperty() {
            using (ScriptTest test = new ScriptTest(@"tell ""My Pistons"" to move to the average height of ""My Pistons""")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("My Pistons", mockPiston, mockPiston2);

                mockPiston.Setup(b => b.CurrentPosition).Returns(4);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(6);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MaxLimit = 5f);
                mockPiston.Verify(b => b.Extend());
                mockPiston2.VerifySet(b => b.MinLimit = 5f);
                mockPiston2.Verify(b => b.Retract());
            }
        }

        [TestMethod]
        public void RearrangePropertyOfBlockConditionWithConditionalSelector() {
            using (ScriptTest test = new ScriptTest(@"
if the ratio of all batteries that are recharging < 0.5
  Print ""Sound the alarm!""")) {
                var mockBattery = new Mock<IMyBatteryBlock>();
                var mockBattery2 = new Mock<IMyBatteryBlock>();
                test.MockBlocksInGroup("My Batteries", mockBattery, mockBattery2);

                mockBattery.Setup(b => b.ChargeMode).Returns(ChargeMode.Recharge);
                mockBattery.Setup(b => b.CurrentStoredPower).Returns(10f);
                mockBattery.Setup(b => b.MaxStoredPower).Returns(40f);

                mockBattery2.Setup(b => b.ChargeMode).Returns(ChargeMode.Auto);
                mockBattery2.Setup(b => b.CurrentStoredPower).Returns(30f);
                mockBattery2.Setup(b => b.MaxStoredPower).Returns(40f);

                test.RunUntilDone();

                Assert.AreEqual("Sound the alarm!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RearrangePropertyOfBlockConditionWithConditionalSelectorInParentheses() {
            using (ScriptTest test = new ScriptTest(@"
if the ratio of all (batteries that are recharging) < 0.5
  Print ""Sound the alarm!""")) {
                var mockBattery = new Mock<IMyBatteryBlock>();
                var mockBattery2 = new Mock<IMyBatteryBlock>();
                test.MockBlocksInGroup("My Batteries", mockBattery, mockBattery2);

                mockBattery.Setup(b => b.ChargeMode).Returns(ChargeMode.Recharge);
                mockBattery.Setup(b => b.CurrentStoredPower).Returns(10f);
                mockBattery.Setup(b => b.MaxStoredPower).Returns(40f);

                mockBattery2.Setup(b => b.ChargeMode).Returns(ChargeMode.Auto);
                mockBattery2.Setup(b => b.CurrentStoredPower).Returns(30f);
                mockBattery2.Setup(b => b.MaxStoredPower).Returns(40f);

                test.RunUntilDone();

                Assert.AreEqual("Sound the alarm!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RearrangeReadyPropertyOfBlockConditionWithConditionalSelector() {
            using (ScriptTest test = new ScriptTest(@"set the strength of any connectors that are ready to connect to 0")) {
                var mockConnector = new Mock<IMyShipConnector>();
                var mockConnector2 = new Mock<IMyShipConnector>();
                test.MockBlocksInGroup("My Connectors", mockConnector, mockConnector2);

                mockConnector.Setup(b => b.Status).Returns(MyShipConnectorStatus.Connectable);
                mockConnector2.Setup(b => b.Status).Returns(MyShipConnectorStatus.Connected);

                test.RunUntilDone();

                mockConnector.VerifySet(b => b.PullStrength = 0f);
                mockConnector2.VerifySet(b => b.PullStrength = 0f, Times.Never);
            }
        }
    }
}
