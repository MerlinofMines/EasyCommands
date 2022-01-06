using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class IteratorParameterProcessorTests : ForceLocale {
        [TestMethod]
        public void SimpleIteration() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("for each item in myItems print \"Item: \" + item");
            Assert.IsTrue(command is ForEachCommand);
            ForEachCommand forEachCommand = (ForEachCommand)command;
            Assert.AreEqual("item", forEachCommand.iterator);
            Assert.IsTrue(forEachCommand.list is AmbiguousStringVariable);
            Assert.AreEqual("myItems", ((AmbiguousStringVariable)forEachCommand.list).value);
            Assert.IsTrue(forEachCommand.command is PrintCommand);
        }

        [TestMethod]
        public void SimpleIterationWithPrecedingCommand() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("print \"Item: \" + item for each item in myItems");
            Assert.IsTrue(command is ForEachCommand);
            ForEachCommand forEachCommand = (ForEachCommand)command;
            Assert.AreEqual("item", forEachCommand.iterator);
            Assert.IsTrue(forEachCommand.list is AmbiguousStringVariable);
            Assert.AreEqual("myItems", ((AmbiguousStringVariable)forEachCommand.list).value);
            Assert.IsTrue(forEachCommand.command is PrintCommand);
        }
    }
}
