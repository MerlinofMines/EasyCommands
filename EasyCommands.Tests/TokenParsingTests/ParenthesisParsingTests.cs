using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.TokenParsingTests {
    [TestClass]
    public class ParenthesisParsingTests {
        [TestInitialize]
        public void InitializeTestClass()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void TestBasicParenthesis() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("test ( string )");
            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("(", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual(")", tokens[3].original);
        }

        [TestMethod]
        public void TestParenthesesMissingSpaces() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("test (string) there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("(", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual(")", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestMissingSpaceBeforeOpeningParenthesis() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("test (string )there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("(", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual(")", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestMissingSpaceAfterClosingParenthesis() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("test( string ) there");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("test", tokens[0].original);
            Assert.AreEqual("(", tokens[1].original);
            Assert.AreEqual("string", tokens[2].original);
            Assert.AreEqual(")", tokens[3].original);
            Assert.AreEqual("there", tokens[4].original);
        }

        [TestMethod]
        public void TestEmbeddedParenthesesMissingSpaces() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("test ((string) there)");
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
