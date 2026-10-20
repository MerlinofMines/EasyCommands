using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using VRage;
using VRage.Game;
using VRage.ObjectBuilders;
using VRageMath;

namespace EasyCommands.Integration {
    public sealed partial class Harness {
        IMyProgrammableBlock lifeVictim, lifeSurvivor;
        Sandbox.ModAPI.Ingame.IMyBatteryBlock lifeBattery;
        Sandbox.ModAPI.Ingame.IMyWarhead lifeWarhead;
        object lifeRuntime;
        long lifeOldId;
        int lifeFrozenCalls, lifeFrozenTicks, lifeSurvivorTicks, lifeMapCount;
        string lifeFrozenOutput;
        bool lifeReplacementStarted;

        string LifeProgram(int seed) {
            return ":main\nset global ecSeed to " + seed + "\nset global ecTicks to 0\nprint 'START|' + ecSeed + '|0'\nwhile true\n  set global ecTicks to ecTicks + 1\n  print 'LIFE|' + ecSeed + '|' + ecTicks";
        }
        void LifeInstall(IMyProgrammableBlock block, int seed) {
            block.Enabled = true; block.CustomData = LifeProgram(seed); block.ProgramData = request.Script;
            if (block.HasCompileErrors) throw new Exception("Lifecycle bridge compilation failed: " + block.DetailedInfo);
            block.Run("");
        }
        int LifeTicks(IMyProgrammableBlock block, int seed) {
            string output = block.DetailedInfo ?? "";
            if (output.Contains("Exception") || output.Contains("bridge stopped") || output.Contains("mod error"))
                throw new Exception("Lifecycle engine failure: " + output);
            var match = Regex.Match(output, @"LIFE\|" + seed + @"\|(\d+)");
            return match.Success ? Int32.Parse(match.Groups[1].Value) : -1;
        }
        void LifeTrace(string stage) {
            var states = ModPocSourceLoader.RuntimeMap("states");
            var subscriptions = ModPocSourceLoader.RuntimeMap("subscriptions");
            File.AppendAllText(Path.Combine(root, "lifecycle.jsonl"), json.Serialize(new {
                Stage = stage, Tick = pocTick, Phase = pocPhase, Wait = pocWait,
                OldId = lifeOldId, VictimId = lifeVictim.EntityId, Closed = lifeVictim.Closed,
                Enabled = lifeVictim.Enabled, Working = lifeVictim.IsWorking, Functional = lifeVictim.IsFunctional,
                RuntimeCount = states.Count, SubscriptionCount = subscriptions.Count,
                OldRuntimePresent = states.Contains(lifeOldId), OldSubscriptionPresent = subscriptions.Contains(lifeOldId),
                VictimCalls = states.Contains(lifeVictim.EntityId) ? ModPocSourceLoader.Calls(states[lifeVictim.EntityId]) : -1,
                SurvivorTicks = LifeTicks(lifeSurvivor, 503), VictimOutput = lifeVictim.DetailedInfo,
                BatteryEnabled = lifeBattery.Enabled
            }) + Environment.NewLine);
        }
        void LifeRequire(bool condition, string message) { if (!condition) throw new Exception(message); }

        void TickPocLifecycle() {
            if (++pocWait > 900) throw new Exception("Lifecycle phase " + pocPhase + " timeout.");
            if (lifeVictim != null && pocWait % 10 == 0) LifeTrace("observing");
            switch (pocPhase) {
                case 19:
                    var all = Blocks();
                    lifeVictim = all.OfType<IMyProgrammableBlock>().Single(b => b.CustomName == "POC Lifecycle Victim");
                    lifeSurvivor = pocPeer;
                    lifeBattery = (Sandbox.ModAPI.Ingame.IMyBatteryBlock)all.Single(b => b.CustomName == "POC Battery 2");
                    lifeWarhead = (Sandbox.ModAPI.Ingame.IMyWarhead)all.Single(b => b.CustomName == "POC Lifecycle Warhead");
                    lifeOldId = lifeVictim.EntityId;
                    MyAPIGateway.Session.SessionSettings.DestructibleBlocks = true;
                    LifeInstall(lifeVictim, 701); LifeInstall(lifeSurvivor, 503);
                    PocPhase(20); break;
                case 20:
                    if (LifeTicks(lifeVictim, 701) < 60 || LifeTicks(lifeSurvivor, 503) < 60) break;
                    lifeRuntime = ModPocSourceLoader.RuntimeMap("states")[lifeOldId];
                    LifeRequire(lifeRuntime != null, "Victim runtime missing before power interruption.");
                    LifeTrace("before-power-loss");
                    lifeBattery.Enabled = false; PocPhase(21); break;
                case 21:
                    if (lifeVictim.IsWorking) break;
                    LifeRequire(lifeVictim.Enabled && lifeVictim.IsFunctional, "Power test disabled or damaged the PB itself.");
                    lifeFrozenCalls = ModPocSourceLoader.Calls(lifeRuntime);
                    lifeFrozenTicks = LifeTicks(lifeVictim, 701); lifeFrozenOutput = lifeVictim.DetailedInfo;
                    lifeSurvivorTicks = LifeTicks(lifeSurvivor, 503);
                    LifeTrace("power-lost"); PocPhase(22); break;
                case 22:
                    LifeRequire(!lifeVictim.IsWorking && lifeVictim.Enabled, "PB recovered power unexpectedly.");
                    LifeRequire(ModPocSourceLoader.Calls(lifeRuntime) == lifeFrozenCalls && lifeVictim.DetailedInfo == lifeFrozenOutput,
                        "Unpowered PB continued invoking/executing its mod runtime.");
                    if (pocWait < 120) break;
                    LifeRequire(LifeTicks(lifeSurvivor, 503) > lifeSurvivorTicks + 60, "Other grid failed to progress during power loss.");
                    LifeRequire(Object.ReferenceEquals(lifeRuntime, ModPocSourceLoader.RuntimeMap("states")[lifeOldId]), "Power loss discarded runtime state.");
                    PocPass("battery-power-loss", "Only the battery was switched off; enabled/functional PB stopped for 120 ticks, invocation count/output froze, other grid advanced.");
                    LifeTrace("before-power-restoration"); lifeBattery.Enabled = true; PocPhase(23); break;
                case 23:
                    if (!lifeVictim.IsWorking || LifeTicks(lifeVictim, 701) <= lifeFrozenTicks + 30) break;
                    LifeRequire(Object.ReferenceEquals(lifeRuntime, ModPocSourceLoader.RuntimeMap("states")[lifeOldId]), "Power restoration reset the interpreter.");
                    LifeRequire(ModPocSourceLoader.Calls(lifeRuntime) > lifeFrozenCalls, "Runtime did not resume after power restoration.");
                    PocPass("battery-power-restoration", "Same runtime resumed with the original seed and counter, without bridge recompile or manual Run.");
                    lifeMapCount = ModPocSourceLoader.RuntimeMap("states").Count;
                    lifeSurvivorTicks = LifeTicks(lifeSurvivor, 503);
                    LifeTrace("before-warhead-detonation");
                    lifeWarhead.IsArmed = true; lifeWarhead.Detonate(); PocPhase(24); break;
                case 24:
                    if (!lifeVictim.Closed) break;
                    LifeRequire(!ModPocSourceLoader.RuntimeMap("states").Contains(lifeOldId), "Destroyed PB still has a runtime entry.");
                    LifeRequire(!ModPocSourceLoader.RuntimeMap("subscriptions").Contains(lifeOldId), "Destroyed PB still has a close subscription.");
                    LifeRequire(ModPocSourceLoader.RuntimeMap("states").Count == lifeMapCount - 1, "Unexpected runtime count after blast.");
                    LifeTrace("warhead-destroyed-pb-and-cleaned-runtime");
                    lifeRuntime = null; PocPhase(25); break;
                case 25:
                    LifeRequire(!ModPocSourceLoader.RuntimeMap("states").Contains(lifeOldId), "Destroyed PB runtime reappeared.");
                    if (pocWait < 120) break;
                    LifeRequire(lifeSurvivor.IsWorking && LifeTicks(lifeSurvivor, 503) > lifeSurvivorTicks + 60, "Survivor failed after explosion.");
                    PocPass("warhead-destruction-cleanup", "Actual armed warhead explosion closed the PB; both runtime and subscription entries disappeared and stayed absent while the other grid executed.");
                    LifeTrace("after-destruction-observation");
                    LifeCreateReplacement(); PocPhase(26); break;
                case 26:
                    var replacement = Blocks().OfType<IMyProgrammableBlock>().SingleOrDefault(b => b.CustomName == "POC Lifecycle Victim" && b.EntityId != lifeOldId);
                    if (replacement == null || !replacement.IsWorking) break;
                    lifeVictim = replacement; LifeInstall(lifeVictim, 902); PocPhase(27); break;
                case 27:
                    lifeReplacementStarted |= (lifeVictim.DetailedInfo ?? "").Contains("START|902|0");
                    if (LifeTicks(lifeVictim, 902) < 10) break;
                    LifeRequire(lifeReplacementStarted && lifeVictim.EntityId != lifeOldId, "Replacement inherited old identity or skipped fresh initialization.");
                    LifeRequire(!ModPocSourceLoader.RuntimeMap("states").Contains(lifeOldId), "Old runtime returned after replacement.");
                    LifeRequire(ModPocSourceLoader.RuntimeMap("states").Count == lifeMapCount, "Replacement runtime count is incorrect.");
                    PocPass("replacement-pb-fresh-state", "New same-named PB has a distinct EntityId and initialized a fresh seed/counter from zero; destroyed PB state remained absent.");
                    LifeTrace("replacement-fresh-state");
                    lifeVictim.Close(); PocPhase(28); break;
                case 28:
                    if (pocWait < 30) break;
                    LifeRequire(!ModPocSourceLoader.RuntimeMap("states").Contains(lifeVictim.EntityId) && !ModPocSourceLoader.RuntimeMap("subscriptions").Contains(lifeVictim.EntityId), "Replacement removal did not clean maps.");
                    PocPass("explicit-removal-cleanup", "Explicit replacement block removal also cleared runtime/subscription maps.");
                    LifeTrace("final-cleanup"); lifeSurvivor.Enabled = false; Finish(); break;
            }
            File.WriteAllText(Path.Combine(root, "poc-progress.json"), json.Serialize(new { Phase = pocPhase, WaitTicks = pocWait }));
        }
        void LifeCreateReplacement() {
            var grid = new MyObjectBuilder_CubeGrid {
                GridSizeEnum = MyCubeSize.Large, IsStatic = true, CreatePhysics = true,
                PersistentFlags = MyPersistentEntityFlags2.InScene | MyPersistentEntityFlags2.Enabled,
                DisplayName = "EC Lifecycle Replacement",
                PositionAndOrientation = new MyPositionAndOrientation(new Vector3D(-40, 0, -300), Vector3.Forward, Vector3.Up)
            };
            for (int x = 0; x < 5; x++) for (int z = 0; z < 3; z++)
                grid.CubeBlocks.Add(new MyObjectBuilder_CubeBlock { SubtypeName = "LargeBlockArmorBlock", Min = new Vector3I(x, 0, z) });
            Add(grid, "BatteryBlock", "LargeBlockBatteryBlock", "POC Replacement Battery", 0).Min = new Vector3I(0, 1, 1);
            Add(grid, "MyProgrammableBlock", "LargeProgrammableBlock", "POC Lifecycle Victim", 2).Min = new Vector3I(2, 1, 1);
            MyAPIGateway.Entities.RemapObjectBuilder(grid);
            MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(grid);
        }
    }
}
