using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class DoorBlockTests {
        [TestMethod]
        public void OpenTheDoor() {
            using (ScriptTest test = new ScriptTest(@"open the ""test door""")) {
                Mock<IMyDoor> mockDoor = new Mock<IMyDoor>();
                test.MockBlocksOfType("test door", mockDoor);

                test.RunUntilDone();

                mockDoor.Verify(b => b.OpenDoor());
            }
        }

        [TestMethod]
        public void CloseTheDoor() {
            using (ScriptTest test = new ScriptTest(@"close the ""test door""")) {
                Mock<IMyDoor> mockDoor = new Mock<IMyDoor>();
                test.MockBlocksOfType("test door", mockDoor);

                test.RunUntilDone();

                mockDoor.Verify(b => b.CloseDoor());
            }
        }

        [TestMethod]
        public void IsDoorOpen() {
            using (ScriptTest test = new ScriptTest(@"Print ""Door Open: "" + the ""test door"" is open")) {
                Mock<IMyDoor> mockDoor = new Mock<IMyDoor>();
                test.MockBlocksOfType("test door", mockDoor);
                mockDoor.Setup(b => b.Status).Returns(DoorStatus.Open);

                test.RunUntilDone();

                Assert.AreEqual("Door Open: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsDoorClosed() {
            using (ScriptTest test = new ScriptTest(@"Print ""Door Closed: "" + the ""test door"" is closed")) {
                Mock<IMyDoor> mockDoor = new Mock<IMyDoor>();
                test.MockBlocksOfType("test door", mockDoor);
                mockDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);

                test.RunUntilDone();

                Assert.AreEqual("Door Closed: True", test.Logger[0]);
            }
        }

        //A Closing door is considered open so that "when door is closed" will operate only when the door is finished closing
        [TestMethod]
        public void IsClosingDoorOpen() {
            using (ScriptTest test = new ScriptTest(@"Print ""Door Open: "" + the ""test door"" is open")) {
                Mock<IMyDoor> mockDoor = new Mock<IMyDoor>();
                test.MockBlocksOfType("test door", mockDoor);
                mockDoor.Setup(b => b.Status).Returns(DoorStatus.Closing);

                test.RunUntilDone();

                Assert.AreEqual("Door Open: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTheDoorRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Door Ratio: "" + the ""test door"" ratio")) {
                Mock<IMyDoor> mockDoor = new Mock<IMyDoor>();
                test.MockBlocksOfType("test door", mockDoor);
                mockDoor.Setup(b => b.OpenRatio).Returns(0.25f);

                test.RunUntilDone();

                Assert.AreEqual("Door Ratio: 0.25", test.Logger[0]);
            }
        }
    }
}
