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
            using (var test = new ScriptTest(script)) {
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

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }


        [TestMethod]
        public void indentedCommentsAreIgnored() {
            String script = @"
:main
if true
  #This is a nested comment
  print 'Hello World'
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void incorrectlyIndentedCommentsAreIgnored() {
            String script = @"
:main
if true
#This is a nested comment with incorrect indenting
    #This is another nested comment with incorrect indenting
  print 'Hello World'
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void sectionCommentsAreIgnored() {
            String script = @"
:main
#First Section
Print ""First Section""

#Second Section
Print ""Second Section""
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("First Section", test.Logger[0]);
                Assert.AreEqual("Second Section", test.Logger[1]);
            }
        }

        [TestMethod]
        public void commentAtEndOfFunctionIsIgnored() {
            String script = @"
:main
#First Section
Print ""First Section""

#Second Section
Print ""Second Section""
#Ignore Me
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("First Section", test.Logger[0]);
                Assert.AreEqual("Second Section", test.Logger[1]);
            }
        }

        [TestMethod]
        public void emptyScriptPrintsWelcomeMessage() {
            String script = "";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;
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

        [TestMethod]
        public void pauseContinuesExecutionAfterResuming() {
            String script = @"
:main
#This is a comment
print 'Hello World'
pause
print 'Hello Again'
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilState(Program.ProgramState.PAUSED);
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                test.Logger.Clear();
                test.RunUntilDone();
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello Again", test.Logger[0]);
            }
        }
    }
}
