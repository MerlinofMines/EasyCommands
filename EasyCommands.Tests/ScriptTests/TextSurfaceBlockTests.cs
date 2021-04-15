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
