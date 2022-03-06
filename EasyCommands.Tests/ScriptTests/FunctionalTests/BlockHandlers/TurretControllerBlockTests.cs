using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Linq;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class TurretControllerBlockTests {
        [TestMethod]
        public void TurnOnTheTurretController() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test turretcontroller""")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void FireTheTurretController() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test turretcontroller"" to shoot")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                var mockGun = new Mock<IMyUserControllableGun>();
                MockTools(mockTurret, mockGun);

                test.RunUntilDone();

                mockGun.VerifySet(b => b.Shoot = true);
            }
        }

        [TestMethod]
        public void StopFiringTheTurretController() {
            using (ScriptTest test = new ScriptTest((string)@"stop firing the ""test turretcontroller""")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                var mockGun = new Mock<IMyUserControllableGun>();
                MockTools(mockTurret, mockGun);

                test.RunUntilDone();

                mockGun.VerifySet(b => b.Shoot = false);
            }
        }

        [TestMethod]
        public void TurretControllerIsFiring() {
            String script = @"
if the ""test turretcontroller"" is shooting
  print ""Get Some!""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                var mockGun = new Mock<IMyUserControllableGun>();
                MockTools(mockTurret, mockGun);

                mockGun.Setup(b => b.IsShooting).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Get Some!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTurretTargetFromTargetedEntity() {
            String script = @"
print the ""test turretcontroller"" target
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

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
print the ""test turretcontroller"" target
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                mockTurret.Setup(b => b.HasTarget).Returns(false);

                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("0:0:0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TurretIsLockedOnTarget() {
            String script = @"
if any ""test turretcontroller"" target is true
  Print ""Locked On Target""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                mockTurret.Setup(b => b.HasTarget).Returns(true);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Locked On Target", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheTurretTargetingWeapons() {
            using (ScriptTest test = new ScriptTest(@"Print ""Targeting Weapons: "" + the ""test turretcontroller"" is targeting ""Weapons""")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                mockTurret.Setup(b => b.GetTargetingGroup()).Returns("Weapons");

                test.RunUntilDone();

                Assert.AreEqual("Targeting Weapons: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TellTheTurretToTargetWeapons() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test turretcontroller"" to target ""Weapons""")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                test.RunUntilDone();

                mockTurret.Verify(b => b.SetTargetingGroup("Weapons"));
            }
        }

        [TestMethod]
        public void GetTheTurretRange() {
            using (ScriptTest test = new ScriptTest(@"Print ""Range: "" + the ""test turretcontroller"" range")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                mockTurret.Setup(b => b.Range).Returns(500f);

                test.RunUntilDone();

                Assert.AreEqual("Range: 500", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretRange() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turretcontroller"" range to 300")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.Range = 300);
            }
        }

        [TestMethod]
        public void GetTheTurretLocking() {
            using (ScriptTest test = new ScriptTest(@"Print ""Target Locking: "" + the ""test turretcontroller"" locking")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                MockGetProperty(mockTurret, "EnableTargetLocking", true);

                test.RunUntilDone();

                Assert.AreEqual("Target Locking: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TurnOnTheTurretTargetLocking() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test turretcontroller"" locking")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                var mockTargetLocking = MockProperty<IMyTurretControlBlock, bool>(mockTurret, "EnableTargetLocking");

                test.RunUntilDone();

                mockTargetLocking.Verify(b => b.SetValue(mockTurret.Object, true));
            }
        }

        [TestMethod]
        public void IsTheTurretAIEnabled() {
            using (ScriptTest test = new ScriptTest(@"Print ""AI Enabled: "" + the ""test turretcontroller"" is on auto")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                MockGetProperty(mockTurret, "AI", true);

                test.RunUntilDone();

                Assert.AreEqual("AI Enabled: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TurnOnTheTurretAI() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turretcontroller"" to auto")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                var mockTargetLocking = MockProperty<IMyTurretControlBlock, bool>(mockTurret, "AI");

                test.RunUntilDone();

                mockTargetLocking.Verify(b => b.SetValue(mockTurret.Object, true));
            }
        }

        [TestMethod]
        public void GetTheTurretIsInUse() {
            using (ScriptTest test = new ScriptTest(@"Print ""Under Control: "" + the ""test turretcontroller"" is in use")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                mockTurret.Setup(b => b.IsUnderControl).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Under Control: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheTurretAngle() {
            using (ScriptTest test = new ScriptTest(@"Print ""Deviation Angle: "" + the ""test turretcontroller"" angle")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                MockGetProperty(mockTurret, "AngleDeviation", 5f);

                test.RunUntilDone();

                Assert.AreEqual("Deviation Angle: 5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretAngle() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turretcontroller"" angle to 5")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                var mockTargetLocking = MockProperty<IMyTurretControlBlock, float>(mockTurret, "AngleDeviation");

                test.RunUntilDone();

                mockTargetLocking.Verify(b => b.SetValue(mockTurret.Object, 5f));
            }
        }

        [TestMethod]
        public void GetTheTurretVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Azimuth Velocity Multiplier: "" + the ""test turretcontroller"" velocity")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                mockTurret.Setup(b => b.VelocityMultiplierAzimuthRpm).Returns(10);

                test.RunUntilDone();

                Assert.AreEqual("Azimuth Velocity Multiplier: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretVelocity() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turretcontroller"" velocity to 10")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.VelocityMultiplierAzimuthRpm = 10);
            }
        }

        [TestMethod]
        public void GetTheTurretRightVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Azimuth Velocity Multiplier: "" + the ""test turretcontroller"" right velocity")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                mockTurret.Setup(b => b.VelocityMultiplierAzimuthRpm).Returns(10);

                test.RunUntilDone();

                Assert.AreEqual("Azimuth Velocity Multiplier: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretRightVelocity() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turretcontroller"" right velocity to 10")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.VelocityMultiplierAzimuthRpm = 10);
            }
        }

        [TestMethod]
        public void GetTheTurretLeftVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Azimuth Velocity Multiplier: "" + the ""test turretcontroller"" left velocity")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                mockTurret.Setup(b => b.VelocityMultiplierAzimuthRpm).Returns(10);

                test.RunUntilDone();

                Assert.AreEqual("Azimuth Velocity Multiplier: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretLeftVelocity() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turretcontroller"" left velocity to 10")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.VelocityMultiplierAzimuthRpm = 10);
            }
        }

        [TestMethod]
        public void GetTheTurretUpwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Elevation Velocity Multiplier: "" + the ""test turretcontroller"" upward velocity")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                mockTurret.Setup(b => b.VelocityMultiplierElevationRpm).Returns(10);

                test.RunUntilDone();

                Assert.AreEqual("Elevation Velocity Multiplier: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretUpwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turretcontroller"" upward velocity to 10")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.VelocityMultiplierElevationRpm = 10);
            }
        }

        [TestMethod]
        public void GetTheTurretDownwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Elevation Velocity Multiplier: "" + the ""test turretcontroller"" downward velocity")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);
                mockTurret.Setup(b => b.VelocityMultiplierElevationRpm).Returns(10);

                test.RunUntilDone();

                Assert.AreEqual("Elevation Velocity Multiplier: 10", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheTurretDownwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"set the ""test turretcontroller"" downward velocity to 10")) {
                var mockTurret = new Mock<IMyTurretControlBlock>();
                test.MockBlocksOfType("test turretcontroller", mockTurret);

                test.RunUntilDone();

                mockTurret.VerifySet(b => b.VelocityMultiplierElevationRpm = 10);
            }
        }

        public void MockTools(Mock<IMyTurretControlBlock> mockTurret, params Mock<IMyUserControllableGun>[] mockGuns) {
            mockTurret.Setup(b => b.GetTools(It.IsAny<List<IMyFunctionalBlock>>()))
                .Callback<List<IMyFunctionalBlock>>(list => {
                    list.Clear();
                    list.AddRange(mockGuns.Select(b => b.Object).Cast<IMyFunctionalBlock>());
                });
        }
    }
}
