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
        public enum LogLevel { DEBUG, INFO, SCRIPT_ONLY }
        #endregion

        public enum Block { PISTON, ROTOR, PROGRAM, TIMER, LIGHT, PROJECTOR, MERGE, CONNECTOR, WELDER, GRINDER, DOOR, DISPLAY, SOUND, CAMERA, SENSOR, BEACON, ANTENNA, COCKPIT, REMOTE, THRUSTER, AIRVENT, GUN, GENERATOR, TANK, GEAR, BATTERY, PARACHUTE, SUSPENSION, DETECTOR, DRILL, ENGINE, SORTER, TURRET, GYROSCOPE, GRAVITY_GENERATOR, GRAVITY_SPHERE, CARGO, WARHEAD, ASSEMBLER, EJECTOR, COLLECTOR, DECOY, HINGE, JUMPDRIVE, LASER_ANTENNA, TERMINAL, REFINERY, REACTOR, TURBINE, SOLAR_PANEL }
        public enum Property { ENABLE, POWER, CONNECTED, LOCKED, COMPLETE, OPEN, TRIGGER, SUPPLY, AUTO, OVERRIDE, LEVEL, ANGLE, VELOCITY, RATIO, FONT_SIZE, VOLUME, RANGE, INPUT, ROLL_INPUT, NAME, RUN, TEXT, COLOR, SONG, BLINK_INTERVAL, BLINK_LENGTH, OFFSET, INTENSITY, FALLOFF, POSITION, DIRECTION, TARGET, WAYPOINTS, TARGET_VELOCITY, STRENGTH, COUNTDOWN, SILENCE, SHOW, PROPERTIES, ACTIONS, NAMES }
        public enum ValueProperty { AMOUNT, CREATE, DESTROY, PROPERTY, ACTION };
        public enum Direction { UP, DOWN, LEFT, RIGHT, FORWARD, BACKWARD, CLOCKWISE, COUNTERCLOCKWISE, NONE }
        public enum Control { RESTART, STOP, REPEAT, PAUSE }
        public enum ProgramState { RUNNING, STOPPED, COMPLETE, PAUSED }
        public enum Return { NUMERIC, BOOLEAN, STRING, VECTOR, COLOR, LIST }
        public enum BiOperand { ADD, SUBTACT, MULTIPLY, DIVIDE, MOD, AND, OR, COMPARE, DOT, EXPONENT, RANGE, CAST, CONTAINS };
        public enum UniOperand { NOT, ABS, SQRT, SIN, COS, TAN, ASIN, ACOS, ATAN, ROUND, KEYS, VALUES, TICKS };
        public enum AggregationMode {ANY, ALL, NONE }
    }
}
