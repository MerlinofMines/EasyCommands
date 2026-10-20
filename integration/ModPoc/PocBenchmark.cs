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
        const string EmptyPocScript = "public void Main(string argument) { Echo(\"POC_READY\"); }";
        IMyProgrammableBlock[] pocBlocks;
        IMyProgrammableBlock pocPeer;
        int pocJob, pocTick, pocPhase, pocWait;
        string pocVariant;
        Stopwatch pocReady;
        double pocInstall, pocFirst;
        bool pocSawA, pocSawB;
        string pocDisabledOutput;
        readonly List<string> pocJobs = new List<string>();

        void CreatePocFixtures() {
            long owner = Sandbox.Game.World.MySession.Static.Players.CreateNewIdentity("EC POC Radio Owner", addToNpcs: true).IdentityId;
            for (int rig = 0; rig < 2; rig++) {
                var grid = new MyObjectBuilder_CubeGrid {
                    GridSizeEnum = MyCubeSize.Large, IsStatic = true, CreatePhysics = true,
                    PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                    DisplayName = "EC POC Rig " + rig,
                    PositionAndOrientation = new MyPositionAndOrientation(new Vector3D(-100, 0, -100 - rig * 100), Vector3.Forward, Vector3.Up)
                };
                for (int x = 0; x < 20; x++)
                    for (int z = 0; z < 3; z++)
                        grid.CubeBlocks.Add(new MyObjectBuilder_CubeBlock { SubtypeName = "LargeBlockArmorBlock", Min = new Vector3I(x, 0, z) });
                Add(grid, "BatteryBlock", "LargeBlockBatteryBlock", "POC Battery " + rig, 0).Min = new Vector3I(0, 1, 1);
                for (int i = 0; i < (rig == 0 ? 10 : 1); i++)
                    Add(grid, "MyProgrammableBlock", "LargeProgrammableBlock", rig == 0 ? "POC " + i : "POC Radio Peer", 2 + i).Min = new Vector3I(2 + i, 1, 1);
                Add(grid, "RadioAntenna", "LargeBlockRadioAntenna", "POC Antenna " + rig, 16).Min = new Vector3I(16, 1, 1);
                foreach (var block in grid.CubeBlocks.OfType<MyObjectBuilder_TerminalBlock>()) block.Owner = owner;
                MyAPIGateway.Entities.RemapObjectBuilder(grid);
                MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(grid);
            }
        }

        void PocLog(object value) {
            File.AppendAllText(Path.Combine(root, "measurements.jsonl"), json.Serialize(value) + Environment.NewLine);
        }
        void PocPass(string name, string detail) {
            results.Add(new Result { Name = name, Passed = true, Detail = detail, Ticks = pocWait });
        }
        string PocOutput(int i) { return pocBlocks[i].DetailedInfo ?? ""; }
        void PocInstall(int i, string source, string commands) {
            var block = pocBlocks[i];
            block.Enabled = true;
            block.CustomData = commands;
            block.StorageData = "";
            block.ProgramData = source;
            if (block.HasCompileErrors) throw new Exception("POC PB compile failed: " + block.DetailedInfo);
        }
        void PocBridge(int i, string commands) { PocInstall(i, request.Script, commands); pocBlocks[i].Run(""); }
        void PocPhase(int next) { pocPhase = next; pocWait = 0; pocSawA = pocSawB = false; }

        void TickPoc() {
            pocTick++;
            if (pocBlocks == null) {
                var all = Blocks();
                pocBlocks = Enumerable.Range(0, 10).Select(i => all.OfType<IMyProgrammableBlock>().Single(b => b.CustomName == "POC " + i)).ToArray();
                pocPeer = all.OfType<IMyProgrammableBlock>().Single(b => b.CustomName == "POC Radio Peer");
                foreach (var antenna in all.OfType<Sandbox.ModAPI.Ingame.IMyRadioAntenna>()) { antenna.Enabled = true; antenna.EnableBroadcasting = true; antenna.Radius = 1000; }
                for (int round = 0; round < 6; round++) {
                    var variants = round % 2 == 0 ? new[] { "empty", "bridge", "full" } : new[] { "full", "bridge", "empty" };
                    foreach (var variant in variants) pocJobs.Add(variant);
                }
                File.WriteAllText(Path.Combine(root, "poc-progress.json"), "{\"phase\":\"benchmark-start\"}");
                if (request.PocSkipSamples) PocPhase(1);
            }
            if (pocPhase == 0) { TickPocSamples(); return; }
            if (++pocWait > 900) throw new Exception("POC phase " + pocPhase + " timeout: A=" + PocOutput(0) + " B=" + PocOutput(1) + " peer=" + pocPeer.DetailedInfo);
            switch (pocPhase) {
                case 1:
                    PocBridge(0, ":main\nset global value to \"POC_A_OK\"\nprint \"POC_A_READY\"\nwait until false\n:outer\ntell \"POC 1\" program to run \"inner\"\nprint value\n:reentry\nprint \"POC_REENTRY\"");
                    PocBridge(1, ":main\nset global value to \"POC_B_OK\"\nprint \"POC_B_READY\"\nwait until false\n:inner\ntell \"POC 0\" program to run \"reentry\"\nprint value");
                    PocPhase(2); break;
                case 2:
                    pocSawA |= PocOutput(0).Contains("POC_A_READY"); pocSawB |= PocOutput(1).Contains("POC_B_READY");
                    if (!pocSawA || !pocSawB) break;
                    pocBlocks[0].Run("outer");
                    if (!PocOutput(0).Contains("POC_A_OK") || !PocOutput(1).Contains("POC_B_OK") || PocOutput(0).Contains("\nPOC_REENTRY\n"))
                        throw new Exception("Nested/context test failed: " + PocOutput(0) + " / " + PocOutput(1));
                    PocPass("instance-isolation-and-nested-run", "A and B retained separate global variables; guarded A-to-B-to-A call preserved A's context.");
                    pocBlocks[0].CustomData = "print \"POC_SOURCE_EDIT\"";
                    pocBlocks[0].Run(""); PocPhase(3); break;
                case 3:
                    if (!PocOutput(0).Contains("POC_SOURCE_EDIT")) break;
                    PocPass("custom-data-edit", "New source reparsed and executed without changing PB source.");
                    PocBridge(0, "print \"POC_SOURCE_EDIT\""); PocPhase(4); break;
                case 4:
                    if (!PocOutput(0).Contains("POC_SOURCE_EDIT")) break;
                    PocPass("bridge-recompile", "Fresh bridge registration rebuilt its runtime and executed source again.");
                    PocBridge(0, "await\n  async work\nwait 0.1 seconds\nprint \"POC_FLOW_OK\"\n:work\nwait 0.1 seconds\nprint \"POC_ASYNC_OK\"");
                    PocPhase(5); break;
                case 5:
                    pocSawA |= PocOutput(0).Contains("POC_ASYNC_OK"); pocSawB |= PocOutput(0).Contains("POC_FLOW_OK");
                    if (!pocSawA || !pocSawB) break;
                    PocPass("wait-function-async", "Function execution, await/async queues and timed waits completed.");
                    PocBridge(0, "set n to 0\nwhile true\n  n++\n  print n"); PocPhase(6); break;
                case 6:
                    if (pocWait < 90) break;
                    pocBlocks[0].Enabled = false; pocDisabledOutput = PocOutput(0); PocPhase(7); break;
                case 7:
                    if (pocWait < 10) break;
                    if (PocOutput(0) != pocDisabledOutput) throw new Exception("Disabled PB continued updating.");
                    pocBlocks[0].Enabled = true; PocPhase(8); break;
                case 8:
                    if (pocWait < 10) break;
                    if (PocOutput(0) == pocDisabledOutput) throw new Exception("Re-enabled PB did not resume.");
                    PocPass("disable-and-resume", "Output froze while disabled and advanced after re-enable.");
                    pocBlocks[0].Enabled = false; PocBridge(0, "listen POC_REQUEST\nprint \"POC_LISTENING\"\nwait until false\n:reply\nsend \"POC_REPLY_PAYLOAD\" to POC_REPLY");
                    pocPeer.Enabled = true; pocPeer.CustomData = "";
                    pocPeer.ProgramData = "IMyBroadcastListener listener; public Program(){listener=IGC.RegisterBroadcastListener(\"POC_REPLY\");Runtime.UpdateFrequency=UpdateFrequency.Update1;} public void Main(string a){if(a==\"send\")IGC.SendBroadcastMessage(\"POC_REQUEST\",\"reply\");if(listener.HasPendingMessage){var m=listener.AcceptMessage();Echo(\"POC_REPLY_RECEIVED:\"+m.Data);Runtime.UpdateFrequency=UpdateFrequency.None;}}";
                    if (pocPeer.HasCompileErrors) throw new Exception("Radio peer compile failed: " + pocPeer.DetailedInfo);
                    PocPhase(9); break;
                case 9:
                    if (!PocOutput(0).Contains("POC_LISTENING")) break;
                    pocPeer.Run("send"); PocPhase(10); break;
                case 10:
                    if (!(pocPeer.DetailedInfo ?? "").Contains("POC_REPLY_RECEIVED:POC_REPLY_PAYLOAD")) break;
                    PocPass("vanilla-igc-cross-grid", "Unmodified IGC peer sent a function request to the mod engine and received its broadcast reply on a separate powered antenna grid, 100m away.");
                    pocBlocks[0].Enabled = false; pocPeer.Enabled = false;
                    PocPhase(11); break;
                case 11:
                    StartPocBatch("bridge"); PocPhase(12); break;
                case 12:
                    if (!pocBlocks.All(b => (b.DetailedInfo ?? "").Contains("POC_BATCH_READY"))) break;
                    EndPocBatch(); PocPhase(request.PocSkipFullBatch ? 15 : 13); break;
                case 13:
                    StartPocBatch("full"); PocPhase(14); break;
                case 14:
                    if (!pocBlocks.All(b => (b.DetailedInfo ?? "").Contains("POC_BATCH_READY"))) break;
                    EndPocBatch(); PocPhase(15); break;
                case 15:
                    PocBridge(9, "print \"POC_REMOVAL_READY\""); PocPhase(16); break;
                case 16:
                    if (!(pocBlocks[9].DetailedInfo ?? "").Contains("POC_REMOVAL_READY")) break;
                    pocBlocks[9].Close(); PocBridge(0, "print \"POC_AFTER_REMOVAL\""); PocPhase(17); break;
                case 17:
                    if (!PocOutput(0).Contains("POC_AFTER_REMOVAL")) break;
                    PocPass("block-removal", "Removing an active registered PB left another runtime operational; memory reclamation was not profiled.");
                    PocBridge(0, ":main\nprint 'POC_RUN_READY_A'\nwait until false\n:invoke\ntell 'POC 1' program to run 'noop'\nprint 'POC_RUN_FINISHED'");
                    PocBridge(1, ":main\nprint 'POC_RUN_READY_B'\nwait until false\n:noop\nprint 'POC_RUN_TARGET'");
                    PocPhase(18); break;
                case 18:
                    pocSawA |= PocOutput(0).Contains("POC_RUN_READY_A"); pocSawB |= PocOutput(1).Contains("POC_RUN_READY_B");
                    if (!pocSawA || !pocSawB) break;
                    pocBlocks[1].Enabled = false;
                    pocBlocks[0].Run("invoke");
                    if (!PocOutput(0).Contains("POC_RUN_FINISHED") || PocOutput(1).Contains("POC_RUN_TARGET"))
                        throw new Exception("Disabled Run target changed language behavior: " + PocOutput(0) + " / " + PocOutput(1));
                    PocPass("disabled-run-target", "Rejected target did not execute and the calling command continued.");
                    pocBlocks[1].Enabled = true;
                    pocBlocks[0].Run("invoke");
                    if (!PocOutput(0).Contains("POC_RUN_FINISHED") || !PocOutput(1).Contains("POC_RUN_TARGET"))
                        throw new Exception("Re-enabled target did not execute: " + PocOutput(0) + " / " + PocOutput(1));
                    PocPass("enabled-run-target", "Re-enabled target executed normally through the mod Run adapter.");
                    pocBlocks[0].Enabled = pocBlocks[1].Enabled = false;
                    Finish(); break;
            }
            File.WriteAllText(Path.Combine(root, "poc-progress.json"), json.Serialize(new { Phase = pocPhase, WaitTicks = pocWait, A = PocOutput(0), B = PocOutput(1), Peer = pocPeer.DetailedInfo }));
        }

        void TickPocSamples() {
            if (pocJob >= pocJobs.Count) { PocPhase(1); return; }
            if (pocReady == null) {
                pocVariant = pocJobs[pocJob];
                var source = pocVariant == "empty" ? EmptyPocScript : pocVariant == "bridge" ? request.Script : request.FullScript;
                var sw = Stopwatch.StartNew();
                PocInstall(0, source, "print \"POC_READY\"");
                pocInstall = sw.Elapsed.TotalMilliseconds;
                pocReady = Stopwatch.StartNew(); sw.Restart();
                pocBlocks[0].Run(""); pocFirst = sw.Elapsed.TotalMilliseconds;
                pocWait = 0;
                return;
            }
            pocWait++;
            var output = PocOutput(0);
            if (output.Contains("Exception") || output.Contains("bridge stopped") || output.Contains("unavailable")) throw new Exception("Benchmark runtime failed: " + output);
            if (!output.Contains("POC_READY")) {
                if (pocWait > 600) throw new Exception("Benchmark readiness timed out: " + output);
                return;
            }
            pocReady.Stop();
            var steady = new List<double>();
            for (int i = 0; i < 20; i++) {
                var sw = Stopwatch.StartNew(); pocBlocks[0].Run("print \"POC_STEADY\""); sw.Stop(); steady.Add(sw.Elapsed.TotalMilliseconds);
                var marker = pocVariant == "empty" ? "POC_READY" : "POC_STEADY";
                if (!PocOutput(0).Contains(marker)) throw new Exception("Steady call failed: " + PocOutput(0));
            }
            PocLog(new { Kind = "sample", Variant = pocVariant, SampleOrder = pocJob, WarmIndex = pocJob / 3,
                FirstInProcessForVariant = pocJob < 3, InstallMs = pocInstall, FirstCallMs = pocFirst,
                ReadyAfterCallMs = pocReady.Elapsed.TotalMilliseconds, ReadyTicks = pocWait, SteadyCallMs = steady });
            PocPass("sample-" + pocJob + "-" + pocVariant, "Install/first-call/readiness and 20 steady calls recorded.");
            pocBlocks[0].Enabled = false; pocJob++; pocReady = null;
        }
        void StartPocBatch(string variant) {
            pocVariant = variant; pocInstall = pocFirst = 0; pocReady = Stopwatch.StartNew();
            var source = variant == "bridge" ? request.Script : request.FullScript;
            for (int i = 0; i < pocBlocks.Length; i++) {
                var sw = Stopwatch.StartNew(); PocInstall(i, source, "print \"POC_BATCH_READY\""); pocInstall += sw.Elapsed.TotalMilliseconds;
                sw.Restart(); pocBlocks[i].Run(""); pocFirst += sw.Elapsed.TotalMilliseconds;
            }
        }
        void EndPocBatch() {
            pocReady.Stop(); PocLog(new { Kind = "batch", Variant = pocVariant, Blocks = pocBlocks.Length, InstallSumMs = pocInstall, FirstCallSumMs = pocFirst, AllReadyMs = pocReady.Elapsed.TotalMilliseconds, ReadyTicks = pocWait });
            PocPass("batch-10-" + pocVariant, "All ten PBs completed their program; timings recorded.");
            foreach (var block in pocBlocks) block.Enabled = false;
        }
    }
}
