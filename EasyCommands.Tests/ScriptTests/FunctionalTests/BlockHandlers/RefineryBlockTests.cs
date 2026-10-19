using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class RefineryBlockTests {
        [DataTestMethod]
        [DataRow(false, false, false)]
        [DataRow(false, true, false)]
        [DataRow(true, false, false)]
        [DataRow(true, true, true)]
        public void RunningRequiresWorkingProduction(bool working, bool producing, bool expected) {
            using (var test = new ScriptTest(@"print ""Ore"" refinery is running")) {
                var refinery = new Mock<IMyRefinery>();
                refinery.Setup(b => b.IsWorking).Returns(working);
                refinery.Setup(b => b.IsProducing).Returns(producing);
                test.MockBlocksOfType("Ore", refinery);
                test.RunUntilDone();
                CollectionAssert.AreEqual(new[] { expected.ToString() }, test.Logger);
            }
        }

        [TestMethod]
        public void TurnOnTheRefineries() {
            String script = @"turn on the refineries";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRefinery = new Mock<IMyRefinery>();
                test.MockBlocksOfType("test refinery", mockRefinery);

                test.RunUntilDone();

                mockRefinery.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheRefineries() {
            String script = @"turn off the refineries";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRefinery = new Mock<IMyRefinery>();
                test.MockBlocksOfType("test refinery", mockRefinery);

                test.RunUntilDone();

                mockRefinery.VerifySet(b => b.Enabled = false);
            }
        }
    }
}
