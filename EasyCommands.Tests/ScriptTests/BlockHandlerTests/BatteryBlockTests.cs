using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class BatteryBlockTests : ForceLocale {
        [TestMethod]
        public void GetTheBatteryLevel() {
            using (ScriptTest test = new ScriptTest(@"Print ""Stored Power: "" + the ""test battery"" level")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();
                test.MockBlocksOfType("test battery", mockBattery);
                mockBattery.Setup(b => b.CurrentStoredPower).Returns(10f);
                test.RunUntilDone();

                Assert.AreEqual("Stored Power: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheBatteryCapacity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Capacity: "" + the ""test battery"" capacity")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();
                test.MockBlocksOfType("test battery", mockBattery);
                mockBattery.Setup(b => b.MaxStoredPower).Returns(100f);
                test.RunUntilDone();

                Assert.AreEqual("Capacity: 100", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheBatteryRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Power Ratio: "" + (100 * the ""test battery"" ratio) + ""%""")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();
                test.MockBlocksOfType("test battery", mockBattery);
                mockBattery.Setup(b => b.CurrentStoredPower).Returns(20f);
                mockBattery.Setup(b => b.MaxStoredPower).Returns(40f);
                test.RunUntilDone();

                Assert.AreEqual("Power Ratio: 50%", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheBatteryOutput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Battery Output: "" + the ""test battery"" output")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();
                test.MockBlocksOfType("test battery", mockBattery);
                mockBattery.Setup(b => b.CurrentOutput).Returns(10f);
                test.RunUntilDone();

                Assert.AreEqual("Battery Output: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheBatteryInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Battery Input: "" + the ""test battery"" input")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();
                test.MockBlocksOfType("test battery", mockBattery);
                mockBattery.Setup(b => b.CurrentInput).Returns(10f);
                test.RunUntilDone();

                Assert.AreEqual("Battery Input: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheBatteryRecharging() {
            using (ScriptTest test = new ScriptTest(@"Print ""Charging: "" + the ""test battery"" is recharging")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();
                test.MockBlocksOfType("test battery", mockBattery);
                mockBattery.Setup(b => b.ChargeMode).Returns(ChargeMode.Recharge);
                test.RunUntilDone();

                Assert.AreEqual("Charging: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RechargeTheBattery() {
            using (ScriptTest test = new ScriptTest(@"recharge the ""test battery""")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();
                test.MockBlocksOfType("test battery", mockBattery);
                test.RunUntilDone();

                mockBattery.VerifySet(b => b.ChargeMode = ChargeMode.Recharge);
            }
        }

        [TestMethod]
        public void IsTheBatteryOnAuto() {
            using (ScriptTest test = new ScriptTest(@"Print ""On Auto: "" + the ""test battery"" is on auto")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();
                test.MockBlocksOfType("test battery", mockBattery);
                mockBattery.Setup(b => b.ChargeMode).Returns(ChargeMode.Auto);
                test.RunUntilDone();

                Assert.AreEqual("On Auto: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheBatteryToAuto() {
            using (ScriptTest test = new ScriptTest(@"set the ""test battery"" to auto")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();
                test.MockBlocksOfType("test battery", mockBattery);
                test.RunUntilDone();

                mockBattery.VerifySet(b => b.ChargeMode = ChargeMode.Auto);
            }
        }
    }
}
