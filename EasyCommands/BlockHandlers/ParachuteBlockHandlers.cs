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
                booleanPropertyGetters.Add(BooleanPropertyType.OPEN, (b) => b.Status != DoorStatus.Closed);//If at all open, you're open
                booleanPropertySetters.Add(BooleanPropertyType.OPEN, (b, v) => { if (v) b.OpenDoor(); else b.CloseDoor(); });
                booleanPropertyGetters.Add(BooleanPropertyType.AUTO, (b) => b.GetValueBool("AutoDeploy"));
                booleanPropertySetters.Add(BooleanPropertyType.AUTO, (b, v) => b.SetValueBool("AutoDeploy",v));
                booleanPropertyGetters.Add(BooleanPropertyType.TRIGGER, (b) => b.Status != DoorStatus.Closed);
                booleanPropertySetters.Add(BooleanPropertyType.TRIGGER, (b, v) => { if (v) b.OpenDoor(); else b.CloseDoor(); });
                defaultBooleanProperty = BooleanPropertyType.OPEN;

                numericPropertyGetters.Add(NumericPropertyType.RATIO, (b) => 1 - b.OpenRatio);
                numericPropertyGetters.Add(NumericPropertyType.HEIGHT, (b) => b.GetValueFloat("AutoDeployHeight"));
                numericPropertySetters.Add(NumericPropertyType.HEIGHT, new PropertyValueNumericPropertySetter<IMyParachute>("AutoDeployHeight", 500));
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.RATIO);

            }
        }
    }
}
