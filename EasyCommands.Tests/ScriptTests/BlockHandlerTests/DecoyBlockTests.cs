using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class DecoyBlockTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void TurnOnTheDecoy() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test decoy""")) {
                Mock<IMyDecoy> mockDecoy = new Mock<IMyDecoy>();
                test.MockBlocksOfType("test decoy", mockDecoy);

                test.RunUntilDone();

                mockDecoy.VerifySet(b => b.Enabled = true);
            }
        }
    }
}
