﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class CockpitBlockTests {
        [TestMethod]
        public void TellTheCockpitToBrake() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test cockpit"" to brake")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);

                test.RunUntilDone();

                mockCockpit.VerifySet(b => b.HandBrake = true);
            }
        }

        [TestMethod]
        public void TurnOnTheCockpitHandBrake() {
            using (ScriptTest test = new ScriptTest(@"tell on the ""test cockpit"" handbrake")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);

                test.RunUntilDone();

                mockCockpit.VerifySet(b => b.HandBrake = true);
            }
        }

        [TestMethod]
        public void LockTheCockpit() {
            using (ScriptTest test = new ScriptTest(@"lock the ""test cockpit""")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);

                test.RunUntilDone();

                mockCockpit.VerifySet(b => b.HandBrake = true);
            }
        }

        [TestMethod]
        public void GetTheCockpitVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Velocity: "" + the ""test cockpit"" velocity")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.GetShipSpeed()).Returns(100);

                test.RunUntilDone();

                Assert.AreEqual("Velocity: 100", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitUpwardsVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Upwards Velocity: "" + the ""test cockpit"" upwards velocity")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                MockShipVelocities(mockCockpit, new Vector3D(0, 1, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Upwards Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitDownwardsVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Downwards Velocity: "" + the ""test cockpit"" downwards velocity")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                MockShipVelocities(mockCockpit, new Vector3D(0, -1, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Downwards Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitLeftVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Left Velocity: "" + the ""test cockpit"" left velocity")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                MockShipVelocities(mockCockpit, new Vector3D(-1, 0, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Left Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitRightVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Right Velocity: "" + the ""test cockpit"" right velocity")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                MockShipVelocities(mockCockpit, new Vector3D(1, 0, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Right Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitForwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Forward Velocity: "" + the ""test cockpit"" forward velocity")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                MockShipVelocities(mockCockpit, new Vector3D(0, 0, -1), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Forward Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitBackwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Backward Velocity: "" + the ""test cockpit"" backward velocity")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                MockShipVelocities(mockCockpit, new Vector3D(0, 0, 1), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Backward Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Input: "" + the ""test cockpit"" input")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.MoveIndicator).Returns(new Vector3D(2, 3, 6));

                test.RunUntilDone();

                Assert.AreEqual("Input: 7", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitUpwardsInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Upwards Input: "" + the ""test cockpit"" upwards input")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Upwards Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitDownwardsInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Downwards Input: "" + the ""test cockpit"" downwards input")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, -1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Downwards Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitLeftInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Left Input: "" + the ""test cockpit"" left input")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.MoveIndicator).Returns(new Vector3D(-1, 0, 0));

                test.RunUntilDone();

                Assert.AreEqual("Left Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitRightInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Right Input: "" + the ""test cockpit"" right input")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.MoveIndicator).Returns(new Vector3D(1, 0, 0));

                test.RunUntilDone();

                Assert.AreEqual("Right Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitForwardInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Forward Input: "" + the ""test cockpit"" forward input")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, 0, -1));

                test.RunUntilDone();

                Assert.AreEqual("Forward Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitBackwardInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Backward Input: "" + the ""test cockpit"" backward input")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, 0, 1));

                test.RunUntilDone();

                Assert.AreEqual("Backward Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Roll: "" + the ""test cockpit"" roll")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.RotationIndicator).Returns(new Vector2(2, 3));
                mockCockpit.Setup(b => b.RollIndicator).Returns(6);

                test.RunUntilDone();

                Assert.AreEqual("Roll: 7", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitUpwardsRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Upwards Roll: "" + the ""test cockpit"" upwards roll")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.RotationIndicator).Returns(new Vector2(-1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Upwards Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitDownwardsRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Downwards Roll: "" + the ""test cockpit"" downwards roll")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.RotationIndicator).Returns(new Vector2(1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Downwards Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitLeftRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Left Roll: "" + the ""test cockpit"" left roll")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.RotationIndicator).Returns(new Vector2(0, -1));

                test.RunUntilDone();

                Assert.AreEqual("Left Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitRightRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Right Roll: "" + the ""test cockpit"" right roll")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.RotationIndicator).Returns(new Vector2(0, 1));

                test.RunUntilDone();

                Assert.AreEqual("Right Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitCounterRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Counter Roll: "" + the ""test cockpit"" counter roll")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.RollIndicator).Returns(-1);

                test.RunUntilDone();

                Assert.AreEqual("Counter Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCockpitClockwiseRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Clockwise Roll: "" + the ""test cockpit"" clockwise roll")) {
                Mock<IMyCockpit> mockCockpit = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", mockCockpit);
                mockCockpit.Setup(b => b.RollIndicator).Returns(1);

                test.RunUntilDone();

                Assert.AreEqual("Clockwise Roll: 1", test.Logger[0]);
            }
        }
    }
}