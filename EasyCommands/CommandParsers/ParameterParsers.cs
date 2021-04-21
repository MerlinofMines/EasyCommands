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
        static String[] decreaseWords = { "decrease", "retract", "reduce" };
        static String[] upWords = { "up", "upward", "upwards", "upper" };
        static String[] downWords = { "down", "downward", "downwards", "lower" };
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
        static String[] asyncWords = { "async", "background", "parallel" };
        static String[] queueWords = { "queue", "schedule" };
        static String[] withWords = { "that", "with", "which", "whose" };
        static String[] withoutWords = { "without" };

        static String[] lessWords = { "less", "<", "below" };
        static String[] lessEqualWords = { "<=" };
        static String[] equalWords = { "is", "are", "equal", "equals", "=", "==" };
        static String[] greaterEqualWords = { ">=" };
        static String[] greaterWords = { "greater", ">", "above", "more" };

        static String[] anyWords = { "any" };
        static String[] allWords = { "all", "every", "each" };
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
        static String[] directionWords = { "direction" };
        static String[] positionWords = { "coordinates", "position", "location" };
        static String[] openWords = { "open", "opened" };
        static String[] closeWords = { "close", "closed", "shut" };
        static String[] targetWords = { "target", "destination", "waypoint" };
        static String[] targetVelocityWords = { "targetvelocity" };

        static String[] gosubKeywords = { "call", "gosub" };
        static String[] gotoKeywords = { "goto" };

        static String[] listenKeywords = { "listen", "channel" };
        static String[] sendKeywords = { "send", "broadcast" };

        static String[] fontKeywords = { "fontsize", "size" };
        static String[] colorKeywords = { "color" };
        static String[] textKeywords = { "text", "message" };

        static String[] blinkIntervalKeywords = { "blinkinterval", "blinkInterval", "interval" };
        static String[] blinkLengthKeywords = { "blinklength", "blinkLength" };
        static String[] blinkOffsetKeywords = { "blinkoffset", "blinkOffset" };
        static String[] intensityKeywords = { "intensity" };
        static String[] falloffKeywrods = { "falloff" };

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
        static String[] autoWords = { "auto", "refill", "drain", "draining" };
        static String[] overrideWords = { "override", "overrides", "overridden" };

        static String[] assignWords = { "assign", "allocate", "designate" };
        static String[] globalWords = { "global" };
        static String[] bindWords = { "bind", "tie", "link" };
        static String[] printWords = { "print", "log", "echo", "write" };
        static String[] selfWords = { "my", "self", "this" };

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

        static Dictionary<String, UniOperandType> uniOperandWords = new Dictionary<String, UniOperandType> {
            { "abs", UniOperandType.ABS },
            { "absolute", UniOperandType.ABS },
            { "sqrt", UniOperandType.SQRT },
            { "sin", UniOperandType.SIN },
            { "cos", UniOperandType.COS },
            { "tan", UniOperandType.TAN },
            { "arcsin", UniOperandType.ASIN},
            { "asin", UniOperandType.ASIN },
            { "arccos", UniOperandType.ACOS },
            { "acos", UniOperandType.ACOS },
            { "arctan", UniOperandType.ATAN },
            { "atan", UniOperandType.ATAN },
            { "round", UniOperandType.ROUND },
        };

        static Dictionary<String, BiOperandType> biOperandWords = new Dictionary<String, BiOperandType> {
            { "plus", BiOperandType.ADD },
            { "+", BiOperandType.ADD },
            { "minus", BiOperandType.SUBTACT },
            { "-", BiOperandType.SUBTACT },
            { "multiply", BiOperandType.MULTIPLY },
            { "*", BiOperandType.MULTIPLY },
            { "divide", BiOperandType.DIVIDE },
            { "/", BiOperandType.DIVIDE },
            { "mod", BiOperandType.MOD },
            { "%", BiOperandType.MOD },
            { "dot", BiOperandType.DOT },
            { ".", BiOperandType.DOT },
        };

        static Dictionary<String, PropertyAggregatorType> aggregationWords = new Dictionary<String, PropertyAggregatorType> {
            { "average", PropertyAggregatorType.AVG },
            { "avg", PropertyAggregatorType.AVG },
            { "min", PropertyAggregatorType.MIN },
            { "minimum", PropertyAggregatorType.MIN },
            { "max", PropertyAggregatorType.MAX },
            { "maximum", PropertyAggregatorType.MAX},
            { "count", PropertyAggregatorType.COUNT},
            { "number", PropertyAggregatorType.COUNT},
            { "sum", PropertyAggregatorType.SUM},
            { "total", PropertyAggregatorType.SUM},
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
            { "rockets", BlockType.GUN },
            { "missiles", BlockType.GUN },
            { "launchers", BlockType.GUN },
            { "turrets", BlockType.TURRET },
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
            { "engines", BlockType.ENGINE },
            { "sorters", BlockType.SORTER },
            { "gyros", BlockType.GYROSCOPE },
            { "gyroscopes", BlockType.GYROSCOPE }
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
            { "rocket", BlockType.GUN },
            { "missile", BlockType.GUN },
            { "launcher", BlockType.GUN },
            { "turret", BlockType.TURRET },
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
            { "engine", BlockType.ENGINE },
            { "sorter", BlockType.SORTER },
            { "gyro", BlockType.GYROSCOPE },
            { "gyroscope", BlockType.GYROSCOPE }
        };

        static Dictionary<String, ControlType> controlTypeWords = new Dictionary<string, ControlType>()
        {
            { "start", ControlType.START },
            { "resume", ControlType.START },
            { "restart", ControlType.RESTART },
            { "reset", ControlType.RESTART },
            { "reboot", ControlType.RESTART },
            { "repeat", ControlType.REPEAT },
            { "rerun", ControlType.REPEAT },
            { "replay", ControlType.REPEAT },
            { "stop", ControlType.STOP },
            { "pause", ControlType.PAUSE },
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
            AddWords(queueWords, new QueueCommandParameter(false));
            AddWords(asyncWords, new QueueCommandParameter(true));
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
            AddWords(listenKeywords, new ListenCommandParameter());
            AddWords(sendKeywords, new SendCommandParameter());
            AddWords(fontKeywords, new PropertyCommandParameter(PropertyType.FONT_SIZE));
            AddWords(textKeywords, new PropertyCommandParameter(PropertyType.TEXT));
            AddWords(colorKeywords, new PropertyCommandParameter(PropertyType.COLOR));
            AddWords(powerKeywords, new PropertyCommandParameter(PropertyType.POWER));
            AddWords(soundKeywords, new PropertyCommandParameter(PropertyType.SOUND));
            AddWords(volumeKeywords, new PropertyCommandParameter(PropertyType.VOLUME));
            AddWords(rangeKeywords, new PropertyCommandParameter(PropertyType.RANGE));
            AddWords(blinkIntervalKeywords, new PropertyCommandParameter(PropertyType.BLINK_INTERVAL));
            AddWords(blinkLengthKeywords, new PropertyCommandParameter(PropertyType.BLINK_LENGTH));
            AddWords(blinkOffsetKeywords, new PropertyCommandParameter(PropertyType.BLINK_OFFSET));
            AddWords(intensityKeywords, new PropertyCommandParameter(PropertyType.INTENSITY));
            AddWords(falloffKeywrods, new PropertyCommandParameter(PropertyType.FALLOFF));
            AddWords(iterationKeywords, new IteratorCommandParameter());
            AddWords(triggerWords, new PropertyCommandParameter(PropertyType.TRIGGER));
            AddWords(produceWords, new PropertyCommandParameter(PropertyType.PRODUCE));
            AddWords(consumeWords, new PropertyCommandParameter(PropertyType.PRODUCE), new BooleanCommandParameter(false));
            AddWords(ratioWords, new PropertyCommandParameter(PropertyType.RATIO));
            AddWords(inputWords, new PropertyCommandParameter(PropertyType.MOVE_INPUT));
            AddWords(rollInputWords, new PropertyCommandParameter(PropertyType.ROLL_INPUT));
            AddWords(autoWords, new PropertyCommandParameter(PropertyType.AUTO));
            AddWords(overrideWords, new PropertyCommandParameter(PropertyType.OVERRIDE));
            AddWords(directionWords, new PropertyCommandParameter(PropertyType.DIRECTION));
            AddWords(positionWords, new PropertyCommandParameter(PropertyType.POSITION));
            AddWords(targetWords, new PropertyCommandParameter(PropertyType.TARGET));
            AddWords(targetVelocityWords, new PropertyCommandParameter(PropertyType.TARGET_VELOCITY));
            AddWords(assignWords, new AssignmentCommandParameter(false));
            AddWords(globalWords, new GlobalCommandParameter());
            AddWords(bindWords, new AssignmentCommandParameter(true));
            AddWords(printWords, new PrintCommandParameter());

            //Register Special CommandParameter Output Values
            RegisterToString<GroupCommandParameter>(p => "group");
            RegisterToString<StringCommandParameter>(p => "\"" + p.value + "\"");
            RegisterToString<ExplicitStringCommandParameter>(p => "'" + p.value + "'");
            RegisterToString<VariableAssignmentCommandParameter>(p => "Assign[name=" + p.variableName + ",global=" + p.isGlobal + ",ref=" + p.useReference + "]");
            RegisterToString<VariableCommandParameter>(p => "[Variable]");
            RegisterToString<VariableSelectorCommandParameter>(p => "[VariableSelector]");
            RegisterToString<IndexSelectorCommandParameter>(p => "[IndexSelector]");
            RegisterToString<FunctionDefinitionCommandParameter>(p => "Function[type=" + p.functionType + ",name=" + p.functionDefinition.functionName + "]");
            RegisterToString<ConditionCommandParameter>(p => "[Condition]");
            RegisterToString<BlockConditionCommandParameter>(p => "[BlockCondition]");
            RegisterToString<CommandReferenceParameter>(p => "[Command]");
            RegisterToString<IterationCommandParameter>(p => "[Iteration]");
            RegisterToString<SelectorCommandParameter>(p => "[Selector]");
            Initialized = true;
        }

        static Dictionary<Type, Func<CommandParameter, object>> commandParameterStrings = new Dictionary<Type, Func<CommandParameter, object>>();

        static void RegisterToString<T>(Func<T, object> toString) where T : CommandParameter {
            commandParameterStrings[typeof(T)] = (p) => toString((T)p);
        }

        static string CommandParameterToString(CommandParameter parameter) {
            Func<CommandParameter, object> func;
            if (!commandParameterStrings.TryGetValue(parameter.GetType(), out func)) {
                func = (p) => p.Token;
            }
            return func(parameter).ToString();
        }

        static void AddWords(String[] words, params CommandParameter[] commands) {
            foreach (String word in words) propertyWords.Add(word, commands.ToList());
        }

        static List<CommandParameter> ParseCommandParameters(List<Token> tokens) {
            InitializeParsers();
            Trace("Command: " + String.Join(" | ", tokens));
            var parameters = new List<CommandParameter>();

            foreach (var token in tokens) {
                var newParameters = ParseCommandParameters(token);
                newParameters.ForEach(p => p.Token = token.original);
                parameters.AddRange(newParameters);
            }

            return parameters;
        }

        static List<CommandParameter> ParseCommandParameters(Token token) {
            List<CommandParameter> commandParameters = new List<CommandParameter>();
            String t = token.token;
            ControlType controlType;
            BlockType blockType;
            UnitType unitType;
            UniOperandType uniOperandType;
            BiOperandType biOperandType;
            PropertyAggregatorType aggregatorType;
            double numericValue;

            if (token.isExplicitString) {
                commandParameters.Add(new ExplicitStringCommandParameter(token.original));
            } else if (token.isString) {
                List<Token> subTokens = ParseTokens(t);
                List<CommandParameter> subtokenParams = ParseCommandParameters(subTokens);
                commandParameters.Add(new StringCommandParameter(token.original, subtokenParams.ToArray()));
            } else if (ignoreWords.Contains(t)) { 
                //ignore
            } else if (propertyWords.ContainsKey(t)) {
                commandParameters.AddList(propertyWords[t]);
            } else if (selfWords.Contains(t)) {
                commandParameters.Add(new SelfCommandParameter());
            } else if (controlTypeWords.TryGetValue(t, out controlType)) {
                commandParameters.Add(new ControlCommandParameter(controlType));
            } else if (blockTypeGroupWords.TryGetValue(t, out blockType)) {
                commandParameters.Add(new BlockTypeCommandParameter(blockType));
                commandParameters.Add(new GroupCommandParameter());
            } else if (blockTypeWords.TryGetValue(t, out blockType)) {
                commandParameters.Add(new BlockTypeCommandParameter(blockType));
            } else if (unitTypeWords.TryGetValue(t, out unitType)) {
                commandParameters.Add(new UnitCommandParameter(unitType));
            } else if (uniOperandWords.TryGetValue(t, out uniOperandType)) {
                commandParameters.Add(new UniOperationCommandParameter(uniOperandType));
            } else if (biOperandWords.TryGetValue(t, out biOperandType)) {
                if (biOperandType == BiOperandType.ADD || biOperandType == BiOperandType.SUBTACT) {
                    commandParameters.Add(new AddCommandParameter(biOperandType));
                } else {
                    commandParameters.Add(new MultiplyCommandParameter(biOperandType));
                }
            } else if (aggregationWords.TryGetValue(t, out aggregatorType)) {
                commandParameters.Add(new PropertyAggregationCommandParameter(aggregatorType));
            } else if (Double.TryParse(t, out numericValue)) {
                commandParameters.Add(new NumericCommandParameter((float)numericValue));
            } else if (t.StartsWith("@")) {
                if (t.Length == 1) commandParameters.Add(new IndexCommandParameter());
                else {
                    int indexValue;
                    if (int.TryParse(t.Substring(1), out indexValue)) {
                        commandParameters.Add(new IndexSelectorCommandParameter(new StaticVariable(new NumberPrimitive(indexValue))));
                    } else {
                        throw new Exception("Unable to parse index indicator: " + t);
                    }
                }
            } else if (t.StartsWith("{") && t.EndsWith("}")) { //Variable References
                commandParameters.Add(new VariableCommandParameter(new InMemoryVariable(token.original.Substring(1, token.original.Length - 2))));
            } else if (t.StartsWith("[") && t.EndsWith("]")) { //Variable References used as Selectors
                commandParameters.Add(new VariableSelectorCommandParameter(new InMemoryVariable(token.original.Substring(1, token.original.Length - 2))));
            } else { //If nothing else matches, must be a string
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
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(ParseParenthesis)
                    .Select(t => new Token(t, false, false))  // Split the item
                    : new Token[] { new Token(element, true, false) })  // Keep the entire item
                .SelectMany(element => element)
                .ToArray();
        }

        static String[] ParseParenthesis(String command) {
            List<String> tokens = new List<String>();
            if (command.StartsWith("(") || command.StartsWith(")")) {
                tokens.Add(command.Substring(0, 1));
                tokens.AddRange(ParseParenthesis(command.Substring(1)));
            } else if (command.EndsWith("(") || command.EndsWith(")")) {
                tokens.Add(command.Substring(0, command.Length - 1));
                tokens.Add(command.Substring(command.Length - 1));
            } else {
                tokens.Add(command);
            }
            return tokens.Where(t => t.Length>0).ToArray();
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
