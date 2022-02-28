using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Threading.Tasks;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage;
using VRageMath;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace EasyCommands.Tests.ScriptTests {

    public class MockBlockGroup : Mock<IMyBlockGroup> {
        List<IMyTerminalBlock> mockBlocks = new List<IMyTerminalBlock>();

        public int BlockCount => mockBlocks.Count;

        public MockBlockGroup(string name) {
            Setup(g => g.Name).Returns(name);
            Setup(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>(), It.IsAny<Func<IMyTerminalBlock, bool>>()))
                .Callback<List<IMyTerminalBlock>, Func<IMyTerminalBlock, bool>>((blocks,collect) => {
                    blocks.Clear();
                    blocks.AddRange(mockBlocks.Where(collect ?? (b => true)));
                });
        }

        public void AddBlock<T>(Mock<T> block) where T : class, IMyTerminalBlock => mockBlocks.Add(block.Object);
        public void AddBlocks<T>(IEnumerable<Mock<T>> blocks) where T : class, IMyTerminalBlock  => mockBlocks.AddRange(blocks.Select(b => b.Object));
        public List<IMyTerminalBlock> GetBlocks() => mockBlocks;
    }

    public class MockGridTerminalSystem : Mock<IMyGridTerminalSystem> {
        HashSet<IMyTerminalBlock> mockBlocks = new HashSet<IMyTerminalBlock>();
        HashSet<IMyBlockGroup> mockGroups = new HashSet<IMyBlockGroup>();

        public int BlockCount => mockBlocks.Count;
        public int GroupCount => mockGroups.Count;

        public MockGridTerminalSystem() {
            Setup(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>()))
                .Callback<List<IMyTerminalBlock>>(blocks => {
                    blocks.Clear();
                    blocks.AddRange(mockBlocks);
                });
            Setup(g => g.GetBlockGroupWithName(It.IsAny<string>()))
                .Returns<string>(name => mockGroups.FirstOrDefault(g => g.Name == name));
        }

        public void AddBlock<T>(Mock<T> block) where T : class, IMyTerminalBlock => mockBlocks.Add(block.Object);
        public void AddBlocks<T>(IEnumerable<Mock<T>> blocks) where T : class, IMyTerminalBlock => blocks.ForEach(block => mockBlocks.Add(block.Object));
        public void AddGroup(MockBlockGroup blockGroup) {
            mockGroups.Add(blockGroup.Object);
            mockBlocks.UnionWith(blockGroup.GetBlocks());
        }
    }
}
