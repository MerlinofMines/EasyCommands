using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class AggregateBlockPropertyProcessorTests {
        [TestMethod]
        public void AssignCountOfBlocks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to count of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.COUNT, aggregate.aggregationType);
        }

        [TestMethod]
        public void AggregatePropertyUsedInComparison() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("wait if count of the \"forward guns\" is 0");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.Condition is ComparisonVariable);
            ComparisonVariable comparison = (ComparisonVariable)conditionalCommand.Condition;
            Assert.IsTrue(comparison.a is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)comparison.a;
            Assert.AreEqual(PropertyAggregate.COUNT, aggregate.aggregationType);
            Assert.IsTrue(comparison.b is StaticVariable);
            Assert.AreEqual(0f, comparison.b.GetValue().value);
        }

        [TestMethod]
        public void AssignSumOfBlocks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to sum of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.SUM, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignAverageOfBlocks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to average of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.AVG, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignMinimumOfBlocks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to minimum of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.MIN, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignMaximumOfBlocks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to maximum of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.MAX, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignSumOfBlocksWithProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to sum of the \"forward guns\" range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.SUM, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignSumOfBlocksWithVariableProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to sum of the \"cargo containers\" \"gold ingot\" amount");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.SUM, aggregate.aggregationType);
            Assert.AreEqual(ValueProperty.AMOUNT + "", aggregate.property.propertyType);
            Assert.AreEqual("gold ingot", aggregate.property.attributeValue.GetValue().value);
        }

        [TestMethod]
        public void AssignAverageOfBlocksWithPropertyFirst() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to avg range of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.AVG, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignAvgOfBlocksWithSelectorFirst() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to \"forward guns\" average range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.AVG, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignAvgOfBlocksWithAggregationFirst() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to \"forward guns\" range average");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.AVG, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to the average gun range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.AVG, aggregate.aggregationType);
            Assert.IsTrue(aggregate.entityProvider is AllEntityProvider);
            Assert.AreEqual(Block.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitAggregate() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to the \"test gun\" range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.SUM, aggregate.aggregationType);
            Assert.IsTrue(aggregate.entityProvider is SelectorEntityProvider);
            Assert.AreEqual(Block.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitAggregateInParentheses() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to the ( \"test gun\" range )" );
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.SUM, aggregate.aggregationType);
            Assert.IsTrue(aggregate.entityProvider is SelectorEntityProvider);
            Assert.AreEqual(Block.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitAggregateAndImplicitSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to the gun range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.SUM, aggregate.aggregationType);
            Assert.IsTrue(aggregate.entityProvider is AllEntityProvider);
            Assert.AreEqual(Block.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitAggregateAndMySelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to my location");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregate.SUM, aggregate.aggregationType);
            Assert.AreEqual(Property.POSITION + "", aggregate.property.propertyType);
            Assert.IsTrue(aggregate.entityProvider is SelfEntityProvider);
            Assert.AreEqual(Block.PROGRAM, aggregate.entityProvider.GetBlockType());
        }
    }
}
