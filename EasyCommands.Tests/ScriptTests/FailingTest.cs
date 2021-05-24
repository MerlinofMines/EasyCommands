using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class FailingTest {
        [TestMethod]
        public void AlwaysFail() {
            Assert.IsTrue(false);
        }
    }
}
