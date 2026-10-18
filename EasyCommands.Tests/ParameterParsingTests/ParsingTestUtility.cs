using IngameScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyCommands.Tests.ParameterParsingTests {
    class ParsingTestUtility {
        public static T GetDelegateProperty<T>(String propertyId, Delegate target) {
            var fields = target.Target.GetType().GetFields().Select(t => t.Name).ToList();
            return (T)target.Target.GetType().GetFields().Where(t => t.Name == propertyId).Select(t => t.GetValue(target.Target)).First();
        }

        static Delegate GetEvaluator(Program.IVariable variable) =>
            (Delegate)typeof(Program.ComputedVariable)
                .GetField("evaluate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .GetValue(variable);

        // Keep the parser's expression-kind assertions as well as its operand/value checks.
        public enum VariableKind { Comparison, Unary, Binary, ListCondition, BlockCondition, BlockProperty, ListAggregate, Index }

        public static bool IsExpression(Program.IVariable variable, VariableKind kind) {
            if (!(variable is Program.ComputedVariable)) return false;
            var fields = GetEvaluator(variable).Target.GetType().GetFields();
            if (kind == VariableKind.Unary || kind == VariableKind.Binary)
                return fields.Any(f => f.Name == "operand" && f.FieldType ==
                    (kind == VariableKind.Unary ? typeof(Program.UniOperand) : typeof(Program.BiOperand)));
            var expected = new Dictionary<VariableKind, string[]> {
                { VariableKind.Comparison, new[] { "a", "b", "comparator" } },
                { VariableKind.ListCondition, new[] { "aggregateCondition", "expectedList", "comparisonValue" } },
                { VariableKind.BlockCondition, new[] { "aggregateCondition", "blockCondition", "entityProvider" } },
                { VariableKind.BlockProperty, new[] { "aggregator", "property", "entityProvider" } },
                { VariableKind.ListAggregate, new[] { "aggregator", "expectedList" } },
                { VariableKind.Index, new[] { "expectedIndex" } }
            };
            return expected[kind].All(name => fields.Any(f => f.Name == name));
        }

        // Inspect captured operands without evaluating the expression.
        public static T GetVariableProperty<T>(String propertyId, Program.IVariable variable) =>
            GetDelegateProperty<T>(propertyId, GetEvaluator(variable));

        public static Program CreateProgram() {
            Program program = ProgramFactory.CreateProgram<Program>();
            Program.PROGRAM = program;
            program.SetGlobalVariable(Program.NUMBER_FORMAT, Program.GetStaticVariable("#0.########"));
            return program;
        }
    }
}
