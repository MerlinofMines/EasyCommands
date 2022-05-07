using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;
using VRage.Game;
using VRage.Game.ModAPI.Ingame;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class GridBlockTests {
        [TestMethod]
        public void PrintAllGridNames() {
            using (ScriptTest test = new ScriptTest(@"print ""Grid Names: "" + all of the grid names")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                Mock<IMyRadioAntenna> mockAntenna2 = new Mock<IMyRadioAntenna>();
                Mock<IMyCubeGrid> mockGrid = new Mock<IMyCubeGrid>();
                Mock<IMyCubeGrid> mockSubGrid = new Mock<IMyCubeGrid>();
                MockCubeGrid(test.me, mockGrid);
                MockCubeGrid(mockAntenna, mockGrid);
                MockCubeGrid(mockAntenna2, mockSubGrid);

                test.MockBlocksOfType("test antenna", mockAntenna);
                test.MockBlocksOfType("test antenna2", mockAntenna2);

                mockGrid.Setup(b => b.CustomName).Returns("GridName");
                mockSubGrid.Setup(b => b.CustomName).Returns("SubGridName");

                test.RunUntilDone();

                Assert.AreEqual("Grid Names: [GridName,SubGridName]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintCountOfAllGrids() {
            using (ScriptTest test = new ScriptTest(@"print ""Grid Count: "" + the count of all grids")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                Mock<IMyRadioAntenna> mockAntenna2 = new Mock<IMyRadioAntenna>();
                Mock<IMyCubeGrid> mockGrid = new Mock<IMyCubeGrid>();
                Mock<IMyCubeGrid> mockSubGrid = new Mock<IMyCubeGrid>();
                MockCubeGrid(test.me, mockGrid);
                MockCubeGrid(mockAntenna, mockGrid);
                MockCubeGrid(mockAntenna2, mockSubGrid);

                test.MockBlocksOfType("test antenna", mockAntenna);
                test.MockBlocksOfType("test antenna2", mockAntenna2);

                test.RunUntilDone();

                Assert.AreEqual("Grid Count: 2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SetBlockGridName() {
            using (ScriptTest test = new ScriptTest(@"set the ""test antenna"" grid name to ""MyGridName""")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                Mock<IMyCubeGrid> mockGrid = new Mock<IMyCubeGrid>();
                MockCubeGrid(mockAntenna, mockGrid);

                test.MockBlocksOfType("test antenna", mockAntenna);

                test.RunUntilDone();

                mockGrid.VerifySet(b => b.CustomName = "MyGridName");
            }
        }

        [TestMethod]
        public void SetTheGridName() {
            using (ScriptTest test = new ScriptTest(@"set the grid name to ""MyGridName""")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                Mock<IMyCubeGrid> mockGrid = new Mock<IMyCubeGrid>();
                MockCubeGrid(mockAntenna, mockGrid);
                MockCubeGrid(test.me, mockGrid);

                test.MockBlocksOfType("test antenna", mockAntenna);

                test.RunUntilDone();

                mockGrid.VerifySet(b => b.CustomName = "MyGridName", Times.Once);
            }
        }

        [TestMethod]
        public void SetMyGridName() {
            using (ScriptTest test = new ScriptTest(@"set my grid name to ""MyGridName""")) {
                Mock<IMyRadioAntenna> mockAntenna = new Mock<IMyRadioAntenna>();
                Mock<IMyCubeGrid> mockGrid = new Mock<IMyCubeGrid>();
                Mock<IMyCubeGrid> mockSubGrid = new Mock<IMyCubeGrid>();
                MockCubeGrid(mockAntenna, mockSubGrid);
                MockCubeGrid(test.me, mockGrid);

                test.MockBlocksOfType("test antenna", mockAntenna);

                test.RunUntilDone();

                mockGrid.VerifySet(b => b.CustomName = "MyGridName", Times.Once);
                mockSubGrid.VerifyNoOtherCalls();
            }
        }

        [TestMethod]
        public void IsMyGridStatic() {
            using (ScriptTest test = new ScriptTest(@"Print ""Static: "" + my grid is static")) {
                Mock<IMyCubeGrid> mockGrid = new Mock<IMyCubeGrid>();
                MockCubeGrid(test.me, mockGrid);

                mockGrid.Setup(b => b.IsStatic).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Static: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintSmallGridSize() {
            using (ScriptTest test = new ScriptTest(@"Print ""Grid Size: "" + my grid size")) {
                Mock<IMyCubeGrid> mockGrid = new Mock<IMyCubeGrid>();
                MockCubeGrid(test.me, mockGrid);

                mockGrid.Setup(b => b.GridSizeEnum).Returns(MyCubeSize.Small);

                test.RunUntilDone();

                Assert.AreEqual("Grid Size: small", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintLargeGridSize() {
            using (ScriptTest test = new ScriptTest(@"Print ""Grid Size: "" + my grid size")) {
                Mock<IMyCubeGrid> mockGrid = new Mock<IMyCubeGrid>();
                MockCubeGrid(test.me, mockGrid);

                mockGrid.Setup(b => b.GridSizeEnum).Returns(MyCubeSize.Large);

                test.RunUntilDone();

                Assert.AreEqual("Grid Size: large", test.Logger[0]);
            }
        }
    }
}
