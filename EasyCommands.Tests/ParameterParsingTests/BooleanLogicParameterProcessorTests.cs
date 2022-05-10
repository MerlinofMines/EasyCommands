using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using static IngameScript.Program;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class BooleanLogicParameterProcessorTests : ForceLocale {
        [TestMethod]
        public void AndSimpleVariableCondition() {
            var program = CreateProgram();
            var command = program.ParseCommand("if true and false turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void AndBlockCondition() {
            var program = CreateProgram();
            var command = program.ParseCommand("if the \"batteries\" are on but are not recharging turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void AndAggregateConditions() {
            var program = CreateProgram();
            var command = program.ParseCommand("if the \"batteries\" are on but none of the \"batteries\" is recharging turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void OrSimpleVariableCondition() {
            var program = CreateProgram();
            var command = program.ParseCommand("if true or false turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void OrBlockCondition() {
            var program = CreateProgram();
            var command = program.ParseCommand("if any of the \"batteries\" ratio is less than 0.5 or is recharging turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void OrAggregateConditions() {
            var program = CreateProgram();
            var command = program.ParseCommand("if the \"batteries\" are off or any of the \"batteries\" ratio is less than 0.25 turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void NotVariableCondition() {
            var program = CreateProgram();
            var command = program.ParseCommand("if not true turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void NotBlockCondition() {
            var program = CreateProgram();
            var command = program.ParseCommand("if not true turn on the \"pistons\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void NotAggregateBlockCondition() {
            var program = CreateProgram();
            var command = program.ParseCommand("if not all of the \"batteries\" ratio > 0.75 turn on the \"generators\"");
            Assert.IsTrue(command is ConditionalCommand);
        }

        [TestMethod]
        public void ImplicitSelectorUsedInAggregateCondition() {
            var program = CreateProgram();
            var command = program.ParseCommand("if any battery ratio < 0.75 turn on the generators");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.condition is AggregateConditionVariable);
            AggregateConditionVariable condition = (AggregateConditionVariable)conditionalCommand.condition;
            Assert.IsTrue(condition.entityProvider is BlockTypeSelector);
            Assert.AreEqual(Block.BATTERY, condition.entityProvider.GetBlockType());
            Assert.AreEqual(AggregationMode.ANY, condition.aggregationMode);
            PropertySupplier property = GetDelegateProperty<PropertySupplier>("property", condition.blockCondition);
            Assert.AreEqual(Property.RATIO + "", property.propertyValues[0].propertyType);

            Assert.IsTrue(conditionalCommand.conditionMetCommand is BlockCommand);
            BlockCommand metCommand = (BlockCommand)conditionalCommand.conditionMetCommand;
            Assert.IsTrue(metCommand.entityProvider is BlockTypeSelector);
            Assert.AreEqual(Block.GENERATOR, metCommand.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void ImplicitSelectorUsedInAggregateConditionWithNot() {
            var program = CreateProgram();
            var command = program.ParseCommand("if not all of the batteries ratio < 0.75 turn on the generators");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.condition is UniOperandVariable);
            UniOperandVariable variable = (UniOperandVariable)conditionalCommand.condition;
            Assert.AreEqual(UniOperand.REVERSE, variable.operand);
            Assert.IsTrue(variable.a is AggregateConditionVariable);
            AggregateConditionVariable condition = (AggregateConditionVariable)variable.a;
            Assert.IsTrue(condition.entityProvider is BlockTypeSelector);
            Assert.AreEqual(Block.BATTERY, condition.entityProvider.GetBlockType());
            Assert.AreEqual(AggregationMode.ALL, condition.aggregationMode);
            PropertySupplier property = GetDelegateProperty<PropertySupplier>("property", condition.blockCondition);
            Assert.AreEqual(Property.RATIO + "", property.propertyValues[0].propertyType);

            Assert.IsTrue(conditionalCommand.conditionMetCommand is BlockCommand);
            BlockCommand metCommand = (BlockCommand)conditionalCommand.conditionMetCommand;
            Assert.IsTrue(metCommand.entityProvider is BlockTypeSelector);
            Assert.AreEqual(Block.GENERATOR, metCommand.entityProvider.GetBlockType());
        }
    }
}
