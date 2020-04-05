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
        public class ConnectorBlockHandler : BlockHandler<IMyShipConnector>
        {
            protected override List<BooleanPropertyGetter<IMyShipConnector>> GetBooleanPropertyGetters()
            {
                return new List<BooleanPropertyGetter<IMyShipConnector>>()
                {
                    new OnOffPropertyGetter<IMyShipConnector>(),
                    new ConnectorConnectedGetter(),
                    new ConnectorLockedGetter(),
                };
            }

            protected override List<BooleanPropertySetter<IMyShipConnector>> GetBooleanPropertySetters()
            {
                return new List<BooleanPropertySetter<IMyShipConnector>>()
                {
                    new OnOffPropertySetter<IMyShipConnector>(),
                    new ConnectorConnectedSetter(),
                    new ConnectorLockedSetter(),
                };
    }
}

        public class ConnectorConnectedGetter : BooleanPropertyGetter<IMyShipConnector>
        {
            public ConnectorConnectedGetter() : base(BooleanPropertyType.CONNECTED){}

            public override bool GetPropertyValue(IMyShipConnector block)
            {
                return block.Status == MyShipConnectorStatus.Connected;
            }
        }

        public class ConnectorConnectedSetter : BooleanPropertySetter<IMyShipConnector>
        {
            public ConnectorConnectedSetter() : base(BooleanPropertyType.CONNECTED){}

            public override void SetPropertyValue(IMyShipConnector block, bool value)
            {
                if (value) { block.Connect(); } else { block.Disconnect(); }
            }
        }

        public class ConnectorLockedGetter : BooleanPropertyGetter<IMyShipConnector>
        {
            public ConnectorLockedGetter() : base(BooleanPropertyType.LOCKED){}

            public override bool GetPropertyValue(IMyShipConnector block)
            {
                return block.Status == MyShipConnectorStatus.Connected;
            }
        }

        public class ConnectorLockedSetter : BooleanPropertySetter<IMyShipConnector>
        {
            public ConnectorLockedSetter() : base(BooleanPropertyType.LOCKED) { }

            public override void SetPropertyValue(IMyShipConnector block, bool value)
            {
                if (value) { block.Connect(); } else { block.Disconnect(); }
            }
        }

        //TODO: Add Connectable Handler
    }
}
