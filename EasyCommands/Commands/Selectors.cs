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

        public class SelectorType {
            public Block? blockType;
            public bool? isGroup;
        }

        public class BlockSelector : ISelector {
            public SelectorType selectorType;
            public IVariable selector;

            public BlockSelector(SelectorType SelectorType, IVariable Selector) {
                selectorType = SelectorType;
                selector = Selector;
            }

            public List<Object> GetEntities() {
                var selectorString = CastString(selector.GetValue());
                var resolvedSelector = ResolveTypeIfMissing();
                var bt = resolvedSelector.blockType.Value;
                var entities = NewList<Object>();
                if (!(resolvedSelector.isGroup ?? false))
                    entities = BlockHandlerRegistry.GetBlocks(bt, selectorString);

                if (resolvedSelector.isGroup ?? entities.Count == 0)
                    entities = BlockHandlerRegistry.GetBlocksInGroup(bt, selectorString);

                return entities;
            }

            public Block GetBlockType() => ResolveTypeIfMissing().blockType.Value;

            SelectorType ResolveTypeIfMissing() {
                if (selectorType.blockType != null) return selectorType;
                var selectorString = CastString(selector.GetValue());
                var parameters = PROGRAM.ParseCommandParameters(PROGRAM.Tokenize(selectorString));
                var blockType = findLast<SelectorTypeCommandParameter>(parameters);
                if (blockType == null) throw new RuntimeException("Cannot parse block type from selector: " + selectorString);
                return ResolveSelectorType(blockType.value, findLast<GroupCommandParameter>(parameters));
            }
        }

        public static SelectorType ResolveSelectorType(SelectorType selector, GroupCommandParameter isGroup) =>
            new SelectorType { blockType = selector?.blockType, isGroup = isGroup != null ? true : selector?.isGroup };

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
