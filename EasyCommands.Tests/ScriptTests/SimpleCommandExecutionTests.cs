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
    }
}
