using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class TerminalBlockTests : ForceLocale {
        [TestMethod]
        public void GetBlockName() {
            String script = @"
#First block is the actual program, so grab 2nd index
print ""Name: "" + terminal[1] name
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                Assert.AreEqual("Name: test terminal", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetBlockName() {
            String script = @"
set blockCount to the count of all terminals
set i to 0
until i >= blockCount
  set terminal[i] name to ""Block "" + (i + 1)
  i++
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                test.me.VerifySet(b => b.CustomName = "Block 1");
                mockBlock.VerifySet(b => b.CustomName = "Block 2");

            }
        }

        [TestMethod]
        public void GetBlockNames() {
            String script = @"
print ""Names: "" + terminal names
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                Assert.AreEqual("Names: [\"Script Program\",\"test terminal\"]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void HideBlock() {
            String script = @"
hide the ""test terminal""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.ShowInTerminal = false);
            }
        }

        [TestMethod]
        public void ShowBlock() {
            String script = @"
show the ""test terminal""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.ShowInTerminal = true);
            }
        }

        [TestMethod]
        public void GetBlockPosition() {
            String script = @"print ""Position: "" + the ""test terminal"" position";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                mockBlock.Setup(b => b.GetPosition()).Returns(new Vector3D(1, 2, 3));

                test.RunUntilDone();

                Assert.AreEqual("Position: 1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetBlockDirectionDefaultsToForward() {
            String script = @"print ""Direction: "" + the ""test terminal"" direction";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockOrientation(mockBlock, new Vector3(1, 0, 0), new Vector3(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Direction: 1:0:0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetBlockForwardsDirection() {
            String script = @"print ""Forward Direction: "" + the ""test terminal"" forwards direction";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockOrientation(mockBlock, new Vector3(1, 0, 0), new Vector3(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Forward Direction: 1:0:0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetBlockBackwardsDirection() {
            String script = @"print ""Backwards Direction: "" + the ""test terminal"" backwards direction";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockOrientation(mockBlock, new Vector3(1, 0, 0), new Vector3(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Backwards Direction: -1:0:0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetBlockUpwardsDirection() {
            String script = @"print ""Upwards Direction: "" + the ""test terminal"" upwards direction";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockOrientation(mockBlock, new Vector3(1, 0, 0), new Vector3(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Upwards Direction: 0:1:0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetBlockDownwardsDirection() {
            String script = @"print ""Downwards Direction: "" + the ""test terminal"" downwards direction";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockOrientation(mockBlock, new Vector3(1, 0, 0), new Vector3(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Downwards Direction: 0:-1:0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetBlockLeftDirection() {
            String script = @"print ""Left Direction: "" + the ""test terminal"" left direction";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockOrientation(mockBlock, new Vector3(1, 0, 0), new Vector3(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Left Direction: 0:0:-1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetBlockRightDirection() {
            String script = @"print ""Right Direction: "" + the ""test terminal"" right direction";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockOrientation(mockBlock, new Vector3(1, 0, 0), new Vector3(0, 1, 0));

                test.RunUntilDone();

                Assert.AreEqual("Right Direction: 0:0:1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetBlockProperties() {
            String script = @"print ""Properties: "" + the ""test terminal"" properties";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var property1 = MockProperty<IMyTerminalBlock, float>(mockBlock, "Property1");
                var property2 = MockProperty<IMyTerminalBlock, StringBuilder>(mockBlock, "Property2");
                List<ITerminalProperty> expectedProperties = new List<ITerminalProperty> { property1.Object, property2.Object };
                mockBlock.Setup(b => b.GetProperties(It.IsAny<List<ITerminalProperty>>(), null))
                    .Callback<List<ITerminalProperty>, Func<ITerminalProperty,bool>>((list,collect)=> list.AddRange(expectedProperties));
                test.RunUntilDone();

                Assert.AreEqual("Properties: [Property1,Property2]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetBlockProperty() {
            String script = @"print ""Property1: "" + the ""test terminal"" ""Property1"" property";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockGetProperty(mockBlock, "Property1", new StringBuilder("Hello World!"));
                test.RunUntilDone();

                Assert.AreEqual("Property1: Hello World!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetBlockProperty() {
            String script = @"set the ""test terminal"" ""Property1"" property to 4";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var property = MockProperty<IMyTerminalBlock, float>(mockBlock, "Property1");

                test.RunUntilDone();

                property.Verify(p => p.SetValue(mockBlock.Object, 4));
            }
        }

        [TestMethod]
        public void GetBlockActions() {
            String script = @"print ""Actions: "" + the ""test terminal"" actions";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var action1 = MockAction(mockBlock, "Action1");
                var action2 = MockAction(mockBlock, "Action2");
                List<ITerminalAction> expectedActions = new List<ITerminalAction> { action1.Object, action2.Object };
                mockBlock.Setup(b => b.GetActions(It.IsAny<List<ITerminalAction>>(), null))
                    .Callback<List<ITerminalAction>, Func<ITerminalAction, bool>>((list, collect) => list.AddRange(expectedActions));
                test.RunUntilDone();

                Assert.AreEqual("Actions: [Action1,Action2]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ApplyBlockAction() {
            String script = @"apply the ""test terminal"" ""Action1"" action";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var action = MockAction(mockBlock, "Action1");

                test.RunUntilDone();

                action.Verify(p => p.Apply(mockBlock.Object));
            }
        }

        [TestMethod]
        public void ApplyActionOnSelector() {
            String script = @"apply the ""Action1"" action on the ""test terminal"" ";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var action = MockAction(mockBlock, "Action1");

                test.RunUntilDone();

                action.Verify(p => p.Apply(mockBlock.Object));
            }
        }

        [TestMethod]
        public void TellSelectorToApplyAction() {
            String script = @"tell the ""test terminal"" to apply the ""Action1"" action";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var action = MockAction(mockBlock, "Action1");

                test.RunUntilDone();

                action.Verify(p => p.Apply(mockBlock.Object));
            }
        }

        [TestMethod]
        public void ApplyBlockVariableAction() {
            String script = @"
set myAction to ""Action1""
apply the ""test terminal"" myAction action";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var action = MockAction(mockBlock, "Action1");

                test.RunUntilDone();

                action.Verify(p => p.Apply(mockBlock.Object));
            }
        }

        [TestMethod]
        public void ApplyVariableActionName() {
            String script = @"
set myAction to ""Action1""
apply myAction action to the ""test terminal""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var action = MockAction(mockBlock, "Action1");

                test.RunUntilDone();

                action.Verify(p => p.Apply(mockBlock.Object));
            }
        }

        [TestMethod]
        public void TellSelectorToApplyVariableAction() {
            String script = @"
set myAction to ""Action1""
tell the ""test terminal"" to apply the myAction action";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                var action = MockAction(mockBlock, "Action1");

                test.RunUntilDone();

                action.Verify(p => p.Apply(mockBlock.Object));
            }
        }
    }
}
