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

        static KeyedList NewKeyedList(IEnumerable<IVariable> values = null) =>
            new KeyedList { keyedValues = NewList((values ?? NewList<IVariable>()).ToArray()).ConvertAll(AsKeyedVariable) };

        public class KeyedList {
            public List<KeyedVariable> keyedValues;

            public List<IVariable> GetValues() => keyedValues.ConvertAll(v => (IVariable)v);

            //If numeric, get by index.  If string, get by key value
            public IVariable GetValue(Primitive key) {
                switch(key.returnType) {
                    case Return.NUMERIC:
                        return keyedValues[(int)CastNumber(key)];
                    case Return.STRING:
                        var keyString = CastString(key);
                        return keyedValues.Where(v => v.GetKey() == CastString(key))
                            .Cast<IVariable>()
                            .DefaultIfEmpty(EmptyList())
                            .First();
                    default:
                        throw new Exception("Cannot lookup collection value for value: " + key.value);
                }
            }

            //If numeric, set by Index.  If string, put (or append) keyed value
            public void SetValue(Primitive key, IVariable value) {
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
                var uniqueKeyedVariables = keyedValues.Where(k => !k.HasKey() || !otherKeys.Contains(k.GetKey()));
                return NewKeyedList(uniqueKeyedVariables.Concat(other.keyedValues.Select(k => k.DeepCopy())));
            }

            public KeyedList Remove(KeyedList other) {
                KeyedList copy = NewKeyedList(keyedValues.Select(v => new KeyedVariable(v.Key, v.Value)));
                other.keyedValues.ForEach(v => copy.SetValue(v.GetValue(), null));
                return NewKeyedList(copy.keyedValues.Where(k => k.Value != null));
            }

            public KeyedList Keys() => NewKeyedList(keyedValues.Where(v => v.HasKey()).Select(v => GetStaticVariable(v.GetKey())));
            public KeyedList Values() => NewKeyedList(keyedValues.Select(v => v.Value));

            public KeyedList DeepCopy() => NewKeyedList(keyedValues.Select(k => k.DeepCopy()));

            public String Print() => "[" + string.Join(",", keyedValues.Select(k => k.Print())) + "]";
        }
    }
}
