using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Sandbox.ModAPI;
using VRage;
using VRage.Game;
using VRage.ObjectBuilders;
using VRageMath;
using Sandbox.Common.ObjectBuilders;

namespace EasyCommands.Integration {
    public sealed partial class Harness {
        const string EmptyModScript = "public void Main(string argument) { Echo(\"EC_MOD_READY\"); }";
        IMyProgrammableBlock[] modBlocks;
        IMyProgrammableBlock modPeer;
        int modJob, modTick, modPhase, modWait;
        string modVariant;
        Stopwatch modReady;
        double modInstall, modFirst;
        bool modSawA, modSawB;
        string modDisabledOutput;
        readonly List<string> modJobs = new List<string>();

        void CreateModFixtures() {
            long owner = Sandbox.Game.World.MySession.Static.Players.CreateNewIdentity("EC EC_MOD Radio Owner", addToNpcs: true).IdentityId;
            for (int rig = 0; rig < 3; rig++) {
                var grid = new MyObjectBuilder_CubeGrid {
                    GridSizeEnum = MyCubeSize.Large, IsStatic = true, CreatePhysics = true,
                    PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                    DisplayName = "EC EC_MOD Rig " + rig,
                    PositionAndOrientation = new MyPositionAndOrientation(new Vector3D(-100, 0, -100 - rig * 100), Vector3.Forward, Vector3.Up)
                };
                for (int x = 0; x < 20; x++)
                    for (int z = 0; z < 3; z++)
                        grid.CubeBlocks.Add(new MyObjectBuilder_CubeBlock { SubtypeName = "LargeBlockArmorBlock", Min = new Vector3I(x, 0, z) });
                Add(grid, "BatteryBlock", "LargeBlockBatteryBlock", "EC_MOD Battery " + rig, 0).Min = new Vector3I(0, 1, 1);
                for (int i = 0; i < (rig == 0 ? 10 : 1); i++)
                    Add(grid, "MyProgrammableBlock", "LargeProgrammableBlock", rig == 0 ? "EC_MOD " + i : rig == 1 ? "EC_MOD Radio Peer" : "EC_MOD Lifecycle Victim", 2 + i).Min = new Vector3I(2 + i, 1, 1);
                Add(grid, "RadioAntenna", "LargeBlockRadioAntenna", "EC_MOD Antenna " + rig, 16).Min = new Vector3I(16, 1, 1);
                if (rig == 2) Add(grid, "Warhead", "LargeWarhead", "EC_MOD Lifecycle Warhead", 4).Min = new Vector3I(4, 1, 1);
                foreach (var block in grid.CubeBlocks.OfType<MyObjectBuilder_TerminalBlock>()) block.Owner = owner;
                MyAPIGateway.Entities.RemapObjectBuilder(grid);
                MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(grid);
            }
        }

        void ModLog(object value) {
            File.AppendAllText(Path.Combine(root, "measurements.jsonl"), json.Serialize(value) + Environment.NewLine);
        }
        void ModPass(string name, string detail) {
            results.Add(new Result { Name = name, Passed = true, Detail = detail, Ticks = modWait });
        }
        string ModOutput(int i) { return modBlocks[i].DetailedInfo ?? ""; }
        void ModInstall(int i, string source, string commands) {
            var block = modBlocks[i];
            block.Enabled = true;
            block.CustomData = commands;
            block.StorageData = "";
            block.ProgramData = source;
            if (block.HasCompileErrors) throw new Exception("EC_MOD PB compile failed: " + block.DetailedInfo);
        }
        void ModBridge(int i, string commands) { ModInstall(i, request.Script, commands); modBlocks[i].Run(""); }
        void ModPhase(int next) { modPhase = next; modWait = 0; modSawA = modSawB = false; }

        void TickMod() {
            modTick++;
            if (modBlocks == null) {
                var all = Blocks();
                modBlocks = Enumerable.Range(0, 10).Select(i => all.OfType<IMyProgrammableBlock>().Single(b => b.CustomName == "EC_MOD " + i)).ToArray();
                modPeer = all.OfType<IMyProgrammableBlock>().Single(b => b.CustomName == "EC_MOD Radio Peer");
                foreach (var antenna in all.OfType<Sandbox.ModAPI.Ingame.IMyRadioAntenna>()) { antenna.Enabled = true; antenna.EnableBroadcasting = true; antenna.Radius = 1000; }
                for (int round = 0; round < 6; round++) {
                    var variants = round % 2 == 0 ? new[] { "empty", "bridge", "full" } : new[] { "full", "bridge", "empty" };
                    foreach (var variant in variants) modJobs.Add(variant);
                }
                File.WriteAllText(Path.Combine(root, "mod-progress.json"), "{\"phase\":\"benchmark-start\"}");
                if (request.SchedulingOnly) ModPhase(29);
                else if (request.LifecycleOnly) ModPhase(19);
                else if (request.SkipBenchmarkSamples) ModPhase(1);
            }
            if (modPhase == 0) { TickModSamples(); return; }
            if (modPhase >= 29) { TickModScheduling(); return; }
            if (modPhase >= 19) { TickModLifecycle(); return; }
            if (++modWait > 900) throw new Exception("EC_MOD phase " + modPhase + " timeout: A=" + ModOutput(0) + " B=" + ModOutput(1) + " peer=" + modPeer.DetailedInfo);
            switch (modPhase) {
                case 1:
                    ModBridge(0, ":main\nset global value to \"EC_MOD_A_OK\"\nprint \"EC_MOD_A_READY\"\nwait until false\n:outer\ntell \"EC_MOD 1\" program to run \"inner\"\nprint value\n:reentry\nprint \"EC_MOD_REENTRY\"");
                    ModBridge(1, ":main\nset global value to \"EC_MOD_B_OK\"\nprint \"EC_MOD_B_READY\"\nwait until false\n:inner\ntell \"EC_MOD 0\" program to run \"reentry\"\nprint value");
                    ModPhase(2); break;
                case 2:
                    modSawA |= ModOutput(0).Contains("EC_MOD_A_READY"); modSawB |= ModOutput(1).Contains("EC_MOD_B_READY");
                    if (!modSawA || !modSawB) break;
                    modBlocks[0].Run("outer");
                    if (!ModOutput(0).Contains("EC_MOD_A_OK") || !ModOutput(1).Contains("EC_MOD_B_OK") || ModOutput(0).Contains("\nEC_MOD_REENTRY\n"))
                        throw new Exception("Nested/context test failed: " + ModOutput(0) + " / " + ModOutput(1));
                    ModPass("instance-isolation-and-nested-run", "A and B retained separate global variables; guarded A-to-B-to-A call preserved A's context.");
                    modBlocks[0].CustomData = "print \"EC_MOD_SOURCE_EDIT\"";
                    modBlocks[0].Run(""); ModPhase(3); break;
                case 3:
                    if (!ModOutput(0).Contains("EC_MOD_SOURCE_EDIT")) break;
                    ModPass("custom-data-edit", "New source reparsed and executed without changing PB source.");
                    ModBridge(0, "print \"EC_MOD_SOURCE_EDIT\""); ModPhase(4); break;
                case 4:
                    if (!ModOutput(0).Contains("EC_MOD_SOURCE_EDIT")) break;
                    ModPass("bridge-recompile", "Fresh bridge registration rebuilt its runtime and executed source again.");
                    ModBridge(0, "await\n  async work\nwait 0.1 seconds\nprint \"EC_MOD_FLOW_OK\"\n:work\nwait 0.1 seconds\nprint \"EC_MOD_ASYNC_OK\"");
                    ModPhase(5); break;
                case 5:
                    modSawA |= ModOutput(0).Contains("EC_MOD_ASYNC_OK"); modSawB |= ModOutput(0).Contains("EC_MOD_FLOW_OK");
                    if (!modSawA || !modSawB) break;
                    ModPass("wait-function-async", "Function execution, await/async queues and timed waits completed.");
                    ModBridge(0, "set n to 0\nwhile true\n  n++\n  print n"); ModPhase(6); break;
                case 6:
                    if (modWait < 90) break;
                    modBlocks[0].Enabled = false; modDisabledOutput = ModOutput(0); ModPhase(7); break;
                case 7:
                    if (modWait < 10) break;
                    if (ModOutput(0) != modDisabledOutput) throw new Exception("Disabled PB continued updating.");
                    modBlocks[0].Enabled = true; ModPhase(8); break;
                case 8:
                    if (modWait < 10) break;
                    if (ModOutput(0) == modDisabledOutput) throw new Exception("Re-enabled PB did not resume.");
                    ModPass("disable-and-resume", "Output froze while disabled and advanced after re-enable.");
                    modBlocks[0].Enabled = false; ModBridge(0, "listen EC_MOD_REQUEST\nprint \"EC_MOD_LISTENING\"\nwait until false\n:reply\nsend \"EC_MOD_REPLY_PAYLOAD\" to EC_MOD_REPLY");
                    modPeer.Enabled = true; modPeer.CustomData = "";
                    modPeer.ProgramData = "IMyBroadcastListener listener; public Program(){listener=IGC.RegisterBroadcastListener(\"EC_MOD_REPLY\");Runtime.UpdateFrequency=UpdateFrequency.Update1;} public void Main(string a){if(a==\"send\")IGC.SendBroadcastMessage(\"EC_MOD_REQUEST\",\"reply\");if(listener.HasPendingMessage){var m=listener.AcceptMessage();Echo(\"EC_MOD_REPLY_RECEIVED:\"+m.Data);Runtime.UpdateFrequency=UpdateFrequency.None;}}";
                    if (modPeer.HasCompileErrors) throw new Exception("Radio peer compile failed: " + modPeer.DetailedInfo);
                    ModPhase(9); break;
                case 9:
                    if (!ModOutput(0).Contains("EC_MOD_LISTENING")) break;
                    modPeer.Run("send"); ModPhase(10); break;
                case 10:
                    if (!(modPeer.DetailedInfo ?? "").Contains("EC_MOD_REPLY_RECEIVED:EC_MOD_REPLY_PAYLOAD")) break;
                    ModPass("vanilla-igc-cross-grid", "Unmodified IGC peer sent a function request to the mod engine and received its broadcast reply on a separate powered antenna grid, 100m away.");
                    modBlocks[0].Enabled = false; modPeer.Enabled = false;
                    ModPhase(11); break;
                case 11:
                    StartModBatch("bridge"); ModPhase(12); break;
                case 12:
                    if (!modBlocks.All(b => (b.DetailedInfo ?? "").Contains("EC_MOD_BATCH_READY"))) break;
                    EndModBatch(); ModPhase(request.SkipStandaloneBatch ? 15 : 13); break;
                case 13:
                    StartModBatch("full"); ModPhase(14); break;
                case 14:
                    if (!modBlocks.All(b => (b.DetailedInfo ?? "").Contains("EC_MOD_BATCH_READY"))) break;
                    EndModBatch(); ModPhase(15); break;
                case 15:
                    ModBridge(9, "print \"EC_MOD_REMOVAL_READY\""); ModPhase(16); break;
                case 16:
                    if (!(modBlocks[9].DetailedInfo ?? "").Contains("EC_MOD_REMOVAL_READY")) break;
                    modBlocks[9].Close(); ModBridge(0, "print \"EC_MOD_AFTER_REMOVAL\""); ModPhase(17); break;
                case 17:
                    if (!ModOutput(0).Contains("EC_MOD_AFTER_REMOVAL")) break;
                    ModPass("block-removal", "Removing an active registered PB left another runtime operational; memory reclamation was not profiled.");
                    ModBridge(0, ":main\nprint 'EC_MOD_RUN_READY_A'\nwait until false\n:invoke\ntell 'EC_MOD 1' program to run 'noop'\nprint 'EC_MOD_RUN_FINISHED'");
                    ModBridge(1, ":main\nprint 'EC_MOD_RUN_READY_B'\nwait until false\n:noop\nprint 'EC_MOD_RUN_TARGET'");
                    ModPhase(18); break;
                case 18:
                    modSawA |= ModOutput(0).Contains("EC_MOD_RUN_READY_A"); modSawB |= ModOutput(1).Contains("EC_MOD_RUN_READY_B");
                    if (!modSawA || !modSawB) break;
                    modBlocks[1].Enabled = false;
                    modBlocks[0].Run("invoke");
                    if (!ModOutput(0).Contains("EC_MOD_RUN_FINISHED") || ModOutput(1).Contains("EC_MOD_RUN_TARGET"))
                        throw new Exception("Disabled Run target changed language behavior: " + ModOutput(0) + " / " + ModOutput(1));
                    ModPass("disabled-run-target", "Rejected target did not execute and the calling command continued.");
                    modBlocks[1].Enabled = true;
                    modBlocks[0].Run("invoke");
                    if (!ModOutput(0).Contains("EC_MOD_RUN_FINISHED") || !ModOutput(1).Contains("EC_MOD_RUN_TARGET"))
                        throw new Exception("Re-enabled target did not execute: " + ModOutput(0) + " / " + ModOutput(1));
                    ModPass("enabled-run-target", "Re-enabled target executed normally through the mod Run adapter.");
                    modBlocks[0].Enabled = modBlocks[1].Enabled = false;
                    ModPhase(19); break;
            }
            File.WriteAllText(Path.Combine(root, "mod-progress.json"), json.Serialize(new { Phase = modPhase, WaitTicks = modWait, A = ModOutput(0), B = ModOutput(1), Peer = modPeer.DetailedInfo }));
        }

        void TickModSamples() {
            if (modJob >= modJobs.Count) { ModPhase(1); return; }
            if (modReady == null) {
                modVariant = modJobs[modJob];
                var source = modVariant == "empty" ? EmptyModScript : modVariant == "bridge" ? request.Script : request.FullScript;
                var sw = Stopwatch.StartNew();
                ModInstall(0, source, "print \"EC_MOD_READY\"");
                modInstall = sw.Elapsed.TotalMilliseconds;
                modReady = Stopwatch.StartNew(); sw.Restart();
                modBlocks[0].Run(""); modFirst = sw.Elapsed.TotalMilliseconds;
                modWait = 0;
                return;
            }
            modWait++;
            var output = ModOutput(0);
            if (output.Contains("Exception") || output.Contains("bridge stopped") || output.Contains("unavailable")) throw new Exception("Benchmark runtime failed: " + output);
            if (!output.Contains("EC_MOD_READY")) {
                if (modWait > 600) throw new Exception("Benchmark readiness timed out: " + output);
                return;
            }
            modReady.Stop();
            var steady = new List<double>();
            for (int i = 0; i < 20; i++) {
                var sw = Stopwatch.StartNew(); modBlocks[0].Run("print \"EC_MOD_STEADY\""); sw.Stop(); steady.Add(sw.Elapsed.TotalMilliseconds);
                var marker = modVariant == "empty" ? "EC_MOD_READY" : "EC_MOD_STEADY";
                if (!ModOutput(0).Contains(marker)) throw new Exception("Steady call failed: " + ModOutput(0));
            }
            ModLog(new { Kind = "sample", Variant = modVariant, SampleOrder = modJob, WarmIndex = modJob / 3,
                FirstInProcessForVariant = modJob < 3, InstallMs = modInstall, FirstCallMs = modFirst,
                ReadyAfterCallMs = modReady.Elapsed.TotalMilliseconds, ReadyTicks = modWait, SteadyCallMs = steady });
            ModPass("sample-" + modJob + "-" + modVariant, "Install/first-call/readiness and 20 steady calls recorded.");
            modBlocks[0].Enabled = false; modJob++; modReady = null;
        }
        void StartModBatch(string variant) {
            modVariant = variant; modInstall = modFirst = 0; modReady = Stopwatch.StartNew();
            var source = variant == "bridge" ? request.Script : request.FullScript;
            for (int i = 0; i < modBlocks.Length; i++) {
                var sw = Stopwatch.StartNew(); ModInstall(i, source, "print \"EC_MOD_BATCH_READY\""); modInstall += sw.Elapsed.TotalMilliseconds;
                sw.Restart(); modBlocks[i].Run(""); modFirst += sw.Elapsed.TotalMilliseconds;
            }
        }
        void EndModBatch() {
            modReady.Stop(); ModLog(new { Kind = "batch", Variant = modVariant, Blocks = modBlocks.Length, InstallSumMs = modInstall, FirstCallSumMs = modFirst, AllReadyMs = modReady.Elapsed.TotalMilliseconds, ReadyTicks = modWait });
            ModPass("batch-10-" + modVariant, "All ten PBs completed their program; timings recorded.");
            foreach (var block in modBlocks) block.Enabled = false;
        }
    }
}
