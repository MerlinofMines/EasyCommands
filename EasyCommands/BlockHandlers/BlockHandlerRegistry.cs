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
            static Dictionary<BlockType, BooleanBlockHandler> booleanBlockHandlers = new Dictionary<BlockType, BooleanBlockHandler>();

            static Dictionary<BlockType, StringBlockHandler> stringBlockHandlers = new Dictionary<BlockType, StringBlockHandler>();

            static Dictionary<BlockType, NumericBlockHandler> numericBlockHandlers = new Dictionary<BlockType, NumericBlockHandler>();

            public static BooleanBlockHandler<T> GetBooleanBlockHandler<T>(BlockType blockType) where T : class, IMyFunctionalBlock
            {
                if (booleanBlockHandlers.ContainsKey(blockType))
                {
                    return (BooleanBlockHandler<T>)booleanBlockHandlers[blockType];
                }
                else
                {
                    return null;
                }
            }

            public static StringBlockHandler<T> GetStringBlockHandler<T>(BlockType blockType) where T : class, IMyFunctionalBlock
            {
                if (stringBlockHandlers.ContainsKey(blockType))
                {
                    return (StringBlockHandler<T>)stringBlockHandlers[blockType];
                }
                else
                {
                    return null;
                }
            }

            public static NumericBlockHandler<T> GetNumericBlockHandler<T>(BlockType blockType) where T : class, IMyFunctionalBlock
            {
                if (numericBlockHandlers.ContainsKey(blockType))
                {
                    return (NumericBlockHandler<T>)numericBlockHandlers[blockType];
                }
                else
                {
                    return null;
                }
            }

            public static void RegisterBlockHandler(BlockType blockType, Object blockHandler)
            {
                if (blockHandler is BooleanBlockHandler)
                {
                    booleanBlockHandlers.Add(blockType, (BooleanBlockHandler) blockHandler);
                }
                if (blockHandler is StringBlockHandler)
                {
                    stringBlockHandlers.Add(blockType, (StringBlockHandler) blockHandler);
                }
                if (blockHandler is NumericBlockHandler)
                {
                    numericBlockHandlers.Add(blockType, (NumericBlockHandler) blockHandler);
                }
            }
        }

    }
}
