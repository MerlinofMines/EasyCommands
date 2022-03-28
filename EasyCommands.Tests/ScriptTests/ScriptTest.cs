using IngameScript;
using Malware.MDKUtilities;
using Moq;
using Sandbox.ModAPI.Ingame;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.ModAPI.Ingame;
using static IngameScript.Program;
using System.Reflection;

namespace EasyCommands.Tests.ScriptTests
{
    /// <summary>
    /// Represents an instance of a ScriptTest. Constructing a new ScriptTest is akin to setting
    /// up the test, as it instantiates required objects and performs basic setup.
    ///
    /// This implements <see cref="IDisposable"/>, so any post-test cleanup is performed
    /// upon destruction of the class. Please use this inside of a using statement.
    /// </summary>
    class ScriptTest : IDisposable
    {
        public Program program;
        public Mock<IMyProgrammableBlock> me;
        public Mock<IMyTextSurface> display;
        public Mock<Random> random;
        public Mock<IMyGridProgramRuntimeInfo> runtime;
        public MockIntergridCommunicationSystem mockIGC;
        public MockGridTerminalSystem mockGrid;

        int entityIdCounter = 1000;

        /// <summary>
        /// Counter of how many times the given script has been invoked by the test engine.
        /// </summary>
        public int RunCounter { get; private set; }

        /// <summary>
        /// Logger that acts as a replacement for the program's ECHO behavior.
        /// </summary>
        public List<String> Logger { get; private set; }

        public ScriptTest(String script)
        {
            SetCulture("en-US");

            Logger = new List<String>();
            RunCounter = 0;

            // Setup Runtime
            runtime = new Mock<IMyGridProgramRuntimeInfo>();
            runtime.Setup(r => r.TimeSinceLastRun).Returns(TimeSinceLastRun);

            //Setup Mock Random
            random = new Mock<Random>();

            // Setup the CUSTOM_DATA to return the given script
            // And other required config for mocking
            mockGrid = new MockGridTerminalSystem();
            me = new Mock<IMyProgrammableBlock>();
            display = new Mock<IMyTextSurface>();
            MockBlocksOfType("Script Program", me);
            MockEntityUtility.MockTextSurfaces(me, display);

            MDKFactory.ProgramConfig config = default;
            config.GridTerminalSystem = mockGrid.Object;
            config.ProgrammableBlock = me.Object;
            config.Echo = (message) => Logger.Add(message);
            config.Runtime = runtime.Object;

            program = MDKFactory.CreateProgram<Program>(config);
            program.commandParseAmount = 1000;
            program.logLevel = Program.LogLevel.SCRIPT_ONLY;

            //Mock message broadcasting
            mockIGC = new MockIntergridCommunicationSystem();
            typeof(MyGridProgram).GetField("m_IGC_ContextGetter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(program, GetIGCSupplier());

            program.randomGenerator = random.Object;

            //Re-initialize items using mock blueprint provider
            program.blueprintProvider = MockEntityUtility.MockBlueprint;
            program.itemNamesToBlueprints.Clear();
            program.itemNamesToFilters.Clear();
            program.InitializeItems();

            SetScript(script);
        }

        public Func<IMyIntergridCommunicationSystem> GetIGCSupplier() => () => mockIGC;

        public void MockMessages(String tag, params String[] messages) {
            var listener = mockIGC.GetMockBroadcastListener(tag);
            listener.messages.AddRange(messages);
        }

        public void MockNextRandoms(params int[] nextRandom) {
            var sequence = random.SetupSequence(b => b.Next());
            nextRandom.ForEach(next => sequence.Returns(next));
        }

        public void MockNextBoundedRandoms(int upperBound, params int[] nextRandom) {
            var sequence = random.SetupSequence(b => b.Next(upperBound));
            nextRandom.ForEach(next => sequence.Returns(next));
        }

        public void SetScript(String script) {
            me.Setup(b => b.CustomData).Returns(script);
        }

        public void SetCulture(String culture) => SetCulture(new CultureInfo(culture));
        public void SetCulture(CultureInfo culture) => System.Threading.Thread.CurrentThread.CurrentCulture = culture;

        public TimeSpan TimeSinceLastRun() {
            long updateTicks;
            switch (program.updateFrequency) {
                case UpdateFrequency.Update100:
                    updateTicks = 16660000;
                    break;
                case UpdateFrequency.Update10:
                    updateTicks = 1666000;
                    break;
                case UpdateFrequency.None:
                    updateTicks = 166600;
                    break;
                case UpdateFrequency.Once:
                    updateTicks = 166600;
                    break;
                case UpdateFrequency.Update1:
                    updateTicks = 166600;
                    break;
                default:
                    updateTicks = 166600;
                    break;
            }
            return new TimeSpan(updateTicks);
        }

        public void SetUpdateFrequency(UpdateFrequency updateFrequency) {
            program.updateFrequency = updateFrequency;
        }

        /// <summary>
        /// Run the script until the given predicate is satisfied.
        /// </summary>
        public void RunUntil(Predicate<ScriptTest> runUntilPredicate)
        {
            do
            {
                MDKFactory.Run(program);
                RunCounter++;
            } while (runUntilPredicate.Invoke(this) != true);
        }

        /// <summary>
        /// Run the script with a default predicate of the program state being considered COMPLETE,
        /// or the run counter exceeding 100 attempts.
        /// </summary>
        public void RunUntilDone()
        {
            RunUntilState(ProgramState.COMPLETE);
        }

        public void RunUntilState(ProgramState state) {
            RunUntil(t => state == program.state || t.RunCounter >= 100);

        }

        /// <summary>
        /// Run the script a given number of iterations, regardless of Program State.
        /// </summary>
        /// <param name="iterations"></param>
        public void RunIterations(int iterations)
        {
            int EndCounter = RunCounter + iterations;
            RunUntil(t => t.RunCounter >= EndCounter);
        }

        /// <summary>
        /// Run the script one iteration
        /// </summary>
        public void RunOnce()
        {
            RunIterations(1);
        }
        /// <summary>
        /// Runs the program with the given argument.  This only invokes the program one iteration.
        /// </summary>
        /// <param name="argument"></param>
        public void RunWithArgument(string argument) {
            MDKFactory.Run(program, argument);
        }

        /// <summary>
        /// Given a custom name for a block (or set of blocks), set up the custom names
        /// for the given mocked blocks and store them for later retrieval when requested by
        /// the grid terminal system.
        ///
        /// Since the GridTerminalSystem's GetBlocksOfType method doesn't return values but
        /// instead modifies the given list, this uses Moq's Callback functionality to
        /// invoke a callback that will attempt to retrieve the stored blocks.
        /// </summary>
        /// <typeparam name="T">The type of the blocks we're mocking retrieval for.</typeparam>
        /// <param name="name">The custom name of the block(s) we're mocking.</param>
        /// <param name="blockMocks">Blocked mocks to be stored and used later.</param>
        public void MockBlocksOfType<T>(String name, params Mock<T>[] blockMocks) where T : class, IMyTerminalBlock => SetupMockBlocksByType(name, blockMocks);

        /// <summary>
        /// Given a group name, set up a mock group that will be retrieved by the GridTerminalSystem.
        /// Store the given mocked blocks for later retrieval.
        ///
        /// Also, set up the group's GetBlocksOfType method to use a Moq Callback so that
        /// those mocked blocks can be found later.
        /// </summary>
        /// <typeparam name="T">The type of the blocks being mocked for this group.</typeparam>
        /// <param name="groupName">The name of the group being mocked.</param>
        /// <param name="blockMocks">The blocks being mocked and that will be returned later.</param>
        public MockBlockGroup MockBlocksInGroup<T>(String groupName, params Mock<T>[] blockMocks) where T : class, IMyTerminalBlock
        {
            for(int i = 0; i < blockMocks.Length; i++ ) {
                blockMocks[i].Setup(x => x.EntityId).Returns(entityIdCounter++);
                blockMocks[i].Setup(x => x.CustomName).Returns(groupName + " " + i);
            }

            var mockGroup = new MockBlockGroup(groupName);
            mockGroup.AddBlocks(blockMocks);
            mockGrid.AddGroup(mockGroup);

            return mockGroup;
        }

        public void Dispose()
        {
            // TODO: Unset any static changes, etc
        }

        private void SetupMockBlocksByType<T>(string name, Mock<T>[] blockMocks) where T : class, IMyTerminalBlock
        {
            // Setup the mock blocks to mock their custom name to match the name provided
            foreach(Mock<T> block in blockMocks)
                block.Setup(x => x.CustomName).Returns(name);

            SetupMockBlocksByType(blockMocks);
        }

        private void SetupMockBlocksByType<T>(Mock<T>[] blockMocks) where T: class, IMyTerminalBlock
        {
            foreach(Mock<T> block in blockMocks)
                block.Setup(x => x.EntityId).Returns(entityIdCounter++);

            mockGrid.AddBlocks(blockMocks);
        }
    }

    public class MockIntergridCommunicationSystem : IMyIntergridCommunicationSystem {
        public long Me => 1234;

        public List<MockBroadcastListener> mockListeners = new List<MockBroadcastListener>();

        public IMyUnicastListener UnicastListener => throw new NotImplementedException();

        public void DisableBroadcastListener(IMyBroadcastListener broadcastListener) {
            mockListeners.Where(listener => listener.Tag == broadcastListener.Tag).ForEach(listener => listener.IsActive = false);
        }

        public MockBroadcastListener GetMockBroadcastListener(String tag) {
            return mockListeners.Where(listener => listener.Tag == tag).FirstOrDefault();
        }

        public void GetBroadcastListeners(List<IMyBroadcastListener> broadcastListeners, Func<IMyBroadcastListener, bool> collect = null) {
            mockListeners.Select(listener => listener).Where(listener => collect == null || collect(listener)).ForEach(l => broadcastListeners?.Add(l));
        }

        public bool IsEndpointReachable(long address, TransmissionDistance transmissionDistance = TransmissionDistance.AntennaRelay) {
            throw new NotImplementedException();
        }

        public IMyBroadcastListener RegisterBroadcastListener(string tag) {
            var broadcastListener = GetMockBroadcastListener(tag);

            if (broadcastListener == null) {
                MockBroadcastListener mockListener = new MockBroadcastListener(tag);
                mockListeners.Add(mockListener);
                broadcastListener = mockListener;
            }
            broadcastListener.IsActive = true;
            return broadcastListener;
        }

        public void SendBroadcastMessage<TData>(string tag, TData data, TransmissionDistance transmissionDistance = TransmissionDistance.AntennaRelay) {
            //Do nothing
        }

        public bool SendUnicastMessage<TData>(long addressee, string tag, TData data) {
            throw new NotImplementedException();
        }
    }

    public class MockBroadcastListener : IMyBroadcastListener {

        public List<string> messages = new List<string>();
        public string Tag { get; set; }
        public bool IsActive { get; set; }

        public MockBroadcastListener(string tag, params string[] messages) {
            this.Tag = tag;
            this.messages = messages.ToList();
        }

        public bool HasPendingMessage => messages.Count > 0;

        public int MaxWaitingMessages => throw new NotImplementedException();

        public MyIGCMessage AcceptMessage() {
            var nextMessage = messages[0];
            messages.RemoveAt(0);
            return new MyIGCMessage(nextMessage, Tag, 1234);
        }

        public void DisableMessageCallback() {
            throw new NotImplementedException();
        }

        public void SetMessageCallback(string argument = "") {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents an instance of a ScriptTest for script of the form "Print expr" for one
    /// or multiple expressions.
    ///
    /// see ScriptTest for more details
    /// </summary>
    class SimpleExpressionsTest : ScriptTest {
        public SimpleExpressionsTest(String expression)
            : base("Print " + expression) { }

        public SimpleExpressionsTest(List<String> expressions)
            : base(String.Join("\n", expressions.Select(e => "Print " + e).ToArray())) {}
    }
}
