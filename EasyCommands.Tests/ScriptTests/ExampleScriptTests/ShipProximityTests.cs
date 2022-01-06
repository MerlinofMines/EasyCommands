using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ShipProximityTests : ForceLocale {
        string script = @"
:setup
assign ""LASER_SCAN_DISTANCE"" to 10000
assign ""LASER_CAMERA"" to 'My Camera'
set $LASER_CAMERA camera range to LASER_SCAN_DISTANCE

while true
  call ""detect""

:detect
when $LASER_CAMERA camera is triggered
  call calculateClosestDistance
  Print ""Distance To Target: "" + round ( abs distanceVector / 10 ) / 100 + ""km""
  Print ""Closest Approach: "" + round ( closestDistance / 10 ) / 100 + ""km""

:calculateClosestDistance
assign ""distanceVector"" to avg $LASER_CAMERA camera position - avg $LASER_CAMERA camera target
assign ""velocityVector"" to avg $LASER_CAMERA camera targetVelocity
if abs velocityVector < 0.0001
  assign ""closestDistance"" to abs distanceVector
else
  assign ""vang"" to acos ( ( distanceVector . velocityVector ) / ( abs distanceVector * abs velocityVector ) )
  assign ""closestDistance"" to sin vang * abs distanceVector
";

        [TestMethod]
        public void GetShipProximity() {
            using (ScriptTest test = new ScriptTest(script)) {

                var mockCamera = new Mock<IMyCameraBlock>();
                test.MockBlocksOfType("My Camera", mockCamera);
                mockCamera.Setup(b => b.CanScan(1000)).Returns(false);
                mockCamera.Setup(b => b.CustomData).Returns("Range=10000");

                test.RunOnce();

                mockCamera.VerifySet(b => b.CustomData = "Range=10000");

                Assert.AreEqual(0, test.Logger.Count);

                mockCamera.Setup(b => b.Position).Returns(Vector3I.Zero);
                mockCamera.Setup(b => b.CanScan(10000)).Returns(true);
                mockCamera.Setup(b => b.Raycast(10000, 0, 0)).Returns(MockDetectedEntity(new Vector3D(1100, 1100, 0), new Vector3D(0, -10, 0)));
                mockCamera.Setup(b => b.CustomData).Returns(@"
Range=10000
Target=1100:1100:0
Velocity=0:-1:0
");

                test.RunOnce();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Distance To Target: 1.56km", test.Logger[0]);
                Assert.AreEqual("Closest Approach: 1.1km", test.Logger[1]);
            }
        }

        [TestMethod]
        public void GetShipProximityStoppedTarget() {
            using (ScriptTest test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();
                test.MockBlocksOfType("My Camera", mockCamera);
                mockCamera.Setup(b => b.CanScan(1000)).Returns(false);
                mockCamera.Setup(b => b.CustomData).Returns("Range=10000");

                test.RunOnce();

                mockCamera.VerifySet(b => b.CustomData = "Range=10000");

                Assert.AreEqual(0, test.Logger.Count);

                mockCamera.Setup(b => b.Position).Returns(Vector3I.Zero);
                mockCamera.Setup(b => b.CanScan(10000)).Returns(true);
                mockCamera.Setup(b => b.Raycast(10000, 0, 0)).Returns(MockDetectedEntity(new Vector3D(1100, 1100, 0), new Vector3D(0, 0, 0)));
                mockCamera.Setup(b => b.CustomData).Returns(@"
Range=10000
Target=1100:1100:0
Velocity=0:0:0
");

                test.RunOnce();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Distance To Target: 1.56km", test.Logger[0]);
                Assert.AreEqual("Closest Approach: 1.56km", test.Logger[1]);
            }
        }

        [TestMethod]
        public void GetShipProximityNoTarget() {
            using (ScriptTest test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();
                test.MockBlocksOfType("My Camera", mockCamera);
                mockCamera.Setup(b => b.CanScan(1000)).Returns(false);
                mockCamera.Setup(b => b.CustomData).Returns("Range=10000");

                test.RunOnce();

                mockCamera.VerifySet(b => b.CustomData = "Range=10000");

                Assert.AreEqual(0, test.Logger.Count);

                mockCamera.Setup(b => b.Position).Returns(Vector3I.Zero);
                mockCamera.Setup(b => b.CanScan(10000)).Returns(true);
                mockCamera.Setup(b => b.Raycast(10000, 0, 0)).Returns(MockNoDetectedEntity());
                mockCamera.Setup(b => b.CustomData).Returns("Range=10000");

                test.RunOnce();

                Assert.AreEqual(0, test.Logger.Count);
            }
        }
    }
}
