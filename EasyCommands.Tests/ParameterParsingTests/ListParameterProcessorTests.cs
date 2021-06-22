using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Malware.MDKUtilities;
using IngameScript;
using static IngameScript.Program;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class ListParameterProcessorTests {
        [TestMethod]
        public void AssignVariableToBasicStaticList() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myList to [\"one\", \"two\", \"three\"]");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignmentCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("myList", assignmentCommand.variableName);
            Assert.IsTrue(assignmentCommand.variable is ListIndexVariable);
            ListIndexVariable listIndex = (ListIndexVariable)assignmentCommand.variable;
            Primitive list = listIndex.expectedList.GetValue();
            Assert.AreEqual(Return.LIST, list.GetPrimitiveType());
            List<Variable> listItems = CastList(list).GetTypedValue().GetValues();
            Assert.AreEqual(3, listItems.Count);
            Assert.AreEqual("one", CastString(listItems[0].GetValue()).GetTypedValue());
            Assert.AreEqual("two", CastString(listItems[1].GetValue()).GetTypedValue());
            Assert.AreEqual("three", CastString(listItems[2].GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void AssignVariableIndexToVariable() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myList[0] to \"value\"");
            Assert.IsTrue(command is ListVariableAssignmentCommand);
            ListVariableAssignmentCommand assignmentCommand = (ListVariableAssignmentCommand)command;
            Assert.IsTrue(assignmentCommand.list.expectedList is InMemoryVariable);
            InMemoryVariable listName = (InMemoryVariable)assignmentCommand.list.expectedList;
            Assert.AreEqual("myList", listName.variableName);
            List<Variable> listIndexes = CastList(assignmentCommand.list.index.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0, CastNumber(listIndexes[0].GetValue()).GetTypedValue());
            Primitive value = assignmentCommand.value.GetValue();
            Assert.AreEqual("value", CastString(value).GetTypedValue());
        }

        [TestMethod]
        public void AssignVariableIndexToStaticList() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myList[0] to [\"one\", \"two\", \"three\"]");
            Assert.IsTrue(command is ListVariableAssignmentCommand);
            ListVariableAssignmentCommand assignmentCommand = (ListVariableAssignmentCommand)command;
            Assert.IsTrue(assignmentCommand.list.expectedList is InMemoryVariable);
            InMemoryVariable listName = (InMemoryVariable)assignmentCommand.list.expectedList;
            Assert.AreEqual("myList", listName.variableName);
            List<Variable> listIndexes = CastList(assignmentCommand.list.index.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0, CastNumber(listIndexes[0].GetValue()).GetTypedValue());
            Primitive list = assignmentCommand.value.GetValue();
            Assert.AreEqual(Return.LIST, list.GetPrimitiveType());
            List<Variable> listItems = CastList(list).GetTypedValue().GetValues();
            Assert.AreEqual(3, listItems.Count);
            Assert.AreEqual("one", CastString(listItems[0].GetValue()).GetTypedValue());
            Assert.AreEqual("two", CastString(listItems[1].GetValue()).GetTypedValue());
            Assert.AreEqual("three", CastString(listItems[2].GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void AssignMultipleVariableIndexesToStaticValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myList[0 .. 2 + [4]] to \"value\"");
            Assert.IsTrue(command is ListVariableAssignmentCommand);
            ListVariableAssignmentCommand assignmentCommand = (ListVariableAssignmentCommand)command;
            Assert.IsTrue(assignmentCommand.list.expectedList is InMemoryVariable);
            InMemoryVariable listName = (InMemoryVariable)assignmentCommand.list.expectedList;
            Assert.AreEqual("myList", listName.variableName);
            List<Variable> listIndexes = CastList(assignmentCommand.list.index.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(Return.LIST, listIndexes[0].GetValue().GetPrimitiveType());
            listIndexes = CastList(listIndexes[0].GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(4, listIndexes.Count);
            Assert.AreEqual(0, CastNumber(listIndexes[0].GetValue()).GetTypedValue());
            Assert.AreEqual(1, CastNumber(listIndexes[1].GetValue()).GetTypedValue());
            Assert.AreEqual(2, CastNumber(listIndexes[2].GetValue()).GetTypedValue());
            Assert.AreEqual(4, CastNumber(listIndexes[3].GetValue()).GetTypedValue());
            Assert.AreEqual("value", CastString(assignmentCommand.value.GetValue()).GetValue());
        }

        [TestMethod]
        public void AssignVariableToListIndexValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myValue to [1, 2, 3][0]");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignmentCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("myValue", assignmentCommand.variableName);
            Primitive value = assignmentCommand.variable.GetValue();
            Assert.AreEqual(1f, CastNumber(value).GetTypedValue());
        }

        [TestMethod]
        public void AssignVariableToListSubRange() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myValue to [3..10][2..4]");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignmentCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("myValue", assignmentCommand.variableName);
            List<Variable> values = CastList(assignmentCommand.variable.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(3, values.Count);
            Assert.AreEqual(5f, CastNumber(values[0].GetValue()).GetTypedValue());
            Assert.AreEqual(6f, CastNumber(values[1].GetValue()).GetTypedValue());
            Assert.AreEqual(7f, CastNumber(values[2].GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void AssignListAtIndexValuesToAnotherList() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myList[1, 2, 3] to [0]");
            Assert.IsTrue(command is ListVariableAssignmentCommand);
            ListVariableAssignmentCommand assignmentCommand = (ListVariableAssignmentCommand)command;
            Assert.IsTrue(assignmentCommand.list.expectedList is InMemoryVariable);
            InMemoryVariable listName = (InMemoryVariable)assignmentCommand.list.expectedList;
            Assert.AreEqual("myList", listName.variableName);
            List<Variable> listIndexes = CastList(assignmentCommand.list.index.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(3, listIndexes.Count);
            Assert.AreEqual(1, CastNumber(listIndexes[0].GetValue()).GetTypedValue());
            Assert.AreEqual(2, CastNumber(listIndexes[1].GetValue()).GetTypedValue());
            Assert.AreEqual(3, CastNumber(listIndexes[2].GetValue()).GetTypedValue());
            List<Variable> assignedValue = CastList(assignmentCommand.value.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(1, assignedValue.Count);
            Assert.AreEqual(0f, assignedValue[0].GetValue().GetValue());
        }

        [TestMethod]
        public void AssignVariableToMultiDimensionalArray() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myList to [ [0,1,2] , [3,4,5] ]");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignmentCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("myList", assignmentCommand.variableName);
            List<Variable> values = CastList(assignmentCommand.variable.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(2, values.Count);

            List<Variable> myList0 = CastList(values[0].GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(0f, CastNumber(myList0[0].GetValue()).GetTypedValue());
            Assert.AreEqual(1f, CastNumber(myList0[1].GetValue()).GetTypedValue());
            Assert.AreEqual(2f, CastNumber(myList0[2].GetValue()).GetTypedValue());

            List<Variable> myList1 = CastList(values[1].GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(3f, CastNumber(myList1[0].GetValue()).GetTypedValue());
            Assert.AreEqual(4f, CastNumber(myList1[1].GetValue()).GetTypedValue());
            Assert.AreEqual(5f, CastNumber(myList1[2].GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void AssignVariableToMultiDimensionalArrayValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myValue to [ [0,1,2] , [3,4,5] ][1][2]");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignmentCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("myValue", assignmentCommand.variableName);
            Assert.AreEqual(5f, CastNumber(assignmentCommand.variable.GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void AssignVariableToMultiDimensionalArraySubList() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myValue to [ [0,1,2] , [3,4,5] ][1]");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignmentCommand = (VariableAssignmentCommand)command;
            Assert.AreEqual("myValue", assignmentCommand.variableName);
            List<Variable> listValues = CastList(assignmentCommand.variable.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(3, listValues.Count);
            Assert.AreEqual(3f, listValues[0].GetValue().GetValue());
            Assert.AreEqual(4f, listValues[1].GetValue().GetValue());
            Assert.AreEqual(5f, listValues[2].GetValue().GetValue());
        }
    }
}
