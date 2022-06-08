using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using IngameScript;
using static IngameScript.Program;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;

namespace EasyCommands.Tests.ParameterParsingTests {
    [TestClass]
    public class SelectorLogicParameterProcessorTests : ForceLocale {
        [TestMethod]
        public void BasicSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("recharge the \"batteries\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.AreEqual(Block.BATTERY, sep.GetBlockType());
            Assert.IsTrue(sep.selectorType.isGroup.Value);
            Assert.IsTrue(sep.selector is StaticVariable);
            Assert.AreEqual("batteries", CastString(sep.selector.GetValue()));
        }

        [TestMethod]
        public void ConditionalSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("recharge \"batteries\" whose ratio < 0.5");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is ConditionalSelector);
        }

        [TestMethod]
        public void IndexSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on \"batteries\" @ 0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexSelector);
        }

        [TestMethod]
        public void ListIndexSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on \"batteries\"[0]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexSelector);
            IndexSelector iep = (IndexSelector)bc.entityProvider;
            List<IVariable> listIndexes = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
        }

        [TestMethod]
        public void AssignListIndexSelectorValuePlusList() {
            var program = CreateProgram();
            var command = program.ParseCommand("set my display[0] to \"Offset: \" + [myOffset]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexSelector);
            IndexSelector iep = (IndexSelector)bc.entityProvider;
            List<IVariable> listIndexes = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
        }

        [TestMethod]
        public void InLineIndexSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on \"batteries\" @0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexSelector);
        }

        [TestMethod]
        public void ConditionalIndexSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("set the \"batteries\" whose ratio < 0.5 @0 to recharge");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexSelector);
            IndexSelector iep = (IndexSelector)bc.entityProvider;
            Assert.IsTrue(iep.selector is ConditionalSelector);
        }

        [TestMethod]
        public void ConditionalIndexSelectorWithValueProperty() {
            var program = CreateProgram();
            var command = program.ParseCommand("drain the \"cargo containers\" whose \"gold ingot\" amount < 0.5");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is ConditionalSelector);
        }

        [TestMethod]
        public void LastBlockTypeImpliedIsUsed() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the \"Boom Door Program\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.AreEqual(Block.PROGRAM, sep.GetBlockType());
        }

        [TestMethod]
        public void SelectorVariableSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the $a sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.IsTrue(sep.selector is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)sep.selector;
            Assert.AreEqual("a", CastString(variable.GetValue()));
            Assert.AreEqual(Block.SOUND, sep.selectorType.blockType.Value);
        }

        [TestMethod]
        public void VariableSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the (a + \" test\") sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.IsTrue(sep.selector is BiOperandVariable);
            BiOperandVariable variable = (BiOperandVariable)sep.selector;
            Assert.AreEqual("a test", CastString(variable.GetValue()));
            Assert.AreEqual(Block.SOUND, sep.selectorType.blockType.Value);
        }

        [TestMethod]
         public void AmbiguousVariableSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the a sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.IsTrue(sep.selector is AmbiguousStringVariable);
            Assert.AreEqual("a", ((AmbiguousStringVariable)sep.selector).value);
            Assert.AreEqual(Block.SOUND, sep.selectorType.blockType.Value);
        }

        [TestMethod]
        public void StringSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand(@"turn on the ""a"" sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.IsTrue(sep.selector is StaticVariable);
            Assert.AreEqual("a", CastString(sep.selector.GetValue()));
            Assert.AreEqual(Block.SOUND, sep.selectorType.blockType.Value);
        }

        [TestMethod]
        public void ExplicitStringSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand(@"turn on the 'a' sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.IsTrue(sep.selector is StaticVariable);
            Assert.AreEqual("a", CastString(sep.selector.GetValue()));
            Assert.AreEqual(Block.SOUND, sep.selectorType.blockType.Value);
        }

        [TestMethod]
        public void ListVariableSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the a[0] sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.AreEqual(Block.SOUND, sep.selectorType.blockType.Value);
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable variable = (ListIndexVariable)sep.selector;
            Assert.IsTrue(variable.expectedList is AmbiguousStringVariable);
            AmbiguousStringVariable list = (AmbiguousStringVariable)variable.expectedList;
            Assert.AreEqual("a", list.value);
            Assert.IsTrue(variable.index is IndexVariable);
            IndexVariable index = (IndexVariable)variable.index;
            Assert.AreEqual(0, CastNumber(CastList(index.GetValue()).GetValues()[0].GetValue()));
        }

        [TestMethod]
        public void MultiDimensionalListVariableSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the a[0][1] sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.AreEqual(Block.SOUND, sep.selectorType.blockType.Value);
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable second = (ListIndexVariable)sep.selector;
            Assert.IsTrue(second.index is IndexVariable);
            IndexVariable secondIndex = (IndexVariable)second.index;
            Assert.AreEqual(1, CastNumber(CastList(secondIndex.GetValue()).GetValues()[0].GetValue()));

            Assert.IsTrue(second.expectedList is ListIndexVariable);
            ListIndexVariable first = (ListIndexVariable)second.expectedList;
            Assert.IsTrue(first.expectedList is AmbiguousStringVariable);
            AmbiguousStringVariable firstList = (AmbiguousStringVariable)first.expectedList;
            Assert.AreEqual("a", firstList.value);
            Assert.IsTrue(first.index is IndexVariable);
            IndexVariable firstIndex = (IndexVariable)first.index;
            Assert.AreEqual(0, CastNumber(CastList(firstIndex.GetValue()).GetValues()[0].GetValue()));
        }

        [TestMethod]
        public void SelectorVariableSelectorWithIndex() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the $mySirens[0]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexSelector);
            IndexSelector iep = (IndexSelector)bc.entityProvider;
            List<IVariable> listIndexes = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
            Assert.IsTrue(iep.selector is BlockSelector);
            BlockSelector variableSelector = (BlockSelector)iep.selector;
            Assert.IsTrue(variableSelector.selector is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)variableSelector.selector;
            Assert.AreEqual("mySirens", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void SelectorVariableInterpretListIndexAsSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on $(mySirens[0])");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable selector = (ListIndexVariable)sep.selector;
            List<IVariable> listIndexes = CastList(selector.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
            Assert.IsTrue(selector.expectedList is AmbiguousStringVariable);
            AmbiguousStringVariable list = (AmbiguousStringVariable)selector.expectedList;
            Assert.AreEqual("mySirens", list.value);
        }

        [TestMethod]
        public void SelectorVariableInterpretListIndexAsSelectorWithBlockType() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on $(mySirens[0]) sirens");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.AreEqual(Block.SOUND, sep.selectorType.blockType.Value);
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable selector = (ListIndexVariable)sep.selector;
            List<IVariable> listIndexes = CastList(selector.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
            Assert.IsTrue(selector.expectedList is AmbiguousStringVariable);
            AmbiguousStringVariable list = (AmbiguousStringVariable)selector.expectedList;
            Assert.AreEqual("mySirens", list.value);
        }

        [TestMethod]
        public void SelectorVariableInterpretListIndexAsSelectorWithIndex() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on $(mySirens[0])[1]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexSelector);
            IndexSelector iep = (IndexSelector)bc.entityProvider;
            List<IVariable> outerIndex = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, outerIndex.Count);
            Assert.AreEqual(1f, outerIndex[0].GetValue().value);

            Assert.IsTrue(iep.selector is BlockSelector);
            BlockSelector sep = (BlockSelector)iep.selector;
            Assert.IsTrue(sep.selector is ListIndexVariable);
            ListIndexVariable selector = (ListIndexVariable)sep.selector;
            List<IVariable> innerIndex = CastList(selector.index.GetValue()).GetValues();
            Assert.AreEqual(1, innerIndex.Count);
            Assert.AreEqual(0f, innerIndex[0].GetValue().value);
            Assert.IsTrue(selector.expectedList is AmbiguousStringVariable);
            AmbiguousStringVariable list = (AmbiguousStringVariable)selector.expectedList;
            Assert.AreEqual("mySirens", list.value);
        }

        [TestMethod]
        public void SelectorVariableSelectorWithBlockTypeAndIndex() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the $mySirens sirens [0]");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexSelector);
            IndexSelector iep = (IndexSelector)bc.entityProvider;
            List<IVariable> listIndexes = CastList(iep.index.GetValue()).GetValues();
            Assert.AreEqual(1, listIndexes.Count);
            Assert.AreEqual(0f, listIndexes[0].GetValue().value);
            Assert.IsTrue(iep.selector is BlockSelector);
            BlockSelector variableSelector = (BlockSelector)iep.selector;
            Assert.IsTrue(variableSelector.selector is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)variableSelector.selector;
            Assert.AreEqual("mySirens", CastString(variable.GetValue()));
            Assert.IsTrue(variableSelector.selectorType.isGroup.Value);
            Assert.AreEqual(Block.SOUND, variableSelector.selectorType.blockType.Value);
        }

        [TestMethod]
        public void ImplicitSelectorVariableSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the $a");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockSelector);
            BlockSelector sep = (BlockSelector)bc.entityProvider;
            Assert.IsTrue(sep.selector is AmbiguousStringVariable);
            AmbiguousStringVariable variable = (AmbiguousStringVariable)sep.selector;
            Assert.AreEqual("a", CastString(variable.GetValue()));
        }

        [TestMethod]
        public void MySelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("assign a to my average location");
            Assert.IsTrue(command is VariableAssignmentCommand);
            VariableAssignmentCommand assignCommand = (VariableAssignmentCommand)command;
            Assert.IsTrue(assignCommand.variable is AggregatePropertyVariable);
            AggregatePropertyVariable variable = (AggregatePropertyVariable)assignCommand.variable;
            Assert.IsTrue(variable.entityProvider is SelfSelector);
            Assert.AreEqual(Block.PROGRAM, variable.entityProvider.GetBlockType());
        }

        [TestMethod]
        public void MySelectorWithBlockType() {
            var program = CreateProgram();
            var command = program.ParseCommand("set my display @0 text to \"hello world\"");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is IndexSelector);
            IndexSelector iep = (IndexSelector)bc.entityProvider;
            Assert.IsTrue(iep.selector is SelfSelector);
            Assert.AreEqual(Block.DISPLAY, iep.selector.GetBlockType());
        }

        [TestMethod]
        public void AllSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("set all piston height to 0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockTypeSelector);
            BlockTypeSelector aep = (BlockTypeSelector)bc.entityProvider;
            Assert.AreEqual(Block.PISTON, aep.GetBlockType());
        }

        [TestMethod]
        public void AllSelectorGroup() {
            var program = CreateProgram();
            var command = program.ParseCommand("set the height of all pistons to 0");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockTypeSelector);
            BlockTypeSelector aep = (BlockTypeSelector)bc.entityProvider;
            Assert.AreEqual(Block.PISTON, aep.GetBlockType());
        }

        [TestMethod]
        public void AllSelectorGroupWithCondition() {
            var program = CreateProgram();
            var command = program.ParseCommand("recharge all batteries whose ratio < 0.25");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is ConditionalSelector);
            ConditionalSelector cep = (ConditionalSelector)bc.entityProvider;
            Assert.IsTrue(cep.selector is BlockTypeSelector);
            BlockTypeSelector aep = (BlockTypeSelector)cep.selector;
            Assert.AreEqual(Block.BATTERY, aep.GetBlockType());
        }

        [TestMethod]
        public void ImplicitAllSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the light");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockTypeSelector);
            BlockTypeSelector aep = (BlockTypeSelector)bc.entityProvider;
            Assert.AreEqual(Block.LIGHT, aep.GetBlockType());
        }

        [TestMethod]
        public void ImplicitAllGroupSelector() {
            var program = CreateProgram();
            var command = program.ParseCommand("turn on the lights");
            Assert.IsTrue(command is BlockCommand);
            BlockCommand bc = (BlockCommand)command;
            Assert.IsTrue(bc.entityProvider is BlockTypeSelector);
            BlockTypeSelector aep = (BlockTypeSelector)bc.entityProvider;
            Assert.AreEqual(Block.LIGHT, aep.GetBlockType());
        }
    }
}
