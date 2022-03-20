using System;
using System.Collections.Generic;
using IngameScript;
using Malware.MDKUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class InterruptableCommandTests {

        [TestMethod]
        public void exitHaltsExecutionAndRunningAgainRestarts() {
            String script = @"
:main
#This is a comment
print 'Hello World'
exit
print 'Hello Again'
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilState(Program.ProgramState.STOPPED);
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                test.Logger.Clear();
                test.RunUntilState(Program.ProgramState.STOPPED);
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void restartImmediatelyRestarts() {
            String script = @"
:main
#This is a comment
print 'Hello World'
restart
print 'Hello Again'
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                Assert.AreEqual(Program.ProgramState.RUNNING, test.program.state);
                test.Logger.Clear();
                test.RunOnce();
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                Assert.AreEqual(Program.ProgramState.RUNNING, test.program.state);
            }
        }

        [TestMethod]
        public void pauseContinuesExecutionAfterResuming() {
            String script = @"
:main
#This is a comment
print 'Hello World'
pause
print 'Hello Again'
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilState(Program.ProgramState.PAUSED);
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                test.Logger.Clear();
                test.RunUntilDone();
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello Again", test.Logger[0]);
            }
        }

        [TestMethod]
        public void repeatRepeatsMainFunctionIfNoGoto() {
            String script = @"
:main
#This is a comment
print 'Hello World'
printAgain

:printAgain
print 'Hello Again'
repeat
print 'Not Printed'
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();
                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                Assert.AreEqual("Hello Again", test.Logger[1]);
                Assert.AreEqual(Program.ProgramState.RUNNING, test.program.state);
                test.Logger.Clear();
                test.RunOnce();
                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                Assert.AreEqual("Hello Again", test.Logger[1]);
                Assert.AreEqual(Program.ProgramState.RUNNING, test.program.state);
            }
        }

        [TestMethod]
        public void repeatRepeatsFromLastGotoFunction() {
            String script = @"
:main
#This is a comment
print 'Hello World'
goto printMe

:printMe
print 'Print Me'
goto printAgain

:printAgain
print 'Hello Again'
repeat
print 'Not Printed'
";

            using (var test = new ScriptTest(script)) {
                test.RunIterations(3);
                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                Assert.AreEqual("Print Me", test.Logger[1]);
                Assert.AreEqual("Hello Again", test.Logger[2]);
                Assert.AreEqual(Program.ProgramState.RUNNING, test.program.state);
                test.Logger.Clear();
                test.RunOnce();
                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello Again", test.Logger[0]);
                Assert.AreEqual(Program.ProgramState.RUNNING, test.program.state);
            }
        }

        [TestMethod]
        public void incorrectBreakUsage() {
            String script = @"
:main
set j to 0
set i to 1
break
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Invalid use of break command", test.Logger[1]);
            }
        }

        [TestMethod]
        public void incorrectContinueUsage() {
            String script = @"
:main
set j to 0
set i to 1
continue
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Invalid use of continue command", test.Logger[1]);
            }
        }

        [TestMethod]
        public void returnOutsideOfCalledFunctionEndsCurrentThreadAndContinues() {
            String script = @"
:main
queue
  print ""Done 1""
  return
  print ""Not Printed""
queue
  print ""Done 2""
  return
  print ""Not Printed""
print ""Done Main""
return
print ""Not Printed""
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Done Main", test.Logger[0]);
                Assert.AreEqual("Done 1", test.Logger[1]);
                Assert.AreEqual("Done 2", test.Logger[2]);
            }
        }

        [TestMethod]
        public void returnFromSwitchedToFunctionEndsCurrentThreadAndContinues() {
            String script = @"
:main
queue
  print ""Done 1""
  return
  print ""Not Printed""
goto switchFunction
print ""Not Printed""

:switchFunction
print ""Done Switched""
return
print ""Not Printed""
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Done Switched", test.Logger[0]);
                Assert.AreEqual("Done 1", test.Logger[1]);
            }
        }

        [TestMethod]
        public void breakConditionalCommand() {
            String script = @"
:main
set j to 0
set i to 1
until i > 10
  Print ""My Item: "" + i
  if i > 5
    break
    j++
  i++
Print ""i is: "" + i
Print ""j is: "" + j
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Item: 1", test.Logger[0]);
                Assert.AreEqual("My Item: 2", test.Logger[1]);
                Assert.AreEqual("My Item: 3", test.Logger[2]);
                Assert.AreEqual("My Item: 4", test.Logger[3]);
                Assert.AreEqual("My Item: 5", test.Logger[4]);
                Assert.AreEqual("My Item: 6", test.Logger[5]);
                //Make sure i++ and j++ aren't executed after break
                Assert.AreEqual("i is: 6", test.Logger[6]);
                Assert.AreEqual("j is: 0", test.Logger[7]);
            }
        }

        [TestMethod]
        public void continueConditionalCommand() {
            String script = @"
:main
set i to 0
set j to 0
until i > 10
  i++
  if i % 2 is 0
    continue
    j++
  Print ""My Item: "" + i
Print ""i is: "" + i
Print ""j is: "" + j
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Item: 1", test.Logger[0]);
                Assert.AreEqual("My Item: 3", test.Logger[1]);
                Assert.AreEqual("My Item: 5", test.Logger[2]);
                Assert.AreEqual("My Item: 7", test.Logger[3]);
                Assert.AreEqual("My Item: 9", test.Logger[4]);
                Assert.AreEqual("My Item: 11", test.Logger[5]);
                Assert.AreEqual("i is: 11", test.Logger[6]);
                Assert.AreEqual("j is: 0", test.Logger[7]);
            }
        }

        [TestMethod]
        public void iterateOverForEachCommandAndBreak() {
            String script = @"
:main
set j to 0
assign myList to [1,2,3]

iterate myList 3 times
print ""j is: "" + j

:iterate myList
for each item in myList
  print ""My Item: "" + item
  if item >= 2
    break
    j++
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Item: 1", test.Logger[0]);
                Assert.AreEqual("My Item: 2", test.Logger[1]);
                Assert.AreEqual("My Item: 1", test.Logger[2]);
                Assert.AreEqual("My Item: 2", test.Logger[3]);
                Assert.AreEqual("My Item: 1", test.Logger[4]);
                Assert.AreEqual("My Item: 2", test.Logger[5]);
                Assert.AreEqual("j is: 0", test.Logger[6]);
            }
        }

        [TestMethod]
        public void iterateOverForEachCommandAndContinue() {
            String script = @"
:main
assign myList to [1,2,3]
set j to 0

iterate myList 3 times
Print ""j is: "" + j

:iterate myList
for each item in myList
  if item is 2
    continue
    j++
  print ""My Item: "" + item
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Item: 1", test.Logger[0]);
                Assert.AreEqual("My Item: 3", test.Logger[1]);
                Assert.AreEqual("My Item: 1", test.Logger[2]);
                Assert.AreEqual("My Item: 3", test.Logger[3]);
                Assert.AreEqual("My Item: 1", test.Logger[4]);
                Assert.AreEqual("My Item: 3", test.Logger[5]);
                Assert.AreEqual("j is: 0", test.Logger[6]);
            }
        }

        [TestMethod]
        public void continueConditionalCommandInsideConditionalCommandOnlyAffectsInnerCommand() {
            String script = @"
:main
set i to 0
until i > 5
  i++
  if i % 2 is 0
    continue
  Print ""i is: "" + i

  set k to 0
  until k > 5
    k++
    if k % 2 is 0
      continue
    Print ""k is: "" + k
Print ""Done""
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("i is: 1", test.Logger[0]);
                Assert.AreEqual("k is: 1", test.Logger[1]);
                Assert.AreEqual("k is: 3", test.Logger[2]);
                Assert.AreEqual("k is: 5", test.Logger[3]);
                Assert.AreEqual("i is: 3", test.Logger[4]);
                Assert.AreEqual("k is: 1", test.Logger[5]);
                Assert.AreEqual("k is: 3", test.Logger[6]);
                Assert.AreEqual("k is: 5", test.Logger[7]);
                Assert.AreEqual("i is: 5", test.Logger[8]);
                Assert.AreEqual("k is: 1", test.Logger[9]);
                Assert.AreEqual("k is: 3", test.Logger[10]);
                Assert.AreEqual("k is: 5", test.Logger[11]);
                Assert.AreEqual("Done", test.Logger[12]);
            }
        }

        [TestMethod]
        public void breakConditionalCommandInForEachOnlyAffectsInner() {
            String script = @"
:main
set myList to [1,2,3]
for each item in myList
  set i to 0
  while i < 3
    if item % 2 is 0
      break
    Print ""i is: "" + i
    i++
  print ""item is: "" + item
print ""Done""
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("i is: 0", test.Logger[0]);
                Assert.AreEqual("i is: 1", test.Logger[1]);
                Assert.AreEqual("i is: 2", test.Logger[2]);
                Assert.AreEqual("item is: 1", test.Logger[3]);
                Assert.AreEqual("item is: 2", test.Logger[4]);
                Assert.AreEqual("i is: 0", test.Logger[5]);
                Assert.AreEqual("i is: 1", test.Logger[6]);
                Assert.AreEqual("i is: 2", test.Logger[7]);
                Assert.AreEqual("item is: 3", test.Logger[8]);
                Assert.AreEqual("Done", test.Logger[9]);
            }
        }

        [TestMethod]
        public void breakForEachCommandInForEachOnlyAffectsInner() {
            String script = @"
:main
set myList to [1,2,3]
for each item in myList
  set i to 0
  for each i in myList
    if item % 2 is 0
      break
    Print ""i is: "" + i
  print ""item is: "" + item
print ""Done""
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("i is: 1", test.Logger[0]);
                Assert.AreEqual("i is: 2", test.Logger[1]);
                Assert.AreEqual("i is: 3", test.Logger[2]);
                Assert.AreEqual("item is: 1", test.Logger[3]);
                Assert.AreEqual("item is: 2", test.Logger[4]);
                Assert.AreEqual("i is: 1", test.Logger[5]);
                Assert.AreEqual("i is: 2", test.Logger[6]);
                Assert.AreEqual("i is: 3", test.Logger[7]);
                Assert.AreEqual("item is: 3", test.Logger[8]);
                Assert.AreEqual("Done", test.Logger[9]);
            }
        }

        [TestMethod]
        public void breakForEachCommandInConditionalCommandOnlyAffectsInner() {
            String script = @"
:main
set myList to [1,2,3]
set item to 1
while item < 4
  set i to 0
  for each i in myList
    if item % 2 is 0
      break
    Print ""i is: "" + i
  print ""item is: "" + item
  item++
print ""Done""
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("i is: 1", test.Logger[0]);
                Assert.AreEqual("i is: 2", test.Logger[1]);
                Assert.AreEqual("i is: 3", test.Logger[2]);
                Assert.AreEqual("item is: 1", test.Logger[3]);
                Assert.AreEqual("item is: 2", test.Logger[4]);
                Assert.AreEqual("i is: 1", test.Logger[5]);
                Assert.AreEqual("i is: 2", test.Logger[6]);
                Assert.AreEqual("i is: 3", test.Logger[7]);
                Assert.AreEqual("item is: 3", test.Logger[8]);
                Assert.AreEqual("Done", test.Logger[9]);
            }
        }

        [TestMethod]
        public void returnFromMainExitsScript() {
            String script = @"
:main
print ""Done""
return
print ""Not Printed""
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Done", test.Logger[0]);

                test.Logger.Clear();
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Done", test.Logger[0]);
            }
        }

        [TestMethod]
        public void returnFromFunctionReturnsAndContinues() {
            String script = @"
:main
printValue ""yes""
printValue ""no""
printValue ""yes again""
print ""Done""

:printValue myValue
if myValue is ""no""
  return
  print ""Not Printed""
print ""My Value: "" + myValue
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Value: yes", test.Logger[0]);
                Assert.AreEqual("My Value: yes again", test.Logger[1]);
                Assert.AreEqual("Done", test.Logger[2]);
            }
        }

        [TestMethod]
        public void returnOnlyAffectsInnerMostFunction() {
            String script = @"
:main
printValues
print ""Done""

:printValues
printValue ""yes""
printValue ""no""
printValue ""yes again""

:printValue myValue
if myValue is ""no""
  return
  print ""Not Printed""
print ""My Value: "" + myValue
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Value: yes", test.Logger[0]);
                Assert.AreEqual("My Value: yes again", test.Logger[1]);
                Assert.AreEqual("Done", test.Logger[2]);
            }
        }
    }
}
