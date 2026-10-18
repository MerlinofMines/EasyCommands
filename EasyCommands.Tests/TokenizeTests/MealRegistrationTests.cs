using IngameScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static EasyCommands.Tests.ParameterParsingTests.ParsingTestUtility;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.TokenizeTests {
    [TestClass]
    public class MealRegistrationTests : ForceLocale {
        Program program;

        [TestInitialize]
        public void Initialize() {
            program = CreateProgram();
            program.blueprintProvider = MockBlueprint;
            program.itemNamesToBlueprints.Clear();
            program.itemNamesToFilters.Clear();
            program.InitializeItems();
        }

        [DataTestMethod]
        [DataRow("kelp crisp", "Position0030_MealPack_KelpCrisp", "MealPack_KelpCrisp")]
        [DataRow("fruit bar", "Position0040_MealPack_FruitBar", "MealPack_FruitBar")]
        [DataRow("garden slaw", "Position0050_MealPack_GardenSlaw", "MealPack_GardenSlaw")]
        [DataRow("red pellets", "Position0060_MealPack_RedPellets", "MealPack_RedPellets")]
        [DataRow("chili", "Position0070_MealPack_Chili", "MealPack_Chili")]
        [DataRow("flatbread", "Position0080_MealPack_Flatbread", "MealPack_Flatbread")]
        [DataRow("ramen", "Position0090_MealPack_Ramen", "MealPack_Ramen")]
        [DataRow("fruit pastry", "Position0100_MealPack_FruitPastry", "MealPack_FruitPastry")]
        [DataRow("veggie burger", "Position0110_MealPack_VeggieBurger", "MealPack_VeggieBurger")]
        [DataRow("curry", "Position0120_MealPack_Curry", "MealPack_Curry")]
        [DataRow("green pellets", "Position0130_MealPack_GreenPellets", "MealPack_GreenPellets")]
        [DataRow("dumplings", "Position0140_MealPack_Dumplings", "MealPack_Dumplings")]
        [DataRow("spaghetti", "Position0150_MealPack_Spaghetti", "MealPack_Spaghetti")]
        [DataRow("lasagna", "Position0160_MealPack_Lasagna", "MealPack_Lasagna")]
        [DataRow("burrito", "Position0170_MealPack_Burrito", "MealPack_Burrito")]
        [DataRow("frontier stew", "Position0180_MealPack_FrontierStew", "MealPack_FrontierStew")]
        [DataRow("seared sabiroid", "Position0190_MealPack_SearedSabiroid", "MealPack_SearedSabiroid")]
        [DataRow("steak dinner", "Position0200_MealPack_SteakDinner", "MealPack_SteakDinner")]
        public void CraftableMealsKeepBlueprintAndItemIdentity(string alias, string blueprint, string subtype) {
            Assert.AreEqual(MockBlueprint(blueprint), program.GetItemBluePrints(alias)[0]);
            var matches = program.AnyItem(program.GetItemFilters(alias));
            Assert.IsTrue(matches(MockConsumable(subtype)));
            Assert.IsFalse(matches(MockConsumable(subtype + "_Other")));
            Assert.IsFalse(matches(MockOre(subtype)));
        }

        [DataTestMethod]
        [DataRow("unknown meal", "MealPack_Unknown")]
        [DataRow("food paste", "MealPack_FoodPaste")]
        [DataRow("synth loaf", "MealPack_SynthLoaf")]
        [DataRow("clang crunchies", "MealPack_ClangCrunchies")]
        [DataRow("banana beef", "MealPack_BananaBeef")]
        [DataRow("hardtack", "MealPack_Hardtack")]
        [DataRow("expired slop", "MealPack_ExpiredSlop")]
        public void LootOnlyMealsKeepTheirFiltersWithoutBlueprints(string alias, string subtype) {
            Assert.IsFalse(program.itemNamesToBlueprints.ContainsKey(alias));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters(alias))(MockConsumable(subtype)));
        }
    }
}
