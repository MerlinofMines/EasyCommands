using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components.Interfaces;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class AiBlockTests {
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
