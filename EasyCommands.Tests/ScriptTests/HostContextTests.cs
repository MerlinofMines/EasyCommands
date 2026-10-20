using System;
using IngameScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class HostContextTests {
        [TestMethod]
        public void ConstructorRestoresOuterContext() {
            using (var outer = new ScriptTest("")) {
                var previous = Program.PROGRAM;
                try {
                    Program.PROGRAM = outer.program;
                    ProgramFactory.CreateProgram<Program>();
                    Assert.AreSame(outer.program, Program.PROGRAM);
                } finally { Program.PROGRAM = previous; }
            }
        }

        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void MainRestoresOuterContextAfterSuccessOrHostFailure(bool fail) {
            using (var outer = new ScriptTest(""))
            using (var inner = new ScriptTest("")) {
                if (fail) inner.me.Setup(b => b.CustomData).Throws(new InvalidOperationException("host failure"));
                var previous = Program.PROGRAM;
                try {
                    Program.PROGRAM = outer.program;
                    ProgramFactory.Run(inner.program);
                    Assert.AreSame(outer.program, Program.PROGRAM);
                    if (fail) Assert.IsTrue(inner.Logger.Contains("host failure"));
                } finally { Program.PROGRAM = previous; }
            }
        }
    }
}
