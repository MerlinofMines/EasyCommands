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

        Dictionary<String, List<ItemFilter>> itemNamesToFilters = new Dictionary<string, List<ItemFilter>>();

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
            AddItems(Words("missile ammo"), Ammo("Missile200mm"));
            AddItems(Words("gatling ammo"), Ammo("NATO_25x184mm"));
            AddItems(Words("rifle ammo"), Ammo("NATO_5p56x45mm"), Ammo("AutomaticRifleGun_Mag_20rd"));
            AddItems(Words("rapid rifle ammo"), Ammo("RapidFireAutomaticRifleGun_Mag_50rd"));
            AddItems(Words("precision rifle ammo"), Ammo("PreciseAutomaticRifleGun_Mag_5rd"));
            AddItems(Words("elite rifle ammo"), Ammo("UltimateAutomaticRifleGun_Mag_30rd"));
            AddItems(Words("pistol ammo"), Ammo("SemiAutoPistolMagazine"));
            AddItems(Words("rapid pistol ammo"), Ammo("FullAutoPistolMagazine"));
            AddItems(Words("elite pistol ammo"), Ammo("ElitePistolMagazine"));

            //Components
            AddItems(Words("components"), Component());
            AddItems(Words("bulletproof glass"), Component("BulletproofGlass"));
            AddItems(Words("canvas"), Component("Canvas"));
            AddItems(Words("computer"), Component("Computer"));
            AddItems(Words("construction component"), Component("Construction"));
            AddItems(Words("detector component"), Component("Detector"));
            AddItems(Words("display"), Component("Display"));
            AddItems(Words("explosives"), Component("Explosives"));
            AddItems(Words("girder"), Component("Girder"));
            AddItems(Words("gravity component"), Component("GravityGenerator"));
            AddItems(Words("interior plate"), Component("InteriorPlate"));
            AddItems(Words("large steel tube"), Component("LargeTube"));
            AddItems(Words("medical component"), Component("Medical"));
            AddItems(Words("metal grid"), Component("MetalGrid"));
            AddItems(Words("motor"), Component("Motor"));
            AddItems(Words("power cell"), Component("PowerCell"));
            AddItems(Words("radio component"), Component("RadioCommunication"));
            AddItems(Words("reactor component"), Component("Reactor"));
            AddItems(Words("small steel tube"), Component("SmallTube"));
            AddItems(Words("solar cell"), Component("SolarCell"));
            AddItems(Words("steel plate"), Component("SteelPlate"));
            AddItems(Words("superconductor"), Component("Superconductor"));
            AddItems(Words("thruster component"), Component("Thrust"));
            AddItems(Words("zone chip"), Component("ZoneChip"));

            //HandTools
            AddItems(Words("tools"), ToolType("Grinder", "Drill", "Welder"));
            AddTools("grinder", "AngleGrinder");
            AddTools("drill", "HandDrill");
            AddTools("welder", "Welder");

            //Weapons
            AddItems(Words("weapons"), ToolType("Rifle", "Pistol", "HandHeldLauncher"));
            AddItems(Words("rifle"), Tool("AutomaticRifleItem"));
            AddItems(Words("rapid rifle"), Tool("RapidFireAutomaticRifleItem"));
            AddItems(Words("precision rifle"), Tool("PreciseAutomaticRifleItem"));
            AddItems(Words("elite rifle"), Tool("UltimateAutomaticRifleItem"));
            AddItems(Words("pistol"), Tool("SemiAutoPistolItem"));
            AddItems(Words("rapid pistol"), Tool("FullAutoPistolItem"));
            AddItems(Words("elite pistol"), Tool("ElitePistolItem"));
            AddItems(Words("rocket launcher"), Tool("BasicHandHeldLauncherItem"));
            AddItems(Words("precision rocket launcher"), Tool("AdvancedHandHeldLauncherItem"));

            //Bottles
            ItemFilter oxygenBottle = IsItemType("MyObjectBuilder_OxygenContainerObject", "OxygenBottle");
            ItemFilter hydrogenBottle = IsItemType("MyObjectBuilder_GasContainerObject", "HydrogenBottle");
            AddItems(Words("bottles"), oxygenBottle, hydrogenBottle);
            AddItems(Words("oxygen bottle"), oxygenBottle);
            AddItems(Words("hydrogen bottle"), hydrogenBottle);

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

        void AddTools(String toolWord, String itemWord) {
            AddItems(Words(toolWord), Tool(itemWord + "Item"));
            AddItems(Words("enhanced " + toolWord), Tool(itemWord + "2Item"));
            AddItems(Words("proficient " + toolWord), Tool(itemWord + "3Item"));
            AddItems(Words("elite " + toolWord), Tool(itemWord + "4Item"));
        }

        void AddItems(String[] words, params ItemFilter[] filters) {
            foreach (String word in words) itemNamesToFilters.Add(word.ToLower(), filters.ToList());
        }

        public delegate bool ItemFilter(MyInventoryItem item);

        public List<ItemFilter> GetItemFilters(String itemString) => itemString.Split(',').SelectMany(i => itemNamesToFilters.GetValueOrDefault(i.Trim().ToLower(), new List<ItemFilter>())).ToList();

        public ItemFilter Consumable(String subType = null) => IsItemType("MyObjectBuilder_ConsumableItem", subType);
        public ItemFilter Component(String subType = null) => IsItemType("MyObjectBuilder_Component", subType);
        public ItemFilter Ammo(String subType = null) => IsItemType("MyObjectBuilder_AmmoMagazine", subType);
        public ItemFilter Ingot(String subType = null) => IsItemType("MyObjectBuilder_Ingot", subType);
        public ItemFilter Ore(String subType = null) => IsItemType("MyObjectBuilder_Ore", subType);
        public ItemFilter Tool(String subType = null) => IsItemType("MyObjectBuilder_PhysicalGunObject", subType);
        public ItemFilter ToolType(params String[] matches) => (i) => i.Type.TypeId.Equals("MyObjectBuilder_PhysicalGunObject") && matches.Any(s => i.Type.SubtypeId.Contains(s));
        public ItemFilter IsItemType(String itemType, String subType = null) => (i) => i.Type.TypeId.Equals(itemType) && (subType == null || i.Type.SubtypeId.Equals(subType));

        public Func<MyInventoryItem, bool> AllItem(List<ItemFilter> filters) => b => filters.TrueForAll(f => f(b));
        public Func<MyInventoryItem, bool> AnyItem(List<ItemFilter> filters) => b => filters.Any(f => f(b));
    }
}
