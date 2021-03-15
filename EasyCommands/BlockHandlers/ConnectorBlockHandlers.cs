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
        public class ConnectorBlockHandler : FunctionalBlockHandler<IMyShipConnector> {
            public ConnectorBlockHandler() : base() {
                AddBooleanHandler(PropertyType.LOCKED, Connected, Connect);
                AddBooleanHandler(PropertyType.CONNECTED, Connected, Connect);
                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.CONNECTED;
                //TODO: Add Strength
            }

            static bool Connected(IMyShipConnector connector) {
                return connector.Status == MyShipConnectorStatus.Connected;
            }

            static void Connect(IMyShipConnector connector, bool value) {
                if (value) { connector.Connect(); } else { connector.Disconnect(); }
            }
        }
    }
}
