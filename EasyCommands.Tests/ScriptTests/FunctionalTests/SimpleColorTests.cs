using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using IngameScript;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleColorTests {
        [TestInitialize]
        public void InitializeTestClass() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");            
        }

        [TestMethod]
        public void PrintStandardColor() {
            using (var test = new ScriptTest(@"Print ""Color: "" + red")) {
                test.RunOnce();

                Assert.AreEqual("Color: #FF0000", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetColorR() {
            using (var test = new ScriptTest(@"Print ""R: "" + red.r")) {
                test.RunOnce();

                Assert.AreEqual("R: 255", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetColorG() {
            using (var test = new ScriptTest(@"Print ""G: "" + green.g")) {
                test.RunOnce();

                Assert.AreEqual("G: 128", test.Logger[0]);
            }
        }

        [TestMethod]
        public void GetColorB() {
            using (var test = new ScriptTest(@"Print ""B: "" + blue.b")) {
                test.RunOnce();

                Assert.AreEqual("B: 255", test.Logger[0]);
            }
        }

        [TestMethod]
        public void ReverseColor() {
            using (var test = new ScriptTest(@"Print ""Color: "" + reverse #FF00FF")) {

                test.RunOnce();

                Assert.AreEqual("Color: #00FF00", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AddColors() {
            using (var test = new ScriptTest(@"Print ""Color: "" + (red + yellow)")) {
                test.RunOnce();

                Assert.AreEqual("Color: #FFFF00", test.Logger[0]);
            }
        }

        [TestMethod]
        public void SubtractColors() {
            using (var test = new ScriptTest(@"Print ""Color: "" + (#00FFFF - #FFFF00)")) {
                test.RunOnce();

                Assert.AreEqual("Color: #0000FF", test.Logger[0]);
            }
        }

        [TestMethod]
        public void MultiplyColorAndNumber() {
            using (var test = new ScriptTest(@"Print ""Color: "" + (#001111 * 2)")) {
                test.RunOnce();

                Assert.AreEqual("Color: #002222", test.Logger[0]);
            }
        }

        [TestMethod]
        public void MultiplyNumberAndColor() {
            using (var test = new ScriptTest(@"Print ""Color: "" + (2 * #001111)")) {
                test.RunOnce();

                Assert.AreEqual("Color: #002222", test.Logger[0]);
            }
        }

        [TestMethod]
        public void Divide() {
            using (var test = new ScriptTest(@"Print ""Color: "" + (#002222 / 2)")) {
                test.RunOnce();

                Assert.AreEqual("Color: #001111", test.Logger[0]);
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
