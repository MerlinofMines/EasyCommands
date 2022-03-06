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
        public void FireTheGuns() {
            using (ScriptTest test = new ScriptTest(@"tell the ""guns"" to fire")) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksInGroup("guns", mockGun);

                test.RunUntilDone();

                mockGun.VerifySet(b => b.Shoot = true);
            }
        }

        [TestMethod]
        public void StopFiringTheGuns() {
            using (ScriptTest test = new ScriptTest(@"stop firing the ""guns""")) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksInGroup("guns", mockGun);

                test.RunUntilDone();

                mockGun.VerifySet(b => b.Shoot = false);
            }
        }

        [TestMethod]
        public void FireTheRailGuns() {
            using (ScriptTest test = new ScriptTest(@"tell the ""railguns"" to fire")) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksInGroup("railguns", mockGun);

                test.RunUntilDone();

                mockGun.VerifySet(b => b.Shoot = true);
            }
        }

        [TestMethod]
        public void FireTheCannons() {
            using (ScriptTest test = new ScriptTest(@"tell the ""cannons"" to fire")) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksInGroup("cannons", mockGun);

                test.RunUntilDone();

                mockGun.VerifySet(b => b.Shoot = true);
            }
        }

        [TestMethod]
        public void FireTheAutoCannons() {
            using (ScriptTest test = new ScriptTest(@"tell the ""autocannons"" to fire")) {
                var mockGun = new Mock<IMyUserControllableGun>();
                test.MockBlocksInGroup("autocannons", mockGun);

                test.RunUntilDone();

                mockGun.VerifySet(b => b.Shoot = true);
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
