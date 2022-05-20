using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class MultiWordPropertyAggregationTests {
        [TestMethod]
        public void GetPropertyValue() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the ""Test Wheel"" steering override")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("Test Wheel", mockWheel);
                mockWheel.Setup(b => b.SteeringOverride).Returns(0.1f);

                test.RunOnce();

                Assert.AreEqual("My Value: 0.1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetListOfPropertyValues() {
            using (var test = new ScriptTest(@"Print ""My Values: "" + the list of ""Test Wheels"" steering overrides")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                var mockWheel2 = new Mock<IMyMotorSuspension>();
                test.MockBlocksInGroup("Test Wheels", mockWheel, mockWheel2);
                mockWheel.Setup(b => b.SteeringOverride).Returns(0.1f);
                mockWheel2.Setup(b => b.SteeringOverride).Returns(0.2f);

                test.RunOnce();

                Assert.AreEqual("My Values: [0.1,0.2]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetPropertyValuesBeforeSelector() {
            using (var test = new ScriptTest(@"Print ""My Values: "" + the list of steering overrides of the ""Test Wheels""")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                var mockWheel2 = new Mock<IMyMotorSuspension>();
                test.MockBlocksInGroup("Test Wheels", mockWheel, mockWheel2);
                mockWheel.Setup(b => b.SteeringOverride).Returns(0.1f);
                mockWheel2.Setup(b => b.SteeringOverride).Returns(0.2f);

                test.RunOnce();

                Assert.AreEqual("My Values: [0.1,0.2]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetValueofDynamicProperty() {
            using (var test = new ScriptTest(@"Print ""My Values: "" + the list of ""Test Wheels"" ""steering override"" property")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                var mockWheel2 = new Mock<IMyMotorSuspension>();
                test.MockBlocksInGroup("Test Wheels", mockWheel, mockWheel2);
                mockWheel.Setup(b => b.SteeringOverride).Returns(0.1f);
                mockWheel2.Setup(b => b.SteeringOverride).Returns(0.2f);

                test.RunOnce();

                Assert.AreEqual("My Values: [0.1,0.2]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SumOfConditionalSelectorWithMultiProperty() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + the count of ""test wheels"" whose steering limit > 0.5")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                var mockWheel2 = new Mock<IMyMotorSuspension>();
                var mockWheel3 = new Mock<IMyMotorSuspension>();
                test.MockBlocksInGroup("test wheels", mockWheel, mockWheel2, mockWheel3);
                mockWheel.Setup(b => b.MaxSteerAngle).Returns(0.75f);
                mockWheel2.Setup(b => b.MaxSteerAngle).Returns(0.6f);
                mockWheel3.Setup(b => b.MaxSteerAngle).Returns(0.4f);

                test.RunOnce();

                Assert.AreEqual("My Value: 2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CheckMultiWordBlockProperty() {
            using (ScriptTest test = new ScriptTest(@"
if the ""test turret"" target velocity > 50
  print ""Target is getting away!""
            ")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);

                mockTurret.Setup(b => b.CustomData).Returns("");
                mockTurret.Setup(b => b.HasTarget).Returns(true);
                mockTurret.Setup(b => b.GetTargetedEntity()).Returns(MockDetectedEntity(new Vector3D(1, 2, 3), new Vector3D(30, 30, 30)));

                test.RunUntilDone();

                Assert.AreEqual("Target is getting away!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CheckAnyMultiWordBlockProperty() {
            using (ScriptTest test = new ScriptTest(@"
if any ""test turrets"" target velocity > 50
  print ""Some are retreating!""
            ")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                var mockTurret2 = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("test turrets", mockTurret, mockTurret2);

                mockTurret.Setup(b => b.CustomData).Returns("");
                mockTurret.Setup(b => b.HasTarget).Returns(true);
                mockTurret.Setup(b => b.GetTargetedEntity()).Returns(MockDetectedEntity(new Vector3D(1, 2, 3), new Vector3D(30, 30, 30)));

                mockTurret2.Setup(b => b.CustomData).Returns("");
                mockTurret2.Setup(b => b.HasTarget).Returns(true);
                mockTurret2.Setup(b => b.GetTargetedEntity()).Returns(MockDetectedEntity(new Vector3D(1, 2, 3)));

                test.RunUntilDone();

                Assert.AreEqual("Some are retreating!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AggregatePropertyUsedInComparison() {
            using (ScriptTest test = new ScriptTest(@"
if the max ""test wheels"" steering limit < 0.5
  print ""Takes forever to turn""
            ")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                var mockWheel2 = new Mock<IMyMotorSuspension>();
                test.MockBlocksInGroup("test wheels", mockWheel, mockWheel2);

                mockWheel.Setup(b => b.MaxSteerAngle).Returns(0.25f);
                mockWheel2.Setup(b => b.MaxSteerAngle).Returns(0.25f);

                test.RunUntilDone();

                Assert.AreEqual("Takes forever to turn", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CheckPropertyWithPropertyWordsSplitAcrossComparison() {
            using (ScriptTest test = new ScriptTest(@"
if the ""test wheel"" steering is inverted
  print ""Steering is inverted!""
")) {
                var mockWheel = new Mock<IMyMotorSuspension>();
                test.MockBlocksOfType("test wheel", mockWheel);

                mockWheel.Setup(b => b.InvertSteer).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Steering is inverted!", test.Logger[0]);
            }
        }
    }
}
