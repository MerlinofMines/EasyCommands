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
            List<Variable> listItems = CastList(list).GetTypedValue();
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
            Assert.AreEqual(0, CastNumber(assignmentCommand.list.index.GetValue()).GetTypedValue());
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
            Assert.AreEqual(0, CastNumber(assignmentCommand.list.index.GetValue()).GetTypedValue());
            Primitive list = assignmentCommand.value.GetValue();
            Assert.AreEqual(Return.LIST, list.GetPrimitiveType());
            List<Variable> listItems = CastList(list).GetTypedValue();
            Assert.AreEqual(3, listItems.Count);
            Assert.AreEqual("one", CastString(listItems[0].GetValue()).GetTypedValue());
            Assert.AreEqual("two", CastString(listItems[1].GetValue()).GetTypedValue());
            Assert.AreEqual("three", CastString(listItems[2].GetValue()).GetTypedValue());
        }

        [TestMethod]
        public void AssignMultipleVariableIndexesToStaticValue() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign myList[0 .. 2 , 4] to \"value\"");
            Assert.IsTrue(command is ListVariableAssignmentCommand);
            ListVariableAssignmentCommand assignmentCommand = (ListVariableAssignmentCommand)command;
            Assert.IsTrue(assignmentCommand.list.expectedList is InMemoryVariable);
            InMemoryVariable listName = (InMemoryVariable)assignmentCommand.list.expectedList;
            Assert.AreEqual("myList", listName.variableName);
            List<Variable> listIndexes = CastList(assignmentCommand.list.index.GetValue()).GetTypedValue();
            Assert.AreEqual(4, listIndexes.Count);
            Assert.AreEqual(0, CastNumber(listIndexes[0].GetValue()).GetTypedValue());
            Assert.AreEqual(1, CastNumber(listIndexes[1].GetValue()).GetTypedValue());
            Assert.AreEqual(2, CastNumber(listIndexes[2].GetValue()).GetTypedValue());
            Assert.AreEqual(4, CastNumber(listIndexes[3].GetValue()).GetTypedValue());
            Assert.AreEqual("value", CastString(assignmentCommand.value.GetValue()).GetValue());
        }
    }
}
