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
        public interface EntityProvider {
            List<Object> GetEntities();
            Block GetBlockType();
        }

        public class ConditionalEntityProvider : EntityProvider {
            public EntityProvider provider;
            public BlockCondition condition;

            public ConditionalEntityProvider(EntityProvider prov, BlockCondition cond) {
                provider = prov;
                condition = cond;
            }

            public Block GetBlockType() => provider.GetBlockType();

            public List<object> GetEntities() => provider.GetEntities().Where(b => condition(b, provider.GetBlockType())).ToList();
        }

        public class IndexEntityProvider : EntityProvider {
            public EntityProvider provider;
            public Variable index;

            public IndexEntityProvider(EntityProvider prov, Variable ind) {
                provider = prov;
                index = ind;
            }

            public Block GetBlockType() => provider.GetBlockType();

            public List<object> GetEntities() {
                var entities = provider.GetEntities();
                var selectedEntities = NewList<Object>();
                BlockHandler b = BlockHandlerRegistry.GetBlockHandler(GetBlockType());

                var indexes = CastList(index.GetValue()).GetValues()
                    .Select(v => v.GetValue()).ToList();

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

        public class SelectorEntityProvider : EntityProvider {
            public Block? blockType;
            public bool isGroup;
            public Variable selector;

            public SelectorEntityProvider(Block? type, bool group, Variable sel) {
                blockType = type;
                isGroup = group;
                selector = sel;
            }

            public List<Object> GetEntities() {
                String selectorString = CastString(selector.GetValue());
                bool resolvedIsGroup = false;
                Block bt = blockType.HasValue ? blockType.Value : ResolveType(selectorString, out resolvedIsGroup);
                bool useGroup = isGroup || resolvedIsGroup;
                List<object> entities = useGroup ? BlockHandlerRegistry.GetBlocksInGroup(bt, selectorString) : BlockHandlerRegistry.GetBlocks(bt, block => block.CustomName.Equals(selectorString));
                return entities;
            }

            public Block GetBlockType() {
                if (blockType.HasValue) return blockType.Value;
                bool ignored;
                return ResolveType(CastString(selector.GetValue()), out ignored);
            }

            Block ResolveType(String selector, out bool isGroup) {
                var tokens = PROGRAM.ParseTokens(selector);
                var parameters = PROGRAM.ParseCommandParameters(tokens);
                var blockType = findLast<BlockTypeCommandParameter>(parameters);
                isGroup = findLast<GroupCommandParameter>(parameters) != null;
                if (blockType == null) throw new Exception("Cannot parse block type from selector: " + selector);
                return blockType.value;
            }

            public override String ToString() => blockType + (isGroup ? " in group named " : " named " + selector);
        }

        public class SelfEntityProvider : EntityProvider {
            public Block blockType;

            public SelfEntityProvider(Block type) {
                blockType = type;
            }

            public Block GetBlockType() => blockType;

            public List<object> GetEntities() => BlockHandlerRegistry.GetBlocks(blockType, (b) => b.EntityId.Equals(PROGRAM.Me.EntityId));
        }

        public class AllEntityProvider : EntityProvider {
            public Block blockType;

            public AllEntityProvider(Block type) {
                blockType = type;
            }

            public Block GetBlockType() => blockType;

            public List<object> GetEntities() => BlockHandlerRegistry.GetBlocks(blockType, block => true);
        }
    }
}
