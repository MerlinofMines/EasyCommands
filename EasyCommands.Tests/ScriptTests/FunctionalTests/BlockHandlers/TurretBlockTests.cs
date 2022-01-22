using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class TurretBlockTests {
        [TestMethod]
        public void TurnOnTheTurrets() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""turrets""")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void FireTheTurrets() {
            String script = @"
tell the ""turrets"" to shoot
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                var turretFire = MockAction(mockTurret, "Shoot_On");

                test.RunUntilDone();

                turretFire.Verify(b => b.Apply(mockTurret.Object));
            }
        }

        [TestMethod]
        public void TurretsAreFiring() {
            String script = @"
if the ""turrets"" are shooting
  print ""Get Some!""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.IsShooting).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Get Some!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTurretTargetFromCustomData() {
            String script = @"
print the ""turrets"" target
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.CustomData).Returns("target=1:2:3");

                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTurretTargetFromTargetedEntity() {
            String script = @"
print the ""turrets"" target
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.CustomData).Returns("");
                mockTurret.Setup(b => b.HasTarget).Returns(true);
                mockTurret.Setup(b => b.GetTargetedEntity()).Returns(MockDetectedEntity(new Vector3D(1, 2, 3)));

                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTurretNoTargetReturnsZeroVector() {
            String script = @"
print the ""turrets"" target
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.CustomData).Returns("");
                mockTurret.Setup(b => b.HasTarget).Returns(false);

                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("0:0:0", test.Logger[0]);
            }
        }


        [TestMethod]
        public void SetTurretTarget() {
            String script = @"
set the ""turrets"" target to ""1:2:3""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.CustomData).Returns("");

                test.RunUntilDone();

                mockTurret.Verify(b => b.SetTarget(new Vector3D(1, 2, 3)));
                mockTurret.VerifySet(b => b.CustomData = "target=1:2:3");
            }
        }

        [TestMethod]
        public void SetTurretTargetVelocityUsingCustomTarget() {
            String script = @"
set the ""turrets"" targetVelocity to ""1:2:3""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.CustomData).Returns("target=4:5:6");
                test.RunUntilDone();

                mockTurret.Verify(b => b.TrackTarget(new Vector3D(4, 5, 6), new Vector3D(1, 2, 3)));
            }
        }

        [TestMethod]
        public void SetTurretTargetVelocityUsingTargetedEntity() {
            String script = @"
set the ""turrets"" targetVelocity to ""1:2:3""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                var target = MockDetectedEntity(new Vector3D(4,5,6));
                mockTurret.Setup(b => b.CustomData).Returns("");
                mockTurret.Setup(b => b.HasTarget).Returns(true);
                mockTurret.Setup(b => b.GetTargetedEntity()).Returns(target);
                test.RunUntilDone();

                mockTurret.Verify(b => b.GetTargetedEntity());
                mockTurret.Verify(b => b.TrackTarget(new Vector3D(4,5,6), new Vector3D(1, 2, 3)));
            }
        }

        [TestMethod]
        public void TurretIsLockedOnTarget() {
            String script = @"
if any ""turrets"" is locked
  Print ""Locked On Target""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.IsAimed).Returns(true);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Locked On Target", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ResetTurretTarget() {
            String script = @"
unlock the ""turrets""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.CustomData).Returns("");
                mockTurret.Setup(b => b.EnableIdleRotation).Returns(true);
                test.RunUntilDone();

                mockTurret.Verify(b => b.ResetTargetingToDefault());
                mockTurret.VerifySet(b => b.EnableIdleRotation = true);
            }
        }

        [TestMethod]
        public void DisableTurretsIdleRotation() {
            String script = @"
set the ""turrets"" auto to false
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.EnableIdleRotation = false);
            }
        }
    }
}
