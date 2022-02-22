﻿using Sandbox.Game.EntityComponents;
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
        public interface ISelector {
            List<Object> GetEntities();
            Block GetBlockType();
        }

        public class ConditionalSelector : ISelector {
            public ISelector selector;
            public BlockCondition condition;

            public ConditionalSelector(ISelector sel, BlockCondition cond) {
                selector = sel;
                condition = cond;
            }

            public Block GetBlockType() => selector.GetBlockType();

            public List<object> GetEntities() => selector.GetEntities().Where(b => condition(b, selector.GetBlockType())).ToList();
        }

        public class IndexSelector : ISelector {
            public ISelector selector;
            public IVariable index;

            public IndexSelector(ISelector sel, IVariable ind) {
                selector = sel;
                index = ind;
            }

            public Block GetBlockType() => selector.GetBlockType();

            public List<Object> GetEntities() {
                var entities = selector.GetEntities();
                IBlockHandler b = BlockHandlerRegistry.GetBlockHandler(GetBlockType());

                return CastList(index.GetValue()).keyedValues
                    .Select(v => v.GetValue())
                    .SelectMany(p => {
                        if (p.returnType == Return.NUMERIC)
                            return entities.GetRange((int)CastNumber(p), 1);
                        else if (p.returnType == Return.STRING) {
                            var s = CastString(p);
                            return entities.Where(o => s == b.GetName(o));
                        } else
                            return Empty<Object>();
                    }).ToList();
            }
        }

        public class BlockSelector : ISelector {
            public Block? blockType;
            public bool isGroup;
            public IVariable selector;

            public BlockSelector(Block? type, bool group, IVariable sel) {
                blockType = type;
                isGroup = group;
                selector = sel;
            }

            public List<Object> GetEntities() {
                var selectorString = CastString(selector.GetValue());
                var resolvedIsGroup = false;
                Block bt = blockType ?? ResolveType(selectorString, out resolvedIsGroup);
                return isGroup || resolvedIsGroup ? BlockHandlerRegistry.GetBlocksInGroup(bt, selectorString) : BlockHandlerRegistry.GetBlocks(bt, selectorString);
            }

            public Block GetBlockType() {
                bool ignored;
                return blockType ?? ResolveType(CastString(selector.GetValue()), out ignored);
            }

            Block ResolveType(String selector, out bool isGroup) {
                var parameters = PROGRAM.ParseCommandParameters(PROGRAM.Tokenize(selector));
                var blockType = findLast<BlockTypeCommandParameter>(parameters);
                isGroup = findLast<GroupCommandParameter>(parameters) != null;
                if (blockType == null) throw new Exception("Cannot parse block type from selector: " + selector);
                return blockType.value;
            }
        }

        public class SelfSelector : ISelector {
            public Block? blockType;

            public SelfSelector(Block? type) {
                blockType = type;
            }

            public Block GetBlockType() => blockType ?? Block.PROGRAM;
            public List<object> GetEntities() => BlockHandlerRegistry.GetSelf(blockType) ?? BlockHandlerRegistry.GetBlocks(GetBlockType());
        }

        public class BlockTypeSelector : ISelector {
            public Block blockType;

            public BlockTypeSelector(Block type) {
                blockType = type;
            }

            public Block GetBlockType() => blockType;
            public List<object> GetEntities() => BlockHandlerRegistry.GetBlocks(blockType);
        }
    }
}
