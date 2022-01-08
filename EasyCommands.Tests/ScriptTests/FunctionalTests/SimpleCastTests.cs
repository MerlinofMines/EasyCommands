using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyCommands.Tests.ScriptTests
{
    [TestClass]
    public class SimpleCastTests
    {
        #region Boolean
        [TestMethod]
        public void CastBoolAsBool() {
            var script = @"
Print true as bool
Print false as bool
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
                Assert.AreEqual("False", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastNumberAsBool() {
            var script = @"
Print 1 as bool
Print 0 as bool
Print -1 as bool
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
                Assert.AreEqual("False", test.Logger[1]);
                Assert.AreEqual("False", test.Logger[2]);
            }
        }

        [TestMethod]
        public void CastStringAsBool() {
            var script = @"
Print ""True"" as bool
Print ""1"" as bool
Print ""False"" as bool
Print ""0"" as bool
Print ""abc"" as bool
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
                Assert.AreEqual("True", test.Logger[1]);
                Assert.AreEqual("False", test.Logger[2]);
                Assert.AreEqual("False", test.Logger[3]);
                Assert.AreEqual("Exception Occurred:", test.Logger[4]);
            }
        }

        [TestMethod]
        public void CastVectorAsBool() {
            using (var test = new ScriptTest(@"Print 0:0:1 as bool")) {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert vector to boolean", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastColorAsBool() {
            using (var test = new ScriptTest(@"Print #ffffff as bool")) {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert color to boolean", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastListAsBool() {
            using (var test = new ScriptTest(@"Print [0, 1, 2] as bool")) {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert list to boolean", test.Logger[1]);
            }
        }
        #endregion Boolean

        #region Number
        [TestMethod]
        public void CastBoolAsNumber() {
            var script = @"
Print true as number
Print false as number
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("0", test.Logger[0]);
                Assert.AreEqual("-1", test.Logger[1]);
            }
        }               

        [TestMethod]
        public void CastNumberAsNumber() {
            var script = @"
Print 1 as number
Print 0.0 as number
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("1", test.Logger[0]);
                Assert.AreEqual("0.0", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastStringAsNumber() {
            var script = @"
Print ""1"" as number
Print ""0.0"" as number
Print ""1.5"" as number
Print ""abc"" as number
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("1", test.Logger[0]);
                Assert.AreEqual("0.0", test.Logger[1]);
                Assert.AreEqual("1.5", test.Logger[2]);
                Assert.AreEqual("Exception Occurred:", test.Logger[3]);
            }
        }

        [TestMethod]
        public void CastVectorAsNumber() {
            using (var test = new ScriptTest(@"Print 0:0:7 as number")) {
                test.RunOnce();

                Assert.AreEqual("7", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastColorAsNumber() {
            using (var test = new ScriptTest(@"Print #ffffff as number")) {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert color to number", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastListAsNumber()
        {
            using (var test = new ScriptTest(@"Print [0, 1, 2] as number"))
            {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert list to number", test.Logger[1]);
            }
        }
        #endregion Number

        #region String
        [TestMethod]
        public void CastBoolAsString()
        {
            using (var test = new ScriptTest(@"Print true as string"))
            {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastNumberAsString()
        {
            using (var test = new ScriptTest(@"Print 1.0 as string"))
            {
                test.RunOnce();

                Assert.AreEqual("1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastStringAsString()
        {
            using (var test = new ScriptTest(@"Print ""abc"" as string"))
            {
                test.RunOnce();

                Assert.AreEqual("abc", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastVectorAsString()
        {
            using (var test = new ScriptTest(@"Print 0:0:7 as string"))
            {
                test.RunOnce();

                Assert.AreEqual("0:0:7", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastColorAsString()
        {
            using (var test = new ScriptTest(@"Print #ffffff as string"))
            {
                test.RunOnce();

                Assert.AreEqual("#FFFFFF", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastListAsString()
        {
            using (var test = new ScriptTest(@"Print [0, 1, 2] as string"))
            {
                test.RunOnce();

                Assert.AreEqual("[0,1,2]", test.Logger[0]);
            }
        }
        #endregion String

        #region Vector
        [TestMethod]
        public void CastBoolAsVector() {
            using (var test = new ScriptTest(@"Print true as vector")) {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert boolean to vector", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastNumberAsVector() {
            using (var test = new ScriptTest(@"Print 1.0 as vector"))
            {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert number to vector", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastStringAsVector() {
            var script = @"
Print ""0:0:7"" as vector
Print ""abc"" as vector
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("0:0:7", test.Logger[0]);
                Assert.AreEqual("Exception Occurred:", test.Logger[1]);

            }
        }

        [TestMethod]
        public void CastVectorAsVector() {
            using (var test = new ScriptTest(@"Print 0:0:7 as vector")) {
                test.RunOnce();

                Assert.AreEqual("0:0:7", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastColorAsVector() {
            using (var test = new ScriptTest(@"Print #ffffff as vector")) {
                test.RunOnce();

                Assert.AreEqual("1:1:1", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastListAsVector() {
            using (var test = new ScriptTest(@"Print [0, 1, 2] as vector")) {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert list to vector", test.Logger[1]);
            }
        }
        #endregion Vector

        #region Color
        [TestMethod]
        public void CastBoolAsColor() {
            using (var test = new ScriptTest(@"Print true as color")) {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert boolean to color", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastNumberAsColor() {
            var script = @"
Print 1 as color
Print -1 as color
Print 2 as color
";

            using (var test = new ScriptTest(script))
            {
                test.RunOnce();

                Assert.AreEqual("#FFFFFF", test.Logger[0]);
                Assert.AreEqual("#000000", test.Logger[0]);
                Assert.AreEqual("#FFFFFF", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastStringAsColor() {
            var script = @"
Print ""#ffffff"" as color
Print ""red"" as color
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("#FFFFFF", test.Logger[0]);
                Assert.AreEqual("#FF0000", test.Logger[1]);

            }
        }

        [TestMethod]
        public void CastVectorAsColor() {
            var script = @"
Print 255:0:0 as color
Print -128:128:356 as color
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("#FF0000", test.Logger[0]);
                Assert.AreEqual("#0080FF", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastColorAsColor() {
            using (var test = new ScriptTest(@"Print #ffffff as color")) {
                test.RunOnce();

                Assert.AreEqual("#FFFFFF", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastListAsColor() {
            using (var test = new ScriptTest(@"Print [0, 1, 2] as color")) {
                test.RunOnce();

                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert list to color", test.Logger[1]);
            }
        }
        #endregion Color

        #region List
        [TestMethod]
        public void CastBoolAsList() {
            using (var test = new ScriptTest(@"Print true as list")) {
                test.RunOnce();

                Assert.AreEqual("[True]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastNumberAsList() {
            using (var test = new ScriptTest(@"Print 1 as list")) {
                test.RunOnce();

                Assert.AreEqual("[1]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastStringAsList() {
            using (var test = new ScriptTest(@"Print ""abc"" as list")) {
                test.RunOnce();

                Assert.AreEqual("[abc]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastVectorAsList() {
            using (var test = new ScriptTest(@"Print 0:0:7 as list")) {
                test.RunOnce();

                Assert.AreEqual("[0:0:7]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastColorAsList() {
            using (var test = new ScriptTest(@"Print #ffffff as list")) {
                test.RunOnce();

                Assert.AreEqual("[#FFFFFF]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastListAsList() {
            using (var test = new ScriptTest(@"Print [0, 1, 2] as list")) {
                test.RunOnce();

                Assert.AreEqual("[0,1,2]", test.Logger[0]);
            }
        }
        #endregion List
    }
}
