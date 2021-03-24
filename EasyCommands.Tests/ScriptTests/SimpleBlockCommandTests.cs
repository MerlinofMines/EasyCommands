using System;
using System.Collections.Generic;
using IngameScript;
using Malware.MDKUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests
{
    [TestClass]
    public class SimpleBlockCommandTests
    {
        [TestMethod]
        public void LightBlockHandlerTest()
        {
            var me = new Mock<IMyProgrammableBlock>();
            MDKFactory.ProgramConfig config = default;
            config.ProgrammableBlock = me.Object;

            var program = MDKFactory.CreateProgram<Program>(config);
            int numCommandsInScript = 7;
            String script = @"
:lightshow
set the ""cool light"" color to ""blue""
set the ""cool light"" intensity to 10
set the ""cool light"" blinkInterval to 0.5
set the ""cool light"" blinkOffset to 0.25
set the ""cool light"" falloff to 1
set the ""cool light"" blinkLength to 2
turn on the ""cool light""
";

            me.Setup(b => b.CustomData).Returns(script);

            var mockLight = new Mock<IMyLightingBlock>();
            //TODO: Replace these with mock objects passed to config setup in Program.
            Program.BROADCAST_MESSAGE_PROVIDER = (x) => new List<MyIGCMessage>();
            Program.BlockHandlerRegistry.BLOCK_PROVIDER = (blockType, name) =>
            {
                var blocks = new List<Object>();
                if (blockType.Equals(Program.BlockType.LIGHT) && name.Equals("cool light"))
                {
                    blocks.Add(mockLight.Object);
                }
                return blocks;
            };
            Program.BlockHandlerRegistry.GROUP_BLOCK_PROVIDER = (blockType, name) =>
            {
                return new List<object>();
            };

            // Seven commands in the script, so we need to run the program seven times
            for (int i = 0; i < numCommandsInScript; i++)
            {
                MDKFactory.Run(program);
            }

            mockLight.VerifySet(b => b.Color = new Color(0, 0, 255));
            mockLight.VerifySet(b => b.Intensity = 10f);
            mockLight.VerifySet(b => b.BlinkIntervalSeconds = 0.5f);
            mockLight.VerifySet(b => b.BlinkOffset = 0.25f);
            mockLight.VerifySet(b => b.Falloff = 1);
            mockLight.VerifySet(b => b.BlinkLength = 2);
            mockLight.VerifySet(b => b.Enabled = true);

        }
    }
}
