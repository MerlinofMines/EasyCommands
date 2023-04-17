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
    public class TerminalBlockTests {
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
        public void GetBlockCustomData() {
            String script = @"
print my data
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();

                test.RunUntilDone();

                Assert.AreEqual(script, test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetBlockCustomData() {
            String script = @"
set ""test terminal"" data to ""custom data""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);

                test.RunUntilDone();

                mockBlock.VerifySet(b => b.CustomData = "custom data");
            }
        }

        [TestMethod]
        public void GetBlockDetailedInfo() {
            String script = @"
print ""Detailed Info: "" + ""test terminal"" info
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                mockBlock.Setup(b => b.DetailedInfo).Returns("all the details");

                test.RunUntilDone();

                Assert.AreEqual("Detailed Info: all the details", test.Logger[0]);
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
                MockWorldMatrix(mockBlock, Vector3D.Zero, Vector3D.UnitX, Vector3D.UnitY);

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
                MockWorldMatrix(mockBlock, Vector3D.Zero, Vector3D.UnitX, Vector3D.UnitY);

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
                MockWorldMatrix(mockBlock, Vector3D.Zero, Vector3D.UnitX, Vector3D.UnitY);

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
                MockWorldMatrix(mockBlock, Vector3D.Zero, Vector3D.UnitX, Vector3D.UnitY);

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
                MockWorldMatrix(mockBlock, Vector3D.Zero, Vector3D.UnitX, Vector3D.UnitY);

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
                MockWorldMatrix(mockBlock, Vector3D.Zero, Vector3D.UnitX, Vector3D.UnitY);

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
                MockWorldMatrix(mockBlock, Vector3D.Zero, Vector3D.UnitX, Vector3D.UnitY);

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
                    .Callback<List<ITerminalProperty>, Func<ITerminalProperty, bool>>((list, collect) => list.AddRange(expectedProperties));
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
        public void GetBlockPropertyWithReservedKeywordName() {
            String script = @"print ""Font: "" + the ""test terminal"" ""Font"" property";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockGetProperty(mockBlock, "Font", new StringBuilder("Hello World!"));
                test.RunUntilDone();

                Assert.AreEqual("Font: Hello World!", test.Logger[0]);
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

        [TestMethod]
        public void IsBuilt() {
            using (ScriptTest test = new ScriptTest(@"Print ""Is Built: "" + ""test terminal"" is built")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 4000, 0);

                test.RunUntilDone();

                Assert.AreEqual("Is Built: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsBuiltWhenNotBuilt() {
            using (ScriptTest test = new ScriptTest(@"Print ""Is Built: "" + ""test terminal"" is built")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 2000, 200);

                test.RunUntilDone();

                Assert.AreEqual("Is Built: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsComplete() {
            using (ScriptTest test = new ScriptTest(@"Print ""Complete: "" + ""test terminal"" is complete")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 4000, 0);

                test.RunUntilDone();

                Assert.AreEqual("Complete: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsFinishedBuilding() {
            using (ScriptTest test = new ScriptTest(@"Print ""Complete: "" + ""test terminal"" is finished building")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 4000, 0);

                test.RunUntilDone();

                Assert.AreEqual("Complete: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsFinishedBeingBuilt() {
            using (ScriptTest test = new ScriptTest(@"Print ""Complete: "" + ""test terminal"" is finished being built")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 4000, 0);

                test.RunUntilDone();

                Assert.AreEqual("Complete: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void BuildLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Build Limit: "" + ""test terminal"" build limit")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 2000, 200);

                test.RunUntilDone();

                Assert.AreEqual("Build Limit: 4000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void BuildLevel() {
            using (ScriptTest test = new ScriptTest(@"Print ""Build Level: "" + ""test terminal"" build level")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 2200, 200);

                test.RunUntilDone();

                Assert.AreEqual("Build Level: 2000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void BuildRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Build Ratio: "" + ""test terminal"" build ratio")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 3200, 200);

                test.RunUntilDone();

                Assert.AreEqual("Build Ratio: 0.75", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsDamaged() {
            using (ScriptTest test = new ScriptTest(@"Print ""Is Damaged: "" + ""test terminal"" is damaged")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 4000, 200);

                test.RunUntilDone();

                Assert.AreEqual("Is Damaged: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsDamagedWhenNotDamaged() {
            using (ScriptTest test = new ScriptTest(@"Print ""Is Damaged: "" + ""test terminal"" is damaged")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 4000, 0);

                test.RunUntilDone();

                Assert.AreEqual("Is Damaged: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetDamage() {
            using (ScriptTest test = new ScriptTest(@"Print ""Damage: "" + ""test terminal"" damage")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 3000, 200);

                test.RunUntilDone();

                Assert.AreEqual("Damage: 1200", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetDamageLevel() {
            using (ScriptTest test = new ScriptTest(@"Print ""Damage: "" + ""test terminal"" damage level")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 3000, 200);

                test.RunUntilDone();

                Assert.AreEqual("Damage: 1200", test.Logger[0]);
            }
        }

        [TestMethod]
        public void DamageLimit() {
            using (ScriptTest test = new ScriptTest(@"Print ""Build Limit: "" + ""test terminal"" damage limit")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 2000, 200);

                test.RunUntilDone();

                Assert.AreEqual("Build Limit: 4000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void DamageRatio() {
            using (ScriptTest test = new ScriptTest(@"Print ""Damage Ratio: "" + ""test terminal"" damage ratio")) {
                var mockBlock = new Mock<IMyTerminalBlock>();
                test.MockBlocksOfType("test terminal", mockBlock);
                MockBuildIntegrity(mockBlock, 4000, 1200, 200);

                test.RunUntilDone();

                Assert.AreEqual("Damage Ratio: 0.75", test.Logger[0]);
            }
        }
    }
}
