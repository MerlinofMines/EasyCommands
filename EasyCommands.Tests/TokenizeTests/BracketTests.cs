using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using static IngameScript.Program;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;

namespace EasyCommands.Tests.TokenizeTests {
    [TestClass]
    public class BracketTests : ForceLocale {
        [TestMethod]
        public void TestBasicBrackets() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test [ string ]");
            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("[", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual("]", tokens[3].original);
        }

        [TestMethod]
        public void TestBracketsMissingSpaces() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test [string] there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("[", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual("]", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestInlineBrackets() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test list[string] there");
            Assert.AreEqual(6, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("list", tokens[1].original);
            Assert.AreEqual("[", tokens[2].original);
            Assert.AreEqual("string", tokens[3].original);
            Assert.AreEqual("]", tokens[4].original);
            Assert.AreEqual("there", tokens[5].original);
        }

        [TestMethod]
        public void TestMultiInlineBrackets() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test list[string1][string2] there");
            Assert.AreEqual(9, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("list", tokens[1].original);
            Assert.AreEqual("[", tokens[2].original);
            Assert.AreEqual("string1", tokens[3].original);
            Assert.AreEqual("]", tokens[4].original);
            Assert.AreEqual("[", tokens[5].original);
            Assert.AreEqual("string2", tokens[6].original);
            Assert.AreEqual("]", tokens[7].original);
            Assert.AreEqual("there", tokens[8].original);
        }

        [TestMethod]
        public void TestCommaSeparatedInlineBrackets() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test list[string1,string2] there");
            Assert.AreEqual(8, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("list", tokens[1].original);
            Assert.AreEqual("[", tokens[2].original);
            Assert.AreEqual("string1", tokens[3].original);
            Assert.AreEqual(",", tokens[4].original);
            Assert.AreEqual("string2", tokens[5].original);
            Assert.AreEqual("]", tokens[6].original);
            Assert.AreEqual("there", tokens[7].original);
        }

        [TestMethod]
        public void TestMissingSpaceEmptyBrackets() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test list[] there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("list", tokens[1].original);
            Assert.AreEqual("[", tokens[2].original);
            Assert.AreEqual("]", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestMissingSpaceBeforeOpeningBracket() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test [string ]there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("[", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual("]", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestMissingSpaceAfterClosingBracket() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test[ string ] there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("[", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual("]", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestEmbeddedBracketsMissingSpaces() {
            var program = CreateProgram();
            var tokens = program.Tokenize("test [[string] there]");
            Assert.AreEqual(7, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("[", tokens[1].original);
            Assert.AreEqual("[", tokens[2].original);
            Assert.AreEqual("string", tokens[3].original);
            Assert.AreEqual("]", tokens[4].original);
            Assert.AreEqual("there", tokens[5].original);
            Assert.AreEqual("]", tokens[6].original);
        }
    }
}
