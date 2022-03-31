using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleListTests {
        [TestMethod]
        public void CheckIfListIsEmpty() {
            String script = @"
:main
assign myList to []
if myList is empty
  Print ""List is empty""
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("List is empty"));
            }
        }

        [TestMethod]
        public void CheckIfListIsNotEmpty() {
            String script = @"
:main
assign myList to [1,2,3]
if myList is not empty
  Print ""List is not empty""
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("List is not empty"));
            }
        }

        [TestMethod]
        public void CompareTwoEqualLists() {
            String script = @"
:main
assign myList to [1,2,3]
if myList is [1,2,3]
  Print ""Lists are equal""
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Lists are equal"));
            }
        }

        [TestMethod]
        public void CompareTwoEqualKeyedLists() {
            String script = @"
:main
assign myList to []
assign myList2 to []
assign myList[""one""] to 1
assign myList2[""one""] to 1

if myList is myList2
  Print ""Lists are equal""
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Lists are equal"));
            }
        }

        [TestMethod]
        public void CompareTwoDifferentLists() {
            String script = @"
:main
assign myList to [1,2,3]
if myList is not [1,2,3,4]
  Print ""Lists are not equal""
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Lists are not equal"));
            }
        }

        [TestMethod]
        public void CompareTwoDifferentListsWithSameValues() {
            String script = @"
:main
assign myList to [1,2,3]
if myList is not [3,2,1]
  Print ""Lists are not equal""
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Lists are not equal"));
            }
        }

        [TestMethod]
        public void CompareTwoDifferentKeyedLists() {
            String script = @"
:main
assign myList to []
assign myList2 to []
assign myList[""one""] to 1
assign myList2[""two""] to 1

if myList is not myList2
  Print ""Lists are not equal""
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Lists are not equal"));
            }
        }

        [TestMethod]
        public void PrintSimpleListSize() {
            String script = @"
:main
assign myList to [1, 2, 3]
Print ""myList Size: "" + count of myList[]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("myList Size: 3"));
            }
        }

        [TestMethod]
        public void PrintSimpleListSum() {
            String script = @"
:main
assign myList to [1, 2, 3]
Print ""myList Sum: "" + sum of myList[]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("myList Sum: 6"));
            }
        }

        [TestMethod]
        public void CheckAllValuesAgainstValue() {
            String script = @"
:main
assign myList to [1, 2, 3]
if all myList[] > 0
  Print ""All values are greater than 0""
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("All values are greater than 0"));
            }
        }

        [TestMethod]
        public void CheckIfListContainsValue() {
            String script = @"
:main
assign myList to [""one"", ""two"", ""three""]
assign i to ""one""
if any myList[] is i
  Print ""myList contains "" + i
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("myList contains one"));
            }
        }

        [TestMethod]
        public void PrintSimpleListValueAtIndex() {
            String script = @"
:main
assign myList to [1, 2, 3]
Print ""myList[2] = "" + myList[2]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("myList[2] = 3"));
            }
        }

        [TestMethod]
        public void IterateOverSimpleListUsingUntil() {
            String script = @"
:main
assign myList to [1, 2, 3]
assign i to 0
until i >= count of myList[]
  Print ""myList["" + i + ""] = "" + myList[i]
  i++
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("myList[0] = 1"));
                Assert.IsTrue(test.Logger.Contains("myList[1] = 2"));
                Assert.IsTrue(test.Logger.Contains("myList[2] = 3"));
            }
        }

        [TestMethod]
        public void IterateOverSimpleListUsingForEach() {
            String script = @"
:main
assign myList to [1, 2, 3]
for each item in myList
  Print ""Item = "" + item
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Item = 1"));
                Assert.IsTrue(test.Logger.Contains("Item = 2"));
                Assert.IsTrue(test.Logger.Contains("Item = 3"));
            }
        }

        [TestMethod]
        public void IterateOverSimpleListUsingForEachIndexes() {
            String script = @"
:main
assign myList to [1, 2, 3]
for each i in 0..count of myList[] - 1
  Print ""myList["" + i + ""] = "" + myList[i]
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("myList[0] = 1"));
                Assert.IsTrue(test.Logger.Contains("myList[1] = 2"));
                Assert.IsTrue(test.Logger.Contains("myList[2] = 3"));
            }
        }

        [TestMethod]
        public void IterateOverSimpleListInReverseUsingForEachIndexes() {
            String script = @"
:main
assign myList to [1, 2, 3]
for each i in count of myList[] - 1 .. 0
  Print ""myList["" + i + ""] = "" + myList[i]
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("myList[2] = 3", test.Logger[0]);
                Assert.AreEqual("myList[1] = 2", test.Logger[1]);
                Assert.AreEqual("myList[0] = 1", test.Logger[2]);
            }
        }

        [TestMethod]
        public void AssignListIndexNewValue() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""Before: "" + myList
assign myList[0] to 0
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [0,2,3,4]"));
            }
        }

        [TestMethod]
        public void AssignListIndexStringValueContainingSelectorName() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""Before: "" + myList
assign myList[0] to ""My Drill""
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [\"My Drill\",2,3,4]"));
            }
        }

        [TestMethod]
        public void AssignListIndexNewValueFromExpression() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""Before: "" + myList
assign myList[0] to myList[1] + myList[2]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [5,2,3,4]"));
            }
        }

        [TestMethod]
        public void AssignListIndexNewValueFromTernaryExpression() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""Before: "" + myList
assign myList[0] to myList[1] > 2 ? myList[2] : myList[3]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [4,2,3,4]"));
            }
        }

        [TestMethod]
        public void AssignListIndexNewValueUsingSet() {
            String script = @"
:main
set myList to [1,2,3,4]
Print ""Before: "" + myList
set myList[0] to 0
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [0,2,3,4]"));
            }
        }

        [TestMethod]
        public void AssignListKeyValue() {
            String script = @"
:main
assign myList to []
Print ""Before: "" + myList
assign myList[""key""] to ""value""
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: []"));
                Assert.IsTrue(test.Logger.Contains("After: [key->value]"));
            }
        }

        [TestMethod]
        public void CreateKeyedListWithPopulatedValues() {
            String script = @"
:main
assign myList to [""key1"" -> ""value1"", ""key2"" -> ""value2""]
Print ""Values: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Values: [key1->value1,key2->value2]"));
            }
        }

        [TestMethod]
        public void PrintKeyedListWithStringValuesIncludingSpacesAddsQuotes() {
            String script = @"
:main
assign myList to []
Print ""Before: "" + myList
assign myList[""my key""] to ""my value""
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: []"));
                Assert.IsTrue(test.Logger.Contains("After: [\"my key\"->\"my value\"]"));
            }
        }

        [TestMethod]
        public void UpdateListKeyValue() {
            String script = @"
:main
assign myList to []
assign myList[""key""] to ""value""
Print ""Before: "" + myList
assign myList[""key""] to ""newValue""
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [key->value]"));
                Assert.IsTrue(test.Logger.Contains("After: [key->newValue]"));
            }
        }

        [TestMethod]
        public void AddKeyValueToNonKeyedListAddsToEnd() {
            String script = @"
:main
assign myList to [1,2,3]
Print ""Before: "" + myList
assign myList[""key""] to ""value""
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3]"));
                Assert.IsTrue(test.Logger.Contains("After: [1,2,3,key->value]"));
            }
        }

        [TestMethod]
        public void GetItemInListByKeyedValue() {
            String script = @"
:main
assign myList to []
assign myKey to ""key""
assign myList[myKey] to ""value""
Print ""myList["" + myKey + ""] = "" + myList[myKey]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("myList[key] = value"));
            }
        }

        [TestMethod]
        public void GetNonExistentValueByKeyReturnsEmptyList() {
            String script = @"
:main
assign myList to []
assign myList[""key""] to ""value""
Print ""myList[missingKey] = "" + myList[""missingKey""]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("myList[missingKey] = []"));
            }
        }

        [TestMethod]
        public void GetSubListFromKeyedList() {
            String script = @"
:main
assign myList to []
assign myList[""key1""] to ""value1""
assign myList[""key2""] to ""value2""
assign myList[""key3""] to ""value3""
Print ""Values: "" + myList[""key1"",""key2""]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Values: [key1->value1,key2->value2]"));
            }
        }

        [TestMethod]
        public void GetListKeys() {
            String script = @"
:main
assign myList to [1,2,3]
assign myList[""key1""] to ""value""
assign myList[""key2""] to ""value""
assign myKeys to myList keys
Print ""Keys: "" + myKeys
Print ""Size of Keys: "" + count of myKeys[]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Keys: [key1,key2]"));
                Assert.IsTrue(test.Logger.Contains("Size of Keys: 2"));
            }
        }

        [TestMethod]
        public void GetListValues() {
            String script = @"
:main
assign myList to [1,2,3]
assign myList[""key1""] to ""value1""
assign myList[""key2""] to ""value2""
assign myValues to myList values
Print ""Values: "" + myValues
Print ""Size of Values: "" + count of myValues[]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Values: [1,2,3,value1,value2]"));
                Assert.IsTrue(test.Logger.Contains("Size of Values: 5"));
            }
        }

        [TestMethod]
        public void IterateOverListByKeysUsingForEach() {
            String script = @"
:main
assign myList to [1,2,3, ""key1"" -> ""value1"", ""key2"" -> ""value2""]
Print ""Keys: "" + myList keys
for each key in myList keys
  Print ""myList["" + key + ""] = "" + myList[key]
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Keys: [key1,key2]"));
                Assert.IsTrue(test.Logger.Contains("myList[key1] = value1"));
                Assert.IsTrue(test.Logger.Contains("myList[key2] = value2"));
            }
        }

        [TestMethod]
        public void IterateOverListByKeysUsingForEachIndexes() {
            String script = @"
:main
assign myList to [1,2,3, ""key1"" -> ""value1"", ""key2"" -> ""value2""]
assign myKeys to myList keys
Print ""Keys: "" + myKeys
for each i in 0..count of myKeys[] - 1
  Print ""myList["" + myKeys[i] + ""] = "" + myList[myKeys[i]]
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Keys: [key1,key2]"));
                Assert.IsTrue(test.Logger.Contains("myList[key1] = value1"));
                Assert.IsTrue(test.Logger.Contains("myList[key2] = value2"));
            }
        }

        [TestMethod]
        public void IterateOverListByKeysUsingUntil() {
            String script = @"
:main
assign myList to [1,2,3]
assign myList[""key1""] to ""value1""
assign myList[""key2""] to ""value2""
assign myKeys to myList keys
Print ""Keys: "" + myKeys
assign i to 0
until i >= count of myKeys[]
  Print ""myList["" + myKeys[i] + ""] = "" + myList[myKeys[i]]
  assign i to i + 1
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("Keys: [key1,key2]"));
                Assert.IsTrue(test.Logger.Contains("myList[key1] = value1"));
                Assert.IsTrue(test.Logger.Contains("myList[key2] = value2"));
            }
        }

        [TestMethod]
        public void AssignListSubRangeNewValue() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""Before: "" + myList
assign myList[0,1] to 0
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [0,0,3,4]"));
            }
        }

        [TestMethod]
        public void AssignEntireListNewValue() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""Before: "" + myList
assign myList[] to 0
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [0,0,0,0]"));
            }
        }

        [TestMethod]
        public void AppendToSimpleList() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""Before: "" + myList
assign myList to myList + [5]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [1,2,3,4,5]"));
            }
        }

        [TestMethod]
        public void AppendSingleStringListToSimpleList() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""Before: "" + myList
assign myList to myList + [""myThing""]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [1,2,3,4,myThing]"));
            }
        }

        [TestMethod]
        public void appendVariableToListAddsValueNotReference() {
            String script = @"
set myString to ""someString""
set myMap to[]

myMap+=[myString]
set myString to ""someOtherString""

print myMap
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("[someString]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void appendKeyedVariableToListAddsValueNotReference() {
            String script = @"
set myString to ""someString""
set myMap to[]

myMap+=[myString -> 0]
set myString to ""someOtherString""

print myMap
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.AreEqual("[someString->0]", test.Logger[0]);
            }
        }

        [TestMethod]
        public void AddTwoLists() {
            String script = @"
:main
assign myList to [1,2,3]
assign myList2 to [4,5,6]
Print ""Before: "" + myList
assign myList to myList + myList2
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3]"));
                Assert.IsTrue(test.Logger.Contains("After: [1,2,3,4,5,6]"));
            }
        }

        [TestMethod]
        public void RemoveFromListByIndexes() {
            String script = @"
:main
assign myList to [1,2,3,4,5,6]
Print ""Before: "" + myList
assign myList to myList - [0,2,4]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4,5,6]"));
                Assert.IsTrue(test.Logger.Contains("After: [2,4,6]"));
            }
        }

        [TestMethod]
        public void RemoveFromListByKey() {
            String script = @"
:main
assign myList to []
assign myList[""key1""] to ""value1""
assign myList[""key2""] to ""value2""
Print ""Before: "" + myList
assign myList to myList - [""key1""]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [key1->value1,key2->value2]"));
                Assert.IsTrue(test.Logger.Contains("After: [key2->value2]"));
            }
        }

        [TestMethod]
        public void RemoveFromBoundListByKey() {
            String script = @"
:main
assign myList to []
assign i to ""oldValue""
assign myList[""key1""] to ""value1""
bind myList[""key2""] to i
Print ""Before: "" + myList
bind subList to myList - [""key1""]
assign i to ""newValue""
Print ""After: "" + subList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [key1->value1,key2->oldValue]"));
                Assert.IsTrue(test.Logger.Contains("After: [key2->newValue]"));
            }
        }

        [TestMethod]
        public void RemoveFromListByIndexesAndKeys() {
            String script = @"
:main
assign myList to [1,2,3,4,5,6]
assign myList[""key1""] to ""value1""
assign myList[""key2""] to ""value2""
Print ""Before: "" + myList
assign myList to myList - [0,2,4,""key1"",""key3""]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4,5,6,key1->value1,key2->value2]"));
                Assert.IsTrue(test.Logger.Contains("After: [2,4,6,key2->value2]"));
            }
        }

        [TestMethod]
        public void AddTwoKeyedListsDedupesKeysAndUsesSecondValue() {
            String script = @"
:main
assign myKey to ""key""
assign myList to []
assign myList[myKey] to ""oldValue""
assign myList[""key1""] to ""value1""
assign myList2 to []
assign myList2[myKey] to ""newValue""
assign myList2[""key2""] to ""value2""
Print ""Before: "" + myList
assign myList to myList + myList2
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [key->oldValue,key1->value1]"));
                Assert.IsTrue(test.Logger.Contains("After: [key1->value1,key->newValue,key2->value2]"));
            }
        }

        [TestMethod]
        public void InsertAtBeginnngSimpleList() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""Before: "" + myList
assign myList to [5] + myList
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [5,1,2,3,4]"));
            }
        }

        [TestMethod]
        public void InsertIntoMiddleOfList() {
            String script = @"
:main
assign myList to [1,2,3,4,5]
Print ""Before: "" + myList
assign myList to myList[0 .. 2] + [6] + myList[3..4]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4,5]"));
                Assert.IsTrue(test.Logger.Contains("After: [1,2,3,6,4,5]"));
            }
        }

        [TestMethod]
        public void ReverseListUsingIndexReversal() {
            String script = @"
:main
assign myList to [1,2,3,4,5]
Print ""Before: "" + myList
assign myList to myList[4..0]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4,5]"));
                Assert.IsTrue(test.Logger.Contains("After: [5,4,3,2,1]"));
            }
        }

        [TestMethod]
        public void ReverseListUsingReverseKeyword() {
            String script = @"
:main
assign myList to [1,2,3,4,5]
Print ""Before: "" + myList
assign myList to reverse myList
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4,5]"));
                Assert.IsTrue(test.Logger.Contains("After: [5,4,3,2,1]"));
            }
        }

        [TestMethod]
        public void ReverseListUsingReversedKeyword() {
            String script = @"
:main
assign myList to [1,2,3,4,5]
Print ""Before: "" + myList
assign myList to reversed myList
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4,5]"));
                Assert.IsTrue(test.Logger.Contains("After: [5,4,3,2,1]"));
            }
        }

        [TestMethod]
        public void SortListOfNumbersUsingSortedKeyword() {
            String script = @"
:main
assign myList to [2,3,1,5,4]
Print ""Before: "" + myList
assign myList to sorted myList
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [2,3,1,5,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [1,2,3,4,5]"));
            }
        }

        [TestMethod]
        public void ReversedSortedListOfNumbersUsingSortedKeyword() {
            String script = @"
:main
assign myList to [2,3,1,5,4]
Print ""Before: "" + myList
assign myList to reversed sorted myList
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [2,3,1,5,4]"));
                Assert.IsTrue(test.Logger.Contains("After: [5,4,3,2,1]"));
            }
        }

        [TestMethod]
        public void SortListOfStringsUsingSortedKeyword() {
            String script = @"
:main
assign myList to [""one"",""two"",""three"",""four"",""five""]
Print ""Before: "" + myList
assign myList to sorted myList
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [one,two,three,four,five]"));
                Assert.IsTrue(test.Logger.Contains("After: [five,four,one,three,two]"));
            }
        }

        [TestMethod]
        public void SortListByKeys() {
            String script = @"
:main
assign myList to [""one"" -> 1, ""two"" -> 2, ""three"" -> 3]
Print ""Before: "" + myList
assign myList to myList[sorted myList keys]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [one->1,two->2,three->3]"));
                Assert.IsTrue(test.Logger.Contains("After: [one->1,three->3,two->2]"));
            }
        }

        [TestMethod]
        public void InsertIntoMiddleOfListOfVariableSize() {
            String script = @"
:main
assign myList to [1,2,3,4,5]
Print ""Before: "" + myList
assign n to count of myList[]
assign i to 1
assign myList to myList[0..i] + [6] + myList[i+1..n-1]
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4,5]"));
                Assert.IsTrue(test.Logger.Contains("After: [1,2,6,3,4,5]"));
            }
        }

        [TestMethod]
        public void AssignVariableToSubList() {
            String script = @"
:main
assign myList to [1,2,3,4]
Print ""List: "" + myList
assign mySubList to myList[2,3]
Print ""SubList: "" + mySubList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("List: [1,2,3,4]"));
                Assert.IsTrue(test.Logger.Contains("SubList: [3,4]"));
            }
        }

        [TestMethod]
        public void AssignVariableToListSetsImmutableValue() {
            String script = @"
:main
assign i to 0
assign myList to [i,1,2,3]
assign i to 1
Print ""List: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("List: [0,1,2,3]"));
            }
        }

        [TestMethod]
        public void BindVariableToListSetsMutableValue() {
            String script = @"
:main
assign i to 0
bind myList to [i,1,2,3]
Print ""Before: "" + myList
assign i to 1
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [0,1,2,3]"));
                Assert.IsTrue(test.Logger.Contains("After: [1,1,2,3]"));
            }
        }

        [TestMethod]
        public void AssignListIndexSetsImmutableValue() {
            String script = @"
:main
assign myList to [0,1,2,3]
assign i to 0
assign myList[0] to i
Print ""Before: "" + myList
assign i to 1
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [0,1,2,3]"));
                Assert.IsTrue(test.Logger.Contains("After: [0,1,2,3]"));
            }
        }

        [TestMethod]
        public void BindListIndexSetsMutableValue() {
            String script = @"
:main
assign myList to [0,1,2,3]
assign i to 0
bind myList[0] to i
Print ""Before: "" + myList
assign i to 1
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [0,1,2,3]"));
                Assert.IsTrue(test.Logger.Contains("After: [1,1,2,3]"));
            }
        }

        [TestMethod]
        public void PrintSimpleMultiDimensionalListValueAtIndex() {
            String script = @"
:main
assign myList to [[0,1],[2,3]]
Print ""myList[0][0] = "" + myList[0][0]
Print ""myList[0][1] = "" + myList[0][1]
Print ""myList[1][0] = "" + myList[1][0]
Print ""myList[1][1] = "" + myList[1][1]
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("myList[0][0] = 0"));
                Assert.IsTrue(test.Logger.Contains("myList[0][1] = 1"));
                Assert.IsTrue(test.Logger.Contains("myList[1][0] = 2"));
                Assert.IsTrue(test.Logger.Contains("myList[1][1] = 3"));
            }
        }

        [TestMethod]
        public void AssignMultiDimensionalListValueAtIndex() {
            String script = @"
:main
assign myList to [[0,1],[2,3]]
Print ""Before: ""  + myList
assign myList[1][1] to 4
Print ""After: "" + myList
";
            using (var test = new ScriptTest(script)) {
                test.RunOnce();

                Assert.IsTrue(test.Logger.Contains("Before: [[0,1],[2,3]]"));
                Assert.IsTrue(test.Logger.Contains("After: [[0,1],[2,4]]"));
            }
        }
    }
}
