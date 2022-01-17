using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class OreDetectorBlockTests {
        [TestMethod]
        public void TellTheOreDetectorToBroadcast() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test ore detector"" to broadcast")) {
                Mock<IMyOreDetector> mockOreDetector = new Mock<IMyOreDetector>();
                test.MockBlocksOfType("test ore detector", mockOreDetector);

                test.RunUntilDone();

                mockOreDetector.VerifySet(b => b.BroadcastUsingAntennas = true);
            }
        }

        [TestMethod]
        public void IsTheOreDetectorBroadcasting() {
            using (ScriptTest test = new ScriptTest(@"Print ""Broadcasting: "" + the ""test ore detector"" is broadcasting")) {
                Mock<IMyOreDetector> mockOreDetector = new Mock<IMyOreDetector>();
                test.MockBlocksOfType("test ore detector", mockOreDetector);
                mockOreDetector.Setup(b => b.BroadcastUsingAntennas).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Broadcasting: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheOreDetectorRange() {
            using (ScriptTest test = new ScriptTest(@"Print ""Ore Detector Range: "" + the ""test ore detector"" range")) {
                Mock<IMyOreDetector> mockOreDetector = new Mock<IMyOreDetector>();
                test.MockBlocksOfType("test ore detector", mockOreDetector);
                MockGetProperty(mockOreDetector, "Range", 1000f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Ore Detector Range: 1000"));
            }
        }

        [TestMethod]
        public void SetTheOreDetectorRange() {
            using (ScriptTest test = new ScriptTest(@"set the ""test ore detector"" range to 5000")) {
                Mock<IMyOreDetector> mockOreDetector = new Mock<IMyOreDetector>();
                test.MockBlocksOfType("test ore detector", mockOreDetector);
                var rangeProperty = MockProperty<IMyOreDetector, float>(mockOreDetector, "Range");
                test.RunUntilDone();

                rangeProperty.Verify(b => b.SetValue(mockOreDetector.Object, 5000));
            }
        }
    }
}
