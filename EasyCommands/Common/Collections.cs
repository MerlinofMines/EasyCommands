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
        //Utilities for constructing collections with few characters
        static List<T> NewList<T>(params T[] elements) => new List<T>(elements);
        static Dictionary<T, U> NewDictionary<T, U>(params KeyValuePair<T, U>[] elements) => elements.ToDictionary(e => e.Key, e => e.Value);
        static KeyValuePair<T, U> KeyValuePair<T, U>(T key, U value) => new KeyValuePair<T, U>(key, value);

        public class KeyedList {
            public List<KeyedVariable> keyedValues;

            public KeyedList(params Variable[] values) {
                keyedValues = NewList(values).ConvertAll(AsKeyedVariable);
            }

            public List<Variable> GetValues() => keyedValues.ConvertAll(v => (Variable)v);

            //If numeric, get by index.  If string, get by key value
            public Variable GetValue(Primitive key) {
                switch(key.returnType) {
                    case Return.NUMERIC:
                        return keyedValues[(int)CastNumber(key)];
                    case Return.STRING:
                        var keyString = CastString(key);
                        return keyedValues.Where(v => v.GetKey() == CastString(key))
                            .Cast<Variable>()
                            .DefaultIfEmpty(EmptyList())
                            .First();
                    default:
                        throw new Exception("Cannot lookup collection value for value: " + key.value);
                }
            }

            //If numeric, set by Index.  If string, put (or append) keyed value
            public void SetValue(Primitive key, Variable value) {
                if (key.returnType == Return.NUMERIC) {
                    keyedValues[(int)CastNumber(key)] = AsKeyedVariable(value);
                } else if (key.returnType == Return.STRING) {
                    var keyString = CastString(key);
                    KeyedVariable existing = keyedValues.Where(v => v.GetKey() == keyString).FirstOrDefault();
                    if (existing == null) {
                        keyedValues.Add(new KeyedVariable(GetStaticVariable(keyString), value));
                    } else existing.Value = value;
                } else throw new Exception("Cannot set collection value by value: " + key.value);
            }

            public KeyedList Combine(KeyedList other) {
                var otherKeys = new HashSet<string>(other.keyedValues.Where(k => k.HasKey()).Select(k => k.GetKey()).Distinct());
                var uniqueKeyedVariables = keyedValues.Where(k => !k.HasKey() || !otherKeys.Contains(k.GetKey())).ToList();
                return new KeyedList(uniqueKeyedVariables.Concat(other.GetValues()).ToArray());
            }

            public KeyedList Remove(KeyedList other) {
                KeyedList copy = new KeyedList(keyedValues.Select(v => new KeyedVariable(v.Key, v.Value)).ToArray());
                other.keyedValues.ForEach(v => copy.SetValue(v.GetValue(), null));
                return new KeyedList(copy.keyedValues.Where(k => k.Value != null).ToArray());
            }

            public KeyedList Keys() => new KeyedList(keyedValues.Where(v => v.HasKey()).Select(v => GetStaticVariable(v.GetKey())).ToArray());
            public KeyedList Values() => new KeyedList(keyedValues.Select(v => v.Value).ToArray());

            public KeyedList DeepCopy() => new KeyedList(keyedValues.Select(k => new KeyedVariable(k.Key, new StaticVariable(k.Value.GetValue().DeepCopy()))).ToArray());

            public String Print() => "[" + string.Join(",", keyedValues.Select(k => k.Print())) + "]";
        }
    }
}
