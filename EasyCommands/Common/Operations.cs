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
        private static bool OperatorsInitialized = false;
        static Dictionary<BiOperand, Dictionary<Return, Dictionary<Return, BiOperation>>> BiOperations 
            = new Dictionary<BiOperand, Dictionary<Return, Dictionary<Return, BiOperation>>>();

        static Dictionary<UniOperand, Dictionary<Return, UniOperation>> UniOperations
            = new Dictionary<UniOperand, Dictionary<Return, UniOperation>>();

        public static Primitive PerformOperation(BiOperand type, Primitive a, Primitive b) {
            InitializeOperators();
            if (!BiOperations.ContainsKey(type)
                || !BiOperations[type].ContainsKey(a.GetPrimitiveType())
                || !BiOperations[type][a.GetPrimitiveType()].ContainsKey(b.GetPrimitiveType())) {
                throw new Exception("Cannot perform operation: " + type + " on types: " + a.GetPrimitiveType() + ", " + b.GetPrimitiveType());
            }
            return BiOperations[type][a.GetPrimitiveType()][b.GetPrimitiveType()](a, b);
        }

        public static Primitive PerformOperation(UniOperand type, Primitive a) {
            InitializeOperators();
            if (!UniOperations.ContainsKey(type)
                || !UniOperations[type].ContainsKey(a.GetPrimitiveType())) { 
                throw new Exception("Cannot perform operation: " + type + " on type: " + a.GetPrimitiveType());
            }
            return UniOperations[type][a.GetPrimitiveType()](a);
        }

        static void AddBiOperation<T, U>(BiOperand type, Func<T, U, object> resolver) {
            Return a = GetType(typeof(T));
            Return b = GetType(typeof(U));

            if (!BiOperations.ContainsKey(type)) BiOperations[type] = new Dictionary<Return, Dictionary<Return, BiOperation>>();
            if (!BiOperations[type].ContainsKey(a)) BiOperations[type][a] = new Dictionary<Return, BiOperation>();
            BiOperations[type][a][b] = SimpleBiOperand(resolver);
        }

        static void AddUniOperation<T>(UniOperand type, Func<T,object> resolver) {
            Return a = GetType(typeof(T));
            if (!UniOperations.ContainsKey(type)) UniOperations[type] = new Dictionary<Return, UniOperation>();
            UniOperations[type][a] = SimpleUniOperand(resolver);
        }

        public static void InitializeOperators() {
            if (OperatorsInitialized) return;
            //Booleans
            AddUniOperation<bool>(UniOperand.NOT, a => !a);
            AddBiOperation<bool, bool>(BiOperand.AND, (a,b) => a && b);
            AddBiOperation<bool, bool>(BiOperand.OR, (a, b) => a || b);
            AddBiOperation<bool, bool>(BiOperand.COMPARE, (a, b) => a.CompareTo(b));
            AddBiOperation<string, string>(BiOperand.COMPARE, (a, b) => a.CompareTo(b));
            AddBiOperation<float, float>(BiOperand.COMPARE, (a, b) => a.CompareTo(b));
            AddBiOperation<Vector3D, Vector3D>(BiOperand.COMPARE, (a, b) => a.Length().CompareTo(b.Length()));
            AddBiOperation<Color, Color>(BiOperand.COMPARE, (a, b) => a.PackedValue.CompareTo(b.PackedValue));
            AddBiOperation<Vector3D, float>(BiOperand.COMPARE, (a, b) => a.Length().CompareTo(b));
            AddBiOperation<float, Vector3D>(BiOperand.COMPARE, (a, b) => a.CompareTo(b.Length()));

            //Numeric
            AddUniOperation<float>(UniOperand.NOT, a => -a);
            AddUniOperation<float>(UniOperand.ABS, a => Math.Abs(a));
            AddUniOperation<float>(UniOperand.SQRT, a => Math.Sqrt(a));
            AddUniOperation<float>(UniOperand.SIN, a => Math.Sin(a));
            AddUniOperation<float>(UniOperand.COS, a => Math.Cos(a));
            AddUniOperation<float>(UniOperand.TAN, a => Math.Tan(a));
            AddUniOperation<float>(UniOperand.ASIN, a => Math.Asin(a));
            AddUniOperation<float>(UniOperand.ACOS, a => Math.Acos(a));
            AddUniOperation<float>(UniOperand.ATAN, a => Math.Atan(a));
            AddUniOperation<float>(UniOperand.ROUND, a => Math.Round(a));
            AddUniOperation<Vector3D>(UniOperand.ABS, a => a.Length());
            AddUniOperation<Vector3D>(UniOperand.SQRT, a => Math.Sqrt(a.Length()));
            AddBiOperation<float, float>(BiOperand.ADD, (a, b) => a + b);
            AddBiOperation<float, float>(BiOperand.SUBTACT, (a, b) => a - b);
            AddBiOperation<float, float>(BiOperand.MULTIPLY, (a, b) => a * b);
            AddBiOperation<float, float>(BiOperand.DIVIDE, (a, b) => a / b);
            AddBiOperation<float, float>(BiOperand.MOD, (a, b) => a % b);
            AddBiOperation<Vector3D, Vector3D>(BiOperand.DOT, (a, b) => a.Dot(b));

            //String
            AddBiOperation<string, string>(BiOperand.ADD, (a, b) => a + b);
            AddBiOperation<string, bool>(BiOperand.ADD, (a, b) => a + b);
            AddBiOperation<string, float>(BiOperand.ADD, (a, b) => a + b);
            AddBiOperation<string, Vector3D>(BiOperand.ADD, (a, b) => a + VectorToString(b));
            AddBiOperation<string, Color>(BiOperand.ADD, (a, b) => b + a.ToString());
            AddBiOperation<bool, string>(BiOperand.ADD, (a, b) => a + b);
            AddBiOperation<float, string>(BiOperand.ADD, (a, b) => a + b);
            AddBiOperation<Color, string>(BiOperand.ADD, (a, b) => a.ToString() + b);
            AddBiOperation<Vector3D, string>(BiOperand.ADD, (a, b) => VectorToString(a) + b);
            AddBiOperation<string, string>(BiOperand.SUBTACT, (a, b) => a.Replace(b, ""));
            AddBiOperation<string, string>(BiOperand.MOD, (a, b) => a.Replace(b, ""));
            AddBiOperation<string, float>(BiOperand.SUBTACT, (a, b) => a.Substring(Convert.ToInt32(b)));

            //Vector
            //TODO add dot product, projection
            AddUniOperation<Vector3D>(UniOperand.NOT, a => -a);
            AddBiOperation<Vector3D, Vector3D>(BiOperand.ADD, (a,b) => Vector3D.Add(a,b));
            AddBiOperation<Vector3D, Vector3D>(BiOperand.SUBTACT, (a, b) => Vector3D.Subtract(a, b));
            AddBiOperation<Vector3D, Vector3D>(BiOperand.MULTIPLY, (a, b) => Vector3D.Cross(a, b));
            AddBiOperation<Vector3D, Vector3D>(BiOperand.DIVIDE, (a, b) => Vector3D.Divide(a, b.Length()));
            AddBiOperation<Vector3D, float>(BiOperand.MULTIPLY, (a, b) => Vector3D.Multiply(a, b));
            AddBiOperation<Vector3D, float>(BiOperand.DIVIDE, (a, b) => Vector3D.Divide(a, b));
            AddBiOperation<float, Vector3D>(BiOperand.MULTIPLY, (a, b) => Vector3D.Multiply(b, a));
            AddBiOperation<float, Vector3D>(BiOperand.DIVIDE, (a, b) => Vector3D.Divide(b, a));

            //Modding a vector by another vector is asking to perform vector rejection.
            //See https://en.wikipedia.org/wiki/Vector_projection
            AddBiOperation<Vector3D, Vector3D>(BiOperand.MOD, (a, b) => Vector3D.Reject(a, b));

            //Color
            AddUniOperation<Color>(UniOperand.NOT, a => new Color(255 - a.R, 255 - a.G, 255 - a.B));
            AddBiOperation<Color, Color>(BiOperand.ADD, (a, b) => a + b);
            AddBiOperation<Color, Color>(BiOperand.SUBTACT, (a, b) => new Color(Math.Max(a.R - b.R, 0), Math.Max(a.G - b.G,0), Math.Max(a.B - b.B, 0)));
            AddBiOperation<Color, float>(BiOperand.MULTIPLY, (a, b) => Color.Multiply(a, b));
            AddBiOperation<float, Color>(BiOperand.MULTIPLY, (a, b) => Color.Multiply(b, a));
            AddBiOperation<Color, float>(BiOperand.DIVIDE, (a, b) => Color.Multiply(a, 1/b));

            OperatorsInitialized = true;
        }

        static UniOperation SimpleUniOperand<T>(Func<T,object> resolver) {
            return (a) => {
                object result = resolver((T)a.GetValue());
                return ResolvePrimitive(result);
            };
        }

        static BiOperation SimpleBiOperand<T,U>(Func<T,U,object> resolver) {
            return (a, b) => {
                object result = resolver((T)a.GetValue(), (U)b.GetValue());
                return ResolvePrimitive(result);
            };
        }
    }
}
