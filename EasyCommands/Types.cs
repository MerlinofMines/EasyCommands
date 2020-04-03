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
        public enum BlockType { PISTON, ROTOR, PROGRAM, TIMER, LIGHT, PROJECTOR, MERGE, CONNECTOR, WELDER, GRINDER }
        public enum BooleanPropertyType { ON_OFF, CONNECTED, CONNECTABLE, ANGLE, LOCKED, LOCKABLE }
        public enum NumericPropertyType { HEIGHT, ANGLE, SPEED }
        public enum StringPropertyType { NAME, BLUEPRINT }
        public enum UnitType { SECONDS, TICKS, DEGREES, RADIANS, METERS, RPM }
        public enum DirectionType { UP, DOWN, CLOCKWISE, COUNTERCLOCKWISE }
    }
}
