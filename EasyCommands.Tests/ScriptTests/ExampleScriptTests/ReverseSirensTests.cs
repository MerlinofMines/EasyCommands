using System;
using System.Collections.Generic;
using IngameScript;
using Malware.MDKUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests
{
    [TestClass]
    public class ReverseSirensTests
    {
        [TestMethod]
        public void conditionalVelocityAndSoundBlockTest()
        {
            String script = @"
:reverseSirens
  if ""rover cockpit"" backwards velocity > 1
    if ""reverse sirens"" are off
      turn on the ""reverse sirens""
  else
    turn off the ""reverse sirens""
";
            using (var test = new ScriptTest(script))
            {
                var mockRoverCockpit = new Mock<IMyCockpit>();
                mockRoverCockpit.Setup(b => b.WorldMatrix).Returns(MatrixD.CreateWorld(Vector3D.Zero));
                mockRoverCockpit.Setup(b => b.GetShipVelocities()).Returns(new MyShipVelocities(new Vector3D(0, 0, 2), Vector3D.Zero));
                var mockReverseSirens = new Mock<IMySoundBlock>();
                mockReverseSirens.Setup(b => b.CustomData).Returns("Playing=False");

                test.MockBlocksOfType("rover cockpit", mockRoverCockpit);
                test.MockBlocksInGroup("reverse sirens", mockReverseSirens);

                test.RunUntilDone();

                mockReverseSirens.Verify(b => b.Play(), Times.Once);
            }
        }
    }
}
