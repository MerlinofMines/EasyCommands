using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class TokenParsingTests {
        [TestMethod]
        public void BasicStrings() {
            var tokens = ParseTokens("turn on the rotors");
            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("turn", tokens[0].original);
            Assert.AreEqual("on", tokens[1].original);
            Assert.AreEqual("the", tokens[2].original);
            Assert.AreEqual("rotors", tokens[3].original);
        }

        [TestMethod]
        public void StringWithDoubleQuotes() {
            var tokens = ParseTokens("turn on the \"test rotors\"");
            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("turn", tokens[0].original);
            Assert.AreEqual("on", tokens[1].original);
            Assert.AreEqual("the", tokens[2].original);
            Assert.AreEqual("test rotors", tokens[3].original);
        }

        [TestMethod]
        public void MultipleDoubleQuotes() {
            var tokens = ParseTokens("tell the \"test program\" to \"run gotoTest\"");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("test program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run gotoTest", tokens[4].original);
        }

        [TestMethod]
        public void SingleQuotes() {
            var tokens = ParseTokens("tell the program to 'run gotoTest'");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run gotoTest", tokens[4].original);
        }

        [TestMethod]
        public void MultipleSingleQuotes() {
            var tokens = ParseTokens("tell the 'test program' to 'run gotoTest'");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("test program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run gotoTest", tokens[4].original);
        }

        [TestMethod]
        public void SingleQuotesAndDoubleQuotes() {
            var tokens = ParseTokens("tell the \"test program\" to 'run gotoTest'");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("test program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run gotoTest", tokens[4].original);
        }

        [TestMethod]
        public void DoubleQuotesInsideSingleQuotes() {
            var tokens = ParseTokens("tell the \"test program\" to 'run \"goto testFunction\"'");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("test program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run \"goto testFunction\"", tokens[4].original);
        }
    }
}
