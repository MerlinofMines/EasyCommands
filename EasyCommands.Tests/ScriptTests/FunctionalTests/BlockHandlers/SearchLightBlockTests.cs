using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SearchlightBlockTests {
        [TestMethod]
        public void TurnOnTheSearchlight() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test searchlights""")) {
                var mockSearchlight = new Mock<IMyFunctionalBlock>();
                var mockNotSearchlight = new Mock<IMyFunctionalBlock>();
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockBlockDefinition(mockNotSearchlight, "Light");

                test.MockBlocksInGroup("test searchlights", mockSearchlight, mockNotSearchlight);

                test.RunUntilDone();

                mockSearchlight.VerifySet(b => b.Enabled = true);
                mockNotSearchlight.VerifySet(b => b.Enabled = true, Times.Never);
            }
        }

        [TestMethod]
        public void GetTheSearchlightColor() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Color: "" + the ""test searchlight"" color")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "Color", Color.Red);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Color: #FF0000"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightColor() {
            using (ScriptTest test = new ScriptTest(@"set the ""test searchlight"" color to red")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockRadius = MockProperty<IMyFunctionalBlock, Color>(mockSearchlight, "Color");

                test.RunUntilDone();

                mockRadius.Verify(p => p.SetValue(mockSearchlight.Object, Color.Red));
            }
        }

        [TestMethod]
        public void GetTheSearchlightRadius() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Radius: "" + the ""test searchlight"" radius")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "Radius", 50f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Radius: 50"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightRadius() {
            using (ScriptTest test = new ScriptTest(@"set the ""test searchlight"" radius to 50")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockRadius = MockProperty<IMyFunctionalBlock, float>(mockSearchlight, "Radius");

                test.RunUntilDone();

                mockRadius.Verify(p => p.SetValue(mockSearchlight.Object, 50f));
            }
        }

        [TestMethod]
        public void GetTheSearchlightRange() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Range: "" + the ""test searchlight"" range")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "Range", 500f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Range: 500"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightRange() {
            using (ScriptTest test = new ScriptTest(@"set the ""test searchlight"" range to 500")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockRange = MockProperty<IMyFunctionalBlock, float>(mockSearchlight, "Range");

                test.RunUntilDone();

                mockRange.Verify(p => p.SetValue(mockSearchlight.Object, 500f));
            }
        }

        [TestMethod]
        public void GetTheSearchlightInterval() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Interval: "" + the ""test searchlight"" interval")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "Blink Interval", 0.2f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Interval: 0.2"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightInterval() {
            using (ScriptTest test = new ScriptTest(@"set the ""test searchlight"" interval to 0.2")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockInterval = MockProperty<IMyFunctionalBlock, float>(mockSearchlight, "Blink Interval");

                test.RunUntilDone();

                mockInterval.Verify(p => p.SetValue(mockSearchlight.Object, 0.2f));
            }
        }

        [TestMethod]
        public void GetTheSearchlightLength() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Length: "" + the ""test searchlight"" length")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "Blink Lenght", 0.2f); //Intentionally Typod

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Length: 0.2"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightLength() {
            using (ScriptTest test = new ScriptTest(@"set the ""test searchlight"" length to 0.2")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockLength = MockProperty<IMyFunctionalBlock, float>(mockSearchlight, "Blink Lenght"); //Intentionally Typod

                test.RunUntilDone();

                mockLength.Verify(p => p.SetValue(mockSearchlight.Object, 0.2f));
            }
        }

        [TestMethod]
        public void GetTheSearchlightOffset() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Offset: "" + the ""test searchlight"" offset")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "Offset", 0.2f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Offset: 0.2"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightOffset() {
            using (ScriptTest test = new ScriptTest(@"set the ""test searchlight"" offset to 0.2")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockOffset = MockProperty<IMyFunctionalBlock, float>(mockSearchlight, "Offset");

                test.RunUntilDone();

                mockOffset.Verify(p => p.SetValue(mockSearchlight.Object, 0.2f));
            }
        }

        [TestMethod]
        public void GetTheSearchlightIntensity() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Intensity: "" + the ""test searchlight"" intensity")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "Intensity", 0.2f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Intensity: 0.2"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightIntensity() {
            using (ScriptTest test = new ScriptTest(@"set the ""test searchlight"" intensity to 0.2")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockIntensity = MockProperty<IMyFunctionalBlock, float>(mockSearchlight, "Intensity");

                test.RunUntilDone();

                mockIntensity.Verify(p => p.SetValue(mockSearchlight.Object, 0.2f));
            }
        }

        [TestMethod]
        public void GetTheSearchlightFalloff() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Falloff: "" + the ""test searchlight"" falloff")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "Falloff", 0.2f);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Falloff: 0.2"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightFalloff() {
            using (ScriptTest test = new ScriptTest(@"set the ""test searchlight"" falloff to 0.2")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockFalloff = MockProperty<IMyFunctionalBlock, float>(mockSearchlight, "Falloff");

                test.RunUntilDone();

                mockFalloff.Verify(p => p.SetValue(mockSearchlight.Object, 0.2f));
            }
        }

        [TestMethod]
        public void GetTheSearchlightRotation() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Rotation: "" + the ""test searchlight"" rotation")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "EnableIdleMovement", true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Rotation: True"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightRotation() {
            using (ScriptTest test = new ScriptTest(@"turn off""test searchlight"" rotation")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockRotation = MockProperty<IMyFunctionalBlock, bool>(mockSearchlight, "EnableIdleMovement");

                test.RunUntilDone();

                mockRotation.Verify(p => p.SetValue(mockSearchlight.Object, false));
            }
        }

        [TestMethod]
        public void GetTheSearchlightLocking() {
            using (ScriptTest test = new ScriptTest(@"Print ""Searchlight Target Locking: "" + the ""test searchlight"" locking")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                MockGetProperty(mockSearchlight, "EnableTargetLocking", true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Searchlight Target Locking: True"));
            }
        }

        [TestMethod]
        public void SetTheSearchlightLocking() {
            using (ScriptTest test = new ScriptTest(@"turn off""test searchlight"" locking")) {
                Mock<IMyFunctionalBlock> mockSearchlight = new Mock<IMyFunctionalBlock>();
                test.MockBlocksOfType("test searchlight", mockSearchlight);
                MockBlockDefinition(mockSearchlight, "Searchlight");
                var mockRotation = MockProperty<IMyFunctionalBlock, bool>(mockSearchlight, "EnableTargetLocking");

                test.RunUntilDone();

                mockRotation.Verify(p => p.SetValue(mockSearchlight.Object, false));
            }
        }
    }
}
