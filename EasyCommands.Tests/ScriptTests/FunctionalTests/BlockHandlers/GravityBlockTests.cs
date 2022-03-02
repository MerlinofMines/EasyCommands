using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class GravityBlockTests {
        [TestMethod]
        public void getGravityGeneratorStrength() {
            String script = @"
assign a to ""test gravityGenerator"" strength
print ""Strength: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                mockGravityGenerator.Setup(b => b.GravityAcceleration).Returns(0.1f);
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Strength: 0.1"));
            }
        }

        [TestMethod]
        public void setGravityGeneratorStrength() {
            String script = @"
set ""test gravityGenerator"" strength to 0.2
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.GravityAcceleration = 0.2f);
            }
        }

        [TestMethod]
        public void getGravityGeneratorRange() {
            String script = @"
assign a to ""test gravityGenerator"" range
print ""Range: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                mockGravityGenerator.Setup(b => b.FieldSize).Returns(new Vector3(1,2,3));
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Range: 1:2:3"));
            }
        }

        [TestMethod]
        public void getGravityGeneratorSize() {
            String script = @"
assign a to ""test gravityGenerator"" size
print ""Field Size: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                mockGravityGenerator.Setup(b => b.FieldSize).Returns(new Vector3(1, 2, 3));
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Field Size: 1:2:3"));
            }
        }

        [TestMethod]
        public void getGravityGeneratorHeight() {
            String script = @"
assign a to ""test gravityGenerator"" upper range
print ""Range: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                mockGravityGenerator.Setup(b => b.FieldSize).Returns(new Vector3(1, 2, 3));
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Range: 2"));
            }
        }

        [TestMethod]
        public void getGravityGeneratorWidth() {
            String script = @"
assign a to ""test gravityGenerator"" left range
print ""Range: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                mockGravityGenerator.Setup(b => b.FieldSize).Returns(new Vector3(1, 2, 3));
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Range: 1"));
            }
        }

        [TestMethod]
        public void getGravityGeneratorDepth() {
            String script = @"
assign a to ""test gravityGenerator"" forward range
print ""Range: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                mockGravityGenerator.Setup(b => b.FieldSize).Returns(new Vector3(1, 2, 3));
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Range: 3"));
            }
        }

        [TestMethod]
        public void setGravityGeneratorRangeVector() {
            String script = @"
set ""test gravityGenerator"" range to 2:4:6
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.FieldSize = new Vector3(2,4,6));
            }
        }

        [TestMethod]
        public void setGravityGeneratorRangeNumeric() {
            String script = @"
set ""test gravityGenerator"" range to 2
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.FieldSize = new Vector3(2, 2, 2));
            }
        }

        [TestMethod]
        public void setGravityGeneratorSizeVector() {
            String script = @"
set ""test gravityGenerator"" size to 2:4:6
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.FieldSize = new Vector3(2, 4, 6));
            }
        }

        [TestMethod]
        public void setGravityGeneratorSizeNumeric() {
            String script = @"
set ""test gravityGenerator"" size to 2
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.FieldSize = new Vector3(2, 2, 2));
            }
        }

        [TestMethod]
        public void setGravityGeneratorHeight() {
            String script = @"
set ""test gravityGenerator"" upper range to 2
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);

                mockGravityGenerator.Setup(b => b.FieldSize).Returns(new Vector3(1, 1, 1));
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.FieldSize = new Vector3(1, 2, 1));
            }
        }

        [TestMethod]
        public void setGravityGeneratorWidth() {
            String script = @"
set ""test gravityGenerator"" left range to 2
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);

                mockGravityGenerator.Setup(b => b.FieldSize).Returns(new Vector3(1, 1, 1));
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.FieldSize = new Vector3(2, 1, 1));
            }
        }

        [TestMethod]
        public void setGravityGeneratorDepth() {
            String script = @"
set ""test gravityGenerator"" forward range to 2
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGenerator>();
                test.MockBlocksOfType("test gravityGenerator", mockGravityGenerator);

                mockGravityGenerator.Setup(b => b.FieldSize).Returns(new Vector3(1, 1, 1));
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.FieldSize = new Vector3(1, 1, 2));
            }
        }

        [TestMethod]
        public void getSphericalGravityGeneratorStrength() {
            String script = @"
assign a to ""test gravitySphere"" strength
print ""Strength: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGeneratorSphere>();
                mockGravityGenerator.Setup(b => b.GravityAcceleration).Returns(0.1f);
                test.MockBlocksOfType("test gravitySphere", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Strength: 0.1"));
            }
        }

        [TestMethod]
        public void setSphericalGravityGeneratorStrength() {
            String script = @"
set ""test gravitySphere"" strength to 0.2
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGeneratorSphere>();
                test.MockBlocksOfType("test gravitySphere", mockGravityGenerator);
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.GravityAcceleration = 0.2f);
            }
        }

        [TestMethod]
        public void GetSphericalGravityGeneratorRadius() {
            using (var test = new ScriptTest(@"Print ""Radius: "" + ""test gravitySphere"" radius")) {
                var mockGravityGenerator = new Mock<IMyGravityGeneratorSphere>();
                mockGravityGenerator.Setup(b => b.Radius).Returns(100f);
                test.MockBlocksOfType("test gravitySphere", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Radius: 100"));
            }
        }

        [TestMethod]
        public void SetSphericalGravityGeneratorRadius() {
            using (var test = new ScriptTest(@"set ""test gravitySphere"" radius to 200")) {
                var mockGravityGenerator = new Mock<IMyGravityGeneratorSphere>();
                test.MockBlocksOfType("test gravitySphere", mockGravityGenerator);
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.Radius = 200f);
            }
        }

        [TestMethod]
        public void GetSphericalGravityGeneratorRange() {
            using (var test = new ScriptTest(@"Print ""Range: "" + ""test gravitySphere"" range")) {
                var mockGravityGenerator = new Mock<IMyGravityGeneratorSphere>();
                mockGravityGenerator.Setup(b => b.Radius).Returns(100f);
                test.MockBlocksOfType("test gravitySphere", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Range: 100"));
            }
        }

        [TestMethod]
        public void SetSphericalGravityGeneratorRange() {
            using (var test = new ScriptTest(@"set ""test gravitySphere"" range to 200")) {
                var mockGravityGenerator = new Mock<IMyGravityGeneratorSphere>();
                test.MockBlocksOfType("test gravitySphere", mockGravityGenerator);
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.Radius = 200f);
            }
        }

        [TestMethod]
        public void getSphericalGravityGeneratorSize() {
            String script = @"
assign a to ""test gravitySphere"" size
print ""Size: "" + a
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGeneratorSphere>();
                mockGravityGenerator.Setup(b => b.Radius).Returns(100f);
                test.MockBlocksOfType("test gravitySphere", mockGravityGenerator);
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Size: 100"));
            }
        }

        [TestMethod]
        public void setSphericalGravityGeneratorSize() {
            String script = @"
set ""test gravitySphere"" size to 200
";
            using (var test = new ScriptTest(script)) {
                var mockGravityGenerator = new Mock<IMyGravityGeneratorSphere>();
                test.MockBlocksOfType("test gravitySphere", mockGravityGenerator);
                test.RunUntilDone();

                mockGravityGenerator.VerifySet(b => b.Radius = 200f);
            }
        }
    }
}
