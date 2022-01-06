using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class MergeBlockTests : ForceLocale {
        [TestMethod]
        public void TurnOnTheMergeBlock() {
            using (ScriptTest test = new ScriptTest(@"turn on the ""test merge block""")) {
                Mock<IMyShipMergeBlock> mockMergeBlock = new Mock<IMyShipMergeBlock>();
                test.MockBlocksOfType("test merge block", mockMergeBlock);

                test.RunUntilDone();

                mockMergeBlock.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheMergeBlock() {
            using (ScriptTest test = new ScriptTest(@"turn off the ""test merge block""")) {
                Mock<IMyShipMergeBlock> mockMergeBlock = new Mock<IMyShipMergeBlock>();
                test.MockBlocksOfType("test merge block", mockMergeBlock);

                test.RunUntilDone();

                mockMergeBlock.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void IsTheMergeBlockConnected() {
            using (ScriptTest test = new ScriptTest(@"Print ""Connected: "" + ""test merge block"" is connected")) {
                Mock<IMyShipMergeBlock> mockMergeBlock = new Mock<IMyShipMergeBlock>();
                test.MockBlocksOfType("test merge block", mockMergeBlock);
                mockMergeBlock.Setup(b => b.IsConnected).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Connected: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsTheMergeBlockLocked() {
            using (ScriptTest test = new ScriptTest(@"Print ""Locked: "" + ""test merge block"" is locked")) {
                Mock<IMyShipMergeBlock> mockMergeBlock = new Mock<IMyShipMergeBlock>();
                test.MockBlocksOfType("test merge block", mockMergeBlock);
                mockMergeBlock.Setup(b => b.IsConnected).Returns(true);

                test.RunUntilDone();

                Assert.AreEqual("Locked: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ConnectTheMergeBlock() {
            using (ScriptTest test = new ScriptTest(@"connect the ""test merge block""")) {
                Mock<IMyShipMergeBlock> mockMergeBlock = new Mock<IMyShipMergeBlock>();
                test.MockBlocksOfType("test merge block", mockMergeBlock);

                test.RunUntilDone();

                mockMergeBlock.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void LockTheMergeBlock() {
            using (ScriptTest test = new ScriptTest(@"lock the ""test merge block""")) {
                Mock<IMyShipMergeBlock> mockMergeBlock = new Mock<IMyShipMergeBlock>();
                test.MockBlocksOfType("test merge block", mockMergeBlock);

                test.RunUntilDone();

                mockMergeBlock.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void DisconnectTheMergeBlock() {
            using (ScriptTest test = new ScriptTest(@"disconnect the ""test merge block""")) {
                Mock<IMyShipMergeBlock> mockMergeBlock = new Mock<IMyShipMergeBlock>();
                test.MockBlocksOfType("test merge block", mockMergeBlock);

                test.RunUntilDone();

                mockMergeBlock.VerifySet(b => b.Enabled = false);
            }
        }

        [TestMethod]
        public void UnlockTheMergeBlock() {
            using (ScriptTest test = new ScriptTest(@"unlock the ""test merge block""")) {
                Mock<IMyShipMergeBlock> mockMergeBlock = new Mock<IMyShipMergeBlock>();
                test.MockBlocksOfType("test merge block", mockMergeBlock);

                test.RunUntilDone();

                mockMergeBlock.VerifySet(b => b.Enabled = false);
            }
        }
    }
}
