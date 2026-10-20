using Sandbox.ModAPI.Ingame;

namespace IngameScript {
    public partial class Program : MyGridProgram {
        public Program() { InitializeEngine(); }

        static void RunProgram(IMyProgrammableBlock block, string argument) {
            block.TryRun(argument);
        }
    }
}
