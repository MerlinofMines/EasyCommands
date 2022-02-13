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
        public interface Selector {
            List<Object> GetEntities();
            Block GetBlockType();
        }

        public class ConditionalSelector : Selector {
            public Selector selector;
            public BlockCondition condition;

            public ConditionalSelector(Selector sel, BlockCondition cond) {
                selector = sel;
                condition = cond;
            }

            public Block GetBlockType() => selector.GetBlockType();

            public List<object> GetEntities() => selector.GetEntities().Where(b => condition(b, selector.GetBlockType())).ToList();
        }

        public class IndexSelector : Selector {
            public Selector selector;
            public Variable index;

            public IndexSelector(Selector sel, Variable ind) {
                selector = sel;
                index = ind;
            }

            public Block GetBlockType() => selector.GetBlockType();

            public List<object> GetEntities() {
                var entities = selector.GetEntities();
                var selectedEntities = NewList<Object>();
                BlockHandler b = BlockHandlerRegistry.GetBlockHandler(GetBlockType());

                var indexes = CastList(index.GetValue()).GetValues()
                    .Select(v => v.GetValue());

                foreach (Primitive p in indexes) {
                    //Return empty list if index > Count
                    if (p.returnType == Return.NUMERIC) {
                        int i = (int)CastNumber(p);
                        if (i < entities.Count) selectedEntities.Add(entities[i]);
                    }
                    if (p.returnType == Return.STRING) {
                        var entityName = CastString(p);
                        selectedEntities.AddRange(entities.Where(o => entityName == b.GetName(o)));
                    }
                    //Other Index types not supported
                }
                return selectedEntities;
            }
        }

        public class BlockSelector : Selector {
            public Block? blockType;
            public bool isGroup;
            public Variable selector;

            public BlockSelector(Block? type, bool group, Variable sel) {
                blockType = type;
                isGroup = group;
                selector = sel;
            }

            public List<Object> GetEntities() {
                String selectorString = CastString(selector.GetValue());
                bool resolvedIsGroup = false;
                Block bt = blockType ?? ResolveType(selectorString, out resolvedIsGroup);
                bool useGroup = isGroup || resolvedIsGroup;
                var entities = useGroup ? BlockHandlerRegistry.GetBlocksInGroup(bt, selectorString) : BlockHandlerRegistry.GetBlocks(bt, b => b.CustomName.Equals(selectorString));
                return entities;
            }

            public Block GetBlockType() {
                bool ignored;
                return blockType ?? ResolveType(CastString(selector.GetValue()), out ignored);
            }

            Block ResolveType(String selector, out bool isGroup) {
                var tokens = PROGRAM.Tokenize(selector);
                var parameters = PROGRAM.ParseCommandParameters(tokens);
                var blockType = findLast<BlockTypeCommandParameter>(parameters);
                isGroup = findLast<GroupCommandParameter>(parameters) != null;
                if (blockType == null) throw new Exception("Cannot parse block type from selector: " + selector);
                return blockType.value;
            }
        }

        public class SelfSelector : Selector {
            public Block? blockType;

            public SelfSelector(Block? type) {
                blockType = type;
            }

            public Block GetBlockType() => blockType.GetValueOrDefault(Block.PROGRAM);

            public List<object> GetEntities() => BlockHandlerRegistry.GetBlocks(GetBlockType(), (b) => (blockType.HasValue && blockType.Value != Block.DISPLAY) || b.EntityId.Equals(PROGRAM.Me.EntityId));
        }

        public class BlockTypeSelector : Selector {
            public Block blockType;

            public BlockTypeSelector(Block type) {
                blockType = type;
            }

            public Block GetBlockType() => blockType;

            public List<object> GetEntities() => BlockHandlerRegistry.GetBlocks(blockType);
        }
    }
}
