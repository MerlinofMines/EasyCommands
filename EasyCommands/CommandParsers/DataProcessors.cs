using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        interface DataProcessor {
            void Clear();
            bool SetValue(object p);
            bool Satisfied();
            bool Left(object p);
            bool Right(object p);
        }

        //Required Data Processors
        class DataProcessor<T> : DataProcessor {
            T value;
            public bool left, right, required;
            public virtual T GetValue() => value;
            public virtual bool SetValue(object p) {
                var setValue = p is T && value == null;
                if (setValue) value = (T)p;
                return setValue;
            }
            public bool Left(object o) => left && SetValue(o);
            public bool Right(object o) => right && SetValue(o);
            public virtual bool Satisfied() => value != null;
            public virtual void Clear() => value = default(T);
        }

        static DataProcessor<T> requiredRight<T>() => required<T>(false, true);
        static DataProcessor<T> requiredLeft<T>() => required<T>(true, false);
        static DataProcessor<T> requiredEither<T>() => required<T>(true, true);
        static DataProcessor<T> required<T>(bool left, bool right) => new DataProcessor<T> {
            left = left,
            right = right
        };

        //Optional Data Processors
        class OptionalDataProcessor<T> : DataProcessor<Optional<T>> {
            T value;
            public override Optional<T> GetValue() => new Optional<T> { t = value };
            public override bool SetValue(object p) {
                var setValue = p is T && value == null;
                if (setValue) value = (T)p;
                return setValue;
            }
            public override bool Satisfied() => true;
            public override void Clear() => value = default(T);
        }

        public class Optional<T> {
            public T t;
            public bool HasValue() => t != null;
            public T GetValue(T defaultValue = default(T)) => t != null ? t : defaultValue;
        }

        static OptionalDataProcessor<T> optionalRight<T>() => optional<T>(false, true);
        static OptionalDataProcessor<T> optionalLeft<T>() => optional<T>(true, false);
        static OptionalDataProcessor<T> optionalEither<T>() => optional<T>(true, true);
        static OptionalDataProcessor<T> optional<T>(bool left, bool right) => new OptionalDataProcessor<T> {
            left = left,
            right = right
        };

        //ListDataProcessors
        class ListDataProcessor<T> : DataProcessor<List<T>> {
            List<T> values = NewList<T>();
            public override bool SetValue(object p) {
                if (p is T) values.Add((T)p);
                return p is T;
            }
            public override bool Satisfied() => !required || values.Count > 0;
            public override List<T> GetValue() => values;
            public override void Clear() => values.Clear();
        }

        static ListDataProcessor<T> rightList<T>(bool required) => list<T>(false, true, required);
        static ListDataProcessor<T> leftList<T>(bool required) => list<T>(true, false, required);
        static ListDataProcessor<T> eitherList<T>(bool required) => list<T>(true, true, required);
        static ListDataProcessor<T> list<T>(bool left, bool right, bool required) => new ListDataProcessor<T> {
            left = left,
            right = right,
            required = required
        };

        static bool AllSatisfied(params DataProcessor[] processors) => processors.ToList().TrueForAll(p => p.Satisfied());
    }
}
