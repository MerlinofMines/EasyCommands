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
            public override BooleanPropertyType GetDefaultBooleanProperty()
            {
                return BooleanPropertyType.ON_OFF;//TODO: Move to base class
            }

            public override StringPropertyType GetDefaultStringProperty()
            {
                return StringPropertyType.NAME;//TODO: Move to base class
            }

            protected override List<BooleanPropertyHandler<IMyShipConnector>> GetBooleanPropertyHandlers()
            {
                return new List<BooleanPropertyHandler<IMyShipConnector>>()
                {
                    new OnOffPropertyHandler<IMyShipConnector>(),
                    new ConnectorConnectedHandler()
                };
            }

            protected override List<StringPropertyHandler<IMyShipConnector>> GetStringPropertyHandlers()
            {
                return new List<StringPropertyHandler<IMyShipConnector>>()
                {
                     //TODO: Add some?  Name?  Make Virtual and only override if necessary? Put in base class.
                };
            }
        }

        public class ConnectorConnectedHandler : BooleanPropertyHandler<IMyShipConnector>
        {
            public BooleanPropertyType GetHandledPropertyType()
            {
                return BooleanPropertyType.CONNECTED;
            }

            public bool GetPropertyValue(IMyShipConnector block)
            {
                return block.Status == MyShipConnectorStatus.Connected;
            }

            public void SetPropertyValue(IMyShipConnector block, bool value)
            {
                if (value) { block.Connect(); } else { block.Disconnect(); }
            }
        }

        public class ConnectorLockedHandler : BooleanPropertyHandler<IMyShipConnector>
        {
            public BooleanPropertyType GetHandledPropertyType()
            {
                return BooleanPropertyType.LOCKED;
            }

            public bool GetPropertyValue(IMyShipConnector block)
            {
                return block.Status == MyShipConnectorStatus.Connected;
            }

            public void SetPropertyValue(IMyShipConnector block, bool value)
            {
                if (value) { block.Connect(); } else { block.Disconnect(); }
            }
        }

        //TODO: Add Connectable Handler
    }
}
