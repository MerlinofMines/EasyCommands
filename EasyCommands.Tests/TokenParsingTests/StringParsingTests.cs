using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.TokenParsingTests {
    [TestClass]
    public class StringParsingTests {
        [TestMethod]
        public void BasicStrings() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("turn on the rotors");
            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("turn", tokens[0].original);
            Assert.AreEqual("on", tokens[1].original);
            Assert.AreEqual("the", tokens[2].original);
            Assert.AreEqual("rotors", tokens[3].original);
        }

        [TestMethod]
        public void StringWithDoubleQuotes() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("turn on the \"test rotors\"");
            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("turn", tokens[0].original);
            Assert.AreEqual("on", tokens[1].original);
            Assert.AreEqual("the", tokens[2].original);
            Assert.AreEqual("test rotors", tokens[3].original);
        }

        [TestMethod]
        public void MultipleDoubleQuotes() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("tell the \"test program\" to \"run gotoTest\"");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("test program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run gotoTest", tokens[4].original);
        }

        [TestMethod]
        public void SingleQuotes() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("tell the program to 'run gotoTest'");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run gotoTest", tokens[4].original);
        }

        [TestMethod]
        public void MultipleSingleQuotes() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("tell the 'test program' to 'run gotoTest'");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("test program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run gotoTest", tokens[4].original);
        }

        [TestMethod]
        public void SingleQuotesAndDoubleQuotes() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("tell the \"test program\" to 'run gotoTest'");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("test program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run gotoTest", tokens[4].original);
        }

        [TestMethod]
        public void DoubleQuotesInsideSingleQuotes() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("tell the \"test program\" to 'run \"goto testFunction\"'");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual("tell", tokens[0].original);
            Assert.AreEqual("the", tokens[1].original);
            Assert.AreEqual("test program", tokens[2].original);
            Assert.AreEqual("to", tokens[3].original);
            Assert.AreEqual("run \"goto testFunction\"", tokens[4].original);
        }

        [TestMethod]
        public void SeparateTokensMissingSpaces() {
            VerifyTokensSplit(",");
            VerifyTokensSplit("+");
            VerifyTokensSplit("*");
            VerifyTokensSplit("/");
            VerifyTokensSplit("!");
            VerifyTokensSplit("^");
            VerifyTokensSplit("..");
            VerifyTokensSplit(".");
            VerifyTokensSplit("%");
            VerifyTokensSplit(">");
            VerifyTokensSplit(">=");
            VerifyTokensSplit("<");
            VerifyTokensSplit("<=");
            VerifyTokensSplit("==");
            VerifyTokensSplit("=");
            VerifyTokensSplit("&&");
            VerifyTokensSplit("&");
            VerifyTokensSplit("||");
            VerifyTokensSplit("|");
            VerifyTokensSplit("@");
        }

        [TestMethod]
        public void SubtractionMissingSpaces() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("assign a to b-c");
            Assert.AreEqual(6, tokens.Count);
            Assert.AreEqual("assign", tokens[0].original);
            Assert.AreEqual("a", tokens[1].original);
            Assert.AreEqual("to", tokens[2].original);
            Assert.AreEqual("b", tokens[3].original);
            Assert.AreEqual("-", tokens[4].original);
            Assert.AreEqual("c", tokens[5].original);
        }

        [TestMethod]
        public void NegativeNumbersAreLeftAlone() {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("assign a to -3");
            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("assign", tokens[0].original);
            Assert.AreEqual("a", tokens[1].original);
            Assert.AreEqual("to", tokens[2].original);
            Assert.AreEqual("-3", tokens[3].original);
        }

        void VerifyTokensSplit(string token) {
            var program = MDKFactory.CreateProgram<Program>();
            var tokens = program.ParseTokens("assign a to b" + token + "c");
            Assert.AreEqual(6, tokens.Count);
            Assert.AreEqual("assign", tokens[0].original);
            Assert.AreEqual("a", tokens[1].original);
            Assert.AreEqual("to", tokens[2].original);
            Assert.AreEqual("b", tokens[3].original);
            Assert.AreEqual(token, tokens[4].original);
            Assert.AreEqual("c", tokens[5].original);
        }
    }
}
