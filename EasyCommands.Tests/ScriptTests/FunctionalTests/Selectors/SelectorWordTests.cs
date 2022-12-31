using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IngameScript;
using Moq;
using Sandbox.ModAPI.Ingame;
using VRageMath;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using VRage.Game.ModAPI.Ingame;
using static EasyCommands.Tests.ScriptTests.MockEntityUtility;

namespace EasyCommands.Tests.ScriptTests {
    [TestClass]
    public class SelectorWordTests {
        [TestMethod]
        public void AirventSelector() {
            TestExpectedSelectorWords<IMyAirVent>(
                Words("airvent", "vent"),
                Words("airvents", "vents")
            );
        }

        [TestMethod]
        public void AntennaSelector() {
            TestExpectedSelectorWords<IMyRadioAntenna>(
                Words("antenna"),
                Words("antennas")
            );
        }

        [TestMethod]
        public void AssemblerSelector() {
            TestExpectedSelectorWords<IMyAssembler>(
                Words("assembler"),
                Words("assemblers")
            );
        }

        [TestMethod]
        public void BatterySelector() {
            TestExpectedSelectorWords<IMyBatteryBlock>(
                Words("battery"),
                Words("batteries")
            );
        }

        [TestMethod]
        public void BeaconSelector() {
            TestExpectedSelectorWords<IMyBeacon>(
                Words("beacon"),
                Words("beacons")
            );
        }

        [TestMethod]
        public void CameraSelector() {
            TestExpectedSelectorWords<IMyCameraBlock>(
                Words("camera"),
                Words("cameras")
            );
        }

        [TestMethod]
        public void CockpitSelector() {
            TestExpectedSelectorWords<IMyCockpit>(
                Words("cockpit", "ship", "rover", "seat", "station", "helm"),
                Words("cockpits", "ships", "rovers", "seats", "stations", "helms")
            );
        }

        [TestMethod]
        public void CollectorSelector() {
            TestExpectedSelectorWords<IMyCollector>(
                Words("collector"),
                Words("collectors")
            );
        }

        [TestMethod]
        public void ConnectorSelector() {
            TestExpectedSelectorWords<IMyShipConnector>(
                Words("connector"),
                Words("connectors")
            );
        }

        [TestMethod]
        public void CryoChamberSelector() {
            TestExpectedSelectorWords<IMyCryoChamber>(
                Words("cryo", "cryo chamber"),
                Words("cryos", "cryo chambers")
            );
        }

        [TestMethod]
        public void DecoySelector() {
            TestExpectedSelectorWords<IMyDecoy>(
                Words("decoy"),
                Words("decoys")
            );
        }

        [TestMethod]
        public void DisplaySelector() {
            TestExpectedSelectorWords<IMyTextPanel>(
                Words("display", "displays", "screen", "screens", "lcd", "lcds"),
                Words("display", "displays", "screen", "screens", "lcd", "lcds")
            );
        }

        [TestMethod]
        public void DoorSelector() {
            TestExpectedSelectorWords<IMyDoor>(
                Words("door", "hangar", "bay", "gate"),
                Words("doors", "hangars", "bays", "gates")
            );
        }

        [TestMethod]
        public void DrillSelector() {
            TestExpectedSelectorWords<IMyShipDrill>(
                Words("drill"),
                Words("drills")
            );
        }

        [TestMethod]
        public void EjectorSelector() {
            TestExpectedSelectorWords<IMyShipConnector>(
                Words("ejector"),
                Words("ejectors")
            );
        }

        [TestMethod]
        public void GeneratorSelector() {
            TestExpectedSelectorWords<IMyGasGenerator>(
                Words("generator"),
                Words("generators")
            );
        }

        [TestMethod]
        public void GravityGeneratorSelector() {
            TestExpectedSelectorWords<IMyGravityGenerator>(
                Words("gravitygenerator"),
                Words("gravitygenerators")
            );
        }

        [TestMethod]
        public void GravitySphereSelector() {
            TestExpectedSelectorWords<IMyGravityGeneratorSphere>(
                Words("gravitysphere"),
                Words("gravityspheres")
            );
        }

        [TestMethod]
        public void GridSelector() {
            TestExpectedSelectorWords<IMyFunctionalBlock>(
                Words("grid", "grids"),
                Words("grid", "grids"),
                block => MockCubeGrid(block, new Mock<IMyCubeGrid>())
            );
        }

        [TestMethod]
        public void GrinderSelector() {
            TestExpectedSelectorWords<IMyShipGrinder>(
                Words("grinder"),
                Words("grinders")
            );
        }

        [TestMethod]
        public void GunSelector() {
            TestExpectedSelectorWords<IMyUserControllableGun>(
                Words("gun", "railgun", "cannon", "autocannon", "rocket", "rocket launcher", "missile", "missile launcher"),
                Words("guns", "railguns", "cannons", "autocannons", "rockets", "rocket launchers", "missiles", "missile launchers")
            );
        }

        [TestMethod]
        public void GyroscopeSelector() {
            TestExpectedSelectorWords<IMyGyro>(
                Words("gyro", "gyroscope"),
                Words("gyros", "gyroscopes")
            );
        }

        [TestMethod]
        public void HeatVentSelector() {
            TestExpectedSelectorWords<IMyHeatVent>(
                Words("heatvent"),
                Words("heatvents")
            );
        }

        [TestMethod]
        public void HingeSelector() {
            TestExpectedSelectorWords<IMyMotorStator>(
                Words("hinge"),
                Words("hinges"),
                block => MockBlockDefinition(block, "LargeHinge")
            );
        }

        [TestMethod]
        public void HydrogenEngineSelector() {
            TestExpectedSelectorWords<IMyPowerProducer>(
                Words("engine"),
                Words("engines"),
                block => MockBlockDefinition(block, "HydrogenEngine")
            );
        }

        [TestMethod]
        public void InventorySelector() {
            TestExpectedSelectorWords<IMyCargoContainer>(
                Words("container", "cargo", "inventory", "inventories"),
                Words("containers", "cargo", "cargos", "inventory", "inventories"),
                block => MockInventories(block, new Mock<IMyInventory>())
            );
        }

        [TestMethod]
        public void JumpDriveSelector() {
            TestExpectedSelectorWords<IMyJumpDrive>(
                Words("jump drive", "jumpdrive"),
                Words("jump drives", "jumpdrives")
            );
        }

        [TestMethod]
        public void LandingGearSelector() {
            TestExpectedSelectorWords<IMyLandingGear>(
                Words("gear", "landing gear", "magnet"),
                Words("magnets", "gears", "landing gear", "landing gears")
            );
        }

        [TestMethod]
        public void LaserAntennaSelector() {
            TestExpectedSelectorWords<IMyLaserAntenna>(
                Words("laser", "laserantenna"),
                Words("lasers", "laserantennas")
            );
        }

        [TestMethod]
        public void LightSelector() {
            TestExpectedSelectorWords<IMyLightingBlock>(
                Words("light", "spotlight"),
                Words("lights", "spotlights")
            );
        }

        [TestMethod]
        public void MergeBlockSelector() {
            TestExpectedSelectorWords<IMyShipMergeBlock>(
                Words("merge", "merge block"),
                Words("merge blocks")
            );
        }

        [TestMethod]
        public void OreDetectorSelector() {
            TestExpectedSelectorWords<IMyOreDetector>(
                Words("detector", "ore detector"),
                Words("detectors", "ore detectors")
            );
        }

        [TestMethod]
        public void ParachuteSelector() {
            TestExpectedSelectorWords<IMyParachute>(
                Words("parachute", "chute"),
                Words("parachutes", "chutes")
            );
        }

        [TestMethod]
        public void PistonSelector() {
            TestExpectedSelectorWords<IMyPistonBase>(
                Words("piston"),
                Words("pistons")
            );
        }

        [TestMethod]
        public void ProgramSelector() {
            TestExpectedSelectorWords<IMyProgrammableBlock>(
                Words("program", "programmable block"),
                Words("programs", "programmable blocks")
            );
        }

        [TestMethod]
        public void ProjectorSelector() {
            TestExpectedSelectorWords<IMyProjector>(
                Words("projector"),
                Words("projectors")
            );
        }

        [TestMethod]
        public void ReactorSelector() {
            TestExpectedSelectorWords<IMyReactor>(
                Words("reactor"),
                Words("reactors")
            );
        }

        [TestMethod]
        public void RefinerySelector() {
            TestExpectedSelectorWords<IMyRefinery>(
                Words("refinery"),
                Words("refineries")
            );
        }

        [TestMethod]
        public void RemoteControlSelector() {
            TestExpectedSelectorWords<IMyRemoteControl>(
                Words("remote", "drone", "robot"),
                Words("remotes", "drones", "robots")
            );
        }

        [TestMethod]
        public void RotorSelector() {
            TestExpectedSelectorWords<IMyMotorStator>(
                Words("rotor"),
                Words("rotors"),
                block => MockBlockDefinition(block, "LargeStator")
            );
        }

        [TestMethod]
        public void SearchLightSelector() {
            TestExpectedSelectorWords<IMyFunctionalBlock>(
                Words("searchlight"),
                Words("searchlights"),
                block => MockBlockDefinition(block, "Searchlight")
            );
        }

        [TestMethod]
        public void SensorSelector() {
            TestExpectedSelectorWords<IMySensorBlock>(
                Words("sensor"),
                Words("sensors")
            );
        }

        [TestMethod]
        public void SolarPanelSelector() {
            TestExpectedSelectorWords<IMySolarPanel>(
                Words("solar panel"),
                Words("solars", "solar panels")
            );
        }

        [TestMethod]
        public void SorterSelector() {
            TestExpectedSelectorWords<IMyConveyorSorter>(
                Words("sorter"),
                Words("sorters")
            );
        }

        [TestMethod]
        public void SoundBlockSelector() {
            TestExpectedSelectorWords<IMySoundBlock>(
                Words("speaker", "alarm", "siren"),
                Words("speakers", "alarms", "sirens")
            );
        }

        [TestMethod]
        public void TankSelector() {
            TestExpectedSelectorWords<IMyGasTank>(
                Words("tank"),
                Words("tanks")
            );
        }

        [TestMethod]
        public void TerminalBlockSelector() {
            TestExpectedSelectorWords<IMyDecoy>(
                Words("terminal", "terminal block"),
                Words("terminals", "terminal blocks")
            );
        }

        [TestMethod]
        public void ThrusterSelector() {
            TestExpectedSelectorWords<IMyThrust>(
                Words("thruster"),
                Words("thrusters")
            );
        }

        [TestMethod]
        public void TimerBlockSelector() {
            TestExpectedSelectorWords<IMyTimerBlock>(
                Words("timer", "timer block"),
                Words("timers", "timer blocks")
            );
        }

        [TestMethod]
        public void TurretSelector() {
            TestExpectedSelectorWords<IMyLargeTurretBase>(
                Words("turret"),
                Words("turrets")
            );
        }

        [TestMethod]
        public void TurretControllerSelector() {
            TestExpectedSelectorWords<IMyTurretControlBlock>(
                Words("turretcontroller"),
                Words("turretcontrollers")
            );
        }


        [TestMethod]
        public void WarheadSelector() {
            TestExpectedSelectorWords<IMyWarhead>(
                Words("warhead", "bomb"),
                Words("warheads", "bombs")
            );
        }

        [TestMethod]
        public void WelderSelector() {
            TestExpectedSelectorWords<IMyShipWelder>(
                Words("welder"),
                Words("welders")
            );
        }

        [TestMethod]
        public void WheelSelector() {
            TestExpectedSelectorWords<IMyMotorSuspension>(
                Words("wheel", "suspension"),
                Words("wheels", "suspension")
            );
        }

        [TestMethod]
        public void WindTurbineSelector() {
            TestExpectedSelectorWords<IMyPowerProducer>(
                Words("turbine"),
                Words("turbines"),
                block => MockBlockDefinition(block, "WindTurbine")
            );
        }

        void TestExpectedSelectorWords<T>(String[] singularWords, String[] groupWords, Action<Mock<T>> extraActions = null) where T : class, IMyTerminalBlock {
            singularWords.ForEach(word => BlockSelectorWordTest<T>(word, false, extraActions));
            groupWords.ForEach(word => BlockSelectorWordTest<T>(word, true, extraActions));
        }

        void BlockSelectorWordTest<T>(string selectorName, bool useGroup, Action<Mock<T>> extraActions = null) where T : class, IMyTerminalBlock {
            using (var test = new ScriptTest("print the count of the \"test " + selectorName +"\"")) {
                var mockBlock = new Mock<T>();

                if (extraActions != null) extraActions(mockBlock);

                if (useGroup) {
                    test.MockBlocksInGroup("test " + selectorName, mockBlock);
                } else {
                    test.MockBlocksOfType("test " + selectorName, mockBlock);
                }

                test.RunOnce();

                Assert.AreEqual("1", test.Logger[0], selectorName + (useGroup ? " group" : "") + " did not match to expected type: " + typeof(T));
            }
        }

        String[] Words(params String[] words) => words;
    }
}
