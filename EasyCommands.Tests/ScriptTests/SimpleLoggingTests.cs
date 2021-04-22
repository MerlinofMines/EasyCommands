using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleLoggingTests {
        [TestMethod]
        public void PrintCorrectLineNumberWhenUnableToParse() {
            String script = @"
:main
print 'Hello World'
'Hello World'
";
            using (var test = new ScriptTest(script)) {
                Program.LOG_LEVEL = Program.LogLevel.INFO;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Unable to parse command from command parameters at line number: 3"));
            }
        }

        [TestMethod]
        public void PrintCorrectLineNumberWhenUnableToParseAndImpliedMain() {
            String script = @"
print 'Hello World'
'Hello World'
";
            using (var test = new ScriptTest(script)) {
                Program.LOG_LEVEL = Program.LogLevel.INFO;
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Unable to parse command from command parameters at line number: 2"));
            }
        }

        [TestMethod]
        public void PrintCorrectLineNumberWhenUnableToParseNestedCommand() {
            String script = @"
if (true)
  print 'Hello World'
  'Hello World'
";
            using (var test = new ScriptTest(script)) {
                Program.LOG_LEVEL = Program.LogLevel.INFO;
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Unable to parse command from command parameters at line number: 3"));
            }
        }

        [TestMethod]
        public void PrintCorrectLineNumberWhenUnableToParseAfterNestedCommand() {
            String script = @"
if (true)
  print 'Hello World'
  wait 5
'Hello World'
";
            using (var test = new ScriptTest(script)) {
                Program.LOG_LEVEL = Program.LogLevel.INFO;
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Unable to parse command from command parameters at line number: 4"));
            }
        }

        [TestMethod]
        public void PrintCorrectLineNumberWhenUnableToParseIncludesComments() {
            String script = @"
if (true)
  print 'Hello World'
  wait 5
#This line isn't working
'Hello World'
";
            using (var test = new ScriptTest(script)) {
                Program.LOG_LEVEL = Program.LogLevel.INFO;
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Unable to parse command from command parameters at line number: 5"));
            }
        }

        [TestMethod]
        public void PrintCorrectLineNumberWhenUnableToParseIncludesEmptySpace() {
            String script = @"
:main
if (true)
  print 'Hello World'
  wait 5

:broken
#This line isn't working
'Hello World'
";
            using (var test = new ScriptTest(script)) {
                Program.LOG_LEVEL = Program.LogLevel.INFO;
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Unable to parse command from command parameters at line number: 8"));
            }
        }

        [TestMethod]
        public void PrintCorrectLineNumberWhenUnableToParseInsideFunction() {
            String script = @"
:main
if (true)
  print 'Hello World'
  wait 5

:broken
'Hello World'
";
            using (var test = new ScriptTest(script)) {
                Program.LOG_LEVEL = Program.LogLevel.INFO;
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Unable to parse command from command parameters at line number: 7"));
            }
        }

        [TestMethod]
        public void PrintParseTreeWhenDebugging() {
            String script = @"
print 'Total: ' + ( 2 + 5 )
";
            using (var test = new ScriptTest(script)) {
                Program.LOG_LEVEL = Program.LogLevel.DEBUG;

                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("print 'Total: ' ADD ( 2 ADD 5 )"));
                Assert.IsTrue(test.Logger.Contains("2 ADD 5"));
                Assert.IsTrue(test.Logger.Contains("[Variable] ADD [Variable]"));
                Assert.IsTrue(test.Logger.Contains("[Variable]"));
                Assert.IsTrue(test.Logger.Contains("print 'Total: ' ADD [Variable]"));
                Assert.IsTrue(test.Logger.Contains("print [Variable] ADD [Variable]"));
                Assert.IsTrue(test.Logger.Contains("print [Variable]"));
                Assert.IsTrue(test.Logger.Contains("[Command]"));
            }
        }
    }
}
