using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ThreadBlockTests {

        [TestMethod]
        public void GetCurrentThreadNameWhenNotCustom() {
            using (ScriptTest test = new ScriptTest(@"print the current thread name")) {
                test.program.logLevel = IngameScript.Program.LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("main", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetCurrentThreadNameWhenCustom() {
            using (ScriptTest test = new ScriptTest(@"
set the current thread name to ""Hello World!""
print the current thread name
")) {
                test.program.logLevel = IngameScript.Program.LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetCurrentThreadNameForAnonymousAsyncThreadWhenNotCustom() {
            using (ScriptTest test = new ScriptTest(@"
async
  print the current thread name
wait 1
")) {
                test.program.logLevel = IngameScript.Program.LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Unknown", test.Logger[0]);
            }
        }
        
        [TestMethod]
        public void GetCurrentThreadNameForAsyncThreadWhenNotCustom() {
            using (ScriptTest test = new ScriptTest(@"
async doThing
wait 1

:doThing
print the current thread name  
")) {
                test.program.logLevel = IngameScript.Program.LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("doThing", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetCurrentThreadNameForAsyncThreadWhenCustom() {
            using (ScriptTest test = new ScriptTest(@"
async
  set the current thread name to ""Hello World!""
  print the current thread name
wait 1
")) {
                test.program.logLevel = IngameScript.Program.LogLevel.SCRIPT_ONLY;
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetAllThreadNames() {
            using (ScriptTest test = new ScriptTest(@"

queue doThing
async 
  set the current thread name to ""Async Thread""
  wait 0.25
async 
  set the current thread name to ""Async Thread 2""
  wait 1
set the current thread name to ""My Main""
wait
print all thread names

:doThing
print all thread names
wait 0.5
print all thread names
")) {
                test.RunUntilDone();

                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("[\"Async Thread\",\"Async Thread 2\",doThing,\"My Main\"]", test.Logger[0]);
                Assert.AreEqual("[\"Async Thread\",\"Async Thread 2\",doThing]", test.Logger[1]);
                Assert.AreEqual("[\"Async Thread 2\",doThing]", test.Logger[2]);
            }
        }

        [TestMethod]
        public void GetAsyncThreadNames() {
            using (ScriptTest test = new ScriptTest(@"
queue doThing
print the list of async threads
async 
  set the current thread name to ""Async Thread""
  wait 0.25
print the list of async threads
async 
  set the current thread name to ""Async Thread 2""
  wait 1
print the list of async threads

:doThing
print the list of async threads
wait 0.5
print the list of async threads
")) {
                test.RunUntilDone();

                Assert.AreEqual(5, test.Logger.Count);
                Assert.AreEqual("[]", test.Logger[0]);
                Assert.AreEqual("[Unknown]", test.Logger[1]);
                Assert.AreEqual("[Unknown,Unknown]", test.Logger[2]);
                Assert.AreEqual("[\"Async Thread\",\"Async Thread 2\"]", test.Logger[3]);
                Assert.AreEqual("[\"Async Thread 2\"]", test.Logger[4]);                
            }
        }

        [TestMethod]
        public void GetChildThreadNames() {
            using (ScriptTest test = new ScriptTest(@"
print the list of child threads
async 
  set the current thread name to ""Async Thread""
  async 
      set the current thread name to ""Async Thread 2""
      wait 1
      print the list of child threads
  wait 0.25
  print the list of child threads
wait 2 ticks
print the list of child threads
")) {
                test.RunUntilDone();

                Assert.AreEqual(4, test.Logger.Count);
                Assert.AreEqual("[]", test.Logger[0]);
                Assert.AreEqual("[\"Async Thread\"]", test.Logger[1]);
                Assert.AreEqual("[\"Async Thread 2\"]", test.Logger[2]);
                Assert.AreEqual("[]", test.Logger[3]);
            }
        }

        [TestMethod]
        public void GetQueuedThreadNames() {
            using (ScriptTest test = new ScriptTest(@"
print the list of queued threads
queue doThing
print the list of queued threads
queue doThing2
print the list of queued threads

:doThing
print the list of queued threads

:doThing2
print the list of queued threads
")) {
                test.RunUntilDone();

                Assert.AreEqual(5, test.Logger.Count);
                Assert.AreEqual("[]", test.Logger[0]);
                Assert.AreEqual("[doThing]", test.Logger[1]);
                Assert.AreEqual("[doThing,doThing2]", test.Logger[2]);
                Assert.AreEqual("[doThing2]", test.Logger[3]);
                Assert.AreEqual("[]", test.Logger[4]);
            }
        }

        [TestMethod]
        public void TerminateCurrentThread() {
            using (ScriptTest test = new ScriptTest(@"
Print ""Beginning Thread""
terminate the current thread
print ""Done""
")) {
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Beginning Thread", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TerminateQueuedThreadsDoesNotTermiateCurrentThread() {
            using (ScriptTest test = new ScriptTest(@"
Print ""Calling Queue""
callQueue 4 times
terminate all queued threads
wait 1
print ""Done""

:callQueue
queue 
  print ""You cant kill me""
")) {
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Calling Queue", test.Logger[0]);
                Assert.AreEqual("Done", test.Logger[1]);
            }
        }

        [TestMethod]
        public void TerminateAsynchronousThreads() {
            using (ScriptTest test = new ScriptTest(@"
Print ""Calling Async""
callAsync 4 times
terminate all async threads
print ""Done""

:callAsync
async 
  wait 1
  print ""You cant kill me""
")) {
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Calling Async", test.Logger[0]);
                Assert.AreEqual("Done", test.Logger[1]);
            }
        }

        [TestMethod]
        public void TerminateChildThreadsDoesNotTerminateOtherAsyncThreads() {
            using (ScriptTest test = new ScriptTest(@"
Print ""Calling Async""
callAsync
wait 0.25
cancel all child threads
print ""Done""

:callAsync
async
  async
    wait 1
    print ""This One Remains""
  wait 1
  print ""You cant kill me""
")) {
                test.RunUntilDone();

                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Calling Async", test.Logger[0]);
                Assert.AreEqual("Done", test.Logger[1]);
                Assert.AreEqual("This One Remains", test.Logger[2]);
            }
        }
    }
}
