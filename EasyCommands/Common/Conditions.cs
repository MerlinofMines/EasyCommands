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

namespace IngameScript {
    partial class Program {
        public delegate bool BlockCondition(Object block, Block blockType);
        public delegate bool PrimitiveComparator(Primitive a, Primitive b);
        public BlockCondition AndCondition(BlockCondition a, BlockCondition b) => new BlockCondition((block, blockType) => a(block, blockType) && b(block, blockType));
        public BlockCondition OrCondition(BlockCondition a, BlockCondition b) => new BlockCondition((block, blockType) => a(block, blockType) || b(block, blockType));

        public static BlockCondition BlockPropertyCondition(PropertySupplier property, PrimitiveComparator comparator, IVariable comparisonValue) => (block, blockType) => {
            Primitive value = comparisonValue.GetValue();
            IBlockHandler handler = BlockHandlerRegistry.GetBlockHandler(blockType);
            return comparator(handler.GetPropertyValue(block, property.WithPropertyValue(comparisonValue).Resolve(handler, value.returnType)), value);
        };
    }
}
