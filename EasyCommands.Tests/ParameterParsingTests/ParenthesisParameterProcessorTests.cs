using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class ParenthesisParameterProcessorTests : ForceLocale {
        [TestMethod]
        public void BasicVariableParentheses() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set the \"pistons\" height to ( {a} + {b} )");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void MultipleVariableParenthesis() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("if ( {a} > {b} ) set the \"pistons\" height to ( {a} + {b} )");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void EmbeddedParanthesis() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("if ( ( {a} + {b} ) > {c} ) set the \"pistons\" height to ( {a} + {b} )");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            IVariable condition = conditionalCommand.condition;
            Assert.IsTrue(condition is ComparisonVariable);
            ComparisonVariable comparison = (ComparisonVariable)condition;
            Assert.IsTrue(comparison.a is BiOperandVariable);
        }
    }
}
