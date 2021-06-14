using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SimpleListTests {
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
        public void IterateOverSimpleList() {
            String script = @"
:main
assign myList to [1, 2, 3]
assign i to 0
until i >= count of myList[]
  Print ""myList["" + i + ""] = "" + myList[i]
  assign i to i + 1
";
            using (var test = new ScriptTest(script)) {
                test.RunUntilDone();

                Assert.IsTrue(test.Logger.Contains("myList[0] = 1"));
                Assert.IsTrue(test.Logger.Contains("myList[1] = 2"));
                Assert.IsTrue(test.Logger.Contains("myList[2] = 3"));
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
                Assert.IsTrue(test.Logger.Contains("After: [key=value]"));
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

                Assert.IsTrue(test.Logger.Contains("Before: [key=value]"));
                Assert.IsTrue(test.Logger.Contains("After: [key=newValue]"));
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
                Assert.IsTrue(test.Logger.Contains("After: [1,2,3,key=value]"));
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

                Assert.IsTrue(test.Logger.Contains("Values: [key1=value1,key2=value2]"));
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
        public void IterateOverListByKeys() {
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

                Assert.IsTrue(test.Logger.Contains("Before: [key1=value1,key2=value2]"));
                Assert.IsTrue(test.Logger.Contains("After: [key2=value2]"));
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

                Assert.IsTrue(test.Logger.Contains("Before: [key1=value1,key2=oldValue]"));
                Assert.IsTrue(test.Logger.Contains("After: [key2=newValue]"));
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

                Assert.IsTrue(test.Logger.Contains("Before: [1,2,3,4,5,6,key1=value1,key2=value2]"));
                Assert.IsTrue(test.Logger.Contains("After: [2,4,6,key2=value2]"));
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

                Assert.IsTrue(test.Logger.Contains("Before: [key=oldValue,key1=value1]"));
                Assert.IsTrue(test.Logger.Contains("After: [key1=value1,key=newValue,key2=value2]"));
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
