using System;
using System.Collections.Generic;
using IngameScript;
using Malware.MDKUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI;
using VRageMath;

namespace EasyCommands.Tests {
    [TestClass]
    public class SimpleCommandExecutionTests {


        [TestMethod]
        public void printCommandTest() {
            List<String> logger = new List<String>();
            var me = new Mock<IMyProgrammableBlock>();

            MDKFactory.ProgramConfig config = default;
            config.Echo = (message) => logger.Add(message);
            config.ProgrammableBlock = me.Object;

            var program = MDKFactory.CreateProgram<Program>(config);
            String script = @"
:main
print 'Hello World'
";
            me.Setup(b => b.CustomData).Returns(script);
            Program.LOG_LEVEL = Program.LogLevel.SCRIPT_ONLY;
            //TODO: Replace this with mock objects passed to config setup in Program.
            Program.BROADCAST_MESSAGE_PROVIDER = (x) => new List<MyIGCMessage>();

            MDKFactory.Run(program);

            Assert.AreEqual(1, logger.Count);
            Assert.AreEqual("Hello World", logger[0]);
        }
    }
}
