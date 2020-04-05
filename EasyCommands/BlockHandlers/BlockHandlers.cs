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
        public static class BlockHandlerRegistry
        {
            static readonly Dictionary<BlockType, BlockHandler> blockHandlers = new Dictionary<BlockType, BlockHandler> {
               { BlockType.PISTON, new PistonBlockHandler() },
               { BlockType.LIGHT, new BlockHandler<IMyLightingBlock>() },
               { BlockType.MERGE, new BlockHandler<IMyShipMergeBlock>() },
               { BlockType.PROJECTOR, new BlockHandler<IMyProjector>() },
               { BlockType.TIMER, new BlockHandler<IMyTimerBlock>() },
               { BlockType.CONNECTOR, new ConnectorBlockHandler() },
               { BlockType.WELDER, new BlockHandler<IMyShipWelder>() },
               { BlockType.GRINDER, new BlockHandler<IMyShipGrinder>() },
               { BlockType.ROTOR, new RotorBlockHandler() },
            };

            public static BlockHandler GetBlockHandler(BlockType blockType)
            {
                if (blockHandlers.ContainsKey(blockType))
                {
                    return blockHandlers[blockType];
                }
                else
                {
                    throw new Exception("Unsupported Block Type");
                }
            }

            public static List<IMyFunctionalBlock> getBlocks(MyGridProgram program, BlockType blockType, String customName)
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

                switch (blockType)
                {
                    case BlockType.PISTON: program.GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.ROTOR: program.GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.LIGHT: program.GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.MERGE: program.GridTerminalSystem.GetBlocksOfType<IMyShipMergeBlock>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.PROJECTOR: program.GridTerminalSystem.GetBlocksOfType<IMyProjector>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.TIMER: program.GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.CONNECTOR: program.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.WELDER: program.GridTerminalSystem.GetBlocksOfType<IMyShipWelder>(blocks, block => block.CustomName.ToLower() == customName); break;
                    case BlockType.GRINDER: program.GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(blocks, block => block.CustomName.ToLower() == customName); break;
                    default: throw new Exception("Unsupported Block Type: " + blockType);
                }
                return blocks.Select(block => (IMyFunctionalBlock)block).ToList();
            }

            public static List<IMyFunctionalBlock> getBlocks(IMyBlockGroup group, BlockType blockType)
            {
                List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

                switch (blockType)
                {
                    case BlockType.PISTON: group.GetBlocksOfType<IMyPistonBase>(blocks); break;
                    case BlockType.ROTOR: group.GetBlocksOfType<IMyMotorStator>(blocks); break;
                    case BlockType.LIGHT: group.GetBlocksOfType<IMyInteriorLight>(blocks); break;
                    case BlockType.MERGE: group.GetBlocksOfType<IMyShipMergeBlock>(blocks); break;
                    case BlockType.PROJECTOR: group.GetBlocksOfType<IMyProjector>(blocks); break;
                    case BlockType.TIMER: group.GetBlocksOfType<IMyTimerBlock>(blocks); break;
                    case BlockType.CONNECTOR: group.GetBlocksOfType<IMyShipConnector>(blocks); break;
                    case BlockType.WELDER: group.GetBlocksOfType<IMyShipWelder>(blocks); break;
                    case BlockType.GRINDER: group.GetBlocksOfType<IMyShipGrinder>(blocks); break;
                    default: throw new Exception("Unsupported Block Type: " + blockType);
                }
                return blocks.Select(block => (IMyFunctionalBlock)block).ToList();
            }
        }

        //Property Getters
        public interface PropertyGetter<T, U>{T GetPropertyValue(IMyFunctionalBlock block);U GetHandledPropertyType();}
        public interface BooleanPropertyGetter : PropertyGetter<bool, BooleanPropertyType>{}
        public interface StringPropertyGetter : PropertyGetter<String, StringPropertyType>{}
        public interface NumericPropertyGetter : PropertyGetter<float, NumericPropertyType>{}

        public abstract class BooleanPropertyGetter<T> : BooleanPropertyGetter where T : class, IMyFunctionalBlock
        {
            private BooleanPropertyType handledType;
            public BooleanPropertyType GetHandledPropertyType() { return handledType; }
            public abstract bool GetPropertyValue(T block);
            public bool GetPropertyValue(IMyFunctionalBlock block) { return GetPropertyValue((T)block); }
            public BooleanPropertyGetter(BooleanPropertyType handledType) { this.handledType = handledType;}
        }

        public abstract class StringPropertyGetter<T> : StringPropertyGetter where T : class, IMyFunctionalBlock
        {
            private StringPropertyType handledType;
            public  StringPropertyType GetHandledPropertyType() { return handledType; }
            public abstract String GetPropertyValue(T block);
            public String GetPropertyValue(IMyFunctionalBlock block) { return GetPropertyValue((T)block);}
            public StringPropertyGetter(StringPropertyType handledType) { this.handledType = handledType; }
        }

        public abstract class NumericPropertyGetter<T> : NumericPropertyGetter where T : class, IMyFunctionalBlock
        {
            private NumericPropertyType handledType;
            public  NumericPropertyType GetHandledPropertyType() { return handledType; }
            public abstract float GetPropertyValue(T block);
            public float GetPropertyValue(IMyFunctionalBlock block) { return GetPropertyValue((T)block); }
            public NumericPropertyGetter(NumericPropertyType handledType) { this.handledType = handledType; }
        }

        //Property Setters
        public interface PropertySetter<T, U>{void SetPropertyValue(IMyFunctionalBlock block, T value);U GetHandledPropertyType();}
        public interface BooleanPropertySetter : PropertySetter<bool, BooleanPropertyType> { }
        public interface StringPropertySetter : PropertySetter<String, StringPropertyType> { }
        public interface NumericPropertySetter : PropertySetter<float, NumericPropertyType> {
            void SetPropertyValue(IMyFunctionalBlock block, DirectionType DirectionType, float value);
            void IncrementPropertyValue(IMyFunctionalBlock block, float deltaValue);
            void IncrementPropertyValue(IMyFunctionalBlock block, DirectionType direction, float deltaValue);
            void MovePropertyValue(IMyFunctionalBlock block, DirectionType direction);
            void ReversePropertyValue(IMyFunctionalBlock block);
        }

        public abstract class BooleanPropertySetter<T> : BooleanPropertySetter where T : class, IMyFunctionalBlock
        {
            private BooleanPropertyType handledType;
            public BooleanPropertyType GetHandledPropertyType() { return handledType; }
            public abstract void SetPropertyValue(T block, bool value);
            public void SetPropertyValue(IMyFunctionalBlock block, bool value) { SetPropertyValue((T)block, value); }
            public BooleanPropertySetter(BooleanPropertyType handledType) { this.handledType = handledType; }
        }

        public abstract class StringPropertySetter<T> : StringPropertySetter where T : class, IMyFunctionalBlock
        {
            private StringPropertyType handledType;
            public StringPropertyType GetHandledPropertyType() { return handledType; }
            public abstract void SetPropertyValue(T block, String value);
            public void SetPropertyValue(IMyFunctionalBlock block, String value) { SetPropertyValue((T)block, value); }
            public StringPropertySetter(StringPropertyType handledType) { this.handledType = handledType; }
        }

        public abstract class NumericPropertySetter<T> : NumericPropertySetter where T : class, IMyFunctionalBlock
        {

            private NumericPropertyType handledType;
            public NumericPropertyType GetHandledPropertyType() { return handledType; }
            public abstract void SetPropertyValue(T block, DirectionType DirectionType, float value);
            public abstract void IncrementPropertyValue(T block, float deltaValue);
            public abstract void IncrementPropertyValue(T block, DirectionType direction, float deltaValue);
            public abstract void MovePropertyValue(T block, DirectionType direction);
            public abstract void ReversePropertyValue(T block);
            public abstract void SetPropertyValue(T block, float value);
            public void SetPropertyValue(IMyFunctionalBlock block, float value) { SetPropertyValue((T)block, value); }
            public void SetPropertyValue(IMyFunctionalBlock block, DirectionType direction, float value) { SetPropertyValue((T)block, direction, value);}
            public void IncrementPropertyValue(IMyFunctionalBlock block, float deltaValue){ IncrementPropertyValue((T)block, deltaValue); }
            public void IncrementPropertyValue(IMyFunctionalBlock block, DirectionType direction, float deltaValue){ IncrementPropertyValue((T)block, direction, deltaValue);}
            public void MovePropertyValue(IMyFunctionalBlock block, DirectionType direction){MovePropertyValue((T)block, direction);}
            public void ReversePropertyValue(IMyFunctionalBlock block){ReversePropertyValue((T)block);}
            public NumericPropertySetter(NumericPropertyType handledType) { this.handledType = handledType; }
        }

        public abstract class BlockHandler {
            protected Dictionary<BooleanPropertyType, BooleanPropertyGetter> booleanPropertyGetters = new Dictionary<BooleanPropertyType, BooleanPropertyGetter>();
            protected Dictionary<StringPropertyType, StringPropertyGetter> stringPropertyGetters = new Dictionary<StringPropertyType, StringPropertyGetter>();
            protected Dictionary<NumericPropertyType, NumericPropertyGetter> numericPropertyGetters = new Dictionary<NumericPropertyType, NumericPropertyGetter>();

            protected Dictionary<BooleanPropertyType, BooleanPropertySetter> booleanPropertySetters = new Dictionary<BooleanPropertyType, BooleanPropertySetter>();
            protected Dictionary<StringPropertyType, StringPropertySetter> stringPropertySetters = new Dictionary<StringPropertyType, StringPropertySetter>();
            protected Dictionary<NumericPropertyType, NumericPropertySetter> numericPropertySetters = new Dictionary<NumericPropertyType, NumericPropertySetter>();

            public virtual BooleanPropertyType GetDefaultBooleanProperty()
            {
                return BooleanPropertyType.ON_OFF;
            }

            public virtual StringPropertyType GetDefaultStringProperty()
            {
                return StringPropertyType.NAME;
            }

            public virtual NumericPropertyType GetDefaultNumericProperty(DirectionType direction)
            {
                throw new Exception("This Block Does Not Have A Default Numeric Property");
            }

            public virtual DirectionType GetDefaultDirection()
            {
                throw new Exception("This Block Does Not Have a Default Direction");
            }

            public bool GetBooleanPropertyValue(IMyFunctionalBlock block, BooleanPropertyType property)
            {
                return booleanPropertyGetters[property].GetPropertyValue(block);
            }
            public string GetStringPropertyValue(IMyFunctionalBlock block, StringPropertyType property)
            {
                return stringPropertyGetters[property].GetPropertyValue(block);
            }
            public float GetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property)
            {
                return numericPropertyGetters[property].GetPropertyValue(block);
            }

            public void SetBooleanPropertyValue(IMyFunctionalBlock block, BooleanPropertyType property, bool value)
            {
                booleanPropertySetters[property].SetPropertyValue(block, value);
            }

            public void SetStringPropertyValue(IMyFunctionalBlock block, StringPropertyType property, String value)
            {
                stringPropertySetters[property].SetPropertyValue(block, value);
            }

            public void SetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, float value)
            {
                numericPropertySetters[property].SetPropertyValue(block, value);
            }

            public void SetNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction, float value)
            {
                numericPropertySetters[property].SetPropertyValue(block, direction, value);
            }

            public void IncrementNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, float deltaValue)
            {
                numericPropertySetters[property].IncrementPropertyValue(block, deltaValue);
            }

            public void IncrementNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction, float deltaValue)
            {
                numericPropertySetters[property].IncrementPropertyValue(block, direction, deltaValue);
            }

            public void MoveNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property, DirectionType direction)
            {
                numericPropertySetters[property].MovePropertyValue(block, direction);
            }

            public void ReverseNumericPropertyValue(IMyFunctionalBlock block, NumericPropertyType property)
            {
                numericPropertySetters[property].ReversePropertyValue(block);
            }
        }

        public class BlockHandler<T> : BlockHandler where T : class, IMyFunctionalBlock
        {

            public BlockHandler()
            {
                GetBooleanPropertyGetters().ForEach(handler => booleanPropertyGetters.Add(handler.GetHandledPropertyType(), handler));
                GetStringPropertyGetters().ForEach(handler => stringPropertyGetters.Add(handler.GetHandledPropertyType(), handler));
                GetNumericPropertyGetters().ForEach(handler => numericPropertyGetters.Add(handler.GetHandledPropertyType(), handler));

                GetBooleanPropertySetters().ForEach(handler => booleanPropertySetters.Add(handler.GetHandledPropertyType(), handler));
                GetStringPropertySetters().ForEach(handler => stringPropertySetters.Add(handler.GetHandledPropertyType(), handler));
                GetNumericPropertySetters().ForEach(handler => numericPropertySetters.Add(handler.GetHandledPropertyType(), handler));
            }

            protected virtual List<BooleanPropertyGetter<T>> GetBooleanPropertyGetters()
            {
                return new List<BooleanPropertyGetter<T>>() {
                        new OnOffPropertyGetter<T>()
                };
            }

            protected virtual List<StringPropertyGetter<T>> GetStringPropertyGetters()
            {
                return new List<StringPropertyGetter<T>>() { };
            }

            protected virtual List<NumericPropertyGetter<T>> GetNumericPropertyGetters()
            {
                return new List<NumericPropertyGetter<T>>() { };
            }

            protected virtual List<BooleanPropertySetter<T>> GetBooleanPropertySetters()
            {
                return new List<BooleanPropertySetter<T>>() {
                        new OnOffPropertySetter<T>()
                };
            }

            protected virtual List<StringPropertySetter<T>> GetStringPropertySetters()
            {
                return new List<StringPropertySetter<T>>() { };
            }

            protected virtual List<NumericPropertySetter<T>> GetNumericPropertySetters()
            {
                return new List<NumericPropertySetter<T>>() { };
            }
        }

        public class OnOffPropertyGetter<T> : BooleanPropertyGetter<T> where T : class, IMyFunctionalBlock
        {
            public OnOffPropertyGetter() : base(BooleanPropertyType.ON_OFF){}

            public override bool GetPropertyValue(T block){return block.Enabled;}
        }

        public class OnOffPropertySetter<T> : BooleanPropertySetter<T> where T : class, IMyFunctionalBlock
        {
            public OnOffPropertySetter() : base(BooleanPropertyType.ON_OFF){}

            public override void SetPropertyValue(T block, bool enabled)
            {
                block.Enabled = enabled;
            }
        }
    }
}
