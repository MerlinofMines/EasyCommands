using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests {
    [TestClass]
    public class ParenthesisParameterProcessorTests {
        [TestMethod]
        public void BasicVariableParentheses() {
            var command = ParseCommand("set the \"pistons\" height to ( {a} + {b} )");
            Assert.IsTrue(command is BlockCommand);
        }

        [TestMethod]
        public void MultipleVariableParenthesis() {
            var command = ParseCommand("if ( {a} > {b} ) set the \"pistons\" height to ( {a} + {b} )");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void EmbeddedParanthesis() {
            var command = ParseCommand("if ( ( {a} + {b} ) > {c} ) set the \"pistons\" height to ( {a} + {b} )");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Variable condition = conditionalCommand.Condition;
            Assert.IsTrue(condition is ComparisonVariable);
            ComparisonVariable comparison = (ComparisonVariable)condition;
            Assert.IsTrue(comparison.a is BiOperandVariable);
        }
    }
}
