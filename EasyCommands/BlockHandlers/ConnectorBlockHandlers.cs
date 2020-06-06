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
        public class ConnectorBlockHandler : BlockHandler<IMyShipConnector> {
            public ConnectorBlockHandler() : base() {
                booleanPropertyGetters.Add(BooleanPropertyType.LOCKED, Connected);
                booleanPropertyGetters.Add(BooleanPropertyType.CONNECTED, Connected);

                booleanPropertySetters.Add(BooleanPropertyType.LOCKED, Connect);
                booleanPropertySetters.Add(BooleanPropertyType.CONNECTED, Connect);
            }

            static bool Connected(IMyShipConnector connector) {
                return connector.Status == MyShipConnectorStatus.Connected;
            }

            static void Connect(IMyShipConnector connector, bool value) {
                if (value) { connector.Connect(); } else { connector.Disconnect(); }
            }
            //TODO: Add Connectable Handler
        }
    }
}
