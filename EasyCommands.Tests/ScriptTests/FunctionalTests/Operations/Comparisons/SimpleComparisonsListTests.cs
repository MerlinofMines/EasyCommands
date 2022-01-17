using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleComparisonsListTests {
        [TestMethod]
        public void IsEqual() {
            var script = @"
set myList to [1,""two"",""three"" -> 3]
Print myList is empty
Print myList is equal to [1,""two"",3]
Print myList is equal to [1,""two"",""three"" -> 3]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0], "[1,two,three->3] is empty");
                Assert.AreEqual("False", test.Logger[1], "[1,two,three->3] is equal to [1,two,3]");
                Assert.AreEqual("True", test.Logger[2], "[1,two,three->3] is equal to [1,two,three->3]");
            }
        }

        [TestMethod]
        public void IsNotEqual() {
            var script = @"
set myList to [1,""two"",""three"" -> 3]
Print myList is not empty
Print myList is not equal to [1,""two"",3]
Print myList is not equal to [1,""two"",""three"" -> 3]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], "[1,two,three->3] is not empty");
                Assert.AreEqual("True", test.Logger[1], "[1,two,three->3] is not equal to [1,two,3]");
                Assert.AreEqual("False", test.Logger[2], "[1,two,three->3] is not equal to [1,two,three->3]");
            }
        }

        [TestMethod]
        public void ContainsOne() {
            var script = @"
set myList to [1,""two"",""three"" -> 3]
Print myList contains 1
Print myList contains ""two""
Print myList contains 3
Print myList contains 4
Print myList contains ""three""
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], "[1,two,three->3] contains 1");
                Assert.AreEqual("True", test.Logger[1], "[1,two,three->3] contains two");
                Assert.AreEqual("True", test.Logger[2], "[1,two,three->3] contains 3");
                Assert.AreEqual("False", test.Logger[3], "[1,two,three->3] contains 4");
                Assert.AreEqual("False", test.Logger[4], "[1,two,three->3] contains three");
            }
        }

        [TestMethod]
        public void ContainsMultiple() {
            var script = @"
set myList to [1,""two"",""three"" -> 3]
Print myList contains [1, ""two""]
Print myList contains [""two"", ""three""]
Print myList contains [1, 3]
Print myList contains [""three"" -> 3]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], "[1,two,three->3] contains [1, two]");
                Assert.AreEqual("False", test.Logger[1], "[1,two,three->3] contains [two, three]");
                Assert.AreEqual("True", test.Logger[2], "[1,two,three->3] contains [1, 3]");
                Assert.AreEqual("True", test.Logger[3], "[1,two,three->3] contains [three->3]");
            }
        }

        [TestMethod]
        public void DoesNotContainOne() {
            var script = @"
set myList to [1,""two"",""three"" -> 3]
Print myList does not contain 4
Print myList does not contain ""four""
Print myList does not contain 1
Print myList does not contain ""three""
Print myList does not contain 3
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], "[1,two,three->3] does not contain 4");
                Assert.AreEqual("True", test.Logger[1], "[1,two,three->3] does not contain four");
                Assert.AreEqual("False", test.Logger[2], "[1,two,three->3] does not contain 1");
                Assert.AreEqual("True", test.Logger[3], "[1,two,three->3] does not contain three");
                Assert.AreEqual("False", test.Logger[4], "[1,two,three->3] does not contain 3");
            }
        }

        [TestMethod]
        public void DoesNotContainMultiple() {
            var script = @"
set myList to [1,""two"",""three"" -> 3]
Print myList does not contain [1, ""four""]
Print myList does not contain [""two"", ""three""]
Print myList does not contain [3, 1]
Print myList does not contain [""three"" -> 3]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0], "[1,two,three->3] does not contain [1, four]");
                Assert.AreEqual("True", test.Logger[1], "[1,two,three->3] does not contain [two, three]");
                Assert.AreEqual("False", test.Logger[2], "[1,two,three->3] does not contain [3, 1]");
                Assert.AreEqual("False", test.Logger[3], "[1,two,three->3] does not contain [three->3]");
            }
        }
    }
}
