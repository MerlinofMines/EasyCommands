using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests.FunctionalTests {
    [TestClass]
    public class CacheTests {
        [TestMethod]
        public void TerminalPropertiesAreCached() {
            using (var test = new ScriptTest(@"increment the ""test terminal"" ""SomeNumber"" property by 1")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var property = MockProperty<IMyTerminalBlock, float>(mockBlock, "SomeNumber");
                property.Setup(p => p.GetValue(mockBlock.Object)).Returns(42f);

                test.RunUntilDone();

                mockBlock.Verify(b => b.GetProperty("SomeNumber"), Times.Once());
                property.Verify(p => p.SetValue(mockBlock.Object, 43f));
            }
        }

        [TestMethod]
        public void TerminalPropertiesAreShared() {
            var script = @"
set the ""test terminal 1"" ""SomeNumber"" property to 1
set the ""test terminal 2"" ""SomeNumber"" property to 2
";

            using (var test = new ScriptTest(script)) {
                var mockBlock1 = new Mock<IMyTerminalBlock>();
                var mockBlock2 = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal 1", mockBlock1);
                test.MockBlocksOfType("test terminal 2", mockBlock2);
                var property1 = MockProperty<IMyTerminalBlock, float>(mockBlock1, "SomeNumber");
                var property2 = MockProperty<IMyTerminalBlock, float>(mockBlock2, "SomeNumber");

                test.RunUntilDone();

                mockBlock1.Verify(b => b.GetProperty("SomeNumber"), Times.Once());
                mockBlock2.Verify(b => b.GetProperty("SomeNumber"), Times.Never());
                property1.Verify(p => p.SetValue(mockBlock1.Object, 1f));
                property1.Verify(p => p.SetValue(mockBlock2.Object, 2f));
                property2.VerifyNoOtherCalls();
            }
        }

        [TestMethod]
        public void TerminalPropertiesAreDistinctByType() {
            var script = @"
set the ""test terminal"" ""SomeNumber"" property to 1
set the my ""SomeNumber"" property to 2
";

            using (var test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var property1 = MockProperty<IMyTerminalBlock, float>(mockBlock, "SomeNumber");
                var property2 = MockProperty<IMyProgrammableBlock, float>(test.me, "SomeNumber");

                test.RunUntilDone();

                mockBlock.Verify(b => b.GetProperty("SomeNumber"), Times.Once());
                test.me.Verify(b => b.GetProperty("SomeNumber"), Times.Once());
                property1.Verify(p => p.SetValue(mockBlock.Object, 1f));
                property2.Verify(p => p.SetValue(test.me.Object, 2f));
            }
        }

        [TestMethod]
        public void TerminalActionsAreCached() {
            var script = @"
apply the ""test terminal"" ""DoSomething"" action
apply the ""test terminal"" ""DoSomething"" action
";

            using (var test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var action = MockAction(mockBlock, "DoSomething");

                test.RunUntilDone();

                mockBlock.Verify(b => b.GetActionWithName("DoSomething"), Times.Once());
                action.Verify(a => a.Apply(mockBlock.Object), Times.Exactly(2));
            }
        }

        [TestMethod]
        public void TerminalActionsAreShared() {
            var script = @"
apply the ""test terminal 1"" ""DoSomething"" action
apply the ""test terminal 2"" ""DoSomething"" action
";

            using (var test = new ScriptTest(script)) {
                var mockBlock1 = new Mock<IMyTerminalBlock>();
                var mockBlock2 = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal 1", mockBlock1);
                test.MockBlocksOfType("test terminal 2", mockBlock2);
                var action1 = MockAction(mockBlock1, "DoSomething");
                var action2 = MockAction(mockBlock2, "DoSomething");

                test.RunUntilDone();

                mockBlock1.Verify(b => b.GetActionWithName("DoSomething"), Times.Once());
                mockBlock2.Verify(b => b.GetActionWithName("DoSomething"), Times.Never());
                action1.Verify(a => a.Apply(mockBlock1.Object), Times.Once());
                action1.Verify(a => a.Apply(mockBlock2.Object), Times.Once());
                action2.VerifyNoOtherCalls();
            }
        }

        [TestMethod]
        public void TerminalActionsAreDistinctByType() {
            var script = @"
apply the ""test terminal"" ""DoSomething"" action
apply the my ""DoSomething"" action
";

            using (var test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var action1 = MockAction(mockBlock, "DoSomething");
                var action2 = MockAction(test.me, "DoSomething");

                test.RunUntilDone();

                mockBlock.Verify(b => b.GetActionWithName("DoSomething"), Times.Once());
                test.me.Verify(b => b.GetActionWithName("DoSomething"), Times.Once());
                action1.Verify(a => a.Apply(mockBlock.Object), Times.Once());
                action2.Verify(a => a.Apply(test.me.Object), Times.Once());
            }
        }

        [TestMethod]
        public void BlockAndSelectorCachesAreClearedAndFilled() {
            var script = @"
set selector to ""test piston""
wait 1 tick
turn on $selector
turn off $selector
wait 2 ticks
turn off my pistons
turn on $selector
";

            using (var test = new ScriptTest(script)) {
                var mockGrid = test.mockGrid;
                var mockBlock = new Mock<IMyPistonBase>();
                test.MockBlocksOfType("test piston", mockBlock);

                test.RunOnce();

                mockGrid.Verify(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>()), Times.Never());

                Assert.AreEqual(0, test.program.selectorCache.storage.Count);

                test.RunOnce();

                mockGrid.Verify(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>()), Times.Once());
                mockBlock.VerifySet(p => p.Enabled = false);

                Assert.AreEqual(1, test.program.selectorCache.storage.Count);

                test.RunOnce();

                mockGrid.Verify(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>()), Times.Once());

                test.RunOnce();

                mockGrid.Verify(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>()), Times.Exactly(2));
                mockBlock.VerifySet(p => p.Enabled = true);

                Assert.AreEqual(2, test.program.selectorCache.storage.Count);

                Assert.AreEqual("[PISTON] <null>;[PISTON] test piston", string.Join(";", test.program.selectorCache.storage.Keys.Select(e => $"[{e.Item1}] {e.Item2 ?? "<null>"}")));
            }
        }

        [TestMethod]
        public void GroupCachesAreClearedAndFilled() {
            var script = @"
set selector to ""test pistons""
wait 1 tick
turn on $selector
turn off $selector
wait 2 ticks
turn on $selector
";

            using (var test = new ScriptTest(script)) {
                var mockGrid = test.mockGrid;
                var mockBlock1 = new Mock<IMyPistonBase>();
                var mockBlock2 = new Mock<IMyPistonBase>();
                var mockGroup = test.MockBlocksInGroup("test pistons", mockBlock1, mockBlock2);

                test.RunOnce();

                mockGroup.Verify(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>(), It.IsAny<Func<IMyTerminalBlock, bool>>()), Times.Never());

                test.RunOnce();

                mockGroup.Verify(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>(), It.IsAny<Func<IMyTerminalBlock, bool>>()), Times.Once());
                mockBlock1.VerifySet(p => p.Enabled = false);
                mockBlock2.VerifySet(p => p.Enabled = false);

                test.RunOnce();

                mockGroup.Verify(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>(), It.IsAny<Func<IMyTerminalBlock, bool>>()), Times.Once());

                Assert.AreEqual(0, test.program.groupCache.storage.Count);

                test.RunOnce();

                mockGrid.Verify(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>()), Times.Never());
                mockGroup.Verify(g => g.GetBlocks(It.IsAny<List<IMyTerminalBlock>>(), It.IsAny<Func<IMyTerminalBlock, bool>>()), Times.Exactly(2));
                mockBlock1.VerifySet(p => p.Enabled = true);
                mockBlock2.VerifySet(p => p.Enabled = true);

                Assert.AreEqual(1, test.program.groupCache.storage.Count);

                Assert.AreEqual("[PISTON] test pistons", string.Join(";", test.program.groupCache.storage.Keys.Select(e => $"[{e.Item1}] {e.Item2}")));
            }
        }
    }
}
