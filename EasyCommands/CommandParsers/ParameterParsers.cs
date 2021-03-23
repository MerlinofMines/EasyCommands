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
        static String[] ignoreWords = { "the", "than", "turned", "block", "to", "from", "then", "of", "either", "for" };
        static String[] groupWords = { "blocks", "group" };
        static String[] activateWords = { "move", "go", "on", "begin", "true" };
        static String[] deactivateWords = { "off", "terminate", "exit", "cancel", "end", "false" };
        static String[] reverseWords = { "reverse" };
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
        static String[] actionWords = { "tell", "turn", "rotate", "set" };

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
        static String[] withWords = { "that", "with", "which", "whose" };
        static String[] withoutWords = { "without" };

        static String[] lessWords = { "less", "<", "below" };
        static String[] lessEqualWords = { "<=" };
        static String[] equalWords = { "is", "are", "equal", "equals", "=", "==" };
        static String[] greaterEqualWords = { ">=" };
        static String[] greaterWords = { "greater", ">", "above", "more" };

        static String[] anyWords = { "any" };
        static String[] allWords = { "all" };
        static String[] noneWords = { "none" };

        static String[] andWords = { "and", "&", "&&", "but", "yet" };
        static String[] orWords = { "or", "|", "||" };
        static String[] notWords = { "not", "!", "isn't", "isnt", "aren't", "arent" };
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
        static String[] switchKeywords = { "switch" };

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
        static String[] triggerWords = { "trigger", "triggered", "trip", "tripped", "deploy", "deployed", "shoot", "shooting", "shot" };
        static String[] consumeWords = { "consume", "stockpile", "depressurize", "depressurized", "gather", "intake", "recharge", "recharging" };
        static String[] produceWords = { "produce", "pressurize", "pressurized", "supply", "generate", "discharge", "discharging" };
        static String[] ratioWords = { "ratio", "percentage", "percent" };
        static String[] inputWords = { "input", "pilot", "user" };
        static String[] rollInputWords = { "roll", "rollInput" };
        static String[] autoWords = { "auto", "refill" };
        static String[] assignWords = { "assign", "allocate", "designate" };
        static String[] bindWords = { "bind", "tie", "link" };
        static String[] printWords = { "print", "log", "echo", "write" };

        static bool Initialized = false;

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

        static Dictionary<String, OperandType> operandWords = new Dictionary<String, OperandType> {
            { "plus", OperandType.ADD },
            { "+", OperandType.ADD },
            { "minus", OperandType.SUBTACT },
            { "-", OperandType.SUBTACT },
            { "multiply", OperandType.MULTIPLY },
            { "*", OperandType.MULTIPLY },
            { "divide", OperandType.DIVIDE },
            { "/", OperandType.DIVIDE },
            { "mod", OperandType.MOD },
            { "%", OperandType.MOD }
        };

        static Dictionary<String, BlockType> blockTypeGroupWords = new Dictionary<String, BlockType>() {
            { "pistons", BlockType.PISTON },
            { "lights", BlockType.LIGHT },
            { "rotors", BlockType.ROTOR },
            { "hinges", BlockType.ROTOR },
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
            { "sirens", BlockType.SOUND },
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
            { "gears", BlockType.GEAR },
            { "batteries", BlockType.BATTERY },
            { "chutes", BlockType.PARACHUTE},
            { "parachutes", BlockType.PARACHUTE},
            { "wheels", BlockType.SUSPENSION},
            { "suspension", BlockType.SUSPENSION},
            { "detectors", BlockType.DETECTOR},
            { "drills", BlockType.DRILL},
            { "engines", BlockType.ENGINE }
        };

        static Dictionary<String, BlockType> blockTypeWords = new Dictionary<String, BlockType>() {
            { "piston", BlockType.PISTON },
            { "light", BlockType.LIGHT },
            { "rotor", BlockType.ROTOR },
            { "hinge", BlockType.ROTOR },
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
            { "siren", BlockType.SOUND },
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
            { "gear", BlockType.GEAR },
            { "battery", BlockType.BATTERY },
            { "chute", BlockType.PARACHUTE},
            { "parachute", BlockType.PARACHUTE},
            { "wheel", BlockType.SUSPENSION},
            { "detector", BlockType.DETECTOR},
            { "drill", BlockType.DRILL},
            { "engine", BlockType.ENGINE }
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

        public static void InitializeParsers() {
            if (Initialized) return;

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
            AddWords(heightWords, new PropertyCommandParameter(PropertyType.HEIGHT));
            AddWords(angleWords, new PropertyCommandParameter(PropertyType.ANGLE));
            AddWords(speedWords, new PropertyCommandParameter(PropertyType.VELOCITY));
            AddWords(connectWords, new PropertyCommandParameter(PropertyType.CONNECTED));
            AddWords(disconnectWords, new PropertyCommandParameter(PropertyType.CONNECTED), new BooleanCommandParameter(false));
            AddWords(lockWords, new PropertyCommandParameter(PropertyType.LOCKED));
            AddWords(unlockWords, new PropertyCommandParameter(PropertyType.LOCKED), new BooleanCommandParameter(false));
            AddWords(waitWords, new WaitCommandParameter());
            AddWords(ifWords, new IfCommandParameter(false, false, false));
            AddWords(unlessWords, new IfCommandParameter(true, false, false));
            AddWords(whileWords, new IfCommandParameter(false, true, false));
            AddWords(untilWords, new IfCommandParameter(true, true, false));
            AddWords(whenWords, new IfCommandParameter(true, true, true));
            AddWords(asyncWords, new AsyncCommandParameter());
            AddWords(elseWords, new ElseCommandParameter());
            AddWords(withWords, new WithCommandParameter(false));
            AddWords(withoutWords, new WithCommandParameter(true));
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
            AddWords(runWords, new PropertyCommandParameter(PropertyType.RUN));
            AddWords(runningWords, new PropertyCommandParameter(PropertyType.RUNNING));
            AddWords(stoppedWords, new PropertyCommandParameter(PropertyType.STOPPED));
            AddWords(pausedWords, new PropertyCommandParameter(PropertyType.PAUSED));
            AddWords(completeWords, new PropertyCommandParameter(PropertyType.COMPLETE));
            AddWords(progressWords, new PropertyCommandParameter(PropertyType.RATIO));
            AddWords(openWords, new PropertyCommandParameter(PropertyType.OPEN), new BooleanCommandParameter(true));
            AddWords(closeWords, new PropertyCommandParameter(PropertyType.OPEN), new BooleanCommandParameter(false));
            AddWords(gosubKeywords, new FunctionCommandParameter(FunctionType.GOSUB));
            AddWords(gotoKeywords, new FunctionCommandParameter(FunctionType.GOTO));
            AddWords(switchKeywords, new FunctionCommandParameter(FunctionType.SWITCH));
            AddWords(listenKeywords, new ListenCommandParameter());
            AddWords(sendKeywords, new SendCommandParameter());
            AddWords(fontKeywords, new PropertyCommandParameter(PropertyType.FONT_SIZE));
            AddWords(textKeywords, new PropertyCommandParameter(PropertyType.TEXT));
            AddWords(colorKeywords, new PropertyCommandParameter(PropertyType.COLOR));
            AddWords(powerKeywords, new PropertyCommandParameter(PropertyType.POWER));
            AddWords(soundKeywords, new PropertyCommandParameter(PropertyType.SOUND));
            AddWords(volumeKeywords, new PropertyCommandParameter(PropertyType.VOLUME));
            AddWords(rangeKeywords, new PropertyCommandParameter(PropertyType.RANGE));
            AddWords(iterationKeywords, new IteratorCommandParameter());
            AddWords(triggerWords, new PropertyCommandParameter(PropertyType.TRIGGER));
            AddWords(produceWords, new PropertyCommandParameter(PropertyType.PRODUCE));
            AddWords(consumeWords, new PropertyCommandParameter(PropertyType.PRODUCE), new BooleanCommandParameter(false));
            AddWords(ratioWords, new PropertyCommandParameter(PropertyType.RATIO));
            AddWords(inputWords, new PropertyCommandParameter(PropertyType.MOVE_INPUT));
            AddWords(rollInputWords, new PropertyCommandParameter(PropertyType.ROLL_INPUT));
            AddWords(autoWords, new PropertyCommandParameter(PropertyType.AUTO));
            AddWords(assignWords, new AssignmentCommandParameter(false));
            AddWords(bindWords, new AssignmentCommandParameter(true));
            AddWords(printWords, new PrintCommandParameter());
            Initialized = true;
        }

        public static int HexToFloat(string hex) { return int.Parse(hex.ToUpper(), System.Globalization.NumberStyles.AllowHexSpecifier); }

        public static Color GetColor(String color) {
            if (colors.ContainsKey(color.ToLower())) return colors[color.ToLower()];
            if (color.StartsWith("#") && color.Length==7) {
                return new Color(HexToFloat(color.Substring(1, 2)), HexToFloat(color.Substring(3, 2)), HexToFloat(color.Substring(5, 2)));
            }
            float rgb; if (float.TryParse(color, out rgb)) return new Color(rgb);
            throw new Exception("Unknown Color: " + color);
        }

        static void AddWords(String[] words, params CommandParameter[] commands) {
            foreach (String word in words) propertyWords.Add(word, commands.ToList());
        }

        static List<CommandParameter> ParseCommandParameters(List<Token> tokens) {
            InitializeParsers();
            Trace("Command: " + String.Join(" | ", tokens));

            List<CommandParameter> commandParameters = new List<CommandParameter>();
            foreach (var token in tokens) {
                String t = token.token;

                if(token.isExplicitString) {
                    commandParameters.Add(new VariableCommandParameter(new StaticVariable(new StringPrimitive(token.original))));
                    continue;
                }

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

                OperandType operandType;
                if (operandWords.TryGetValue(t, out operandType)) {
                    if (operandType == OperandType.ADD || operandType == OperandType.SUBTACT) {
                        commandParameters.Add(new AddCommandParameter(operandType));
                    } else {
                        commandParameters.Add(new MultiplyCommandParameter(operandType));
                    }
                    continue;
                }

                double numericValue;
                if (Double.TryParse(t, out numericValue)) {
                    commandParameters.Add(new NumericCommandParameter((float)numericValue));
                    continue;
                }

                //TODO: Fix/Add support for more than hardcoded indexes
                if (t.StartsWith("@")) {
                    if (t.Length == 1) commandParameters.Add(new IndexCommandParameter());
                    else {
                        int indexValue;
                        if (int.TryParse(t.Substring(1), out indexValue)) {
                            commandParameters.Add(new IndexSelectorCommandParameter(new StaticVariable(new NumberPrimitive(indexValue))));
                        } else {
                            throw new Exception("Unable to parse index indicator: " + t);
                        }
                    }
                    continue;
                }

                //Variable References
                if (t.StartsWith("{") && t.EndsWith("}")) {
                    commandParameters.Add(new VariableCommandParameter(new InMemoryVariable(token.original.Substring(1, token.original.Length - 2))));
                    continue;
                }

                //Variable References used as Selectors
                if (t.StartsWith("[") && t.EndsWith("]")) {
                    commandParameters.Add(new VariableSelectorCommandParameter(new InMemoryVariable(token.original.Substring(1, token.original.Length - 2))));
                    continue;
                }

                //If nothing else matches, must be a string
                commandParameters.Add(new StringCommandParameter(token.original));
            }
            return commandParameters;
        }

        //Taken shamelessly from https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        public static List<Token> ParseTokens(String commandString) {
            List<Token> singleQuoteTokens = commandString.Trim().Split('\'')
            .SelectMany((element, index) => index % 2 == 0  // If even index
                ? ParseDoubleQuotes(element)  // Split the item
                : new Token[] { new Token(element, true, true) })  // Keep the entire item
            .ToList();

            return singleQuoteTokens;
        }

        static Token[] ParseDoubleQuotes(String commandString) {
            return commandString.Trim().Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(t => new Token(t, false, false))  // Split the item
                    : new Token[] { new Token(element, true, false) })  // Keep the entire item
                .SelectMany(element => element)
                .ToArray();
        }

        public class Token {
            public String token;
            public String original;
            public bool isString;
            public bool isExplicitString;

            public Token(string token, bool isString, bool isExplicitString) {
                this.isString = isString;
                this.isExplicitString = isExplicitString;
                this.token = token.ToLower();
                this.original = token;
            }

            public override string ToString() { return token; }
        }
    }
}
