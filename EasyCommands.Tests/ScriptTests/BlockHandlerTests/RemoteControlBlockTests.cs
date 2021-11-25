using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class RemoteControlBlockTests {
        [TestMethod]
        public void IsTheRemoteControlOn() {
            using (ScriptTest test = new ScriptTest(@"Print ""Remote Control On: "" + the ""test remote control"" is on")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.IsAutoPilotEnabled).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Remote Control On: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheRemoteControlRunning() {
            using (ScriptTest test = new ScriptTest(@"Print ""Remote Control Running: "" + the ""test remote control"" is running")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.IsAutoPilotEnabled).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Remote Control Running: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheRemoteControlOnAuto() {
            using (ScriptTest test = new ScriptTest(@"Print ""Remote Control Auto: "" + the ""test remote control"" is on auto")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.IsAutoPilotEnabled).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Remote Control Auto: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TurnOnTheRemoteControl() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test remote control""")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);

                test.RunUntilDone();

                mockRemoteControl.Verify(b => b.SetAutoPilotEnabled(true));
            }
        }

        [TestMethod]
        public void TellTheRemoteControlToRun() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test remote control"" to run")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);

                test.RunUntilDone();

                mockRemoteControl.Verify(b => b.SetAutoPilotEnabled(true));
            }
        }

        [TestMethod]
        public void SetTheRemoteControlToAuto() {
            using (ScriptTest test = new ScriptTest(@"set the ""test remote control"" to auto")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);

                test.RunUntilDone();

                mockRemoteControl.Verify(b => b.SetAutoPilotEnabled(true));
            }
        }

        [TestMethod]
        public void GetTheRemoteControlLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Speed Limit: "" + ""test remote control"" limit")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.SpeedLimit).Returns(50f);

                test.RunUntilDone();

                Assert.AreEqual("Speed Limit: 50", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheRemoteControlLimit() {
            using (ScriptTest test = new ScriptTest(@"set the ""test remote control"" limit to 50")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);

                test.RunUntilDone();

                mockRemoteControl.VerifySet(b => b.SpeedLimit = 50f);
            }
        }

        [TestMethod]
        public void IsTheRemoteControlDocking() {
            using (ScriptTest test = new ScriptTest(@"Print ""Docking: "" + ""test remote control"" is docking")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                MockGetProperty(mockRemoteControl, "DockingMode", true);

                test.RunUntilDone();

                Assert.AreEqual("Docking: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TellTheRemoteControlToDock() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test remote control"" to dock")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                var dockingProperty = MockProperty<IMyRemoteControl, bool>(mockRemoteControl, "DockingMode");

                test.RunUntilDone();

                dockingProperty.Verify(b => b.SetValue(mockRemoteControl.Object, true));
            }
        }

        [TestMethod]
        public void GetTheRemoteControlWaypoint() {
            using (ScriptTest test = new ScriptTest(@"Print ""Waypoint: "" + the ""test remote control"" waypoint")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.CurrentWaypoint).Returns(new MyWaypointInfo("Waypoint", new Vector3D(1, 2, 3)));

                test.RunUntilDone();

                Assert.AreEqual("Waypoint: 1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheRemoteControlWaypoint() {
            using (ScriptTest test = new ScriptTest(@"set the ""test remote control"" waypoint to 4:5:6")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);

                test.RunUntilDone();

                mockRemoteControl.Verify(b => b.ClearWaypoints());
                mockRemoteControl.Verify(b => b.AddWaypoint(new MyWaypointInfo("Waypoint", new Vector3D(4, 5, 6))));
            }
        }

        [TestMethod]
        public void GetTheRemoteControlWaypoints() {
            using (ScriptTest test = new ScriptTest(@"Print ""Waypoints: "" + the ""test remote control"" waypoints")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                var expectedWaypoints = new List<MyWaypointInfo> {
                    new MyWaypointInfo("Waypoint 1", new Vector3D(1,2,3)),
                    new MyWaypointInfo("Waypoint 2", new Vector3D(4,5,6)),
                    new MyWaypointInfo("Waypoint 3", new Vector3D(7,8,9))
                };
                mockRemoteControl.Setup(b => b.GetWaypointInfo(new List<MyWaypointInfo>()))
                    .Callback<List<MyWaypointInfo>>(waypoints => waypoints.AddRange(expectedWaypoints));

                test.RunUntilDone();

                Assert.AreEqual("Waypoints: [\"Waypoint 1\"->1:2:3,\"Waypoint 2\"->4:5:6,\"Waypoint 3\"->7:8:9]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheRemoteControlWaypoints() {
            using (ScriptTest test = new ScriptTest(@"
set myWaypoints to []
set myWaypoints [""Waypoint 1""] to 1:2:3
set myWaypoints [""Waypoint 2""] to 4:5:6
set myWaypoints [""Waypoint 3""] to 7:8:9
set the ""test remote control"" waypoints to myWaypoints")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);

                test.RunUntilDone();

                mockRemoteControl.Verify(b => b.ClearWaypoints());
                mockRemoteControl.Verify(b => b.AddWaypoint(new MyWaypointInfo("Waypoint 1", new Vector3D(1, 2, 3))));
                mockRemoteControl.Verify(b => b.AddWaypoint(new MyWaypointInfo("Waypoint 2", new Vector3D(4, 5, 6))));
                mockRemoteControl.Verify(b => b.AddWaypoint(new MyWaypointInfo("Waypoint 3", new Vector3D(7, 8, 9))));
            }
        }

        [TestMethod]
        public void TellTheRemoteControlToBrake() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test remote control"" to brake")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);

                test.RunUntilDone();

                mockRemoteControl.VerifySet(b => b.HandBrake = true);
            }
        }

        [TestMethod]
        public void TurnOnTheRemoteControlHandBrake() {
            using (ScriptTest test = new ScriptTest(@"tell on the ""test remote control"" handbrake")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);

                test.RunUntilDone();

                mockRemoteControl.VerifySet(b => b.HandBrake = true);
            }
        }

        [TestMethod]
        public void LockTheRemoteControl() {
            using (ScriptTest test = new ScriptTest(@"lock the ""test remote control""")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);

                test.RunUntilDone();

                mockRemoteControl.VerifySet(b => b.HandBrake = true);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Velocity: "" + the ""test remote control"" velocity")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.GetShipSpeed()).Returns(100);

                test.RunUntilDone();

                Assert.AreEqual("Velocity: 100", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlUpwardsVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Upwards Velocity: "" + the ""test remote control"" upwards velocity")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                MockShipVelocities(mockRemoteControl, new Vector3D(0, 1, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Upwards Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlDownwardsVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Downwards Velocity: "" + the ""test remote control"" downwards velocity")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                MockShipVelocities(mockRemoteControl, new Vector3D(0, -1, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Downwards Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlLeftVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Left Velocity: "" + the ""test remote control"" left velocity")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                MockShipVelocities(mockRemoteControl, new Vector3D(-1, 0, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Left Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlRightVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Right Velocity: "" + the ""test remote control"" right velocity")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                MockShipVelocities(mockRemoteControl, new Vector3D(1, 0, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Right Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlForwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Forward Velocity: "" + the ""test remote control"" forward velocity")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                MockShipVelocities(mockRemoteControl, new Vector3D(0, 0, -1), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Forward Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlBackwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Backward Velocity: "" + the ""test remote control"" backward velocity")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                MockShipVelocities(mockRemoteControl, new Vector3D(0, 0, 1), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Backward Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Input: "" + the ""test remote control"" input")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.MoveIndicator).Returns(new Vector3D(2, 3, 6));

                test.RunUntilDone();

                Assert.AreEqual("Input: 7", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlUpwardsInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Upwards Input: "" + the ""test remote control"" upwards input")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Upwards Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlDownwardsInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Downwards Input: "" + the ""test remote control"" downwards input")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, -1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Downwards Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlLeftInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Left Input: "" + the ""test remote control"" left input")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.MoveIndicator).Returns(new Vector3D(-1, 0, 0));

                test.RunUntilDone();

                Assert.AreEqual("Left Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlRightInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Right Input: "" + the ""test remote control"" right input")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.MoveIndicator).Returns(new Vector3D(1, 0, 0));

                test.RunUntilDone();

                Assert.AreEqual("Right Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlForwardInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Forward Input: "" + the ""test remote control"" forward input")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, 0, -1));

                test.RunUntilDone();

                Assert.AreEqual("Forward Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlBackwardInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Backward Input: "" + the ""test remote control"" backward input")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, 0, 1));

                test.RunUntilDone();

                Assert.AreEqual("Backward Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Roll: "" + the ""test remote control"" roll")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.RotationIndicator).Returns(new Vector2(2, 3));
                mockRemoteControl.Setup(b => b.RollIndicator).Returns(6);

                test.RunUntilDone();

                Assert.AreEqual("Roll: 7", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlUpwardsRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Upwards Roll: "" + the ""test remote control"" upwards roll")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.RotationIndicator).Returns(new Vector2(-1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Upwards Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlDownwardsRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Downwards Roll: "" + the ""test remote control"" downwards roll")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.RotationIndicator).Returns(new Vector2(1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Downwards Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlLeftRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Left Roll: "" + the ""test remote control"" left roll")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.RotationIndicator).Returns(new Vector2(0, -1));

                test.RunUntilDone();

                Assert.AreEqual("Left Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlRightRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Right Roll: "" + the ""test remote control"" right roll")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.RotationIndicator).Returns(new Vector2(0, 1));

                test.RunUntilDone();

                Assert.AreEqual("Right Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlCounterRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Counter Roll: "" + the ""test remote control"" counter roll")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.RollIndicator).Returns(-1);

                test.RunUntilDone();

                Assert.AreEqual("Counter Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheRemoteControlClockwiseRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Clockwise Roll: "" + the ""test remote control"" clockwise roll")) {
                Mock<IMyRemoteControl> mockRemoteControl = new Mock<IMyRemoteControl>();
                test.MockBlocksOfType("test remote control", mockRemoteControl);
                mockRemoteControl.Setup(b => b.RollIndicator).Returns(1);

                test.RunUntilDone();

                Assert.AreEqual("Clockwise Roll: 1", test.Logger[0]);
            }
        }
    }
}
