using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyCommands.Tests.ScriptTests
{
    [TestClass]
    public class SimpleCastTests
    {
        #region RightUniOperand
        [TestMethod]
        public void CastBool() {
            using (var test = new ScriptTest(@"
set myVariable to cast true
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
                Assert.AreEqual("boolean", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastTrueStringToBool() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""true""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
                Assert.AreEqual("boolean", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastFalseStringToBool() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""false""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
                Assert.AreEqual("boolean", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastTRUEStringToBool() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""TRUE""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
                Assert.AreEqual("boolean", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastFALSEStringToBool() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""FALSE""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("False", test.Logger[0]);
                Assert.AreEqual("boolean", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastIntegerToNumber() {
            using (var test = new ScriptTest(@"
set myVariable to cast 100
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("100", test.Logger[0]);
                Assert.AreEqual("number", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastIntegerStringToNumber() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""100""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("100", test.Logger[0]);
                Assert.AreEqual("number", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastDecimalToNumber() {
            using (var test = new ScriptTest(@"
set myVariable to cast 3.1415
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("3.1415", test.Logger[0]);
                Assert.AreEqual("number", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastDecimalStringToNumber() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""3.1415""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("3.1415", test.Logger[0]);
                Assert.AreEqual("number", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastVector() {
            using (var test = new ScriptTest(@"
set myVariable to cast 1:2:3
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("1:2:3", test.Logger[0]);
                Assert.AreEqual("vector", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastVectorString() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""1:2:3""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("1:2:3", test.Logger[0]);
                Assert.AreEqual("vector", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastGPSCoordinateStringAsVector() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""GPS:surface:53573.9750085028:-26601.8512032533:12058.8229348438:#FF75C9F1:""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("53573.9750085028:-26601.8512032533:12058.8229348438", test.Logger[0]);
                Assert.AreEqual("vector", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastColor() {
            using (var test = new ScriptTest(@"
set myVariable to cast red
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("#FF0000", test.Logger[0]);
                Assert.AreEqual("color", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastColorString() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""red""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("#FF0000", test.Logger[0]);
                Assert.AreEqual("color", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastHexColor() {
            using (var test = new ScriptTest(@"
set myVariable to cast #FF0000
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("#FF0000", test.Logger[0]);
                Assert.AreEqual("color", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastHexColorString() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""#FF0000""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("#FF0000", test.Logger[0]);
                Assert.AreEqual("color", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastList() {
            using (var test = new ScriptTest(@"
set myVariable to cast [1,2,3]
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("[1,2,3]", test.Logger[0]);
                Assert.AreEqual("list", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastListStringNotSupported() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""[1,2,3]""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("[1,2,3]", test.Logger[0]);
                Assert.AreEqual("string", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastNonPrimitiveReturnOriginalString() {
            using (var test = new ScriptTest(@"
set myVariable to cast unknownString
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("unknownString", test.Logger[0]);
                Assert.AreEqual("string", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastString() {
            using (var test = new ScriptTest(@"
set myVariable to cast ""unknownString""
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("unknownString", test.Logger[0]);
                Assert.AreEqual("string", test.Logger[1]);
            }
        }
        #endregion

        #region LeftUniOperand
        [TestMethod]
        public void BoolStringResolved() {
            using (var test = new ScriptTest(@"
set myVariable to ""true"" resolved
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
                Assert.AreEqual("boolean", test.Logger[1]);
            }

        }

        [TestMethod]
        public void DecimalStringResolved() {
            using (var test = new ScriptTest(@"
set myVariable to ""3.1415"" resolved
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("3.1415", test.Logger[0]);
                Assert.AreEqual("number", test.Logger[1]);
            }
        }

        [TestMethod]
        public void VectorStringResolved() {
            using (var test = new ScriptTest(@"
set myVariable to ""1:2:3"" resolved
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("1:2:3", test.Logger[0]);
                Assert.AreEqual("vector", test.Logger[1]);
            }
        }

        [TestMethod]
        public void HexColorStringResolved() {
            using (var test = new ScriptTest(@"
set myVariable to ""#FF0000"" resolved
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("#FF0000", test.Logger[0]);
                Assert.AreEqual("color", test.Logger[1]);
            }
        }

        [TestMethod]
        public void StringResolved() {
            using (var test = new ScriptTest(@"
set myVariable to ""unknownString"" resolved
print myVariable
print myVariable type
")) {
                test.RunOnce();

                Assert.AreEqual("unknownString", test.Logger[0]);
                Assert.AreEqual("string", test.Logger[1]);
            }
        }
        #endregion

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
                Assert.AreEqual("True", test.Logger[2]);
            }
        }

        [TestMethod]
        public void CastStringAsBool() {
            var script = @"
Print ""True"" as bool
Print ""true"" as bool
Print ""False"" as bool
Print ""false"" as bool
Print ""1"" as bool
Print ""0"" as bool
Print ""-1"" as bool
Print ""abc"" as bool
Print ""0:0:0"" as bool
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("True", test.Logger[0]);
                Assert.AreEqual("True", test.Logger[1]);
                Assert.AreEqual("False", test.Logger[2]);
                Assert.AreEqual("False", test.Logger[3]);
                Assert.AreEqual("True", test.Logger[4]);
                Assert.AreEqual("False", test.Logger[5]);
                Assert.AreEqual("True", test.Logger[6]);
                Assert.AreEqual("False", test.Logger[7]);
                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[8]);
                Assert.AreEqual("Cannot convert vector 0:0:0 to boolean", test.Logger[9]);
            }
        }

        [TestMethod]
        public void CastVectorAsBool() {
            using (var test = new ScriptTest(@"Print 0:0:1 as bool")) {
                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert vector 0:0:1 to boolean", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastColorAsBool() {
            using (var test = new ScriptTest(@"Print #ffffff as bool")) {
                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert color #FFFFFF to boolean", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastListAsBool() {
            using (var test = new ScriptTest(@"Print [0, 1, 2] as bool")) {
                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert list [0,1,2] to boolean", test.Logger[1]);
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

                Assert.AreEqual("1", test.Logger[0]);
                Assert.AreEqual("0", test.Logger[1]);
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
                Assert.AreEqual("0", test.Logger[1]);
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
                Assert.AreEqual("0", test.Logger[1]);
                Assert.AreEqual("1.5", test.Logger[2]);
                Assert.AreEqual("Unknown Exception Occurred:", test.Logger[3]);
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

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert color #FFFFFF to number", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastListAsNumber()
        {
            using (var test = new ScriptTest(@"Print [0, 1, 2] as number"))
            {
                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert list [0,1,2] to number", test.Logger[1]);
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

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert boolean True to vector", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CastNumberAsVector() {
            using (var test = new ScriptTest(@"Print 1.0 as vector"))
            {
                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert number 1 to vector", test.Logger[1]);
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
                Assert.AreEqual("Unknown Exception Occurred:", test.Logger[1]);

            }
        }

        [TestMethod]
        public void CastGPSStringAsVector() {
            var script = @"Print ""GPS:surface:53573.9750085028:-26601.8512032533:12058.8229348438:#FF75C9F1:"" as vector";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual("53573.9750085028:-26601.8512032533:12058.8229348438", test.Logger[0]);
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

                Assert.AreEqual("255:255:255", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CastListAsVector() {
            using (var test = new ScriptTest(@"Print [0, 1, 2] as vector")) {
                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert list [0,1,2] to vector", test.Logger[1]);
            }
        }
        #endregion Vector

        #region Color
        [TestMethod]
        public void CastBoolAsColor() {
            using (var test = new ScriptTest(@"Print true as color")) {
                test.RunOnce();

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert boolean True to color", test.Logger[1]);
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
                Assert.AreEqual("#000000", test.Logger[1]);
                Assert.AreEqual("#FFFFFF", test.Logger[2]);
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
                Assert.AreEqual("#0080FF", test.Logger[1]);
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

                Assert.AreEqual("Runtime Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Cannot convert list [0,1,2] to color", test.Logger[1]);
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
