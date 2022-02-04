# Collections

EasyCommands supports a basic type of collection called a "KeyedList".  Effectively, this is a list of [Variables](https://spaceengineers.merlinofmines.com/EasyCommands/variables "Variables") which may or not be Keyed Variables.  A KeyedVariable is a variable that has a Key and a Value (both Variables).

Collections allow you to store lists of items so that you can iterate over, track, queue, you name it.  Some block properties also return collections, such as "properties", "actions", and "waypoints".  

The following will describe how you can create and interact with Collections to do interesting things in your script.


## Keyed Variables

To create a keyed Variable, use the syntax ```key -> value```, where "key" and "value" are both Variables.  Keys are intended to be String identifiers which point to the corresponding values.

Keyed Variables allow you to create a Dictionary of key/value pairs, so that you can look up by keys instead of by indexes.

```
set myLookup to ["one" -> 1, "two" -> 2, "three" -> 3]

#My Value: 3
Print "My Value: " + myLookup["three"]
```

While keyes can be specified using any type of [Primitive](https://spaceengineers.merlinofmines.com/EasyCommands/primitive "Primitive Types"), be warned that lookup of keys is always done by String.  So if you specify a non-string as a key, don't expect to look up your value by that key later.

## Creating a Collection

Creating a collection is pretty easy.  A collection is denoted by "[]", with optional Variable values in between, which need to be comma separated.

```
#Empty List
set myList to []

#List of Values
set myList to [1,2,3]

#List of Keyed Values
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]
```

### The Range Operation
Sometimes you may want to quickly create a list of numbers from x to y without manually creating the list.  A common use case for this might be iterating over all the items in your list.  The Range operation, denoted by ``x .. y``, can do this for you.

```
#The really long way
set i to 0
set myList to []
until i > 10
  set myList to myList + i
  i++
Print "The really long way: "
Print "myList: " + myList

#The long way
Print "The long way: "
set myList to [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
Print "myList: " + myList

#The range operation way
Print "The quick way: "
set myList to 0..10
Print "myList: " + myList
```

The Range operation can take variable start & end points, making it useful for creating lists of arbitrary indexes.  This is especially useful for looping over a collection of unknown size:

```
set myList to 0..10

#Pretend I don't know how long myList is
set myCount to count of myList[]

#0 based, so need to subtract 1 from count
for each i in 0 .. myCount - 1
  Print "Value: " + myList[i]
```

## Retrieving Items From a List

You can retrieve values from a list either by a numeric index, or by a string key value.  The returned value will be a Variable, so you can use the result in an expresssion as well.

the general form is ```<list>[<index>]``` or ```<list>[<key>]```

### By Index

```
set myList to [0,1,2]
#myList[0]: 0
Print "myList[0]: " + myList[0]
```

Indexes are zero based, so the first element in a list is at index 0, and the last element will be at (count of myList[]) - 1.  

If you specify an index < 0, or >= the size of the collection, you will get an exception that halts execution of the script.  So..don't do that.

### By Key

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]
#myList["one"]: 1
Print 'myList["one"]: ' + myList["one"]
```

If you specify a key that is not in the given list, the result will be an empty list ([]).

## Getting All Keys or Values from a List

Given a KeyedList, sometimes you might want to get just the keys, or just the values, from the keyed list.  This is especially true if you have a lookup table and want to iterate through each key to do something to the value.

There are special operations for "keys" and "values" that you can use to do this.

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3, 4]

#Get Keys
set myKeys to myList keys

#Keys: ["one", "two", "three"]
Print "Keys: " + myKeys

#Get Values
set myValues to myList values

#Values: [1,2,3,4]
Print "Values: " + myValues
```

The "keys" and "values" keywords need to come after the intended list.  "keys" will return a collection containing all of the "keys".  It will not return an entry for items that do not have keys.  If no items in the list have keys, and empty list ([]) is returned. "values" will always return a collection of the same size as the original list, but none of the returned entries will be keyed.

## Iterating Over a List
Often times you will want to iterate over a collection.  There are a few ways to do this.

### Iterating by each value
The most direct way to iterate through a collection is to simply grab each item, give it a name, and then act on it, using the "for each" [Command](https://spaceengineers.merlinofmines.com/EasyCommands/command "Command").  The name you give to "item" will be used inside of the statement to determine what you do on each item.  Be careful of reserved keywords.

```
set myList to ["one", "two", "three"]
for each item in myList
  Print "Item: " + item
```

### Iterating by Indexes
If you want to iterate over your items using numeric indexes instead of directly grabbing the values, you can do the following:
```
set myList to ["one", "two", "three"]
for each i in 0 .. count of myList[] - 1
  Print "Item: " + myList[i]
```

The above is using the Aggregate list Variable to get the count of the items, and the range operation to create a new list of values between 0 - (count of myList[]) - 1, which represents the indexes in the given list.  This list is then used as the input to the for each command.


### Iterating over a Keyed List using the Keys
If you have a Lookup table, you might want to iterate over the list using keys instead of indexes or values.  To do this, you'll need to iterate over the lists keys:

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]

for each key in myList keys
  Print "Item: " + myList[key]
```

The above is using the "keys" keyword to get the keys for myList, and then using that as the input to the for each command.

## Updating values in an existing list

You can update the value of items in a list by key or by index.

```
set myList to [1,2,3]
set myList[0] to 0

#[0,2,3]
print myList
```

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]
set myList["one"] to 0

#[one -> 0, two -> 2, three -> 3]
print myList
```

By specifying a list of items you can actually set multiple indexes or keys to the same value in a single command

```
set myList to [1,2,3]
set myList[0..2] to 0

#[0,0,0]
print myList
```

```
set myList to [1,2,3]
set myList[] to 0

#[0,0,0]
print myList
```

If you attempt to update an index that is < 0 or greater than the size of the collection, you will get a script halting exception.

If you attempt to update an index by key for a key that does not exist in the collection, the effect is to add the item to the list instead of update.

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]
set myList["four"] to 4

#[one -> 1, two -> 2, three -> 3, four -> 4]
print myList
```

## Adding an item to an existing List
You can add items to an existing list using the "+" operation.  Any variable type can be added to a list.  

```
set myList to [1,2,3]
set myList to myList + 4

#[1,2,3,4]
Print myList
```

Note that if you add a list to a list, each item is added to the resulting list (vs adding 1 item of the entire list).  

```
set myList to [1,2,3]
set myList to myList + [4,5,6]

#[1,2,3,4,5,6]
Print myList
```

If you want to add an entire list as 1 entry, you will need to wrap it in a list:

```
set myList to [1,2,3]
set myList to myList + [[4,5,6]]

#[1,2,3,[4,5,6]]
Print myList
```

### Merging Keyed Lists

A special case for merging two lists is when you merge two keyed lists and the two lists contain an item with the same key.

In this instance, only the second entry will be kept.  Otherwise the ordering is maintained.

```
set list1 to ["one" -> 1, "two" -> 2]
set list2 to ["one" -> 4, "three" -> 3]

#["two" -> 2, "one" -> 4, "three" -> 3]
print list1 + list2
```

### Inserting at the beginning

To insert at the beginning of a list, simply switch the operand of the "+" operation.

```
set myList to [1,2,3]
set myList to 0 + myList

#[0,1,2,3]
Print myList
```


### Inserting at a specific index
Inserting an item at a specific index is a little trickier, but can be done.

```
set myList to [0,1,2,4,5]
set myList to myList[0..2] + 3 + myList[3..4]

#[0,1,2,3,4,5]
Print myList
```

For a list of unknown size, it's even more tricky but still doable:

```
set myList to [1,2,4,5]

#Pretend I don't know the size
set insertIndex to 2
set insertValue to 3

set myList to myList[0..insertIndex - 1] + insertValue + myList[insertIndex..count of myList[] - 1]

#[0,1,2,3,4,5]
Print myList
```

The above commands are taking advantage of the Range operation and SubLists, which is discussed next.

## Getting a Sub List of Items

Sometimes you might want to get a sub list of items from a collection.  You can do this the same as if you were fetching a value by index, but pass in a list instead of a single value.

```
set myList to [1,2,3,4]

set mySubList to myList[0,2]

#[1,3]
Print mySubList
```

You can also use the range operation:

```
set myList to [1,2,3,4]

set mySubList to myList[0..2]

#[1,2,3]
Print mySubList
```

Additionally, you can request multiple items by key, or specify a combination of keys and values:

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3, "four" -> 4]
set mySubList to myList["one", "two"]

#[one -> 1, two -> 2]
print mySubList

set mySubList to myList[0, "two", 3]

#[one -> 1, two -> 2, four -> 4]
print mySubList
```

Note that when you request a sub list of items which have keys, the keys are kept (as seen in above example).  This is different than when requesting a single value (in which case only the value is returned). 

## Removing Items from a List

Just like you can add items, you can remove items.  Use the "-" operation.  If the supplied value to subtract is a string, the item will be removed by key.  If the supplied value is a number, the item will be removed by index (0 based).  If the supplied value is a list, all items in the list will try to be removed either by key if strings or index if numeric.

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]

set myList to myList - 1

#[one -> 1, three -> 3]
print myList
```

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]

set myList to myList - "one"

#[two -> 2, three -> 3]
print myList
```

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]

set myList to myList - ["one", 2]

#[two -> 2]
print myList
```

If you attempt to remove an item by an index that is < 0 or greater than the size of the list, you will get a script halting exception...so don't do this.

If you attempt to remove an item by a key that isn't in the list, the resulting list is returned, unmodified.

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]
set myList to myList - "four"

#[one -> 1, two -> 2, three -> 3]
print myList
```

## Checking if a list is empty or not

You can check if a list is empty a couple ways:

```
set myList to []
if myList is empty
  Print "My List is empty!"

set myList to [1,2,3]
if myList is not empty
  Print "My List is not empty!"
```

```
set myList to []
if count of myList[] is 0
  Print "My List is empty!"

set myList to [1,2,3]
if count of myList[] > 0
  Print "My List is not empty!"
```

## Checking if two lists are equal

Lists support the comparison operation for equality, so you can determine if two lists are equal or not.  Lists are only considered equal if the key/values pairs and ordering are the same.

```
set myList to [1,2,3]

if myList is [1,2,3]
  Print "List is equal!"

if myList is not [3,2,1]
  Print "Lists are not equal, because order is important!"

if myList is not ["one" -> 1, 2, 3]
  Print "Lists are not equal, because the keys are not the same"
```

Fun fact, this is actually how "is empty" works.  It's comparing the list against a build in global variable called "empty" which is an empty list.

## Checking if a list contains a value or values

Lists support the "contains" operation, so you can check if a list contains a value

```
set myList to [1,2,3]

if myList contains 3
  Print "myList contains 3"

if myList does not contain 4
  Print "myList does not contain 4"
```

If the supplied value is a list, contains will return true if and only if the given list contains all of the specified values.  "does not contain" will return false only if the list does not contain all of the specified values.  "does not contain" will return true if a list is supplied but the given list only contains some of the requested items.

```
set myList to [1,2,3,4]

if myList contains [1,2]
  Print "myList contains 1 and 2"

if myList does not contain [2,5]
  Print "myList does not contain both 2 and 5, but might contain 2 and might contain 5"
```

You can also use Aggregate Conditions + "any" to check for contains.  Remember to use the [] so the script knows your variable is a list.

```
set myList to [1,2,3,4]
if any of myList[] is 2
  Print "myList contains 2"
```

### Checking if a list contains a key

By default, contains is checking the values of the list, not the keys.

To check if the keys contain a given string, you'll need to use the "keys" keyword

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]

if myList keys contains "one"
  Print 'myList has a key for "one"'
```








## Multi-Dimensional Lists

Since a list can contain any type of Variable, including another list, it is possible to create multi-dimensional lists by embedding lists within lists.

```
set myMatrix to [[0, 1], [1,0]]

#My Matrix: [[0,1],[1,0]]
Print "My Matrix: " + myMatrix

#My Matrix[0]: [0,1]
Print "My Matrix[0]: " + myMatrix[0]

#My Matrix[0][1]: 1
Print "My Matrix[0][1]: " + myMatrix[0][1]
```

## Reversing a List

Sometimes you might want to reverse a list.  Maybe you want to iterate the list backwards.  There are a couple ways you can reverse a list. The easiest is to use the "reversed" keyword, in front of the requested list.

```
set myList to [1,2,3]

set myReverseList to reversed myList

#[3,2,1]
print myReversedList
```

You can also use the Range operation to get a reversed list:

```
set myList to [1,2,3]

set myReversedList to myList[count of myList[] - 1 .. 0]

#[3,2,1]
print myReversedList
```

The above is using the Range operation and some clever fetching of items by index value to effectively get a new list of items using the reversed index order.


## Sorting a List

Sometimes you get an unordered list and need to order it.  You can do so using the "sorted" keyword in front of the list.  Sorted is a rather expensive operation, so try not to use it on large lists or you might get the dreaded "Script Too Complex" issue.

```
set myList to [2,3,4,1,5]

set mySortedList to sorted myList

#[1,2,3,4,5]
print mySortedList
```

When sorting, items are sorted using the comparison operation.  This means that attempting to sort a list of items that aren't comparable will result in having a bad day. Try not to sort lists of lists, booleans, etc.

### Sorting a list by keys
By default, lists are sorted by values, not by keys.

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]

set mySortedList to sorted myList

#[one -> 1, two -> 2, three -> 3]
print mySortedList
```

To sort a list by keys instead of values, you'll need to effectively create a new list that is created using the sorted list keys, and use the sorted list keys as a lookup to the original list.

```
set myList to ["one" -> 1, "two" -> 2, "three" -> 3]

set mySortedList to myList[sorted myList keys]

#[one -> 1, three -> 3, two -> 2]
print mySortedList
```

Note that this is going to strip any values from the list that aren't keyed, so keep that in mind.
