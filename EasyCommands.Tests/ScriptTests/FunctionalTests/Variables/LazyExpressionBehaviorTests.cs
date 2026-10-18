using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class LazyExpressionBehaviorTests {
        [DataTestMethod]
        [DataRow("assign ecSource to 2\nbind ecResult to -ecSource\nprint ecResult\nassign ecSource to 5\nprint ecResult", "-2", "-5")]
        [DataRow("assign ecSource to 2\nbind ecResult to ecSource * 3 + 1\nprint ecResult\nassign ecSource to 5\nprint ecResult", "7", "16")]
        [DataRow("assign ecSource to 2\nbind ecResult to ecSource < 3\nprint ecResult\nassign ecSource to 5\nprint ecResult", "True", "False")]
        [DataRow("assign ecValues to [1,2,3]\nbind ecResult to sum of ecValues[]\nprint ecResult\nassign ecValues[1] to 5\nprint ecResult", "6", "9")]
        [DataRow("assign ecValues to [1,2,3]\nbind ecResult to all of ecValues[] > 0\nprint ecResult\nassign ecValues[1] to -5\nprint ecResult", "True", "False")]
        [DataRow("assign ecValues to [10,20,30]\nassign ecIndex to [0,2]\nbind ecResult to ecValues[ecIndex]\nprint ecResult\nassign ecIndex to [1]\nprint ecResult", "[10,30]", "20")]
        [DataRow("assign ecSource to 2\nbind ecFirst to ecSource + 3\nbind ecSecond to ecSource * 4\nassign ecSource to 5\nprint ecFirst\nprint ecSecond", "8", "20")]
        public void BindingsEvaluateCurrentInputsThroughScriptSyntax(string script, string ecFirst, string ecSecond) {
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();
                CollectionAssert.AreEqual(new[] { ecFirst, ecSecond }, test.Logger, string.Join("\n", test.Logger));
            }
        }

        [DataTestMethod]
        [DataRow("sum of \"test piston\" velocity", "2", "9")]
        [DataRow("all of \"test piston\" velocity > 5", "False", "True")]
        public void BlockBindingsReadCurrentGameProperties(string expression, string ecFirst, string ecSecond) {
            using (var test = new ScriptTest("bind ecResult to " + expression + "\nprint ecResult\nset \"test piston\" velocity to 9\nprint ecResult")) {
                var block = new Mock<IMyPistonBase>();
                block.SetupProperty(b => b.Velocity, 2f);
                test.MockBlocksOfType("test piston", block);
                test.RunUntilDone();
                CollectionAssert.AreEqual(new[] { ecFirst, ecSecond }, test.Logger, string.Join("\n", test.Logger));
                Assert.AreEqual(9f, block.Object.Velocity);
            }
        }
    }
}
