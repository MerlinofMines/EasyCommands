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
    public class SimpleCommandExecutionTests {

        [TestMethod]
        public void printCommandTest() {
            String script = @"
:main
print 'Hello World'
";
            using (var test = new ScriptTest(script))
            {
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }

        }

        [TestMethod]
        public void commentsAreIgnored() {
            String script = @"
:main
#This is a comment
print 'Hello World'
";

            using (var test = new ScriptTest(script))
            {
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void emptyScriptPrintsWelcomeMessage() {
            String script = "";

            using (var test = new ScriptTest(script)) {
                Program.LOG_LEVEL = Program.LogLevel.INFO;
                test.RunOnce();
                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Welcome to EasyCommands!", test.Logger[0]);
                Assert.AreEqual("Add Commands to Custom Data", test.Logger[1]);
            }
        }

        [TestMethod]
        public void updatedScriptAutoParsesAndRestarts() {
            String script = @"
:main
#This is a comment
print 'Hello World'
replay
";

            String newScript = @"
:main
#This is a comment
print 'Hello New World'
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                test.setScript(newScript);
                test.Logger.Clear();
                test.RunOnce();
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello New World", test.Logger[0]);
            }
        }

    }
}
