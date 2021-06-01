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
        public enum Block { PISTON, ROTOR, PROGRAM, TIMER, LIGHT, PROJECTOR, MERGE, CONNECTOR, WELDER, GRINDER, DOOR, DISPLAY, SOUND, CAMERA, SENSOR, BEACON, ANTENNA, COCKPIT, REMOTE, THRUSTER, AIRVENT, GUN, REACTOR, GENERATOR, TANK, GEAR, BATTERY, PARACHUTE, SUSPENSION, DETECTOR, DRILL, ENGINE, SORTER, TURRET, GYROSCOPE, GRAVITY_GENERATOR, GRAVITY_SPHERE, CARGO, WARHEAD, ASSEMBLER, EJECTOR, COLLECTOR, DECOY }
        public enum Property { POWER, CONNECTED, LOCKED, COMPLETE, OPEN, TRIGGER, SUPPLY, AUTO, OVERRIDE, HEIGHT, ANGLE, VELOCITY, RATIO, FONT_SIZE, VOLUME, RANGE, MOVE_INPUT, ROLL_INPUT, NAME, RUN, TEXT, COLOR, SONG, BLINK_INTERVAL, BLINK_LENGTH, BLINK_OFFSET, INTENSITY, FALLOFF, POSITION, DIRECTION, TARGET, TARGET_VELOCITY, STRENGTH, COUNTDOWN, SILENCE }
        public enum ValueProperty { AMOUNT, CREATE, DESTROY };
        public enum Unit { SECONDS, TICKS, DEGREES, RADIANS, METERS, RPM }
        public enum Direction { UP, DOWN, LEFT, RIGHT, FORWARD, BACKWARD, CLOCKWISE, COUNTERCLOCKWISE }
        public enum Comparison { GREATER, GREATER_OR_EQUAL, EQUAL, LESS_OR_EQUAL, LESS, NOT_EQUALS }
        public enum Control { RESTART, STOP, REPEAT, PAUSE }
        public enum ProgramState { RUNNING, STOPPED, COMPLETE, PAUSED }
        public enum Function { GOTO, GOSUB, SWITCH }
        public enum Return { NUMERIC, BOOLEAN, STRING, VECTOR, COLOR }
        public enum BiOperand { ADD, SUBTACT, MULTIPLY, DIVIDE, MOD, AND, OR, COMPARE, DOT, EXPONENT };
        public enum UniOperand { NOT, ABS, SQRT, SIN, COS, TAN, ASIN, ACOS, ATAN, ROUND };
        public enum LogLevel { TRACE, DEBUG, INFO, SCRIPT_ONLY }
        public enum PropertyAggregate { VALUE, SUM, COUNT, AVG, MIN, MAX };
        #endregion
    }
}
