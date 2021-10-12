using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class TerminalBlockTests {
        [TestMethod]
        public void GetBlockName() {
            String script = @"
#First block is the actual program, so grab 2nd index
print ""Name: "" + terminal[1] name
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                Assert.AreEqual("Name: test terminal", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetBlockName() {
            String script = @"
set blockCount to the count of all terminals
set i to 0
until i >= blockCount
  set terminal[i] name to ""Block "" + (i + 1)
  set i to i + 1
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                test.me.VerifySet(b => b.CustomName = "Block 1");
                mockBlock.VerifySet(b => b.CustomName = "Block 2");

            }
        }

        [TestMethod]
        public void HideBlock() {
            String script = @"
hide the ""test terminal""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.ShowInTerminal = false);
            }
        }

        [TestMethod]
        public void ShowBlock() {
            String script = @"
show the ""test terminal""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.ShowInTerminal = true);
            }
        }
    }
}
