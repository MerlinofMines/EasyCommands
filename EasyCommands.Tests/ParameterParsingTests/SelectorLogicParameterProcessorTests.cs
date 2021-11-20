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
    public class SelectorLogicParameterProcessorTests {
        [TestMethod]
        public void BasicSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("recharge the \"batteries\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.AreEqual(Block.BATTERY, sep.GetBlockType());
            Assert.IsTrue(sep.isGroup);
            Assert.IsTrue(sep.selector is StaticVariable);
            Assert.AreEqual("batteries", CastString(sep.selector.GetValue()));
        }

        [TestMethod]
        public void ConditionalSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("recharge \"batteries\" whose ratio < 0.5");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is ConditionalEntityProvider);
        }

        [TestMethod]
        public void IndexSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on \"batteries\" @ 0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
        }

        [TestMethod]
        public void ListIndexSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on \"batteries\"[0]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            List<Variable> listIndexes = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
        }

        [TestMethod]
        public void AssignListIndexSelectorValuePlusList() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set my display[0] to \"Offset: \" + [myOffset]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            List<Variable> listIndexes = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
        }

        [TestMethod]
        public void InLineIndexSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on \"batteries\" @0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
        }

        [TestMethod]
        public void ConditionalIndexSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set the \"batteries\" whose ratio < 0.5 @0 to recharge");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            Assert.IsTrue(iep.provider is ConditionalEntityProvider);
        }

        [TestMethod]
        public void ConditionalIndexSelectorWithValueProperty() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("drain the \"cargo containers\" whose \"gold ingot\" amount < 0.5");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is ConditionalEntityProvider);
        }

        [TestMethod]
        public void LastBlockTypeImpliedIsUsed() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the \"Boom Door Program\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.AreEqual(Block.PROGRAM, sep.GetBlockType());
        }

        [TestMethod]
        public void SelectorVariableSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the $a sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.IsTrue(sep.selector is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)sep.selector;
            Assert.AreEqual("a", CastString(variable.GetValue()));
            Assert.AreEqual(Block.SOUND, sep.blockType);
        }

        [TestMethod]
        public void VariableSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the (a + \" test\") sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.IsTrue(sep.selector is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)sep.selector;
            Assert.AreEqual("a test", CastString(variable.GetValue()));
            Assert.AreEqual(Block.SOUND, sep.blockType);
        }

        [TestMethod]
         public void AmbiguousVariableSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the a sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.IsTrue(sep.selector is AmbiguousStringVariable);
            Assert.AreEqual(Block.SOUND, sep.blockType);
        }

        [TestMethod]
        public void ListVariableSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the a[0] sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.AreEqual(Block.SOUND, sep.blockType);
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable variable = (ListIndexVariable)sep.selector;
            Assert.IsTrue(variable.expectedList is InMemoryVariable);
            InMemoryVariable list = (InMemoryVariable)variable.expectedList;
            Assert.AreEqual("a", list.variableName);
            Assert.IsTrue(variable.index is IndexVariable);
            IndexVariable index = (IndexVariable)variable.index;
            Assert.AreEqual(0, CastNumber(CastList(index.GetValue()).GetValues()[0].GetValue()));
        }

        [TestMethod]
        public void MultiDimensionalListVariableSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the a[0][1] sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.AreEqual(Block.SOUND, sep.blockType);
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable second = (ListIndexVariable)sep.selector;
            Assert.IsTrue(second.index is IndexVariable);
            IndexVariable secondIndex = (IndexVariable)second.index;
            Assert.AreEqual(1, CastNumber(CastList(secondIndex.GetValue()).GetValues()[0].GetValue()));

            Assert.IsTrue(second.expectedList is ListIndexVariable);
            ListIndexVariable first = (ListIndexVariable)second.expectedList;
            Assert.IsTrue(first.expectedList is InMemoryVariable);
            InMemoryVariable firstList = (InMemoryVariable)first.expectedList;
            Assert.AreEqual("a", firstList.variableName);
            Assert.IsTrue(first.index is IndexVariable);
            IndexVariable firstIndex = (IndexVariable)first.index;
            Assert.AreEqual(0, CastNumber(CastList(firstIndex.GetValue()).GetValues()[0].GetValue()));
        }

        [TestMethod]
        public void SelectorVariableSelectorWithIndex() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the $mySirens[0]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            List<Variable> listIndexes = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
            Assert.IsTrue(iep.provider is SelectorEntityProvider);
            SelectorEntityProvider variableSelector = (SelectorEntityProvider)iep.provider;
            Assert.IsTrue(variableSelector.selector is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)variableSelector.selector;
            Assert.AreEqual("mySirens", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void SelectorVariableInterpretListIndexAsSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on $(mySirens[0])");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable selector = (ListIndexVariable)sep.selector;
            List<Variable> listIndexes = CastList(selector.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
            Assert.IsTrue(selector.expectedList is InMemoryVariable);
            InMemoryVariable list = (InMemoryVariable)selector.expectedList;
            Assert.AreEqual("mySirens", list.variableName);
        }

        [TestMethod]
        public void SelectorVariableInterpretListIndexAsSelectorWithBlockType() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on $(mySirens[0]) sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.AreEqual(Block.SOUND, sep.blockType);
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable selector = (ListIndexVariable)sep.selector;
            List<Variable> listIndexes = CastList(selector.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
            Assert.IsTrue(selector.expectedList is InMemoryVariable);
            InMemoryVariable list = (InMemoryVariable)selector.expectedList;
            Assert.AreEqual("mySirens", list.variableName);
        }

        [TestMethod]
        public void SelectorVariableInterpretListIndexAsSelectorWithIndex() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on $(mySirens[0])[1]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            List<Variable> outerIndex = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, outerIndex.Count);
            Assert.AreEqual(1f, outerIndex[0].GetValue().value);

            Assert.IsTrue(iep.provider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)iep.provider;
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable selector = (ListIndexVariable)sep.selector;
            List<Variable> innerIndex = CastList(selector.index.GetValue()).GetValues();
            Assert.AreEqual(1, innerIndex.Count);
            Assert.AreEqual(0f, innerIndex[0].GetValue().value);
            Assert.IsTrue(selector.expectedList is InMemoryVariable);
            InMemoryVariable list = (InMemoryVariable)selector.expectedList;
            Assert.AreEqual("mySirens", list.variableName);
        }

        [TestMethod]
        public void SelectorVariableSelectorWithBlockTypeAndIndex() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the $mySirens sirens [0]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            List<Variable> listIndexes = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
            Assert.IsTrue(iep.provider is SelectorEntityProvider);
            SelectorEntityProvider variableSelector = (SelectorEntityProvider)iep.provider;
            Assert.IsTrue(variableSelector.selector is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)variableSelector.selector;
            Assert.AreEqual("mySirens", CastString(variable.GetValue()));
            Assert.IsTrue(variableSelector.isGroup);
            Assert.AreEqual(Block.SOUND, variableSelector.blockType);
        }

        [TestMethod]
        public void ImplicitSelectorVariableSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the $a");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.IsTrue(sep.selector is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)sep.selector;
            Assert.AreEqual("a", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void MySelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("assign a to my average location");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable variable = (AggregatePropertyVariable)assignCommand.variable;
            Assert.IsTrue(variable.entityProvider is SelfEntityProvider);
            Assert.AreEqual(Block.PROGRAM, variable.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void MySelectorWithBlockType() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set my display @0 text to \"hello world\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            Assert.IsTrue(iep.provider is SelfEntityProvider);
            Assert.AreEqual(Block.DISPLAY, iep.provider.GetBlockType());
        }

        [TestMethod]
        public void AllSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set all piston height to 0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)bc.entityProvider;
            Assert.AreEqual(Block.PISTON, aep.GetBlockType());
        }

        [TestMethod]
        public void AllSelectorGroup() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set the height of all pistons to 0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)bc.entityProvider;
            Assert.AreEqual(Block.PISTON, aep.GetBlockType());
        }

        [TestMethod]
        public void AllSelectorGroupWithCondition() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("recharge all batteries whose ratio < 0.25");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is ConditionalEntityProvider);
            ConditionalEntityProvider cep = (ConditionalEntityProvider)bc.entityProvider;
            Assert.IsTrue(cep.provider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)cep.provider;
            Assert.AreEqual(Block.BATTERY, aep.GetBlockType());
        }

        [TestMethod]
        public void ImplicitAllSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the light");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)bc.entityProvider;
            Assert.AreEqual(Block.LIGHT, aep.GetBlockType());
        }

        [TestMethod]
        public void ImplicitAllGroupSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the lights");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is AllEntityProvider);
            AllEntityProvider aep = (AllEntityProvider)bc.entityProvider;
            Assert.AreEqual(Block.LIGHT, aep.GetBlockType());
        }
    }
}
