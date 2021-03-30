using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class BooleanLogicParameterProcessorTests {
        [TestMethod]
        public void AndSimpleVariableCondition() {
            var command = ParseCommand("if true and false turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void AndBlockCondition() {
            var command = ParseCommand("if the \"batteries\" are on but are not recharging turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void AndAggregateConditions() {
            var command = ParseCommand("if the \"batteries\" are on but none of the \"batteries\" is recharging turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void OrSimpleVariableCondition() {
            var command = ParseCommand("if true or false turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void OrBlockCondition() {
            var command = ParseCommand("if any of the \"batteries\" ratio is less than 0.5 or is recharging turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void OrAggregateConditions() {
            var command = ParseCommand("if the \"batteries\" are off or any of the \"batteries\" ratio is less than 0.25 turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void NotVariableCondition() {
            var command = ParseCommand("if not true turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void NotBlockCondition() {
            var command = ParseCommand("if not true turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void NotAggregateBlockCondition() {
            var command = ParseCommand("if not all of the \"batteries\" ratio > 0.75 turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void ImplicitSelectorUsedInAggregateCondition() {
            var command = ParseCommand("if any battery ratio < 0.75 turn on the generators");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.Condition is AggregateConditionVariable);
            AggregateConditionVariable condition = (AggregateConditionVariable)conditionalCommand.Condition;
            Assert.IsTrue(condition.entityProvider is AllEntityProvider);
            Assert.AreEqual(BlockType.BATTERY, condition.entityProvider.GetBlockType());
            Assert.AreEqual(AggregationMode.ANY, condition.aggregationMode);
            Assert.IsTrue(condition.blockCondition is BlockPropertyCondition);
            BlockPropertyCondition propertyCondition = (BlockPropertyCondition)condition.blockCondition;
            Assert.AreEqual(PropertyType.RATIO, propertyCondition.property);

            Assert.IsTrue(conditionalCommand.conditionMetCommand is BlockCommand);
            BlockCommand metCommand = (BlockCommand)conditionalCommand.conditionMetCommand;
            Assert.IsTrue(metCommand.entityProvider is AllEntityProvider);
            Assert.AreEqual(BlockType.GENERATOR, metCommand.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void ImplicitSelectorUsedInAggregateConditionWithNot() {
            var command = ParseCommand("if not all of the batteries ratio < 0.75 turn on the generators");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.Condition is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)conditionalCommand.Condition;
            Assert.AreEqual(UniOperandType.NOT, variable.operand);
            Assert.IsTrue(variable.a is AggregateConditionVariable);
            AggregateConditionVariable condition = (AggregateConditionVariable)variable.a;
            Assert.IsTrue(condition.entityProvider is AllEntityProvider);
            Assert.AreEqual(BlockType.BATTERY, condition.entityProvider.GetBlockType());
            Assert.AreEqual(AggregationMode.ALL, condition.aggregationMode);
            Assert.IsTrue(condition.blockCondition is BlockPropertyCondition);
            BlockPropertyCondition propertyCondition = (BlockPropertyCondition)condition.blockCondition;
            Assert.AreEqual(PropertyType.RATIO, propertyCondition.property);

            Assert.IsTrue(conditionalCommand.conditionMetCommand is BlockCommand);
            BlockCommand metCommand = (BlockCommand)conditionalCommand.conditionMetCommand;
            Assert.IsTrue(metCommand.entityProvider is AllEntityProvider);
            Assert.AreEqual(BlockType.GENERATOR, metCommand.entityProvider.GetBlockType());
        }
    }
}
