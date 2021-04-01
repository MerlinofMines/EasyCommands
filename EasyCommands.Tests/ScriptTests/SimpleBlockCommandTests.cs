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

            using (ScriptTest test = new ScriptTest(script))
            {
                var mockLight = new Mock<IMyLightingBlock>();
                test.MockBlocksOfType("cool light", mockLight);

                test.RunUntil(Program.ProgramState.COMPLETE);

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
}
