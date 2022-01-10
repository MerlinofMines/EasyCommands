using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ThrustBlockTests {
        [TestMethod]
        public void GetTheThrustLimit() {
            using (ScriptTest test = new ScriptTest(@"print ""Thruster Limit: "" + the ""test thruster"" limit")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                mockThruster.Setup(b => b.MaxThrust).Returns(10000);
                test.RunUntilDone();

                Assert.AreEqual("Thruster Limit: 10000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheThrustLimit() {
            using (ScriptTest test = new ScriptTest(@"set the""test thruster"" to 5000")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                test.RunUntilDone();

                mockThruster.VerifySet(b => b.ThrustOverride = 5000);
            }
        }

        [TestMethod]
        public void GetTheThrustLevel() {
            using (ScriptTest test = new ScriptTest(@"print ""Thruster Level: "" + the ""test thruster"" level")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                mockThruster.Setup(b => b.CurrentThrust).Returns(10000);
                test.RunUntilDone();

                Assert.AreEqual("Thruster Level: 10000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheThrustLevel() {
            using (ScriptTest test = new ScriptTest(@"set the""test thruster"" level to 5000")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                test.RunUntilDone();

                mockThruster.VerifySet(b => b.ThrustOverride = 5000);
            }
        }

        [TestMethod]
        public void GetTheThrustOutput() {
            using (ScriptTest test = new ScriptTest(@"print ""Thruster Output: "" + the ""test thruster"" output")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                mockThruster.Setup(b => b.CurrentThrust).Returns(10000);
                test.RunUntilDone();

                Assert.AreEqual("Thruster Output: 10000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheThrustOutput() {
            using (ScriptTest test = new ScriptTest(@"set the""test thruster"" output to 5000")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                test.RunUntilDone();

                mockThruster.VerifySet(b => b.ThrustOverride = 5000);
            }
        }

        [TestMethod]
        public void GetTheThrustPercentage() {
            using (ScriptTest test = new ScriptTest(@"print ""Thruster Percentage: "" + the ""test thruster"" percentage")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                mockThruster.Setup(b => b.ThrustOverridePercentage).Returns(0.5f);
                test.RunUntilDone();

                Assert.AreEqual("Thruster Percentage: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheThrustPercentage() {
            using (ScriptTest test = new ScriptTest(@"set the""test thruster"" percentage to 0.5")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                test.RunUntilDone();

                mockThruster.VerifySet(b => b.ThrustOverridePercentage = 0.5f);
            }
        }

        [TestMethod]
        public void GetTheThrustOverride() {
            using (ScriptTest test = new ScriptTest(@"print ""Thruster Override: "" + the ""test thruster"" override")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                mockThruster.Setup(b => b.ThrustOverride).Returns(10000);
                test.RunUntilDone();

                Assert.AreEqual("Thruster Override: 10000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheThrustOverride() {
            using (ScriptTest test = new ScriptTest(@"set the""test thruster"" override to 5000")) {
                Mock<IMyThrust> mockThruster = new Mock<IMyThrust>();
                test.MockBlocksOfType("test thruster", mockThruster);

                test.RunUntilDone();

                mockThruster.VerifySet(b => b.ThrustOverride = 5000);
            }
        }
    }
}
