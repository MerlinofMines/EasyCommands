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
            public ConnectorBlockHandler() {
                var connectedHandler = BooleanHandler(b => b.Status == MyShipConnectorStatus.Connected, (b, v) => { if (v) b.Connect(); else b.Disconnect(); });

                AddPropertyHandler(Property.LOCKED, connectedHandler);
                AddPropertyHandler(Property.CONNECTED, connectedHandler);
                AddNumericHandler(Property.STRENGTH, b => b.PullStrength, (b, v) => b.PullStrength = v, 0.01f);

                var readyHandler = BooleanHandler(b => b.Status == MyShipConnectorStatus.Connectable);
                AddPropertyHandler(Property.ABLE, readyHandler);
                AddPropertyHandler(NewList(Property.ABLE, Property.LOCKED), readyHandler);
                AddPropertyHandler(NewList(Property.ABLE, Property.CONNECTED), readyHandler);
                AddStringHandler(Property.COUNTERPART, b => b.OtherConnector?.CustomName ?? "");

                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.STRENGTH;
            }
        }

        public class EjectorBlockHandler : FunctionalBlockHandler<IMyShipConnector> {
            public EjectorBlockHandler() {
                AddBooleanHandler(Property.AUTO, b => b.ThrowOut, (b,v) => b.ThrowOut = v);
                AddBooleanHandler(Property.SUPPLY, b => !b.CollectAll, (b,v) => b.CollectAll = !v);
            }
        }
    }
}
