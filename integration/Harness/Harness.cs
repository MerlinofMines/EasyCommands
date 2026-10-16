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
                    foreach (var name in test.RequiredBlocks ?? new string[0])
                        if (blocks.Count(b => b.CustomName == name) != 1)
                            throw new Exception("Fixture must contain exactly one block named: " + name);
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
                    pb.Enabled = true;
                    pb.Run("");
                    return;
                }
                ticks++;
                var output = pb.DetailedInfo ?? "";
                if (ticks % 60 == 0) File.WriteAllText(Path.Combine(root, "progress.json"), json.Serialize(new { Scenario = test.Name, Ticks = ticks, Output = output, Working = pb.IsWorking }));
                if (pb.HasCompileErrors || output.Contains("Exception Occurred:") || output.Contains("Caught exception") || output.Contains(failToken))
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
            for (int x = 0; x < 50; x++)
                for (int z = 0; z < 6; z++)
                    grid.CubeBlocks.Add(new MyObjectBuilder_CubeBlock {
                        SubtypeName = "LargeBlockArmorBlock", Min = new Vector3I(x, 0, z)
                    });
            Add(grid, "BatteryBlock", "LargeBlockBatteryBlock", "EC Test Battery", 0);
            Add(grid, "MyProgrammableBlock", "LargeProgrammableBlock", "EC Test PB", 6);
            Add(grid, "InteriorLight", "SmallLight", "EC Test Light", 12);
            Add(grid, "FunctionalBlock", "LargeBlockFarmPlot", "EC Test Farm", 18);
            Add(grid, "FunctionalBlock", "LargeBlockAlgaeFarm", "EC Test Algae", 24);
            Add(grid, "Assembler", "FoodProcessor", "EC Test Kitchen", 30);
            Add(grid, "OxygenGenerator", "IrrigationSystem", "EC Test Irrigation", 36);
            Add(grid, "CargoContainer", "LargeBlockSmallContainer", "source cargo", 42);
            Add(grid, "CargoContainer", "LargeBlockSmallContainer", "destination cargo", 43);
            MyAPIGateway.Entities.RemapObjectBuilder(grid);
            MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(grid);
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
        static void Add(MyObjectBuilder_CubeGrid grid, string type, string subtype, string name, int x) {
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
        }
        void Record(bool passed, string detail) {
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
