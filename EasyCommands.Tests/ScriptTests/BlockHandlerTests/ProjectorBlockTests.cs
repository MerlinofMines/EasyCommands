using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ProjectorBlockTests {

        [TestMethod]
        public void GetProjectorOffsetVector() {
            using (var test = new ScriptTest(@"print ""Offset: "" + the ""test projector"" offset")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1,2,3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Offset: 1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorUpOffset() {
            using (var test = new ScriptTest(@"print ""Up Offset: "" + the ""test projector"" up offset")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Up Offset: 2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorDownOffset() {
            using (var test = new ScriptTest(@"print ""Down Offset: "" + the ""test projector"" down offset")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Down Offset: -2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorLeftOffset() {
            using (var test = new ScriptTest(@"print ""Left Offset: "" + the ""test projector"" left offset")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Left Offset: -1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorRightOffset() {
            using (var test = new ScriptTest(@"print ""Right Offset: "" + the ""test projector"" right offset")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Right Offset: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorForwardOffset() {
            using (var test = new ScriptTest(@"print ""Forward Offset: "" + the ""test projector"" forward offset")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Forward Offset: 3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorBackwardOffset() {
            using (var test = new ScriptTest(@"print ""Backward Offset: "" + the ""test projector"" backward offset")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Backward Offset: -3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetProjectorOffsetVector() {
            using (var test = new ScriptTest(@"set ""test projector"" offset to 2:4:6")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionOffset = new Vector3I(2,4,6));
            }
        }

        [TestMethod]
        public void SetProjectorUpOffset() {
            using (var test = new ScriptTest(@"set ""test projector"" up offset to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionOffset = new Vector3I(1, 2, 1));
            }
        }

        [TestMethod]
        public void SetProjectorDownOffset() {
            using (var test = new ScriptTest(@"set ""test projector"" down offset to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionOffset = new Vector3I(1, -2, 1));
            }
        }

        [TestMethod]
        public void SetProjectorRightOffset() {
            using (var test = new ScriptTest(@"set ""test projector"" right offset to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionOffset = new Vector3I(2, 1, 1));
            }
        }

        [TestMethod]
        public void SetProjectorLeftOffset() {
            using (var test = new ScriptTest(@"set ""test projector"" left offset to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionOffset = new Vector3I(-2, 1, 1));
            }
        }

        [TestMethod]
        public void SetProjectorForwardOffset() {
            using (var test = new ScriptTest(@"set ""test projector"" forward offset to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionOffset = new Vector3I(1, 1, 2));
            }
        }

        [TestMethod]
        public void SetProjectorBackwardOffset() {
            using (var test = new ScriptTest(@"set ""test projector"" backward offset to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionOffset).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionOffset = new Vector3I(1, 1, -2));
            }
        }

        [TestMethod]
        public void GetProjectorRotationVector() {
            using (var test = new ScriptTest(@"print ""Rotation: "" + the ""test projector"" rotation")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Rotation: 1:2:3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorUpRotation() {
            using (var test = new ScriptTest(@"print ""Up Rotation: "" + the ""test projector"" up rotation")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Up Rotation: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorDownRotation() {
            using (var test = new ScriptTest(@"print ""Down Rotation: "" + the ""test projector"" down rotation")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Down Rotation: -1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorLeftRotation() {
            using (var test = new ScriptTest(@"print ""Left Rotation: "" + the ""test projector"" left rotation")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Left Rotation: -2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorRightRotation() {
            using (var test = new ScriptTest(@"print ""Right Rotation: "" + the ""test projector"" right rotation")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Right Rotation: 2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorClockwiseRotation() {
            using (var test = new ScriptTest(@"print ""Clockwise Rotation: "" + the ""test projector"" clockwise rotation")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Clockwise Rotation: 3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorBackwardRotation() {
            using (var test = new ScriptTest(@"print ""Counterclockwise Rotation: "" + the ""test projector"" counter rotation")) {
                var mockProjector = new Mock<IMyProjector>();
                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 2, 3));
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Counterclockwise Rotation: -3", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetProjectorRotationVector() {
            using (var test = new ScriptTest(@"set ""test projector"" rotation to 1:-1:0")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(1, -1, 0));
            }
        }

        [TestMethod]
        public void SetProjectorRotationVectorClamped() {
            using (var test = new ScriptTest(@"set ""test projector"" rotation to 3:-4:6")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(2, -2, 2));
            }
        }

        [TestMethod]
        public void SetProjectorUpRotation() {
            using (var test = new ScriptTest(@"set ""test projector"" up rotation to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(2, 1, 1));
            }
        }

        [TestMethod]
        public void SetProjectorUpRotationClampedUpper() {
            using (var test = new ScriptTest(@"set ""test projector"" up rotation to 3")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(2, 1, 1));
            }
        }

        [TestMethod]
        public void SetProjectorDownRotation() {
            using (var test = new ScriptTest(@"set ""test projector"" down rotation to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(-2, 1, 1));
            }
        }

        [TestMethod]
        public void SetProjectorDownRotationClamped() {
            using (var test = new ScriptTest(@"set ""test projector"" down rotation to 3")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(-2, 1, 1));
            }
        }

        [TestMethod]
        public void SetProjectorRightRotation() {
            using (var test = new ScriptTest(@"set ""test projector"" right rotation to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(1, 2, 1));
            }
        }

        [TestMethod]
        public void SetProjectorRightRotationClamped() {
            using (var test = new ScriptTest(@"set ""test projector"" right rotation to 3")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(1, 2, 1));
            }
        }

        [TestMethod]
        public void SetProjectorLeftRotation() {
            using (var test = new ScriptTest(@"set ""test projector"" left rotation to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(1, -2, 1));
            }
        }

        [TestMethod]
        public void SetProjectorLeftRotationClamped() {
            using (var test = new ScriptTest(@"set ""test projector"" left rotation to 3")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(1, -2, 1));
            }
        }

        [TestMethod]
        public void SetProjectorClockwiseRotation() {
            using (var test = new ScriptTest(@"set ""test projector"" clockwise rotation to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(1, 1, 2));
            }
        }

        [TestMethod]
        public void SetProjectorClockwiseRotationClamped() {
            using (var test = new ScriptTest(@"set ""test projector"" clockwise rotation to 3")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(1, 1, 2));
            }
        }

        [TestMethod]
        public void SetProjectorCounterClockwiseRotation() {
            using (var test = new ScriptTest(@"set ""test projector"" counter rotation to 2")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(1, 1, -2));
            }
        }

        [TestMethod]
        public void SetProjectorCounterClockwiseRotationClamped() {
            using (var test = new ScriptTest(@"set ""test projector"" counter rotation to 3")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                mockProjector.Setup(b => b.ProjectionRotation).Returns(new Vector3I(1, 1, 1));

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ProjectionRotation = new Vector3I(1, 1, -2));
            }
        }

        [TestMethod]
        public void GetProjectorIsComplete() {
            String script = @"print ""Complete: "" + the""test projector"" is complete";
            using (var test = new ScriptTest(script)) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                mockProjector.Setup(b => b.RemainingBlocks).Returns(0);

                test.RunUntilDone();

                Assert.AreEqual("Complete: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorIsCompleteWhenNotComplete() {
            String script = @"print ""Complete: "" + the""test projector"" is complete";
            using (var test = new ScriptTest(script)) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                mockProjector.Setup(b => b.RemainingBlocks).Returns(1);

                test.RunUntilDone();

                Assert.AreEqual("Complete: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorRatio() {
            String script = @"print ""Completion Ratio: "" + the ""test projector"" ratio";
            using (var test = new ScriptTest(script)) {
                var mockProjector = new Mock<IMyProjector>(); 
                test.MockBlocksOfType("test projector", mockProjector);
                mockProjector.Setup(b => b.RemainingBlocks).Returns(10);
                mockProjector.Setup(b => b.TotalBlocks).Returns(20);

                test.RunUntilDone();

                Assert.AreEqual("Completion Ratio: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetProjectorKeepProjection() {
            String script = @"print ""Keep Projection: "" + the""test projector"" is locked";
            using (var test = new ScriptTest(script)) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                MockGetProperty(mockProjector, "KeepProjection", true);

                test.RunUntilDone();

                Assert.AreEqual("Keep Projection: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetProjectorKeepProjection() {
            using (var test = new ScriptTest(@"lock the ""test projector""")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                var mockScale = MockProperty<IMyProjector, bool>(mockProjector, "KeepProjection");

                test.RunUntilDone();

                mockScale.Verify(property => property.SetValue(mockProjector.Object, true));
            }
        }

        [TestMethod]
        public void IsTheProjectorShowing() {
            String script = @"print ""Showing: "" + the""test projector"" is showing";
            using (var test = new ScriptTest(script)) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                mockProjector.Setup(b => b.IsProjecting).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Showing: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheProjectorShowingWhenNotShowing() {
            String script = @"print ""Showing: "" + the""test projector"" is showing";
            using (var test = new ScriptTest(script)) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                mockProjector.Setup(b => b.IsProjecting).Returns(false);

                test.RunUntilDone();

                Assert.AreEqual("Showing: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ShowTheProjector() {
            using (var test = new ScriptTest(@"show the ""test projector""")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ShowOnlyBuildable = false);
            }
        }

        [TestMethod]
        public void HideTheProjector() {
            using (var test = new ScriptTest(@"hide the ""test projector""")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);

                test.RunUntilDone();

                mockProjector.VerifySet(b => b.ShowOnlyBuildable = true);
            }
        }

        [TestMethod]
        public void GetProjectorScale() {
            String script = @"print ""Scale: "" + the""test projector"" scale";
            using (var test = new ScriptTest(script)) {
                var mockProjector = new Mock<IMyProjector>();
                MockGetProperty(mockProjector, "Scale", 0.5f);
                test.MockBlocksOfType("test projector", mockProjector);
                test.RunUntilDone();

                Assert.AreEqual("Scale: 0.5", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetProjectorScale() {
            using (var test = new ScriptTest(@"set ""test projector"" scale to 0.8")) {
                var mockProjector = new Mock<IMyProjector>();
                test.MockBlocksOfType("test projector", mockProjector);
                var mockScale = MockProperty<IMyProjector, float>(mockProjector, "Scale");

                test.RunUntilDone();

                mockScale.Verify(property => property.SetValue(mockProjector.Object, 0.8f));
            }
        }
    }
}
