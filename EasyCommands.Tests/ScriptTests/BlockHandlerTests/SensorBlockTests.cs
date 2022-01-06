using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SensorBlockTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void SensorTargetReturnsZeroVectorWhenNothingDetected() {
            String script = @"
print ""Target: "" + ""mock sensor"" target
";
            using (var test = new ScriptTest(script)) {
                var mockSensor = new Mock<IMySensorBlock>();
                mockSensor.Setup(b => b.CustomData).Returns("");
                mockSensor.Setup(b => b.LastDetectedEntity).Returns(MockNoDetectedEntity());

                test.MockBlocksOfType("mock sensor", mockSensor);
                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Target: 0:0:0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TriggerTheSensorAndGetTarget() {
            String script = @"
when ""mock sensor"" is triggered
  print ""Target: "" + ""mock sensor"" target
";
            using (var test = new ScriptTest(script)) {
                var mockSensor = new Mock<IMySensorBlock>();
                mockSensor.Setup(b => b.CustomData).Returns("");
                mockSensor.Setup(b => b.IsActive).Returns(false);

                test.MockBlocksOfType("mock sensor", mockSensor);
                test.RunOnce();

                Assert.AreEqual(0, test.Logger.Count);

                test.RunOnce();

                Assert.AreEqual(0, test.Logger.Count);

                mockSensor.Setup(b => b.IsActive).Returns(true);
                mockSensor.Setup(b => b.LastDetectedEntity).Returns(MockDetectedEntity(new Vector3D(1, 2, 3)));
                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Target: 1:2:3", test.Logger[0]);
            }
        }
    }
}
