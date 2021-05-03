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
        public class MergeBlockHandler : FunctionalBlockHandler<IMyShipMergeBlock> {
            public MergeBlockHandler() : base() {
                AddBooleanHandler(Property.LOCKED, IsMerged);
                AddBooleanHandler(Property.CONNECTED, IsMerged);
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.CONNECTED;
            }

            //https://forum.keenswh.com/threads/merge-block-lock-state-checking.7378572/
            //Credit goes to JoeTheDestroyer
            bool IsMerged(IMyShipMergeBlock b) {
                if (b.IsConnected) return false;

                //Find direction that block merges to
                Matrix mat;
                b.Orientation.GetMatrix(out mat);
                Vector3I right1 = new Vector3I(mat.Right);

                //Check if there is a block in front of merge face
                IMySlimBlock sb = b.CubeGrid.GetCubeBlock(b.Position + right1);
                if (sb == null) return false;

                //Check if the other block is actually a merge block
                IMyShipMergeBlock mrg2 = sb.FatBlock as IMyShipMergeBlock;
                if (mrg2 == null) return false;

                //Check that other block is correctly oriented
                mrg2.Orientation.GetMatrix(out mat);
                Vector3I right2 = new Vector3I(mat.Right);
                return right2 == -right1;
            }
        }
    }
}
