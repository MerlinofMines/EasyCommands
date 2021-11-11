﻿using Sandbox.Game.EntityComponents;
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
                AddBooleanHandler(Property.OPEN, (b) => b.Status != DoorStatus.Closed, (b, v) => { if (v) b.OpenDoor(); else b.CloseDoor(); });
                AddPropertyHandler(Property.RATIO, new DoorRatioHandler());
                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.OPEN;
                defaultPropertiesByDirection[Direction.UP] = Property.RATIO;
                defaultDirection = Direction.UP;
            }

            public class DoorRatioHandler : PropertyHandler<IMyDoor> {
                public DoorRatioHandler() {
                    Get = (b, p) => ResolvePrimitive(1 - b.OpenRatio);
                    Set = (b, p, v) => Exception();
                    SetDirection = (b, p, d, v) => Exception();
                    Increment = (b, p, v) => Exception();
                    IncrementDirection = (b, p, d, v) => Exception();
                    Reverse = (b, p) => b.ToggleDoor();
                    Move = (b, p, d) => {
                        if (d == Direction.UP) b.OpenDoor();
                        if (d == Direction.DOWN) b.CloseDoor();
                    };
                }
            }

            public static void Exception() { throw new Exception("Cannot manually set door open amount"); }

        }
    }
}
