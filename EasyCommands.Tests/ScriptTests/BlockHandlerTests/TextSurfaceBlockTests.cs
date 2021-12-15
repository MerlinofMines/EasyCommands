using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class TextSurfaceBlockTests {
        [TestMethod]
        public void SetTextPanelText() {
            String script = @"
set the ""text panel"" display text to ""Hello World""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.Verify(b => b.WriteText("Hello World", false));
            }
        }

        [TestMethod]
        public void SetTextPanelGroupText() {
            String script = @"
set the ""text panels"" display group text to ""Hello World""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                var mockTextPanel2 = new Mock<IMyTextPanel>();
                test.MockBlocksInGroup("text panels", mockTextPanel, mockTextPanel2);

                test.RunUntilDone();

                mockTextPanel.Verify(b => b.WriteText("Hello World", false));
                mockTextPanel2.Verify(b => b.WriteText("Hello World", false));
            }
        }

        [TestMethod]
        public void SetTextPanelTextMultiLine() {
            String script = @"
set the ""text panel"" display text to ""Hello World\nThis is my life""
";

            var expectedText = "Hello World\nThis is my life";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.Verify(b => b.WriteText(expectedText, false));
            }
        }

        [TestMethod]
        public void IncrementTextPanelTextMultiLine() {
            String script = @"
set the ""text panel"" display text to ""Hello World""
wait
increase the ""text panel"" display text by ""\nThis is my life""
";

            var expectedText = "Hello World\nThis is my life";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();

                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunOnce();

                mockTextPanel.Verify(b => b.WriteText("Hello World", false));

                mockTextPanel.Setup(b => b.GetText()).Returns("Hello World");

                test.RunOnce();

                mockTextPanel.Verify(b => b.WriteText(expectedText, false));
            }
        }

        [TestMethod]
        public void SetProgrammableBlockDisplayText() {
            String script = @"
set the ""test program"" display @ 0 text to ""Hello World""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockProgram = new Mock<IMyProgrammableBlock>();
                var mockDisplay = new Mock<IMyTextSurface>();

                MockTextSurfaces(mockProgram, mockDisplay);

                test.MockBlocksOfType("test program", mockProgram);

                test.RunUntilDone();

                mockDisplay.Verify(b => b.WriteText("Hello World", false));
            }
        }

        /**
         * Since Blocks with displays often have more than 1, ensure that we are still referring to
         * a block, not a block group.
         */
        [TestMethod]
        public void SetProgrammableBlockDisplaysText() {
            String script = @"
set the ""test program"" displays @ 0 text to ""Hello World""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockProgram = new Mock<IMyProgrammableBlock>();
                var mockDisplay = new Mock<IMyTextSurface>();

                MockTextSurfaces(mockProgram, mockDisplay);

                test.MockBlocksOfType("test program", mockProgram);

                test.RunUntilDone();

                mockDisplay.Verify(b => b.WriteText("Hello World", false));
            }
        }

        /**
 * Since Blocks with displays often have more than 1, ensure that we are still referring to
 * a block, not a block group.
 */
        [TestMethod]
        public void SetMyDisplaysText() {
            String script = @"
set my displays @ 0 text to ""Hello World""
";

            using (ScriptTest test = new ScriptTest(script)) {
                test.RunUntilDone();

                test.display.Verify(b => b.WriteText("Hello World", false));
            }
        }

        [TestMethod]
        public void SetProgrammableBlockKeyboardText() {
            String script = @"
set the ""test program"" display @ 1 text to ""Hello World""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockProgram = new Mock<IMyProgrammableBlock>();
                var mockDisplay = new Mock<IMyTextSurface>();
                var mockKeyboard = new Mock<IMyTextSurface>();

                MockTextSurfaces(mockProgram, mockDisplay, mockKeyboard);

                test.MockBlocksOfType("test program", mockProgram);

                test.RunUntilDone();

                mockKeyboard.Verify(b => b.WriteText("Hello World", false));
            }
        }

        [TestMethod]
        public void SetCockpitDisplayText() {
            String script = @"
set the ""test cockpit"" display @ 0 text to ""Hello World""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockCockpit = new Mock<IMyCockpit>();
                var mockDisplay = new Mock<IMyTextSurface>();

                MockTextSurfaces(mockCockpit, mockDisplay);

                test.MockBlocksOfType("test cockpit", mockCockpit);

                test.RunUntilDone();

                mockDisplay.Verify(b => b.WriteText("Hello World", false));
            }
        }

        [TestMethod]
        public void SetCockpitKeyboardText() {
            String script = @"
set the ""test cockpit"" display @ 1 text to ""Hello World""
";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockCockpit = new Mock<IMyCockpit>();
                var mockDisplay = new Mock<IMyTextSurface>();
                var mockKeyboard = new Mock<IMyTextSurface>();

                MockTextSurfaces(mockCockpit, mockDisplay, mockKeyboard);

                test.MockBlocksOfType("test cockpit", mockCockpit);

                test.RunUntilDone();

                mockKeyboard.Verify(b => b.WriteText("Hello World", false));
            }
        }
    }
}
