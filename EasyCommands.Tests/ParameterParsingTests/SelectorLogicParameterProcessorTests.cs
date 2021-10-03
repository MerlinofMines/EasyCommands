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
            Assert.AreEqual("batteries", CastString(sep.selector.GetValue()).GetTypedValue());
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
            List<Variable> listIndexes = CastList(iep.index.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().GetValue());
        }

        [TestMethod]
        public void AssignListIndexSelectorValuePlusList() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("set my display[0] to \"Offset: \" + [offset]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            List<Variable> listIndexes = CastList(iep.index.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().GetValue());
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
        public void VariableSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the $a sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.IsTrue(sep.selector is InMemoryVariable);
            InMemoryVariable variable = (InMemoryVariable)sep.selector;
            Assert.AreEqual("a", variable.variableName);
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
        public void VariableSelectorWithIndex() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the $mySirens[0]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            List<Variable> listIndexes = CastList(iep.index.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().GetValue());
            Assert.IsTrue(iep.provider is SelectorEntityProvider);
            SelectorEntityProvider variableSelector = (SelectorEntityProvider)iep.provider;
            Assert.IsTrue(variableSelector.selector is InMemoryVariable);
            InMemoryVariable variable = (InMemoryVariable)variableSelector.selector;
        }

        [TestMethod]
        public void VariableSelectorWithBlockTypeAndIndex() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the $mySirens sirens [0]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexEntityProvider);
            IndexEntityProvider iep = (IndexEntityProvider)bc.entityProvider;
            List<Variable> listIndexes = CastList(iep.index.GetValue()).GetTypedValue().GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().GetValue());
            Assert.IsTrue(iep.provider is SelectorEntityProvider);
            SelectorEntityProvider variableSelector = (SelectorEntityProvider)iep.provider;
            Assert.IsTrue(variableSelector.selector is InMemoryVariable);
            InMemoryVariable variable = (InMemoryVariable)variableSelector.selector;
            Assert.AreEqual("mySirens", variable.variableName);
            Assert.IsTrue(variableSelector.isGroup);
            Assert.AreEqual(Block.SOUND, variableSelector.blockType);
        }

        [TestMethod]
        public void ImplicitVariableSelector() {
            var program = MDKFactory.CreateProgram<Program>();
            var command = program.ParseCommand("turn on the $a");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is SelectorEntityProvider);
            SelectorEntityProvider sep = (SelectorEntityProvider)bc.entityProvider;
            Assert.IsTrue(sep.selector is InMemoryVariable);
            InMemoryVariable variable = (InMemoryVariable)sep.selector;
            Assert.AreEqual("a", variable.variableName);
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
