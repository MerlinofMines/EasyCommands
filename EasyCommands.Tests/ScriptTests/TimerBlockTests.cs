using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class TimerBlockTests {

        [TestMethod]
        public void triggerTheTimer() {
            String script = @"
trigger the ""timer""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTimer = new Mock<IMyTimerBlock>();
                test.MockBlocksOfType("timer", mockTimer);

                test.RunUntilDone();

                mockTimer.Verify(b => b.Trigger());
            }
        }

        [TestMethod]
        public void timerIsTriggered() {
            String script = @"
if the ""timer"" is triggered
  Print ""Tick tock""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTimer = new Mock<IMyTimerBlock>();
                test.MockBlocksOfType("timer", mockTimer);

                mockTimer.Setup(b => b.IsCountingDown).Returns(true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Tick tock"));
            }
        }

        [TestMethod]
        public void getTimerDelay() {
            String script = @"
Print ""Timer Block Delay: "" + the ""timer"" delay
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTimer = new Mock<IMyTimerBlock>();
                test.MockBlocksOfType("timer", mockTimer);

                mockTimer.Setup(b => b.TriggerDelay).Returns(10);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Timer Block Delay: 10"));
            }
        }

        [TestMethod]
        public void setTimerDelay() {
            String script = @"
set the ""timer"" delay to 10
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTimer = new Mock<IMyTimerBlock>();
                test.MockBlocksOfType("timer", mockTimer);

                test.RunUntilDone();

                mockTimer.VerifySet(b => b.TriggerDelay = 10);
            }
        }

        [TestMethod]
        public void isTimerSilent() {
            String script = @"
if the ""timer"" is silent
  Print ""Be very, very quiet""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTimer = new Mock<IMyTimerBlock>();
                test.MockBlocksOfType("timer", mockTimer);

                mockTimer.Setup(b => b.Silent).Returns(true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Be very, very quiet"));
            }
        }

        [TestMethod]
        public void silenceTheTimer() {
            String script = @"
silence the ""timer""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTimer = new Mock<IMyTimerBlock>();
                test.MockBlocksOfType("timer", mockTimer);

                test.RunUntilDone();

                mockTimer.VerifySet(b => b.Silent = true);
            }
        }

        [TestMethod]
        public void startTheTimerCountdown() {
            String script = @"
start the ""timer"" countdown
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTimer = new Mock<IMyTimerBlock>();
                test.MockBlocksOfType("timer", mockTimer);

                test.RunUntilDone();

                mockTimer.Verify(b => b.StartCountdown());
            }
        }

        [TestMethod]
        public void stopTheTimerCountdown() {
            String script = @"
stop the ""timer"" countdown
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTimer = new Mock<IMyTimerBlock>();
                test.MockBlocksOfType("timer", mockTimer);

                test.RunUntilDone();

                mockTimer.Verify(b => b.StopCountdown());
            }
        }

        [TestMethod]
        public void isTheTimerCountdownStarted() {
            String script = @"
if the ""timer"" countdown is started
  Print ""Tick tock""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTimer = new Mock<IMyTimerBlock>();
                test.MockBlocksOfType("timer", mockTimer);

                mockTimer.Setup(b => b.IsCountingDown).Returns(true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Tick tock"));
            }
        }
    }
}
