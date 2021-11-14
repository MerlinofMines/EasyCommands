using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class RotorControlTests {
        string script = @"
:setup
assign desiredSecondsToLimit to 2.5
assign minRotorRPM to 0.02
assign maxRotorRPM to 3.5
assign myRotors to 'test rotors'
assign i to 0
until i >= count of $myRotors
  async call setVelocity i
  assign i to i + 1

:setVelocity ""index""
assign myVelocity to $myRotors @ index velocity
Print myRotors + "" "" + index + "": "" + myVelocity

if absolute myVelocity < 0.0001 goto setVelocity index

assign isNegative to myVelocity < 0
assign anglesPerSecond to 6 * myVelocity
assign currentAngle to $myRotors @ index angle

#Print ""IsNegative: "" + isNegative
#Print ""anglePerSecond: "" + anglesPerSecond
#Print ""currentAngle: "" + currentAngle

if isNegative
  assign rotorLimit to $myRotors @ index lower limit - currentAngle
else
  assign rotorLimit to $myRotors @ index upper limit - currentAngle

#Print ""rotorLimit: "" + rotorLimit

assign timeToLimit to rotorLimit / anglesPerSecond
assign newSpeed to ( timeToLimit * myVelocity ) / desiredSecondsToLimit

#Print ""timeToLimit: "" + timeToLimit
#Print ""desiredSecondsToLimit: "" + desiredSecondsToLimit
#Print ""newSpeed: "" + newSpeed

if isNegative
  if timeToLimit < desiredSecondsToLimit
    call ""min"" -1 * minRotorRPM newSpeed
    assign newSpeed to myOutput
  else
    call ""max"" -1 * maxRotorRPM newSpeed
    assign newSpeed to myOutput
else
  if timeToLimit < desiredSecondsToLimit
    call ""max"" minRotorRPM newSpeed
    assign newSpeed to myOutput
  else
    call ""min"" maxRotorRPM newSpeed
    assign newSpeed to myOutput

#Print ""Final New Speed: "" + newSpeed

set $myRotors @ index velocity to newSpeed

goto setVelocity index

:max a b
if a > b
  assign myOutput to a
else
  assign myOutput to b

:min a b
if a > b
  assign myOutput to b
else
  assign myOutput to a
";
        [TestMethod]
        public void RotorControlTestNoMovement() {
            using (ScriptTest test = new ScriptTest(script)) {

                var mockRotor0 = new Mock<IMyMotorStator>();
                var mockRotor1 = new Mock<IMyMotorStator>();
                MockBlockDefinition(mockRotor0, "LargeStator");
                MockBlockDefinition(mockRotor1, "LargeStator");

                mockRotor0.Setup(b => b.TargetVelocityRPM).Returns(0);
                mockRotor1.Setup(b => b.TargetVelocityRPM).Returns(0);

                test.MockBlocksInGroup("test rotors", mockRotor0, mockRotor1);

                test.RunOnce();
                test.RunOnce();

                //Index isn't populated.  why not.  need test for a while loop
                Assert.IsTrue(test.Logger.Contains("test rotors 0: 0"));
                Assert.IsTrue(test.Logger.Contains("test rotors 1: 0"));
            }
        }

        [TestMethod]
        public void RotorControlTestMoveRotor0Max() {
            using (ScriptTest test = new ScriptTest(script)) {

                var mockRotor0 = new Mock<IMyMotorStator>();
                var mockRotor1 = new Mock<IMyMotorStator>();
                MockBlockDefinition(mockRotor0, "LargeStator");
                MockBlockDefinition(mockRotor1, "LargeStator");

                mockRotor0.Setup(b => b.TargetVelocityRPM).Returns(1);
                mockRotor0.Setup(b => b.UpperLimitDeg).Returns(90);
                mockRotor0.Setup(b => b.Angle).Returns(0);
                mockRotor1.Setup(b => b.TargetVelocityRPM).Returns(0);

                test.MockBlocksInGroup("test rotors", mockRotor0, mockRotor1);

                test.RunOnce();
                test.RunOnce();

                mockRotor0.VerifySet(b => b.TargetVelocityRPM = 3.5f);
            }
        }

        [TestMethod]
        public void RotorControlTestMoveRotor0Min() {
            using (ScriptTest test = new ScriptTest(script)) {

                var mockRotor0 = new Mock<IMyMotorStator>();
                var mockRotor1 = new Mock<IMyMotorStator>();
                MockBlockDefinition(mockRotor0, "LargeStator");
                MockBlockDefinition(mockRotor1, "LargeStator");

                mockRotor0.Setup(b => b.TargetVelocityRPM).Returns(1);
                mockRotor0.Setup(b => b.UpperLimitDeg).Returns(90);
                mockRotor0.Setup(b => b.Angle).Returns((float)(89.9 * Math.PI / 180));
                mockRotor1.Setup(b => b.TargetVelocityRPM).Returns(0);

                test.MockBlocksInGroup("test rotors", mockRotor0, mockRotor1);

                test.RunOnce();
                test.RunOnce();

                mockRotor0.VerifySet(b => b.TargetVelocityRPM = 0.02f);
            }
        }

        [TestMethod]
        public void RotorControlTestMoveRotor0Middle() {
            using (ScriptTest test = new ScriptTest(script)) {

                var mockRotor0 = new Mock<IMyMotorStator>();
                var mockRotor1 = new Mock<IMyMotorStator>();
                MockBlockDefinition(mockRotor0, "LargeStator");
                MockBlockDefinition(mockRotor1, "LargeStator");

                mockRotor0.Setup(b => b.TargetVelocityRPM).Returns(1);
                mockRotor0.Setup(b => b.UpperLimitDeg).Returns(90);
                mockRotor0.Setup(b => b.Angle).Returns((float)(60 * Math.PI / 180));
                mockRotor1.Setup(b => b.TargetVelocityRPM).Returns(0);

                test.MockBlocksInGroup("test rotors", mockRotor0, mockRotor1);

                test.RunOnce();
                test.RunOnce();

                mockRotor0.VerifySet(b => b.TargetVelocityRPM = 1.99999976f);
            }
        }

        [TestMethod]
        public void RotorControlTestMoveRotor0MaxNegative() {
            using (ScriptTest test = new ScriptTest(script)) {

                var mockRotor0 = new Mock<IMyMotorStator>();
                var mockRotor1 = new Mock<IMyMotorStator>();
                MockBlockDefinition(mockRotor0, "LargeStator");
                MockBlockDefinition(mockRotor1, "LargeStator");

                mockRotor0.Setup(b => b.TargetVelocityRPM).Returns(-1);
                mockRotor0.Setup(b => b.LowerLimitDeg).Returns(0);
                mockRotor0.Setup(b => b.Angle).Returns((float)(90 * Math.PI / 180));
                mockRotor1.Setup(b => b.TargetVelocityRPM).Returns(0);

                test.MockBlocksInGroup("test rotors", mockRotor0, mockRotor1);

                test.RunOnce();
                test.RunOnce();

                mockRotor0.VerifySet(b => b.TargetVelocityRPM = -3.5f);
            }
        }

        [TestMethod]
        public void RotorControlTestMoveRotor0MinNegative() {
            using (ScriptTest test = new ScriptTest(script)) {

                var mockRotor0 = new Mock<IMyMotorStator>();
                var mockRotor1 = new Mock<IMyMotorStator>();
                MockBlockDefinition(mockRotor0, "LargeStator");
                MockBlockDefinition(mockRotor1, "LargeStator");

                mockRotor0.Setup(b => b.TargetVelocityRPM).Returns(-1);
                mockRotor0.Setup(b => b.LowerLimitDeg).Returns(00);
                mockRotor0.Setup(b => b.Angle).Returns((float)(0.1 * Math.PI / 180));
                mockRotor1.Setup(b => b.TargetVelocityRPM).Returns(0);

                test.MockBlocksInGroup("test rotors", mockRotor0, mockRotor1);

                test.RunOnce();
                test.RunOnce();

                mockRotor0.VerifySet(b => b.TargetVelocityRPM = -0.02f);
            }
        }

        [TestMethod]
        public void RotorControlTestMoveRotor0MiddleNegative() {
            using (ScriptTest test = new ScriptTest(script)) {

                var mockRotor0 = new Mock<IMyMotorStator>();
                var mockRotor1 = new Mock<IMyMotorStator>();
                MockBlockDefinition(mockRotor0, "LargeStator");
                MockBlockDefinition(mockRotor1, "LargeStator");

                mockRotor0.Setup(b => b.TargetVelocityRPM).Returns(-1);
                mockRotor0.Setup(b => b.LowerLimitDeg).Returns(0);
                mockRotor0.Setup(b => b.Angle).Returns((float)(30 * Math.PI / 180));
                mockRotor1.Setup(b => b.TargetVelocityRPM).Returns(0);

                test.MockBlocksInGroup("test rotors", mockRotor0, mockRotor1);

                test.RunOnce();
                test.RunOnce();

                mockRotor0.VerifySet(b => b.TargetVelocityRPM = -2.00000024f);
            }
        }

        [TestMethod]
        public void RotorControlTestMoveRotor1Max() {
            using (ScriptTest test = new ScriptTest(script)) {

                var mockRotor0 = new Mock<IMyMotorStator>();
                var mockRotor1 = new Mock<IMyMotorStator>();
                MockBlockDefinition(mockRotor0, "LargeStator");
                MockBlockDefinition(mockRotor1, "LargeStator");

                mockRotor0.Setup(b => b.TargetVelocityRPM).Returns(0);
                mockRotor1.Setup(b => b.TargetVelocityRPM).Returns(1);
                mockRotor1.Setup(b => b.UpperLimitDeg).Returns(90);
                mockRotor1.Setup(b => b.Angle).Returns(0);

                test.MockBlocksInGroup("test rotors", mockRotor0, mockRotor1);

                test.RunOnce();
                test.RunOnce();

                mockRotor1.VerifySet(b => b.TargetVelocityRPM = 3.5f);
            }
        }
    }
}
