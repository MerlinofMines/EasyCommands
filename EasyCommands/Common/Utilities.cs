using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public const float RPMToRadiansPerSec = (float)Math.PI / 30f;
        public const float RadiansPerSecToRPM = 30f / (float)Math.PI;

        public static T Type<T>() => default(T);

        public delegate T Supplier<T>();

        //Utilities for constructing collections with few characters
        public static List<T> NewList<T>(params T[] elements) => new List<T>(elements);
        public static Dictionary<T, U> NewDictionary<T, U>(params KeyValuePair<T, U>[] elements) => elements.ToDictionary(e => e.Key, e => e.Value);
        public static KeyValuePair<T, U> KeyValuePair<T, U>(T key, U value) => new KeyValuePair<T, U>(key, value);
        public static Variable EmptyList() => GetStaticVariable(NewKeyedList());

        //Other useful utilities
        public static Variable GetStaticVariable(object o) => new StaticVariable(ResolvePrimitive(o));
        public static Vector3D Vector(double x, double y, double z) => new Vector3D(x, y, z);
    }
}
