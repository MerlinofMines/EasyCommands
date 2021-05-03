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

            public ConditionalEntityProvider(EntityProvider provider, BlockCondition condition) {
                this.provider = provider;
                this.condition = condition;
            }

            public Block GetBlockType() {
                return provider.GetBlockType();
            }

            public List<object> GetEntities() {
                return provider.GetEntities().Where(b => condition.evaluate(b, provider.GetBlockType())).ToList();
            }
        }

        public class IndexEntityProvider : EntityProvider {
            public EntityProvider provider;
            public Variable index;

            public IndexEntityProvider(EntityProvider provider, Variable index) {
                this.provider = provider;
                this.index = index;
            }

            public Block GetBlockType() {
                return provider.GetBlockType();
            }

            public List<object> GetEntities() {
                List<object> entities = provider.GetEntities();
                List<object> selectedEntities = new List<Object>();

                //Return empty list if index > Count
                int i = (int)CastNumber(index.GetValue()).GetNumericValue();
                if (i < entities.Count) selectedEntities.Add(entities[i]);

                return selectedEntities;
            }
        }

        public class SelectorEntityProvider : EntityProvider {
            public Block? blockType;
            public bool isGroup;
            public Variable selector;

            public SelectorEntityProvider(Block? blockType, bool isGroup, Variable selector) {
                this.blockType = blockType;
                this.isGroup = isGroup;
                this.selector = selector;
            }

            public List<Object> GetEntities() {
                String selectorString = CastString(selector.GetValue()).GetStringValue();
                bool resolvedIsGroup = false;
                Block bt = blockType.HasValue ? blockType.Value : ResolveType(selectorString, out resolvedIsGroup);
                bool useGroup = isGroup || resolvedIsGroup;
                List<object> entities = useGroup ? BlockHandlerRegistry.GetBlocksInGroup(bt, selectorString) : BlockHandlerRegistry.GetBlocks(bt, block => block.CustomName.Equals(selectorString));
                return entities;
            }

            public Block GetBlockType() {
                if (blockType.HasValue) return blockType.Value;
                bool ignored;
                return ResolveType(CastString(selector.GetValue()).GetStringValue(), out ignored);
            }

            public Block ResolveType(String selector, out bool isGroup) {
                var tokens = PROGRAM.ParseTokens(selector);
                var parameters = PROGRAM.ParseCommandParameters(tokens);
                var blockType = extractFirst<BlockTypeCommandParameter>(parameters);
                isGroup = extractFirst<GroupCommandParameter>(parameters) != null;
                if (blockType == null) throw new Exception("Cannot parse block type from selector: " + selector);
                return blockType.value;
            }

            public override String ToString() {
                return blockType + (isGroup ? " in group named " : " named " + selector);
            }
        }

        public class SelfEntityProvider : EntityProvider {
            public Block blockType;

            public SelfEntityProvider(Block blockType) {
                this.blockType = blockType;
            }

            public Block GetBlockType() {
                return blockType;
            }

            public List<object> GetEntities() {
                return BlockHandlerRegistry.GetBlocks(blockType, (b) => b.CustomName.Equals(PROGRAM.Me.CustomName));
            }
        }

        public class AllEntityProvider : EntityProvider {
            public Block blockType;

            public AllEntityProvider(Block blockType) {
                this.blockType = blockType;
            }

            public Block GetBlockType() {
                return blockType;
            }

            public List<object> GetEntities() {
                return BlockHandlerRegistry.GetBlocks(blockType, block => true);
            }
        }
    }
}
