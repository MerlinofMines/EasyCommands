using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class GunBlockTests {
        [TestMethod]
        public void TurnOnTheGuns() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""guns""")) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksInGroup("guns", mockGun);

                test.RunUntilDone();

                mockGun.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void GetTheGunRange() {
            using (ScriptTest test = new ScriptTest(@"Print ""Range: "" + the ""test gun"" range")) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksOfType("test gun", mockGun);
                MockGetProperty(mockGun, "Range", 100f);

                test.RunUntilDone();

                Assert.AreEqual("Range: 100", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheGunRange() {
            using (ScriptTest test = new ScriptTest(@"set the ""test gun"" range to 300")) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksOfType("test gun", mockGun);
                var mockRange = MockProperty<IMyUserControllableGun, float>(mockGun, "Range");

                test.RunUntilDone();

                mockRange.Verify(b => b.SetValue(mockGun.Object, 300));
            }
        }

        [TestMethod]
        public void FireTheGuns() {
            String script = @"
tell the ""guns"" to fire
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksInGroup("guns", mockGun);

                var turretFire = MockAction(mockGun, "Shoot_On");

                test.RunUntilDone();

                turretFire.Verify(b => b.Apply(mockGun.Object));
            }
        }

        [TestMethod]
        public void FireTheRailGuns() {
            String script = @"
tell the ""railguns"" to fire
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksInGroup("railguns", mockGun);

                var turretFire = MockAction(mockGun, "Shoot_On");

                test.RunUntilDone();

                turretFire.Verify(b => b.Apply(mockGun.Object));
            }
        }

        [TestMethod]
        public void GunsAreFiring() {
            String script = @"
if the ""guns"" are firing
  print ""Get Some!""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksInGroup("guns", mockGun);

                mockGun.Setup(b => b.IsShooting).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Get Some!", test.Logger[0]);
            }
        }
    }
}
