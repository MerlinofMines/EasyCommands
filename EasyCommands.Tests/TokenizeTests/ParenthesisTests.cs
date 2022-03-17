using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using static IngameScript.Program;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;

namespace EasyCommands.Tests.TokenizeTests {
    [TestClass]
    public class ParenthesisTests : ForceLocale {
        [TestMethod]
        public void TestBasicParenthesis() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test ( string )");
            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("(", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual(")", tokens[3].original);
        }

        [TestMethod]
        public void TestParenthesesMissingSpaces() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test (string) there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("(", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual(")", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestMissingSpaceBeforeOpeningParenthesis() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test (string )there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("(", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual(")", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestMissingSpaceAfterClosingParenthesis() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test( string ) there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("(", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual(")", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestEmbeddedParenthesesMissingSpaces() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test ((string) there)");
            Assert.AreEqual(7, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("(", tokens[1].original);
            Assert.AreEqual("(", tokens[2].original);
            Assert.AreEqual("string", tokens[3].original);
            Assert.AreEqual(")", tokens[4].original);
            Assert.AreEqual("there", tokens[5].original);
            Assert.AreEqual(")", tokens[6].original);
        }
    }
}
