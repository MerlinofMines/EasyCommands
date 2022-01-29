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
    public class ListenCommandTests {

        [TestMethod]
        public void ListenChannel() {
            using (var test = new ScriptTest(@"listen myChannel")) {
                test.RunUntilDone();

                var registeredChannel = test.mockIGC.GetMockBroadcastListener("myChannel");
                Assert.IsNotNull(registeredChannel);
                Assert.IsTrue(registeredChannel.IsActive);
            }
        }

        [TestMethod]
        public void ListenOnAlreadyRegisteredChannelDoesNothing() {
            using (var test = new ScriptTest(@"listen myChannel")) {

                test.RunUntilDone();

                var registeredChannel = test.mockIGC.GetMockBroadcastListener("myChannel");
                Assert.IsNotNull(registeredChannel);
                Assert.IsTrue(registeredChannel.IsActive);

                test.RunUntilDone();

                Assert.AreEqual(1, test.mockIGC.mockListeners.Count);
                registeredChannel = test.mockIGC.GetMockBroadcastListener("myChannel");
                Assert.IsNotNull(registeredChannel);
                Assert.IsTrue(registeredChannel.IsActive);
            }
        }

        [TestMethod]
        public void ListenOnIgnoredChannelReregisters() {
            using (var test = new ScriptTest(@"
listen myChannel
wait
forget myChannel
wait
listen myChannel
")) {
                test.RunOnce();

                var registeredChannel = test.mockIGC.GetMockBroadcastListener("myChannel");
                Assert.IsNotNull(registeredChannel);
                Assert.IsTrue(registeredChannel.IsActive);

                test.RunOnce();

                Assert.AreEqual(1, test.mockIGC.mockListeners.Count);
                registeredChannel = test.mockIGC.GetMockBroadcastListener("myChannel");
                Assert.IsNotNull(registeredChannel);
                Assert.IsFalse(registeredChannel.IsActive);

                test.RunOnce();

                Assert.AreEqual(1, test.mockIGC.mockListeners.Count);
                registeredChannel = test.mockIGC.GetMockBroadcastListener("myChannel");
                Assert.IsNotNull(registeredChannel);
                Assert.IsTrue(registeredChannel.IsActive);
            }
        }

        [TestMethod]
        public void IgnoreChannel() {
            using (var test = new ScriptTest(@"
listen myChannel
wait
forget myChannel
")) {
                test.RunOnce();

                var registeredChannel = test.mockIGC.GetMockBroadcastListener("myChannel");
                Assert.IsNotNull(registeredChannel);
                Assert.IsTrue(registeredChannel.IsActive);

                test.RunOnce();

                Assert.AreEqual(1, test.mockIGC.mockListeners.Count);
                registeredChannel = test.mockIGC.GetMockBroadcastListener("myChannel");
                Assert.IsNotNull(registeredChannel);
                Assert.IsFalse(registeredChannel.IsActive);
            }
        }

        [TestMethod]
        public void IgnoreUnregisteredChannelDoesNothing() {
            using (var test = new ScriptTest(@"ignore myChannel")) {
                test.RunUntilDone();

                Assert.AreEqual(0, test.mockIGC.mockListeners.Count);
            }
        }
    }
}
