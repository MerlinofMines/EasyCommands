using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sandbox.Common.ObjectBuilders.Definitions;
using Malware.MDKUtilities;
using IngameScript;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.TokenizeTests {
    [TestClass]
    public class BlueprintParsingTests : ForceLocale {
        Program program = MDKFactory.CreateProgram<Program>();

        [TestInitialize]
        public void testInit() {
            program.blueprintProvider = MockBlueprint;
            program.itemNamesToBlueprints.Clear();
            program.itemNamesToFilters.Clear();
            program.InitializeItems();
        }

        [TestMethod]
        public void ParseAmmunition() {
            Assert.AreEqual(MockBlueprint("Missile200mm"), program.GetItemBluePrints("missile ammo")[0]);
            Assert.AreEqual(MockBlueprint("NATO_25x184mmMagazine"), program.GetItemBluePrints("gatling ammo")[0]);
            Assert.AreEqual(MockBlueprint("AutomaticRifleGun_Mag_20rd"), program.GetItemBluePrints("rifle ammo")[0]);
            Assert.AreEqual(MockBlueprint("RapidFireAutomaticRifleGun_Mag_50rd"), program.GetItemBluePrints("rapid rifle ammo")[0]);
            Assert.AreEqual(MockBlueprint("PreciseAutomaticRifleGun_Mag_5rd"), program.GetItemBluePrints("precision rifle ammo")[0]);
            Assert.AreEqual(MockBlueprint("UltimateAutomaticRifleGun_Mag_30rd"), program.GetItemBluePrints("elite rifle ammo")[0]);
            Assert.AreEqual(MockBlueprint("SemiAutoPistolMagazine"), program.GetItemBluePrints("pistol ammo")[0]);
            Assert.AreEqual(MockBlueprint("FullAutoPistolMagazine"), program.GetItemBluePrints("rapid pistol ammo")[0]);
            Assert.AreEqual(MockBlueprint("ElitePistolMagazine"), program.GetItemBluePrints("elite pistol ammo")[0]);
        }

        [TestMethod]
        public void ParseComponents() {
            Assert.AreEqual(MockBlueprint("BulletproofGlass"), program.GetItemBluePrints("bulletproof glass")[0]);
            Assert.AreEqual(MockBlueprint("Canvas"), program.GetItemBluePrints("canvas")[0]);
            Assert.AreEqual(MockBlueprint("ComputerComponent"), program.GetItemBluePrints("computer")[0]);
            Assert.AreEqual(MockBlueprint("ConstructionComponent"), program.GetItemBluePrints("construction component")[0]);
            Assert.AreEqual(MockBlueprint("DetectorComponent"), program.GetItemBluePrints("detector component")[0]);
            Assert.AreEqual(MockBlueprint("Display"), program.GetItemBluePrints("display")[0]);
            Assert.AreEqual(MockBlueprint("ExplosivesComponent"), program.GetItemBluePrints("explosives")[0]);
            Assert.AreEqual(MockBlueprint("GirderComponent"), program.GetItemBluePrints("girder")[0]);
            Assert.AreEqual(MockBlueprint("GravityGeneratorComponent"), program.GetItemBluePrints("gravity component")[0]);
            Assert.AreEqual(MockBlueprint("InteriorPlate"), program.GetItemBluePrints("interior plate")[0]);
            Assert.AreEqual(MockBlueprint("LargeTube"), program.GetItemBluePrints("large steel tube")[0]);
            Assert.AreEqual(MockBlueprint("MedicalComponent"), program.GetItemBluePrints("medical component")[0]);
            Assert.AreEqual(MockBlueprint("MetalGrid"), program.GetItemBluePrints("metal grid")[0]);
            Assert.AreEqual(MockBlueprint("MotorComponent"), program.GetItemBluePrints("motor")[0]);
            Assert.AreEqual(MockBlueprint("PowerCell"), program.GetItemBluePrints("power cell")[0]);
            Assert.AreEqual(MockBlueprint("RadioCommunicationComponent"), program.GetItemBluePrints("radio component")[0]);
            Assert.AreEqual(MockBlueprint("ReactorComponent"), program.GetItemBluePrints("reactor component")[0]);
            Assert.AreEqual(MockBlueprint("SmallTube"), program.GetItemBluePrints("small steel tube")[0]);
            Assert.AreEqual(MockBlueprint("SolarCell"), program.GetItemBluePrints("solar cell")[0]);
            Assert.AreEqual(MockBlueprint("SteelPlate"), program.GetItemBluePrints("steel plate")[0]);
            Assert.AreEqual(MockBlueprint("Superconductor"), program.GetItemBluePrints("superconductor")[0]);
            Assert.AreEqual(MockBlueprint("ThrustComponent"), program.GetItemBluePrints("thruster component")[0]);
            Assert.AreEqual(MockBlueprint("ZoneChip"), program.GetItemBluePrints("zone chip")[0]);
        }

        [TestMethod]
        public void ParseTools() {
            Assert.AreEqual(MockBlueprint("AngleGrinder"), program.GetItemBluePrints("grinder")[0]);
            Assert.AreEqual(MockBlueprint("AngleGrinder2"), program.GetItemBluePrints("enhanced grinder")[0]);
            Assert.AreEqual(MockBlueprint("AngleGrinder3"), program.GetItemBluePrints("proficient grinder")[0]);
            Assert.AreEqual(MockBlueprint("AngleGrinder4"), program.GetItemBluePrints("elite grinder")[0]);
            Assert.AreEqual(MockBlueprint("HandDrill"), program.GetItemBluePrints("drill")[0]);
            Assert.AreEqual(MockBlueprint("HandDrill2"), program.GetItemBluePrints("enhanced drill")[0]);
            Assert.AreEqual(MockBlueprint("HandDrill3"), program.GetItemBluePrints("proficient drill")[0]);
            Assert.AreEqual(MockBlueprint("HandDrill4"), program.GetItemBluePrints("elite drill")[0]);
            Assert.AreEqual(MockBlueprint("Welder"), program.GetItemBluePrints("welder")[0]);
            Assert.AreEqual(MockBlueprint("Welder2"), program.GetItemBluePrints("enhanced welder")[0]);
            Assert.AreEqual(MockBlueprint("Welder3"), program.GetItemBluePrints("proficient welder")[0]);
            Assert.AreEqual(MockBlueprint("Welder4"), program.GetItemBluePrints("elite welder")[0]);
        }

        [TestMethod]
        public void ParseWeapons() {
            Assert.AreEqual(MockBlueprint("AutomaticRifle"), program.GetItemBluePrints("rifle")[0]);
            Assert.AreEqual(MockBlueprint("RapidFireAutomaticRifle"), program.GetItemBluePrints("rapid rifle")[0]);
            Assert.AreEqual(MockBlueprint("PreciseAutomaticRifle"), program.GetItemBluePrints("precision rifle")[0]);
            Assert.AreEqual(MockBlueprint("UltimateAutomaticRifle"), program.GetItemBluePrints("elite rifle")[0]);
            Assert.AreEqual(MockBlueprint("SemiAutoPistol"), program.GetItemBluePrints("pistol")[0]);
            Assert.AreEqual(MockBlueprint("FullAutoPistol"), program.GetItemBluePrints("rapid pistol")[0]);
            Assert.AreEqual(MockBlueprint("EliteAutoPistol"), program.GetItemBluePrints("elite pistol")[0]);
            Assert.AreEqual(MockBlueprint("BasicHandHeldLauncher"), program.GetItemBluePrints("rocket launcher")[0]);
            Assert.AreEqual(MockBlueprint("AdvancedHandHeldLauncher"), program.GetItemBluePrints("precision rocket launcher")[0]);
        }

        [TestMethod]
        public void ParseBottles() {
            Assert.AreEqual(MockBlueprint("OxygenBottle"), program.GetItemBluePrints("oxygen bottle")[0]);
            Assert.AreEqual(MockBlueprint("HydrogenBottle"), program.GetItemBluePrints("hydrogen bottle")[0]);
        }

        [TestMethod]
        public void ParseDynamicTypes() {
            Assert.AreEqual(MockBlueprint("MyFakeBlueprint"), program.GetItemBluePrints("MyFakeBlueprint")[0]);
        }
    }
}
