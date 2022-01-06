using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ProgramBlockTests : ForceLocale {
        [TestMethod]
        public void IsTheProgramComplete() {
            using (ScriptTest test = new ScriptTest(@"Print ""Complete: "" + the ""test program"" is complete")) {
                Mock<IMyProgrammableBlock> mockProgram = new Mock<IMyProgrammableBlock>();
                test.MockBlocksOfType("test program", mockProgram);

                mockProgram.Setup(b => b.IsRunning).Returns(false);

                test.RunUntilDone();

                Assert.AreEqual("Complete: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheProgramRunning() {
            using (ScriptTest test = new ScriptTest(@"Print ""Running: "" + the ""test program"" is running")) {
                Mock<IMyProgrammableBlock> mockProgram = new Mock<IMyProgrammableBlock>();
                test.MockBlocksOfType("test program", mockProgram);

                mockProgram.Setup(b => b.IsRunning).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Running: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheProgramRunArgument() {
            using (ScriptTest test = new ScriptTest(@"Print ""Run Argument: "" + the ""test program"" text")) {
                Mock<IMyProgrammableBlock> mockProgram = new Mock<IMyProgrammableBlock>();
                test.MockBlocksOfType("test program", mockProgram);

                mockProgram.Setup(b => b.TerminalRunArgument).Returns("Hello World");

                test.RunUntilDone();

                Assert.AreEqual("Run Argument: Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void RunTheProgram() {
            using (ScriptTest test = new ScriptTest(@"run the""test program""")) {
                Mock<IMyProgrammableBlock> mockProgram = new Mock<IMyProgrammableBlock>();
                test.MockBlocksOfType("test program", mockProgram);

                test.RunUntilDone();

                mockProgram.Verify(b => b.TryRun(""));
            }
        }

        [TestMethod]
        public void RunTheProgramWithArgument() {
            using (ScriptTest test = new ScriptTest(@"tell the""test program"" to run ""Hello World""")) {
                Mock<IMyProgrammableBlock> mockProgram = new Mock<IMyProgrammableBlock>();
                test.MockBlocksOfType("test program", mockProgram);

                test.RunUntilDone();

                mockProgram.Verify(b => b.TryRun("Hello World"));
            }
        }

        [TestMethod]
        public void StopTheProgram() {
            using (ScriptTest test = new ScriptTest(@"stop the""test program""")) {
                Mock<IMyProgrammableBlock> mockProgram = new Mock<IMyProgrammableBlock>();
                test.MockBlocksOfType("test program", mockProgram);

                test.RunUntilDone();

                mockProgram.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void TellTheProgamStopRunning() {
            using (ScriptTest test = new ScriptTest(@"tell the""test program"" to stop running")) {
                Mock<IMyProgrammableBlock> mockProgram = new Mock<IMyProgrammableBlock>();
                test.MockBlocksOfType("test program", mockProgram);

                test.RunUntilDone();

                mockProgram.VerifySet(b => b.Enabled = false);
            }
        }
    }
}
