using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using VRage.Game.GUI.TextPanel;
using System.Collections.Generic;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class TextSurfaceBlockTests {
        [TestMethod]
        public void EnableTextPanel() {
            String script = @"enable the ""text panel"" display";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ContentType = ContentType.TEXT_AND_IMAGE);
            }
        }

        [TestMethod]
        public void DisableTextPanel() {
            String script = @"disable the ""text panel"" display";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ContentType = ContentType.NONE);
            }
        }

        [TestMethod]
        public void IsTextPanelEnabled() {
            String script = @"Print ""Enabled: ""+ the ""text panel"" display is enabled";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.TEXT_AND_IMAGE);
                test.RunUntilDone();

                Assert.AreEqual("Enabled: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTextPanelText() {
            String script = @"Print ""Text: ""+ the ""text panel"" display text";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.GetText()).Returns("Hello World!");
                test.RunUntilDone();

                Assert.AreEqual("Text: Hello World!", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelText() {
            String script = @"set the ""text panel"" display text to ""Hello World""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ContentType = ContentType.TEXT_AND_IMAGE);
                mockTextPanel.Verify(b => b.WriteText("Hello World", false));
            }
        }

        [TestMethod]
        public void SetTextPanelGroupText() {
            String script = @"set the ""text panels"" display group text to ""Hello World""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                var mockTextPanel2 = new Mock<IMyTextPanel>();
                test.MockBlocksInGroup("text panels", mockTextPanel, mockTextPanel2);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ContentType = ContentType.TEXT_AND_IMAGE);
                mockTextPanel.Verify(b => b.WriteText("Hello World", false));

                mockTextPanel2.Verify(b => b.WriteText("Hello World", false));
                mockTextPanel2.VerifySet(b => b.ContentType = ContentType.TEXT_AND_IMAGE);
            }
        }

        [TestMethod]
        public void SetTextPanelTextMultiLine() {
            String script = @"set the ""text panel"" display text to ""Hello World\nThis is my life""";

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
            String script = @"set the ""test program"" display @ 0 text to ""Hello World""";

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
            String script = @"set the ""test program"" displays @ 0 text to ""Hello World""";

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
            String script = @"set my displays @ 0 text to ""Hello World""";

            using (ScriptTest test = new ScriptTest(script)) {
                test.RunUntilDone();

                test.display.Verify(b => b.WriteText("Hello World", false));
            }
        }

        [TestMethod]
        public void SetProgrammableBlockKeyboardText() {
            String script = @"set the ""test program"" display @ 1 text to ""Hello World""";

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
            String script = @"set the ""test cockpit"" display @ 0 text to ""Hello World""";

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
            String script = @"set the ""test cockpit"" display @ 1 text to ""Hello World""";

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

        [TestMethod]
        public void GetTextPanelImage() {
            String script = @"Print ""Image: ""+ the ""text panel"" display image";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.CurrentlyShownImage).Returns("myImage");
                test.RunUntilDone();

                Assert.AreEqual("Image: myImage", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTextPanelImageWhenNoneReturnsEmpty() {
            String script = @"Print ""Image: "" + the ""text panel"" display image";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.CurrentlyShownImage).Returns((string)null);
                test.RunUntilDone();

                Assert.AreEqual("Image: ", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelImage() {
            String script = @"set the ""text panel"" display image to ""myImage""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ContentType = ContentType.TEXT_AND_IMAGE);
                mockTextPanel.Verify(b => b.ClearImagesFromSelection());
                mockTextPanel.Verify(b => b.AddImagesToSelection(new List<string> { "myImage" }, false));
            }
        }

        [TestMethod]
        public void GetTextPanelImages() {
            String script = @"Print ""Images: ""+ the ""text panel"" display images";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                List<string> expectedList = new List<string>();
                mockTextPanel.Setup(b => b.GetSelectedImages(expectedList)).Callback<List<string>>(t => {
                    t.Add("myImage1");
                    t.Add("myImage2");
                });

                test.RunUntilDone();

                Assert.AreEqual("Images: [myImage1,myImage2]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelImages() {
            String script = @"set the ""text panel"" display images to [""myImage1"", ""myImage2""]";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ContentType = ContentType.TEXT_AND_IMAGE);
                mockTextPanel.Verify(b => b.ClearImagesFromSelection());
                mockTextPanel.Verify(b => b.AddImagesToSelection(new List<string> { "myImage1", "myImage2" }, false));
            }
        }

        [TestMethod]
        public void GetTextPanelScript() {
            String script = @"Print ""Script: ""+ the ""text panel"" display script";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.Script).Returns("myScript");

                test.RunUntilDone();

                Assert.AreEqual("Script: myScript", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTextPanelScriptWhenNoneReturnsEmpty() {
            String script = @"Print ""Script: "" + the ""text panel"" display script";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.Script).Returns((string)null);

                test.RunUntilDone();

                Assert.AreEqual("Script: ", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelScript() {
            String script = @"set the ""text panel"" display script to ""myScript""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ContentType = ContentType.SCRIPT);
                mockTextPanel.VerifySet(b => b.Script = "myScript");
            }
        }

        [TestMethod]
        public void GetTextPanelPadding() {
            String script = @"Print ""Padding: ""+ the ""text panel"" display padding";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.TextPadding).Returns(2);

                test.RunUntilDone();

                Assert.AreEqual("Padding: 2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelPadding() {
            String script = @"set the ""text panel"" display padding to 2";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.TextPadding = 2);
            }
        }

        [TestMethod]
        public void GetTextPanelRatio() {
            String script = @"Print ""Preserving Ratio: ""+ the ""text panel"" display ratio is true";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.PreserveAspectRatio).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Preserving Ratio: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelRatio() {
            String script = @"set the ""text panel"" display ratio to true";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.PreserveAspectRatio = true);
            }
        }

        [TestMethod]
        public void GetTextPanelAlignmentWhenLeft() {
            String script = @"Print ""Alignment: ""+ the ""text panel"" display alignment";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.Alignment).Returns(TextAlignment.LEFT);

                test.RunUntilDone();

                Assert.AreEqual("Alignment: left", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTextPanelAlignmentWhenCenter() {
            String script = @"Print ""Alignment: ""+ the ""text panel"" display alignment";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.Alignment).Returns(TextAlignment.CENTER);

                test.RunUntilDone();

                Assert.AreEqual("Alignment: center", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTextPanelAlignmentWhenRight() {
            String script = @"Print ""Alignment: ""+ the ""text panel"" display alignment";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.Alignment).Returns(TextAlignment.RIGHT);

                test.RunUntilDone();

                Assert.AreEqual("Alignment: right", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelAlignmentLeft() {
            String script = @"set the ""text panel"" display alignment to ""left""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.Alignment = TextAlignment.LEFT);
            }
        }

        [TestMethod]
        public void SetTextPanelAlignmentCenter() {
            String script = @"set the ""text panel"" display alignment to ""center""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.Alignment = TextAlignment.CENTER);
            }
        }

        [TestMethod]
        public void SetTextPanelAlignmentRight() {
            String script = @"set the ""text panel"" display alignment to ""right""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.Alignment = TextAlignment.RIGHT);
            }
        }

        [TestMethod]
        public void SetTextPanelAlignmentLeftOnUnknownValue() {
            String script = @"set the ""text panel"" display alignment to ""unknown""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.Alignment = TextAlignment.LEFT);
            }
        }

        [TestMethod]
        public void GetTextPanelChangeInterval() {
            String script = @"Print ""Change Interval: ""+ the ""text panel"" display interval";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ChangeInterval).Returns(3);

                test.RunUntilDone();

                Assert.AreEqual("Change Interval: 3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelChangeInterval() {
            String script = @"set the ""text panel"" display interval to 3";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ChangeInterval = 3);
            }
        }

        [TestMethod]
        public void GetTextPanelFontSize() {
            String script = @"Print ""Font Size: ""+ the ""text panel"" display size";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.FontSize).Returns(3);

                test.RunUntilDone();

                Assert.AreEqual("Font Size: 3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelFontSizeUsingSize() {
            String script = @"set the ""text panel"" display size to 3";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.FontSize = 3);
            }
        }

        [TestMethod]
        public void SetTextPanelFontSizeUsingFont() {
            String script = @"set the ""text panel"" display font to 3";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.FontSize = 3);
            }
        }

        [TestMethod]
        public void GetTextPanelFontColorWhenText() {
            String script = @"Print ""Font Color: ""+ the ""text panel"" display color";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.TEXT_AND_IMAGE);
                mockTextPanel.Setup(b => b.FontColor).Returns(Color.Red);

                test.RunUntilDone();

                Assert.AreEqual("Font Color: #FF0000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTextPanelFontColorWhenScript() {
            String script = @"Print ""Script Font Color: ""+ the ""text panel"" display color";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.SCRIPT);
                mockTextPanel.Setup(b => b.ScriptForegroundColor).Returns(Color.Red);

                test.RunUntilDone();

                Assert.AreEqual("Script Font Color: #FF0000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelFontColorUsingColor() {
            String script = @"set the ""text panel"" display color to red";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.TEXT_AND_IMAGE);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.FontColor = Color.Red);
            }
        }

        [TestMethod]
        public void SetTextPanelFontColorUsingFont() {
            String script = @"set the ""text panel"" display font to red";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.TEXT_AND_IMAGE);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.FontColor = Color.Red);
            }
        }

        [TestMethod]
        public void SetTextPanelScriptForegroundColorUsingColor() {
            String script = @"set the ""text panel"" display color to red";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.SCRIPT);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ScriptForegroundColor = Color.Red);
            }
        }

        [TestMethod]
        public void SetTextPanelScriptForegroundColorUsingFont() {
            String script = @"set the ""text panel"" display font to red";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.SCRIPT);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ScriptForegroundColor = Color.Red);
            }
        }

        [TestMethod]
        public void GetTextPanelBackgroundColorWhenText() {
            String script = @"Print ""Background Color: ""+ the ""text panel"" display background";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.TEXT_AND_IMAGE);
                mockTextPanel.Setup(b => b.BackgroundColor).Returns(Color.Red);

                test.RunUntilDone();

                Assert.AreEqual("Background Color: #FF0000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetTextPanelBackgroundColorWhenScript() {
            String script = @"Print ""Script Background Color: ""+ the ""text panel"" display background";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.SCRIPT);
                mockTextPanel.Setup(b => b.ScriptBackgroundColor).Returns(Color.Red);

                test.RunUntilDone();

                Assert.AreEqual("Script Background Color: #FF0000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelBackgroundColor() {
            String script = @"set the ""text panel"" display background to red";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.TEXT_AND_IMAGE);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.BackgroundColor = Color.Red);
            }
        }

        [TestMethod]
        public void SetTextPanelScriptBackgroundColor() {
            String script = @"set the ""text panel"" display background to red";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.ContentType).Returns(ContentType.SCRIPT);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.ScriptBackgroundColor = Color.Red);
            }
        }

        [TestMethod]
        public void GetTextPanelFont() {
            String script = @"Print ""Font: ""+ the ""text panel"" display font";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);
                mockTextPanel.Setup(b => b.Font).Returns("DEBUG");

                test.RunUntilDone();

                Assert.AreEqual("Font: DEBUG", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetTextPanelFont() {
            String script = @"set the ""text panel"" display font to ""DEBUG""";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockTextPanel = new Mock<IMyTextPanel>();
                test.MockBlocksOfType("text panel", mockTextPanel);

                test.RunUntilDone();

                mockTextPanel.VerifySet(b => b.Font = "DEBUG");
            }
        }
    }
}
