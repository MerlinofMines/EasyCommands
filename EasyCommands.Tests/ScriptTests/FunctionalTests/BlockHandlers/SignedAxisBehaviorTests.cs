using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Globalization;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    // Exercise public script syntax and game API values, without inspecting handlers or closures.
    [TestClass]
    public class SignedAxisBehaviorTests {
        public static IEnumerable<object[]> ControllerCases() {
            var mappings = new[] {
                new object[] { "velocity", "up", 3f }, new object[] { "velocity", "down", -3f },
                new object[] { "velocity", "left", -2f }, new object[] { "velocity", "right", 2f },
                new object[] { "velocity", "forward", -6f }, new object[] { "velocity", "backward", 6f },
                new object[] { "input", "up", 3f }, new object[] { "input", "down", -3f },
                new object[] { "input", "left", -2f }, new object[] { "input", "right", 2f },
                new object[] { "input", "forward", -6f }, new object[] { "input", "backward", 6f },
                new object[] { "roll", "up", -2f }, new object[] { "roll", "down", 2f },
                new object[] { "roll", "left", -3f }, new object[] { "roll", "right", 3f },
                new object[] { "roll", "counterclockwise", -4f }, new object[] { "roll", "clockwise", 4f }
            };
            foreach (var mapping in mappings)
                foreach (var factor in new[] { -1f, 0f, 1f })
                    foreach (var rotated in new[] { false, true })
                        yield return new[] { mapping[0], mapping[1], mapping[2], factor, rotated };
        }

        [DataTestMethod]
        [DynamicData(nameof(ControllerCases), DynamicDataSourceType.Method)]
        public void ControllerDirectionsReadLocalAxesAndIgnoreWrites(string property, string direction, float expected, float factor, bool rotated) {
            var target = "\"test cockpit\" " + direction + " " + property;
            using (var test = new ScriptTest("print " + target + "\nset " + target + " to 17\nincrease " + target + " by 2\ndecrease " + target + " by 3\nprint " + target)) {
                var block = new Mock<IMyCockpit>();
                test.MockBlocksOfType("test cockpit", block);
                // A 90-degree orientation makes world X/Y differ from local X/Y.
                var matrix = rotated ? MatrixD.CreateRotationZ(Math.PI / 2) : MatrixD.Identity;
                MockShipVelocities(block, rotated ? new Vector3D(-3, 2, 6) * factor : new Vector3D(2, 3, 6) * factor, Vector3D.Zero);
                block.Setup(b => b.WorldMatrix).Returns(matrix);
                block.Setup(b => b.MoveIndicator).Returns(new Vector3(2, 3, 6) * factor);
                block.Setup(b => b.RotationIndicator).Returns(new Vector2(18, 27) * factor);
                block.Setup(b => b.RollIndicator).Returns(4 * factor);
                test.RunUntilDone();
                AssertNumbers(test, expected * factor, expected * factor);
                block.VerifySet(b => b.DampenersOverride = It.IsAny<bool>(), Times.Never);
                block.VerifySet(b => b.IsMainCockpit = It.IsAny<bool>(), Times.Never);
            }
        }

        public static IEnumerable<object[]> WritableCases() {
            // Axis and sign expectations are the pre-refactor game API contract.
            var mappings = new[] {
                new object[] { "gyro", "rotation", "up", 0, -1f }, new object[] { "gyro", "rotation", "down", 0, 1f },
                new object[] { "gyro", "rotation", "left", 1, -1f }, new object[] { "gyro", "rotation", "right", 1, 1f },
                new object[] { "gyro", "rotation", "clockwise", 2, 1f }, new object[] { "gyro", "rotation", "counterclockwise", 2, -1f },
                new object[] { "projector", "offset", "right", 0, 1f }, new object[] { "projector", "offset", "left", 0, -1f },
                new object[] { "projector", "offset", "up", 1, 1f }, new object[] { "projector", "offset", "down", 1, -1f },
                new object[] { "projector", "offset", "forward", 2, 1f }, new object[] { "projector", "offset", "backward", 2, -1f },
                new object[] { "projector", "rotation", "up", 0, 1f }, new object[] { "projector", "rotation", "down", 0, -1f },
                new object[] { "projector", "rotation", "right", 1, 1f }, new object[] { "projector", "rotation", "left", 1, -1f },
                new object[] { "projector", "rotation", "clockwise", 2, 1f }, new object[] { "projector", "rotation", "counterclockwise", 2, -1f }
            };
            foreach (var mapping in mappings)
                foreach (var factor in new[] { -1f, 0f, 1f })
                    yield return new[] { mapping[0], mapping[1], mapping[2], mapping[3], mapping[4], factor };
        }

        [DataTestMethod]
        [DynamicData(nameof(WritableCases), DynamicDataSourceType.Method)]
        public void WritableDirectionsPreserveSignsUnitsOtherAxesAndDefaultSteps(string type, string property, string direction, int axis, float sign, float factor) {
            var target = "\"test " + type + "\" " + direction + " " + property;
            var value = factor.ToString(CultureInfo.InvariantCulture);
            // Read initial state, set an absolute value, increment, decrement, then use the default step.
            var script = "print " + target + "\nset " + target + " to " + value + "\nprint " + target +
                "\nincrease " + target + " by 1\nprint " + target + "\ndecrease " + target + " by 1\nprint " + target +
                "\nincrease " + target + "\nprint " + target;
            using (var test = new ScriptTest(script)) {
                var state = new Vector3(1, -1, 2);
                var initial = state;
                const float radiansPerRpm = (float)(Math.PI / 30);
                if (type == "gyro") {
                    var block = new Mock<IMyGyro>();
                    block.Setup(b => b.Pitch).Returns(() => state.X * radiansPerRpm);
                    block.Setup(b => b.Yaw).Returns(() => state.Y * radiansPerRpm);
                    block.Setup(b => b.Roll).Returns(() => state.Z * radiansPerRpm);
                    block.SetupSet(b => b.Pitch = It.IsAny<float>()).Callback<float>(v => state.X = v / radiansPerRpm);
                    block.SetupSet(b => b.Yaw = It.IsAny<float>()).Callback<float>(v => state.Y = v / radiansPerRpm);
                    block.SetupSet(b => b.Roll = It.IsAny<float>()).Callback<float>(v => state.Z = v / radiansPerRpm);
                    test.MockBlocksOfType("test gyro", block);
                } else {
                    var block = new Mock<IMyProjector>();
                    block.Setup(b => b.ProjectionOffset).Returns(() => new Vector3I(state));
                    block.Setup(b => b.ProjectionRotation).Returns(() => new Vector3I(state));
                    block.SetupSet(b => b.ProjectionOffset = It.IsAny<Vector3I>()).Callback<Vector3I>(v => state = v);
                    block.SetupSet(b => b.ProjectionRotation = It.IsAny<Vector3I>()).Callback<Vector3I>(v => state = v);
                    test.MockBlocksOfType("test projector", block);
                }
                test.RunUntilDone();
                var step = type == "gyro" ? 5 : 0;
                AssertNumbers(test, Component(initial, axis) * sign, factor, factor + 1, factor, factor + step);
                for (var i = 0; i < 3; i++)
                    Assert.AreEqual(i == axis ? (factor + step) * sign : Component(initial, i), Component(state, i), 0.00001f, "Game API axis " + i);
            }
        }

        [DataTestMethod]
        [DataRow("up", 0, 1)] [DataRow("down", 0, -1)]
        [DataRow("right", 1, 1)] [DataRow("left", 1, -1)]
        [DataRow("clockwise", 2, 1)] [DataRow("counterclockwise", 2, -1)]
        public void ProjectorRotationClampsBothSigns(string direction, int axis, int sign) {
            var target = "\"test projector\" " + direction + " rotation";
            using (var test = new ScriptTest("set " + target + " to 9\nprint " + target + "\nset " + target + " to -9\nprint " + target)) {
                var block = new Mock<IMyProjector>();
                var state = new Vector3I(1, -1, 1);
                var initial = state;
                block.Setup(b => b.ProjectionRotation).Returns(() => state);
                block.SetupSet(b => b.ProjectionRotation = It.IsAny<Vector3I>()).Callback<Vector3I>(v => state = v);
                test.MockBlocksOfType("test projector", block);
                test.RunUntilDone();
                AssertNumbers(test, 2, -2);
                for (var i = 0; i < 3; i++) Assert.AreEqual(i == axis ? -2 * sign : initial[i], state[i]);
            }
        }

        static float Component(Vector3 value, int axis) => axis == 0 ? value.X : axis == 1 ? value.Y : value.Z;

        static void AssertNumbers(ScriptTest test, params float[] expected) {
            Assert.AreEqual(expected.Length, test.Logger.Count, string.Join("\n", test.Logger));
            for (var i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], float.Parse(test.Logger[i], CultureInfo.InvariantCulture), 0.00001f, "Output " + i);
        }
    }
}
