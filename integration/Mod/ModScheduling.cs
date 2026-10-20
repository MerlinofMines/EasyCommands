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
            var states = ModSourceLoader.RuntimeMap("states");
            return states.Contains(modBlocks[i].EntityId) ? ModSourceLoader.Calls(states[modBlocks[i].EntityId]) : 0;
        }
        void TickModScheduling() {
            if (++modWait > 1800) throw new Exception("Scheduling timeout in phase " + modPhase);
            switch (modPhase) {
                case 29:
                    scheduleManualBaseline = ScheduleCalls(3);
                    for (int i = 0; i < 4; i++) ModInstall(i, ScheduleBridge(scheduleNames[i]), LifeProgram(800 + i));
                    ModPhase(30); break;
                case 30:
                    if (Enumerable.Range(0, 3).Any(i => LifeTicks(modBlocks[i], 800 + i) < 2)) break;
                    scheduleStartCalls = Enumerable.Range(0, 3).Select(ScheduleCalls).ToArray();
                    scheduleLastCalls = (int[])scheduleStartCalls.Clone();
                    scheduleLastTicks = new int[3];
                    ModPhase(31); break;
                case 31:
                    for (int i = 0; i < 3; i++) {
                        int calls = ScheduleCalls(i);
                        if (calls == scheduleLastCalls[i]) continue;
                        LifeRequire(calls == scheduleLastCalls[i] + 1, "Unexpected multiple scheduled calls in one frame.");
                        if (scheduleLastTicks[i] != 0) scheduleIntervals[i].Add(modWait - scheduleLastTicks[i]);
                        scheduleLastTicks[i] = modWait; scheduleLastCalls[i] = calls;
                    }
                    LifeRequire(ScheduleCalls(3) == scheduleManualBaseline, "Manual-only bridge started automatically.");
                    if (modWait < 600) break;
                    for (int i = 0; i < 3; i++) {
                        int observedCalls = ScheduleCalls(i) - scheduleStartCalls[i], expected = 600 / schedulePeriods[i];
                        LifeRequire(Math.Abs(observedCalls - expected) <= 1, "Requested " + scheduleNames[i] + " scheduled " + observedCalls + " calls, expected " + expected);
                        LifeRequire(scheduleIntervals[i].Count > 2 && scheduleIntervals[i].All(v => v == schedulePeriods[i]), "Wrong cadence for " + scheduleNames[i]);
                        var state = ModSourceLoader.RuntimeMap("states")[modBlocks[i].EntityId];
                        var engine = state.GetType().GetField("Engine").GetValue(state);
                        LifeRequire(engine.GetType().GetField("updateFrequency").GetValue(engine).ToString() == scheduleNames[i], "Bridge did not forward updateFrequency to engine.");
                        ModPass("requested-interval-" + scheduleNames[i], observedCalls + " calls over 600 simulation ticks; every measured interval was " + schedulePeriods[i] + " ticks; engine setting matched.");
                        modBlocks[i].Enabled = false;
                    }
                    for (int attempt = 0; attempt < 40 && LifeTicks(modBlocks[3], 803) < 1; attempt++) modBlocks[3].Run("");
                    LifeRequire(LifeTicks(modBlocks[3], 803) >= 1, "Manual bridge did not execute explicit Run.");
                    scheduleLastCalls[0] = ScheduleCalls(3);
                    ModPhase(32); break;
                case 32:
                    LifeRequire(ScheduleCalls(3) == scheduleLastCalls[0], "None scheduled automatic calls after manual execution.");
                    if (modWait < 120) break;
                    modBlocks[3].Run("");
                    LifeRequire(ScheduleCalls(3) == scheduleLastCalls[0] + 1, "None ignored a subsequent explicit Run.");
                    ModPass("requested-interval-None", "No automatic calls before or after manual execution; explicit Run still worked.");
                    modBlocks[3].Enabled = false;
                    File.WriteAllText(Path.Combine(root, "scheduling.json"), json.Serialize(new { Periods = schedulePeriods, Intervals = scheduleIntervals }));
                    ModPhase(33); break;
                case 33:
                    for (int i = 0; i < 4; i++) {
                        string source = i < 2 ? ScheduleBridge("None") : request.FullScript.Replace("public UpdateFrequency updateFrequency = UpdateFrequency.Update1;", "public UpdateFrequency updateFrequency = UpdateFrequency.None;");
                        if (i % 2 == 1) source = source.Replace("public LogLevel logLevel = LogLevel.INFO;", "public LogLevel logLevel = LogLevel.SCRIPT_ONLY;");
                        LifeRequire(source.Contains("public UpdateFrequency updateFrequency = UpdateFrequency.None;"), "Output comparison requires manual scheduling.");
                        ModInstall(i, source, "print 'EC_OUTPUT_ONE'\nprint 'EC_OUTPUT_TWO'");
                    }
                    ModPhase(34); break;
                case 34:
                    for (int i = 0; i < 4; i++) modBlocks[i].Run("");
                    for (int i = 0; i < 2; i++) {
                        LifeRequire(ModOutput(i) == ModOutput(i + 2), "Bridge/standalone output differs for " + (i == 0 ? "INFO" : "SCRIPT_ONLY") + " at call " + outputCalls + ": " + ModOutput(i) + " / " + ModOutput(i + 2));
                        outputEvidence.Add(new { Mode = i == 0 ? "INFO" : "SCRIPT_ONLY", Call = outputCalls, Bridge = ModOutput(i), Standalone = ModOutput(i + 2) });
                    }
                    if (++outputCalls < 30) break;
                    for (int i = 0; i < 2; i++) {
                        var state = ModSourceLoader.RuntimeMap("states")[modBlocks[i].EntityId];
                        var engine = state.GetType().GetField("Engine").GetValue(state);
                        LifeRequire(engine.GetType().GetField("logLevel").GetValue(engine).ToString() == (i == 0 ? "INFO" : "SCRIPT_ONLY"), "Bridge did not forward logLevel.");
                    }
                    LifeRequire(ModOutput(0).Contains("Queued Threads:") && !ModOutput(1).Contains("Queued Threads:"), "Logging modes did not differ.");
                    ModPass("logging-and-output-parity", "Thirty manual invocations matched standalone PB DetailedInfo exactly at INFO and SCRIPT_ONLY, including incremental parsing and script completion.");
                    foreach (string argument in new[] { "print '" + new string('x', 12000) + "'", "break" }) {
                        for (int i = 0; i < 4; i++) modBlocks[i].Run(argument);
                        for (int i = 0; i < 2; i++) {
                            LifeRequire(ModOutput(i) == ModOutput(i + 2), "Long output/error parity failed for logging mode " + i);
                            outputEvidence.Add(new { Mode = i == 0 ? "INFO" : "SCRIPT_ONLY", Kind = argument == "break" ? "error" : "long-output", Bridge = ModOutput(i), Standalone = ModOutput(i + 2) });
                        }
                    }
                    File.WriteAllText(Path.Combine(root, "output-parity.json"), json.Serialize(outputEvidence));
                    LifeRequire(ModOutput(1).Contains("Exception"), "SCRIPT_ONLY suppressed errors.");
                    ModPass("long-output-and-error-parity", "Native PB output matched standalone for 12,000-character prints and runtime errors at both logging levels.");

                    for (int i = 0; i < 4; i++) modBlocks[i].Enabled = false;
                    Finish(); break;
            }
            File.WriteAllText(Path.Combine(root, "mod-progress.json"), json.Serialize(new { Phase = modPhase, WaitTicks = modWait, Calls = Enumerable.Range(0, 4).Select(ScheduleCalls).ToArray() }));
        }
        void TickMissingModBridge() {
            if (pb == null) {
                pb = Blocks().OfType<IMyProgrammableBlock>().Single(b => b.CustomName == request.ProgrammableBlock);
                pb.CustomData = "print 'EC_MOD_SHOULD_NOT_RUN'";
                pb.Enabled = true;
                string instrumented = "int __missingCalls;\n" + request.Script.Replace("\r\n", "\n").Replace(
                    "public void Main(string argument, UpdateType source)\n{",
                    "public void Main(string argument, UpdateType source)\n{\n    Storage = (++__missingCalls).ToString();");
                pb.ProgramData = instrumented;
                if (pb.HasCompileErrors) throw new Exception("Missing-mod bridge compile failed: " + pb.DetailedInfo);
                pb.Run("print 'EC_MOD_ARGUMENT'");
                missingOutput = pb.DetailedInfo ?? "";
                LifeRequire(pb.StorageData == "1", "Missing-mod instrumentation failed.");
                LifeRequire(missingOutput.Contains("mod unavailable or incompatible") && missingOutput.Contains("argument was not accepted") && !missingOutput.Contains("EC_MOD_SHOULD_NOT_RUN"), "Missing mod did not reject the argument.");
                return;
            }
            LifeRequire(pb.StorageData == "1" && pb.DetailedInfo == missingOutput, "Missing-mod bridge polled automatically instead of stopping.");
            if (++missingTicks < 180) return;
            pb.Run("print 'EC_MOD_ARGUMENT'");
            LifeRequire(pb.StorageData == "2", "Explicit Run did not retry missing-mod discovery.");
            results.Add(new Result { Name = "missing-mod-no-polling-manual-retry", Passed = true,
                Detail = "Missing mod rejected the argument, ran no automatic retry during 180 ticks, and retried on explicit Run.", Ticks = missingTicks });
            pb.Enabled = false; Finish();
        }
    }
}
