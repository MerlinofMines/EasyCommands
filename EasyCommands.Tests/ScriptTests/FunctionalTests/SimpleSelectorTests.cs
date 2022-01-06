using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using IngameScript;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleSelectorTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void BasicSelector() {
            using (var test = new ScriptTest(@"turn on the ""mockPiston"" piston")) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("mockPiston", mockPiston);

                test.RunOnce();

                mockPiston.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void BasicImpliedSelector() {
            using (var test = new ScriptTest(@"turn on the ""test piston""")) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);

                test.RunOnce();

                mockPiston.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void BasicGroupSelector() {
            using (var test = new ScriptTest(@"turn on the ""mockPistons"" pistons")) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("mockPistons", mockPiston1, mockPiston2);

                test.RunOnce();

                mockPiston1.VerifySet(p => p.Enabled = true);
                mockPiston2.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void MissingGroupDoesIsOk() {
            using (var test = new ScriptTest(@"Print ""Piston Count: "" + the count of ""mockPistons"" pistons")) {

                test.RunOnce();

                Assert.AreEqual("Piston Count: 0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void BasicImpliedGroupSelector() {
            using (var test = new ScriptTest(@"turn on the ""test pistons""")) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2);

                test.RunOnce();

                mockPiston1.VerifySet(p => p.Enabled = true);
                mockPiston2.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void AllSelector() {
            using (var test = new ScriptTest(@"turn on all the pistons")) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2);
                test.MockBlocksOfType("mockPiston", mockPiston3);

                test.RunOnce();

                mockPiston1.VerifySet(p => p.Enabled = true);
                mockPiston2.VerifySet(p => p.Enabled = true);
                mockPiston3.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void MySelector() {
            using (var test = new ScriptTest(@"Print ""My Position: "" + my position")) {
                test.me.Setup(me => me.GetPosition()).Returns(new Vector3D(0, 1, 2));

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("My Position: 0:1:2"));
            }
        }

        [TestMethod]
        public void MySelectorMeaningAll() {
            using (var test = new ScriptTest(@"turn on my battery")) {
                Mock<IMyBatteryBlock> mockBattery = new Mock<IMyBatteryBlock>();

                test.MockBlocksOfType("Battery 1", mockBattery);

                test.RunOnce();

                mockBattery.VerifySet(b => b.ChargeMode = ChargeMode.Auto);
            }
        }

        [TestMethod]
        public void MySelectorDisplays() {
            using (var test = new ScriptTest(@"set my display text to ""Hello World!""")) {
                Mock<IMyProgrammableBlock> otherBlock = new Mock<IMyProgrammableBlock>();
                Mock<IMyTextSurface> mockSurface = new Mock<IMyTextSurface>();
                MockEntityUtility.MockTextSurfaces(otherBlock, mockSurface);

                test.RunOnce();

                test.display.Verify(b => b.WriteText("Hello World!", false));
                mockSurface.VerifyNoOtherCalls();

            }
        }

        [TestMethod]
        public void MySelectorMeaningAllGroup() {
            using (var test = new ScriptTest(@"Print ""Total Batteries: "" + the count of my batteries")) {
                Mock<IMyBatteryBlock> mockBattery1 = new Mock<IMyBatteryBlock>();
                Mock<IMyBatteryBlock> mockBattery2 = new Mock<IMyBatteryBlock>();

                test.MockBlocksOfType("Battery 1", mockBattery1);
                test.MockBlocksOfType("Battery 2", mockBattery2);

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Total Batteries: 2"));
            }
        }

        [TestMethod]
        public void NumericIndexSelector() {
            using (var test = new ScriptTest(@"turn on the ""test pistons"" @ 0")) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2);

                test.RunOnce();

                mockPiston1.VerifySet(p => p.Enabled = true);
                mockPiston2.VerifyNoOtherCalls();
            }
        }

        [TestMethod]
        public void StringIndexSelector() {
            using (var test = new ScriptTest(@"turn on the ""test pistons"" @ ""test piston 1""")) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2);
                test.MockBlocksOfType("test piston 1", mockPiston2);
                test.MockBlocksOfType("test piston 1", mockPiston3);
                test.RunOnce();

                mockPiston2.VerifySet(p => p.Enabled = true);
                mockPiston1.Verify(p => p.CustomName);
                mockPiston1.VerifyNoOtherCalls();
                mockPiston3.VerifyNoOtherCalls();
            }
        }

        [TestMethod]
        public void ListIndexSelector() {
            using (var test = new ScriptTest(@"turn on the ""test pistons"" [0..1]")) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2, mockPiston3);

                test.RunOnce();

                mockPiston1.VerifySet(p => p.Enabled = true);
                mockPiston2.VerifySet(p => p.Enabled = true);
                mockPiston3.VerifyNoOtherCalls();
            }
        }

        [TestMethod]
        public void NamedIndexSelector() {
            using (var test = new ScriptTest(@"turn on the ""test pistons"" ['top piston']")) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2, mockPiston3);
                test.MockBlocksOfType("top piston", mockPiston3);
                test.RunOnce();

                mockPiston1.Verify(p => p.CustomName);
                mockPiston1.VerifyNoOtherCalls();
                mockPiston2.Verify(p => p.CustomName);
                mockPiston2.VerifyNoOtherCalls();
                mockPiston3.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void ConditionalIndexSelector() {
            using (var test = new ScriptTest(@"turn on the power to ""test batteries"" that are recharging")) {
                var mockBattery1 = new Mock<IMyBatteryBlock>();
                var mockBattery2 = new Mock<IMyBatteryBlock>();
                var mockBattery3 = new Mock<IMyBatteryBlock>();
                test.MockBlocksInGroup("test batteries", mockBattery1, mockBattery2, mockBattery3);

                mockBattery1.Setup(p => p.ChargeMode).Returns(ChargeMode.Recharge);
                mockBattery2.Setup(p => p.ChargeMode).Returns(ChargeMode.Auto);
                mockBattery3.Setup(p => p.ChargeMode).Returns(ChargeMode.Recharge);

                test.RunOnce();

                mockBattery1.VerifySet(p => p.Enabled = true);
                mockBattery2.Verify(p => p.ChargeMode);
                mockBattery2.VerifyNoOtherCalls();
                mockBattery3.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void AndConditionalIndexSelector() {
            using (var test = new ScriptTest(@"turn on the power to ""test batteries"" that are recharging and whose level > 10000")) {
                var mockBattery1 = new Mock<IMyBatteryBlock>();
                var mockBattery2 = new Mock<IMyBatteryBlock>();
                var mockBattery3 = new Mock<IMyBatteryBlock>();
                test.MockBlocksInGroup("test batteries", mockBattery1, mockBattery2, mockBattery3);

                mockBattery1.Setup(p => p.ChargeMode).Returns(ChargeMode.Recharge);
                mockBattery2.Setup(p => p.ChargeMode).Returns(ChargeMode.Auto);
                mockBattery3.Setup(p => p.ChargeMode).Returns(ChargeMode.Recharge);

                mockBattery1.Setup(p => p.CurrentStoredPower).Returns(5000);
                mockBattery2.Setup(p => p.CurrentStoredPower).Returns(11000);
                mockBattery3.Setup(p => p.CurrentStoredPower).Returns(12000);

                test.RunOnce();

                mockBattery1.Verify(p => p.ChargeMode);
                mockBattery1.Verify(p => p.CurrentStoredPower);
                mockBattery1.VerifyNoOtherCalls();
                mockBattery2.Verify(p => p.ChargeMode);
                mockBattery2.VerifyNoOtherCalls();
                mockBattery3.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void MultiAndConditionalIndexSelector() {
            using (var test = new ScriptTest(@"turn on the power to ""test batteries"" that are recharging and whose level > 10000 and that are off")) {
                var mockBattery1 = new Mock<IMyBatteryBlock>();
                var mockBattery2 = new Mock<IMyBatteryBlock>();
                var mockBattery3 = new Mock<IMyBatteryBlock>();
                test.MockBlocksInGroup("test batteries", mockBattery1, mockBattery2, mockBattery3);

                mockBattery1.Setup(p => p.ChargeMode).Returns(ChargeMode.Recharge);
                mockBattery2.Setup(p => p.ChargeMode).Returns(ChargeMode.Auto);
                mockBattery3.Setup(p => p.ChargeMode).Returns(ChargeMode.Recharge);

                mockBattery1.Setup(p => p.CurrentStoredPower).Returns(5000);
                mockBattery2.Setup(p => p.CurrentStoredPower).Returns(11000);
                mockBattery3.Setup(p => p.CurrentStoredPower).Returns(12000);

                mockBattery1.Setup(p => p.Enabled).Returns(false);
                mockBattery2.Setup(p => p.Enabled).Returns(false);
                mockBattery3.Setup(p => p.Enabled).Returns(false);

                test.RunOnce();

                mockBattery1.Verify(p => p.ChargeMode);
                mockBattery1.Verify(p => p.CurrentStoredPower);
                mockBattery1.VerifyNoOtherCalls();
                mockBattery2.Verify(p => p.ChargeMode);
                mockBattery2.VerifyNoOtherCalls();
                mockBattery3.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void OrConditionalIndexSelector() {
            using (var test = new ScriptTest(@"turn on the power to ""test batteries"" that are recharging or whose level > 10000")) {
                var mockBattery1 = new Mock<IMyBatteryBlock>();
                var mockBattery2 = new Mock<IMyBatteryBlock>();
                var mockBattery3 = new Mock<IMyBatteryBlock>();
                test.MockBlocksInGroup("test batteries", mockBattery1, mockBattery2, mockBattery3);

                mockBattery1.Setup(p => p.ChargeMode).Returns(ChargeMode.Auto);
                mockBattery2.Setup(p => p.ChargeMode).Returns(ChargeMode.Auto);
                mockBattery3.Setup(p => p.ChargeMode).Returns(ChargeMode.Recharge);

                mockBattery1.Setup(p => p.CurrentStoredPower).Returns(5000);
                mockBattery2.Setup(p => p.CurrentStoredPower).Returns(11000);
                mockBattery3.Setup(p => p.CurrentStoredPower).Returns(5000);

                test.RunOnce();

                mockBattery1.Verify(p => p.ChargeMode);
                mockBattery1.Verify(p => p.CurrentStoredPower);
                mockBattery1.VerifyNoOtherCalls();
                mockBattery2.VerifySet(p => p.Enabled = true);
                mockBattery3.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void VariableSelector() {
            String script = @"
assign myPistons to ""test pistons""
turn on $myPistons
";

            using (var test = new ScriptTest(script)) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2);

                test.RunOnce();

                mockPiston1.VerifySet(p => p.Enabled = true);
                mockPiston2.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void AmbiguousVariableSelector() {
            String script = @"
assign myPistons to ""test pistons""
turn on myPistons pistons
";

            using (var test = new ScriptTest(script)) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2);

                test.RunOnce();

                mockPiston1.VerifySet(p => p.Enabled = true);
                mockPiston2.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void VariableAtIndexSelector() {
            String script = @"
assign myPistons to ""test pistons""
turn on $myPistons @ 0
";

            using (var test = new ScriptTest(script)) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2);

                test.RunOnce();

                mockPiston1.VerifySet(p => p.Enabled = true);
                mockPiston2.VerifyNoOtherCalls();
            }
        }

        [TestMethod]
        public void VariableListIndexSelector() {
            String script = @"
assign myPistons to ""test pistons""
turn on $myPistons[1..2]
";

            using (var test = new ScriptTest(script)) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2, mockPiston3);

                test.RunOnce();

                mockPiston1.VerifyNoOtherCalls();
                mockPiston2.VerifySet(p => p.Enabled = true);
                mockPiston3.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void VariableListStringIndexSelector() {
            String script = @"
assign myPistons to ""test pistons""
turn on $myPistons['top piston']
";

            using (var test = new ScriptTest(script)) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2);
                test.MockBlocksOfType("top piston", mockPiston2);
                test.RunOnce();

                mockPiston1.Verify(p => p.CustomName);
                mockPiston1.VerifyNoOtherCalls();
                mockPiston2.VerifySet(p => p.Enabled = true);
            }
        }

        [TestMethod]
        public void VariableListNumericIndexOverStringIndexSelector() {
            String script = @"
assign myPistons to ""test pistons""
turn on $myPistons['top piston'][1]
";

            using (var test = new ScriptTest(script)) {
                var mockPiston1 = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston1, mockPiston2, mockPiston3);
                test.MockBlocksOfType("top piston", mockPiston2, mockPiston3);
                test.RunOnce();

                mockPiston1.Verify(p => p.CustomName);
                mockPiston1.VerifyNoOtherCalls();
                mockPiston2.Verify(p => p.CustomName);
                mockPiston2.VerifyNoOtherCalls();
                mockPiston3.VerifySet(p => p.Enabled = true);
            }
        }
    }
}
