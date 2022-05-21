using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class CameraBlockTests {
        [TestMethod]
        public void TurnOnTheCamera() {
            using (var test = new ScriptTest(@"turn on the ""test camera""")) {
                var mockCamera = new Mock<IMyCameraBlock>();
                test.MockBlocksOfType("test camera", mockCamera);

                test.RunUntilDone();

                mockCamera.VerifySet(c => c.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheCamera() {
            using (var test = new ScriptTest(@"turn off the ""test camera""")) {
                var mockCamera = new Mock<IMyCameraBlock>();
                test.MockBlocksOfType("test camera", mockCamera);

                test.RunUntilDone();

                mockCamera.VerifySet(c => c.Enabled = false);
            }
        }

        [TestMethod]
        public void getCameraRange() {
            String script = @"
assign a to ""mock camera"" range
print ""Range: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();
                mockCamera.Setup(b => b.CustomData).Returns("Range=200");

                test.MockBlocksOfType("mock camera", mockCamera);
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Range: 200", test.Logger[0]);
            }
        }

        [TestMethod]
        public void setCameraRange() {
            String script = @"
set the ""mock camera"" range to 200
";
            using (var test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();

                mockCamera.Setup(c => c.CustomData).Returns("");
                test.MockBlocksOfType("mock camera", mockCamera);
                test.RunUntilDone();

                mockCamera.VerifySet(c => c.CustomData = "Range=200");
            }
        }

        [TestMethod]
        public void triggerTheCamera() {
            String script = @"
trigger the ""mock camera""
";
            using (var test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();

                test.MockBlocksOfType("mock camera", mockCamera);
                test.RunUntilDone();

                mockCamera.VerifySet(c => c.EnableRaycast = true);
            }
        }

        [TestMethod]
        public void cameraIsTriggeredGetTarget() {
            String script = @"
when ""mock camera"" is triggered
  print ""Target: "" + ""mock camera"" target
";
            using (var test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();
                mockCamera.Setup(b => b.CustomData).Returns("");
                mockCamera.Setup(b => b.CanScan(1000)).Returns(false);

                test.MockBlocksOfType("mock camera", mockCamera);
                test.RunOnce();

                Assert.AreEqual(0, test.Logger.Count);
                mockCamera.VerifySet(b => b.EnableRaycast = true);
                mockCamera.Verify(b => b.CanScan(1000));

                test.RunOnce();

                Assert.AreEqual(0, test.Logger.Count);
                mockCamera.Verify(b => b.CanScan(1000));

                mockCamera.Setup(b => b.CanScan(1000)).Returns(true);
                mockCamera.Setup(b => b.Raycast(1000,0,0)).Returns(MockDetectedEntity(new Vector3D(1, 2, 3)));
                mockCamera.Setup(b => b.CustomData).Returns("Target=1:2:3");
                test.RunOnce();

                mockCamera.VerifySet(b => b.CustomData = "Target=1:2:3");
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Target: 1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void cameraIsTriggeredGetTargetVelocity() {
            String script = @"
when ""mock camera"" is triggered
  print ""Target Velocity: "" + ""mock camera"" target velocity
";
            using (var test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();
                mockCamera.Setup(b => b.CustomData).Returns("");
                mockCamera.Setup(b => b.CanScan(1000)).Returns(false);

                test.MockBlocksOfType("mock camera", mockCamera);
                test.RunOnce();

                Assert.AreEqual(0, test.Logger.Count);
                mockCamera.VerifySet(b => b.EnableRaycast = true);
                mockCamera.Verify(b => b.CanScan(1000));

                test.RunOnce();

                Assert.AreEqual(0, test.Logger.Count);
                mockCamera.Verify(b => b.CanScan(1000));

                mockCamera.Setup(b => b.CanScan(1000)).Returns(true);
                mockCamera.Setup(b => b.Raycast(1000, 0, 0)).Returns(MockDetectedEntity(new Vector3D(1, 2, 3), new Vector3D(4, 5, 6)));
                mockCamera.Setup(b => b.CustomData).Returns("Target=1:2:3\nVelocity=4:5:6");
                test.RunOnce();

                mockCamera.VerifySet(b => b.CustomData = "Target=1:2:3\nVelocity=4:5:6");
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Target Velocity: 4:5:6", test.Logger[0]);
            }
        }

        [TestMethod]
        public void LastTargetIsStillAvailableWhenCannotScan() {
            String script = @"
print ""Target: "" + ""mock camera"" target
";
            using (var test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();
                mockCamera.Setup(b => b.CanScan(1000)).Returns(false);
                mockCamera.Setup(b => b.CustomData).Returns("Target=1:2:3");

                test.MockBlocksOfType("mock camera", mockCamera);
                test.RunOnce();
                mockCamera.VerifySet(b => b.EnableRaycast = true);
                mockCamera.Verify(b => b.CanScan(1000));

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Target: 1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NoTargetReturnsZeroVector() {
            String script = @"
print ""Target: "" + ""mock camera"" target
";
            using (var test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();
                mockCamera.Setup(b => b.CustomData).Returns("");
                mockCamera.Setup(b => b.CanScan(1000)).Returns(false);

                test.MockBlocksOfType("mock camera", mockCamera);
                test.RunOnce();
                mockCamera.VerifySet(b => b.EnableRaycast = true);
                mockCamera.Verify(b => b.CanScan(1000));

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Target: 0:0:0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NoScannedTargetDeletesPreviouslyStoredTarget() {
            String script = @"
print ""Target: "" + avg ""mock camera"" target
";
            using (var test = new ScriptTest(script)) {
                var mockCamera = new Mock<IMyCameraBlock>();
                mockCamera.Setup(b => b.CanScan(1000)).Returns(false);
                mockCamera.Setup(b => b.CustomData).Returns("Target=1:2:3");

                test.MockBlocksOfType("mock camera", mockCamera);
                test.RunOnce();
                mockCamera.VerifySet(b => b.EnableRaycast = true);
                mockCamera.Verify(b => b.CanScan(1000));

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Target: 1:2:3", test.Logger[0]);

                test.Logger.Clear();
                mockCamera.Setup(b => b.CanScan(1000)).Returns(true);
                mockCamera.Setup(b => b.Raycast(1000, 0, 0)).Returns(MockNoDetectedEntity());
                mockCamera.Setup(b => b.CustomData).Returns("");
                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Target: 0:0:0", test.Logger[0]);
                mockCamera.VerifySet(b => b.CustomData = "");
            }
        }
    }
}
