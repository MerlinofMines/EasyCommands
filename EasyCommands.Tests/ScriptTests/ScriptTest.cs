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
        private GenericListDictionary mockedBlocksByName;
        private List<IMyBlockGroup> mockedGroups;
        private Mock<IMyGridTerminalSystem> gridMock;
        private Program program;

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
            mockedBlocksByName = new GenericListDictionary();
            mockedGroups = new List<IMyBlockGroup>();
            Logger = new List<String>();
            RunCounter = 0;

            // Setup the CUSTOM_DATA to return the given script
            // And other required config for mocking
            gridMock = new Mock<IMyGridTerminalSystem>();
            var me = new Mock<IMyProgrammableBlock>();
            me.Setup(b => b.CustomData).Returns(script);
            MDKFactory.ProgramConfig config = default;
            config.GridTerminalSystem = gridMock.Object;
            config.ProgrammableBlock = me.Object;
            config.Echo = (message) => Logger.Add(message);
            program = MDKFactory.CreateProgram<Program>(config);
            // Init static fields that may have been modified from other tests, prepare for testing
            Program.STATE = ProgramState.STOPPED;
            Program.LOG_LEVEL = Program.LogLevel.SCRIPT_ONLY;

            // Default behavior for broadcast messages
            // TODO: Replace this with mock objects passed to config setup in Program
            program.broadcastMessageProvider = () => new List<MyIGCMessage>();

            // Default behavior for locating blocks on the grid
            gridMock.Setup(g => g.GetBlockGroups(It.IsAny<List<IMyBlockGroup>>(), It.IsAny<Func<IMyBlockGroup, bool>>()))
                .Callback((Action<List<IMyBlockGroup>, Func<IMyBlockGroup, bool>>)GetBlockGroupsCallback);

            // TODO: Handle custom logic in TextSurfaceHandler
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
        /// Run the script a given number of iterations, regarldess of Program State.  
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

            // Set up the mocked GridTerminalSystem to invoke the appropriate callback method for the appropriate type
            gridMock.Setup(g => g.GetBlocksOfType(It.IsAny<List<T>>(), It.IsAny<Func<T, bool>>()))
                .Callback((Action<List<T>, Func<T, bool>>)GetBlocksOfTypeCallback);
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
            var mockGroup = new Mock<IMyBlockGroup>();
            mockGroup.Setup(g => g.Name).Returns(groupName);
            mockGroup.Setup(g => g.GetBlocksOfType(It.IsAny<List<T>>(), It.IsAny<Func<T, bool>>()))
                .Callback((Action<List<T>, Func<T, bool>>)GetBlocksOfTypeCallback);
            mockedGroups.Add(mockGroup.Object);

            SetupMockBlocksByType(blockMocks);
        }

        private void GetBlocksOfTypeCallback<T>(List<T> blocks, Func<T, bool> collect) where T : class, IMyTerminalBlock
        {
            // Find the appropriate blocks by type and then invoke the given collector function on them as a filter
            blocks.AddRange(mockedBlocksByName.GetValue<T>(typeof(T))
                .Select(x => x)
                .Where(collect != null ? collect : (x) => true)
                .ToList());
        }

        private void GetBlockGroupsCallback(List<IMyBlockGroup> groups, Func<IMyBlockGroup, bool> collect)
        {
            // Invoke the given collector function on all groups and return those groups matching
            groups.AddRange(mockedGroups
                .Select(x => x)
                .Where(collect != null ? collect : (x) => true)
                .ToList());
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
            List<T> blockObjects = blockMocks.Select(b =>
            {
                b.Setup(x => x.CustomName).Returns(name);
                return b.Object;
            }).ToList();
            // Store the mock block objects by their type
            mockedBlocksByName.Add(blockObjects);
        }

        private void SetupMockBlocksByType<T>(Mock<T>[] blockMocks) where T: class, IMyTerminalBlock
        {
            // Store the mock block objects by their type
            mockedBlocksByName.Add(blockMocks.Select(b => b.Object).ToList());
        }

        public class GenericListDictionary
        {
            private Dictionary<Type, HashSet<Object>> dictionary = new Dictionary<Type, HashSet<object>>();

            public void Add<T>(List<T> values)
            {
                Type key = typeof(T);
                HashSet<object> valueObjects = values.Select(x => (Object)x).ToHashSet();
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].UnionWith(valueObjects);
                } else
                {
                    dictionary.Add(key, valueObjects);
                }
            }

            public HashSet<T> GetValue<T>(Type type)
            {
                if (!dictionary.ContainsKey(type))
                {
                    return new HashSet<T>();
                }

                return dictionary[type].Select(x => (T)x).ToHashSet();
            }
        }

    }
}
