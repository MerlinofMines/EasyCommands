using IngameScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyCommands.Tests.Bridge {
    [TestClass]
    public class ModOutputTests {
        [TestMethod]
        public void HostPreservesEchoBoundariesNullsAndLongOutputWithoutItsOwnLimit() {
            var host = new ModHostContext();
            host.Begin(null, null, 0);
            var lines = new[] { "first\nsecond", "", null, new string('x', 12000), "last" };
            foreach (var line in lines) host.Echo(line);
            var snapshot = host.Output;
            CollectionAssert.AreEqual(lines, snapshot);
            host.End();
            Assert.AreEqual(0, host.Output.Length);
            CollectionAssert.AreEqual(lines, snapshot);
            host.Begin(null, null, 0);
            Assert.AreEqual(0, host.Output.Length);
            host.Echo("next invocation");
            CollectionAssert.AreEqual(new[] { "next invocation" }, host.Output);
        }
    }
}
