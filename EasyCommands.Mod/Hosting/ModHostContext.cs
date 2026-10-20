namespace IngameScript {
    using System;
    using System.Collections.Generic;
    using Sandbox.ModAPI.Ingame;

    // Invocation services are borrowed from the PB and cleared before the delegate returns.
    public sealed class ModHostContext {
        public IMyProgrammableBlock Me;
        public IMyGridTerminalSystem GridTerminalSystem;
        public IMyIntergridCommunicationSystem IGC;
        public readonly ModRuntime Runtime = new ModRuntime();
        readonly List<string> output = new List<string>();

        public string[] Output { get { return output.ToArray(); } }
        public void Begin(IMyGridTerminalSystem terminal, IMyIntergridCommunicationSystem igc, double elapsed) {
            GridTerminalSystem = terminal;
            IGC = igc;
            Runtime.TimeSinceLastRun = TimeSpan.FromMilliseconds(Math.Max(0, elapsed));
            output.Clear();
        }
        public void Echo(string value) {
            output.Add(value);
        }
        public void End() {
            output.Clear();
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
