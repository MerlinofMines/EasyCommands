using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleComparisonTests {

        [TestMethod]
        public void LessThan() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 < 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void LessThanIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (3 < 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void EqualIsNotLessThan() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 < 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotLessThan() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (3 not less than 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotLessThanIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 not less than 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void LessThanOrEqual() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 <= 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void LessThanOrEqualIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (3 <= 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void EqualIsLessThanEqual() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 <= 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotLessThanOrEqual() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (3 !<= 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotLessThanOrEqualIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 !<= 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void EqualTo() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 == 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GreaterIsNotEqualTo() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (3 = 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void LessIsNotEqualTo() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 = 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotEqualTo() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 != 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void IsNotEqualTo() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 is not equal to 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotEqualToIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 != 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GreaterThan() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (3 > 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GreaterThanIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 > 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void EqualIsNotGreaterThan() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 > 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotGreaterThan() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 not greater than 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotGreaterThanIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (3 not greater than 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GreaterThanOrEqual() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (3 >= 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GreaterThanOrEqualIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 >= 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void EqualIsGreaterThanOrEqual() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 >= 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotGreaterThanOrEqual() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (2 !>= 3)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void NotGreaterThanOrEqualIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (3 !>= 2)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void StringContains() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (""cat in the hat"" contains ""cat"")")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void StringContainsIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (""cat in the hat"" contains ""dog"")")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void StringDoesNotContain() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (""cat in the hat"" does not contain ""dog"")")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void StringDoesNotContainIsFalse() {
            using (var test = new ScriptTest(@"Print ""My Value: "" + (""cat in the hat"" does not contain ""cat"")")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListContainsOneElement() {
            using (var test = new ScriptTest(@"
set myList to [""one"",""two"",""three""]
Print ""My Value: "" + (myList contains ""one"")")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListContainsOneElementIsFalse() {
            using (var test = new ScriptTest(@"
set myList to [""one"",""two"",""three""]
Print ""My Value: "" + (myList contains ""four"")")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListContainsListOfElements() {
            using (var test = new ScriptTest(@"
set myList to [""one"",""two"",""three""]
Print ""My Value: "" + (myList contains [""one"",""two""])")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListContainsListOfElementsIsFalse() {
            using (var test = new ScriptTest(@"
set myList to [""one"",""two"",""three""]
Print ""My Value: "" + (myList contains [""one"", ""four""])")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListContainsNumeric() {
            using (var test = new ScriptTest(@"
set myList to [7, 8, 9]
Print ""My Value: "" + (myList contains 7)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListContainsNumericIsFalse() {
            using (var test = new ScriptTest(@"
set myList to [7, 8, 9]
Print ""My Value: "" + (myList contains 4)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListDoesNotContainOneElement() {
            using (var test = new ScriptTest(@"
set myList to [""one"",""two"",""three""]
Print ""My Value: "" + (myList does not contain ""four"")")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListDoesNotContainOneElementIsFalse() {
            using (var test = new ScriptTest(@"
set myList to [""one"",""two"",""three""]
Print ""My Value: "" + (myList does not contain ""one"")")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }


        [TestMethod]
        public void ListDoesNotContainListOfElements() {
            using (var test = new ScriptTest(@"
set myList to [""one"",""two"",""three""]
Print ""My Value: "" + (myList does not contain [""one"",""four""])")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListDoesNotContainListOfElementsIsFalse() {
            using (var test = new ScriptTest(@"
set myList to [""one"",""two"",""three""]
Print ""My Value: "" + (myList does not contain [""one"", ""two""])")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListDoesNotContainNumeric() {
            using (var test = new ScriptTest(@"
set myList to [7, 8, 9]
Print ""My Value: "" + (myList does not contain 4)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ListDoesNotContainNumericIsFalse() {
            using (var test = new ScriptTest(@"
set myList to [7, 8, 9]
Print ""My Value: "" + (myList does not contain 7)")) {

                test.RunOnce();

                Assert.AreEqual("My Value: False", test.Logger[0]);
            }
        }
    }
}
