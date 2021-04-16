using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class AggregateBlockPropertyProcessorTests {
        [TestMethod]
        public void AssignCountOfBlocks() {
            var command = ParseCommand("assign \"a\" to count of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.COUNT, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignSumOfBlocks() {
            var command = ParseCommand("assign \"a\" to sum of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.SUM, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignAverageOfBlocks() {
            var command = ParseCommand("assign \"a\" to average of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.AVG, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignMinimumOfBlocks() {
            var command = ParseCommand("assign \"a\" to minimum of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.MIN, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignMaximumOfBlocks() {
            var command = ParseCommand("assign \"a\" to maximum of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.MAX, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignSumOfBlocksWithProperty() {
            var command = ParseCommand("assign \"a\" to sum of the \"forward guns\" range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.SUM, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignAverageOfBlocksWithPropertyFirst() {
            var command = ParseCommand("assign \"a\" to avg range of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.AVG, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignAvgOfBlocksWithSelectorFirst() {
            var command = ParseCommand("assign \"a\" to \"forward guns\" average range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.AVG, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignAvgOfBlocksWithAggregationFirst() {
            var command = ParseCommand("assign \"a\" to \"forward guns\" range average");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.AVG, aggregate.aggregationType);
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitSelector() {
            var command = ParseCommand("assign \"a\" to the average gun range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.AVG, aggregate.aggregationType);
            Assert.IsTrue(aggregate.entityProvider is AllEntityProvider);
            Assert.AreEqual(BlockType.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitAggregate() {
            var command = ParseCommand("assign \"a\" to the \"test gun\" range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.VALUE, aggregate.aggregationType);
            Assert.IsTrue(aggregate.entityProvider is SelectorEntityProvider);
            Assert.AreEqual(BlockType.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitAggregateInParentheses() {
            var command = ParseCommand("assign \"a\" to the ( \"test gun\" range )" );
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.VALUE, aggregate.aggregationType);
            Assert.IsTrue(aggregate.entityProvider is SelectorEntityProvider);
            Assert.AreEqual(BlockType.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitAggregateAndImplicitSelector() {
            var command = ParseCommand("assign \"a\" to the gun range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.VALUE, aggregate.aggregationType);
            Assert.IsTrue(aggregate.entityProvider is AllEntityProvider);
            Assert.AreEqual(BlockType.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitAggregateAndMySelector() {
            var command = ParseCommand("assign \"a\" to my location");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            Assert.AreEqual(PropertyAggregatorType.VALUE, aggregate.aggregationType);
            Assert.AreEqual(PropertyType.POSITION, aggregate.property);
            Assert.IsTrue(aggregate.entityProvider is SelfEntityProvider);
            Assert.AreEqual(BlockType.PROGRAM, aggregate.entityProvider.GetBlockType());
        }
    }
}
