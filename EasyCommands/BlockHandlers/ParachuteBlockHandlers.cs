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
                AddBooleanHandler(PropertyType.OPEN, (b) => b.Status != DoorStatus.Closed, (b, v) => { if (v) b.OpenDoor(); else b.CloseDoor(); });
                AddBooleanHandler(PropertyType.AUTO, (b) => b.GetValueBool("AutoDeploy"), (b, v) => b.SetValueBool("AutoDeploy", v));
                AddBooleanHandler(PropertyType.TRIGGER, (b) => b.Status != DoorStatus.Closed, (b, v) => { if (v) b.OpenDoor(); else b.CloseDoor(); });
                AddNumericHandler(PropertyType.RATIO, (b) => 1 - b.OpenRatio);
                AddPropertyHandler(PropertyType.HEIGHT, new PropertyValueNumericPropertyHandler<IMyParachute>("AutoDeployHeight", 500));
                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.OPEN;
                defaultPropertiesByPrimitive[PrimitiveType.NUMERIC] = PropertyType.HEIGHT;
                defaultPropertiesByDirection.Add(DirectionType.UP, PropertyType.RATIO);
                defaultDirection = DirectionType.UP;

            }
        }
    }
}
