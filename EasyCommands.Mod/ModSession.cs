namespace EasyCommands.Mod {
    using System;
    using System.Collections.Generic;
    using Sandbox.ModAPI;
    using Sandbox.ModAPI.Interfaces;
    using Sandbox.ModAPI.Interfaces.Terminal;
    using VRage;
    using VRage.Game.Components;
    using VRage.ModAPI;
    using PbBlock = Sandbox.ModAPI.IMyProgrammableBlock;
    using PbView = Sandbox.ModAPI.Ingame.IMyProgrammableBlock;
    using Frequency = Sandbox.ModAPI.Ingame.UpdateFrequency;
    using Update = Sandbox.ModAPI.Ingame.UpdateType;
    using Terminal = Sandbox.ModAPI.Ingame.IMyGridTerminalSystem;
    using Igc = Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem;
    using Step = System.Func<bool, string, Sandbox.ModAPI.Ingame.UpdateType, Sandbox.ModAPI.Ingame.IMyGridTerminalSystem, Sandbox.ModAPI.Ingame.IMyIntergridCommunicationSystem, double, Sandbox.ModAPI.Ingame.UpdateFrequency, int, VRage.MyTuple<Sandbox.ModAPI.Ingame.UpdateFrequency, string[]>>;

    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public sealed class ModSession : MySessionComponentBase {
        const string ApiId = "EasyCommands.Bridge.v3";
        readonly Dictionary<long, RuntimeState> states = new Dictionary<long, RuntimeState>();
        readonly Dictionary<long, PbBlock> subscriptions = new Dictionary<long, PbBlock>();
        IMyTerminalControlProperty<Step> property;
        bool registered;

        sealed class RuntimeState {
            public readonly IngameScript.ModHostContext Host = new IngameScript.ModHostContext();
            public IngameScript.Program Engine;
            public int Calls;
            public bool Busy;
        }

        public override void BeforeStart() { RegisterApi(); }
        public override void UpdateAfterSimulation() { RegisterApi(); }

        void RegisterApi() {
            if (registered || MyAPIGateway.TerminalControls == null) return;
            property = MyAPIGateway.TerminalControls.CreateProperty<Step, PbBlock>(ApiId);
            property.Getter = block => Bind(block as PbBlock);
            MyAPIGateway.TerminalControls.AddControl<PbBlock>(property);
            registered = true;
            MyAPIGateway.Utilities.ShowMessage("EasyCommands", "Source mod registered " + ApiId);
        }

        Step Bind(PbBlock block) {
            if (block == null) return null;
            return (reset, argument, source, terminal, igc, elapsed, frequency, logging) => Invoke(block, reset, argument, source, terminal, igc, elapsed, frequency, logging);
        }

        MyTuple<Frequency, string[]> Invoke(PbBlock block, bool reset, string argument, Update source, Terminal terminal, Igc igc, double elapsed, Frequency frequency, int logging) {
            if (!MyAPIGateway.Multiplayer.IsServer) return Reply(Frequency.None, "EasyCommands is server only.");
            if (block == null || block.Closed || !block.IsFunctional || !block.Enabled || !block.IsWorking)
                return Reply(Frequency.None, "EasyCommands PB is unavailable.");
            if (terminal == null || igc == null) return Reply(Frequency.None, "EasyCommands PB services are unavailable.");
            RuntimeState state;
            states.TryGetValue(block.EntityId, out state);
            if (state != null && state.Busy) return Reply(Frequency.None, "EasyCommands recursive invocation rejected.");
            if (reset || state == null) {
                state = new RuntimeState();
                states[block.EntityId] = state;
                if (!subscriptions.ContainsKey(block.EntityId)) {
                    subscriptions[block.EntityId] = block;
                    block.OnClose += OnBlockClose;
                }
            }
            if (state.Busy) return Reply(Frequency.None, "EasyCommands recursive invocation rejected.");
            state.Busy = true;
            try {
                state.Host.Me = block as PbView;
                if (state.Host.Me == null) return Reply(Frequency.None, "EasyCommands PB view unavailable.");
                state.Host.Begin(terminal, igc, elapsed);
                if (state.Engine == null) state.Engine = new IngameScript.Program(state.Host);
                state.Engine.updateFrequency = frequency;
                state.Engine.logLevel = (IngameScript.Program.LogLevel)logging;
                state.Calls++;
                state.Engine.Main(argument ?? "");
                return new MyTuple<Frequency, string[]>(state.Host.Runtime.UpdateFrequency, state.Host.Output);
            } catch (Exception ex) {
                return Reply(Frequency.None, "EasyCommands mod error: " + ex.Message);
            } finally {
                state.Host.End();
                state.Busy = false;
            }
        }

        static MyTuple<Frequency, string[]> Reply(Frequency frequency, string message) {
            return new MyTuple<Frequency, string[]>(frequency, new[] { message });
        }

        void OnBlockClose(IMyEntity entity) {
            states.Remove(entity.EntityId);
            PbBlock block;
            if (subscriptions.TryGetValue(entity.EntityId, out block)) {
                block.OnClose -= OnBlockClose;
                subscriptions.Remove(entity.EntityId);
            }
        }

        protected override void UnloadData() {
            foreach (var block in subscriptions.Values) block.OnClose -= OnBlockClose;
            subscriptions.Clear();
            states.Clear();
            if (registered && MyAPIGateway.TerminalControls != null)
                MyAPIGateway.TerminalControls.RemoveControl<PbBlock>(property);
            registered = false;
            property = null;
        }
    }
}
