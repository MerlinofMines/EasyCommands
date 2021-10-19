using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleAggregationTests {

        [TestMethod]
        public void ValueOfEmptyListReturnsZero() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + ""test piston"" height")) {

                test.RunOnce();

                Assert.AreEqual("My Value: 0", test.Logger[0]);
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
        public void ValueOfMultipleItemsReturnsSum() {
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
    }
}
