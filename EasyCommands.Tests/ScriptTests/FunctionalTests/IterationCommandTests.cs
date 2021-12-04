using System;
using System.Collections.Generic;
using IngameScript;
using Malware.MDKUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI;
using VRageMath;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class IterationCommandTests {
        [TestMethod]
        public void iterateOverList() {
            String script = @"
set myList to [""one"",""two"",""three""]
for each item in myList
  print ""My Item: "" + item
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Item: one", test.Logger[0]);
                Assert.AreEqual("My Item: two", test.Logger[1]);
                Assert.AreEqual("My Item: three", test.Logger[2]);
            }
        }

        [TestMethod]
        public void iterateOverListPreCommand() {
            String script = @"
set myList to [""one"",""two"",""three""]
print ""My Item: "" + item for each item in myList
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Item: one", test.Logger[0]);
                Assert.AreEqual("My Item: two", test.Logger[1]);
                Assert.AreEqual("My Item: three", test.Logger[2]);
            }
        }

        [TestMethod]
        public void iterateOverListUsingListIndexes() {
            String script = @"
set myList to [""one"",""two"",""three""]
for each i in 0..count of myList[] - 1
  print ""My Item: "" + myList[i]
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Item: one", test.Logger[0]);
                Assert.AreEqual("My Item: two", test.Logger[1]);
                Assert.AreEqual("My Item: three", test.Logger[2]);
            }
        }

        [TestMethod]
        public void iterateOverEmptyListDoesNothing() {
            String script = @"
for each i in []
  print ""My Item: "" + myList[i]
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(0, test.Logger.Count);
            }
        }

        [TestMethod]
        public void iterateMultipleTimesResetsAndWorksProperly() {
            String script = @"
:main
assign myList to [1,2,3]

iterate myList 3 times

:iterate list
for each item in list
  print ""My Item: "" + item
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("My Item: 1", test.Logger[0]);
                Assert.AreEqual("My Item: 2", test.Logger[1]);
                Assert.AreEqual("My Item: 3", test.Logger[2]);
                Assert.AreEqual("My Item: 1", test.Logger[3]);
                Assert.AreEqual("My Item: 2", test.Logger[4]);
                Assert.AreEqual("My Item: 3", test.Logger[5]);
                Assert.AreEqual("My Item: 1", test.Logger[6]);
                Assert.AreEqual("My Item: 2", test.Logger[7]);
                Assert.AreEqual("My Item: 3", test.Logger[8]);
            }
        }
    }
}
