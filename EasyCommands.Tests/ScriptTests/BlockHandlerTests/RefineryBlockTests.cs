﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class RefineryBlockTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void TurnOnTheRefineries() {
            String script = @"turn on the refineries";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRefinery = new Mock<IMyRefinery>();
                test.MockBlocksOfType("test refinery", mockRefinery);

                test.RunUntilDone();

                mockRefinery.VerifySet(b => b.Enabled = true);
            }
        }

        [TestMethod]
        public void TurnOffTheRefineries() {
            String script = @"turn off the refineries";

            using (ScriptTest test = new ScriptTest(script)) {
                var mockRefinery = new Mock<IMyRefinery>();
                test.MockBlocksOfType("test refinery", mockRefinery);

                test.RunUntilDone();

                mockRefinery.VerifySet(b => b.Enabled = false);
            }
        }
    }
}
