using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;

namespace EasyCommands.Tests {
    [TestClass]
    public class ProgramFactoryTests {
        [TestMethod]
        public void HostContextIsAvailableDuringScriptConstruction() {
            var runtime = new Mock<IMyGridProgramRuntimeInfo>();
            runtime.SetupAllProperties();
            string echo = null;
            var program = ProgramFactory.CreateProgram<ProbeProgram>(new ProgramFactory.ProgramConfig {
                Runtime = runtime.Object,
                Echo = text => echo = text
            });
            Assert.AreSame(runtime.Object, program.RuntimeAtConstruction);
            Assert.AreEqual(UpdateFrequency.Update10, runtime.Object.UpdateFrequency);
            Assert.AreEqual("constructed", echo);
        }

        [TestMethod]
        public void RunsMainThroughTheGameInterface() {
            var program = ProgramFactory.CreateProgram<ProbeProgram>();
            ProgramFactory.Run(program, "start");
            Assert.AreEqual("start", program.Argument);
            Assert.AreEqual(UpdateType.Trigger, program.Source);
        }

        public class ProbeProgram : MyGridProgram {
            public IMyGridProgramRuntimeInfo RuntimeAtConstruction;
            public string Argument;
            public UpdateType Source;

            public ProbeProgram() {
                RuntimeAtConstruction = Runtime;
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
                Echo("constructed");
            }

            public void Main(string argument, UpdateType source) {
                Argument = argument;
                Source = source;
            }
        }
    }
}
