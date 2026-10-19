using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components.Interfaces;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class AiBlockTests {
        [TestMethod]
        public void FlightAltitudeAndGravityAlignmentUseExistingVocabulary() {
            using (var test = new ScriptTest(@"set ""Drone"" ai flight altitude to 150
set ""Drone"" ai flight gravity alignment to true
print ""Drone"" ai flight altitude
print ""Drone"" ai flight gravity alignment
set ""Drone"" ai flight gravity alignment to false")) {
                var block = new Mock<IMyFlightMovementBlock>();
                block.SetupAllProperties();
                var altitude = MockProperty<IMyFlightMovementBlock, float>(block, "MinHeightAboveTerrain");
                altitude.Setup(p => p.GetValue(block.Object)).Returns(150);
                test.MockBlocksOfType("Drone", block);
                test.RunUntilDone();
                altitude.Verify(p => p.SetValue(block.Object, 150));
                block.VerifyGet(b => b.MinimalAltitude, Times.Never());
                block.VerifySet(b => b.AlignToPGravity = true);
                block.VerifySet(b => b.AlignToPGravity = false);
                CollectionAssert.AreEqual(new[] { "150", "True" }, test.Logger);
            }
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void CombatTargetsReportPresenceWithoutExposingIds(bool present) {
            using (var test = new ScriptTest("print \"Defense\" ai defensive target\nprint \"Offense\" ai offensive target\nset \"Defense\" ai defensive target to false\nset \"Offense\" ai offensive target to false")) {
                var search = new Mock<IMySearchEnemyComponent>();
                search.Setup(s => s.FoundEnemyId).Returns(present ? (long?)9007199254740993L : null);
                var defense = new Mock<IMyDefensiveCombatBlock>();
                var offense = new Mock<IMyOffensiveCombatBlock>();
                defense.Setup(b => b.SearchEnemyComponent).Returns(search.Object);
                offense.Setup(b => b.SearchEnemyComponent).Returns(search.Object);
                test.MockBlocksOfType("Defense", defense);
                test.MockBlocksOfType("Offense", offense);
                test.RunUntilDone();
                CollectionAssert.AreEqual(new[] { present.ToString(), present.ToString() }, test.Logger);
            }
        }

        [TestMethod]
        public void RetreatPositionCanOverrideAndRestoreTerminalDestination() {
            using (var test = new ScriptTest(@"print ""Drone"" ai defensive target position
set ""Drone"" ai defensive target position to 1:2:3
print ""Drone"" ai defensive target override
print ""Drone"" ai defensive target position
set ""Drone"" ai defensive target override to false
print ""Drone"" ai defensive target override
print ""Drone"" ai defensive target position")) {
                var block = new Mock<IMyDefensiveCombatBlock>();
                block.SetupAllProperties();
                block.Setup(b => b.FleeCoordinates).Returns(() => block.Object.CustomFleeCoordinates ?? new Vector3D(4, 5, 6));
                test.MockBlocksOfType("Drone", block);
                test.RunUntilDone();
                block.VerifySet(b => b.CustomFleeCoordinates = new Vector3D(1, 2, 3));
                Assert.IsNull(block.Object.CustomFleeCoordinates);
                CollectionAssert.AreEqual(new[] { "4:5:6", "True", "1:2:3", "False", "4:5:6" }, test.Logger);
            }
        }

        [TestMethod]
        public void RecorderRepeatAliasesDoNotChangeBehaviorOrPlayback() {
            using (var test = new ScriptTest(@"set ""Route"" ai recorder repeat path to true
print ""Route"" ai recorder loop path
set ""Route"" ai recorder loop path to false
print ""Route"" ai recorder repeat path")) {
                var block = new Mock<IMyPathRecorderBlock>();
                var recorder = new Mock<IMyPathRecorderComponent>();
                recorder.SetupAllProperties();
                var components = new Mock<IMyEntityComponentContainer>();
                var component = recorder.Object;
                components.Setup(c => c.TryGet(out component)).Returns(true);
                block.Setup(b => b.Components).Returns(components.Object);
                test.MockBlocksOfType("Route", block);
                test.RunUntilDone();
                CollectionAssert.AreEqual(new[] { "True", "False" }, test.Logger, string.Join("\n", test.Logger));
                recorder.Verify(r => r.Play(), Times.Never());
                block.Verify(b => b.GetProperty("ActivateBehavior"), Times.Never());
            }
        }

        void CheckBehavior<T>(string selector) where T : class, IMyFunctionalBlock {
            using (var test = new ScriptTest("set \"Drone\" " + selector + " behavior to true\nprint \"Drone\" " + selector + " behavior\nset \"Drone\" " + selector + " behavior to false\nset \"Drone\" " + selector + " power to true")) {
                var block = new Mock<T>();
                var behavior = MockProperty<T, bool>(block, "ActivateBehavior");
                behavior.Setup(p => p.GetValue(block.Object)).Returns(true);
                test.MockBlocksOfType("Drone", block);
                test.RunUntilDone();
                behavior.Verify(p => p.SetValue(block.Object, true), Times.Once());
                behavior.Verify(p => p.SetValue(block.Object, false), Times.Once());
                block.VerifySet(b => b.Enabled = true, Times.Once());
                CollectionAssert.AreEqual(new[] { "True" }, test.Logger);
            }
        }

        [TestMethod]
        public void AllAiSelectorsSeparateBehaviorFromPower() {
            CheckBehavior<IMyFlightMovementBlock>("ai flight");
            CheckBehavior<IMyBasicMissionBlock>("ai basic");
            CheckBehavior<IMyPathRecorderBlock>("ai recorder");
            CheckBehavior<IMyDefensiveCombatBlock>("ai defensive");
            CheckBehavior<IMyOffensiveCombatBlock>("ai offensive");
        }

        [TestMethod]
        public void FlightSettingsAndReadback() {
            using (var test = new ScriptTest(@"set ""Drone"" ai flight speed to 25
set ""Drone"" ai flight precision to true
set ""Drone"" ai flight avoidance to false
print ""Drone"" ai flight speed
print ""Drone"" ai flight precision
print ""Drone"" ai flight avoidance")) {
                var block = new Mock<IMyFlightMovementBlock>();
                block.SetupAllProperties();
                test.MockBlocksOfType("Drone", block);
                test.RunUntilDone();
                Assert.AreEqual(25f, block.Object.SpeedLimit);
                Assert.IsTrue(block.Object.PrecisionMode);
                Assert.IsFalse(block.Object.CollisionAvoidance);
                block.VerifySet(b => b.CollisionAvoidance = false, Times.Once());
                CollectionAssert.AreEqual(new[] { "25", "True", "False" }, test.Logger);
            }
        }

        [TestMethod]
        public void RecorderStartsStopsAndReportsPlayback() {
            using (var test = new ScriptTest(@"set ""Route"" ai recorder play to true
print ""Route"" ai recorder playing
set ""Route"" ai recorder play to false")) {
                var block = new Mock<IMyPathRecorderBlock>();
                var recorder = new Mock<IMyPathRecorderComponent>();
                recorder.Setup(r => r.IsPlaying).Returns(true);
                var components = new Mock<IMyEntityComponentContainer>();
                var component = recorder.Object;
                components.Setup(c => c.TryGet(out component)).Returns(true);
                block.Setup(b => b.Components).Returns(components.Object);
                test.MockBlocksOfType("Route", block);
                test.RunUntilDone();
                recorder.Verify(r => r.Play(), Times.Once());
                recorder.Verify(r => r.StopPlay(), Times.Once());
                CollectionAssert.AreEqual(new[] { "True" }, test.Logger);
            }
        }

        [TestMethod]
        public void FleeStartsOnlyForTrueAndReportsState() {
            using (var test = new ScriptTest(@"set ""Drone"" ai defensive flee to true
print ""Drone"" ai defensive fleeing
set ""Drone"" ai defensive flee to false")) {
                var block = new Mock<IMyDefensiveCombatBlock>();
                block.Setup(b => b.IsFleeing).Returns(true);
                test.MockBlocksOfType("Drone", block);
                test.RunUntilDone();
                block.Verify(b => b.Flee(), Times.Once());
                CollectionAssert.AreEqual(new[] { "True" }, test.Logger);
            }
        }
    }
}
