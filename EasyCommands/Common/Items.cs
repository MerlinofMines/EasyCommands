using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {

        public Dictionary<String, List<ItemFilter>> itemNamesToFilters = NewDictionary<string, List<ItemFilter>>();
        public Dictionary<String, MyDefinitionId> itemNamesToBlueprints = NewDictionary<string, MyDefinitionId>();
        public Func<String, MyDefinitionId> blueprintProvider = GetBlueprint;

        public void InitializeItems() {
            //Ores
            AddItems(Words("ore"), Ore());
            AddOre("Cobalt");
            AddOre("Nickel");
            AddOre("Gold");
            AddItems(Words("ice"), Ore("Ice"));
            AddOre("Iron");
            AddOre("Magnesium");
            AddOre("Platinum");
            AddOre("Silicon");
            AddOre("Silver");
            AddOre("Uranium");
            AddItems(Words("stone"), Ore("Stone"));
            AddItems(Words("scrap metal"), Ore("Scrap"));

            //Ingots
            AddItems(Words("ingots"), Ingot());
            AddIngot("Cobalt");
            AddIngot("Nickel");
            AddIngot("Gold");
            AddItems(Words("gravel"), Ingot("Stone"));
            AddIngot("Iron");
            AddItems(Words("magnesium ingot", "magnesium powder"), Ingot("Magnesium"));
            AddIngot("Platinum");
            AddItems(Words("silicon ingot", "silicon wafer"), Ingot("Silicon"));
            AddIngot("Silver");
            AddIngot("Uranium");
            AddItems(Words("old scrap metal"), Ingot("Scrap"));

            //Ammo
            AddItems(Words("ammo", "ammunition"), Ammo());
            AddAmmo(Words("missile ammo"), "Missile200mm");
            AddBlueprintItems(Words("gatling ammo"), "NATO_25x184mmMagazine", Ammo("NATO_25x184mm"));
            AddBlueprintItems(Words("rifle ammo"), "AutomaticRifleGun_Mag_20rd", Ammo("NATO_5p56x45mm"), Ammo("AutomaticRifleGun_Mag_20rd"));
            AddAmmo(Words("rapid rifle ammo"), "RapidFireAutomaticRifleGun_Mag_50rd");
            AddAmmo(Words("precision rifle ammo"), "PreciseAutomaticRifleGun_Mag_5rd");
            AddAmmo(Words("elite rifle ammo"), "UltimateAutomaticRifleGun_Mag_30rd");
            AddAmmo(Words("pistol ammo"), "SemiAutoPistolMagazine");
            AddAmmo(Words("rapid pistol ammo"), "FullAutoPistolMagazine");
            AddAmmo(Words("elite pistol ammo"), "ElitePistolMagazine");

            //Components
            AddItems(Words("components"), Component());
            AddComponent(Words("bulletproof glass"), "BulletproofGlass", "");
            AddComponent(Words("canvas"), "Canvas", "");
            AddComponent(Words("computer"), "Computer");
            AddComponent(Words("construction component"), "Construction");
            AddComponent(Words("detector component"), "Detector");
            AddComponent(Words("display"), "Display", "");
            AddComponent(Words("explosives"), "Explosives");
            AddComponent(Words("girder"), "Girder");
            AddComponent(Words("gravity component"), "GravityGenerator");
            AddComponent(Words("interior plate"), "InteriorPlate", "");
            AddComponent(Words("large steel tube"), "LargeTube", "");
            AddComponent(Words("medical component"), "Medical");
            AddComponent(Words("metal grid"), "MetalGrid", "");
            AddComponent(Words("motor"), "Motor");
            AddComponent(Words("power cell"), "PowerCell", "");
            AddComponent(Words("radio component"), "RadioCommunication");
            AddComponent(Words("reactor component"), "Reactor");
            AddComponent(Words("small steel tube"), "SmallTube", "");
            AddComponent(Words("solar cell"), "SolarCell", "");
            AddComponent(Words("steel plate"), "SteelPlate", "");
            AddComponent(Words("superconductor"), "Superconductor", "");
            AddComponent(Words("thruster component"), "Thrust");
            AddComponent(Words("zone chip"), "ZoneChip", "");

            //HandTools
            AddItems(Words("tools"), ToolType("Grinder", "Drill", "Welder"));
            AddTools("grinder", "AngleGrinder");
            AddTools("drill", "HandDrill");
            AddTools("welder", "Welder");

            //Weapons
            AddItems(Words("weapons"), ToolType("Rifle", "Pistol", "HandHeldLauncher"));
            AddWeapon(Words("rifle"), "AutomaticRifle");
            AddWeapon(Words("rapid rifle"), "RapidFireAutomaticRifle");
            AddWeapon(Words("precision rifle"), "PreciseAutomaticRifle");
            AddWeapon(Words("elite rifle"), "UltimateAutomaticRifle");
            AddWeapon(Words("pistol"), "SemiAutoPistol");
            AddWeapon(Words("rapid pistol"), "FullAutoPistol");
            AddBlueprintItems(Words("elite pistol"), "EliteAutoPistol", Tool("ElitePistolItem"));
            AddWeapon(Words("rocket launcher"), "BasicHandHeldLauncher");
            AddWeapon(Words("precision rocket launcher"), "AdvancedHandHeldLauncher");

            //Bottles
            ItemFilter oxygenBottle = IsItemType("MyObjectBuilder_OxygenContainerObject", "OxygenBottle");
            ItemFilter hydrogenBottle = IsItemType("MyObjectBuilder_GasContainerObject", "HydrogenBottle");
            AddItems(Words("bottles"), oxygenBottle, hydrogenBottle);
            AddBlueprintItems(Words("oxygen bottle"), "OxygenBottle", oxygenBottle);
            AddBlueprintItems(Words("hydrogen bottle"), "HydrogenBottle", hydrogenBottle);

            //Consumables
            AddItems(Words("consumables"), Consumable());
            AddItems(Words("clang cola"), Consumable("ClangCola"));
            AddItems(Words("cosmic coffee"), Consumable("CosmicCoffee"));
            AddItems(Words("medkit"), Consumable("Medkit"));
            AddItems(Words("powerkit"), Consumable("Powerkit"));

            //Misc
            AddItems(Words("package"), IsItemType("MyObjectBuilder_Package", "Package"));
            AddItems(Words("datapad"), IsItemType("MyObjectBuilder_Datapad", "Datapad"));
            AddItems(Words("space credit"), IsItemType("MyObjectBuilder_PhysicalObject", "SpaceCredit"));
        }

        void AddOre(String oreWord) => AddItems(Words(oreWord.ToLower() + " ore"), Ore(oreWord));

        void AddIngot(String ingotWord) => AddItems(Words(ingotWord.ToLower() + " ingot"), Ingot(ingotWord));

        void AddAmmo(String[] words, String ammoWord) => AddBlueprintItems(words, ammoWord, Ammo(ammoWord));

        void AddWeapon(String[] words, String weaponWord) => AddBlueprintItems(words, weaponWord, Tool(weaponWord + "Item"));

        void AddComponent(String[] words, String componentWord, String blueprintSuffix = "Component") => AddBlueprintItems(words, componentWord + blueprintSuffix, Component(componentWord));

        void AddTools(String toolWord, String itemWord) {
            AddBlueprintItems(Words(toolWord), itemWord, Tool(itemWord + "Item"));
            AddBlueprintItems(Words("enhanced " + toolWord), itemWord + "2", Tool(itemWord + "2Item"));
            AddBlueprintItems(Words("proficient " + toolWord), itemWord + "3", Tool(itemWord + "3Item"));
            AddBlueprintItems(Words("elite " + toolWord), itemWord + "4", Tool(itemWord + "4Item"));
        }

        void AddBlueprintItems(String[] words, String blueprintId, params ItemFilter[] filters) {
            AddItems(words, filters);
            AddBluePrint(words, blueprintId);
        }

        void AddBluePrint(string[] words, string blueprintId) {
            var blueprint = blueprintProvider(blueprintId);
            foreach (String word in words) itemNamesToBlueprints.Add(word, blueprint);
        }

        void AddItems(String[] words, params ItemFilter[] filters) {
            foreach (String word in words) itemNamesToFilters.Add(word.ToLower(), filters.ToList());
        }

        public static MyDefinitionId GetBlueprint(string blueprintId) {
            MyDefinitionId definition;
            MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition", blueprintId, out definition);
            return definition;
        }

        public delegate bool ItemFilter(MyInventoryItem item);

        public List<ItemFilter> GetItemFilters(String itemString) => GetItemsFromString(itemString, itemNamesToFilters, i => NewList(DynamicItemType(i.Split('.')))).SelectMany(x => x).ToList();
        public List<MyDefinitionId> GetItemBluePrints(String itemString) => GetItemsFromString(itemString, itemNamesToBlueprints, blueprintProvider);
        List<T> GetItemsFromString<T>(string itemString, Dictionary<string, T> values, Func<string, T> dynamicValue) => itemString.Split(',').Select(i => values.GetValueOrDefault(i.Trim().ToLower(), dynamicValue(i))).ToList();

        public ItemFilter Consumable(String subType = null) => IsItemType("MyObjectBuilder_ConsumableItem", subType);
        public ItemFilter Component(String subType = null) => IsItemType("MyObjectBuilder_Component", subType);
        public ItemFilter Ammo(String subType = null) => IsItemType("MyObjectBuilder_AmmoMagazine", subType);
        public ItemFilter Ingot(String subType = null) => IsItemType("MyObjectBuilder_Ingot", subType);
        public ItemFilter Ore(String subType = null) => IsItemType("MyObjectBuilder_Ore", subType);
        public ItemFilter Tool(String subType = null) => IsItemType("MyObjectBuilder_PhysicalGunObject", subType);
        public ItemFilter ToolType(params String[] matches) => (i) => i.Type.TypeId.Equals("MyObjectBuilder_PhysicalGunObject") && matches.Any(s => i.Type.SubtypeId.Contains(s));
        public ItemFilter IsItemType(String itemType, String subType = null) => (i) => (string.IsNullOrEmpty(itemType) || i.Type.TypeId.Equals(itemType)) && (string.IsNullOrEmpty(subType) || i.Type.SubtypeId.Equals(subType));

        public ItemFilter DynamicItemType(string[] itemTypeSplit) => itemTypeSplit.Count() == 2 ? IsItemType(itemTypeSplit[0], itemTypeSplit[1]) : IsItemType("", itemTypeSplit[0]);

        public Func<MyInventoryItem, bool> AllItem(List<ItemFilter> filters) => b => filters.All(f => f(b));
        public Func<MyInventoryItem, bool> AnyItem(List<ItemFilter> filters) => b => filters.Any(f => f(b));
    }
}
