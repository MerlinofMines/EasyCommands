using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class CryoChamberBlockTests {
        [TestMethod]
        public void TurnOnTheCryoChamber() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test cryo chamber""")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);

                test.RunUntilDone();

                mockCryoChamber.VerifySet(b => b.IsMainCockpit = true);
            }
        }

        [TestMethod]
        public void TurnOffTheCryoChamber() {
            using (ScriptTest test = new ScriptTest(@"turn off the ""test cryo chamber""")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);

                test.RunUntilDone();

                mockCryoChamber.VerifySet(b => b.IsMainCockpit = false);
            }
        }

        [TestMethod]
        public void EnableTheCryoChamber() {
            using (ScriptTest test = new ScriptTest(@"enable the ""test cryo chamber""")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);

                test.RunUntilDone();

                mockCryoChamber.VerifySet(b => b.IsMainCockpit = true);
            }
        }

        [TestMethod]
        public void DisableTheCryoChamber() {
            using (ScriptTest test = new ScriptTest(@"disable the ""test cryo chamber""")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);

                test.RunUntilDone();

                mockCryoChamber.VerifySet(b => b.IsMainCockpit = false);
            }
        }

        [TestMethod]
        public void TurnOnTheCryoChamberDampeners() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test cryo chamber"" dampeners")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);

                test.RunUntilDone();

                mockCryoChamber.VerifySet(b => b.DampenersOverride = true);
            }
        }

        [TestMethod]
        public void SetTheCryoChamberToAuto() {
            using (ScriptTest test = new ScriptTest(@"set the ""test cryo chamber"" to auto")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);

                test.RunUntilDone();

                mockCryoChamber.VerifySet(b => b.DampenersOverride = true);
            }
        }

        [TestMethod]
        public void TellTheCryoChamberToBrake() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test cryo chamber"" to brake")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);

                test.RunUntilDone();

                mockCryoChamber.VerifySet(b => b.HandBrake = true);
            }
        }

        [TestMethod]
        public void TurnOnTheCryoChamberHandBrake() {
            using (ScriptTest test = new ScriptTest(@"tell on the ""test cryo chamber"" handbrake")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);

                test.RunUntilDone();

                mockCryoChamber.VerifySet(b => b.HandBrake = true);
            }
        }

        [TestMethod]
        public void LockTheCryoChamber() {
            using (ScriptTest test = new ScriptTest(@"lock the ""test cryo chamber""")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);

                test.RunUntilDone();

                mockCryoChamber.VerifySet(b => b.HandBrake = true);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberIsOccupied() {
            using (ScriptTest test = new ScriptTest(@"Print ""Occupied: "" + the ""test cryo chamber"" is occupied")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.IsUnderControl).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Occupied: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberIsOccupiedWhenNotOccupied() {
            using (ScriptTest test = new ScriptTest(@"Print ""Occupied: "" + the ""test cryo chamber"" is occupied")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.IsUnderControl).Returns(false);

                test.RunUntilDone();

                Assert.AreEqual("Occupied: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberOxygenCapacity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Oxygen Capacity: "" + the ""test cryo chamber"" capacity")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.OxygenCapacity).Returns(20f);

                test.RunUntilDone();

                Assert.AreEqual("Oxygen Capacity: 20", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberOxygenRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Oxygen Ratio: "" + the ""test cryo chamber"" ratio")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.OxygenFilledRatio).Returns(0.5f);

                test.RunUntilDone();

                Assert.AreEqual("Oxygen Ratio: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberGravity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Gravity: "" + the ""test cryo chamber"" gravity")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.GetTotalGravity()).Returns(new Vector3D(1, 2, 3));

                test.RunUntilDone();

                Assert.AreEqual("Gravity: 1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Velocity: "" + the ""test cryo chamber"" velocity")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                MockShipVelocities(mockCryoChamber, new Vector3D(2, 3, 6), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Velocity: 2:3:6", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberUpwardsVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Upwards Velocity: "" + the ""test cryo chamber"" upwards velocity")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                MockShipVelocities(mockCryoChamber, new Vector3D(0, 1, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Upwards Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberDownwardsVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Downwards Velocity: "" + the ""test cryo chamber"" downwards velocity")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                MockShipVelocities(mockCryoChamber, new Vector3D(0, -1, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Downwards Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberLeftVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Left Velocity: "" + the ""test cryo chamber"" left velocity")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                MockShipVelocities(mockCryoChamber, new Vector3D(-1, 0, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Left Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberRightVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Right Velocity: "" + the ""test cryo chamber"" right velocity")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                MockShipVelocities(mockCryoChamber, new Vector3D(1, 0, 0), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Right Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberForwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Forward Velocity: "" + the ""test cryo chamber"" forward velocity")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                MockShipVelocities(mockCryoChamber, new Vector3D(0, 0, -1), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Forward Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberBackwardVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Backward Velocity: "" + the ""test cryo chamber"" backward velocity")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                MockShipVelocities(mockCryoChamber, new Vector3D(0, 0, 1), Vector3D.Zero);

                test.RunUntilDone();

                Assert.AreEqual("Backward Velocity: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Input: "" + the ""test cryo chamber"" input")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.MoveIndicator).Returns(new Vector3D(2, 3, -6));

                test.RunUntilDone();

                Assert.AreEqual("Input: 2:3:6", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberUpwardsInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Upwards Input: "" + the ""test cryo chamber"" upwards input")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Upwards Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberDownwardsInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Downwards Input: "" + the ""test cryo chamber"" downwards input")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, -1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Downwards Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberLeftInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Left Input: "" + the ""test cryo chamber"" left input")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.MoveIndicator).Returns(new Vector3D(-1, 0, 0));

                test.RunUntilDone();

                Assert.AreEqual("Left Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberRightInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Right Input: "" + the ""test cryo chamber"" right input")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.MoveIndicator).Returns(new Vector3D(1, 0, 0));

                test.RunUntilDone();

                Assert.AreEqual("Right Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberForwardInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Forward Input: "" + the ""test cryo chamber"" forward input")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, 0, -1));

                test.RunUntilDone();

                Assert.AreEqual("Forward Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberBackwardInput() {
            using (ScriptTest test = new ScriptTest(@"Print ""Backward Input: "" + the ""test cryo chamber"" backward input")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.MoveIndicator).Returns(new Vector3D(0, 0, 1));

                test.RunUntilDone();

                Assert.AreEqual("Backward Input: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Roll: "" + the ""test cryo chamber"" roll")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.RotationIndicator).Returns(new Vector2(4.5f, 9f));
                mockCryoChamber.Setup(b => b.RollIndicator).Returns(6);

                test.RunUntilDone();

                Assert.AreEqual("Roll: -0.5:1:6", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberUpwardsRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Upwards Roll: "" + the ""test cryo chamber"" upwards roll")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.RotationIndicator).Returns(new Vector2(4.5f, 0));

                test.RunUntilDone();

                Assert.AreEqual("Upwards Roll: -0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberDownwardsRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Downwards Roll: "" + the ""test cryo chamber"" downwards roll")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.RotationIndicator).Returns(new Vector2(4.5f, 0));

                test.RunUntilDone();

                Assert.AreEqual("Downwards Roll: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberLeftRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Left Roll: "" + the ""test cryo chamber"" left roll")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.RotationIndicator).Returns(new Vector2(0, -4.5f));

                test.RunUntilDone();

                Assert.AreEqual("Left Roll: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberRightRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Right Roll: "" + the ""test cryo chamber"" right roll")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.RotationIndicator).Returns(new Vector2(0, 4.5f));

                test.RunUntilDone();

                Assert.AreEqual("Right Roll: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberCounterRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Counter Roll: "" + the ""test cryo chamber"" counter roll")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.RollIndicator).Returns(-1);

                test.RunUntilDone();

                Assert.AreEqual("Counter Roll: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheCryoChamberClockwiseRoll() {
            using (ScriptTest test = new ScriptTest(@"Print ""Clockwise Roll: "" + the ""test cryo chamber"" clockwise roll")) {
                Mock<IMyCryoChamber> mockCryoChamber = new Mock<IMyCryoChamber>();
                test.MockBlocksOfType("test cryo chamber", mockCryoChamber);
                mockCryoChamber.Setup(b => b.RollIndicator).Returns(1);

                test.RunUntilDone();

                Assert.AreEqual("Clockwise Roll: 1", test.Logger[0]);
            }
        }
    }
}
