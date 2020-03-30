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
        public class ActivationHandler<U> : OneParameterEntityCommandHandler<ActivationCommandParameter, U> where U : class, IMyTerminalBlock
        {
            public ActivationHandler(IEntityProvider<U> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                if (GetParameter().IsActivate())
                {
                    entityProvider.GetEntities(program).ForEach(block => block.ApplyAction("OnOff_On"));
                }
                else
                {
                    entityProvider.GetEntities(program).ForEach(block => block.ApplyAction("OnOff_Off"));
                }
                return true;
            }
        }

        public class ReverseHandler<U> : OneParameterEntityCommandHandler<ReverseCommandParameter, U> where U : class, IMyTerminalBlock
        {
            public ReverseHandler(IEntityProvider<U> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                entityProvider.GetEntities(program).ForEach(entity => entity.ApplyAction("Reverse"));
                return true;
            }
        }
    }
}
