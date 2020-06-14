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
        public class DoorBlockHandler : FunctionalBlockHandler<IMyDoor> {
            public DoorBlockHandler() : base() {
                booleanPropertyGetters.Add(BooleanPropertyType.OPEN, (b) => b.Status != DoorStatus.Closed);//If at all open, you're open
                booleanPropertySetters.Add(BooleanPropertyType.OPEN, (b, v) => { if (v) b.OpenDoor(); else b.CloseDoor(); });
                defaultBooleanProperty = BooleanPropertyType.OPEN;
                //TODO: Add Opening, Closing Boolean Handlers?

                numericPropertyGetters.Add(NumericPropertyType.RATIO, (b) => 1 - b.OpenRatio);
                numericPropertySetters.Add(NumericPropertyType.RATIO, new DoorRatioSetter());
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.RATIO);

            }

            public class DoorRatioSetter : NumericPropertySetter<IMyDoor> {
                public DoorRatioSetter() {
                    Set = (b, v) => Exception();
                    SetDirection = (b, d, v) => Exception();
                    Increment = (b, v) => Exception();
                    IncrementDirection = (b, d, v) => Exception();
                    Reverse = (b) => b.ToggleDoor();
                    Move = (b, d) => {
                        if (d == DirectionType.UP) b.OpenDoor();
                        if (d == DirectionType.DOWN) b.CloseDoor();
                    };
                }
            }

            public static void Exception() { throw new Exception("Cannot manually set door open amount"); }

        }
    }
}
