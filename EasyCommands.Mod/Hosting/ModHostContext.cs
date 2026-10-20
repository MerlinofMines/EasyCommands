using System;
using System.Text;
using Sandbox.ModAPI.Ingame;

namespace IngameScript {
    // Invocation services are borrowed from the PB and cleared before the delegate returns.
    public sealed class ModHostContext {
        public IMyProgrammableBlock Me;
        public IMyGridTerminalSystem GridTerminalSystem;
        public IMyIntergridCommunicationSystem IGC;
        public readonly ModRuntime Runtime = new ModRuntime();
        readonly StringBuilder output = new StringBuilder();

        public string Output { get { return output.ToString(); } }
        public void Begin(IMyGridTerminalSystem terminal, IMyIntergridCommunicationSystem igc, double elapsed) {
            GridTerminalSystem = terminal;
            IGC = igc;
            Runtime.TimeSinceLastRun = TimeSpan.FromMilliseconds(Math.Max(0, elapsed));
            output.Clear();
        }
        public void Echo(string value) {
            if (output.Length >= 4096) return;
            var remaining = 4096 - output.Length;
            output.Append((value ?? "").Substring(0, Math.Min((value ?? "").Length, remaining)));
            if (output.Length < 4096) output.Append('\n');
        }
        public void End() {
            GridTerminalSystem = null;
            IGC = null;
            Runtime.TimeSinceLastRun = TimeSpan.Zero;
        }
    }
    public sealed class ModRuntime {
        public TimeSpan TimeSinceLastRun { get; set; }
        public UpdateFrequency UpdateFrequency { get; set; }
    }
    public abstract class ModHostBase {
        readonly ModHostContext host;
        protected ModHostBase(ModHostContext host) { this.host = host; }
        public IMyProgrammableBlock Me { get { return host.Me; } }
        public IMyGridTerminalSystem GridTerminalSystem { get { return host.GridTerminalSystem; } }
        public IMyIntergridCommunicationSystem IGC { get { return host.IGC; } }
        public ModRuntime Runtime { get { return host.Runtime; } }
        public void Echo(string value) { host.Echo(value); }
    }
}
