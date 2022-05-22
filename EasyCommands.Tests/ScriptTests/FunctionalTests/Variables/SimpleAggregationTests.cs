using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleAggregationTests {

        [TestMethod]
        public void ValueOfEmptyListReturnsEmptyList() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + ""test piston"" height")) {

                test.RunOnce();

                Assert.AreEqual("My Value: []", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ValueOfOneItemReturnsValue() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);

                test.RunOnce();

                Assert.AreEqual("My Value: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetValueofDynamicProperty() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + ""test piston"" ""height"" property")) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);

                test.RunOnce();

                Assert.AreEqual("My Value: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetValueofDynamicImplicitNegativeProperty() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + ""test tank"" ""stockpile"" property is on")) {
                var mockTank = new Mock<IMyGasTank>();
                test.MockBlocksOfType("test tank", mockTank);
                mockTank.Setup(b => b.Stockpile).Returns(true);

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetValueofDynamicTerminalBlockProperty() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + ""test wheel"" ""SpeedLimit"" property")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);
                MockGetProperty(mockWheel, "SpeedLimit", 50f);

                test.RunOnce();

                Assert.AreEqual("My Value: 50", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ValueOfMultipleNumericItemsReturnsSum() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston, mockPiston2);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3f);

                test.RunOnce();

                Assert.AreEqual("My Value: 8", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ValueOfMultipleNonNumericItemsReturnsSum() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + piston names")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston, mockPiston2);
                mockPiston.Setup(b => b.CustomName).Returns("piston 1");
                mockPiston2.Setup(b => b.CustomName).Returns("piston 2");

                test.RunOnce();

                Assert.AreEqual("My Value: [\"piston 1\",\"piston 2\"]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SumOfEmptyListReturnsZero() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the sum of ""test piston"" height")) {

                test.RunOnce();

                Assert.AreEqual("My Value: 0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SumOfOneItemReturnsValue() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the sum of ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);

                test.RunOnce();

                Assert.AreEqual("My Value: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SumOfMultipleItemsReturnsSum() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the sum of ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston, mockPiston2);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3f);

                test.RunOnce();

                Assert.AreEqual("My Value: 8", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AvgOfEmptyListReturnsZero() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the avg ""test piston"" height")) {

                test.RunOnce();

                Assert.AreEqual("My Value: 0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AvgOfOneItemReturnsValue() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the avg ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);

                test.RunOnce();

                Assert.AreEqual("My Value: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AvgOfMultipleItemsReturnsAvg() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the avg ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston, mockPiston2);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3f);

                test.RunOnce();

                Assert.AreEqual("My Value: 4", test.Logger[0]);
            }
        }

        [TestMethod]
        public void MinOfEmptyListReturnsZero() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the min ""test piston"" height")) {

                test.RunOnce();

                Assert.AreEqual("My Value: 0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void MinOfOneItemReturnsValue() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the min ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);

                test.RunOnce();

                Assert.AreEqual("My Value: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void MinOfMultipleItemsReturnsMin() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the min ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston, mockPiston2);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3f);

                test.RunOnce();

                Assert.AreEqual("My Value: 3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void MaxOfEmptyListReturnsZero() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the max ""test piston"" height")) {

                test.RunOnce();

                Assert.AreEqual("My Value: 0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void MaxOfOneItemReturnsValue() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the max ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);

                test.RunOnce();

                Assert.AreEqual("My Value: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void MaxOfMultipleItemsReturnsMax() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the max ""test piston"" height")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston, mockPiston2);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3f);

                test.RunOnce();

                Assert.AreEqual("My Value: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListOfEmptySelectorListReturnsZero() {
            using (var test = new ScriptTest(@"Print ""My Values: "" + the list of ""test piston"" heights")) {

                test.RunOnce();

                Assert.AreEqual("My Values: []", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListOfOneItemReturnsListOfOneItem() {
            using (var test = new ScriptTest(@"Print ""My Values: "" + the list of ""test piston"" heights")) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);

                test.RunOnce();

                Assert.AreEqual("My Values: [5]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListOfMultipleItemsReturnsListWithAllItems() {
            using (var test = new ScriptTest(@"Print ""My Values: "" + the list of ""test piston"" heights")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockPiston, mockPiston2, mockPiston3);
                mockPiston.Setup(b => b.CurrentPosition).Returns(5f);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(3f);
                mockPiston3.Setup(b => b.CurrentPosition).Returns(5f);

                test.RunOnce();

                Assert.AreEqual("My Values: [5,3,5]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SumOfConditionalSelector() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the total height of ""test pistons"" that are on")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston, mockPiston2, mockPiston3);
                mockPiston.Setup(b => b.CurrentPosition).Returns(3f);
                mockPiston.Setup(b => b.Enabled).Returns(false);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(4f);
                mockPiston2.Setup(b => b.Enabled).Returns(true);
                mockPiston3.Setup(b => b.CurrentPosition).Returns(5f);
                mockPiston3.Setup(b => b.Enabled).Returns(true);

                test.RunOnce();

                Assert.AreEqual("My Value: 9", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SumOfAndConditionalSelector() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the total height of ""test pistons"" that are on and whose height > 4")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston, mockPiston2, mockPiston3);
                mockPiston.Setup(b => b.CurrentPosition).Returns(3f);
                mockPiston.Setup(b => b.Enabled).Returns(false);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(4f);
                mockPiston2.Setup(b => b.Enabled).Returns(true);
                mockPiston3.Setup(b => b.CurrentPosition).Returns(5f);
                mockPiston3.Setup(b => b.Enabled).Returns(true);

                test.RunOnce();

                Assert.AreEqual("My Value: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SumOfOrConditionalSelector() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the total height of ""test pistons"" that are on or whose height > 3")) {
                var mockPiston = new Mock<IMyPistonBase>();
                var mockPiston2 = new Mock<IMyPistonBase>();
                var mockPiston3 = new Mock<IMyPistonBase>();
                test.MockBlocksInGroup("test pistons", mockPiston, mockPiston2, mockPiston3);
                mockPiston.Setup(b => b.CurrentPosition).Returns(3f);
                mockPiston.Setup(b => b.Enabled).Returns(false);
                mockPiston2.Setup(b => b.CurrentPosition).Returns(4f);
                mockPiston2.Setup(b => b.Enabled).Returns(true);
                mockPiston3.Setup(b => b.CurrentPosition).Returns(5f);
                mockPiston3.Setup(b => b.Enabled).Returns(true);

                test.RunOnce();

                Assert.AreEqual("My Value: 9", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CheckBlockProperty() {
            using (ScriptTest test = new ScriptTest(@"
if the ""test thruster"" output > 10000
  print ""Full Power!""
            ")) {
                var mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                mockThruster.Setup(b => b.CurrentThrust).Returns(20000);

                test.RunUntilDone();

                Assert.AreEqual("Full Power!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CheckBlockPropertyWithPropertyInFront() {
            using (ScriptTest test = new ScriptTest(@"
if the range of the ""test beacon"" > 5000
  print ""Long Range Beacon!""
            ")) {
                var mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("test beacon", mockBeacon);

                mockBeacon.Setup(b => b.Radius).Returns(10000);

                test.RunUntilDone();

                Assert.AreEqual("Long Range Beacon!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ImplicitBlockPropertyAggregation() {
            using (ScriptTest test = new ScriptTest(@"
if any ""test thrusters"" output < 10000
  print ""Losing Thrust!""
            ")) {
                var mockThruster = new Mock<IMyThrust>();
                var mockThruster2 = new Mock<IMyThrust>();
                test.MockBlocksInGroup("test thrusters", mockThruster, mockThruster2);

                mockThruster.Setup(b => b.CurrentThrust).Returns(20000);
                mockThruster2.Setup(b => b.CurrentThrust).Returns(5000);

                test.RunUntilDone();

                Assert.AreEqual("Losing Thrust!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ImplicitBlockPropertyAggregationWithoutProperty() {
            using (ScriptTest test = new ScriptTest(@"
if the ""test thrusters"" are on
  print ""Thrusters On!""
            ")) {
                var mockThruster = new Mock<IMyThrust>();
                var mockThruster2 = new Mock<IMyThrust>();
                test.MockBlocksInGroup("test thrusters", mockThruster, mockThruster2);

                mockThruster.Setup(b => b.Enabled).Returns(true);
                mockThruster2.Setup(b => b.Enabled).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Thrusters On!", test.Logger[0]);
            }
        }
    }
}
