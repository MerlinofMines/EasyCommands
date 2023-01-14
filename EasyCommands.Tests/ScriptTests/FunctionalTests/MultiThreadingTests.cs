using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static IngameScript.Program;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class MultiThreadingTests {
        [TestMethod]
        public void finishedCommandsGetProcessedInSameTick() {
            String script = @"
:main
print 'Hello World'
print 'I Got Printed'
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                Assert.AreEqual("I Got Printed", test.Logger[1]);
            }
        }

        [TestMethod]
        public void blockingCommandsAreHonored() {
            String script = @"
:main
print 'Hello World'
wait
print 'I Got Printed'
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);

                test.RunOnce();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("I Got Printed", test.Logger[1]);
            }
        }

        [TestMethod]
        public void runArgumentIsQueuedAtFront() {
            String script = @"
:main
print 'Main'
";

            using (var test = new ScriptTest(script)) {
                test.RunWithArgument("print 'Message'");

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Message", test.Logger[0]);

                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Main", test.Logger[1]);
            }
        }

        [TestMethod]
        public void queueCommandIsQueuedAtBack() {
            String script = @"
:main
queue print 'Queued'
print 'Main'
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Main", test.Logger[0]);

                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Queued", test.Logger[1]);
            }
        }

        [TestMethod]
        public void asyncThreadsAreAllExecuted() {
            String script = @"
:main
async call runAsync
async call runAsync
async call runAsync
print 'Main'

:runAsync
wait
print 'Async'
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();//Execute main, add async threads and execute wait in all 3

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Main", test.Logger[0]);

                test.RunOnce();//Execute print in all async threads

                Assert.AreEqual(4, test.Logger.Count);
                Assert.AreEqual("Async", test.Logger[1]);
                Assert.AreEqual("Async", test.Logger[2]);
                Assert.AreEqual("Async", test.Logger[3]);
            }
        }

        [TestMethod]
        public void programRunsUntilAsyncMethodsComplete() {
            String script = @"
:main
async call runAsync
print 'Main'

:runAsync
wait 10 ticks
print 'Async'
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();//Execute main, add async thread

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Main", test.Logger[0]);

                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Async", test.Logger[1]);
            }
        }

        [TestMethod]
        public void asyncCommandGotoStaysInSameThread() {
            String script = @"
:main
async call runAsync
async call runAsync
print 'Main'
wait
print 'Still Main'

:runAsync
wait
print 'Async'
goto ""stillInThread""

:stillInThread
print 'Still Async'
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();//Execute main, add async threads which wait 1 turn

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Main", test.Logger[0]);

                test.RunOnce();//Execute print in both async threads. execute goto in both async threads

                Assert.AreEqual(4, test.Logger.Count);
                Assert.AreEqual("Still Main", test.Logger[1]);
                Assert.AreEqual("Async", test.Logger[2]);
                Assert.AreEqual("Async", test.Logger[3]);

                test.RunUntilDone(); //Execute stillInThread in both async threads, which should complete and terminate.

                Assert.AreEqual(6, test.Logger.Count);
                Assert.AreEqual("Still Async", test.Logger[4]);
                Assert.AreEqual("Still Async", test.Logger[5]);
            }
        }

        [TestMethod]
        public void asyncCommandVariablesAreThreadLocal() {
            String script = @"
:main
async call printLocalVariable 1
async call printLocalVariable 2

:printLocalVariable ""a""
print 'Variable is: ' + a
assign a to a + 2
goto printLocalVariable a
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();//Execute main, add async threads which set their variable and print
                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Variable is: 1", test.Logger[0]);
                Assert.AreEqual("Variable is: 2", test.Logger[1]);

                test.Logger.Clear();
                test.RunOnce();//Executing async threads again, expect variables are incremented independently
                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Variable is: 3", test.Logger[0]);
                Assert.AreEqual("Variable is: 4", test.Logger[1]);
            }
        }

        [TestMethod]
        public void asyncCommandsShareGlobalVariables() {
            String script = @"
:main
assign global a to 1
async call printGlobalVariable
async call printGlobalVariable

:printGlobalVariable
print 'Variable is: ' + a
assign global a to a + 2
goto printGlobalVariable
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();//Execute main, add async threads which set their variable and print
                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Variable is: 1", test.Logger[0]);
                Assert.AreEqual("Variable is: 3", test.Logger[1]);

                test.Logger.Clear();
                test.RunOnce();//Executing async threads again, expect global variable is incremented cumulatively
                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Variable is: 5", test.Logger[0]);
                Assert.AreEqual("Variable is: 7", test.Logger[1]);
            }
        }

        [TestMethod]
        public void asyncCommandLocalVariablesTakePrecedenceOverGlobalVariables() {
            String script = @"
:main
assign global a to 1
async call printVariable
async call printVariable
call ""printGlobalVariable""

:printGlobalVariable
Print ""Global Variable is: "" + a
goto ""printGlobalVariable""

:printVariable
print 'Variable is: ' + a
assign a to a + 2
goto printVariable
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();//Execute main, add async threads which set their variable and print
                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Global Variable is: 1", test.Logger[0]);
                Assert.AreEqual("Variable is: 1", test.Logger[1]);
                Assert.AreEqual("Variable is: 1", test.Logger[2]);

                test.Logger.Clear();
                test.RunOnce();//Executing async threads again, expect variables are incremented independently
                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Global Variable is: 1", test.Logger[0]);
                Assert.AreEqual("Variable is: 3", test.Logger[1]);
                Assert.AreEqual("Variable is: 3", test.Logger[2]);
            }
        }

        [TestMethod]
        public void asyncCommandVariablesArePassedToAsyncThread() {
            String script = @"
:main
assign i to 0
async call printLocalVariable i
assign i to i + 1
async call printLocalVariable i
assign i to i + 1

:printLocalVariable ""a""
print 'Variable is: ' + a
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();//Execute main, add async threads which set their variable and print
                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Variable is: 0", test.Logger[0]);
                Assert.AreEqual("Variable is: 1", test.Logger[1]);
            }
        }

        [TestMethod]
        public void asyncCommandParametersAreProperlySetInAWhileLoop() {
            String script = @"
:main
assign i to 0
while i < 2
  async call printLocalVariable i
  assign i to i + 1

:printLocalVariable a
print 'Variable is: ' + a
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();
                Assert.IsTrue(test.Logger.Contains("Variable is: 0"));
                Assert.IsTrue(test.Logger.Contains("Variable is: 1"));
            }
        }

        [TestMethod]
        public void threadNamesAreProperlySet() {
            String script = @"
:main
async call runAsync
print 'Main'

:runAsync
wait
print 'Async'

:handleMessage
Print ""Message""
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = LogLevel.INFO;
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("[Main] main"));
                Assert.IsTrue(test.Logger.Contains("[Async] runAsync"));

                test.program.Main("call handleMessage");

                Assert.IsTrue(test.Logger.Contains("[Request] call handleMessage"));
            }
        }

        [TestMethod]
        public void asyncVariableIsNotAffectedByMainThread() {
            String script = @"
:main
set a to 1
async call runAsync
set a to 2

:runAsync
print 'Variable is: ' + a
wait 1
print 'Variable is: ' + a
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Variable is: 1", test.Logger[0]);
                Assert.AreEqual("Variable is: 1", test.Logger[1]);
            }
        }

        [TestMethod]
        public void awaitCommandExecutesButBlocksOnAsyncThreads() {
            String script = @"
:main
await
  async
    Print ""Async Thread""
    wait 1
    Print ""Async Thread Done""
  print ""Main Thread""
  async
    Print ""Async Thread 2""
    wait 0.5
    Print ""Async Thread 2 Done""
print ""Main Thread Done""
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(6, test.Logger.Count);
                Assert.AreEqual("Main Thread", test.Logger[0]);
                Assert.AreEqual("Async Thread", test.Logger[1]);
                Assert.AreEqual("Async Thread 2", test.Logger[2]);
                Assert.AreEqual("Async Thread 2 Done", test.Logger[3]);
                Assert.AreEqual("Async Thread Done", test.Logger[4]);
                Assert.AreEqual("Main Thread Done", test.Logger[5]);
            }
        }

        [TestMethod]
        public void awaitCommandWaitsForAsyncThreadSpawnedInSubFunction() {
            String script = @"
:main
await
  callAsync ""Async Thread"" 1
  print ""Main Thread""
  callAsync ""Async Thread 2"" 0.5
print ""Main Thread Done""

:callAsync threadName timeout
async
  Print threadName
  wait timeout
  Print threadName + "" Done""
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(6, test.Logger.Count);
                Assert.AreEqual("Main Thread", test.Logger[0]);
                Assert.AreEqual("Async Thread", test.Logger[1]);
                Assert.AreEqual("Async Thread 2", test.Logger[2]);
                Assert.AreEqual("Async Thread 2 Done", test.Logger[3]);
                Assert.AreEqual("Async Thread Done", test.Logger[4]);
                Assert.AreEqual("Main Thread Done", test.Logger[5]);
            }
        }

        [TestMethod]
        public void nestedAwaitCommandsProperlyBlocks() {
            String script = @"
:main
await
  callAsync ""Async Thread 2"" 1
  await 
    callAsync ""Async Thread"" 0.5
  print ""Main Thread""
print ""Main Thread Done""

:callAsync threadName timeout
async
  Print threadName
  wait timeout
  Print threadName + "" Done""
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(6, test.Logger.Count);
                Assert.AreEqual("Async Thread 2", test.Logger[0]);
                Assert.AreEqual("Async Thread", test.Logger[1]);
                Assert.AreEqual("Async Thread Done", test.Logger[2]);
                Assert.AreEqual("Main Thread", test.Logger[3]);
                Assert.AreEqual("Async Thread 2 Done", test.Logger[4]);
                Assert.AreEqual("Main Thread Done", test.Logger[5]);
            }
        }

        [TestMethod]
        public void awaitCommandCanBeBrokenOutOf() {
            String script = @"
:main
await
  print ""Main Thread""
  async
    Print ""Async Thread""
    wait 1
    Print ""Async Thread Done""
  wait 0.5
  break
print ""Main Thread Done""
  
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(4, test.Logger.Count);
                Assert.AreEqual("Main Thread", test.Logger[0]);
                Assert.AreEqual("Async Thread", test.Logger[1]);
                Assert.AreEqual("Main Thread Done", test.Logger[2]);
                Assert.AreEqual("Async Thread Done", test.Logger[3]);
            }
        }

        [TestMethod]
        public void awaitCommandContinueActsAsBreak() {
            String script = @"
:main
await
  print ""Main Thread""
  async
    Print ""Async Thread""
    wait 1
    Print ""Async Thread Done""
  wait 0.5
  continue
print ""Main Thread Done""
  
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(4, test.Logger.Count);
                Assert.AreEqual("Main Thread", test.Logger[0]);
                Assert.AreEqual("Async Thread", test.Logger[1]);
                Assert.AreEqual("Main Thread Done", test.Logger[2]);
                Assert.AreEqual("Async Thread Done", test.Logger[3]);
            }
        }

        [TestMethod]
        public void awaitCommandResetsCorrectly() {
            String script = @"
:main
await
  async
    Print ""Async Thread""
    Print ""Async Thread Done""
print ""Main Thread Done""
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Async Thread", test.Logger[0]);
                Assert.AreEqual("Async Thread Done", test.Logger[1]);
                Assert.AreEqual("Main Thread Done", test.Logger[2]);

                test.Logger.Clear();
                test.RunUntilDone();

                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Async Thread", test.Logger[0]);
                Assert.AreEqual("Async Thread Done", test.Logger[1]);
                Assert.AreEqual("Main Thread Done", test.Logger[2]);
            }
        }


        [TestMethod]
        public void terminateAllThreadsFromAsyncThread() {
            String script = @"
await
  async
    set the current thread name to ""My Async Thread""
    wait 1.25
    print ""Terminating Threads!""
    terminate all threads
  wait 1
print ""Done!""
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Terminating Threads!", test.Logger[0]);
            }
        }


    }
}
