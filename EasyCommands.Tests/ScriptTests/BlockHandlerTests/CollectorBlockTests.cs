using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class CollectorBlockTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void TurnOnTheCollector() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test collector""")) {
                Mock<IMyCollector> mockCollector = new Mock<IMyCollector>();
                test.MockBlocksOfType("test collector", mockCollector);

                test.RunUntilDone();

                mockCollector.VerifySet(b => b.Enabled = true);
            }
        }
    }
}
