using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class GasGeneratorBlockTests {
        [TestMethod]
        public void TurnOnTheGasGenerator() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test generator""")) {
                Mock<IMyGasGenerator> mockGenerator = new Mock<IMyGasGenerator>();
                test.MockBlocksOfType("test generator", mockGenerator);

                test.RunUntilDone();

                mockGenerator.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void SetTheGasGeneratorToAuto() {
            using (ScriptTest test = new ScriptTest(@"set the ""test generator"" to auto")) {
                Mock<IMyGasGenerator> mockGenerator = new Mock<IMyGasGenerator>();
                test.MockBlocksOfType("test generator", mockGenerator);

                test.RunUntilDone();

                mockGenerator.VerifySet(b => b.AutoRefill = true);
            }
        }
    }
}
