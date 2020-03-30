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

        public interface CommandParameter
        {

        }

        public class GroupCommandParameter : CommandParameter
        {

        }

        public class BlockTypeCommandParameter : CommandParameter
        {
            private BlockType blockType;

            public BlockTypeCommandParameter(BlockType blockType)
            {
                this.blockType = blockType;
            }

            public BlockType GetBlockType()
            {
                return blockType;
            }
        }

        public class SelectorCommandParameter : CommandParameter
        {
            private String selector; 

            public SelectorCommandParameter(String selector) {
                this.selector = selector;
            }

            public String GetSelector()
            {
                return selector;
            }
        }

        public class ActivationCommandParameter : CommandParameter
        {
            private bool activate;

            public ActivationCommandParameter(bool activate)
            {
                this.activate = activate;
            }

            public bool IsActivate()
            {
                return activate;
            }
        }

        public class ReverseCommandParameter : CommandParameter
        {

        }

        public class IncrementCommandParameter : CommandParameter
        {
            bool increment;//false if decrement

            public IncrementCommandParameter(bool increment)
            {
                this.increment = increment;
            }

            public bool IsIncrement()
            {
                return increment;
            }
        }

        public class LockCommandParameter : CommandParameter
        {
            private bool locked;

            public LockCommandParameter(bool locked)
            {
                this.locked = locked;
            }

            public bool IsLock()
            {
                return locked;
            }
        }

        public class ConnectCommandParameter : CommandParameter
        {
            private bool connect;

            public ConnectCommandParameter(bool connect)
            {
                this.connect = connect;
            }

            public bool IsConnect()
            {
                return connect;
            }
        }

        public class RelativeCommandParameter : CommandParameter
        {

        }

        public class VelocityCommandParameter : CommandParameter
        {

        }

        public class DelayCommandParameter : CommandParameter
        {

        }

        public class WaitCommandParameter : CommandParameter
        {

        }

        public class UnitCommandParameter : CommandParameter
        {
            UnitType unit;

            public UnitCommandParameter(UnitType unit)
            {
                this.unit = unit;
            }

            public UnitType GetUnit()
            {
                return unit;
            }
        }

        public class NumericCommandParameter : CommandParameter
        {
            float value;

            public NumericCommandParameter(float value)
            {
                this.value = value;
            }

            public float GetValue()
            {
                return value;
            }
        }
    }
}
