using System;
using System.Reflection;
using System.Runtime.Serialization;
using Moq;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Ingame;
using IMyGridTerminalSystem = Sandbox.ModAPI.Ingame.IMyGridTerminalSystem;
using IMyProgrammableBlock = Sandbox.ModAPI.Ingame.IMyProgrammableBlock;

namespace EasyCommands.Tests {
    // Install the host context before the script constructor runs, just as the
    // programmable block does. Mock the current runtime interface rather than
    // depending on MDK1's implementation of an obsolete game interface.
    internal static class ProgramFactory {
        internal struct ProgramConfig {
            public IMyProgrammableBlock ProgrammableBlock;
            public IMyGridTerminalSystem GridTerminalSystem;
            public IMyGridProgramRuntimeInfo Runtime;
            public Action<string> Echo;
        }

        internal static T CreateProgram<T>(ProgramConfig config = default) where T : MyGridProgram {
            var program = (T)FormatterServices.GetUninitializedObject(typeof(T));
            var context = (IMyGridProgram)program;
            var runtime = new Mock<IMyGridProgramRuntimeInfo>();
            runtime.SetupAllProperties();
            runtime.SetupGet(r => r.MaxInstructionCount).Returns(50000);
            runtime.SetupGet(r => r.MaxCallChainDepth).Returns(10000);
            context.Runtime = config.Runtime ?? runtime.Object;
            context.Storage = string.Empty;
            context.Me = config.ProgrammableBlock;
            context.GridTerminalSystem = config.GridTerminalSystem;
            context.Echo = config.Echo ?? Console.WriteLine;

            var constructor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null, Type.EmptyTypes, null);
            if (constructor == null) throw new InvalidOperationException("Script must have a parameterless constructor.");
            constructor.Invoke(program, null);
            return program;
        }

        internal static void Run(IMyGridProgram program, string argument = "") {
            program.Main(argument ?? string.Empty, UpdateType.Trigger);
        }
    }
}
