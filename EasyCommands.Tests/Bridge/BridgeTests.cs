using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage;
using BridgeProgram = EasyCommands.Bridge.UnitTests.Program;
using Step = System.Func<bool, string, Sandbox.ModAPI.Ingame.UpdateType,
    Sandbox.ModAPI.Ingame.IMyGridTerminalSystem, Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem,
    double, Sandbox.ModAPI.Ingame.UpdateFrequency, int, VRage.MyTuple<Sandbox.ModAPI.Ingame.UpdateFrequency, string[]>>;

namespace EasyCommands.Tests.Bridge {
    [TestClass]
    public class BridgeTests {
        const string ApiId = "EasyCommands.Bridge.v3";

        sealed class Invocation {
            public bool Reset;
            public string Argument;
            public UpdateType Source;
            public IMyGridTerminalSystem Terminal;
            public IMyIntergridCommunicationSystem Igc;
            public double Elapsed;
            public UpdateFrequency Requested;
            public int Logging;
        }
        sealed class Fixture {
            public readonly Mock<IMyProgrammableBlock> Block = new Mock<IMyProgrammableBlock>();
            public readonly Mock<IMyGridTerminalSystem> Terminal = new Mock<IMyGridTerminalSystem>();
            public readonly Mock<IMyIntergridCommunicationSystem> Igc = new Mock<IMyIntergridCommunicationSystem>();
            public readonly Mock<IMyGridProgramRuntimeInfo> Runtime = new Mock<IMyGridProgramRuntimeInfo>();
            public readonly Mock<ITerminalProperty<Step>> Property = new Mock<ITerminalProperty<Step>>();
            public readonly List<string> Output = new List<string>();
            public readonly List<Invocation> Calls = new List<Invocation>();
            public Func<MyTuple<UpdateFrequency, string[]>> Reply = () => MyTuple.Create(UpdateFrequency.Update1, new[] { "ok" });
            public Step Api;
            public readonly BridgeProgram Program;

            public Fixture() {
                Runtime.SetupAllProperties();
                Runtime.SetupGet(r => r.TimeSinceLastRun).Returns(TimeSpan.FromMilliseconds(1250));
                Api = (reset, argument, source, terminal, igc, elapsed, requested, logging) => {
                    Calls.Add(new Invocation { Reset = reset, Argument = argument, Source = source,
                        Terminal = terminal, Igc = igc, Elapsed = elapsed, Requested = requested, Logging = logging });
                    return Reply();
                };
                Property.SetupGet(p => p.Id).Returns(ApiId);
                Property.SetupGet(p => p.TypeName).Returns(typeof(Step).Name);
                Property.Setup(p => p.GetValue(Block.Object)).Returns(() => Api);
                Available();
                Program = ProgramFactory.CreateProgram<BridgeProgram>(new ProgramFactory.ProgramConfig {
                    ProgrammableBlock = Block.Object, GridTerminalSystem = Terminal.Object,
                    Runtime = Runtime.Object, Echo = text => Output.Add(text)
                });
                typeof(MyGridProgram).GetField("m_IGC_ContextGetter", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(Program, (Func<IMyIntergridCommunicationSystem>)(() => Igc.Object));
            }
            public void Available() { Block.Setup(b => b.GetProperty(ApiId)).Returns(Property.Object); }
            public void Missing() { Block.Setup(b => b.GetProperty(ApiId)).Returns((ITerminalProperty)null); }
            public void Run(string argument = "", UpdateType source = UpdateType.Trigger) {
                Output.Clear();
                ((Sandbox.ModAPI.IMyGridProgram)Program).Main(argument, source);
            }
            public void StoppedWithRetryHint() {
                Assert.AreEqual(UpdateFrequency.None, Runtime.Object.UpdateFrequency);
                Assert.IsTrue(Output.Any(s => s.Contains("Run again") || s.Contains("Run it again")), String.Join("\n", Output));
            }
        }

        [TestMethod]
        public void ConstructionRequestsDefaultIntervalWithoutDiscoveringOrCallingMod() {
            var f = new Fixture();
            Assert.AreEqual(UpdateFrequency.Update1, f.Program.updateFrequency);
            Assert.AreEqual(UpdateFrequency.Update1, f.Runtime.Object.UpdateFrequency);
            Assert.AreEqual(0, f.Calls.Count);
            f.Block.Verify(b => b.GetProperty(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void ForwardsExactArgumentSourceServicesElapsedTimeAndInterval() {
            var f = new Fixture();
            f.Program.updateFrequency = UpdateFrequency.Update10;
            var source = UpdateType.Terminal | UpdateType.IGC;
            f.Run("function 'quoted value'\nnext line", source);
            Assert.AreEqual(1, f.Calls.Count);
            var call = f.Calls.Single();
            Assert.IsTrue(call.Reset);
            Assert.AreEqual("function 'quoted value'\nnext line", call.Argument);
            Assert.AreEqual(source, call.Source);
            Assert.AreSame(f.Terminal.Object, call.Terminal);
            Assert.AreSame(f.Igc.Object, call.Igc);
            Assert.AreEqual(1250d, call.Elapsed);
            Assert.AreEqual(UpdateFrequency.Update10, call.Requested);
            f.Property.Verify(p => p.GetValue(f.Block.Object), Times.Once());
        }

        [DataTestMethod]
        [DataRow(UpdateFrequency.Update1)]
        [DataRow(UpdateFrequency.Update10)]
        [DataRow(UpdateFrequency.Update100)]
        [DataRow(UpdateFrequency.None)]
        [DataRow(UpdateFrequency.Update10 | UpdateFrequency.Update100)]
        public void ForwardsConfiguredIntervalIncludingManualAndCombinedFlags(UpdateFrequency requested) {
            var f = new Fixture();
            f.Program.updateFrequency = requested;
            f.Run();
            Assert.AreEqual(requested, f.Calls.Single().Requested);
        }

        [DataTestMethod]
        [DataRow(UpdateFrequency.Update1)]
        [DataRow(UpdateFrequency.Update10)]
        [DataRow(UpdateFrequency.Update100)]
        [DataRow(UpdateFrequency.None)]
        public void AppliesReturnedScheduleAndOutputInsteadOfForcingRequestedInterval(UpdateFrequency returned) {
            var f = new Fixture();
            f.Program.updateFrequency = UpdateFrequency.Update10;
            f.Reply = () => MyTuple.Create(returned, new[] { "engine output" });
            f.Run();
            Assert.AreEqual(returned, f.Runtime.Object.UpdateFrequency);
            CollectionAssert.AreEqual(new[] { "engine output" }, f.Output);
        }

        [TestMethod]
        public void NullEngineOutputDoesNotInventAnEchoCall() {
            var f = new Fixture();
            f.Reply = () => MyTuple.Create(UpdateFrequency.None, (string[])null);
            f.Run();
            Assert.AreEqual(0, f.Output.Count);
        }

        [DataTestMethod]
        [DataRow(BridgeProgram.LogLevel.INFO)]
        [DataRow(BridgeProgram.LogLevel.SCRIPT_ONLY)]
        public void ForwardsLoggingSettingOnEveryCall(BridgeProgram.LogLevel logging) {
            var f = new Fixture();
            Assert.AreEqual(BridgeProgram.LogLevel.INFO, f.Program.logLevel);
            f.Run();
            f.Program.logLevel = logging;
            f.Run();
            CollectionAssert.AreEqual(new[] { 0, (int)logging }, f.Calls.Select(c => c.Logging).ToArray());
            CollectionAssert.AreEqual(new[] { true, false }, f.Calls.Select(c => c.Reset).ToArray());
        }

        [TestMethod]
        public void ReplaysExactEchoCallsWithoutJoiningTruncatingOrAddingLines() {
            var f = new Fixture();
            var lines = new[] { "first\nsecond", "", null, new string('x', 12000), "last" };
            f.Reply = () => MyTuple.Create(UpdateFrequency.None, lines);
            f.Run();
            CollectionAssert.AreEqual(lines, f.Output);
            f.Reply = () => MyTuple.Create(UpdateFrequency.None, new string[0]);
            f.Run();
            Assert.AreEqual(0, f.Output.Count);
        }

        [TestMethod]
        public void SuccessfulCallsCacheBoundDelegateAndResetOnlyOnFirstCall() {
            var f = new Fixture();
            f.Reply = () => MyTuple.Create(UpdateFrequency.None, new[] { "done" });
            f.Run("first");
            f.Missing();
            f.Run("second");
            CollectionAssert.AreEqual(new[] { true, false }, f.Calls.Select(c => c.Reset).ToArray());
            CollectionAssert.AreEqual(new[] { "first", "second" }, f.Calls.Select(c => c.Argument).ToArray());
            f.Block.Verify(b => b.GetProperty(ApiId), Times.Once());
            f.Property.Verify(p => p.GetValue(f.Block.Object), Times.Once());
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("request that must not be replayed")]
        public void MissingModStopsWithoutPollingAndRejectsOnlyNonemptyArguments(string argument) {
            var f = new Fixture();
            f.Missing();
            f.Run(argument);
            Assert.AreEqual(0, f.Calls.Count);
            f.StoppedWithRetryHint();
            Assert.IsTrue(f.Output.Any(s => s.Contains("unavailable or incompatible")));
            Assert.AreEqual(argument.Length != 0, f.Output.Any(s => s.Contains("argument was not accepted")));
        }

        [TestMethod]
        public void ExplicitRetryDiscoversNewlyAvailableModWithoutReplayingRejectedArgument() {
            var f = new Fixture();
            f.Missing(); f.Run("rejected");
            f.Available(); f.Run("new request");
            Assert.AreEqual(1, f.Calls.Count);
            Assert.IsTrue(f.Calls.Single().Reset);
            Assert.AreEqual("new request", f.Calls.Single().Argument);
            f.Block.Verify(b => b.GetProperty(ApiId), Times.Exactly(2));
        }

        [TestMethod]
        public void NullCallableStopsAndCanBeDiscoveredOnExplicitRetry() {
            var f = new Fixture();
            Step validApi = f.Api;
            f.Api = null; f.Run("rejected");
            f.StoppedWithRetryHint();
            Assert.AreEqual(0, f.Calls.Count);
            f.Api = validApi; f.Run("retry");
            Assert.IsTrue(f.Calls.Single().Reset);
            f.Property.Verify(p => p.GetValue(f.Block.Object), Times.Exactly(2));
        }

        [TestMethod]
        public void IncompatiblePropertyStopsAndCanRecoverOnExplicitRetry() {
            var f = new Fixture();
            var wrong = new Mock<ITerminalProperty<string>>();
            wrong.SetupGet(p => p.TypeName).Returns("String");
            f.Block.Setup(b => b.GetProperty(ApiId)).Returns(wrong.Object);
            f.Run("rejected");
            f.StoppedWithRetryHint();
            Assert.AreEqual(0, f.Calls.Count);
            f.Available(); f.Run("retry");
            Assert.IsTrue(f.Calls.Single().Reset);
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void DiscoveryFailuresStopAndRetryWithFreshBinding(bool getterFails) {
            var f = new Fixture();
            if (getterFails) f.Property.Setup(p => p.GetValue(f.Block.Object)).Throws(new InvalidOperationException("lookup failed"));
            else f.Block.Setup(b => b.GetProperty(ApiId)).Throws(new InvalidOperationException("lookup failed"));
            f.Run("rejected");
            f.StoppedWithRetryHint();
            Assert.IsTrue(f.Output.Any(s => s.Contains("lookup failed")));
            f.Available();
            f.Property.Setup(p => p.GetValue(f.Block.Object)).Returns(() => f.Api);
            f.Run("retry");
            Assert.IsTrue(f.Calls.Single().Reset);
        }

        [TestMethod]
        public void InvocationFailureDiscardsCachedDelegateAndResetsNextSuccessfulAttachment() {
            var f = new Fixture();
            f.Run("initial success");
            f.Reply = () => { throw new InvalidOperationException("engine failed"); };
            f.Run("failed invocation");
            f.StoppedWithRetryHint();
            Assert.IsTrue(f.Output.Any(s => s.Contains("engine failed")));
            var replacementResets = new List<bool>();
            f.Api = (reset, argument, source, terminal, igc, elapsed, requested, logging) => {
                replacementResets.Add(reset);
                return MyTuple.Create(UpdateFrequency.Update100, new[] { "reconnected" });
            };
            f.Run("reconnect"); f.Run("continue");
            CollectionAssert.AreEqual(new[] { true, false }, f.Calls.Select(c => c.Reset).ToArray());
            CollectionAssert.AreEqual(new[] { true, false }, replacementResets);
            f.Block.Verify(b => b.GetProperty(ApiId), Times.Exactly(2));
            Assert.AreEqual(UpdateFrequency.Update100, f.Runtime.Object.UpdateFrequency);
        }

        [TestMethod]
        public void BridgeInstancesDoNotShareCachedDelegateOrAttachmentState() {
            var a = new Fixture(); var b = new Fixture();
            a.Run("A1"); b.Run("B1"); a.Run("A2");
            CollectionAssert.AreEqual(new[] { "A1", "A2" }, a.Calls.Select(c => c.Argument).ToArray());
            CollectionAssert.AreEqual(new[] { true, false }, a.Calls.Select(c => c.Reset).ToArray());
            Assert.AreEqual("B1", b.Calls.Single().Argument);
            Assert.IsTrue(b.Calls.Single().Reset);
            Assert.AreSame(a.Terminal.Object, a.Calls[0].Terminal);
            Assert.AreSame(b.Terminal.Object, b.Calls[0].Terminal);
        }
    }
}
