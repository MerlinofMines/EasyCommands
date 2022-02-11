using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class IncrementVariableTests {
        [TestMethod]
        public void IncrementLocalVariable() {
            var script = @"
assign a to 1
a++
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 2"));
            }
        }

        [TestMethod]
        public void IncrementGlobalVariable() {
            var script = @"
assign global a to 1
a++
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 2"));
            }
        }

        [TestMethod]
        public void IncrementLocalVariableByAmount() {
            var script = @"
assign a to 1
a+=2
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 3"));
            }
        }

        [TestMethod]
        public void IncrementLocalVariableByVariableAmount() {
            var script = @"
assign a to 1
assign b to 2
a+=b
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 3"));
            }
        }

        [TestMethod]
        public void DecrementLocalVariable() {
            var script = @"
assign a to 1
a--
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 0"));
            }
        }

        [TestMethod]
        public void DecrementGlobalVariable() {
            var script = @"
assign global a to 1
a--
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 0"));
            }
        }

        [TestMethod]
        public void DecrementLocalVariableByAmount() {
            var script = @"
assign a to 3
a-=2
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 1"));
            }
        }

        [TestMethod]
        public void DecrementLocalVariableByVariableAmount() {
            var script = @"
assign a to 3
assign b to 2
a-=b
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 1"));
            }
        }

        [TestMethod]
        public void IncreaseVariableBySelectoProperty() {
            var script = @"
assign a to 1
increase a by ""My Beacon"" range
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                var mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("My Beacon", mockBeacon);
                mockBeacon.Setup(b => b.Radius).Returns(10f);

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 11"));
            }
        }

        [TestMethod]
        public void IncrementVariableBySelectoProperty() {
            var script = @"
assign a to 1
a += ""My Beacon"" range
Print ""a is: "" + a
";

            using (var test = new ScriptTest(script)) {
                var mockBeacon = new Mock<IMyBeacon>();
                test.MockBlocksOfType("My Beacon", mockBeacon);
                mockBeacon.Setup(b => b.Radius).Returns(10f);

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("a is: 11"));
            }
        }
    }
}
