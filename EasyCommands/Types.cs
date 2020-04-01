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
        public enum BlockType
        {
            PISTON,
            ROTOR,
            PROGRAM,
            TIMER,
            LIGHT,
            PROJECTOR,
            MERGE,
            CONNECTOR
        }

        public enum CommandParameterType
        {
            BLOCKTYPE,
            GROUP,
            ACTIVATE,
            DEACTIVATE,
            REVERSE,
            UNIT,
            INCREMENT,
            DECREMENT,
            DELAY,
            VELOCITY,
            SELECTOR,
            NUMERIC,
            RELATIVE,
            CONNECT,
            DISCONNECT,
            LOCK,
            UNLOCK,
            WAIT
        } 

        public enum PropertyType
        {
            ON_OFF,
            HEIGHT,
            CONNECTED,
            CONNECTABLE,
            ANGLE,
            VELOCITY,
            LOCKED,
            LOCKABLE
        }

        public enum UnitType
        {
            SECONDS,
            TICKS,
            DEGREES,
            RADIANS,
            METERS,
            RPM
        }
    }
}
