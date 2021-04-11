using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class PistonBlockTests {
        [TestMethod]
        public void getUpperLimit() {
            String script = @"
print the avg ""piston"" upper limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);

                mockPiston.Setup(b => b.MaxLimit).Returns(5);
                test.RunUntilDone();
                mockPiston.Verify(b => b.MaxLimit);

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void getLowerLimit() {
            String script = @"
print the avg ""piston"" lower limit
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);

                mockPiston.Setup(b => b.MinLimit).Returns(5);
                test.RunUntilDone();
                mockPiston.Verify(b => b.MinLimit);

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void setUpperLimit() {
            String script = @"
set the ""piston"" upper limit to 5
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MaxLimit = 5);
            }
        }

        [TestMethod]
        public void setLowerLimit() {
            String script = @"
set the ""piston"" lower limit to 5
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockPiston = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("piston", mockPiston);

                test.RunUntilDone();

                mockPiston.VerifySet(b => b.MinLimit = 5);
            }
        }
    }
}
