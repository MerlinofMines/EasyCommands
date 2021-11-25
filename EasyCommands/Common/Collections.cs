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
        static List<T> NewList<T>(params T[] elements) => new List<T>(elements);
        static Dictionary<T, U> NewDictionary<T, U>() => new Dictionary<T, U>();

        public class KeyedList {
            public List<KeyedVariable> keyedValues;

            public KeyedList(params Variable[] values) {
                keyedValues = values.ToList().ConvertAll(AsKeyedVariable);
            }

            public List<Variable> GetValues() => keyedValues.ConvertAll(v => (Variable)v);

            //If numeric, get by index.  If string, get by key value
            public Variable GetValue(Primitive key) {
                switch(key.returnType) {
                    case Return.NUMERIC:
                        return keyedValues[(int)CastNumber(key)];
                    case Return.STRING:
                        var keyString = CastString(key);
                        return keyedValues.Where(v => v.Key == CastString(key))
                            .Cast<Variable>()
                            .DefaultIfEmpty(EmptyList())
                            .First();
                    default:
                        throw new Exception("Cannot lookup collection value by Primitive Type: " + key.returnType);
                }
            }

            //If numeric, set by Index.  If string, put (or append) keyed value
            public void SetValue(Primitive key, Variable value) {
                if (key.returnType == Return.NUMERIC) {
                    keyedValues[(int)CastNumber(key)] = AsKeyedVariable(value);
                } else if (key.returnType == Return.STRING) {
                    var keyString = CastString(key);
                    KeyedVariable existing = keyedValues.Where(v => v.Key == keyString).FirstOrDefault();
                    if (existing == null) {
                        keyedValues.Add(new KeyedVariable(keyString, value));
                    } else existing.Value = value;
                } else throw new Exception("Cannot set collection value by Primitive Type: " + key.returnType);
            }

            public KeyedList Combine(KeyedList other) {
                var otherKeys = new HashSet<string>(other.keyedValues.Where(k => k.HasKey()).Select(k => k.Key).Distinct());
                var uniqueKeyedVariables = keyedValues.Where(k => !k.HasKey() || !otherKeys.Contains(k.Key)).ToList();
                return new KeyedList(uniqueKeyedVariables.Concat(other.GetValues()).ToArray());
            }

            public KeyedList Remove(KeyedList other) {
                KeyedList copy = new KeyedList(keyedValues.Select(v => new KeyedVariable(v.Key, v.Value)).ToArray());
                other.keyedValues.ForEach(v => copy.SetValue(v.GetValue(), null));
                return new KeyedList(copy.keyedValues.Where(k => k.Value != null).ToArray());
            }

            public KeyedList Keys() => new KeyedList(keyedValues.Where(v => v.HasKey()).Select(v => GetStaticVariable(v.Key)).ToArray());
            public KeyedList Values() => new KeyedList(keyedValues.Select(v => v.Value).ToArray());

            public KeyedList DeepCopy() => new KeyedList(keyedValues.Select(k => new KeyedVariable(k.Key, new StaticVariable(k.Value.GetValue().DeepCopy()))).ToArray());

            public String Print() => "[" + string.Join(",", keyedValues.Select(k => (k.HasKey() ? k.Key + "=" : "") + CastString(k.Value.GetValue()))) + "]";
        }
    }
}
