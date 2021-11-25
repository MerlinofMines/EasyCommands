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
        public class ConnectorBlockHandler : EjectorBlockHandler {
            public ConnectorBlockHandler() : base() {
                AddBooleanHandler(Property.LOCKED, Connected, Connect);
                AddBooleanHandler(Property.CONNECTED, Connected, Connect);
                AddNumericHandler(Property.STRENGTH, b => b.PullStrength, (b, v) => b.PullStrength = v, 0.01f);
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.STRENGTH;
            }

            static bool Connected(IMyShipConnector connector) => connector.Status == MyShipConnectorStatus.Connected;

            static void Connect(IMyShipConnector connector, bool value) {
                if (value) { connector.Connect(); } else { connector.Disconnect(); }
            }
        }

        public class EjectorBlockHandler : FunctionalBlockHandler<IMyShipConnector> {
            public EjectorBlockHandler() : base() {
                AddBooleanHandler(Property.AUTO, b => b.ThrowOut, (b,v) => b.ThrowOut = v);
                AddBooleanHandler(Property.SUPPLY, b => !b.CollectAll, (b,v) => b.CollectAll = !v);
            }
        }
    }
}
