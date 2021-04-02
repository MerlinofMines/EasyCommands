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
        public void multipleSimpleCommandsGetProcessedInSameTick() {
            List<String> logger = new List<String>();
            var me = new Mock<IMyProgrammableBlock>();

            MDKFactory.ProgramConfig config = default;
            config.Echo = (message) => logger.Add(message);
            config.ProgrammableBlock = me.Object;

            var program = MDKFactory.CreateProgram<Program>(config);
            String script = @"
:main
print 'Hello World'
print 'I Got Printed'
";
            me.Setup(b => b.CustomData).Returns(script);
            Program.LOG_LEVEL = Program.LogLevel.SCRIPT_ONLY;
            //TODO: Replace this with mock objects passed to config setup in Program.
            Program.BROADCAST_MESSAGE_PROVIDER = (x) => new List<MyIGCMessage>();

            MDKFactory.Run(program);

            Assert.AreEqual(2, logger.Count);
            Assert.AreEqual("Hello World", logger[0]);
            Assert.AreEqual("I Got Printed", logger[1]);
        }

        [TestMethod]
        public void blockingCommandsAreStillHonored() {
            List<String> logger = new List<String>();
            var me = new Mock<IMyProgrammableBlock>();

            MDKFactory.ProgramConfig config = default;
            config.Echo = (message) => logger.Add(message);
            config.ProgrammableBlock = me.Object;

            var program = MDKFactory.CreateProgram<Program>(config);
            String script = @"
:main
print 'Hello World'
wait
print 'I Got Printed'
";
            me.Setup(b => b.CustomData).Returns(script);
            Program.LOG_LEVEL = Program.LogLevel.SCRIPT_ONLY;
            //TODO: Replace this with mock objects passed to config setup in Program.
            Program.BROADCAST_MESSAGE_PROVIDER = (x) => new List<MyIGCMessage>();

            MDKFactory.Run(program);

            Assert.AreEqual(1, logger.Count);
            Assert.AreEqual("Hello World", logger[0]);

            MDKFactory.Run(program);

            Assert.AreEqual(2, logger.Count);
            Assert.AreEqual("I Got Printed", logger[1]);
        }
    }
}
