using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class VectorVariableTests {
        [TestMethod]
        public void AssignVariableVector() {
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
        public void BindVariableVector() {
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
        public void TernaryAndVariableVector() {
            // worst case formatting
            var script = @"
assign a to 2
Print true?a:0:0:1:0:0
Print false?a:0:0:1:0:0
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("2:0:0", test.Logger[0]);
                Assert.AreEqual("1:0:0", test.Logger[1]);
            }
        }

        [TestMethod]
        public void AssignUniOperandVariableVector() {
            var script = @"
assign vec to sqrt 4:round 0.5:-2
Print vec
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("2:0:-4", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AssignBiOperandVariableVector() {
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
    }
}
