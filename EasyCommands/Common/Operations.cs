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
        public delegate Primitive UniOperation(Primitive a);
        public delegate Primitive BiOperation(Primitive a, Primitive b);

        Dictionary<BiOperand, Dictionary<Return, Dictionary<Return, BiOperation>>> BiOperations
            = NewDictionary<BiOperand, Dictionary<Return, Dictionary<Return, BiOperation>>>();

        Dictionary<UniOperand, Dictionary<Return, UniOperation>> UniOperations
            = NewDictionary<UniOperand, Dictionary<Return, UniOperation>>();

        public Primitive PerformOperation(BiOperand type, Primitive a, Primitive b) {
            if (!BiOperations.ContainsKey(type)
                || !BiOperations[type].ContainsKey(a.returnType)
                || !BiOperations[type][a.returnType].ContainsKey(b.returnType)) {
                throw new Exception("Cannot perform operation: " + PROGRAM.biOperandToString[type] + " on types: " + PROGRAM.returnToString[a.returnType] + ", " + PROGRAM.returnToString[b.returnType]);
            }
            return BiOperations[type][a.returnType][b.returnType](a, b);
        }

        public Primitive PerformOperation(UniOperand type, Primitive a) {
            if (!UniOperations.ContainsKey(type)
                || !UniOperations[type].ContainsKey(a.returnType)) {
                throw new Exception("Cannot perform operation: " + PROGRAM.uniOperandToString[type] + " on type: " + PROGRAM.returnToString[a.returnType]);
            }
            return UniOperations[type][a.returnType](a);
        }

        void AddBiOperation<T, U>(BiOperand type, Func<T, U, object> resolver) {
            if (!BiOperations.ContainsKey(type)) BiOperations[type] = NewDictionary<Return, Dictionary<Return, BiOperation>>();
            foreach (Return a in GetTypes(typeof(T))) {
                foreach(Return b in GetTypes(typeof(U))) {
                    if (!BiOperations[type].ContainsKey(a)) BiOperations[type][a] = NewDictionary<Return, BiOperation>();
                    BiOperations[type][a][b] = (t, u) => ResolvePrimitive(resolver((T)t.value, (U)u.value));
                }
            }
        }

        void AddUniOperation<T>(UniOperand type, Func<T,object> resolver) {
            if (!UniOperations.ContainsKey(type)) UniOperations[type] = NewDictionary<Return, UniOperation>();
            foreach (Return a in GetTypes(typeof(T))) {
                UniOperations[type][a] = t => ResolvePrimitive(resolver((T)t.value));
            }
        }

        public void InitializeOperators() {
            //Object
            AddUniOperation<KeyedList>(UniOperand.RANDOM, a => a.GetValue(ResolvePrimitive(randomGenerator.Next(a.keyedValues.Count))).GetValue().value);
            AddUniOperation<object>(UniOperand.CAST, a => a);
            AddUniOperation<string>(UniOperand.CAST, a => {
                Primitive output;
                return ParsePrimitive(a, out output) ? output.value : a;
            });

            //List
            AddBiOperation<KeyedList, object>(BiOperand.ADD, (a, b) => Combine(a, b));
            AddBiOperation<object, KeyedList>(BiOperand.ADD, (a, b) => Combine(a, b));
            AddBiOperation<KeyedList, object>(BiOperand.SUBTRACT, (a, b) => a.Remove(CastList(ResolvePrimitive(b))));
            AddBiOperation<float, float>(BiOperand.RANGE, (a, b) => {
                var range = Enumerable.Range((int)Math.Min(a, b), (int)(Math.Abs(b - a) + 1)).Select(i => GetStaticVariable(i));
                if (a > b) range = range.Reverse();
                return NewKeyedList(range);
            });
            AddBiOperation<string, string>(BiOperand.SPLIT, (a, b) => NewKeyedList(a.Split(NewList(CastString(ResolvePrimitive(b))).ToArray(), StringSplitOptions.None).Select(GetStaticVariable)));
            AddUniOperation<KeyedList>(UniOperand.KEYS, a => a.Keys());
            AddUniOperation<KeyedList>(UniOperand.VALUES, a => a.Values());
            AddUniOperation<KeyedList>(UniOperand.REVERSE, a => NewKeyedList(a.keyedValues.Select(b => b).Reverse()));
            AddUniOperation<KeyedList>(UniOperand.SORT, a => NewKeyedList(a.keyedValues.OrderBy(k => k)));
            AddUniOperation<KeyedList>(UniOperand.SHUFFLE, a => NewKeyedList(a.keyedValues.OrderBy(k => randomGenerator.Next())));

            //Booleans
            AddUniOperation<bool>(UniOperand.REVERSE, a => !a);
            AddBiOperation<bool, bool>(BiOperand.AND, (a, b) => a && b);
            AddBiOperation<bool, bool>(BiOperand.OR, (a, b) => a || b);
            AddBiOperation<bool, bool>(BiOperand.EXPONENT, (a, b) => a ^ b);
            AddBiOperation<String, object>(BiOperand.CONTAINS, (a, b) => a.Contains(CastString(ResolvePrimitive(b))));
            AddBiOperation<KeyedList, object>(BiOperand.CONTAINS, (a, b) => CastList(ResolvePrimitive(b)).keyedValues.Select(v => v.GetValue().value).Except(a.keyedValues.Select(v => v.GetValue().value)).Count() == 0);

            //Comparisons
            AddBiOperation<bool, bool>(BiOperand.COMPARE, (a, b) => a.CompareTo(b));
            AddBiOperation<string, string>(BiOperand.COMPARE, (a, b) => a.CompareTo(b));
            AddBiOperation<float, float>(BiOperand.COMPARE, (a, b) => a.CompareTo(b));
            AddBiOperation<Vector3D, Vector3D>(BiOperand.COMPARE, (a, b) => !a.Equals(b));
            AddBiOperation<Color, Color>(BiOperand.COMPARE, (a, b) => a.PackedValue.CompareTo(b.PackedValue));
            AddBiOperation<Vector3D, float>(BiOperand.COMPARE, (a, b) => CastNumber(ResolvePrimitive(a.Length())).CompareTo(b));
            AddBiOperation<float, Vector3D>(BiOperand.COMPARE, (a, b) => a.CompareTo(CastNumber(ResolvePrimitive(b.Length()))));
            AddBiOperation<KeyedList, KeyedList>(BiOperand.COMPARE, (a, b) => !Enumerable.SequenceEqual(a.keyedValues, b.keyedValues));

            //Numeric
            AddUniOperation<float>(UniOperand.REVERSE, a => -a);
            AddUniOperation<float>(UniOperand.ABS, a => Math.Abs(a));
            AddUniOperation<float>(UniOperand.SQRT, a => Math.Sqrt(a));
            AddUniOperation<float>(UniOperand.SIN, a => Math.Sin(a));
            AddUniOperation<float>(UniOperand.COS, a => Math.Cos(a));
            AddUniOperation<float>(UniOperand.TAN, a => Math.Tan(a));
            AddUniOperation<float>(UniOperand.ASIN, a => Math.Asin(a));
            AddUniOperation<float>(UniOperand.ACOS, a => Math.Acos(a));
            AddUniOperation<float>(UniOperand.ATAN, a => Math.Atan(a));
            AddUniOperation<float>(UniOperand.ROUND, a => Math.Round(a));
            AddUniOperation<float>(UniOperand.LN, a => Math.Log(a));
            AddUniOperation<Vector3D>(UniOperand.ABS, a => a.Length());
            AddUniOperation<Vector3D>(UniOperand.SQRT, a => Math.Sqrt(a.Length()));
            AddUniOperation<float>(UniOperand.TICKS, a => a / 60);
            AddUniOperation<float>(UniOperand.RANDOM, a => randomGenerator.Next((int)a));
            AddUniOperation<float>(UniOperand.SIGN, a => Math.Sign(a));
            AddBiOperation<float, float>(BiOperand.ADD, (a, b) => a + b);
            AddBiOperation<float, float>(BiOperand.SUBTRACT, (a, b) => a - b);
            AddBiOperation<float, float>(BiOperand.MULTIPLY, (a, b) => a * b);
            AddBiOperation<float, float>(BiOperand.DIVIDE, (a, b) => a / b);
            AddBiOperation<float, float>(BiOperand.MOD, (a, b) => a % b);
            AddBiOperation<float, float>(BiOperand.EXPONENT, (a, b) => Math.Pow(a, b));
            AddBiOperation<float, float>(BiOperand.ROUND, (a, b) => Math.Round(a, (int)b));
            AddBiOperation<Vector3D, Vector3D>(BiOperand.DOT, (a, b) => a.Dot(b));
            AddBiOperation<Color, Vector3D>(BiOperand.DOT, (a, b) => (a.ToVector3()*255).Dot(b));
            AddBiOperation<Vector3D, Vector3D>(BiOperand.EXPONENT, (a, b) => 180 * Math.Acos(a.Dot(b) / (a.Length() * b.Length())) / Math.PI);

            //String
            AddUniOperation<string>(UniOperand.REVERSE, a => new string(a.Reverse().ToArray()));
            AddUniOperation<object>(UniOperand.TYPE, a => PROGRAM.returnToString[ResolvePrimitive(a).returnType]);
            AddBiOperation<string, object>(BiOperand.ADD, (a, b) => a + CastString(ResolvePrimitive(b)));
            AddBiOperation<object, string>(BiOperand.ADD, (a, b) => CastString(ResolvePrimitive(a)) + b);
            AddBiOperation<string, string>(BiOperand.SUBTRACT, (a, b) => a.Contains(b) ? a.Remove(a.IndexOf(b)) + a.Substring(a.IndexOf(b) + b.Length) :  a);
            AddBiOperation<string, string>(BiOperand.MOD, (a, b) => a.Replace(b, ""));
            AddBiOperation<string, float>(BiOperand.SUBTRACT, (a, b) => b >= a.Length ? "" : a.Substring(0, (int)(a.Length - b)));
            AddBiOperation<object, string>(BiOperand.CAST, (a, b) => castMap[b](ResolvePrimitive(a)));
            AddBiOperation<KeyedList, string>(BiOperand.JOIN, (a, b) => string.Join(CastString(ResolvePrimitive(b)), a.keyedValues.Select(v => CastString(v.GetValue()))));

            //Vector
            AddUniOperation<Vector3D>(UniOperand.SIGN, a => Vector3D.Sign(a));
            AddUniOperation<Vector3D>(UniOperand.ROUND, a => Vector3D.Round(a, 0));
            AddUniOperation<Vector3D>(UniOperand.REVERSE, a => -a);
            AddBiOperation<Vector3D, Vector3D>(BiOperand.ADD, (a,b) => a + b);
            AddBiOperation<Vector3D, Vector3D>(BiOperand.SUBTRACT, (a, b) => a - b);
            AddBiOperation<Vector3D, float>(BiOperand.ADD, (a, b) => Vector3D.Multiply(a, (a.Length() + b) / a.Length()));
            AddBiOperation<Vector3D, float>(BiOperand.SUBTRACT, (a, b) => Vector3D.Multiply(a, (a.Length() - b) / a.Length()));
            AddBiOperation<Vector3D, Vector3D>(BiOperand.MULTIPLY, (a, b) => Vector3D.Cross(a, b));
            AddBiOperation<Vector3D, Vector3D>(BiOperand.DIVIDE, (a, b) => Vector3D.Divide(a, b.Length()));
            AddBiOperation<Vector3D, float>(BiOperand.MULTIPLY, (a, b) => Vector3D.Multiply(a, b));
            AddBiOperation<Vector3D, float>(BiOperand.DIVIDE, (a, b) => Vector3D.Divide(a, b));
            AddBiOperation<float, Vector3D>(BiOperand.MULTIPLY, (a, b) => Vector3D.Multiply(b, a));
            AddBiOperation<Vector3D, float>(BiOperand.ROUND, (a, b) => Vector3D.Round(a, (int)b));

            //Modding a vector by another vector is asking to perform vector rejection.
            //See https://en.wikipedia.org/wiki/Vector_projection
            AddBiOperation<Vector3D, Vector3D>(BiOperand.MOD, (a, b) => Vector3D.Reject(a, b));

            //Color
            AddUniOperation<Color>(UniOperand.REVERSE, a => new Color(255 - a.R, 255 - a.G, 255 - a.B));
            AddBiOperation<Color, Color>(BiOperand.ADD, (a, b) => a + b);
            AddBiOperation<Color, Color>(BiOperand.SUBTRACT, (a, b) => new Color(a.R - b.R, a.G - b.G, a.B - b.B));
            AddBiOperation<Color, float>(BiOperand.MULTIPLY, (a, b) => Color.Multiply(a, b));
            AddBiOperation<float, Color>(BiOperand.MULTIPLY, (a, b) => Color.Multiply(b, a));
            AddBiOperation<Color, float>(BiOperand.DIVIDE, (a, b) => Color.Multiply(a, 1/b));
        }

        static List<Variable> GetVariables(params object[] o) => o.ToList().Select(p => GetStaticVariable(p)).ToList();
        static KeyedList Combine(object a, object b) => CastList(ResolvePrimitive(a)).Combine(CastList(ResolvePrimitive(b)));
    }
}
