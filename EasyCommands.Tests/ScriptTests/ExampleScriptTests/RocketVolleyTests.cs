using Microsoft.VisualStudio.TestTools.UnitTesting;
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

                test.MockBlocksInGroup("Rockets", mockRocket1, mockRocket2);

                test.RunOnce(); //Setup but no firing yet

                mockRocket1.VerifySet(b => b.Shoot = false);
                mockRocket2.VerifySet(b => b.Shoot = false);
                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();

                test.RunWithArgument("call startFiring"); //Fire Rocket 1

                mockRocket1.VerifySet(b => b.Shoot = false);
                mockRocket2.VerifySet(b => b.Shoot = false);
                mockRocket1.VerifySet(b => b.Shoot = true);
                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();

                test.RunOnce(); //Wait 1 tick

                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();

                test.RunOnce(); //Fire Rocket 2

                mockRocket1.VerifySet(b => b.Shoot = false);
                mockRocket2.VerifySet(b => b.Shoot = false);
                mockRocket2.VerifySet(b => b.Shoot = true);
                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();

                test.RunOnce(); //Wait 1 tick

                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();

                //Should restart the loop!
                test.RunOnce(); //Fire Rocket 1

                mockRocket1.VerifySet(b => b.Shoot = false);
                mockRocket2.VerifySet(b => b.Shoot = false);
                mockRocket1.VerifySet(b => b.Shoot = true);
                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();

                test.RunOnce(); //Wait 1 tick

                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();

                test.RunOnce(); //Fire Rocket 2

                mockRocket1.VerifySet(b => b.Shoot = false);
                mockRocket2.VerifySet(b => b.Shoot = false);
                mockRocket2.VerifySet(b => b.Shoot = true);
                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();

                test.RunOnce(); //Wait 1 tick

                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();

                test.RunWithArgument("call stopFiring"); //Fire Rocket 1

                mockRocket1.VerifySet(b => b.Shoot = false);
                mockRocket2.VerifySet(b => b.Shoot = false);
                mockRocket1.VerifyNoOtherCalls();
                mockRocket2.VerifyNoOtherCalls();
            }
        }
    }
}
