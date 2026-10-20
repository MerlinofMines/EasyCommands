using Sandbox.ModAPI.Ingame;

namespace IngameScript {
    public partial class Program : ModHostBase {
        public Program(ModHostContext host) : base(host) { InitializeEngine(); }

        static void RunProgram(IMyProgrammableBlock block, string argument) {
            var target = (object)block as Sandbox.ModAPI.IMyProgrammableBlock;
            // Match the existing fire-and-forget handler: a rejected Run is not
            // a language exception. Mod Run has no TryRun return value.
            if (target == null || target.Closed || !target.Enabled || !target.IsWorking || target.IsRunning) return;
            target.Run(argument ?? "", UpdateType.Mod);
        }
    }
}
