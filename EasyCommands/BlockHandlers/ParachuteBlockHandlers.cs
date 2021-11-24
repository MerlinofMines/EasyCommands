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
        public class ParachuteBlockHandler : FunctionalBlockHandler<IMyParachute> {
            public ParachuteBlockHandler() {
                var openHandler = BooleanHandler(b => b.Status != DoorStatus.Closed, (b, v) => { if (v) b.OpenDoor(); else b.CloseDoor(); });
                AddPropertyHandler(Property.OPEN, openHandler);
                AddPropertyHandler(Property.TRIGGER, openHandler);
                AddPropertyHandler(Property.AUTO, TerminalBlockPropertyHandler("AutoDeploy", true));
                AddPropertyHandler(Property.RANGE, TerminalBlockPropertyHandler("AutoDeployHeight", 500));
                AddNumericHandler(Property.RATIO, b => 1 - b.OpenRatio);
                AddNumericHandler(Property.VELOCITY, b => (float)b.GetVelocity().Length());
                AddNumericHandler(Property.STRENGTH, b => (float)b.GetTotalGravity().Length());
                AddNumericHandler(Property.LEVEL, b => {
                    Vector3D? closestPoint;
                    return (float)(b.TryGetClosestPoint(out closestPoint) ? closestPoint.Value.Length() : -1);
                });
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.OPEN;
                defaultPropertiesByPrimitive[Return.NUMERIC] = Property.LEVEL;
                defaultPropertiesByDirection.Add(Direction.UP, Property.RATIO);
            }
        }
    }
}
