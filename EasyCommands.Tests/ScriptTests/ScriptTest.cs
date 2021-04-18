using EasyCommands.Tests.Util;
using IngameScript;
using Malware.MDKUtilities;
using Moq;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IngameScript.Program;

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
        MockGridTerminalSystem mockGrid;
        private Program program;
        private Mock<IMyProgrammableBlock> me;

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
            Logger = new List<String>();
            RunCounter = 0;
            Program.FUNCTION_PARSE_AMOUNT = 1000;

            // Setup the CUSTOM_DATA to return the given script
            // And other required config for mocking
            mockGrid = new MockGridTerminalSystem();
            me = new Mock<IMyProgrammableBlock>();

            MDKFactory.ProgramConfig config = default;
            config.GridTerminalSystem = mockGrid;
            config.ProgrammableBlock = me.Object;
            config.Echo = (message) => Logger.Add(message);
            program = MDKFactory.CreateProgram<Program>(config);
            // Init static fields that may have been modified from other tests, prepare for testing
            Program.STATE = ProgramState.STOPPED;
            Program.LOG_LEVEL = Program.LogLevel.SCRIPT_ONLY;

            // Default behavior for broadcast messages
            // TODO: Replace this with mock objects passed to config setup in Program
            program.broadcastMessageProvider = () => new List<MyIGCMessage>();

            setScript(script);
        }

        public void setScript(String script) {
            me.Setup(b => b.CustomData).Returns(script);
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
            RunUntil(t => Program.ProgramState.COMPLETE == Program.STATE || t.RunCounter >= 100);
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
        public void MockBlocksOfType<T>(String name, params Mock<T>[] blockMocks) where T : class, IMyTerminalBlock
        {
            SetupMockBlocksByType(name, blockMocks);
        }

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
        public void MockBlocksInGroup<T>(String groupName, params Mock<T>[] blockMocks) where T : class, IMyTerminalBlock
        {
            var mockGroup = new MockBlockGroup(groupName);
            mockGroup.AddBlocks(blockMocks
                .Select(block => (IMyTerminalBlock)block.Object)
                .ToList());

            mockGrid.AddGroup(mockGroup);
        }

        public void Dispose()
        {
            // TODO: Unset any static changes, etc
            Program.STATE = ProgramState.STOPPED;
            Program.LOG_LEVEL = LogLevel.INFO;
        }

        private void SetupMockBlocksByType<T>(string name, Mock<T>[] blockMocks) where T : class, IMyTerminalBlock
        {
            // Setup the mock blocks to mock their custom name to match the name provided
            foreach(Mock<T> block in blockMocks) {
                block.Setup(x => x.CustomName).Returns(name);
            }

            // Store the mock block objects by their type
            SetupMockBlocksByType(blockMocks);
        }

        private void SetupMockBlocksByType<T>(Mock<T>[] blockMocks) where T: class, IMyTerminalBlock
        {
            // Store the mock block objects by their type
            mockGrid.AddBlocks(blockMocks.Select(b => (IMyTerminalBlock)b.Object).ToList());
        }

        public class MockBlockGroup : IMyBlockGroup {
            HashSet<IMyTerminalBlock> mockBlocks = new HashSet<IMyTerminalBlock>();
            string name;

            public MockBlockGroup(string name) {
                this.name = name;
            }

            public string Name => name;

            public void AddBlock(IMyTerminalBlock block) {
                mockBlocks.Add(block);
            }

            public void AddBlocks(List<IMyTerminalBlock> blocks) {
                blocks.ForEach(block => mockBlocks.Add(block));
            }

            public void GetBlocks(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) {
                blocks.AddRange(mockBlocks
                    .Where(block => collect == null ? true : collect(block))
                    .ToList());
            }

            public void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) where T : class {
                blocks.AddRange(mockBlocks
                    .Where(block => block is T)
                    .Where(block => collect == null ? true : collect(block))
                    .ToList());
            }

            public void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null) where T : class {
                blocks.AddRange(mockBlocks
                    .Where(block => block is T)
                    .Select(block => (T) block)
                    .Where(block => collect == null ? true : collect(block))
                    .ToList());
            }
        }

        public class MockGridTerminalSystem : IMyGridTerminalSystem {
            HashSet<IMyTerminalBlock> mockBlocks = new HashSet<IMyTerminalBlock>();
            HashSet<IMyBlockGroup> mockGroups = new HashSet<IMyBlockGroup>();

            public void AddBlock(IMyTerminalBlock block) {
                mockBlocks.Add(block);
            }

            public void AddBlocks(List<IMyTerminalBlock> blocks) {
                blocks.ForEach(block => mockBlocks.Add(block));
            }

            public void AddGroup(IMyBlockGroup blockGroup) {
                mockGroups.Add(blockGroup);
            }

            public void GetBlockGroups(List<IMyBlockGroup> blockGroups, Func<IMyBlockGroup, bool> collect = null) {
                blockGroups.AddRange(mockGroups
                    .Where(group => collect == null ? true : collect(group))
                    .ToList());
            }

            public IMyBlockGroup GetBlockGroupWithName(string name) {
                return mockGroups
                    .Where(group => group.Name == name)
                    .FirstOrDefault(null);
            }

            public void GetBlocks(List<IMyTerminalBlock> blocks) {
                blocks.AddRange(mockBlocks.ToList());
            }

            public void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) where T : class {
                blocks.AddRange(mockBlocks
                    .Where(block => block is T)
                    .Where(block => collect == null ? true : collect(block))
                    .ToList());
            }

            public void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null) where T : class {
                blocks.AddRange(mockBlocks
                    .Where(block => block is T)
                    .Select(block => (T) block)
                    .Where(block => collect == null ? true : collect(block))
                    .ToList());
            }

            public IMyTerminalBlock GetBlockWithId(long id) {
                throw new NotImplementedException();
            }

            public IMyTerminalBlock GetBlockWithName(string name) {
                return mockBlocks
                    .Where(block => block.CustomName == name)
                    .FirstOrDefault(null);
            }

            public void SearchBlocksOfName(string name, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) {
                blocks.AddRange(mockBlocks
                    .Where(block => block.CustomName == name)
                    .Where(block => collect == null ? true : collect(block))
                    .ToList());
            }
        }
    }
}
