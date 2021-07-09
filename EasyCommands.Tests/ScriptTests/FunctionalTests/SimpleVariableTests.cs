using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleVariableTests {
        [TestMethod]
        public void TestImplicitVariableAssignmentAndGetValue() {
            var script = @"
assign a to 1
assign b to a + 1
Print ""b is: "" + b
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("b is: 2"));
            }
        }

        [TestMethod]
        public void TestImplicitStringValue() {
            var script = @"
assign b to a + 1
Print ""b is: "" + b
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("b is: a1"));
            }
        }

        [TestMethod]
        public void AssignVariableWithSet() {
            var script = @"
set b to a + 1
Print ""b is: "" + b
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("b is: a1"));
            }
        }

        [TestMethod]
        public void AssignGlobalVariableWithSet() {
            var script = @"
set global b to a + 1
Print ""b is: "" + b
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("b is: a1"));
            }
        }

        [TestMethod]
        public void piIsAvailable() {
            var script = @"
Print ""Pi is: "" + pi
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Pi is: 3.141593"));
            }
        }

        [TestMethod]
        public void eIsAvailable() {
            var script = @"
Print ""e is: "" + e
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("e is: 2.718282"));
            }
        }
    }
}
