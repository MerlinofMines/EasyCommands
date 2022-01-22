using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class WarheadBlockTests {
        [TestMethod]
        public void TurnOffTheWarheads() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""warheads""")) {
                var mockWarheads = new Mock<IMyWarhead>();
                test.MockBlocksInGroup("warheads", mockWarheads);

                test.RunUntilDone();

                mockWarheads.VerifySet(b => b.IsArmed = true);
            }
        }

        [TestMethod]
        public void detonateTheBomb() {
            String script = @"
detonate the ""warheads""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksInGroup("warheads", mockWarhead);

                test.RunUntilDone();

                mockWarhead.Verify(b => b.Detonate());
            }
        }

        [TestMethod]
        public void bombIsTriggered() {
            String script = @"
if the ""warhead"" is triggered
  Print ""Its gonna blow!""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                mockWarhead.Setup(b => b.IsCountingDown).Returns(true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Its gonna blow!"));
            }
        }

        [TestMethod]
        public void getBombDelay() {
            String script = @"
Print ""Warhead Timer: "" + the ""warhead"" delay
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                mockWarhead.Setup(b => b.DetonationTime).Returns(10);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Warhead Timer: 10"));
            }
        }

        [TestMethod]
        public void setBombDelay() {
            String script = @"
set the ""warhead"" delay to 10
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                test.RunUntilDone();

                mockWarhead.VerifySet(b => b.DetonationTime = 10);
            }
        }

        [TestMethod]
        public void isBombArmed() {
            String script = @"
if the ""warhead"" is armed
  Print ""Dont cut the wrong wire""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                mockWarhead.Setup(b => b.IsArmed).Returns(true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Dont cut the wrong wire"));
            }
        }

        [TestMethod]
        public void isBombDisarmed() {
            String script = @"
if the ""warhead"" is disarmed
  Print ""Bomb is safe""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                mockWarhead.Setup(b => b.IsArmed).Returns(false);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Bomb is safe"));
            }
        }

        [TestMethod]
        public void armTheBomb() {
            String script = @"
arm the ""warhead""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                test.RunUntilDone();

                mockWarhead.VerifySet(b => b.IsArmed = true);
            }
        }

        [TestMethod]
        public void disarmTheBomb() {
            String script = @"
disarm the ""warhead""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                test.RunUntilDone();

                mockWarhead.VerifySet(b => b.IsArmed = false);
            }
        }

        [TestMethod]
        public void startTheBombCountdown() {
            String script = @"
start the ""warhead"" countdown
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                test.RunUntilDone();

                mockWarhead.Verify(b => b.StartCountdown());
            }
        }

        [TestMethod]
        public void stopTheBombCountdown() {
            String script = @"
stop the ""warhead"" countdown
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                test.RunUntilDone();

                mockWarhead.Verify(b => b.StopCountdown());
            }
        }

        [TestMethod]
        public void isTheBombCountdownStarted() {
            String script = @"
if the ""warhead"" countdown is started
  Print ""Evacuate!""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockWarhead = new Mock<IMyWarhead>();
                test.MockBlocksOfType("warhead", mockWarhead);

                mockWarhead.Setup(b => b.IsCountingDown).Returns(true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Evacuate!"));
            }
        }
    }
}
