using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class IntegrationScenarioSyntaxTests {
        public class Scenario {
            public string Name { get; set; }
            public string Commands { get; set; }
            public int TimeoutTicks { get; set; }
        }
        [DataTestMethod]
        [DataRow("smoke")]
        [DataRow("apex")]
        public void ScenarioCommandsParse(string suite) {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scenarios", suite + ".json");
            var scenarios = new JavaScriptSerializer().Deserialize<Scenario[]>(File.ReadAllText(path));
            Assert.IsTrue(scenarios.Length > 0);
            foreach (var scenario in scenarios) {
                Assert.IsTrue(scenario.TimeoutTicks > 0, scenario.Name);
                Assert.IsTrue(scenario.Commands.Contains("EC_PASS"), scenario.Name);
                using (var test = new ScriptTest(scenario.Commands)) {
                    var source = new Mock<IMyCargoContainer>();
                    var destination = new Mock<IMyCargoContainer>();
                    MockEntityUtility.MockInventories(source, new Mock<IMyInventory>());
                    MockEntityUtility.MockInventories(destination, new Mock<IMyInventory>());
                    test.MockBlocksOfType("source cargo", source);
                    test.MockBlocksOfType("destination cargo", destination);
                    test.RunUntilDone();
                    Assert.IsFalse(test.Logger.Any(line => line.Contains("Parsing Exception")),
                        scenario.Name + ": " + String.Join("\n", test.Logger.Distinct().Take(20)));
                }
            }
        }
    }
}
