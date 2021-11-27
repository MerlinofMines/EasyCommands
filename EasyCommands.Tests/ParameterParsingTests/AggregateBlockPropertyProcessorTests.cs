using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class AggregateBlockPropertyProcessorTests {
        List<object> aggregationList = new List<object> { 1, 2, 3 };

        [TestMethod]
        public void AssignCountOfBlocks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to count of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 3f); //Count
        }

        [TestMethod]
        public void AggregatePropertyUsedInComparison() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("wait if count of the \"forward guns\" is 0");
            Assert.IsTrue(command is ConditionalCommand);
            ConditionalCommand conditionalCommand = (ConditionalCommand)command;
            Assert.IsTrue(conditionalCommand.condition is ComparisonVariable);
            ComparisonVariable comparison = (ComparisonVariable)conditionalCommand.condition;
            Assert.IsTrue(comparison.a is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)comparison.a;
            VerifyAggregator(aggregate.aggregator, 3f); //Count
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
            VerifyAggregator(aggregate.aggregator, 6f); //Sum
        }

        [TestMethod]
        public void AssignAverageOfBlocks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to average of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 2f); //Avg
        }

        [TestMethod]
        public void AssignMinimumOfBlocks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to minimum of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 1f); //Min
        }

        [TestMethod]
        public void AssignMaximumOfBlocks() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to maximum of the \"forward guns\"");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 3f); //Max
        }

        [TestMethod]
        public void AssignSumOfBlocksWithProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to sum of the \"forward guns\" range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 6f); //Sum
        }

        [TestMethod]
        public void AssignSumOfBlocksWithVariableProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to sum of the \"cargo containers\" \"gold ingot\" amount");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 6f); //Sum
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
            VerifyAggregator(aggregate.aggregator, 2f); //Avg
        }

        [TestMethod]
        public void AssignAvgOfBlocksWithSelectorFirst() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to \"forward guns\" average range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 2f); //Avg
        }

        [TestMethod]
        public void AssignAvgOfBlocksWithAggregationFirst() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to \"forward guns\" range average");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 2f); //Avg
        }

        [TestMethod]
        public void AssignAvgOfBlocksUsingImplicitSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to the average gun range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 2f); //Avg
            Assert.IsTrue(aggregate.entityProvider is BlockTypeSelector);
            Assert.AreEqual(Block.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignSumOfBlocksUsingImplicitAggregate() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to the \"test gun\" range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 6f); //Sum
            Assert.IsTrue(aggregate.entityProvider is BlockSelector);
            Assert.AreEqual(Block.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignSumOfBlocksUsingImplicitAggregateInParentheses() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to the ( \"test gun\" range )" );
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 6f); //Sum
            Assert.IsTrue(aggregate.entityProvider is BlockSelector);
            Assert.AreEqual(Block.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignSumOfBlocksUsingImplicitAggregateAndImplicitSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to the gun range");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 6f); //Sum
            Assert.IsTrue(aggregate.entityProvider is BlockTypeSelector);
            Assert.AreEqual(Block.GUN, aggregate.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void AssignSumOfBlocksUsingImplicitAggregateAndMySelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign \"a\" to my location");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable aggregate = (AggregatePropertyVariable)assignCommand.variable;
            VerifyAggregator(aggregate.aggregator, 6f); //Sum
            Assert.AreEqual(Property.POSITION + "", aggregate.property.propertyType);
            Assert.IsTrue(aggregate.entityProvider is SelfSelector);
            Assert.AreEqual(Block.PROGRAM, aggregate.entityProvider.GetBlockType());
        }

        void VerifyAggregator(Aggregator aggregator, float expectedValue) {
            Assert.AreEqual(expectedValue, CastNumber(aggregator(aggregationList, ResolvePrimitive)));
        }
    }
}
