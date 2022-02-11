using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleTypeOperatorTests {
        [TestMethod]
        public void PrintBooleanType() {
            using (var test = new ScriptTest(@"print True type")) {

                test.RunOnce();

                Assert.AreEqual("boolean", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintTrueVariableType() {
            using (var test = new ScriptTest(@"
set myVariable to False
print myVariable type")) {

                test.RunOnce();

                Assert.AreEqual("boolean", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintStringType() {
            using (var test = new ScriptTest(@"print ""myString"" type")) {

                test.RunOnce();

                Assert.AreEqual("string", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintStringVariableType() {
            using (var test = new ScriptTest(@"
set myVariable to ""myString""
print myVariable type")) {

                test.RunOnce();

                Assert.AreEqual("string", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintNumberType() {
            using (var test = new ScriptTest(@"print 1.23 type")) {

                test.RunOnce();

                Assert.AreEqual("number", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintNumberVariableType() {
            using (var test = new ScriptTest(@"
set myVariable to 1.23
print myVariable type")) {

                test.RunOnce();

                Assert.AreEqual("number", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintVectorType() {
            using (var test = new ScriptTest(@"print 1:2:3 type")) {

                test.RunOnce();

                Assert.AreEqual("vector", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintVectorVariableType() {
            using (var test = new ScriptTest(@"
set myVariable to 1:2:3
print myVariable type")) {

                test.RunOnce();

                Assert.AreEqual("vector", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintColorType() {
            using (var test = new ScriptTest(@"print red type")) {

                test.RunOnce();

                Assert.AreEqual("color", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintHexColorType() {
            using (var test = new ScriptTest(@"print #FF0000 type")) {

                test.RunOnce();

                Assert.AreEqual("color", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintColorVariableType() {
            using (var test = new ScriptTest(@"
set myVariable to red
print myVariable type")) {

                test.RunOnce();

                Assert.AreEqual("color", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintListType() {
            using (var test = new ScriptTest(@"print [1,2,3] type")) {

                test.RunOnce();

                Assert.AreEqual("list", test.Logger[0]);
            }
        }

        [TestMethod]
        public void PrintListVariableType() {
            using (var test = new ScriptTest(@"
set myVariable to [1,2,3]
print myVariable type")) {

                test.RunOnce();

                Assert.AreEqual("list", test.Logger[0]);
            }
        }
    }
}
