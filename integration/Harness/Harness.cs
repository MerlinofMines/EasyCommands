using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage;

using VRage.Plugins;
using VRage.Game;
using VRage.ObjectBuilders;
using VRageMath;
using Sandbox.Common.ObjectBuilders;
using VRage.Game.ObjectBuilders.ComponentSystem;

namespace EasyCommands.Integration {
    public sealed class Request {
        public bool CreateFixture { get; set; }
        public string Script { get; set; }
        public string ProgrammableBlock { get; set; }
        public Scenario[] Scenarios { get; set; }
    }
    public sealed class Scenario {
        public string Name { get; set; }
        public string Commands { get; set; }
        public string[] RequiredBlocks { get; set; }
        public int TimeoutTicks { get; set; }
        public string Setup { get; set; }
    }
    public sealed class Result {
        public string Name { get; set; }
        public bool Passed { get; set; }
        public string Detail { get; set; }
        public int Ticks { get; set; }
    }
    // Loaded only by the disposable integration server. All game access is on Update.
    public sealed class Harness : IPlugin {
        readonly JavaScriptSerializer json = new JavaScriptSerializer();
        readonly List<Result> results = new List<Result>();
        string root;
        Request request;
        IMyProgrammableBlock pb;
        int index, ticks, settle;
        bool finished;
        int startup;
        string passToken, failToken;
        public void Init(object gameInstance) {
            root = Environment.GetEnvironmentVariable("EC_TEST_ROOT");
        }
        public void Dispose() {}
        public void Update() {
            if (finished || String.IsNullOrEmpty(root)) return;
            try {
                if (MyAPIGateway.Session == null || MyAPIGateway.Entities == null) return;
                if (request == null) {
                    request = json.Deserialize<Request>(File.ReadAllText(Path.Combine(root, "request.json")));
                    if (request.Scenarios == null || request.Scenarios.Length == 0)
                        throw new Exception("No scenarios supplied.");
                }
                if (startup == 0) {
                    if (request.CreateFixture) CreateFixture();
                    startup++;
                    return;
                }
                if (startup++ < 180) return;
                if (startup == 181) CaptureMeasurements("startup");
                if (startup == 181) File.WriteAllText(Path.Combine(root, "fixture.json"),
                    json.Serialize(Blocks().Select(b => new { Name = b.CustomName, Definition = b.BlockDefinition.ToString(),
                        Working = b.IsWorking, Functional = b.IsFunctional, Enabled = (b as IMyFunctionalBlock)?.Enabled })));
                if (index >= request.Scenarios.Length) { Finish(); return; }
                var test = request.Scenarios[index];
                if (pb == null) {
                    File.WriteAllText(Path.Combine(root, "progress.json"), json.Serialize(new { Scenario = test.Name, Stage = "compiling", Completed = results }));
                    var blocks = Blocks();
                    pb = blocks.OfType<IMyProgrammableBlock>()
                        .SingleOrDefault(b => b.CustomName == request.ProgrammableBlock);
                    if (pb == null) throw new Exception("Missing or duplicate test programmable block: " + request.ProgrammableBlock);
                    if (!pb.IsFunctional) throw new Exception("Test programmable block is not functional.");
                    pb.Enabled = false;
                    passToken = "EC_PASS_" + Guid.NewGuid().ToString("N");
                    failToken = "EC_FAIL_" + Guid.NewGuid().ToString("N");
                    pb.CustomData = test.Commands.Replace("EC_PASS", passToken).Replace("EC_FAIL", failToken);
                    pb.StorageData = "";
                    pb.ProgramData = request.Script;
                    settle = 0;
                    ticks = 0;
                    return;
                }
                // Give the game's compilation work time to complete.
                if (++settle < 120) return;
                if (settle == 120) {
                    if (pb.HasCompileErrors) { Record(false, "Compilation failed: " + pb.DetailedInfo); return; }
                    PrepareScenario(test, Blocks());
                    var preparedBlocks = Blocks();
                    foreach (var name in test.RequiredBlocks ?? new string[0])
                        if (preparedBlocks.Count(b => b.CustomName == name) != 1)
                            throw new Exception("Fixture must contain exactly one block named: " + name);
                    pb.Enabled = true;
                    pb.Run("");
                    return;
                }
                ticks++;
                if (test.Setup == "connector-production" && ticks % 60 == 0) CaptureMeasurements(test.Name + "-tick-" + ticks);
                var output = pb.DetailedInfo ?? "";
                if (ticks % 60 == 0) File.WriteAllText(Path.Combine(root, "progress.json"), json.Serialize(new { Scenario = test.Name, Ticks = ticks, Output = output, Working = pb.IsWorking }));
                if (pb.HasCompileErrors || output.Contains("Exception Occurred:") || output.Contains("Caught exception") || output.Contains("Script execution terminated") || output.Contains(failToken))
                    Record(false, output);
                else if (output.Contains(passToken)) Record(true, output);
                else if (ticks >= test.TimeoutTicks) Record(false, "Timed out. Last output: " + output);
            } catch (Exception ex) {
                results.Add(new Result { Name = request != null && index < request.Scenarios.Length
                    ? request.Scenarios[index].Name : "harness", Passed = false, Detail = ex.ToString(), Ticks = ticks });
                Finish();
            }
        }
        List<IMyTerminalBlock> Blocks() {
            var entities = new HashSet<VRage.ModAPI.IMyEntity>();
            MyAPIGateway.Entities.GetEntities(entities, e => e is IMyCubeGrid);
            var blocks = new List<IMyTerminalBlock>();
            foreach (var grid in entities.Cast<IMyCubeGrid>()) {
                var slim = new List<IMySlimBlock>();
                grid.GetBlocks(slim);
                blocks.AddRange(slim.Select(b => b.FatBlock).OfType<IMyTerminalBlock>());
            }
            return blocks;
        }
        void CreateFixture() {
            var grid = new MyObjectBuilder_CubeGrid {
                GridSizeEnum = MyCubeSize.Large, IsStatic = true, CreatePhysics = true,
                PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                DisplayName = "EC Integration Rig",
                PositionAndOrientation = new MyPositionAndOrientation(Vector3D.Zero, Vector3.Forward, Vector3.Up)
            };
            // One connected deck, with enough clearance for the Apex production blocks.
            for (int x = 0; x < 80; x++)
                for (int z = 0; z < 6; z++)
                    grid.CubeBlocks.Add(new MyObjectBuilder_CubeBlock {
                        SubtypeName = "LargeBlockArmorBlock", Min = new Vector3I(x, 0, z)
                    });
            Add(grid, "BatteryBlock", "LargeBlockBatteryBlock", "EC Test Battery", 0);
            Add(grid, "Reactor", "LargeBlockSmallGenerator", "EC Test Reactor", 4);
            Add(grid, "MyProgrammableBlock", "LargeProgrammableBlock", "EC Test PB", 6);
            Add(grid, "InteriorLight", "SmallLight", "EC Test Light", 12);
            Add(grid, "FunctionalBlock", "LargeBlockFarmPlot", "EC Test Farm", 18);
            Add(grid, "FunctionalBlock", "LargeBlockAlgaeFarm", "EC Test Algae", 24);
            Add(grid, "Assembler", "FoodProcessor", "EC Test Kitchen", 30);
            Add(grid, "OxygenGenerator", "IrrigationSystem", "EC Test Irrigation", 36);
            Add(grid, "CargoContainer", "LargeBlockSmallContainer", "source cargo", 42);
            Add(grid, "CargoContainer", "LargeBlockSmallContainer", "destination cargo", 43);
            Add(grid, "OxygenFarm", "LargeBlockOxygenFarm", "EC Test Oxygen", 48);
            Add(grid, "OxygenFarm", "LargeBlockOxygenFarmReskin", "EC Test Half Oxygen", 54);
            Add(grid, "FunctionalBlock", "LargeBlockAlgaeFarmReskin", "EC Test Half Algae", 60);
            Add(grid, "Gyro", "LargeBlockGyro", "EC Test Gyro", 66);
            Add(grid, "Projector", "LargeProjector", "EC Test Projector", 70);
            if (request.Scenarios.Any(s => s.Setup == "connector-production")) {
                Add(grid, "Refinery", "LargeRefinery", "EC Test Refinery", 74);
                var connector = Add(grid, "ShipConnector", "Connector", "EC Test Dock", 78);
                connector.Min = new Vector3I(78, 1, -1);
                Add(grid, "ShipConnector", "Connector", "EC Spare Dock", 70).Min = new Vector3I(70, 1, -1);
                var partnerGrid = new MyObjectBuilder_CubeGrid {
                    GridSizeEnum = MyCubeSize.Large, IsStatic = false, CreatePhysics = true,
                    PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                    DisplayName = "EC Dock Partner Rig",
                    PositionAndOrientation = new MyPositionAndOrientation(new Vector3D(195, 2.5, -5.1), Vector3.Backward, Vector3.Up)
                };
                Add(partnerGrid, "ShipConnector", "Connector", "EC Partner Dock", 0).Min = Vector3I.Zero;
                Add(partnerGrid, "BatteryBlock", "LargeBlockBatteryBlock", "EC Partner Battery", 0).Min = new Vector3I(0, 0, 1);
                MyAPIGateway.Entities.RemapObjectBuilder(partnerGrid);
                MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(partnerGrid);
            }
            MyAPIGateway.Entities.RemapObjectBuilder(grid);
            MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(grid);
            var reactor = Blocks().Single(b => b.CustomName == "EC Test Reactor");
            reactor.GetInventory().AddItems((MyFixedPoint)100, MyObjectBuilderSerializer.CreateNewObject(MyObjectBuilderType.Parse("MyObjectBuilder_Ingot"), "Uranium") as MyObjectBuilder_PhysicalObject);
            var source = Blocks().Single(b => b.CustomName == "source cargo");
            var seed = MyObjectBuilderSerializer.CreateNewObject(
                MyObjectBuilderType.Parse("MyObjectBuilder_SeedItem"), "Fruit") as MyObjectBuilder_PhysicalObject;
            if (seed == null) throw new Exception("Fruit seed definition unavailable.");
            source.GetInventory().AddItems((VRage.MyFixedPoint)5, seed);
            // Deliberate decoy: fruit and fruit seeds must not share a filter.
            var fruit = MyObjectBuilderSerializer.CreateNewObject(
                MyObjectBuilderType.Parse("MyObjectBuilder_ConsumableItem"), "Fruit") as MyObjectBuilder_PhysicalObject;
            source.GetInventory().AddItems((VRage.MyFixedPoint)99, fruit);
        }
        static MyObjectBuilder_TerminalBlock Add(MyObjectBuilder_CubeGrid grid, string type, string subtype, string name, int x) {
            var block = MyObjectBuilderSerializer.CreateNewObject(
                MyObjectBuilderType.Parse("MyObjectBuilder_" + type), subtype) as MyObjectBuilder_TerminalBlock;
            if (block == null) throw new Exception("Cannot create fixture block: " + type + "/" + subtype);
            block.IntegrityPercent = 1f;
            block.BuildPercent = 1f;
            block.Min = new Vector3I(x, 1, 2);
            block.CustomName = name;
            var battery = block as MyObjectBuilder_BatteryBlock;
            if (battery != null) battery.CurrentStoredPower = 3;
            grid.CubeBlocks.Add(block);
            return block;
        }
        static void AddFarmState(MyObjectBuilder_CubeGrid grid, string name, int x, float health) {
            var block = Add(grid, "FunctionalBlock", "LargeBlockFarmPlot", name, x);
            block.ComponentContainer = new MyObjectBuilder_ComponentContainer {
                Components = new List<MyObjectBuilder_ComponentContainer.ComponentData> {
                    new MyObjectBuilder_ComponentContainer.ComponentData {
                        TypeId = "MyFarmPlotLogic",
                        Component = new MyObjectBuilder_FarmPlotLogic {
                            PlantedSeedDefinitionId = new SerializableDefinitionId(MyObjectBuilderType.Parse("MyObjectBuilder_SeedItem"), "Fruit"),
                            PlantGrowthTimeRemainingInFrames = 0,
                            PlantHealth = health,
                            // Use the vanilla minimum so exposure does not change the expected yield.
                            OutputItemAmountFromRange = 2
                        }
                    }
                }
            };
        }
        void PrepareScenario(Scenario scenario, List<IMyTerminalBlock> blocks) {
            if (String.IsNullOrEmpty(scenario.Setup)) return;
            IMyResourceStorageComponent water;
            IMyFarmPlotLogic crop;
            if (scenario.Setup == "connector-production") {
                var refinery = (IMyRefinery)blocks.Single(b => b.CustomName == "EC Test Refinery");
                refinery.Enabled = false;
                refinery.UseConveyorSystem = false;
                refinery.GetInventory(0).Clear();
                refinery.GetInventory(1).Clear();
                var ore = MyObjectBuilderSerializer.CreateNewObject(MyObjectBuilderType.Parse("MyObjectBuilder_Ore"), "Iron") as MyObjectBuilder_PhysicalObject;
                refinery.GetInventory(0).AddItems((MyFixedPoint)1000, ore);
            } else if (scenario.Setup == "water-quarter") {
                var farm = blocks.Single(b => b.CustomName == "EC Test Farm");
                if (!farm.Components.TryGet(out water)) throw new Exception("Missing water component");
                water.ChangeFilledRatio(.25, true);
            } else if (scenario.Setup == "crop-planted") {
                var farm = blocks.Single(b => b.CustomName == "EC Test Farm");
                if (!farm.Components.TryGet(out crop)) throw new Exception("Missing crop component");
                if (!crop.PlantSeed(new MyDefinitionId(MyObjectBuilderType.Parse("MyObjectBuilder_SeedItem"), "Fruit"))) throw new Exception("Fixture planting failed");
            } else if (scenario.Setup == "algae-blocked") {
                ((IMyFunctionalBlock)blocks.Single(b => b.CustomName == "EC Test Half Algae")).Enabled = false;
            } else if (scenario.Setup == "crop-states") {
                var farm = blocks.Single(b => b.CustomName == "EC Test Farm");
                var builder = new MyObjectBuilder_CubeGrid();
                AddFarmState(builder, "EC Test Mature Farm", 66, 100);
                AddFarmState(builder, "EC Test Dead Farm", 72, 0);
                foreach (var block in builder.CubeBlocks)
                    if (farm.CubeGrid.AddBlock(block, false) == null) throw new Exception("Could not add crop fixture");
            } else if (scenario.Setup == "food-ingredients") {
                var kitchen = (IMyAssembler)blocks.Single(b => b.CustomName == "EC Test Kitchen");
                kitchen.Enabled = false;
                kitchen.ClearQueue();
                var algae = MyObjectBuilderSerializer.CreateNewObject(MyObjectBuilderType.Parse("MyObjectBuilder_PhysicalObject"), "Algae") as MyObjectBuilder_PhysicalObject;
                kitchen.GetInventory(0).AddItems((MyFixedPoint)40, algae);
            } else throw new Exception("Unknown scenario setup: " + scenario.Setup);
            CaptureMeasurements("setup-" + scenario.Setup);
        }
        void CaptureMeasurements(string stage) {
            var readings = new List<object>();
            foreach (var block in Blocks()) {
                var values = new Dictionary<string, object>();
                Sandbox.ModAPI.Ingame.IMyResourceStorageComponent water;
                if (block.Components.TryGet(out water)) {
                    values["WaterType"] = water.GetType().FullName;
                    values["WaterCapacity"] = water.ResourceCapacity;
                    values["WaterRatio"] = water.FilledRatio;
                }
                Sandbox.ModAPI.Ingame.IMyFarmPlotLogic crop;
                if (block.Components.TryGet(out crop)) {
                    values["CropType"] = crop.GetType().FullName;
                    values["CropAssembly"] = crop.GetType().Assembly.GetName().Name;
                    values["CropInfo"] = crop.GetDetailedInfoWithoutRequiredInput();
                    values["SeedRequirement"] = crop.AmountOfSeedsRequired;
                    values["HarvestAmount"] = crop.OutputItemAmount;
                }
                Sandbox.ModAPI.Ingame.IMySolarFoodGenerator algae;
                if (block.Components.TryGet(out algae)) {
                    values["ItemsPerMinute"] = algae.ItemsPerMinute;
                    values["CountdownSeconds"] = float.IsNaN(algae.TimeRemainingUntilNextBatch) || float.IsInfinity(algae.TimeRemainingUntilNextBatch) ? (object)algae.TimeRemainingUntilNextBatch.ToString(System.Globalization.CultureInfo.InvariantCulture) : algae.TimeRemainingUntilNextBatch;
                }
                var assembler = block as IMyAssembler;
                var production = block as IMyProductionBlock;
                if (production != null) {
                    values["Producing"] = production.IsProducing;
                    values["Working"] = production.IsWorking;
                    values["Enabled"] = production.Enabled;
                }
                var connector = block as IMyShipConnector;
                if (connector != null) {
                    values["Status"] = connector.Status.ToString();
                    values["OtherConnector"] = connector.OtherConnector?.CustomName;
                    values["Working"] = connector.IsWorking;
                }
                if (assembler != null) {
                    values["Producing"] = assembler.IsProducing;
                    values["Progress"] = assembler.CurrentProgress;
                    values["QueueEmpty"] = assembler.IsQueueEmpty;
                    values["Working"] = assembler.IsWorking;
                    var items = new List<VRage.Game.ModAPI.Ingame.MyInventoryItem>();
                    for (int inventory = 0; inventory < assembler.InventoryCount; inventory++) {
                        items.Clear();
                        assembler.GetInventory(inventory).GetItems(items);
                        values["Inventory" + inventory] = items.Select(i => i.Type.ToString() + ":" + i.Amount).ToArray();
                    }
                }
                var oxygen = block as SpaceEngineers.Game.ModAPI.Ingame.IMyOxygenFarm;
                if (oxygen != null) {
                    values["OxygenOutput"] = oxygen.GetOutput();
                    values["CanProduce"] = oxygen.CanProduce;
                }
                if (values.Count > 0) readings.Add(new { Name = block.CustomName, Values = values, Detail = block.DetailedInfo });
            }
            File.AppendAllText(Path.Combine(root, "measurements.jsonl"), json.Serialize(new { Stage = stage, Readings = readings }) + Environment.NewLine);
        }
        void Record(bool passed, string detail) {
            CaptureMeasurements(request.Scenarios[index].Name);
            results.Add(new Result { Name = request.Scenarios[index].Name, Passed = passed, Detail = detail, Ticks = ticks });
            pb.Enabled = false;
            pb = null;
            index++;
        }
        void Finish() {
            finished = true;
            var temporary = Path.Combine(root, "results.tmp");
            File.WriteAllText(temporary, json.Serialize(results));
            File.Move(temporary, Path.Combine(root, "results.json"));
        }
    }
}
