using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sandbox.Common.ObjectBuilders.Definitions;
using Malware.MDKUtilities;
using IngameScript;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.TokenParsingTests {
    [TestClass]
    public class ItemParsingTests {
        Program program = MDKFactory.CreateProgram<Program>();

        [TestMethod]
        public void ParseOres() {
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("cobalt ore")).Invoke(MockOre("Cobalt")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("nickel ore")).Invoke(MockOre("Nickel")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("gold ore")).Invoke(MockOre("Gold")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ice")).Invoke(MockOre("Ice")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("iron ore")).Invoke(MockOre("Iron")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("magnesium ore")).Invoke(MockOre("Magnesium")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("platinum ore")).Invoke(MockOre("Platinum")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("silicon ore")).Invoke(MockOre("Silicon")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("silver ore")).Invoke(MockOre("Silver")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("uranium ore")).Invoke(MockOre("Uranium")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("stone")).Invoke(MockOre("Stone")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("scrap metal")).Invoke(MockOre("Scrap")));

            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Cobalt")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Nickel")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Gold")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Ice")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Iron")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Magnesium")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Platinum")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Silicon")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Silver")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Uranium")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Stone")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ore")).Invoke(MockOre("Scrap")));
        }

        [TestMethod]
        public void ParseIngots() {
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("cobalt ingot")).Invoke(MockIngot("Cobalt")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("nickel ingot")).Invoke(MockIngot("Nickel")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("gold ingot")).Invoke(MockIngot("Gold")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("iron ingot")).Invoke(MockIngot("Iron")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("magnesium ingot")).Invoke(MockIngot("Magnesium")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("magnesium powder")).Invoke(MockIngot("Magnesium")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("platinum ingot")).Invoke(MockIngot("Platinum")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("silicon ingot")).Invoke(MockIngot("Silicon")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("silicon wafer")).Invoke(MockIngot("Silicon")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("silver ingot")).Invoke(MockIngot("Silver")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("uranium ingot")).Invoke(MockIngot("Uranium")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("gravel")).Invoke(MockIngot("Stone")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("old scrap metal")).Invoke(MockIngot("Scrap")));

            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Cobalt")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Nickel")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Gold")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Iron")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Magnesium")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Platinum")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Silicon")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Silver")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Uranium")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Stone")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ingots")).Invoke(MockIngot("Scrap")));
        }

        [TestMethod]
        public void ParseAmmunition() {
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("missile ammo")).Invoke(MockAmmo("Missile200mm")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("gatling ammo")).Invoke(MockAmmo("NATO_25x184mm")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("rifle ammo")).Invoke(MockAmmo("NATO_5p56x45mm")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("rifle ammo")).Invoke(MockAmmo("AutomaticRifleGun_Mag_20rd")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("rapid rifle ammo")).Invoke(MockAmmo("RapidFireAutomaticRifleGun_Mag_50rd")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("precision rifle ammo")).Invoke(MockAmmo("PreciseAutomaticRifleGun_Mag_5rd")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("elite rifle ammo")).Invoke(MockAmmo("UltimateAutomaticRifleGun_Mag_30rd")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("pistol ammo")).Invoke(MockAmmo("SemiAutoPistolMagazine")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("rapid pistol ammo")).Invoke(MockAmmo("FullAutoPistolMagazine")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("elite pistol ammo")).Invoke(MockAmmo("ElitePistolMagazine")));

            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("Missile200mm")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("NATO_25x184mm")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("NATO_5p56x45mm")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("AutomaticRifleGun_Mag_20rd")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("RapidFireAutomaticRifleGun_Mag_50rd")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("PreciseAutomaticRifleGun_Mag_5rd")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("UltimateAutomaticRifleGun_Mag_30rd")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("SemiAutoPistolMagazine")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("FullAutoPistolMagazine")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("ammo")).Invoke(MockAmmo("ElitePistolMagazine")));
        }

        [TestMethod]
        public void ParseComponents() {
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("bulletproof glass")).Invoke(MockComponent("BulletproofGlass")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("canvas")).Invoke(MockComponent("Canvas")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("computer")).Invoke(MockComponent("Computer")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("construction component")).Invoke(MockComponent("Construction")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("detector component")).Invoke(MockComponent("Detector")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("display")).Invoke(MockComponent("Display")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("explosives")).Invoke(MockComponent("Explosives")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("girder")).Invoke(MockComponent("Girder")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("gravity component")).Invoke(MockComponent("GravityGenerator")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("interior plate")).Invoke(MockComponent("InteriorPlate")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("large steel tube")).Invoke(MockComponent("LargeTube")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("medical component")).Invoke(MockComponent("Medical")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("metal grid")).Invoke(MockComponent("MetalGrid")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("motor")).Invoke(MockComponent("Motor")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("power cell")).Invoke(MockComponent("PowerCell")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("radio component")).Invoke(MockComponent("RadioCommunication")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("reactor component")).Invoke(MockComponent("Reactor")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("small steel tube")).Invoke(MockComponent("SmallTube")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("solar cell")).Invoke(MockComponent("SolarCell")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("steel plate")).Invoke(MockComponent("SteelPlate")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("superconductor")).Invoke(MockComponent("Superconductor")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("thruster component")).Invoke(MockComponent("Thrust")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("zone chip")).Invoke(MockComponent("ZoneChip")));

            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("BulletproofGlass")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Canvas")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Computer")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Construction")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Detector")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Display")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Explosives")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Girder")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("GravityGenerator")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("InteriorPlate")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("LargeTube")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Medical")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("MetalGrid")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Motor")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("PowerCell")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("RadioCommunication")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Reactor")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("SmallTube")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("SolarCell")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("SteelPlate")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Superconductor")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("Thrust")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("components")).Invoke(MockComponent("ZoneChip")));

        }

        [TestMethod]
        public void ParseTools() {
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("grinder")).Invoke(MockTool("AngleGrinderItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("enhanced grinder")).Invoke(MockTool("AngleGrinder2Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("proficient grinder")).Invoke(MockTool("AngleGrinder3Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("elite grinder")).Invoke(MockTool("AngleGrinder4Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("drill")).Invoke(MockTool("HandDrillItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("enhanced drill")).Invoke(MockTool("HandDrill2Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("proficient drill")).Invoke(MockTool("HandDrill3Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("elite drill")).Invoke(MockTool("HandDrill4Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("welder")).Invoke(MockTool("WelderItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("enhanced welder")).Invoke(MockTool("Welder2Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("proficient welder")).Invoke(MockTool("Welder3Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("elite welder")).Invoke(MockTool("Welder4Item")));

            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("AngleGrinderItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("AngleGrinder2Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("AngleGrinder3Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("AngleGrinder4Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("HandDrillItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("HandDrill2Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("HandDrill3Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("HandDrill4Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("WelderItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("Welder2Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("Welder3Item")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("tools")).Invoke(MockTool("Welder4Item")));
        }

        [TestMethod]
        public void ParseWeapons() {
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("rifle")).Invoke(MockTool("AutomaticRifleItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("rapid rifle")).Invoke(MockTool("RapidFireAutomaticRifleItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("precision rifle")).Invoke(MockTool("PreciseAutomaticRifleItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("elite rifle")).Invoke(MockTool("UltimateAutomaticRifleItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("pistol")).Invoke(MockTool("SemiAutoPistolItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("rapid pistol")).Invoke(MockTool("FullAutoPistolItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("elite pistol")).Invoke(MockTool("ElitePistolItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("rocket launcher")).Invoke(MockTool("BasicHandHeldLauncherItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("precision rocket launcher")).Invoke(MockTool("AdvancedHandHeldLauncherItem")));

            Assert.IsTrue(program.AnyItem(program.GetItemFilters("weapons")).Invoke(MockTool("AutomaticRifleItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("weapons")).Invoke(MockTool("RapidFireAutomaticRifleItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("weapons")).Invoke(MockTool("PreciseAutomaticRifleItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("weapons")).Invoke(MockTool("UltimateAutomaticRifleItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("weapons")).Invoke(MockTool("SemiAutoPistolItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("weapons")).Invoke(MockTool("FullAutoPistolItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("weapons")).Invoke(MockTool("ElitePistolItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("weapons")).Invoke(MockTool("BasicHandHeldLauncherItem")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("weapons")).Invoke(MockTool("AdvancedHandHeldLauncherItem")));
        }

        [TestMethod]
        public void ParseBottles() {
            var oxygenBottle = MockItem(typeof(MyObjectBuilder_OxygenContainerObject), "OxygenBottle");
            var hydrogenBottle = MockItem(typeof(MyObjectBuilder_GasContainerObject), "HydrogenBottle");

            Assert.IsTrue(program.AnyItem(program.GetItemFilters("oxygen bottle")).Invoke(oxygenBottle));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("bottles")).Invoke(oxygenBottle));

            Assert.IsTrue(program.AnyItem(program.GetItemFilters("hydrogen bottle")).Invoke(hydrogenBottle));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("bottles")).Invoke(hydrogenBottle));
        }

        [TestMethod]
        public void ParseConsumables() {
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("clang cola")).Invoke(MockConsumable("ClangCola")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("cosmic coffee")).Invoke(MockConsumable("CosmicCoffee")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("medkit")).Invoke(MockConsumable("Medkit")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("powerkit")).Invoke(MockConsumable("Powerkit")));

            Assert.IsTrue(program.AnyItem(program.GetItemFilters("consumables")).Invoke(MockConsumable("ClangCola")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("consumables")).Invoke(MockConsumable("CosmicCoffee")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("consumables")).Invoke(MockConsumable("Medkit")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("consumables")).Invoke(MockConsumable("Powerkit")));
        }

        [TestMethod]
        public void ParseMisc() {
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("space credit")).Invoke(MockPhysicalObject("SpaceCredit")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("package")).Invoke(MockItem(typeof(MyObjectBuilder_Package), "Package")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("datapad")).Invoke(MockItem(typeof(MyObjectBuilder_Datapad), "Datapad")));
        }

        [TestMethod]
        public void ParseMultipleTypes() {
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("Iron Ore, Iron Ingot")).Invoke(MockOre("Iron")));
            Assert.IsTrue(program.AnyItem(program.GetItemFilters("Iron ore, Iron Ingot")).Invoke(MockIngot("Iron")));
        }
    }
}
