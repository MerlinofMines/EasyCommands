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

        public abstract class Primitive {
            Return type;

            protected Primitive(Return type) {
                this.type = type;
            }

            public abstract object GetValue();
            public virtual object GetDeepCopyValue() => GetValue();

            public Return GetPrimitiveType() {
                return type;
            }

            public Primitive Plus(Primitive p) {
                return PROGRAM.PerformOperation(BiOperand.ADD, this, p);
            }

            public Primitive Minus(Primitive p) {
                return PROGRAM.PerformOperation(BiOperand.SUBTACT, this, p);
            }

            public Primitive Multiply(Primitive p) {
                return PROGRAM.PerformOperation(BiOperand.MULTIPLY, this, p);
            }

            public Primitive Divide(Primitive p) {
                return PROGRAM.PerformOperation(BiOperand.DIVIDE, this, p);
            }

            public int Compare(Primitive p) {
                return Convert.ToInt32(CastNumber(PROGRAM.PerformOperation(BiOperand.COMPARE, this, p)).GetTypedValue());
            }

            public Primitive Not() {
                return PROGRAM.PerformOperation(UniOperand.NOT, this);
            }

            public Primitive DeepCopy() {
                return ResolvePrimitive(GetDeepCopyValue());
            }
        }

        static Dictionary<Type, Return> PrimitiveTypeMap = new Dictionary<Type, Return>() {
            { typeof(bool), Return.BOOLEAN },
            { typeof(string), Return.STRING},
            { typeof(float), Return.NUMERIC },
            { typeof(int), Return.NUMERIC },
            { typeof(double), Return.NUMERIC },
            { typeof(Vector3D), Return.VECTOR},
            { typeof(Color), Return.COLOR },
            { typeof(KeyedList), Return.LIST }
        };

        static Dictionary<string, Func<Primitive, Primitive>> castMap = new Dictionary<string, Func<Primitive, Primitive>>() {
            { "bool", CastBoolean },
            { "string", CastString },
            { "number", CastNumber },
            { "vector", CastVector },
            { "color", CastColor },
            { "list", CastList }
        };

        static Dictionary<String, Color> colors = new Dictionary<String, Color>{
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
            return new List<Return> { primitiveType };
        }

        static Primitive ResolvePrimitive(object o) {
            if (o is bool) {
                return new BooleanPrimitive((bool)o);
            } else if (o is float) {
                return new NumberPrimitive((float)o);
            } else if (o is double) {
                return new NumberPrimitive(Convert.ToSingle((double)o));
            } else if (o is int) {
                return new NumberPrimitive(Convert.ToSingle((int)o));
            } else if (o is string) {
                return new StringPrimitive((string)o);
            } else if (o is Vector3D) {
                return new VectorPrimitive((Vector3D)o);
            } else if (o is Color) {
                return new ColorPrimitive((Color)o);
            } else if (o is KeyedList) {
                return new ListPrimitive((KeyedList)o);
            }
            else throw new Exception("Cannot convert type: " + o.GetType() + " to primitive");
        }

        public static BooleanPrimitive CastBoolean(Primitive p) {
            switch (p.GetPrimitiveType()) {
                case Return.BOOLEAN: return (BooleanPrimitive)p;
                case Return.NUMERIC: return new BooleanPrimitive((float)p.GetValue() > 0);
                case Return.STRING: return new BooleanPrimitive(bool.Parse((string)p.GetValue()));
                default: throw new Exception("Cannot convert Primitive Type: " + p.GetPrimitiveType() + " To Boolean");
            }
        }

        public static NumberPrimitive CastNumber(Primitive p) {
            switch (p.GetPrimitiveType()) {
                case Return.BOOLEAN: return new NumberPrimitive((bool)p.GetValue() ? 1 : 0);
                case Return.NUMERIC: return (NumberPrimitive)p;
                case Return.STRING: return new NumberPrimitive(float.Parse((string)p.GetValue()));
                case Return.VECTOR: return new NumberPrimitive((float)((Vector3D)p.GetValue()).Length());
                default: throw new Exception("Cannot convert Primitive Type: " + p.GetPrimitiveType() + " To Number");
            }
        }

        public static StringPrimitive CastString(Primitive p) {
            switch (p.GetPrimitiveType()) {
                case Return.VECTOR:
                    return new StringPrimitive(VectorToString(CastVector(p).GetTypedValue()));
                case Return.COLOR:
                    return new StringPrimitive(ColorToString(CastColor(p).GetTypedValue()));
                case Return.LIST:
                    return new StringPrimitive(CastList(p).GetTypedValue().Print());
                default: return new StringPrimitive(p.GetValue().ToString());
            }
        }

        public static VectorPrimitive CastVector(Primitive p) {
            switch (p.GetPrimitiveType()) {
                case Return.VECTOR: return (VectorPrimitive)p;
                case Return.STRING:
                    Vector3D vector;
                    if (GetVector((String)p.GetValue(), out vector)) return new VectorPrimitive(vector);
                    goto default;
                default: throw new Exception("Cannot convert Primitive Type: " + p.GetPrimitiveType() + " to Vector");
            }
        }

        public static ColorPrimitive CastColor(Primitive p) {
            switch (p.GetPrimitiveType()) {
                case Return.COLOR: return (ColorPrimitive)p;
                case Return.STRING:
                    Color color;
                    if (GetColor((String)p.GetValue(), out color)) return new ColorPrimitive(color);
                    goto default;
                case Return.NUMERIC:
                    return new ColorPrimitive(new Color((float)p.GetValue()));
                case Return.VECTOR:
                    var vector = CastVector(p).GetTypedValue();
                    return new ColorPrimitive(new Color((int)vector.X, (int)vector.Y, (int)vector.Z));
                default: throw new Exception("Cannot convert Primitive type: " + p.GetPrimitiveType() + " to Color");
            }
        }

        public static ListPrimitive CastList(Primitive p) => new ListPrimitive(AsList(p));
        public static KeyedList AsList(Primitive p) => p.GetPrimitiveType() == Return.LIST ? (KeyedList)p.GetValue() : new KeyedList(GetStaticVariable(p.GetValue()));

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
            List<double> components = new List<double>();
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

        public class SimplePrimitive<T> : Primitive {
            public T value;

            public SimplePrimitive(Return type, T v) : base(type) {
                value = v;
            }

            public override object GetValue() => value;
            public T GetTypedValue() => (T) GetValue();
        }

        public class BooleanPrimitive : SimplePrimitive<bool> {
            public BooleanPrimitive(bool value) : base(Return.BOOLEAN, value) {}
        }

        public class NumberPrimitive : SimplePrimitive<float> {
            public NumberPrimitive(float number) : base(Return.NUMERIC, number) {}
        }

        public class StringPrimitive : SimplePrimitive<string> {
            public StringPrimitive(String value) : base(Return.STRING, value) {}
            public override object GetValue() => value.Replace("\\n", "\n");
        }

        public class VectorPrimitive : SimplePrimitive<Vector3D> {
            public VectorPrimitive(Vector3D vector) : base(Return.VECTOR, vector) {}
        }

        public class ColorPrimitive : SimplePrimitive<Color> {
            public ColorPrimitive(Color color) : base(Return.COLOR, color) {}
        }

        public class ListPrimitive : SimplePrimitive<KeyedList> {
            public ListPrimitive(KeyedList list) : base(Return.LIST, list) { }
            public override object GetDeepCopyValue() => GetTypedValue().DeepCopy();
        }
    }
}