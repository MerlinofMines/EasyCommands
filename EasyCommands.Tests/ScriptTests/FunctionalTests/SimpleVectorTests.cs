using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using IngameScript;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleVectorTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void PrintVector() {
            using (var test = new ScriptTest(@"Print ""Vector: "" + 0:1:2")) {
                test.RunOnce();

                Assert.AreEqual("Vector: 0:1:2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetVectorX() {
            using (var test = new ScriptTest(@"Print ""X: "" + (0:1:2).x")) {
                test.RunOnce();

                Assert.AreEqual("X: 0", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetVectorY() {
            using (var test = new ScriptTest(@"Print ""Y: "" + (0:1:2).y")) {
                test.RunOnce();

                Assert.AreEqual("Y: 1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetVectorZ() {
            using (var test = new ScriptTest(@"Print ""Z: "" + (0:1:2).z")) {
                test.RunOnce();

                Assert.AreEqual("Z: 2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AddVectors() {
            using (var test = new ScriptTest(@"Print ""Vector: "" + (0:1:2 + 4:5:6)")) {
                test.RunOnce();

                Assert.AreEqual("Vector: 4:6:8", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SubtractVectors() {
            using (var test = new ScriptTest(@"Print ""Vector: "" + (3:2:1 - 1:2:3)")) {
                test.RunOnce();

                Assert.AreEqual("Vector: 2:0:-2", test.Logger[0]);
            }
        }

        [TestMethod]
        public void VectorLength() {
            using (var test = new ScriptTest(@"Print ""Length: "" + abs 2:3:6")) {
                test.RunOnce();

                Assert.AreEqual("Length: 7", test.Logger[0]);
            }
        }
    }
}
