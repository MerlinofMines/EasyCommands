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
        public class ConnectorLockHandler : OneParameterEntityCommandHandler<LockCommandParameter, IMyShipConnector>
        {
            public ConnectorLockHandler(IEntityProvider<IMyShipConnector> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                if(GetParameter().IsLock())
                {
                    entityProvider.GetEntities(program).ForEach(param => param.Connect());
                } else
                {
                    entityProvider.GetEntities(program).ForEach(param => param.Disconnect());
                }
                return true;
            }
        }

        public class ConnectorConnectHandler : OneParameterEntityCommandHandler<ConnectCommandParameter, IMyShipConnector>
        {
            public ConnectorConnectHandler(IEntityProvider<IMyShipConnector> entityProvider) : base(entityProvider)
            {
            }

            public override bool Handle(MyGridProgram program)
            {
                if (GetParameter().IsConnect())
                {
                    entityProvider.GetEntities(program).ForEach(param => param.Connect());
                }
                else
                {
                    entityProvider.GetEntities(program).ForEach(param => param.Disconnect());
                }
                return true;
            }
        }
    }
}
