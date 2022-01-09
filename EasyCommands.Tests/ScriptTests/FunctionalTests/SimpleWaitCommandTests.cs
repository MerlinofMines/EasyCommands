using System;
using System.Collections.Generic;
using IngameScript;
using Malware.MDKUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleWaitCommandTests {
        [TestMethod]
        public void wait() {
            String script = @"
:main
wait
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual(0, test.Logger.Count);

                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void wait2Ticks() {
            String script = @"
:main
wait 2 ticks
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.RunIterations(2);

                Assert.AreEqual(0, test.Logger.Count);

                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void wait1() {
            String script = @"
:main
wait 1
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.RunIterations(60);

                Assert.AreEqual(0, test.Logger.Count);

                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void wait1Second() {
            String script = @"
:main
wait 1 second
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.RunIterations(60);

                Assert.AreEqual(0, test.Logger.Count);

                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void wait1SecondAtUpdate10() {
            String script = @"
:main
wait 1 second
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.SetUpdateFrequency(UpdateFrequency.Update10);
                test.RunIterations(6);

                Assert.AreEqual(0, test.Logger.Count);

                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void waitHalfSecondAtUpdate10() {
            String script = @"
:main
wait 0.5
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.SetUpdateFrequency(UpdateFrequency.Update10);
                test.RunIterations(3);

                Assert.AreEqual(0, test.Logger.Count);

                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void wait1SecondAtUpdate100() {
            String script = @"
:main
wait 1
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.SetUpdateFrequency(UpdateFrequency.Update100);
                test.RunOnce();

                Assert.AreEqual(0, test.Logger.Count);

                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void wait10SecondsAtUpdate100() {
            String script = @"
:main
wait 10
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.SetUpdateFrequency(UpdateFrequency.Update100);
                test.RunIterations(6);

                Assert.AreEqual(0, test.Logger.Count);

                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }
    }
}
