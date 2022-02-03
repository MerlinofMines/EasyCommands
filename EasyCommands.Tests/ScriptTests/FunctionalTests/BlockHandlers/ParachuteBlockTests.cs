using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ParachuteBlockTests {
        [TestMethod]
        public void TurnOnTheParachute() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test parachute""")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);

                test.RunUntilDone();

                mockParachute.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheParachute() {
            using (ScriptTest test = new ScriptTest(@"turn off the ""test parachute""")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);

                test.RunUntilDone();

                mockParachute.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void OpenTheParachute() {
            using (ScriptTest test = new ScriptTest(@"open the ""test parachute""")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);

                test.RunUntilDone();

                mockParachute.Verify(b => b.OpenDoor());
            }
        }

        [TestMethod]
        public void CloseTheParachute() {
            using (ScriptTest test = new ScriptTest(@"close the ""test parachute""")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);

                test.RunUntilDone();

                mockParachute.Verify(b => b.CloseDoor());
            }
        }

        [TestMethod]
        public void IsTheParachuteOpened() {
            using (ScriptTest test = new ScriptTest(@"print ""Parachute Open: "" + ""test parachute"" is open")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);
                mockParachute.Setup(b => b.Status).Returns(DoorStatus.Open);

                test.RunUntilDone();

                Assert.AreEqual("Parachute Open: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTheParachuteToAuto() {
            using (ScriptTest test = new ScriptTest(@"set the ""test parachute"" to auto")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);
                var autoProperty = MockProperty<IMyParachute, bool>(mockParachute, "AutoDeploy");
                test.RunUntilDone();

                autoProperty.Verify(b => b.SetValue(mockParachute.Object, true));
            }
        }

        [TestMethod]
        public void IsTheParachuteOnAuto() {
            using (ScriptTest test = new ScriptTest(@"Print ""Parachute Auto: "" + the ""test parachute"" is on auto")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);
                MockGetProperty(mockParachute, "AutoDeploy", true);

                test.RunUntilDone();

                Assert.AreEqual("Parachute Auto: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheParachuteRange() {
            using (ScriptTest test = new ScriptTest(@"Print ""Parachute Range: "" + the ""test parachute"" range")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);
                MockGetProperty(mockParachute, "AutoDeployHeight", 1000f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Parachute Range: 1000"));
            }
        }

        [TestMethod]
        public void SetTheParachuteRange() {
            using (ScriptTest test = new ScriptTest(@"set the ""test parachute"" range to 5000")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);
                var rangeProperty = MockProperty<IMyParachute, float>(mockParachute, "AutoDeployHeight");

                test.RunUntilDone();

                rangeProperty.Verify(b => b.SetValue(mockParachute.Object, 5000));
            }
        }

        [TestMethod]
        public void GetTheParachuteVelocity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Parachute Velocity: "" + the ""test parachute"" velocity")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);
                mockParachute.Setup(b => b.GetVelocity()).Returns(new Vector3D(3, 0, 0));

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Parachute Velocity: 3"));
            }
        }

        [TestMethod]
        public void GetTheParachuteGravity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Parachute Gravity: "" + the ""test parachute"" gravity")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);
                mockParachute.Setup(b => b.GetTotalGravity()).Returns(new Vector3D(1, 2, 3));

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Parachute Gravity: 1:2:3"));
            }
        }

        [TestMethod]
        public void GetTheParachuteHeight() {
            using (ScriptTest test = new ScriptTest(@"Print ""Parachute Height: "" + the ""test parachute"" height")) {
                Mock<IMyParachute> mockParachute = new Mock<IMyParachute>();
                test.MockBlocksOfType("test parachute", mockParachute);
                Vector3D? expectedVector = new Vector3D(1000, 0, 0);
                mockParachute.Setup(b => b.GetPosition()).Returns(new Vector3D(900, 0, 0));
                mockParachute.Setup(b => b.TryGetClosestPoint(out expectedVector)).Returns(true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Parachute Height: 100"));
            }
        }
    }
}
