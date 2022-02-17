using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public class Cache<T, U> {
            public Dictionary<MyTuple<T, string>, U> storage = NewDictionary<MyTuple<T, string>, U>();
            public U GetOrCreate(T key1, string key2, Func<string, U> create) {
                var key = MyTuple.Create(key1, key2);
                return storage.ContainsKey(key) ? storage[key] : storage[key] = create(key2);
            }
            public void Clear() => storage.Clear();
        }
    }
}
