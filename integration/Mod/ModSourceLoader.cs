using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using Sandbox.ModAPI;
using VRage;
using VRage.Game.Components;
using VRage.Scripting;

namespace EasyCommands.Integration {
    // Test bootstrap only. Engine source is compiled by the real Mod-target
    // compiler, with its whitelist and instrumentation, into another assembly.
    // This does not verify normal local/Workshop package discovery or loading.
    internal static class ModSourceLoader {
        // Read-only test inspection of the actual compiled mod instance. No
        // diagnostics endpoints or reflection are added to the shipped mod.
        internal static MySessionComponentBase Component;
        internal static System.Collections.IDictionary RuntimeMap(string field) {
            if (field != "states" && field != "subscriptions") throw new ArgumentException("Unknown runtime map");
            return (System.Collections.IDictionary)Component.GetType().GetField(field,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Component);
        }
        internal static int Calls(object state) { return (int)state.GetType().GetField("Calls").GetValue(state); }

        internal static void Load(string sourceDirectory, string outputDirectory) {
            var sources = Directory.GetFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories);
            var timer = Stopwatch.StartNew();
            List<Message> diagnostics;
            var assembly = MyVRage.Platform.Scripting.CompileAsync(MyApiTarget.Mod,
                Path.Combine(outputDirectory, "EasyCommandsModSandbox"),
                sources.Select(path => new Script(path, File.ReadAllText(path))),
                out diagnostics, "EasyCommandsMod test bootstrap").Result;
            timer.Stop();
            File.WriteAllText(Path.Combine(outputDirectory, "mod-compile.json"),
                new JavaScriptSerializer().Serialize(new {
                    CompilerTarget = "Mod", SourceFiles = sources.Length,
                    ElapsedMs = timer.Elapsed.TotalMilliseconds,
                    Succeeded = assembly != null,
                    Diagnostics = diagnostics.Select(d => new { d.IsError, d.Text }).ToArray(),
                    PackageLoadingVerified = false
                }));
            if (assembly == null) throw new Exception("Mod-target sandbox compilation failed; see mod-compile.json.");
            var type = assembly.GetType("EasyCommands.Mod.ModSession", true);
            var component = (MySessionComponentBase)Activator.CreateInstance(type);
            MyAPIGateway.Session.RegisterComponent(component, MyUpdateOrder.AfterSimulation, 1000);
            Component = component;
        }
    }
}
