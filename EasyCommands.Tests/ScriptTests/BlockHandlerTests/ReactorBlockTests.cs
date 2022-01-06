using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ReactorBlockTests : ForceLocale {
        [TestMethod]
        public void GetReactorOutput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Reactor Output: "" + ""test reactor"" output")) {
                Mock<IMyReactor> mockReactor = new Mock<IMyReactor>();
                test.MockBlocksOfType("test reactor", mockReactor);
                mockReactor.Setup(b => b.CurrentOutput).Returns(20f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Reactor Output: 20"));
            }
        }

        [TestMethod]
        public void GetReactorLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Reactor Max Output: "" + ""test reactor"" limit")) {
                Mock<IMyReactor> mockReactor = new Mock<IMyReactor>();
                test.MockBlocksOfType("test reactor", mockReactor);
                mockReactor.Setup(b => b.MaxOutput).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Reactor Max Output: 100"));
            }
        }

        [TestMethod]
        public void GetReactorRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Reactor Output: "" + ( 100 * ""test reactor"" ratio ) + ""%""")) {
                Mock<IMyReactor> mockReactor = new Mock<IMyReactor>();
                test.MockBlocksOfType("test reactor", mockReactor);
                mockReactor.Setup(b => b.CurrentOutput).Returns(20f);
                mockReactor.Setup(b => b.MaxOutput).Returns(100f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Reactor Output: 20%"));
            }
        }
    }
}
