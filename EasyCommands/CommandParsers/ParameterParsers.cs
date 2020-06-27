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
        //Configuration.  Keep all words lowercase
        static String[] ignoreWords = { "the", "than", "turned", "is", "block", "tell", "to", "from", "then", "of", "either", "are", "for" };
        static String[] groupWords = { "blocks", "group" };
        static String[] activateWords = { "move", "go", "on", "begin", "true" };
        static String[] deactivateWords = { "off", "terminate", "exit", "cancel", "end", "false" };
        static String[] reverseWords = { "reverse", "switch direction", "turn around" };
        static String[] increaseWords = { "increase", "raise", "extend", "expand" };
        static String[] decreaseWords = { "decrease", "lower", "retract", "reduce" };
        static String[] upWords = { "up", "upward" };
        static String[] downWords = { "down", "downward" };
        static String[] leftWords = { "left", "lefthand" };
        static String[] rightWords = { "right", "righthand" };
        static String[] forwardWords = { "forward", "forwards", "front" };
        static String[] backwardWords = { "backward", "backwards", "back" };
        static String[] clockwiseWords = { "clockwise", "clock" };
        static String[] counterclockwiseWords = { "counter", "counterclock", "counterclockwise" };
        static String[] actionWords = { "tell", "turn", "rotate", "set"};

        static String[] relativeWords = { "by" };
        static String[] increaseRelativeWords = { "add" };
        static String[] decreaseRelativeWords = { "subtact" };
        static String[] heightWords = { "height", "length" };
        static String[] angleWords = { "angle" };
        static String[] speedWords = { "speed", "velocity", "rate", "pace" };
        static String[] waitWords = { "wait", "hold" };
        static String[] connectWords = { "connect", "join", "attach", "connected", "joined", "attached", "dock", "docked" };
        static String[] disconnectWords = { "disconnect", "separate", "detach", "disconnected", "separated", "detached", "undock", "undocked" };

        static String[] lockWords = { "lock", "locked", "freeze" };
        static String[] unlockWords = { "unlock", "unlocked", "unfreeze" };

        static String[] ifWords = { "if" };
        static String[] elseWords = { "else", "otherwise" };
        static String[] unlessWords = { "unless" };
        static String[] whileWords = { "while" };
        static String[] untilWords = { "until" };
        static String[] whenWords = { "when" };
        static String[] asyncWords = { "async" };

        static String[] lessWords = { "less", "<", "below" };
        static String[] lessEqualWords = { "<=" };
        static String[] equalWords = { "equal", "equals", "=", "==" };
        static String[] greaterEqualWords = { ">=" };
        static String[] greaterWords = { "greater", ">", "above", "more" };

        static String[] anyWords = { "any" };
        static String[] allWords = { "all" };
        static String[] noneWords = { "none" };

        static String[] andWords = { "and", "&", "&&", "but", "yet" };
        static String[] orWords = { "or", "|", "||" };
        static String[] notWords = { "not", "!", "isn't", "isnt" };
        static String[] openParenthesisWords = { "(" };
        static String[] closeParenthesisWords = { ")" };

        static String[] runWords = { "run", "execute" };
        static String[] runningWords = { "running", "executing" };
        static String[] stoppedWords = { "stopped", "terminated" };
        static String[] pausedWords = { "paused", "halted" };
        static String[] completeWords = { "done", "complete", "finished", "built" };
        static String[] progressWords = { "progress", "completion" };

        static String[] openWords = { "open", "opened" };
        static String[] closeWords = { "close", "closed", "shut" };

        static String[] gosubKeywords = { "call", "gosub" };
        static String[] gotoKeywords = { "goto" };

        static String[] listenKeywords = { "listen", "channel" };
        static String[] sendKeywords = { "send", "broadcast" };

        static String[] fontKeywords = { "fontsize", "size" };
        static String[] colorKeywords = { "color" };
        static String[] textKeywords = { "text", "message" };

        static String[] powerKeywords = { "power" };
        static String[] soundKeywords = { "music", "song" };
        static String[] volumeKeywords = { "volume" };
        static String[] rangeKeywords = { "range", "distance", "limit" };
        static String[] iterationKeywords = { "times", "iterations" };
        static String[] triggerWords = { "trigger", "triggered", "trip", "tripped"};
        static String[] consumeWords = { "consume", "stockpile", "depressurize", "depressurized", "gather", "intake" };
        static String[] produceWords = { "produce", "pressurize", "pressurized", "supply", "generate" };
        static String[] ratioWords = { "ratio", "percentage", "percent" };
        static String[] inputWords = { "input", "pilot", "user" };
        static String[] rollInputWords = { "roll", "rollInput" };
        static String[] autoWords = { "auto", "refill"};


        static Dictionary<String, UnitType> unitTypeWords = new Dictionary<String, UnitType>()
        {
            { "second", UnitType.SECONDS },
            { "seconds", UnitType.SECONDS },
            { "tick", UnitType.TICKS },
            { "ticks", UnitType.TICKS },
            { "degree", UnitType.DEGREES },
            { "degrees", UnitType.DEGREES },
            { "meter", UnitType.METERS },
            { "meters", UnitType.METERS },
            { "rpm", UnitType.RPM }
        };

        static Dictionary<String, BlockType> blockTypeGroupWords = new Dictionary<String, BlockType>() {
            { "pistons", BlockType.PISTON },
            { "lights", BlockType.LIGHT },
            { "rotors", BlockType.ROTOR },
            { "programs", BlockType.PROGRAM },
            { "timers", BlockType.TIMER },
            { "projectors", BlockType.PROJECTOR },
            { "connectors", BlockType.CONNECTOR },
            { "welders", BlockType.WELDER },
            { "grinders", BlockType.GRINDER },
            { "doors", BlockType.DOOR },
            { "hangars", BlockType.DOOR },
            { "bays", BlockType.DOOR },
            { "gates", BlockType.DOOR },
            { "displays", BlockType.DISPLAY },
            { "screens", BlockType.DISPLAY },
            { "monitors", BlockType.DISPLAY },
            { "lcds", BlockType.DISPLAY },
            { "speakers", BlockType.SOUND },
            { "alarms", BlockType.SOUND },
            { "cameras", BlockType.CAMERA },
            { "sensors", BlockType.SENSOR },
            { "beacons", BlockType.BEACON },
            { "antennae", BlockType.ANTENNA },
            { "antennas", BlockType.ANTENNA },
            { "ships", BlockType.COCKPIT },
            { "rovers", BlockType.COCKPIT },
            { "cockpits", BlockType.COCKPIT },
            { "seats", BlockType.COCKPIT },
            { "drones", BlockType.REMOTE },
            { "remotes", BlockType.REMOTE },
            { "robots", BlockType.REMOTE },
            { "thrusters", BlockType.THRUSTER },
            { "airvents", BlockType.AIRVENT },
            { "vents", BlockType.AIRVENT },
            { "guns", BlockType.GUN },
            { "launchers", BlockType.GUN },
            { "turrets", BlockType.GUN },
            { "reactors", BlockType.REACTOR },
            { "generators", BlockType.GENERATOR },
            { "tanks", BlockType.TANK },
            { "gears", BlockType.GEAR }
        };

        static Dictionary<String, BlockType> blockTypeWords = new Dictionary<String, BlockType>() {
            { "piston", BlockType.PISTON },
            { "light", BlockType.LIGHT },
            { "rotor", BlockType.ROTOR },
            { "program", BlockType.PROGRAM },
            { "timer", BlockType.TIMER },
            { "projector", BlockType.PROJECTOR },
            { "merge", BlockType.MERGE },
            { "connector", BlockType.CONNECTOR },
            { "welder", BlockType.WELDER },
            { "grinder", BlockType.GRINDER },
            { "door", BlockType.DOOR },
            { "hangar", BlockType.DOOR },
            { "bay", BlockType.DOOR },
            { "gate", BlockType.DOOR },
            { "display", BlockType.DISPLAY },
            { "screen", BlockType.DISPLAY },
            { "monitor", BlockType.DISPLAY },
            { "lcd", BlockType.DISPLAY },
            { "speaker", BlockType.SOUND },
            { "alarm", BlockType.SOUND },
            { "camera", BlockType.CAMERA },
            { "sensor", BlockType.SENSOR },
            { "beacon", BlockType.BEACON },
            { "antenna", BlockType.ANTENNA },
            { "ship", BlockType.COCKPIT },
            { "rover", BlockType.COCKPIT },
            { "cockpit", BlockType.COCKPIT },
            { "seat", BlockType.COCKPIT },
            { "drone", BlockType.REMOTE },
            { "remote", BlockType.REMOTE },
            { "robot", BlockType.REMOTE },
            { "thruster", BlockType.THRUSTER },
            { "airvent", BlockType.AIRVENT },
            { "vent", BlockType.AIRVENT },
            { "gun", BlockType.GUN },
            { "launcher", BlockType.GUN },
            { "turret", BlockType.GUN },
            { "reactor", BlockType.REACTOR },
            { "generator", BlockType.GENERATOR },
            { "tank", BlockType.TANK },
            { "gear", BlockType.GEAR }
        };

        static Dictionary<String, ControlType> controlTypeWords = new Dictionary<string, ControlType>()
        {
            { "start", ControlType.START },
            { "restart", ControlType.RESTART },
            { "reset", ControlType.RESTART },
            { "reboot", ControlType.RESTART },
            { "loop", ControlType.LOOP },
            { "iterate", ControlType.LOOP },
            { "repeat", ControlType.LOOP },
            { "rerun", ControlType.LOOP },
            { "replay", ControlType.LOOP },
            { "stop", ControlType.STOP },
            { "parse", ControlType.PARSE },
            { "pause", ControlType.PAUSE },
            { "resume", ControlType.RESUME },
        };

        static Dictionary<String, Color> colors = new Dictionary<String, Color>{
            { "red", Color.Red },
            { "blue", Color.Blue },
            { "green", Color.Green },
            { "orange", Color.Orange },
            { "yellow", Color.Yellow },
            { "white", Color.White },
            { "black", Color.Black}
        };

        //Internal (Don't touch!)
        private static Dictionary<String, List<CommandParameter>> propertyWords = new Dictionary<string, List<CommandParameter>>();

        public static void initParsers() {
            AddWords(groupWords, new GroupCommandParameter());
            AddWords(activateWords, new BooleanCommandParameter(true));
            AddWords(deactivateWords, new BooleanCommandParameter(false));
            AddWords(increaseWords, new ActionCommandParameter(), new DirectionCommandParameter(DirectionType.UP));
            AddWords(increaseRelativeWords, new ActionCommandParameter(), new RelativeCommandParameter(), new DirectionCommandParameter(DirectionType.UP));
            AddWords(decreaseWords, new ActionCommandParameter(), new DirectionCommandParameter(DirectionType.DOWN));
            AddWords(decreaseRelativeWords, new ActionCommandParameter(), new RelativeCommandParameter(), new DirectionCommandParameter(DirectionType.DOWN));
            AddWords(upWords, new DirectionCommandParameter(DirectionType.UP));
            AddWords(downWords, new DirectionCommandParameter(DirectionType.DOWN));
            AddWords(leftWords, new DirectionCommandParameter(DirectionType.LEFT));
            AddWords(rightWords, new DirectionCommandParameter(DirectionType.RIGHT));
            AddWords(forwardWords, new DirectionCommandParameter(DirectionType.FORWARD));
            AddWords(backwardWords, new DirectionCommandParameter(DirectionType.BACKWARD));
            AddWords(clockwiseWords, new DirectionCommandParameter(DirectionType.CLOCKWISE));
            AddWords(counterclockwiseWords, new DirectionCommandParameter(DirectionType.COUNTERCLOCKWISE));
            AddWords(actionWords, new ActionCommandParameter());
            AddWords(reverseWords, new ReverseCommandParameter());
            AddWords(relativeWords, new RelativeCommandParameter());
            AddWords(heightWords, new NumericPropertyCommandParameter(NumericPropertyType.HEIGHT));
            AddWords(angleWords, new NumericPropertyCommandParameter(NumericPropertyType.ANGLE));
            AddWords(speedWords, new NumericPropertyCommandParameter(NumericPropertyType.VELOCITY));
            AddWords(connectWords, new BooleanPropertyCommandParameter(BooleanPropertyType.CONNECTED));
            AddWords(disconnectWords, new BooleanPropertyCommandParameter(BooleanPropertyType.CONNECTED), new BooleanCommandParameter(false));
            AddWords(lockWords, new BooleanPropertyCommandParameter(BooleanPropertyType.LOCKED));
            AddWords(unlockWords, new BooleanPropertyCommandParameter(BooleanPropertyType.LOCKED), new BooleanCommandParameter(false));
            AddWords(waitWords, new WaitCommandParameter());
            AddWords(ifWords, new IfCommandParameter(false, false, false));
            AddWords(unlessWords, new IfCommandParameter(true, false, false));
            AddWords(whileWords, new IfCommandParameter(false, true, false));
            AddWords(untilWords, new IfCommandParameter(true, true, false));
            AddWords(whenWords, new IfCommandParameter(true, true, true));
            AddWords(asyncWords, new AsyncCommandParameter());
            AddWords(elseWords, new ElseCommandParameter());
            AddWords(lessWords, new ComparisonCommandParameter(ComparisonType.LESS));
            AddWords(lessEqualWords, new ComparisonCommandParameter(ComparisonType.LESS_OR_EQUAL));
            AddWords(equalWords, new ComparisonCommandParameter(ComparisonType.EQUAL));
            AddWords(greaterEqualWords, new ComparisonCommandParameter(ComparisonType.GREATER_OR_EQUAL));
            AddWords(greaterWords, new ComparisonCommandParameter(ComparisonType.GREATER));
            AddWords(anyWords, new AggregationModeCommandParameter(AggregationMode.ANY));
            AddWords(allWords, new AggregationModeCommandParameter(AggregationMode.ALL));
            AddWords(noneWords, new AggregationModeCommandParameter(AggregationMode.NONE));
            AddWords(andWords, new AndCommandParameter());
            AddWords(orWords, new OrCommandParameter());
            AddWords(notWords, new NotCommandParameter());
            AddWords(openParenthesisWords, new OpenParenthesisCommandParameter());
            AddWords(closeParenthesisWords, new CloseParenthesisCommandParameter());
            AddWords(runWords, new StringPropertyCommandParameter(StringPropertyType.RUN));
            AddWords(runningWords, new BooleanPropertyCommandParameter(BooleanPropertyType.RUNNING));
            AddWords(stoppedWords, new BooleanPropertyCommandParameter(BooleanPropertyType.STOPPED));
            AddWords(pausedWords, new BooleanPropertyCommandParameter(BooleanPropertyType.PAUSED));
            AddWords(completeWords, new BooleanPropertyCommandParameter(BooleanPropertyType.COMPLETE));
            AddWords(progressWords, new NumericPropertyCommandParameter(NumericPropertyType.RATIO));
            AddWords(openWords, new BooleanPropertyCommandParameter(BooleanPropertyType.OPEN), new BooleanCommandParameter(true));
            AddWords(closeWords, new BooleanPropertyCommandParameter(BooleanPropertyType.OPEN), new BooleanCommandParameter(false));
            AddWords(gosubKeywords, new FunctionCommandParameter(FunctionType.GOSUB));
            AddWords(gotoKeywords, new FunctionCommandParameter(FunctionType.GOTO));
            AddWords(listenKeywords, new ListenCommandParameter());
            AddWords(sendKeywords, new SendCommandParameter());
            AddWords(fontKeywords, new NumericPropertyCommandParameter(NumericPropertyType.FONT_SIZE));
            AddWords(textKeywords, new StringPropertyCommandParameter(StringPropertyType.TEXT));
            AddWords(colorKeywords, new StringPropertyCommandParameter(StringPropertyType.COLOR));
            AddWords(powerKeywords, new BooleanPropertyCommandParameter(BooleanPropertyType.POWER));
            AddWords(soundKeywords, new StringPropertyCommandParameter(StringPropertyType.SOUND));
            AddWords(volumeKeywords, new NumericPropertyCommandParameter(NumericPropertyType.VOLUME));
            AddWords(rangeKeywords, new NumericPropertyCommandParameter(NumericPropertyType.RANGE));
            AddWords(iterationKeywords, new IterationCommandParameter(1));
            AddWords(triggerWords, new BooleanPropertyCommandParameter(BooleanPropertyType.TRIGGER));
            AddWords(produceWords, new BooleanPropertyCommandParameter(BooleanPropertyType.PRODUCE));
            AddWords(consumeWords, new BooleanPropertyCommandParameter(BooleanPropertyType.PRODUCE), new BooleanCommandParameter(false));
            AddWords(ratioWords, new NumericPropertyCommandParameter(NumericPropertyType.RATIO));
            AddWords(inputWords, new NumericPropertyCommandParameter(NumericPropertyType.MOVE_INPUT));
            AddWords(rollInputWords, new NumericPropertyCommandParameter(NumericPropertyType.ROLL_INPUT));
            AddWords(autoWords, new BooleanPropertyCommandParameter(BooleanPropertyType.AUTO));
        }

        public static Color GetColor(String color) {
            if (colors.ContainsKey(color)) return colors[color];
            float rgb; if (float.TryParse(color, out rgb)) return new Color(rgb);
            throw new Exception("Unknown Color: " + color);
        }

        static void AddWords(String[] words, params CommandParameter[] commands) {
            foreach (String word in words) propertyWords.Add(word, commands.ToList());
        }

        static List<CommandParameter> ParseCommandParameters(List<Token> tokens) {
            Print("Command: " + String.Join(" | ", tokens));

            List<CommandParameter> commandParameters = new List<CommandParameter>();
            foreach (var token in tokens) {
                String t = token.token;

                if (token.isString) {
                    List<Token> subTokens = ParseTokens(t);
                    List<CommandParameter> subtokenParams = ParseCommandParameters(subTokens);
                    commandParameters.Add(new StringCommandParameter(token.original, subtokenParams.ToArray()));
                    continue;
                }

                if (ignoreWords.Contains(t)) continue;

                if (propertyWords.ContainsKey(t)) {
                    commandParameters.AddList(propertyWords[t]);
                    continue;
                }

                ControlType controlType;
                if (controlTypeWords.TryGetValue(t, out controlType)) {
                    commandParameters.Add(new ControlCommandParameter(controlType));
                    continue;
                }

                BlockType blockType;
                if (blockTypeGroupWords.TryGetValue(t, out blockType)) {
                    commandParameters.Add(new BlockTypeCommandParameter(blockType));
                    commandParameters.Add(new GroupCommandParameter());
                    continue;
                } else if (blockTypeWords.TryGetValue(t, out blockType)) {
                    commandParameters.Add(new BlockTypeCommandParameter(blockType));
                    continue;
                }

                UnitType unitType;
                if (unitTypeWords.TryGetValue(t, out unitType)) {
                    commandParameters.Add(new UnitCommandParameter(unitType));
                    continue;
                }

                double numericValue;
                if (Double.TryParse(t, out numericValue)) {
                    commandParameters.Add(new NumericCommandParameter((float)numericValue));
                    continue;
                }

                int indexValue;
                if (t.StartsWith("@")) {
                    if(int.TryParse(t.Substring(1), out indexValue)) {
                        commandParameters.Add(new IndexCommandParameter(indexValue));
                    }
                    continue;
                }
                //If nothing else matches, must be a string
                commandParameters.Add(new StringCommandParameter(token.original));
            }
            return commandParameters;
        }

        //Taken shamelessly from https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        public static List<Token> ParseTokens(String commandString) {
            return commandString.Trim().Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(t => new Token(t, false))  // Split the item
                                    : new Token[] { new Token(element, true) })  // Keep the entire item
                .SelectMany(element => element)
                .ToList();
        }

        public class Token {
            public String token;
            public String original;
            public bool isString;

            public Token(string token, bool isString) {
                this.isString = isString;
                this.token = token.ToLower();
                this.original = token;
            }

            public override string ToString() { return token; }
        }
    }
}
