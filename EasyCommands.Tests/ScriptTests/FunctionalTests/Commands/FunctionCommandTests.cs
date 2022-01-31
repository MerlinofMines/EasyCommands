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
    public class FunctionCommandTests {
        [TestMethod]
        public void CallFunctionExplicitly() {
            String script = @"
:main
call myFunction
print ""Done""

:myFunction
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                Assert.AreEqual("Done", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CallFunctionImplicitly() {
            String script = @"
:main
myFunction
print ""Done""

:myFunction
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                Assert.AreEqual("Done", test.Logger[1]);
            }
        }

        [TestMethod]
        public void GotoFunction() {
            String script = @"
:main
goto myFunction
print ""Done""

:myFunction
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.program.logLevel = Program.LogLevel.INFO;

                test.RunOnce();
                Assert.AreEqual(4, test.Logger.Count);
                Assert.AreEqual("Queued Threads: 1", test.Logger[0]);
                Assert.AreEqual("Async Threads: 0", test.Logger[1]);
                Assert.AreEqual("[Main] main", test.Logger[2]);
                Assert.AreEqual("Running", test.Logger[3]);

                test.Logger.Clear();
                test.RunUntilDone();

                Assert.AreEqual(5, test.Logger.Count);
                Assert.AreEqual("Queued Threads: 1", test.Logger[0]);
                Assert.AreEqual("Async Threads: 0", test.Logger[1]);
                Assert.AreEqual("[Main] myFunction", test.Logger[2]);
                Assert.AreEqual("Hello World", test.Logger[3]);
                Assert.AreEqual("Complete", test.Logger[4]);
            }
        }

        [TestMethod]
        public void CallFunctionExplicitlyWithVariableParameters() {
            String script = @"
:main
set a to ""one""
set b to ""two""

call myFunction a b
print ""Done""

:myFunction param1 param2
print ""Param1: "" + param1
print ""Param2: "" + param2
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Param1: one", test.Logger[0]);
                Assert.AreEqual("Param2: two", test.Logger[1]);
                Assert.AreEqual("Done", test.Logger[2]);
            }
        }

        [TestMethod]
        public void CallFunctionImplicitlyWithVariableParameters() {
            String script = @"
:main
set a to ""one""
set b to ""two""

myFunction a b
print ""Done""

:myFunction param1 param2
print ""Param1: "" + param1
print ""Param2: "" + param2
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Param1: one", test.Logger[0]);
                Assert.AreEqual("Param2: two", test.Logger[1]);
                Assert.AreEqual("Done", test.Logger[2]);
            }
        }

        [TestMethod]
        public void GotoFunctionWithVariableParameters() {
            String script = @"
:main
set a to ""one""
set b to ""two""

goto myFunction a b
print ""Done""

:myFunction param1 param2
print ""Param1: "" + param1
print ""Param2: "" + param2
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Param1: one", test.Logger[0]);
                Assert.AreEqual("Param2: two", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CallVariableFunction() {
            String script = @"
:main
set myFunctionName to ""myFunction""

call myFunctionName

set myFunctionName to ""mySecondFunction""
call myFunctionName

print ""Done""

:myFunction
print ""Hello World""

:mySecondFunction
print ""How are your?""
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
                Assert.AreEqual("How are your?", test.Logger[1]);
                Assert.AreEqual("Done", test.Logger[2]);
            }
        }

        [TestMethod]
        public void GotoVariableFunction() {
            String script = @"
:main
set myFunctionName to ""myFunction""

goto myFunctionName
print ""Done""

:myFunction
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(1, test.Logger.Count);
                Assert.AreEqual("Hello World", test.Logger[0]);
            }
        }

        [TestMethod]
        public void CallVariableFunctionWithVariableParameters() {
            String script = @"
:main
set a to ""one""
set b to ""two""
set myFunctionName to ""myFunction""
call myFunctionName a b
print ""Done""

:myFunction param1 param2
print ""Param1: "" + param1
print ""Param2: "" + param2
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(3, test.Logger.Count);
                Assert.AreEqual("Param1: one", test.Logger[0]);
                Assert.AreEqual("Param2: two", test.Logger[1]);
                Assert.AreEqual("Done", test.Logger[2]);
            }
        }

        [TestMethod]
        public void GotoVariableFunctionWithVariableParameters() {
            String script = @"
:main
set a to ""one""
set b to ""two""
set myFunctionName to ""myFunction""

goto myFunctionName a b
print ""Done""

:myFunction param1 param2
print ""Param1: "" + param1
print ""Param2: "" + param2
";

            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Param1: one", test.Logger[0]);
                Assert.AreEqual("Param2: two", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CallInvalidFunction() {
            String script = @"
:main
call myFunction
print ""Done""
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Invalid Function Name: myFunction", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CallInvalidVariableFunction() {
            String script = @"
:main
set myFunctionName to ""myFunction""

call myFunctionName
print ""Done""
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Invalid Function Name: myFunction", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CallFunctionWithIncorrectParameters() {
            String script = @"
:main
call myFunction
print ""Done""

:myFunction a b
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Function myFunction expects 2 parameters", test.Logger[1]);
            }
        }

        [TestMethod]
        public void CallVariableFunctionWithIncorrectParameters() {
            String script = @"
:main
set myFunctionName to ""myFunction""

call myFunctionName
print ""Done""

:myFunction a b
print ""Hello World""
";

            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.AreEqual(2, test.Logger.Count);
                Assert.AreEqual("Exception Occurred:", test.Logger[0]);
                Assert.AreEqual("Function myFunction expects 2 parameters", test.Logger[1]);
            }
        }
    }
}
