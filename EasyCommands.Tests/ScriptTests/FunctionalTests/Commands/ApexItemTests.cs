using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRage;
using VRage.Game.ModAPI.Ingame;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class ApexItemTests {
        static MyInventoryItem Item(string type, string subtype, int amount = 1) =>
            MockItem(type == "SeedItem" ? typeof(Sandbox.Common.ObjectBuilders.MyObjectBuilder_SeedItem) : type == "PhysicalObject" ? typeof(VRage.Game.MyObjectBuilder_PhysicalObject) : typeof(VRage.Game.MyObjectBuilder_ConsumableItem), subtype, amount);

        [DataTestMethod]
        [DataRow("algae", "PhysicalObject", "Algae")]
        [DataRow("grain", "PhysicalObject", "Grain")]
        [DataRow("fruit", "ConsumableItem", "Fruit")]
        [DataRow("mushrooms", "ConsumableItem", "Mushrooms")]
        [DataRow("vegetables", "ConsumableItem", "Vegetables")]
        [DataRow("raw mammal meat", "ConsumableItem", "MammalMeatRaw")]
        [DataRow("raw insect meat", "ConsumableItem", "InsectMeatRaw")]
        [DataRow("fruit seeds", "SeedItem", "Fruit")]
        [DataRow("grain seeds", "SeedItem", "Grain")]
        [DataRow("vegetable seeds", "SeedItem", "Vegetables")]
        [DataRow("mushroom spores", "SeedItem", "Mushrooms")]
        [DataRow("anti-radiation medkit", "ConsumableItem", "RadiationKit")]
        public void AliasesMatchExactItemTypes(string alias, string type, string subtype) {
            using (var test = new ScriptTest("")) {
                var matches = test.program.AnyItem(test.program.GetItemFilters(alias));
                Assert.IsTrue(matches(Item(type, subtype)));
                Assert.IsFalse(matches(Item(type == "SeedItem" ? "ConsumableItem" : "SeedItem", subtype)));
                Assert.IsFalse(matches(Item(type, subtype + "Modded")));
            }
        }

        [DataTestMethod]
        [DataRow("kelp crisp", "MealPack_KelpCrisp", "Position0030_MealPack_KelpCrisp")]
        [DataRow("ramen", "MealPack_Ramen", "Position0090_MealPack_Ramen")]
        [DataRow("flatbread", "MealPack_Flatbread", "Position0080_MealPack_Flatbread")]
        [DataRow("cooked insect meat", "InsectMeatCooked", "Position0020_CookSpiderMeat")]
        [DataRow("medkit", "Medkit", "Position0021_Medkit")]
        [DataRow("powerkit", "Powerkit", "Position0022_Powerkit")]
        public void RecipesUseBlueprintIdsRatherThanItemIds(string alias, string subtype, string blueprint) {
            using (var test = new ScriptTest("")) {
                Assert.AreEqual(MockBlueprint(blueprint), test.program.GetItemBluePrints(alias)[0]);
                Assert.IsTrue(test.program.AnyItem(test.program.GetItemFilters(alias))(Item("ConsumableItem", subtype)));
            }
        }

        [TestMethod]
        public void CategoriesAndRawIdsRemainExtensible() {
            using (var test = new ScriptTest("")) {
                var food = test.program.AnyItem(test.program.GetItemFilters("food"));
                Assert.IsTrue(food(Item("PhysicalObject", "Algae")));
                Assert.IsTrue(food(Item("PhysicalObject", "Grain")));
                Assert.IsTrue(food(Item("ConsumableItem", "MealPack_Custom")));
                Assert.IsFalse(food(Item("SeedItem", "Fruit")));
                Assert.IsFalse(food(Item("ConsumableItem", "Medkit")));
                Assert.IsFalse(food(Item("ConsumableItem", "ClangCola")));
                Assert.IsTrue(test.program.AnyItem(test.program.GetItemFilters("seeds"))(Item("SeedItem", "ModdedSeed")));
                Assert.IsTrue(test.program.AnyItem(test.program.GetItemFilters("MyObjectBuilder_SeedItem.ModdedSeed"))(Item("SeedItem", "ModdedSeed")));
                Assert.IsTrue(test.program.AnyItem(test.program.GetItemFilters("consumables"))(Item("ConsumableItem", "Medkit")));
                Assert.IsFalse(test.program.itemNamesToBlueprints.ContainsKey("anti-radiation medkit"));
                Assert.IsFalse(test.program.itemNamesToBlueprints.ContainsKey("food paste"));
            }
        }

        [TestMethod]
        public void SeedCountingAndTransfersUseExistingInventoryCommands() {
            using (var test = new ScriptTest(@"print ""source cargo"" amount ""fruit seeds""
transfer 2 ""fruit seeds"" from ""source cargo"" to ""destination cargo""")) {
                var source = new Mock<IMyCargoContainer>();
                var destination = new Mock<IMyCargoContainer>();
                var from = new Mock<IMyInventory>();
                var to = new Mock<IMyInventory>();
                var seed = Item("SeedItem", "Fruit", 5);
                MockInventories(source, from);
                MockInventories(destination, to);
                MockInventoryItems(from, seed, Item("ConsumableItem", "Fruit", 99));
                var initialMass = from.Object.CurrentMass;
                from.Setup(i => i.TransferItemTo(to.Object, seed, (MyFixedPoint?)2))
                    .Callback(() => from.Setup(i => i.CurrentMass).Returns(initialMass - (MyFixedPoint)2))
                    .Returns(true);
                test.MockBlocksOfType("source cargo", source);
                test.MockBlocksOfType("destination cargo", destination);
                test.RunUntilDone();
                Assert.AreEqual("5", test.Logger[0]);
                from.Verify(i => i.TransferItemTo(to.Object, seed, (MyFixedPoint)2));
            }
        }
    }
}
