using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
                test.RunUntilDone();

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
        public void runArgumentIsQueudAtFront() {
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
        public void queueCommandIsQueudAtBack() {
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
                test.RunOnce();//Parse
                Assert.AreEqual(0, test.Logger.Count);

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
                test.RunOnce();//Parse
                Assert.AreEqual(0, test.Logger.Count);

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
                test.RunIterations(2);//Parse
                Assert.AreEqual(0, test.Logger.Count);

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
    }
}
