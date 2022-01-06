﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class RocketVolleyTests {
        [TestMethod]
        public void RocketVolleyTest() {
            String script = @"
:setup
assign global rocketGroup to 'Rockets'
assign global fireTickInterval to 1

#Background Firing Loop
async call ""fireLoop""
call ""stopFiring""

:toggle
assign global isFiring to not isFiring
assign global i to 0

:startFiring
assign global isFiring to true
assign global i to 0

:stopFiring
tell $rocketGroup rockets to not shoot
assign global isFiring to false

:fireLoop
if not isFiring replay
print ""i = "" + i
if i >= count of $rocketGroup rockets
  assign global i to 0
tell $rocketGroup rockets @ i to shoot
tell $rocketGroup rockets to not shoot
wait fireTickInterval ticks
i++
goto ""fireLoop""
";
            using (var test = new ScriptTest(script)) {
                var mockRocket1 = new Mock<IMyUserControllableGun>();
                var mockRocket2 = new Mock<IMyUserControllableGun>();

                var rocket1Off = MockAction(mockRocket1, "Shoot_Off");
                var rocket1On = MockAction(mockRocket1, "Shoot_On");
                var rocket2Off = MockAction(mockRocket2, "Shoot_Off");
                var rocket2On = MockAction(mockRocket2, "Shoot_On");

                test.MockBlocksInGroup("Rockets", mockRocket1, mockRocket2);

                test.RunOnce(); //Setup but no firing yet

                rocket1Off.Verify(b => b.Apply(mockRocket1.Object));
                rocket2Off.Verify(b => b.Apply(mockRocket2.Object));
                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();

                test.RunWithArgument("call startFiring"); //Fire Rocket 1

                rocket1Off.Verify(b => b.Apply(mockRocket1.Object));
                rocket2Off.Verify(b => b.Apply(mockRocket2.Object));
                rocket1On.Verify(b => b.Apply(mockRocket1.Object));
                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();

                test.RunOnce(); //Wait 1 tick

                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();

                test.RunOnce(); //Fire Rocket 2

                rocket1Off.Verify(b => b.Apply(mockRocket1.Object));
                rocket2Off.Verify(b => b.Apply(mockRocket2.Object));
                rocket2On.Verify(b => b.Apply(mockRocket2.Object));
                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();

                test.RunOnce(); //Wait 1 tick

                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();

                //Should restart the loop!
                test.RunOnce(); //Fire Rocket 1

                rocket1Off.Verify(b => b.Apply(mockRocket1.Object));
                rocket2Off.Verify(b => b.Apply(mockRocket2.Object));
                rocket1On.Verify(b => b.Apply(mockRocket1.Object));
                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();

                test.RunOnce(); //Wait 1 tick

                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();

                test.RunOnce(); //Fire Rocket 2

                rocket1Off.Verify(b => b.Apply(mockRocket1.Object));
                rocket2Off.Verify(b => b.Apply(mockRocket2.Object));
                rocket2On.Verify(b => b.Apply(mockRocket2.Object));
                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();

                test.RunOnce(); //Wait 1 tick

                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();

                test.RunWithArgument("call stopFiring"); //Fire Rocket 1

                rocket1Off.Verify(b => b.Apply(mockRocket1.Object));
                rocket2Off.Verify(b => b.Apply(mockRocket2.Object));
                rocket1Off.VerifyNoOtherCalls();
                rocket2Off.VerifyNoOtherCalls();
                rocket1On.VerifyNoOtherCalls();
                rocket2On.VerifyNoOtherCalls();
            }
        }
    }
}
