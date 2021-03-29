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
        static Dictionary<BiOperandType, Dictionary<PrimitiveType, Dictionary<PrimitiveType, BiOperation>>> BiOperations 
            = new Dictionary<BiOperandType, Dictionary<PrimitiveType, Dictionary<PrimitiveType, BiOperation>>>();

        static Dictionary<UniOperandType, Dictionary<PrimitiveType, UniOperation>> UniOperations
            = new Dictionary<UniOperandType, Dictionary<PrimitiveType, UniOperation>>();

        public static Primitive PerformOperation(BiOperandType type, Primitive a, Primitive b) {
            if (!OperatorsInitialized) InitializeOperators();
            if (!BiOperations.ContainsKey(type)
                || !BiOperations[type].ContainsKey(a.GetPrimitiveType())
                || !BiOperations[type][a.GetPrimitiveType()].ContainsKey(b.GetPrimitiveType())) {
                throw new Exception("Cannot perform operation: " + type + " on types: " + a.GetPrimitiveType() + ", " + b.GetPrimitiveType());
            }
            return BiOperations[type][a.GetPrimitiveType()][b.GetPrimitiveType()](a, b);
        }

        public static Primitive PerformOperation(UniOperandType type, Primitive a) {
            if (!OperatorsInitialized) InitializeOperators();
            if (!UniOperations.ContainsKey(type)
                || !UniOperations[type].ContainsKey(a.GetPrimitiveType())) { 
                throw new Exception("Cannot perform operation: " + type + " on type: " + a.GetPrimitiveType());
            }
            return UniOperations[type][a.GetPrimitiveType()](a);
        }

        static void AddBiOperation<T, U>(BiOperandType type, Func<T, U, object> resolver) {
            PrimitiveType a = GetType(typeof(T));
            PrimitiveType b = GetType(typeof(U));

            if (!BiOperations.ContainsKey(type)) BiOperations[type] = new Dictionary<PrimitiveType, Dictionary<PrimitiveType, BiOperation>>();
            if (!BiOperations[type].ContainsKey(a)) BiOperations[type][a] = new Dictionary<PrimitiveType, BiOperation>();
            BiOperations[type][a][b] = SimpleBiOperand(resolver);
        }

        static void AddUniOperation<T>(UniOperandType type, Func<T,object> resolver) {
            PrimitiveType a = GetType(typeof(T));
            if (!UniOperations.ContainsKey(type)) UniOperations[type] = new Dictionary<PrimitiveType, UniOperation>();
            UniOperations[type][a] = SimpleUniOperand(resolver);
        }

        public static void InitializeOperators() {

            //Booleans
            AddUniOperation<bool>(UniOperandType.NOT, a => !a);
            AddBiOperation<bool, bool>(BiOperandType.AND, (a,b) => a && b);
            AddBiOperation<bool, bool>(BiOperandType.OR, (a, b) => a || b);
            AddBiOperation<bool, bool>(BiOperandType.COMPARE, (a, b) => a.CompareTo(b));
            AddBiOperation<string, string>(BiOperandType.COMPARE, (a, b) => a.CompareTo(b));
            AddBiOperation<float, float>(BiOperandType.COMPARE, (a, b) => a.CompareTo(b));
            AddBiOperation<Vector3D, Vector3D>(BiOperandType.COMPARE, (a, b) => a.Length().CompareTo(b.Length()));
            AddBiOperation<Vector3D, float>(BiOperandType.COMPARE, (a, b) => a.Length().CompareTo(b));
            AddBiOperation<float, Vector3D>(BiOperandType.COMPARE, (a, b) => a.CompareTo(b.Length()));

            //Numeric
            AddUniOperation<float>(UniOperandType.NOT, a => -a);
            AddBiOperation<float, float>(BiOperandType.ADD, (a, b) => a + b);
            AddBiOperation<float, float>(BiOperandType.SUBTACT, (a, b) => a - b);
            AddBiOperation<float, float>(BiOperandType.MULTIPLY, (a, b) => a * b);
            AddBiOperation<float, float>(BiOperandType.DIVIDE, (a, b) => a / b);
            AddBiOperation<float, float>(BiOperandType.MOD, (a, b) => a % b);

            //String
            AddBiOperation<string, string>(BiOperandType.ADD, (a, b) => a + b);
            AddBiOperation<string, bool>(BiOperandType.ADD, (a, b) => a + b);
            AddBiOperation<string, float>(BiOperandType.ADD, (a, b) => a + b);
            AddBiOperation<string, Vector3D>(BiOperandType.ADD, (a, b) => a + ToString(b));
            AddBiOperation<bool, string>(BiOperandType.ADD, (a, b) => a + b);
            AddBiOperation<float, string>(BiOperandType.ADD, (a, b) => a + b);
            AddBiOperation<Vector3D, string>(BiOperandType.ADD, (a, b) => ToString(a) + b);
            AddBiOperation<string, string>(BiOperandType.SUBTACT, (a, b) => a.Replace(b, ""));
            AddBiOperation<string, string>(BiOperandType.MOD, (a, b) => a.Replace(b, ""));
            AddBiOperation<string, float>(BiOperandType.SUBTACT, (a, b) => a.Substring(Convert.ToInt32(b)));

            //Vector
            //TODO add dot product, projection
            AddUniOperation<Vector3D>(UniOperandType.NOT, a => -a);
            AddBiOperation<Vector3D, Vector3D>(BiOperandType.ADD, (a,b) => Vector3D.Add(a,b));
            AddBiOperation<Vector3D, Vector3D>(BiOperandType.SUBTACT, (a, b) => Vector3D.Subtract(a, b));
            AddBiOperation<Vector3D, Vector3D>(BiOperandType.MULTIPLY, (a, b) => Vector3D.Cross(a, b));
            AddBiOperation<Vector3D, Vector3D>(BiOperandType.DIVIDE, (a, b) => Vector3D.Divide(a, b.Length()));
            AddBiOperation<Vector3D, float>(BiOperandType.MULTIPLY, (a, b) => Vector3D.Multiply(a, b));
            AddBiOperation<Vector3D, float>(BiOperandType.DIVIDE, (a, b) => Vector3D.Divide(a, b));
            AddBiOperation<float, Vector3D>(BiOperandType.MULTIPLY, (a, b) => Vector3D.Multiply(b, a));
            AddBiOperation<float, Vector3D>(BiOperandType.DIVIDE, (a, b) => Vector3D.Divide(b, a));

            //Modding a vector by another vector is asking to perform vector rejection.
            //See https://en.wikipedia.org/wiki/Vector_projection
            AddBiOperation<Vector3D, Vector3D>(BiOperandType.MOD, (a, b) => Vector3D.Reject(a, b));
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
