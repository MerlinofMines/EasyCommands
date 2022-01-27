using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class VectorVariableTests {
        [TestMethod]
        public void Assign() {
            var script = @"
assign a to 1
bind b to a + 1
assign vec to a:0:b
Print vec
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("1:0:2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void Bind() {
            var script = @"
assign a to 1
bind b to a + 1
bind vec to a:0:b
Print vec
a+=1
Print vec
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("1:0:2", test.Logger[0]);
                Assert.AreEqual("2:0:3", test.Logger[1]);
            }
        }

        [TestMethod]
        public void AssignUniOperands() {
            var script = @"
assign vec to sqrt 4:round 0.5:-2
Print vec
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("2:0:-2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignBiOperands() {
            var script = @"
assign a to 1
assign vec to a:0:0 + a:0:0 + a + 1
Print vec
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("4:0:0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void TernaryLeft() {
            // worst case formatting
            var script = @"
assign a to 2
Print true ? a:0:0 : a
Print false ? a:0:0 : a
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("2:0:0", test.Logger[0]);
                Assert.AreEqual("2", test.Logger[1]);
            }
        }

        [TestMethod]
        public void TernaryRight() {
            // worst case formatting
            var script = @"
assign a to 2
Print true ? a : 0:0:a
Print false ? a : 0:0:a
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("2", test.Logger[0]);
                Assert.AreEqual("0:0:2", test.Logger[1]);
            }
        }

        [TestMethod]
        public void TernaryBoth() {
            // worst case formatting
            var script = @"
assign a to 2
Print true ? a:0:0 : 0:0:a
Print false ? a:0:0 : 0:0:a
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("2:0:0", test.Logger[0]);
                Assert.AreEqual("0:0:2", test.Logger[1]);
            }
        }

        [TestMethod]
        public void TernaryBothPrimitiveLeft() {
            // worst case formatting
            var script = @"
assign a to 2
Print true ? 1:2:3 : 0:0:a
Print false ? 1:2:3 : 0:0:a
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("1:2:3", test.Logger[0]);
                Assert.AreEqual("0:0:2", test.Logger[1]);
            }
        }

        [TestMethod]
        public void TernaryBothPrimitiveRight() {
            // worst case formatting
            var script = @"
assign a to 2
Print true ? a:0:0 : 1:2:3
Print false ? a:0:0 : 1:2:3
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("2:0:0", test.Logger[0]);
                Assert.AreEqual("1:2:3", test.Logger[1]);
            }
        }

        [TestMethod]
        public void TernaryBothPrimitiveBoth() {
            // worst case formatting
            var script = @"
Print true ? 3:2:1 : 1:2:3
Print false ? 3:2:1 : 1:2:3
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("3:2:1", test.Logger[0]);
                Assert.AreEqual("1:2:3", test.Logger[1]);
            }
        }

        [TestMethod]
        public void TernaryBothNoSpace() {
            var script = @"
assign a to 2
Print true?a:0:0:0:a:0
Print false?a:0:0:0:a:0
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("2:0:0", test.Logger[0]);
                Assert.AreEqual("0:2:0", test.Logger[1]);
            }
        }

        [TestMethod]
        public void NestedExpressions() {
            var script = @"
assign a to 2
assign vec to ((a ^ a) % a):(0:a:0 as number):(red . 1:0:0)
Print vec
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("0:2:255", test.Logger[0]);
            }
        }
    }
}
