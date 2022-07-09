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

        public class Primitive : IComparable<Primitive> {
            public Return returnType;
            public object value;

            public Primitive(Return t, object v) {
                returnType = t;
                value = v;
            }

            public Primitive Plus(Primitive p) => PerformOperation(BiOperand.ADD, this, p);
            public Primitive Minus(Primitive p) => PerformOperation(BiOperand.SUBTRACT, this, p);
            public Primitive Multiply(Primitive p) => PerformOperation(BiOperand.MULTIPLY, this, p);
            public Primitive Divide(Primitive p) => PerformOperation(BiOperand.DIVIDE, this, p);
            public int CompareTo(Primitive p) => Convert.ToInt32(CastNumber(PerformOperation(BiOperand.COMPARE, this, p)));
            public Primitive Not() => PerformOperation(UniOperand.REVERSE, this);
            public Primitive DeepCopy() => ResolvePrimitive((value as KeyedList)?.DeepCopy() ?? value);
        }

        delegate Object Converter(Primitive p);
        static KeyValuePair<T, Converter> CastFunction<T>(T r, Converter func) => KeyValuePair(r, func);
        static Converter Failure(Return returnType) => p => { throw new RuntimeException($"Cannot convert {returnToString[p.returnType]} {CastString(p)} to {returnToString[returnType]}"); };

        static Dictionary<Type, Dictionary<Return, Converter>> castFunctions = NewDictionary(
            KeyValuePair(typeof(bool), NewDictionary(
                CastFunction(Return.BOOLEAN, p => p.value),
                CastFunction(Return.NUMERIC, p => CastNumber(p) != 0),
                CastFunction(Return.STRING, p => {
                    Primitive primitive;
                    return ParsePrimitive(CastString(p), out primitive) && CastBoolean(primitive);
                }),
                CastFunction(Return.DEFAULT, Failure(Return.BOOLEAN))
            )),
            KeyValuePair(typeof(float), NewDictionary(
                CastFunction(Return.BOOLEAN, p => CastBoolean(p) ? 1.0f : 0.0f),
                CastFunction(Return.NUMERIC, p => (float)p.value),
                CastFunction(Return.STRING, p => float.Parse(CastString(p))),
                CastFunction(Return.VECTOR, p => (float)CastVector(p).Length()),
                CastFunction(Return.DEFAULT, Failure(Return.NUMERIC))
            )),
            KeyValuePair(typeof(string), NewDictionary(
                CastFunction(Return.NUMERIC, p => CastNumber(p).ToString(PROGRAM.globalVariables[NUMBER_FORMAT].GetValue().value + "")),
                CastFunction(Return.VECTOR, p => VectorToString(CastVector(p))),
                CastFunction(Return.COLOR, p => ColorToString(CastColor(p))),
                CastFunction(Return.LIST, p => CastList(p).Print()),
                CastFunction(Return.DEFAULT, p => "" + p.value)

            )),
            KeyValuePair(typeof(Vector3D), NewDictionary(
                CastFunction(Return.STRING, p => GetVector(CastString(p)).Value),
                CastFunction(Return.VECTOR, p => p.value),
                CastFunction(Return.COLOR, p => Vector(CastColor(p).R, CastColor(p).G, CastColor(p).B)),
                CastFunction(Return.DEFAULT, Failure(Return.VECTOR))
            )),
            KeyValuePair(typeof(Color), NewDictionary(
                CastFunction(Return.NUMERIC, p => new Color(CastNumber(p))),
                CastFunction(Return.STRING, p => GetColor(CastString(p)).Value),
                CastFunction(Return.VECTOR, p => new Color((int)CastVector(p).X, (int)CastVector(p).Y, (int)CastVector(p).Z)),
                CastFunction(Return.COLOR, p => p.value),
                CastFunction(Return.DEFAULT, Failure(Return.COLOR))
            )),
            KeyValuePair(typeof(KeyedList), NewDictionary(
                CastFunction(Return.LIST, p => p.value),
                CastFunction(Return.DEFAULT, p => NewKeyedList(Once(GetStaticVariable(p.value))))
            ))
            );

        static Dictionary<Type, Return> PrimitiveTypeMap = NewDictionary(
            KeyValuePair(typeof(bool), Return.BOOLEAN),
            KeyValuePair(typeof(string), Return.STRING),
            KeyValuePair(typeof(float), Return.NUMERIC),
            KeyValuePair(typeof(int), Return.NUMERIC),
            KeyValuePair(typeof(double), Return.NUMERIC),
            KeyValuePair(typeof(Vector3D), Return.VECTOR),
            KeyValuePair(typeof(Color), Return.COLOR),
            KeyValuePair(typeof(KeyedList), Return.LIST)
        );

        public static List<Return> GetTypes(Type type) =>
            type != typeof(object)
            ? NewList(PrimitiveTypeMap[type])
            : NewList((Return[])Enum.GetValues(typeof(Return)));

        public static Primitive ResolvePrimitive(object o) => new Primitive(PrimitiveTypeMap[o.GetType()], (o is double || o is int) ? Convert.ToSingle(o) : o);

        public static T Cast<T>(Primitive p) => (T)castFunctions[typeof(T)].GetValueOrDefault(p.returnType, castFunctions[typeof(T)][Return.DEFAULT])(p);

        public static bool CastBoolean(Primitive p) => Cast<bool>(p);
        public static float CastNumber(Primitive p) => Cast<float>(p);
        public static string CastString(Primitive p) => Cast<string>(p).Replace("\\n", "\n");
        public static Vector3D CastVector(Primitive p) => Cast<Vector3D>(p);
        public static Color CastColor(Primitive p) => Cast<Color>(p);
        public static KeyedList CastList(Primitive p) => Cast<KeyedList>(p);

        public static Color? GetColor(String s) =>
            (s.StartsWith("#") && s.Length == 7)
            ? new Color(HexToInt(s.Substring(1, 2)), HexToInt(s.Substring(3, 2)), HexToInt(s.Substring(5, 2)))
            : (colors.ContainsKey(s.ToLower()) ? colors[s.ToLower()] : (Color?)null);

        public static Vector3D? GetVector(String s) {
            var components = NewList<double>();
            foreach (string component in s.Split(':')) {
                double result;
                if (Double.TryParse(component, out result)) components.Add(result);
            }
            return components.Count() == 3 ? Vector(components[0], components[1], components[2]) : (Vector3D?)null;
        }

        static string VectorToString(Vector3D vector) => vector.X + ":" + vector.Y + ":" + vector.Z;
        static string ColorToString(Color color) => "#" + IntToHex(color.R) + IntToHex(color.G) + IntToHex(color.B);

        static int HexToInt(string hex) => int.Parse(hex.ToUpper(), System.Globalization.NumberStyles.AllowHexSpecifier);
        static string IntToHex(int hex) => hex.ToString("X2");
    }
}
