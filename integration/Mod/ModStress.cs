using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRage.Game;
using VRage;
using VRage.ObjectBuilders;
using VRageMath;

namespace EasyCommands.Integration {
    public sealed partial class Harness {
        IMyProgrammableBlock[] stressBlocks;
        IMyProgrammableBlock stressPeer;
        int stressPhase, stressTick, stressWait;
        int[] stressGeneration, stressMessages, stressPokes, stressChains, stressCount;
        bool[] stressReady;
        double[] stressPreviousTotal;
        long[] stressPreviousCalls;
        readonly Stopwatch stressClock = Stopwatch.StartNew();
        double stressLastWall;
        readonly List<object> stressPhases = new List<object>();
        readonly List<double> stressFrameTimes = new List<double>();
        readonly List<double> stressWallTimes = new List<double>();
        long stressCalls;

        void CreateStressFixtures() {
            long owner = Sandbox.Game.World.MySession.Static.Players.CreateNewIdentity("EC Stress Owner", addToNpcs: true).IdentityId;
            int rigs = (request.StressBlocks + 9) / 10;
            for (int rig = 0; rig <= rigs; rig++) {
                var grid = new MyObjectBuilder_CubeGrid {
                    GridSizeEnum = MyCubeSize.Large, IsStatic = true, CreatePhysics = true,
                    PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                    DisplayName = "EC Stress Rig " + rig,
                    PositionAndOrientation = new MyPositionAndOrientation(new Vector3D(-100, 0, -100 - rig * 100), Vector3.Forward, Vector3.Up)
                };
                for (int x = 0; x < 20; x++) for (int z = 0; z < 3; z++)
                    grid.CubeBlocks.Add(new MyObjectBuilder_CubeBlock { SubtypeName = "LargeBlockArmorBlock", Min = new Vector3I(x, 0, z) });
                Add(grid, "BatteryBlock", "LargeBlockBatteryBlock", "Stress Battery " + rig, 0).Min = new Vector3I(0, 1, 1);
                for (int slot = 0; slot < (rig == rigs ? 1 : Math.Min(10, request.StressBlocks - rig * 10)); slot++) {
                    string name = rig == rigs ? "Stress Peer" : "Stress " + (rig * 10 + slot);
                    Add(grid, "MyProgrammableBlock", "LargeProgrammableBlock", name, 2 + slot).Min = new Vector3I(2 + slot, 1, 1);
                }
                Add(grid, "RadioAntenna", "LargeBlockRadioAntenna", "Stress Antenna " + rig, 16).Min = new Vector3I(16, 1, 1);
                foreach (var block in grid.CubeBlocks.OfType<MyObjectBuilder_TerminalBlock>()) block.Owner = owner;
                MyAPIGateway.Entities.RemapObjectBuilder(grid);
                MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(grid);
            }
        }

        // Test-only instrumentation. LastRunTimeMs includes the previous synchronous
        // Main call, including the mod delegate. Both variants get identical overhead.
        string StressInstrument(string source) {
            var pattern = new Regex(@"public\s+void\s+Main\s*\([^)]*\)\s*\{");
            if (pattern.Matches(source).Count != 1 || source.Contains("__ecStressCalls")) throw new Exception("Unexpected PB Main shape.");
            string entry = " __ecStressCalls++; __ecStressTotal += Runtime.LastRunTimeMs; Storage = __ecStressCalls.ToString() + \"|\" + __ecStressTotal.ToString(\"R\",System.Globalization.CultureInfo.InvariantCulture); ";
            return "long __ecStressCalls; double __ecStressTotal;\n" + pattern.Replace(source, m => m.Value + entry);
        }
        int StressSeed(int i) { return 1000 + i * 97 + stressGeneration[i] * 100000; }
        int StressDelta(int i) { return i % 5 == 0 ? 24 + i : i % 5 == 1 ? 7 : i % 5 == 2 ? 21 : i % 5 == 3 ? 9 : 7; }
        int StressExpected(int i, int count) { return StressSeed(i) + (i % 5 == 1 ? count / 2 * 7 + count % 2 * 3 : count * StressDelta(i)); }
        string StressTag(int i) { return "EC_STRESS_" + i + "_" + stressGeneration[i]; }
        string StressProgram(int i) {
            var s = new StringBuilder();
            int seed = StressSeed(i), gen = stressGeneration[i];
            s.Append(":main\nset global count to 0\nset global checksum to " + seed + "\nset global sentinel to " + seed + "\nset global workSum to 0\nset global messages to 0\nset global pokes to 0\nset global chains to 0\nlisten " + StressTag(i) + "\nprint 'READY|" + i + "|" + gen + "'\nwhile true\n  set global workSum to 0\n");
            switch (i % 5) {
                case 0: for (int n = 0; n < StressDelta(i); n++) s.Append("  set global workSum to workSum + 1\n"); break;
                case 1: s.Append("  if count % 2 = 0\n    set global workSum to 3\n  else\n    set global workSum to 4\n"); break;
                case 2: s.Append("  for each item in [1,2,3,4,5,6]\n    set global workSum to workSum + item\n"); break;
                case 3: s.Append("  bump\n  bump\n  bump\n"); break;
                case 4: s.Append("  await\n    async worker\n  set global workSum to workSum + 4\n"); break;
            }
            s.Append("  set global checksum to checksum + workSum\n  set global count to count + 1\n  print 'STATE|" + i + "|" + gen + "|' + count + '|' + checksum\n");
            s.Append(":bump\nset global workSum to workSum + 3\n:worker\nwait 0.02 seconds\nset global workSum to workSum + 3\n:incoming\nset global messages to messages + 1\n:poke\nset global pokes to pokes + 1\n:chain\nset global chains to chains + 1\n");
            int peer = i % 10 == 0 && i + 1 < request.StressBlocks ? i + 1 : i;
            s.Append("tell 'Stress " + peer + "' program to run 'poke'\nif sentinel != " + seed + "\n  print 'STRESS_FAIL_CONTEXT'\n");
            s.Append(":report\nprint 'REPORT|" + i + "|" + gen + "|' + count + '|' + checksum + '|' + messages + '|' + pokes + '|' + chains + '|' + sentinel + '|'\n");
            // Unused parsed functions make source loading significant and varied.
            for (int f = 0; f < 20 + (i % 5) * 3 + gen * 20; f++)
                s.Append(":unused" + f + "\nset temporary to " + f + "\nset temporary to temporary + 2\nset temporary to temporary * 3\nprint temporary\n");
            // Avoid reserved language words (notably the "count" aggregator).
            string program = Regex.Replace(s.ToString(), @"\b(count|checksum|sentinel|workSum|messages|pokes|chains|temporary)\b", m => "ec" + m.Value);
            Directory.CreateDirectory(Path.Combine(root, "stress-programs"));
            File.WriteAllText(Path.Combine(root, "stress-programs", i + "-" + gen + ".ec"), program);
            return program;
        }
        string StressOutput(int i) { return stressBlocks[i].DetailedInfo ?? ""; }
        void StressCheck(int i) {
            string output = StressOutput(i);
            if (stressBlocks[i].HasCompileErrors || output.Contains("Exception Occurred:") || output.Contains("STRESS_FAIL") || output.Contains("bridge stopped") || output.Contains("Script execution terminated"))
                throw new Exception("Stress PB " + i + ": " + output);
            int gen = stressGeneration[i];
            if (output.Contains("READY|" + i + "|" + gen)) stressReady[i] = true;
            var match = Regex.Match(output, @"STATE\|" + i + @"\|" + gen + @"\|(\d+)\|(\d+)");
            if (!match.Success) return;
            stressReady[i] = true;
            int count = Int32.Parse(match.Groups[1].Value), checksum = Int32.Parse(match.Groups[2].Value);
            if (count < stressCount[i] || checksum != StressExpected(i, count))
                throw new Exception("Checksum/progress mismatch PB " + i + ": " + match.Value);
            stressCount[i] = count;
        }
        void StressNext(int phase) { stressPhase = phase; stressWait = 0; }
        void StressSample() {
            double total = 0, wall = stressClock.Elapsed.TotalMilliseconds;
            long calls = 0;
            for (int i = 0; i < stressBlocks.Length; i++) {
                string[] storage = (stressBlocks[i].StorageData ?? "").Split('|');
                if (storage.Length != 2) continue;
                long count = Int64.Parse(storage[0]);
                double duration = Double.Parse(storage[1], CultureInfo.InvariantCulture);
                total += duration - stressPreviousTotal[i]; calls += count - stressPreviousCalls[i];
                stressPreviousTotal[i] = duration; stressPreviousCalls[i] = count;
            }
            if (stressWait > 1) {
                stressFrameTimes.Add(total); stressWallTimes.Add(wall - stressLastWall); stressCalls += calls;
                File.AppendAllText(Path.Combine(root, "stress-metrics.jsonl"), json.Serialize(new { Phase = stressPhase, Tick = stressTick, MainMs = total, Calls = calls, WallMs = wall - stressLastWall }) + Environment.NewLine);
            }
            stressLastWall = wall;
        }
        static double StressPercentile(List<double> values, double q) {
            var ordered = values.OrderBy(v => v).ToArray();
            return ordered.Length == 0 ? 0 : ordered[(int)Math.Round((ordered.Length - 1) * q)];
        }
        void StressSavePhase(string name) {
            var row = new { Name = name, Blocks = stressBlocks.Length, Frames = stressFrameTimes.Count, Calls = stressCalls,
                TotalMainMs = stressFrameTimes.Sum(), MeanMainMs = stressFrameTimes.DefaultIfEmpty().Average(),
                MedianMainMs = StressPercentile(stressFrameTimes, .5), P95MainMs = StressPercentile(stressFrameTimes, .95), MaxMainMs = stressFrameTimes.DefaultIfEmpty().Max(),
                FramesOver16Ms = stressFrameTimes.Count(v => v > 1000.0 / 60),
                ObservedFramesPerSecond = stressWallTimes.Sum() == 0 ? 0 : 1000.0 * stressWallTimes.Count / stressWallTimes.Sum(),
                MinCompletedCycles = stressCount.Min(), MaxCompletedCycles = stressCount.Max() };
            stressPhases.Add(row);
            File.WriteAllText(Path.Combine(root, "stress-summary.json"), json.Serialize(stressPhases));
            stressFrameTimes.Clear(); stressWallTimes.Clear(); stressCalls = 0;
        }
        void StressInject() {
            int target = stressTick % stressBlocks.Length;
            if (stressReady[target]) { stressPeer.Run(StressTag(target)); stressMessages[target]++; }
            if (stressTick % 20 == 0 && stressReady[target]) { stressBlocks[target].Run("poke"); stressPokes[target]++; StressCheck(target); }
            if (stressTick % 120 == 0) for (int i = 0; i + 1 < stressBlocks.Length; i += 10)
                if (stressReady[i] && stressReady[i + 1]) {
                    stressBlocks[i].Run("chain"); stressChains[i]++; stressPokes[i + 1]++;
                    StressCheck(i); StressCheck(i + 1);
                }
        }
        void TickStress() {
            stressTick++; stressWait++;
            if (stressBlocks == null) {
                var all = Blocks(); int n = request.StressBlocks;
                stressBlocks = Enumerable.Range(0, n).Select(i => all.OfType<IMyProgrammableBlock>().Single(b => b.CustomName == "Stress " + i)).ToArray();
                stressPeer = all.OfType<IMyProgrammableBlock>().Single(b => b.CustomName == "Stress Peer");
                foreach (var antenna in all.OfType<Sandbox.ModAPI.Ingame.IMyRadioAntenna>()) { antenna.Enabled = true; antenna.EnableBroadcasting = true; antenna.Radius = 1000; }
                stressGeneration = new int[n]; stressMessages = new int[n]; stressPokes = new int[n]; stressChains = new int[n]; stressCount = new int[n]; stressReady = new bool[n];
                stressPreviousTotal = new double[n]; stressPreviousCalls = new long[n];
                string instrumented = StressInstrument(request.Script);
                File.WriteAllText(Path.Combine(root, "stress-instrumented.cs.txt"), instrumented);
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < n; i++) {
                    File.WriteAllText(Path.Combine(root, "stress-progress.json"), json.Serialize(new { Phase = "installing", Completed = i, Blocks = n, ElapsedMs = sw.Elapsed.TotalMilliseconds }));
                    var block = stressBlocks[i]; block.Enabled = false; block.CustomData = StressProgram(i); block.StorageData = ""; block.ProgramData = instrumented;
                    if (block.HasCompileErrors) throw new Exception("Stress compile failed: " + block.DetailedInfo);
                }
                File.WriteAllText(Path.Combine(root, "stress-install.json"), json.Serialize(new { Blocks = n, InstrumentedCharacters = instrumented.Length, InstallMs = sw.Elapsed.TotalMilliseconds }));
                stressPeer.ProgramData = "public void Main(string a){if(a.Length>0)IGC.SendBroadcastMessage(a,\"incoming\");}";
                if (stressPeer.HasCompileErrors) throw new Exception("Stress peer compile failed: " + stressPeer.DetailedInfo);
                StressNext(1); return;
            }
            foreach (int i in Enumerable.Range(0, stressBlocks.Length)) StressCheck(i);
            if (stressTick % 60 == 0) File.WriteAllText(Path.Combine(root, "stress-progress.json"), json.Serialize(new { Phase = stressPhase, Wait = stressWait, Ready = stressReady, Cycles = stressCount }));
            if (stressWait > request.StressTicks + 3000) throw new Exception("Stress timeout phase " + stressPhase + ": " + String.Join("\n", stressBlocks.Select(b => b.CustomName + ": " + b.DetailedInfo)));
            switch (stressPhase) {
                case 1: // Let the server recover from compilation; idle baseline.
                    StressSample();
                    if (stressWait < 180) return;
                    StressSavePhase("idle");
                    for (int i = 0; i < stressBlocks.Length; i++) { stressBlocks[i].Enabled = true; stressBlocks[i].Run(""); }
                    StressNext(2); break;
                case 2:
                    StressSample();
                    if (!stressReady.All(v => v)) return;
                    StressSavePhase("initial-parse-and-start"); StressNext(7); break;
                case 7:
                    StressSample(); StressInject();
                    if (stressWait < 180) return;
                    StressSavePhase("warmup-first-messages-and-cross-pb-calls"); StressNext(3); break;
                case 3:
                    StressSample(); StressInject();
                    if (stressWait < request.StressTicks) return;
                    StressSavePhase("concurrent-steady");
                    for (int i = 0; i < stressBlocks.Length; i += 5) {
                        stressGeneration[i]++; stressMessages[i] = stressPokes[i] = stressChains[i] = stressCount[i] = 0; stressReady[i] = false;
                        stressBlocks[i].CustomData = StressProgram(i);
                    }
                    StressNext(4); break;
                case 4:
                    StressSample(); StressInject();
                    if (!stressReady.All(v => v)) return;
                    StressSavePhase("reload-20-percent-while-running"); StressNext(5); break;
                case 5:
                    StressSample(); StressInject();
                    if (stressWait < request.StressTicks / 2) return;
                    StressSavePhase("steady-after-reload"); StressNext(6); break;
                case 6:
                    if (stressWait < 120) return;
                    for (int i = 0; i < stressBlocks.Length; i++) {
                        stressBlocks[i].Run("report");
                        string output = StressOutput(i);
                        var m = Regex.Match(output, @"REPORT\|" + i + @"\|" + stressGeneration[i] + @"\|(\d+)\|(\d+)\|(\d+)\|(\d+)\|(\d+)\|(\d+)\|");
                        if (!m.Success) throw new Exception("Missing report PB " + i + ": " + output);
                        var v = Enumerable.Range(1, 6).Select(k => Int32.Parse(m.Groups[k].Value)).ToArray();
                        bool pass = v[0] >= 10 && v[1] == StressExpected(i, v[0]) && v[2] == stressMessages[i] && v[3] == stressPokes[i] && v[4] == stressChains[i] && v[5] == StressSeed(i);
                        results.Add(new Result { Name = "stress-pb-" + i, Passed = pass, Ticks = stressTick, Detail = m.Value + " expected messages/pokes/chains=" + stressMessages[i] + "/" + stressPokes[i] + "/" + stressChains[i] });
                        stressBlocks[i].Enabled = false;
                    }
                    File.WriteAllText(Path.Combine(root, "stress-final-output.json"), json.Serialize(stressBlocks.Select(b => new { Name = b.CustomName, Output = b.DetailedInfo })));
                    Finish(); break;
            }
        }
    }
}
