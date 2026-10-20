using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sandbox.ModAPI;

namespace EasyCommands.Integration {
    public sealed partial class Harness {
        readonly int[] schedulePeriods = { 1, 10, 100 };
        readonly string[] scheduleNames = { "Update1", "Update10", "Update100", "None" };
        int[] scheduleStartCalls, scheduleLastCalls, scheduleLastTicks;
        readonly List<int>[] scheduleIntervals = { new List<int>(), new List<int>(), new List<int>() };
        int missingTicks, scheduleManualBaseline, outputCalls;
        readonly List<object> outputEvidence = new List<object>();
        string missingOutput;

        string ScheduleBridge(string frequency) {
            const string setting = "public UpdateFrequency updateFrequency = UpdateFrequency.Update1;";
            if (request.Script.Split(new[] { setting }, StringSplitOptions.None).Length != 2)
                throw new Exception("Expected one user-configurable updateFrequency field.");
            return request.Script.Replace(setting, "public UpdateFrequency updateFrequency = UpdateFrequency." + frequency + ";");
        }
        int ScheduleCalls(int i) {
            var states = ModPocSourceLoader.RuntimeMap("states");
            return states.Contains(pocBlocks[i].EntityId) ? ModPocSourceLoader.Calls(states[pocBlocks[i].EntityId]) : 0;
        }
        void TickPocScheduling() {
            if (++pocWait > 1800) throw new Exception("Scheduling timeout in phase " + pocPhase);
            switch (pocPhase) {
                case 29:
                    scheduleManualBaseline = ScheduleCalls(3);
                    for (int i = 0; i < 4; i++) PocInstall(i, ScheduleBridge(scheduleNames[i]), LifeProgram(800 + i));
                    PocPhase(30); break;
                case 30:
                    if (Enumerable.Range(0, 3).Any(i => LifeTicks(pocBlocks[i], 800 + i) < 2)) break;
                    scheduleStartCalls = Enumerable.Range(0, 3).Select(ScheduleCalls).ToArray();
                    scheduleLastCalls = (int[])scheduleStartCalls.Clone();
                    scheduleLastTicks = new int[3];
                    PocPhase(31); break;
                case 31:
                    for (int i = 0; i < 3; i++) {
                        int calls = ScheduleCalls(i);
                        if (calls == scheduleLastCalls[i]) continue;
                        LifeRequire(calls == scheduleLastCalls[i] + 1, "Unexpected multiple scheduled calls in one frame.");
                        if (scheduleLastTicks[i] != 0) scheduleIntervals[i].Add(pocWait - scheduleLastTicks[i]);
                        scheduleLastTicks[i] = pocWait; scheduleLastCalls[i] = calls;
                    }
                    LifeRequire(ScheduleCalls(3) == scheduleManualBaseline, "Manual-only bridge started automatically.");
                    if (pocWait < 600) break;
                    for (int i = 0; i < 3; i++) {
                        int observedCalls = ScheduleCalls(i) - scheduleStartCalls[i], expected = 600 / schedulePeriods[i];
                        LifeRequire(Math.Abs(observedCalls - expected) <= 1, "Requested " + scheduleNames[i] + " scheduled " + observedCalls + " calls, expected " + expected);
                        LifeRequire(scheduleIntervals[i].Count > 2 && scheduleIntervals[i].All(v => v == schedulePeriods[i]), "Wrong cadence for " + scheduleNames[i]);
                        var state = ModPocSourceLoader.RuntimeMap("states")[pocBlocks[i].EntityId];
                        var engine = state.GetType().GetField("Engine").GetValue(state);
                        LifeRequire(engine.GetType().GetField("updateFrequency").GetValue(engine).ToString() == scheduleNames[i], "Bridge did not forward updateFrequency to engine.");
                        PocPass("requested-interval-" + scheduleNames[i], observedCalls + " calls over 600 simulation ticks; every measured interval was " + schedulePeriods[i] + " ticks; engine setting matched.");
                        pocBlocks[i].Enabled = false;
                    }
                    for (int attempt = 0; attempt < 40 && LifeTicks(pocBlocks[3], 803) < 1; attempt++) pocBlocks[3].Run("");
                    LifeRequire(LifeTicks(pocBlocks[3], 803) >= 1, "Manual bridge did not execute explicit Run.");
                    scheduleLastCalls[0] = ScheduleCalls(3);
                    PocPhase(32); break;
                case 32:
                    LifeRequire(ScheduleCalls(3) == scheduleLastCalls[0], "None scheduled automatic calls after manual execution.");
                    if (pocWait < 120) break;
                    pocBlocks[3].Run("");
                    LifeRequire(ScheduleCalls(3) == scheduleLastCalls[0] + 1, "None ignored a subsequent explicit Run.");
                    PocPass("requested-interval-None", "No automatic calls before or after manual execution; explicit Run still worked.");
                    pocBlocks[3].Enabled = false;
                    File.WriteAllText(Path.Combine(root, "scheduling.json"), json.Serialize(new { Periods = schedulePeriods, Intervals = scheduleIntervals }));
                    PocPhase(33); break;
                case 33:
                    for (int i = 0; i < 4; i++) {
                        string source = i < 2 ? ScheduleBridge("None") : request.FullScript.Replace("public UpdateFrequency updateFrequency = UpdateFrequency.Update1;", "public UpdateFrequency updateFrequency = UpdateFrequency.None;");
                        if (i % 2 == 1) source = source.Replace("public LogLevel logLevel = LogLevel.INFO;", "public LogLevel logLevel = LogLevel.SCRIPT_ONLY;");
                        LifeRequire(source.Contains("public UpdateFrequency updateFrequency = UpdateFrequency.None;"), "Output comparison requires manual scheduling.");
                        PocInstall(i, source, "print 'EC_OUTPUT_ONE'\nprint 'EC_OUTPUT_TWO'");
                    }
                    PocPhase(34); break;
                case 34:
                    for (int i = 0; i < 4; i++) pocBlocks[i].Run("");
                    for (int i = 0; i < 2; i++) {
                        LifeRequire(PocOutput(i) == PocOutput(i + 2), "Bridge/standalone output differs for " + (i == 0 ? "INFO" : "SCRIPT_ONLY") + " at call " + outputCalls + ": " + PocOutput(i) + " / " + PocOutput(i + 2));
                        outputEvidence.Add(new { Mode = i == 0 ? "INFO" : "SCRIPT_ONLY", Call = outputCalls, Bridge = PocOutput(i), Standalone = PocOutput(i + 2) });
                    }
                    if (++outputCalls < 30) break;
                    for (int i = 0; i < 2; i++) {
                        var state = ModPocSourceLoader.RuntimeMap("states")[pocBlocks[i].EntityId];
                        var engine = state.GetType().GetField("Engine").GetValue(state);
                        LifeRequire(engine.GetType().GetField("logLevel").GetValue(engine).ToString() == (i == 0 ? "INFO" : "SCRIPT_ONLY"), "Bridge did not forward logLevel.");
                    }
                    LifeRequire(PocOutput(0).Contains("Queued Threads:") && !PocOutput(1).Contains("Queued Threads:"), "Logging modes did not differ.");
                    PocPass("logging-and-output-parity", "Thirty manual invocations matched standalone PB DetailedInfo exactly at INFO and SCRIPT_ONLY, including incremental parsing and script completion.");
                    foreach (string argument in new[] { "print '" + new string('x', 12000) + "'", "break" }) {
                        for (int i = 0; i < 4; i++) pocBlocks[i].Run(argument);
                        for (int i = 0; i < 2; i++) {
                            LifeRequire(PocOutput(i) == PocOutput(i + 2), "Long output/error parity failed for logging mode " + i);
                            outputEvidence.Add(new { Mode = i == 0 ? "INFO" : "SCRIPT_ONLY", Kind = argument == "break" ? "error" : "long-output", Bridge = PocOutput(i), Standalone = PocOutput(i + 2) });
                        }
                    }
                    File.WriteAllText(Path.Combine(root, "output-parity.json"), json.Serialize(outputEvidence));
                    LifeRequire(PocOutput(1).Contains("Exception"), "SCRIPT_ONLY suppressed errors.");
                    PocPass("long-output-and-error-parity", "Native PB output matched standalone for 12,000-character prints and runtime errors at both logging levels.");

                    for (int i = 0; i < 4; i++) pocBlocks[i].Enabled = false;
                    Finish(); break;
            }
            File.WriteAllText(Path.Combine(root, "poc-progress.json"), json.Serialize(new { Phase = pocPhase, WaitTicks = pocWait, Calls = Enumerable.Range(0, 4).Select(ScheduleCalls).ToArray() }));
        }
        void TickMissingModBridge() {
            if (pb == null) {
                pb = Blocks().OfType<IMyProgrammableBlock>().Single(b => b.CustomName == request.ProgrammableBlock);
                pb.CustomData = "print 'POC_SHOULD_NOT_RUN'";
                pb.Enabled = true;
                string instrumented = "int __missingCalls;\n" + request.Script.Replace("\r\n", "\n").Replace(
                    "public void Main(string argument, UpdateType source)\n{",
                    "public void Main(string argument, UpdateType source)\n{\n    Storage = (++__missingCalls).ToString();");
                pb.ProgramData = instrumented;
                if (pb.HasCompileErrors) throw new Exception("Missing-mod bridge compile failed: " + pb.DetailedInfo);
                pb.Run("print 'POC_ARGUMENT'");
                missingOutput = pb.DetailedInfo ?? "";
                LifeRequire(pb.StorageData == "1", "Missing-mod instrumentation failed.");
                LifeRequire(missingOutput.Contains("mod unavailable or incompatible") && missingOutput.Contains("argument was not accepted") && !missingOutput.Contains("POC_SHOULD_NOT_RUN"), "Missing mod did not reject the argument.");
                return;
            }
            LifeRequire(pb.StorageData == "1" && pb.DetailedInfo == missingOutput, "Missing-mod bridge polled automatically instead of stopping.");
            if (++missingTicks < 180) return;
            pb.Run("print 'POC_ARGUMENT'");
            LifeRequire(pb.StorageData == "2", "Explicit Run did not retry missing-mod discovery.");
            results.Add(new Result { Name = "missing-mod-no-polling-manual-retry", Passed = true,
                Detail = "Missing mod rejected the argument, ran no automatic retry during 180 ticks, and retried on explicit Run.", Ticks = missingTicks });
            pb.Enabled = false; Finish();
        }
    }
}
