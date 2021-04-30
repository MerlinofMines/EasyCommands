using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class AirLockManagerTests {
        String script = @"
:setup
assign exteriorDoors to 'Exterior Doors'
assign interiorDoors to 'Interior Doors'
assign airlockVent to 'Air Lock Vent'
assign airlockSensor to 'Air Lock Sensor'
async goto runAirLock

:runAirLock
if [airlockSensor] sensor is triggered
  call ""goOut""
else
  call ""comeIn""
replay

:goOut
if [interiorDoors] doors are open
  close the [interiorDoors] doors
else if [airlockVent] vent ratio > 0.1
  depressurize the [airlockVent] vent
else
  open the [exteriorDoors] doors

:comeIn
if [exteriorDoors] doors are open
  close the [exteriorDoors] doors
else if [airlockVent] vent ratio < 0.99
  pressurize the [airlockVent] vent
else
  open the [interiorDoors] doors
";

        [TestMethod]
        public void SensorTriggeredOpensTheOuterDoor() {
            using (var test = new ScriptTest(script)) {
                var mockSensor = new Mock<IMySensorBlock>();
                var mockExteriorDoor = new Mock<IMyDoor>();
                var mockInteriorDoor = new Mock<IMyDoor>();
                var mockAirVent = new Mock<IMyAirVent>();

                test.MockBlocksOfType("Air Lock Sensor", mockSensor);
                test.MockBlocksInGroup("Exterior Doors", mockExteriorDoor);
                test.MockBlocksInGroup("Interior Doors", mockInteriorDoor);
                test.MockBlocksOfType("Air Lock Vent", mockAirVent);

                //Setup, nothing happens first tick
                test.RunOnce();

                //Close Inner Doors
                mockSensor.Setup(b => b.IsActive).Returns(true);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Open);
                test.RunOnce(); 
                mockInteriorDoor.Verify(b => b.CloseDoor());
                mockSensor.Verify(b => b.IsActive);

                //Wait For Inner Doors To Close
                mockSensor.Setup(b => b.IsActive).Returns(true);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closing);
                test.RunOnce();
                mockInteriorDoor.Verify(b => b.CloseDoor());
                mockSensor.Verify(b => b.IsActive);

                //Depressurize the Air Vent
                mockSensor.Setup(b => b.IsActive).Returns(true);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(1);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockAirVent.VerifySet(b => b.Depressurize = true);

                //Wait for the Air Vent to Depressurize
                mockSensor.Setup(b => b.IsActive).Returns(true);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0.5f);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockAirVent.VerifySet(b => b.Depressurize = true);

                //Open The Outer Doors
                mockSensor.Setup(b => b.IsActive).Returns(true);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockExteriorDoor.Verify(b => b.OpenDoor());

                //Wait For Outer Doors To Open
                mockSensor.Setup(b => b.IsActive).Returns(true);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Opening);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockExteriorDoor.Verify(b => b.OpenDoor());

                //Complete still opens doors
                mockSensor.Setup(b => b.IsActive).Returns(true);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Open);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockExteriorDoor.Verify(b => b.OpenDoor());
            }
        }

        [TestMethod]
        public void SensorNotTriggeredClosesTheOuterDoor() {
            using (var test = new ScriptTest(script)) {
                var mockSensor = new Mock<IMySensorBlock>();
                var mockExteriorDoor = new Mock<IMyDoor>();
                var mockInteriorDoor = new Mock<IMyDoor>();
                var mockAirVent = new Mock<IMyAirVent>();

                test.MockBlocksOfType("Air Lock Sensor", mockSensor);
                test.MockBlocksInGroup("Exterior Doors", mockExteriorDoor);
                test.MockBlocksInGroup("Interior Doors", mockInteriorDoor);
                test.MockBlocksOfType("Air Lock Vent", mockAirVent);

                //Setup, nothing happens first tick
                test.RunOnce();

                //Close Outer Doors
                mockSensor.Setup(b => b.IsActive).Returns(false);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Open);
                test.RunOnce();
                mockExteriorDoor.Verify(b => b.CloseDoor());
                mockSensor.Verify(b => b.IsActive);

                //Wait For Inner Doors To Close
                mockSensor.Setup(b => b.IsActive).Returns(false);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closing);
                test.RunOnce();
                mockExteriorDoor.Verify(b => b.CloseDoor());
                mockSensor.Verify(b => b.IsActive);

                //Pressurize the Air Vent
                mockSensor.Setup(b => b.IsActive).Returns(false);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockAirVent.VerifySet(b => b.Depressurize = false);

                //Wait for the Air Vent to Pressurize
                mockSensor.Setup(b => b.IsActive).Returns(false);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(0.5f);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockAirVent.VerifySet(b => b.Depressurize = false);

                //Open The Inner Doors
                mockSensor.Setup(b => b.IsActive).Returns(false);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(1);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockInteriorDoor.Verify(b => b.OpenDoor());

                //Wait For Inner Doors To Open
                mockSensor.Setup(b => b.IsActive).Returns(false);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(1);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Opening);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockInteriorDoor.Verify(b => b.OpenDoor());

                //Complete still opens doors
                mockSensor.Setup(b => b.IsActive).Returns(false);
                mockExteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Closed);
                mockAirVent.Setup(b => b.GetOxygenLevel()).Returns(1);
                mockInteriorDoor.Setup(b => b.Status).Returns(DoorStatus.Open);
                test.RunOnce();
                mockSensor.Verify(b => b.IsActive);
                mockInteriorDoor.Verify(b => b.OpenDoor());
            }
        }
    }
}
