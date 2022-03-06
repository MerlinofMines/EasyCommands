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
            using (ScriptTest test = new ScriptTest(@"tell the ""turrets"" to shoot")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);


                test.RunUntilDone();

                mockTurret.VerifySet(b => b.Shoot = true);
            }
        }

        [TestMethod]
        public void StopFiringTheTurrets() {
            using (ScriptTest test = new ScriptTest((string)@"stop firing the ""turrets""")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.Shoot = false);
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
            using (ScriptTest test = new ScriptTest(@"set the ""turrets"" target to 1:2:3")) {
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
if any ""turrets"" target is true
  Print ""Locked On Target""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.HasTarget).Returns(true);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Locked On Target", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ResetTurretTarget() {
            String script = @"turn off the the ""turrets"" target";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                mockTurret.Setup(b => b.CustomData).Returns("");
                mockTurret.Setup(b => b.EnableIdleRotation).Returns(true);
                test.RunUntilDone();

                mockTurret.Verify(b => b.ResetTargetingToDefault());
                mockTurret.VerifySet(b => b.EnableIdleRotation = true);
                mockTurret.Verify(b => b.SyncEnableIdleRotation());
            }
        }

        [TestMethod]
        public void IsTheTurretTargetingWeapons() {
            using (ScriptTest test = new ScriptTest(@"Print ""Targeting Weapons: "" + the ""test turret"" is targeting ""Weapons""")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);
                mockTurret.Setup(b => b.GetTargetingGroup()).Returns("Weapons");

                test.RunUntilDone();

                Assert.AreEqual("Targeting Weapons: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TellTheTurretToTargetWeapons() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test turret"" to target ""Weapons""")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);

                test.RunUntilDone();

                mockTurret.Verify(b => b.SetTargetingGroup("Weapons"));
            }
        }

        [TestMethod]
        public void GetTheTurretRange() {
            using (ScriptTest test = new ScriptTest(@"Print ""Range: "" + the ""turrets"" range")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);
                mockTurret.Setup(b => b.Range).Returns(500f);

                test.RunUntilDone();

                Assert.AreEqual("Range: 500", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretRange() {
            using (ScriptTest test = new ScriptTest(@"set the ""turrets"" range to 300")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksInGroup("turrets", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.Range = 300);
            }
        }

        [TestMethod]
        public void GetTheTurretLocking() {
            using (ScriptTest test = new ScriptTest(@"Print ""Target Locking: "" + the ""test turret"" locking")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);
                MockGetProperty(mockTurret, "EnableTargetLocking", true);

                test.RunUntilDone();

                Assert.AreEqual("Target Locking: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TurnOnTheTurretTargetLocking() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test turret"" locking")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);
                var mockTargetLocking = MockProperty<IMyLargeTurretBase, bool>(mockTurret, "EnableTargetLocking");

                test.RunUntilDone();

                mockTargetLocking.Verify(b => b.SetValue(mockTurret.Object, true));
            }
        }

        [TestMethod]
        public void GetTheTurretRotation() {
            using (ScriptTest test = new ScriptTest(@"Print ""Idle Rotation: "" + the ""test turret"" rotation is on")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);
                mockTurret.Setup(b => b.EnableIdleRotation).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Idle Rotation: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TurnOnTheTurretRotation() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test turret"" rotation")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.EnableIdleRotation = true);
                mockTurret.Verify(b => b.SyncEnableIdleRotation());
            }
        }

        [TestMethod]
        public void GetTheTurretIsInUse() {
            using (ScriptTest test = new ScriptTest(@"Print ""Under Control: "" + the ""test turret"" is in use")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);
                mockTurret.Setup(b => b.IsUnderControl).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Under Control: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheTurretAzimuth() {
            using (ScriptTest test = new ScriptTest(@"Print ""Turret Azimuth: "" + the ""test turret"" azimuth")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);
                mockTurret.Setup(b => b.Azimuth).Returns((float)Math.PI/2);

                test.RunUntilDone();

                Assert.AreEqual("Turret Azimuth: 90", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretAzimuth() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turret"" azimuth to 90")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.Azimuth = (float)Math.PI/2);
                mockTurret.Verify(b => b.SyncAzimuth());
            }
        }

        [TestMethod]
        public void GetTheTurretElevation() {
            using (ScriptTest test = new ScriptTest(@"Print ""Turret Elevation: "" + the ""test turret"" elevation")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);
                mockTurret.Setup(b => b.Elevation).Returns((float)Math.PI / 2);

                test.RunUntilDone();

                Assert.AreEqual("Turret Elevation: 90", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretElevation() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turret"" elevation to 90")) {
                var mockTurret = new Mock<IMyLargeTurretBase>();
                test.MockBlocksOfType("test turret", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.Elevation = (float)Math.PI / 2);
                mockTurret.Verify(b => b.SyncElevation());
            }
        }
    }
}
