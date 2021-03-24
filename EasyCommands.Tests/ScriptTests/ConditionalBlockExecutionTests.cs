using System;
using System.Collections.Generic;
using IngameScript;
using Malware.MDKUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI;
using VRageMath;

namespace EasyCommands.Tests
{
    [TestClass]
    public class ConditionalBlockExecutionTests
    {
        [TestMethod]
        public void conditionalVelocityAndSoundBlockTest()
        {
            var mockRoverCockpit = new Mock<IMyCockpit>();
            var mockReverseSirens = new Mock<IMySoundBlock>();

            mockRoverCockpit.Setup(b => b.WorldMatrix).Returns(MatrixD.CreateWorld(Vector3D.Zero));
            mockRoverCockpit.Setup(b => b.GetShipVelocities()).Returns(new MyShipVelocities(new Vector3D(0, 0, 2), Vector3D.Zero));
            mockReverseSirens.Setup(b => b.CustomData).Returns("Playing=False");

            var me = new Mock<IMyProgrammableBlock>();

            MDKFactory.ProgramConfig config = default;
            config.ProgrammableBlock = me.Object;

            var program = MDKFactory.CreateProgram<Program>(config);

            String script = @"
:reverseSirens
  if ""rover cockpit"" backwards velocity > 1
    if ""reverse sirens"" are off
      turn on the ""reverse sirens""
  else
    turn off the ""reverse sirens""
";

            me.Setup(b => b.CustomData).Returns(script);

            //TODO: Replace these with mock objects passed to config setup in Program.
            Program.BROADCAST_MESSAGE_PROVIDER = (x) => new List<MyIGCMessage>();
            Program.BlockHandlerRegistry.BLOCK_PROVIDER = (blockType, name) =>
            {
                var blocks = new List<Object>();
                if (blockType.Equals(Program.BlockType.COCKPIT) && name.Equals("rover cockpit"))
                {
                    blocks.Add(mockRoverCockpit.Object);
                }
                return blocks;
            };
            Program.BlockHandlerRegistry.GROUP_BLOCK_PROVIDER = (blockType, name) =>
            {
                var blocks = new List<Object>();
                if (blockType.Equals(Program.BlockType.SOUND) && name.Equals("reverse sirens"))
                {
                    blocks.Add(mockReverseSirens.Object);
                }
                return blocks;
            };
            MDKFactory.Run(program);

            mockReverseSirens.Verify(b => b.Play(), Times.Once);
        }
    }
}
