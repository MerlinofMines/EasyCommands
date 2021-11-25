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

        public class Primitive {
            public Return returnType;
            public object value;

            public Primitive(Return t, object v) {
                returnType = t;
                value = v;
            }

            public Primitive Plus(Primitive p) => PROGRAM.PerformOperation(BiOperand.ADD, this, p);
            public Primitive Minus(Primitive p) => PROGRAM.PerformOperation(BiOperand.SUBTACT, this, p);
            public Primitive Multiply(Primitive p) => PROGRAM.PerformOperation(BiOperand.MULTIPLY, this, p);
            public Primitive Divide(Primitive p) => PROGRAM.PerformOperation(BiOperand.DIVIDE, this, p);
            public int Compare(Primitive p) => Convert.ToInt32(CastNumber(PROGRAM.PerformOperation(BiOperand.COMPARE, this, p)));
            public Primitive Not() => PROGRAM.PerformOperation(UniOperand.NOT, this);
            public Primitive DeepCopy() => ResolvePrimitive((value is KeyedList) ? ((KeyedList)value).DeepCopy() : value);
        }

        static Dictionary<Type, Return> PrimitiveTypeMap = new Dictionary<Type, Return> {
            { typeof(bool), Return.BOOLEAN },
            { typeof(string), Return.STRING},
            { typeof(float), Return.NUMERIC },
            { typeof(int), Return.NUMERIC },
            { typeof(double), Return.NUMERIC },
            { typeof(Vector3D), Return.VECTOR},
            { typeof(Color), Return.COLOR },
            { typeof(KeyedList), Return.LIST }
        };

        static Dictionary<string, Func<Primitive, object>> castMap = new Dictionary<string, Func<Primitive, object>> {
            { "bool", p => CastBoolean(p) },
            { "string", CastString },
            { "number", p => CastNumber(p) },
            { "vector", p => CastVector(p) },
            { "color", p => CastColor(p) },
            { "list", CastList }
        };

        static Dictionary<String, Color> colors = new Dictionary<String, Color> {
            { "red", Color.Red },
            { "blue", Color.Blue },
            { "green", Color.Green },
            { "orange", Color.Orange },
            { "yellow", Color.Yellow },
            { "white", Color.White },
            { "black", Color.Black}
        };

        static List<Return> GetTypes(Type type) {
            if (type == typeof(object)) return ((Return[])Enum.GetValues(typeof(Return))).ToList();
            Return primitiveType;
            if (!PrimitiveTypeMap.TryGetValue(type, out primitiveType)) throw new Exception("No Primitive Type present for type: " + type);
            return NewList<Return>(primitiveType);
        }

        static Primitive ResolvePrimitive(object o) {
            var type = PrimitiveTypeMap[o.GetType()];
            if (o is double) {
                return new Primitive(type, Convert.ToSingle((double)o));
            } else if (o is int) {
                return new Primitive(type, Convert.ToSingle((int)o));
            } else return new Primitive(type, o);
        }

        public static bool CastBoolean(Primitive p) {
            switch (p.returnType) {
                case Return.BOOLEAN: return (bool)p.value;
                case Return.NUMERIC: return CastNumber(p) > 0;
                case Return.STRING: return bool.Parse(CastString(p));
                default: throw new Exception("Cannot convert Primitive Type: " + p.returnType + " To Boolean");
            }
        }

        public static float CastNumber(Primitive p) {
            switch (p.returnType) {
                case Return.BOOLEAN: return CastBoolean(p) ? 0 : -1;
                case Return.NUMERIC: return (float)p.value;
                case Return.STRING: return float.Parse(CastString(p));
                case Return.VECTOR: return (float)(CastVector(p)).Length();
                default: throw new Exception("Cannot convert Primitive Type: " + p.returnType + " To Number");
            }
        }

        public static string CastString(Primitive p) {
            var value = p.value.ToString();
            if (p.returnType == Return.VECTOR) value = VectorToString(CastVector(p));
            if (p.returnType == Return.COLOR) value = ColorToString(CastColor(p));
            if (p.returnType == Return.LIST) value = CastList(p).Print();
            return value.Replace("\\n", "\n");
        }

        public static Vector3D CastVector(Primitive p) {
            switch (p.returnType) {
                case Return.VECTOR: 
                    return (Vector3D)p.value;
                case Return.STRING:
                    Vector3D vector;
                    if (GetVector(CastString(p), out vector)) return vector;
                    goto default;
                default: throw new Exception("Cannot convert Primitive Type: " + p.returnType + " to Vector");
            }
        }

        public static Color CastColor(Primitive p) {
            switch (p.returnType) {
                case Return.COLOR: return (Color)p.value;
                case Return.STRING:
                    Color color;
                    if (GetColor(CastString(p), out color)) return color;
                    goto default;
                case Return.NUMERIC:
                    return new Color(CastNumber(p));
                case Return.VECTOR:
                    var vector = CastVector(p);
                    return new Color((int)vector.X, (int)vector.Y, (int)vector.Z);
                default: throw new Exception("Cannot convert Primitive type: " + p.returnType + " to Color");
            }
        }

        public static KeyedList CastList(Primitive p) => p.returnType == Return.LIST ? (KeyedList)p.value : new KeyedList(GetStaticVariable(p.value));

        public static bool GetColor(String s, out Color color) {
            Color? possibleColor = null;
            if (colors.ContainsKey(s.ToLower())) possibleColor = colors[s.ToLower()];
            else if (s.StartsWith("#") && s.Length == 7) {
                possibleColor = new Color(HexToInt(s.Substring(1, 2)), HexToInt(s.Substring(3, 2)), HexToInt(s.Substring(5, 2)));
            }
            color = possibleColor.GetValueOrDefault(Color.White);
            return possibleColor.HasValue;
        }

        public static bool GetVector(String s, out Vector3D vector) {
            vector = Vector3D.Zero;
            var components = NewList<double>();
            string[] vectorStrings = s.Split(':');

            foreach (string component in vectorStrings) {
                double result;
                if (Double.TryParse(component, out result)) components.Add(result);
            }

            if (components.Count() != 3) return false;
            vector = new Vector3D(components[0], components[1], components[2]);
            return true;
        }

        static string VectorToString(Vector3D vector) {
            return vector.X + ":" + vector.Y + ":" + vector.Z;
        }

        static string ColorToString(Color color) {
            return "#" + IntToHex(color.R) + IntToHex(color.G) + IntToHex(color.B);
        }

        static int HexToInt(string hex) {
            return int.Parse(hex.ToUpper(), System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        static string IntToHex(int hex) {
            return hex.ToString("X");
        }
    }
}