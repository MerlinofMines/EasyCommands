using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class MessageHandlingTests {
        [TestMethod]
        public void NewlyParsedScriptIgnoresMessages() {
            using (var test = new ScriptTest(@"print 'Hello World'")) {
                test.mockIGC.RegisterBroadcastListener("channel");
                test.MockMessages("channel", @"print 'Incoming Message'");

                test.RunOnce();

                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PendingMessagesAreProcessedFirst() {
            var script = @"
print 'Hello World'
wait
print 'Hello World 2'
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("Hello World", test.Logger[0]);
                test.Logger.Clear();

                test.mockIGC.RegisterBroadcastListener("channel");
                test.MockMessages("channel", @"print 'Incoming Message'");

                test.RunOnce();

                Assert.AreEqual("Incoming Message", test.Logger[0]);
                test.Logger.Clear();

                test.RunOnce();

                Assert.AreEqual("Hello World 2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void UpdatedScriptDisablesAllBroadcastListenersAndClearsPendingMessages() {
            using (var test = new ScriptTest(@"print 'Hello World'")) {
                test.RunOnce();

                Assert.AreEqual("Hello World", test.Logger[0]);
                test.Logger.Clear();

                test.mockIGC.RegisterBroadcastListener("channel");
                test.MockMessages("channel", @"print 'Incoming Message'");

                test.RunOnce();

                Assert.AreEqual("Incoming Message", test.Logger[0]);
                test.Logger.Clear();

                test.SetScript(@"print 'Hello World 2'");
                test.MockMessages("channel", "print message1", "print message2");

                test.RunOnce();

                Assert.AreEqual("Hello World 2", test.Logger[0]);
                test.mockIGC.mockListeners.TrueForAll(listener => !listener.IsActive && listener.messages.Count == 0);
            }
        }
    }
}
