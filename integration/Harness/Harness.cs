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
        public string ModSourceDirectory { get; set; }
        public bool ModBenchmark { get; set; }
        public bool MissingMod { get; set; }
        public bool SkipBenchmarkSamples { get; set; }
        public bool SkipStandaloneBatch { get; set; }
        public bool LifecycleOnly { get; set; }
        public bool SchedulingOnly { get; set; }
        public string FullScript { get; set; }
        public int StressBlocks { get; set; }
        public int StressTicks { get; set; }
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
    public sealed partial class Harness : IPlugin {
        readonly JavaScriptSerializer json = new JavaScriptSerializer();
        readonly List<Result> results = new List<Result>();
        string root;
        Request request;
        IMyProgrammableBlock pb;
        int index, ticks, settle;
        bool finished;
        int startup;
        bool modLoaded;
        string passToken, failToken;
        Vector3D motionStart, motionDestination;
        double maxTravel, maxSpeed;
        bool sawPlaying, sawFleeing, sawEnemy, sawStopped;
        Sandbox.Game.Entities.MyPlanet testPlanet;
        Vector3D planetCenter;
        int goalVisits, startReturns, loopLeg;
        double clearance, minClearance, maxClearance, alignmentAngle, initialAlignmentAngle, naturalGravity;
        double protectedClearanceMin;
        int protectedSamples;
        BoundingBoxD obstacleBox;
        bool hasObstacle, obstacleOverlap, enemyRemoved, obstacleCleared, sawObstacleStop;
        VRage.ModAPI.IMyEntity obstacleEntity;
        double maxLateralDeviation, minObstacleDistance;
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
                if (!modLoaded && !String.IsNullOrEmpty(request.ModSourceDirectory)) {
                    ModSourceLoader.Load(request.ModSourceDirectory, root);
                    modLoaded = true;
                    return;
                }
                if (startup == 0) {
                    if (request.StressBlocks > 0) CreateStressFixtures();
                    else if (request.CreateFixture) CreateFixture();
                    if (request.ModBenchmark) CreateModFixtures();
                    startup++;
                    return;
                }
                if (startup++ < 180) return;
                if (startup == 181) CaptureMeasurements("startup");
                if (startup == 181) File.WriteAllText(Path.Combine(root, "fixture.json"),
                    json.Serialize(Blocks().Select(b => new { Name = b.CustomName, Definition = b.BlockDefinition.ToString(),
                        Working = b.IsWorking, Functional = b.IsFunctional, Enabled = (b as IMyFunctionalBlock)?.Enabled })));
                if (request.ModBenchmark) { TickMod(); return; }
                if (request.StressBlocks > 0) { TickStress(); return; }
                if (request.MissingMod) { TickMissingModBridge(); return; }
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
                if (test.Setup == "ai-motion-target-loss" && ticks >= 1800 && !enemyRemoved) {
                    Blocks().Single(b => b.CustomName == "EC Threat Battery").CubeGrid.Close();
                    enemyRemoved = true;
                }
                if (test.Setup == "ai-motion-obstacle" && ticks >= 2700 && !obstacleCleared) {
                    obstacleEntity.Close();
                    obstacleCleared = true;
                }
                if (test.Setup == "connector-production" && ticks % 60 == 0) CaptureMeasurements(test.Name + "-tick-" + ticks);
                if (test.Setup != null && test.Setup.StartsWith("ai-motion") && ticks % 30 == 0) ObserveMotion();
                var output = pb.DetailedInfo ?? "";
                if (ticks % 60 == 0) File.WriteAllText(Path.Combine(root, "progress.json"), json.Serialize(new { Scenario = test.Name, Ticks = ticks, Output = output, Working = pb.IsWorking }));
                if (pb.HasCompileErrors || output.Contains("Exception Occurred:") || output.Contains("Caught exception") || output.Contains("Script execution terminated") || output.Contains(failToken))
                    Record(false, output);
                else if (output.Contains(passToken)) Record(true, output);
                else if (ticks >= test.TimeoutTicks) Record(false, "Timed out. Last output: " + output);
            } catch (Exception ex) {
                results.Add(new Result { Name = request != null && request.StressBlocks > 0 ? "stress-phase-" + stressPhase : request != null && request.ModBenchmark ? "mod-phase-" + modPhase : request != null && index < request.Scenarios.Length
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
            if (request.Scenarios.Any(s => s.Setup != null && s.Setup.StartsWith("ai-motion"))) {
                CreateDroneFixture();
                return;
            }
            var grid = new MyObjectBuilder_CubeGrid {
                GridSizeEnum = MyCubeSize.Large, IsStatic = true, CreatePhysics = true,
                PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                DisplayName = "EC Integration Rig",
                PositionAndOrientation = new MyPositionAndOrientation(Vector3D.Zero, Vector3.Forward, Vector3.Up)
            };
            // One connected deck, with enough clearance for the Apex production blocks.
            for (int x = 0; x < 122; x++)
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
            Add(grid, "FlightMovementBlock", "LargeFlightMovement", "EC AI Flight", 82);
            Add(grid, "BasicMissionBlock", "LargeBasicMission", "EC AI Basic", 87);
            Add(grid, "PathRecorderBlock", "LargePathRecorderBlock", "EC AI Recorder", 92);
            Add(grid, "DefensiveCombatBlock", "LargeDefensiveCombat", "EC AI Defensive", 97);
            Add(grid, "OffensiveCombatBlock", "LargeOffensiveCombat", "EC AI Offensive", 102);
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
        void CreateDroneFixture() {
            // The compact test rig verifies controls, not exhaust-clearance damage.
            MyAPIGateway.Session.SessionSettings.ThrusterDamage = false;
            bool planetary = request.Scenarios.Any(s => s.Setup == "ai-motion-planet");
            Vector3D spawn = Vector3D.Zero;
            Vector3 up = Vector3.Up;
            if (planetary) {
                testPlanet = Sandbox.Game.World.MyWorldGenerator.AddPlanet("EC Moon", "EC Moon", "Moon", new Vector3D(50000, 0, 0), 12345, 6000, false, userCreated: true);
                if (testPlanet == null) throw new Exception("Could not generate the planetary fixture");
                planetCenter = testPlanet.PositionComp.WorldAABB.Center;
                spawn = testPlanet.GetClosestSurfacePointGlobal(planetCenter + Vector3D.Up * 10000) + Vector3D.Up * 100;
                up = new Vector3((float)-Math.Sin(Math.PI / 5), (float)Math.Cos(Math.PI / 5), 0);
            }
            long owner = Sandbox.Game.World.MySession.Static.Players.CreateNewIdentity("EC Drone Owner", addToNpcs: true).IdentityId;
            long enemy = Sandbox.Game.World.MySession.Static.Players.CreateNewIdentity("EC Threat Owner", addToNpcs: true).IdentityId;
            var grid = new MyObjectBuilder_CubeGrid {
                GridSizeEnum = MyCubeSize.Large, IsStatic = planetary, CreatePhysics = true,
                PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                DisplayName = "EC Flight Rig",
                PositionAndOrientation = new MyPositionAndOrientation(spawn, Vector3.Forward, up)
            };
            for (int x = 0; x < 16; x++)
                for (int z = 0; z < 9; z++) grid.CubeBlocks.Add(new MyObjectBuilder_CubeBlock { SubtypeName = "LargeBlockArmorBlock", Min = new Vector3I(x, 0, z) });
            AddDroneBlock(grid, "BatteryBlock", "LargeBlockBatteryBlock", "EC Drone Battery", new Vector3I(0, 1, 0));
            AddDroneBlock(grid, "Reactor", "LargeBlockSmallGenerator", "EC Drone Reactor", new Vector3I(0, 1, 6));
            AddDroneBlock(grid, "MyProgrammableBlock", "LargeProgrammableBlock", "EC Test PB", new Vector3I(4, 1, 0));
            AddDroneBlock(grid, "FlightMovementBlock", "LargeFlightMovement", "EC Drone Flight", new Vector3I(6, 1, 0));
            AddDroneBlock(grid, "PathRecorderBlock", "LargePathRecorderBlock", "EC Drone Recorder", new Vector3I(8, 1, 0));
            AddDroneBlock(grid, "DefensiveCombatBlock", "LargeDefensiveCombat", "EC Drone Defensive", new Vector3I(10, 1, 0));
            AddDroneBlock(grid, "OffensiveCombatBlock", "LargeOffensiveCombat", "EC Drone Offensive", new Vector3I(14, 1, 0));
            AddDroneBlock(grid, "Gyro", "LargeBlockGyro", "EC Drone Gyro", new Vector3I(12, 1, 0));
            for (int i = 0; i < 5; i++) AddDroneBlock(grid, "Gyro", "LargeBlockGyro", "EC Drone Gyro " + i, new Vector3I(10 + i % 3 * 2, 1, 3 + i / 3));
            var directions = new[] { Base6Directions.Direction.Forward, Base6Directions.Direction.Backward, Base6Directions.Direction.Left, Base6Directions.Direction.Right, Base6Directions.Direction.Up, Base6Directions.Direction.Down };
            for (int i = 0; i < directions.Length; i++) {
                var thrust = AddDroneBlock(grid, "Thrust", "LargeBlockSmallThrust", "EC Drone Thrust " + i, new Vector3I(4 + i % 4 * 3, 1, i < 4 ? 6 : 3));
                thrust.BlockOrientation = new SerializableBlockOrientation(directions[i], i < 4 ? Base6Directions.Direction.Up : Base6Directions.Direction.Forward);
            }
            if (planetary) {
                AddDroneBlock(grid, "Reactor", "LargeBlockSmallGenerator", "EC Drone Lift Reactor", new Vector3I(1, 1, 6));
                for (int i = 0; i < 6; i++) {
                    var lift = AddDroneBlock(grid, "Thrust", "LargeBlockSmallThrust", "EC Lift " + i, new Vector3I(i % 3, 1, 3 + i / 3));
                    lift.BlockOrientation = new SerializableBlockOrientation(Base6Directions.Direction.Up, Base6Directions.Direction.Forward);
                    var down = AddDroneBlock(grid, "Thrust", "LargeBlockSmallThrust", "EC Down " + i, new Vector3I(i < 3 ? i : 3, 1, i < 3 ? 5 : i));
                    down.BlockOrientation = new SerializableBlockOrientation(Base6Directions.Direction.Down, Base6Directions.Direction.Forward);
                }
            }
            foreach (var block in grid.CubeBlocks) block.Owner = owner;
            MyAPIGateway.Entities.RemapObjectBuilder(grid);
            MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(grid);
            var target = new MyObjectBuilder_CubeGrid {
                GridSizeEnum = MyCubeSize.Large, IsStatic = true, CreatePhysics = true,
                PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                DisplayName = "EC Threat Grid",
                PositionAndOrientation = new MyPositionAndOrientation(new Vector3D(1000, 0, 0), Vector3.Forward, Vector3.Up)
            };
            AddDroneBlock(target, "BatteryBlock", "LargeBlockBatteryBlock", "EC Threat Battery", Vector3I.Zero).Owner = enemy;
            MyAPIGateway.Entities.RemapObjectBuilder(target);
            MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(target);
            if (request.Scenarios.Any(s => s.Setup == "ai-motion-obstacle")) {
                var obstacle = new MyObjectBuilder_CubeGrid {
                    GridSizeEnum = MyCubeSize.Large, IsStatic = true, CreatePhysics = true,
                    PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                    DisplayName = "EC Obstacle",
                    PositionAndOrientation = new MyPositionAndOrientation(new Vector3D(10, 2.5, -150), Vector3.Forward, Vector3.Up)
                };
                for (int x = -4; x <= 4; x++) for (int y = -4; y <= 4; y++) for (int z = -4; z <= 4; z++)
                    obstacle.CubeBlocks.Add(new MyObjectBuilder_CubeBlock { SubtypeName = "LargeBlockArmorBlock", Min = new Vector3I(x, y, z) });
                MyAPIGateway.Entities.RemapObjectBuilder(obstacle);
                var entity = MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(obstacle);
                obstacleEntity = entity;
                obstacleBox = entity.WorldAABB;
                hasObstacle = true;
            }
            foreach (var reactor in Blocks().OfType<IMyReactor>()) reactor.GetInventory().AddItems((MyFixedPoint)100, MyObjectBuilderSerializer.CreateNewObject(MyObjectBuilderType.Parse("MyObjectBuilder_Ingot"), "Uranium") as MyObjectBuilder_PhysicalObject);
        }
        static MyObjectBuilder_TerminalBlock AddDroneBlock(MyObjectBuilder_CubeGrid grid, string type, string subtype, string name, Vector3I position) {
            var block = Add(grid, type, subtype, name, position.X);
            block.Min = position;
            return block;
        }
        static void AddFixtureWaypoint(IMyPathRecorderComponent recorder, Vector3D position, string name, Vector3D forward, Vector3D up, bool forceRotation = true) {
            recorder.AddWaypoint(new Sandbox.Game.Entities.MyAutopilotWaypoint(MatrixD.CreateWorld(position, forward, up), name, null, null, false));
            // AddWaypoint's game RPC forces recorded rotation even for coordinate-only
            // constructors (whose matrix axes are zero). Planet tests use GPS semantics
            // so that the EC gravity-alignment control must supply the orientation.
            ((Sandbox.Game.EntityComponents.MyPathRecorderComponent)recorder).Waypoints.Last().ForceRotation = forceRotation;
        }
        void ObserveMotion() {
            var blocks = Blocks();
            var recorderBlock = blocks.Single(b => b.CustomName == "EC Drone Recorder");
            Sandbox.ModAPI.Ingame.IMyPathRecorderComponent recorder;
            recorderBlock.Components.TryGet(out recorder);
            var defensive = (Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock)blocks.Single(b => b.CustomName == "EC Drone Defensive");
            var position = pb.GetPosition();
            var speed = pb.CubeGrid.Physics.LinearVelocity.Length();
            maxTravel = Math.Max(maxTravel, Vector3D.Distance(motionStart, position));
            maxSpeed = Math.Max(maxSpeed, speed);
            sawPlaying |= recorder.IsPlaying;
            sawFleeing |= defensive.IsFleeing;
            sawEnemy |= defensive.SearchEnemyComponent.FoundEnemyId.HasValue;
            // Require stopping during the explicit pause, before playback resumes.
            sawStopped |= ticks > 600 && ticks < 1000 && !recorder.IsPlaying && maxTravel > 5 && speed < .5;
            if (hasObstacle && !obstacleCleared) {
                var delta = position - motionStart;
                maxLateralDeviation = Math.Max(maxLateralDeviation, Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y));
                minObstacleDistance = Math.Min(minObstacleDistance, Vector3D.Distance(position, Vector3D.Clamp(position, obstacleBox.Min, obstacleBox.Max)));
                obstacleOverlap |= pb.CubeGrid.WorldAABB.Intersects(obstacleBox);
                sawObstacleStop |= maxTravel > 80 && Vector3D.Distance(position, motionDestination) > 100 && !recorder.IsPlaying && speed < .5;
            }
            var setup = request.Scenarios[index].Setup;
            if (setup == "ai-motion-repeat") {
                if (loopLeg == 0 && Vector3D.Distance(position, motionDestination) < 20) { goalVisits++; loopLeg = 1; }
                if (loopLeg == 1 && Vector3D.Distance(position, motionStart) < 20) { startReturns++; loopLeg = 0; }
            }
            if (testPlanet != null) {
                var gravity = Sandbox.Game.GameSystems.MyGravityProviderSystem.CalculateNaturalGravityInPoint(position);
                naturalGravity = gravity.Length();
                var radial = Vector3D.Normalize(position - planetCenter);
                clearance = Vector3D.Dot(position - testPlanet.GetClosestSurfacePointGlobal(position), radial);
                minClearance = Math.Min(minClearance, clearance);
                maxClearance = Math.Max(maxClearance, clearance);
                alignmentAngle = Math.Acos(Math.Max(-1, Math.Min(1, Vector3D.Dot(pb.WorldMatrix.Up, -Vector3D.Normalize(gravity))))) * 180 / Math.PI;
                // Minimum altitude is a safety floor, not a requested hover height.
                // Allow the initial climb, then require clearance throughout the
                // high-floor phase before EC lowers the setting at 60 seconds.
                if (ticks >= 1200 && ticks < 3600) {
                    protectedClearanceMin = Math.Min(protectedClearanceMin, clearance);
                    protectedSamples++;
                }
            }
            File.AppendAllText(Path.Combine(root, "motion.jsonl"), json.Serialize(new {
                Scenario = request.Scenarios[index].Name, Ticks = ticks, Position = new[] { position.X, position.Y, position.Z },
                Speed = speed, Distance = Vector3D.Distance(position, motionDestination), Playing = recorder.IsPlaying,
                Forward = new[] { pb.WorldMatrix.Forward.X, pb.WorldMatrix.Forward.Y, pb.WorldMatrix.Forward.Z }, AngularSpeed = pb.CubeGrid.Physics.AngularVelocity.Length(),
                Fleeing = defensive.IsFleeing, Enemy = defensive.SearchEnemyComponent.FoundEnemyId,
                goalVisits, startReturns, clearance, minClearance, maxClearance, alignmentAngle, naturalGravity,
                protectedClearanceMin, protectedSamples,
                maxLateralDeviation, minObstacleDistance, obstacleOverlap, obstacleCleared, sawObstacleStop, enemyRemoved,
                Mass = pb.CubeGrid.Physics.Mass,
                FlightInfo = blocks.Single(b => b.CustomName == "EC Drone Flight").DetailedInfo,
                DefenseInfo = blocks.Single(b => b.CustomName == "EC Drone Defensive").DetailedInfo
            }) + Environment.NewLine);
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
            if (scenario.Setup.StartsWith("ai-motion")) {
                maxTravel = maxSpeed = 0;
                goalVisits = startReturns = loopLeg = 0;
                minClearance = double.MaxValue;
                maxClearance = 0;
                protectedClearanceMin = double.MaxValue;
                protectedSamples = 0;
                maxLateralDeviation = 0;
                minObstacleDistance = double.MaxValue;
                obstacleOverlap = obstacleCleared = sawObstacleStop = enemyRemoved = false;
                sawPlaying = sawFleeing = sawEnemy = sawStopped = false;
                foreach (var block in blocks.Where(b => b.CustomName.StartsWith("EC Drone ") && (b is IMyFlightMovementBlock || b is IMyPathRecorderBlock || b is IMyDefensiveCombatBlock || b is IMyOffensiveCombatBlock)))
                    Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValueBool(block, "ActivateBehavior", false);
                pb.CubeGrid.Physics.ClearSpeed();
                motionStart = pb.GetPosition();
                motionDestination = motionStart + new Vector3D(0, 0, scenario.Setup == "ai-motion-repeat" ? -60 : -150);
                if (scenario.Setup == "ai-motion-obstacle") motionDestination = motionStart + new Vector3D(0, 0, -300);
                var rb = blocks.Single(b => b.CustomName == "EC Drone Recorder");
                IMyPathRecorderComponent recorder;
                rb.Components.TryGet(out recorder);
                recorder.StopPlay();
                recorder.DeleteAllWaypoints();
                var waypointUp = testPlanet == null ? Vector3D.Up : Vector3D.Normalize(motionDestination - planetCenter);
                var waypointForward = Vector3D.Normalize(Vector3D.Forward - waypointUp * Vector3D.Dot(Vector3D.Forward, waypointUp));
                AddFixtureWaypoint(recorder, motionDestination, "EC arrival", waypointForward, waypointUp, testPlanet == null);
                if (scenario.Setup == "ai-motion-repeat") AddFixtureWaypoint(recorder, motionStart, "EC return", Vector3D.Backward, Vector3D.Up);
                var defensive = (IMyDefensiveCombatBlock)blocks.Single(b => b.CustomName == "EC Drone Defensive");
                defensive.FleeTrigger = VRage.Game.ModAPI.Ingame.FleeTrigger.Never;
                // Extended test must replace this deliberately wrong destination through EC.
                defensive.CustomFleeCoordinates = scenario.Setup == "ai-motion-extended" ? motionStart + new Vector3D(0, 0, 150) : motionDestination;
                defensive.WaypointZoneSize = 5;
                defensive.SearchEnemyComponent.TargetingLockOptions = VRage.Game.ModAPI.Ingame.MyGridTargetingRelationFiltering.Enemy | VRage.Game.ModAPI.Ingame.MyGridTargetingRelationFiltering.Neutral;
                defensive.SearchEnemyComponent.SubsystemsToDestroy = VRage.Utils.MyStringHash.GetOrCompute("PowerSystems");
                var offensive = (IMyOffensiveCombatBlock)blocks.Single(b => b.CustomName == "EC Drone Offensive");
                offensive.SearchEnemyComponent.TargetingLockOptions = defensive.SearchEnemyComponent.TargetingLockOptions;
                offensive.SearchEnemyComponent.SubsystemsToDestroy = defensive.SearchEnemyComponent.SubsystemsToDestroy;
                Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValue<long>(defensive, "FleeMode", 1);
                if (testPlanet != null) {
                    var flight = blocks.Single(b => b.CustomName == "EC Drone Flight");
                    Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValueFloat(flight, "MaxPitchAngle", 0);
                    Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValueFloat(flight, "MaxRollAngle", 0);
                    var gravity = Sandbox.Game.GameSystems.MyGravityProviderSystem.CalculateNaturalGravityInPoint(pb.GetPosition());
                    initialAlignmentAngle = Math.Acos(Math.Max(-1, Math.Min(1, Vector3D.Dot(pb.WorldMatrix.Up, -Vector3D.Normalize(gravity))))) * 180 / Math.PI;
                    pb.CubeGrid.IsStatic = false;
                }
                CaptureMeasurements("setup-" + scenario.Setup);
                return;
            }
            if (scenario.Setup == "ai-reset") {
                foreach (var block in blocks.Where(b => b.CustomName.StartsWith("EC AI "))) {
                    ((IMyFunctionalBlock)block).Enabled = true;
                    Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValueBool(block, "ActivateBehavior", false);
                }
                var recorderBlock = blocks.Single(b => b.CustomName == "EC AI Recorder");
                IMyPathRecorderComponent recorder;
                if (!recorderBlock.Components.TryGet(out recorder)) throw new Exception("Missing recorder component");
                recorder.StopPlay();
                recorder.DeleteAllWaypoints();
                AddFixtureWaypoint(recorder, new Vector3D(5000, 0, 0), "EC distant waypoint", Vector3D.Forward, Vector3D.Up);
                CaptureMeasurements("setup-ai-reset");
                return;
            }
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
                // A healthy new crop can report zero output until yield is established.
                // Positive known yield is checked separately by the mature-crop fixture.
                var farm = blocks.Single(b => b.CustomName == "EC Test Farm");
                if (!farm.Components.TryGet(out crop)) throw new Exception("Missing crop component");
                if (!crop.PlantSeed(new MyDefinitionId(MyObjectBuilderType.Parse("MyObjectBuilder_SeedItem"), "Fruit"))) throw new Exception("Fixture planting failed");
            } else if (scenario.Setup == "algae-blocked") {
                ((IMyFunctionalBlock)blocks.Single(b => b.CustomName == "EC Test Half Algae")).Enabled = false;
            } else if (scenario.Setup == "crop-states") {
                var farm = blocks.Single(b => b.CustomName == "EC Test Farm");
                var builder = new MyObjectBuilder_CubeGrid();
                AddFarmState(builder, "EC Test Mature Farm", 110, 100);
                AddFarmState(builder, "EC Test Dead Farm", 116, 0);
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
                var thrust = block as Sandbox.ModAPI.Ingame.IMyThrust;
                if (thrust != null) {
                    values["Working"] = thrust.IsWorking;
                    values["Functional"] = thrust.IsFunctional;
                    values["Direction"] = thrust.Orientation.Forward.ToString();
                    values["MaximumForce"] = thrust.MaxEffectiveThrust;
                }
                if (block.CustomName.StartsWith("EC AI ")) {
                    values["Enabled"] = ((IMyFunctionalBlock)block).Enabled;
                    values["Behavior"] = Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValueBool(block, "ActivateBehavior");
                    var flight = block as Sandbox.ModAPI.Ingame.IMyFlightMovementBlock;
                    if (flight != null) {
                        values["SpeedLimit"] = flight.SpeedLimit;
                        values["Precision"] = flight.PrecisionMode;
                        values["Avoidance"] = flight.CollisionAvoidance;
                        values["MinimumAltitudeApi"] = flight.MinimalAltitude;
                        values["MinimumAltitudeSetting"] = Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValueFloat(block, "MinHeightAboveTerrain");
                        values["GravityAlignment"] = flight.AlignToPGravity;
                    }
                    Sandbox.ModAPI.Ingame.IMyPathRecorderComponent recorder;
                    if (block.Components.TryGet(out recorder)) values["Playing"] = recorder.IsPlaying;
                    var defensive = block as Sandbox.ModAPI.Ingame.IMyDefensiveCombatBlock;
                    if (defensive != null) {
                        values["Fleeing"] = defensive.IsFleeing;
                        values["EnemyId"] = defensive.SearchEnemyComponent.FoundEnemyId;
                    }
                }
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
            var setup = request.Scenarios[index].Setup;
            if (passed && setup != null && setup.StartsWith("ai-motion")) {
                ObserveMotion();
                var distance = Vector3D.Distance(pb.GetPosition(), motionDestination);
                var speed = pb.CubeGrid.Physics.LinearVelocity.Length();
                var thrusters = Blocks().OfType<Sandbox.ModAPI.Ingame.IMyThrust>().Where(b => b.CubeGrid.EntityId == pb.CubeGrid.EntityId).ToList();
                bool healthyThrust = thrusters.Count >= 6 && thrusters.Select(b => b.Orientation.Forward).Distinct().Count() == 6 && thrusters.All(b => b.IsFunctional && b.MaxEffectiveThrust > 0);
                bool valid = maxTravel > 100 && distance < 12 && speed < 1 && healthyThrust;
                if (setup == "ai-motion-flee" || setup == "ai-motion-extended") valid &= sawEnemy && sawFleeing;
                else valid &= sawPlaying;
                if (setup == "ai-motion-stop") valid &= sawStopped;
                if (setup == "ai-motion-repeat") {
                    IMyPathRecorderComponent recorder;
                    Blocks().Single(b => b.CustomName == "EC Drone Recorder").Components.TryGet(out recorder);
                    valid = goalVisits >= 2 && startReturns >= 2 && Vector3D.Distance(pb.GetPosition(), motionStart) < 20 && speed < 1 && !recorder.IsPlaying && !recorder.RepeatPath && healthyThrust;
                }
                if (setup == "ai-motion-planet") valid = sawPlaying && maxTravel > 100 && protectedSamples >= 40 && protectedClearanceMin > 180 && clearance > 85 && clearance < 130 && distance < 25 && minClearance > 30 && initialAlignmentAngle > 25 && alignmentAngle < 5 && naturalGravity > .1 && speed < 1 && healthyThrust;
                if (setup == "ai-motion-obstacle") valid &= maxTravel > 250 && sawObstacleStop && obstacleCleared && minObstacleDistance > 25 && !obstacleOverlap;
                if (setup == "ai-motion-target-loss") {
                    var blocks = Blocks();
                    valid = sawEnemy && enemyRemoved && !((IMyDefensiveCombatBlock)blocks.Single(b => b.CustomName == "EC Drone Defensive")).SearchEnemyComponent.FoundEnemyId.HasValue && !((IMyOffensiveCombatBlock)blocks.Single(b => b.CustomName == "EC Drone Offensive")).SearchEnemyComponent.FoundEnemyId.HasValue;
                }
                passed = valid;
                detail += " Motion: " + json.Serialize(new { maxTravel, maxSpeed, distance, speed, sawPlaying, sawFleeing, sawEnemy, sawStopped, healthyThrust });
                if (setup == "ai-motion-repeat" || setup == "ai-motion-planet") detail += " Extended: " + json.Serialize(new { goalVisits, startReturns, clearance, minClearance, maxClearance, initialAlignmentAngle, alignmentAngle, naturalGravity, protectedClearanceMin, protectedSamples });
                if (setup == "ai-motion-obstacle" || setup == "ai-motion-target-loss") detail += " Boundary: " + json.Serialize(new { maxLateralDeviation, minObstacleDistance, obstacleOverlap, obstacleCleared, sawObstacleStop, enemyRemoved });
            }
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
