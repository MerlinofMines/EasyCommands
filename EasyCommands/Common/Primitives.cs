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
            PrimitiveType type;

            protected Primitive(PrimitiveType type) {
                this.type = type;
            }

            public abstract object GetValue();

            public PrimitiveType GetPrimitiveType() {
                return type;
            }

            public Primitive Plus(Primitive p) {
                return PerformOperation(BiOperandType.ADD, this, p);
            }

            public Primitive Minus(Primitive p) {
                return PerformOperation(BiOperandType.ADD, this, p);
            }

            public Primitive Multiply(Primitive p) {
                return PerformOperation(BiOperandType.MULTIPLY, this, p);
            }

            public Primitive Divide(Primitive p) {
                return PerformOperation(BiOperandType.DIVIDE, this, p);
            }

            public int Compare(Primitive p) {
                return Convert.ToInt32(CastNumber(PerformOperation(BiOperandType.COMPARE, this, p)).GetNumericValue());
            }

            public Primitive Not() {
                return PerformOperation(UniOperandType.NOT, this);
            }
        }

        static Dictionary<Type, PrimitiveType> PrimitiveTypeMap = new Dictionary<Type, PrimitiveType>() {
            { typeof(bool), PrimitiveType.BOOLEAN },
            { typeof(string), PrimitiveType.STRING},
            { typeof(float), PrimitiveType.NUMERIC },
            { typeof(int), PrimitiveType.NUMERIC },
            { typeof(double), PrimitiveType.NUMERIC },
            { typeof(Vector3D), PrimitiveType.VECTOR},
        };

        static PrimitiveType GetType(Type type) {
            PrimitiveType primitiveType;
            if (!PrimitiveTypeMap.TryGetValue(type, out primitiveType)) throw new Exception("No Primitive Type present for type: " + type);
            return primitiveType;
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
            } else throw new Exception("Cannot convert type: " + o.GetType() + " to primitive");
        }

        public static BooleanPrimitive CastBoolean(Primitive p) {
            switch (p.GetPrimitiveType()) {
                case PrimitiveType.BOOLEAN: return new BooleanPrimitive((bool)p.GetValue());
                case PrimitiveType.NUMERIC: return new BooleanPrimitive((float)p.GetValue() > 0);
                case PrimitiveType.STRING: return new BooleanPrimitive(bool.Parse((string)p.GetValue()));
                default: throw new Exception("Cannot convert Primitive Type: " + p.GetPrimitiveType() + " To Boolean");
            }
        }

        public static NumberPrimitive CastNumber(Primitive p) {
            switch (p.GetPrimitiveType()) {
                case PrimitiveType.BOOLEAN: return new NumberPrimitive((bool)p.GetValue() ? 1 : 0);
                case PrimitiveType.NUMERIC: return new NumberPrimitive((float)p.GetValue());
                case PrimitiveType.STRING: return new NumberPrimitive(float.Parse((string)p.GetValue()));
                case PrimitiveType.VECTOR: return new NumberPrimitive((float)((Vector3D)p.GetValue()).Length());
                default: throw new Exception("Cannot convert Primitive Type: " + p.GetPrimitiveType() + " To Boolean");
            }
        }

        public static StringPrimitive CastString(Primitive p) {
            switch (p.GetPrimitiveType()) {
                case PrimitiveType.VECTOR:
                    Vector3D vector = CastVector(p).GetVectorValue();
                    return new StringPrimitive(ToString(vector));
                default: return new StringPrimitive(p.GetValue().ToString());
            }
        }

        static string ToString(Vector3D vector) {
            return vector.X + ":" + vector.Y + ":" + vector.Z;
        }

        public static VectorPrimitive CastVector(Primitive p) {
            switch (p.GetPrimitiveType()) {
                case PrimitiveType.VECTOR: return new VectorPrimitive(((VectorPrimitive)p).GetVectorValue());
                case PrimitiveType.STRING:
                    Vector3D vector;
                    if (GetVector((String)p.GetValue(), out vector)) return new VectorPrimitive(vector);
                    goto default;
                default: throw new Exception("Cannot convert Primitive Type: " + p.GetPrimitiveType() + " To Vector");
            }
        }

        public class BooleanPrimitive : Primitive {
            private bool value;

            public BooleanPrimitive(bool value) : base(PrimitiveType.BOOLEAN) { this.value = value; }

            public override object GetValue() {
                return value;
            }

            public bool GetBooleanValue() {
                return value;
            }
        }

        public class NumberPrimitive : Primitive {
            float number;

            public NumberPrimitive(float number) : base(PrimitiveType.NUMERIC) {
                this.number = number;
            }

            public override object GetValue() {
                return number;
            }

            public float GetNumericValue() {
                return number;
            }
        }

        public class StringPrimitive : Primitive {
            String stringValue;

            public StringPrimitive(String value) : base(PrimitiveType.STRING) {
                stringValue = value;
            }

            public override object GetValue() {
                return stringValue;
            }

            public String GetStringValue() {
                return stringValue;
            }
        }

        public class VectorPrimitive : Primitive {
            Vector3D vector;

            public VectorPrimitive(Vector3D vector) : base(PrimitiveType.VECTOR) {
                this.vector = vector;
            }

            public override object GetValue() {
                return vector;
            }

            public Vector3D GetVectorValue() {
                return vector;
            }
        }

        //Add Color Primitive
    }
}
