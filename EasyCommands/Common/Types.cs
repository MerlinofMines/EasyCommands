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
        #region mdk preserve
        public enum LogLevel { INFO, SCRIPT_ONLY }
        #endregion

        public enum Block {
            AIRVENT,
            ANTENNA,
            ASSEMBLER,
            BATTERY,
            BEACON,
            CAMERA,
            CARGO,
            COCKPIT,
            COLLECTOR,
            CONNECTOR,
            CRYO_CHAMBER,
            DECOY,
            DETECTOR,
            DISPLAY,
            DOOR,
            DRILL,
            EJECTOR,
            ENGINE,
            GENERATOR,
            GRAVITY_GENERATOR,
            GRAVITY_SPHERE,
            GRID,
            GRINDER,
            GUN,
            GYROSCOPE,
            HEAT_VENT,
            HINGE,
            JUMPDRIVE,
            LASER_ANTENNA,
            LIGHT,
            MAGNET,
            MERGE,
            PARACHUTE,
            PISTON,
            PROGRAM,
            PROJECTOR,
            REACTOR,
            REFINERY,
            REMOTE,
            ROTOR,
            SEARCHLIGHT,
            SENSOR,
            SOLAR_PANEL,
            SORTER,
            SOUND,
            SUSPENSION,
            TANK,
            TERMINAL,
            THRUSTER,
            TIMER,
            TURBINE,
            TURRET,
            TURRET_CONTROLLER,
            WARHEAD,
            WELDER
        }

        public enum Property {
            //Simple Properties
            ACTIONS,
            ALTITUDE,
            ANGLE,
            ARTIFICIAL_GRAVITY,
            AUTO,
            BACKGROUND,
            COLOR,
            COMPLETE,
            CONNECTED,
            COUNTDOWN,
            DATA,
            DIRECTION,
            ENABLE,
            FALLOFF,
            FONT,
            INFO,
            INPUT,
            INTERVAL,
            LEVEL,
            LOCKED,
            MEDIA_LIST,
            MEDIA,
            NAME,
            NAMES,
            NATURAL_GRAVITY,
            OFFSET,
            OPEN,
            OVERRIDE,
            POSITION,
            POWER,
            PROPERTIES,
            RADIUS,
            RANGE,
            RATIO,
            ROLL_INPUT,
            RUN,
            SHOW,
            STRENGTH,
            SUPPLY,
            TARGET_VELOCITY,
            TARGET,
            TEXT,
            TRIGGER,
            TYPES,
            USE,
            VELOCITY,
            VOLUME,
            WAYPOINTS,
            WEIGHT,

            //Value Properties
            ACTION,
            AMOUNT,
            CREATE,
            DESTROY,
            PROPERTY
        }

        public enum Direction { UP, DOWN, LEFT, RIGHT, FORWARD, BACKWARD, CLOCKWISE, COUNTERCLOCKWISE, NONE }
        public enum ProgramState { RUNNING, STOPPED, COMPLETE, PAUSED }
        public enum Return { NUMERIC, BOOLEAN, STRING, VECTOR, COLOR, LIST, DEFAULT }
        public enum BiOperand { ADD, SUBTRACT, MULTIPLY, DIVIDE, MOD, AND, OR, COMPARE, DOT, EXPONENT, RANGE, CAST, CONTAINS, SPLIT, JOIN, ROUND };
        public enum UniOperand { REVERSE, ABS, SQRT, SIN, COS, TAN, ASIN, ACOS, ATAN, ROUND, KEYS, VALUES, TICKS, SORT, LN, RANDOM, SHUFFLE, SIGN, CAST, TYPE};
        public enum AggregationMode {ANY, ALL, NONE }
    }
}
