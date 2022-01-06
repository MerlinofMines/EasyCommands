using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class AssemblerBlockTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void TellAssemblerToSupply() {
            using(ScriptTest test = new ScriptTest(@"tell the ""test assembler"" to supply")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.VerifySet(b => b.Mode = MyAssemblerMode.Assembly);
            }
        }

        [TestMethod]
        public void IsAssemblerSupplying() {
            using (ScriptTest test = new ScriptTest(@"Print ""Supplying: "" + ""test assembler"" is supplying")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);
                mockAssembler.Setup(b => b.Mode).Returns(MyAssemblerMode.Assembly);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Supplying: True"));
            }
        }

        [TestMethod]
        public void TellAssemblerToConsume() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test assembler"" to consume")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.VerifySet(b => b.Mode = MyAssemblerMode.Disassembly);
            }
        }

        [TestMethod]
        public void IsAssemblerConsuming() {
            using (ScriptTest test = new ScriptTest(@"Print ""Supplying: "" + ""test assembler"" is consuming")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);
                mockAssembler.Setup(b => b.Mode).Returns(MyAssemblerMode.Disassembly);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Supplying: True"));
            }
        }

        [TestMethod]
        public void IsAssemblerComplete() {
            using (ScriptTest test = new ScriptTest(@"Print ""Complete: "" + ""test assembler"" is complete")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);
                mockAssembler.Setup(b => b.IsQueueEmpty).Returns(true);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Complete: True"));
            }
        }

        [TestMethod]
        public void SetAssemblerToAuto() {
            using (ScriptTest test = new ScriptTest(@"set ""test assembler"" to auto")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.VerifySet(b => b.CooperativeMode = true);
            }
        }

        [TestMethod]
        public void TellAssemblerToCooperate() {
            using (ScriptTest test = new ScriptTest(@"tell ""test assembler"" to cooperate")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.VerifySet(b => b.CooperativeMode = true);
            }
        }

        [TestMethod]
        public void GetAssemblerSteelPlateAmountWithEmptyQueue() {
            using (ScriptTest test = new ScriptTest(@"Print ""Steel Plate Remaining: "" + ""test assembler"" ""steel plate"" amount")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);
                MockProductionQueue(mockAssembler);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Steel Plate Remaining: 0"));
            }
        }

        [TestMethod]
        public void GetAssemblerSteelPlateAmount() {
            using (ScriptTest test = new ScriptTest(@"Print ""Steel Plate Remaining: "" + ""test assembler"" ""steel plate"" amount")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);
                MockProductionQueue(mockAssembler, MockProductionItem("SteelPlate", 100));

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Steel Plate Remaining: 100"));
            }
        }

        [TestMethod]
        public void StopTheAssembler() {
            using (ScriptTest test = new ScriptTest(@"stop the ""test assembler""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.Verify(b => b.ClearQueue());
            }
        }

        [TestMethod]
        public void CreateSteelPlate() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test assembler"" to create ""steel plate""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.VerifySet(b => b.Mode = MyAssemblerMode.Assembly);
                mockAssembler.Verify(b => b.AddQueueItem(MockBlueprint("SteelPlate"), (MyFixedPoint)1));
            }
        }

        [TestMethod]
        public void CreateTenSteelPlate() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test assembler"" to create 10 ""steel plate""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.VerifySet(b => b.Mode = MyAssemblerMode.Assembly);
                mockAssembler.Verify(b => b.AddQueueItem(MockBlueprint("SteelPlate"), (MyFixedPoint)10));
            }
        }

        [TestMethod]
        public void IsTheAssemblerCreatingSteelPlate() {
            using (ScriptTest test = new ScriptTest(@"Print ""Creating Steel Plate: "" + ""test assembler"" is creating ""steel plate""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);
                MockProductionQueue(mockAssembler, MockProductionItem("SteelPlate", 100));
                mockAssembler.Setup(b => b.Mode).Returns(MyAssemblerMode.Assembly);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Creating Steel Plate: True"));
            }
        }

        [TestMethod]
        public void DestroyingSteelPlateIsNotCreating() {
            using (ScriptTest test = new ScriptTest(@"Print ""Creating Steel Plate: "" + ""test assembler"" is creating ""steel plate""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);
                MockProductionQueue(mockAssembler, MockProductionItem("SteelPlate", 100));
                mockAssembler.Setup(b => b.Mode).Returns(MyAssemblerMode.Disassembly);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Creating Steel Plate: False"));
            }
        }

        [TestMethod]
        public void DestroySteelPlate() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test assembler"" to destroy ""steel plate""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.VerifySet(b => b.Mode = MyAssemblerMode.Disassembly);
                mockAssembler.Verify(b => b.AddQueueItem(MockBlueprint("SteelPlate"), (MyFixedPoint)1));
            }
        }

        [TestMethod]
        public void DestroyTenSteelPlate() {
            using (ScriptTest test = new ScriptTest(@"tell the ""test assembler"" to destroy 10 ""steel plate""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);

                test.RunUntilDone();

                mockAssembler.VerifySet(b => b.Mode = MyAssemblerMode.Disassembly);
                mockAssembler.Verify(b => b.AddQueueItem(MockBlueprint("SteelPlate"), (MyFixedPoint)10));
            }
        }

        [TestMethod]
        public void IsTheAssemblerDestroyingSteelPlate() {
            using (ScriptTest test = new ScriptTest(@"Print ""Destroying Steel Plate: "" + ""test assembler"" is destroying ""steel plate""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);
                MockProductionQueue(mockAssembler, MockProductionItem("SteelPlate", 100));
                mockAssembler.Setup(b => b.Mode).Returns(MyAssemblerMode.Disassembly);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Destroying Steel Plate: True"));
            }
        }

        [TestMethod]
        public void AssemblingSteelPlateIsNotDestroying() {
            using (ScriptTest test = new ScriptTest(@"Print ""Destroying Steel Plate: "" + ""test assembler"" is creating ""steel plate""")) {
                Mock<IMyAssembler> mockAssembler = new Mock<IMyAssembler>();
                test.MockBlocksOfType("test assembler", mockAssembler);
                MockProductionQueue(mockAssembler, MockProductionItem("SteelPlate", 100));
                mockAssembler.Setup(b => b.Mode).Returns(MyAssemblerMode.Disassembly);

                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Destroying Steel Plate: False"));
            }
        }
    }
}
