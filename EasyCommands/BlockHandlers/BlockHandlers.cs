using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public interface BlockPropertyHandler<T, U, V>
        {
            V GetHandledPropertyType();
            U GetPropertyValue(T block);
            void SetPropertyValue(T block, U value);
        }

        public interface BooleanPropertyHandler<T> : BlockPropertyHandler<T, bool, BooleanPropertyType>
        {

        }

        public interface StringPropertyHandler<T> : BlockPropertyHandler<T, String, StringPropertyType>
        {

        }

        public interface NumericPropertyHandler<T> : BlockPropertyHandler<T, float, NumericPropertyType>
        {
            void SetPropertyValue(T block, DirectionType DirectionType, float value);
            void IncrementPropertyValue(T block, float deltaValue);
            void IncrementPropertyValue(T block, DirectionType direction, float deltaValue);
            void MovePropertyValue(T block, DirectionType direction);
            void ReversePropertyValue(T block);
        }

        public interface BooleanBlockHandler
        {

        }

        public interface BooleanBlockHandler<T> : BooleanBlockHandler where T : class, IMyFunctionalBlock
        {
            BooleanPropertyType GetDefaultBooleanProperty();
            bool GetBooleanPropertyValue(T block, BooleanPropertyType property);
            void SetBooleanPropertyValue(T block, BooleanPropertyType property, bool value);
        }

        public interface StringBlockHandler
        {

        }

        public interface StringBlockHandler<T> : StringBlockHandler where T: class, IMyFunctionalBlock
        {
            StringPropertyType GetDefaultStringProperty();
            String GetStringPropertyValue(T block, StringPropertyType property);
            void SetStringPropertyValue(T block, StringPropertyType property, String value);
        }

        public interface NumericBlockHandler
        {

        }

        public interface NumericBlockHandler<T> : NumericBlockHandler where T : class, IMyFunctionalBlock
        {
            NumericPropertyType GetDefaultNumericProperty(DirectionType direction);
            DirectionType GetDefaultDirection();
            float GetNumericPropertyValue(T block, NumericPropertyType property);
            void SetNumericPropertyValue(T block, NumericPropertyType property, float value);
            void SetNumericPropertyValue(T block, NumericPropertyType property, DirectionType direction, float value);
            void IncrementNumericPropertyValue(T block, NumericPropertyType property, float deltaValue);
            void IncrementNumericPropertyValue(T block, NumericPropertyType property, DirectionType direction, float deltaValue);
            void MoveNumericPropertyValue(T block, NumericPropertyType property, DirectionType direction);
            void ReverseNumericPropertyValue(T block, NumericPropertyType property);
        }

        public class BaseBlockHandler<T> : BooleanBlockHandler<T>, StringBlockHandler<T> where T : class, IMyFunctionalBlock
        {
            Dictionary<BooleanPropertyType, BooleanPropertyHandler<T>> booleanPropertyHandlers = new Dictionary<BooleanPropertyType, BooleanPropertyHandler<T>>();
            Dictionary<StringPropertyType, StringPropertyHandler<T>> stringPropertyHandlers = new Dictionary<StringPropertyType, StringPropertyHandler<T>>();

            public BaseBlockHandler()
            {
                GetBooleanPropertyHandlers().ForEach(handler => booleanPropertyHandlers.Add(handler.GetHandledPropertyType(), handler));
                GetStringPropertyHandlers().ForEach(handler => stringPropertyHandlers.Add(handler.GetHandledPropertyType(), handler));
            }

            protected virtual List<BooleanPropertyHandler<T>> GetBooleanPropertyHandlers()
            {
                return new List<BooleanPropertyHandler<T>>() {
                        new OnOffPropertyHandler<T>()
                };
            }

            protected virtual List<StringPropertyHandler<T>> GetStringPropertyHandlers()
            {
                return new List<StringPropertyHandler<T>>() { };
            }

            public virtual BooleanPropertyType GetDefaultBooleanProperty()
            {
                return BooleanPropertyType.ON_OFF;
            }

            public virtual StringPropertyType GetDefaultStringProperty()
            {
                return StringPropertyType.NAME;
            }

            public bool GetBooleanPropertyValue(T block, BooleanPropertyType property)
            {
                return booleanPropertyHandlers[property].GetPropertyValue(block);
            }

            public string GetStringPropertyValue(T block, StringPropertyType property)
            {
                return stringPropertyHandlers[property].GetPropertyValue(block);
            }

            public void SetBooleanPropertyValue(T block, BooleanPropertyType property, bool value)
            {
                booleanPropertyHandlers[property].SetPropertyValue(block, value);
            }

            public void SetStringPropertyValue(T block, StringPropertyType property, String value)
            {
                stringPropertyHandlers[property].SetPropertyValue(block, value);
            }
        }

        public abstract class NumericBaseBlockHandler<T> : BaseBlockHandler<T>, NumericBlockHandler<T> where T : class, IMyFunctionalBlock
        {
            Dictionary<NumericPropertyType, NumericPropertyHandler<T>> numericPropertyHandlers = new Dictionary<NumericPropertyType, NumericPropertyHandler<T>>();

            public NumericBaseBlockHandler() : base()
            {
                GetNumericPropertyHandlers().ForEach(handler => numericPropertyHandlers.Add(handler.GetHandledPropertyType(), handler));
            }

            public abstract NumericPropertyType GetDefaultNumericProperty(DirectionType direction);
            public abstract DirectionType GetDefaultDirection();

            protected abstract List<NumericPropertyHandler<T>> GetNumericPropertyHandlers();

            public float GetNumericPropertyValue(T block, NumericPropertyType property)
            {
                return numericPropertyHandlers[property].GetPropertyValue(block);
            }

            public void SetNumericPropertyValue(T block, NumericPropertyType property, float value)
            {
                numericPropertyHandlers[property].SetPropertyValue(block, value);
            }

            public void SetNumericPropertyValue(T block, NumericPropertyType property, DirectionType direction, float value)
            {
                numericPropertyHandlers[property].SetPropertyValue(block, direction, value);
            }

            public void IncrementNumericPropertyValue(T block, NumericPropertyType property, float deltaValue)
            {
                numericPropertyHandlers[property].IncrementPropertyValue(block, deltaValue);
            }

            public void IncrementNumericPropertyValue(T block, NumericPropertyType property, DirectionType direction, float deltaValue)
            {
                numericPropertyHandlers[property].IncrementPropertyValue(block, direction, deltaValue);
            }

            public void MoveNumericPropertyValue(T block, NumericPropertyType property, DirectionType direction)
            {
                numericPropertyHandlers[property].MovePropertyValue(block, direction);
            }

            public void ReverseNumericPropertyValue(T block, NumericPropertyType property)
            {
                numericPropertyHandlers[property].ReversePropertyValue(block);
            }
        }

        public class OnOffPropertyHandler<T> : BooleanPropertyHandler<T> where T : class, IMyFunctionalBlock
        {
            public BooleanPropertyType GetHandledPropertyType()
            {
                return BooleanPropertyType.ON_OFF;
            }

            public bool GetPropertyValue(T block)
            {
                return block.Enabled;
            }

            public void SetPropertyValue(T block, bool value)
            {
                block.Enabled = value;
            }
        }
    }
}
