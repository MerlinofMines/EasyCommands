using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SubclassBlockTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        //Performs some basic tests to confirm we can retrieve classes using their blockhandler's parent class.
        [TestMethod]
        public void MissileLauncherIsAGun() {
            String script = @"
power on the ""test missile"" gun 
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockMissileLauncher = new Mock<IMySmallMissileLauncher>();
                test.MockBlocksOfType("test missile", mockMissileLauncher);

                test.RunUntilDone();

                mockMissileLauncher.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void CockpitIsAShipController() {
            String script = @"
lock the ""test cockpit""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);

                test.RunUntilDone();

                mockCockpit.VerifySet(b => b.HandBrake = true);
            }
        }
    }
}
