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
            BlockType GetBlockType();
        }

        public class ConditionalEntityProvider : EntityProvider {
            public EntityProvider provider;
            public BlockCondition condition;

            public ConditionalEntityProvider(EntityProvider provider, BlockCondition condition) {
                this.provider = provider;
                this.condition = condition;
            }

            public BlockType GetBlockType() {
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

            public BlockType GetBlockType() {
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
            public BlockType blockType;
            public bool isGroup;
            public Variable selector;

            public SelectorEntityProvider(BlockType blockType, bool isGroup, Variable selector) {
                this.blockType = blockType;
                this.isGroup = isGroup;
                this.selector = selector;
            }

            public List<Object> GetEntities() {
                String selectorString = CastString(selector.GetValue()).GetStringValue();
                List<object> entities = isGroup ? BlockHandlerRegistry.GROUP_BLOCK_PROVIDER(blockType, selectorString) : BlockHandlerRegistry.BLOCK_PROVIDER(blockType, selectorString);
                return entities;
            }

            public BlockType GetBlockType() {
                return blockType;
            }

            public override String ToString() {
                return blockType + (isGroup ? " in group named " : " named " + selector);
            }
        }
    }
}
