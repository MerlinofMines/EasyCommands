using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class FunctionalBlockTests {
        [TestMethod]
        public void TurnOnTheWelder() {
            String script = @"turn on the ""test welder""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyShipWelder>();
                test.MockBlocksOfType("test welder", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheWelder() {
            String script = @"turn off the ""test welder""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyShipWelder>();
                test.MockBlocksOfType("test welder", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void EnableTheWelder() {
            String script = @"enable the ""test welder""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyShipWelder>();
                test.MockBlocksOfType("test welder", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void DisableTheWelder() {
            String script = @"disable the ""test welder""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyShipWelder>();
                test.MockBlocksOfType("test welder", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void PowerOnTheWelder() {
            String script = @"power on the ""test welder""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyShipWelder>();
                test.MockBlocksOfType("test welder", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void PowerOffTheWelder() {
            String script = @"power off the ""test welder""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyShipWelder>();
                test.MockBlocksOfType("test welder", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.Enabled = false);
            }
        }
    }
}
