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
        static bool Initialized = false;

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
            //Configuration.  Keep all words lowercase
            if (Initialized) return;

            //Ignored words that have no command parameters
            AddWords(Words("the", "than", "turned", "block", "to", "from", "then", "of", "either", "for"));

            //Selector Related Words
            AddWords(Words("blocks", "group"), new GroupCommandParameter());
            AddWords(Words("my", "self", "this"), new SelfCommandParameter());

            //Direction Words
            AddWords(Words("up", "upward", "upwards", "upper"), new DirectionCommandParameter(DirectionType.UP));
            AddWords(Words("down", "downward", "downwards", "lower"), new DirectionCommandParameter(DirectionType.DOWN));
            AddWords(Words("left", "lefthand"), new DirectionCommandParameter(DirectionType.LEFT));
            AddWords(Words("right", "righthand"), new DirectionCommandParameter(DirectionType.RIGHT));
            AddWords(Words("forward", "forwards", "front"), new DirectionCommandParameter(DirectionType.FORWARD));
            AddWords(Words("backward", "backwards", "back"), new DirectionCommandParameter(DirectionType.BACKWARD));
            AddWords(Words("clockwise", "clock"), new DirectionCommandParameter(DirectionType.CLOCKWISE));
            AddWords(Words("counter", "counterclock", "counterclockwise"), new DirectionCommandParameter(DirectionType.COUNTERCLOCKWISE));

            //Action Words
            AddWords(Words("move", "go", "tell", "turn", "rotate", "set"), new ActionCommandParameter());
            AddWords(Words("increase", "raise", "extend", "expand"), new ActionCommandParameter(), new DirectionCommandParameter(DirectionType.UP));
            AddWords(Words("add"), new ActionCommandParameter(), new RelativeCommandParameter(), new DirectionCommandParameter(DirectionType.UP));
            AddWords(Words("subtact"), new ActionCommandParameter(), new RelativeCommandParameter(), new DirectionCommandParameter(DirectionType.DOWN));
            AddWords(Words("decrease", "retract", "reduce"), new ActionCommandParameter(), new DirectionCommandParameter(DirectionType.DOWN));
            AddWords(Words("reverse"), new ReverseCommandParameter());
            AddWords(Words("by"), new RelativeCommandParameter());

            //Value Words
            AddWords(Words("on", "begin", "true"), new BooleanCommandParameter(true));
            AddWords(Words("off", "terminate", "cancel", "end", "false"), new BooleanCommandParameter(false));

            //Property Words
            AddWords(Words("height", "length"), new PropertyCommandParameter(PropertyType.HEIGHT));
            AddWords(Words("angle"), new PropertyCommandParameter(PropertyType.ANGLE));
            AddWords(Words("speed", "velocity", "rate", "pace"), new PropertyCommandParameter(PropertyType.VELOCITY));
            AddWords(Words("connect", "join", "attach", "connected", "joined", "attached", "dock", "docked"), new PropertyCommandParameter(PropertyType.CONNECTED));
            AddWords(Words("disconnect", "separate", "detach", "disconnected", "separated", "detached", "undock", "undocked"), new PropertyCommandParameter(PropertyType.CONNECTED), new BooleanCommandParameter(false));
            AddWords(Words("lock", "locked", "freeze"), new PropertyCommandParameter(PropertyType.LOCKED));
            AddWords(Words("unlock", "unlocked", "unfreeze"), new PropertyCommandParameter(PropertyType.LOCKED), new BooleanCommandParameter(false));
            AddWords(Words("run", "execute"), new PropertyCommandParameter(PropertyType.RUN));
            AddWords(Words("running", "executing"), new PropertyCommandParameter(PropertyType.RUNNING));
            AddWords(Words("stopped", "terminated"), new PropertyCommandParameter(PropertyType.STOPPED));
            AddWords(Words("paused", "halted"), new PropertyCommandParameter(PropertyType.PAUSED));
            AddWords(Words("done", "complete", "finished", "built"), new PropertyCommandParameter(PropertyType.COMPLETE));
            AddWords(Words("progress", "completion"), new PropertyCommandParameter(PropertyType.RATIO));
            AddWords(Words("open", "opened"), new PropertyCommandParameter(PropertyType.OPEN), new BooleanCommandParameter(true));
            AddWords(Words("close", "closed", "shut"), new PropertyCommandParameter(PropertyType.OPEN), new BooleanCommandParameter(false));
            AddWords(Words("fontsize", "size"), new PropertyCommandParameter(PropertyType.FONT_SIZE));
            AddWords(Words("text", "message"), new PropertyCommandParameter(PropertyType.TEXT));
            AddWords(Words("color"), new PropertyCommandParameter(PropertyType.COLOR));
            AddWords(Words("power"), new PropertyCommandParameter(PropertyType.POWER));
            AddWords(Words("music", "song"), new PropertyCommandParameter(PropertyType.SOUND));
            AddWords(Words("volume"), new PropertyCommandParameter(PropertyType.VOLUME));
            AddWords(Words("range", "distance", "limit", "radius"), new PropertyCommandParameter(PropertyType.RANGE));
            AddWords(Words("blinkinterval", "blinkInterval", "interval"), new PropertyCommandParameter(PropertyType.BLINK_INTERVAL));
            AddWords(Words("blinklength", "blinkLength"), new PropertyCommandParameter(PropertyType.BLINK_LENGTH));
            AddWords(Words("blinkoffset", "blinkOffset"), new PropertyCommandParameter(PropertyType.BLINK_OFFSET));
            AddWords(Words("intensity"), new PropertyCommandParameter(PropertyType.INTENSITY));
            AddWords(Words("falloff"), new PropertyCommandParameter(PropertyType.FALLOFF));
            AddWords(Words("times", "iterations"), new IteratorCommandParameter());
            AddWords(Words("trigger", "triggered", "trip", "tripped", "deploy", "deployed", "shoot", "shooting", "shot"), new PropertyCommandParameter(PropertyType.TRIGGER));
            AddWords(Words("produce", "pressurize", "pressurized", "supply", "generate", "discharge", "discharging"), new PropertyCommandParameter(PropertyType.PRODUCE));
            AddWords(Words("consume", "stockpile", "depressurize", "depressurized", "gather", "intake", "recharge", "recharging"), new PropertyCommandParameter(PropertyType.PRODUCE), new BooleanCommandParameter(false));
            AddWords(Words("ratio", "percentage", "percent"), new PropertyCommandParameter(PropertyType.RATIO));
            AddWords(Words("input", "pilot", "user"), new PropertyCommandParameter(PropertyType.MOVE_INPUT));
            AddWords(Words("roll", "rollInput"), new PropertyCommandParameter(PropertyType.ROLL_INPUT));
            AddWords(Words("auto", "refill", "drain", "draining"), new PropertyCommandParameter(PropertyType.AUTO));
            AddWords(Words("override", "overrides", "overridden"), new PropertyCommandParameter(PropertyType.OVERRIDE));
            AddWords(Words("direction"), new PropertyCommandParameter(PropertyType.DIRECTION));
            AddWords(Words("coordinates", "position", "location"), new PropertyCommandParameter(PropertyType.POSITION));
            AddWords(Words("target", "destination", "waypoint"), new PropertyCommandParameter(PropertyType.TARGET));
            AddWords(Words("targetvelocity"), new PropertyCommandParameter(PropertyType.TARGET_VELOCITY));
            AddWords(Words("strength", "force", "gravity"), new PropertyCommandParameter(PropertyType.STRENGTH));

            //Special Command Words
            AddWords(Words("wait", "hold"), new WaitCommandParameter());
            AddWords(Words("call", "gosub"), new FunctionCommandParameter(FunctionType.GOSUB));
            AddWords(Words("goto"), new FunctionCommandParameter(FunctionType.GOTO));
            AddWords(Words("listen", "channel"), new ListenCommandParameter());
            AddWords(Words("send", "broadcast"), new SendCommandParameter());
            AddWords(Words("assign", "allocate", "designate"), new AssignmentCommandParameter(false));
            AddWords(Words("global"), new GlobalCommandParameter());
            AddWords(Words("bind", "tie", "link"), new AssignmentCommandParameter(true));
            AddWords(Words("print", "log", "echo", "write"), new PrintCommandParameter());
            AddWords(Words("queue", "schedule"), new QueueCommandParameter(false));
            AddWords(Words("async", "background", "parallel"), new QueueCommandParameter(true));

            //Conditional Words
            AddWords(Words("if"), new IfCommandParameter(false, false, false));
            AddWords(Words("unless"), new IfCommandParameter(true, false, false));
            AddWords(Words("while"), new IfCommandParameter(false, true, false));
            AddWords(Words("until"), new IfCommandParameter(true, true, false));
            AddWords(Words("when"), new IfCommandParameter(true, true, true));
            AddWords(Words("else", "otherwise"), new ElseCommandParameter());
            AddWords(Words("that", "with", "which", "whose"), new WithCommandParameter(false));
            AddWords(Words("without"), new WithCommandParameter(true));

            //Comparison Words
            AddWords(Words("less", "<", "below"), new ComparisonCommandParameter(ComparisonType.LESS));
            AddWords(Words("<="), new ComparisonCommandParameter(ComparisonType.LESS_OR_EQUAL));
            AddWords(Words("is", "are", "equal", "equals", "=", "=="), new ComparisonCommandParameter(ComparisonType.EQUAL));
            AddWords(Words(">="), new ComparisonCommandParameter(ComparisonType.GREATER_OR_EQUAL));
            AddWords(Words("greater", ">", "above", "more"), new ComparisonCommandParameter(ComparisonType.GREATER));

            //Aggregation Words
            AddWords(Words("any"), new AggregationModeCommandParameter(AggregationMode.ANY));
            AddWords(Words("all", "every", "each"), new AggregationModeCommandParameter(AggregationMode.ALL));
            AddWords(Words("none"), new AggregationModeCommandParameter(AggregationMode.NONE));
            AddWords(Words("average", "avg"), new PropertyAggregationCommandParameter(PropertyAggregatorType.AVG));
            AddWords(Words("minimum", "min"), new PropertyAggregationCommandParameter(PropertyAggregatorType.MIN));
            AddWords(Words("maximum", "max"), new PropertyAggregationCommandParameter(PropertyAggregatorType.MAX));
            AddWords(Words("count", "number"), new PropertyAggregationCommandParameter(PropertyAggregatorType.COUNT));
            AddWords(Words("sum", "total"), new PropertyAggregationCommandParameter(PropertyAggregatorType.SUM));

            //Operations Words
            AddWords(Words("("), new OpenParenthesisCommandParameter());
            AddWords(Words(")"), new CloseParenthesisCommandParameter());
            AddWords(Words("and", "&", "&&", "but", "yet"), new AndCommandParameter());
            AddWords(Words("or", "|", "||"), new OrCommandParameter());
            AddWords(Words("not", "!", "isn't", "isnt", "aren't", "arent"), new NotCommandParameter());
            AddWords(Words("absolute", "abs"), new UniOperationCommandParameter(UniOperandType.ABS));
            AddWords(Words("sqrt"), new UniOperationCommandParameter(UniOperandType.SQRT));
            AddWords(Words("sin"), new UniOperationCommandParameter(UniOperandType.SIN));
            AddWords(Words("cosine", "cos"), new UniOperationCommandParameter(UniOperandType.COS));
            AddWords(Words("tangent", "tan"), new UniOperationCommandParameter(UniOperandType.TAN));
            AddWords(Words("arcsin", "asin"), new UniOperationCommandParameter(UniOperandType.ASIN));
            AddWords(Words("arcos", "acos"), new UniOperationCommandParameter(UniOperandType.ACOS));
            AddWords(Words("arctan", "atan"), new UniOperationCommandParameter(UniOperandType.ATAN));
            AddWords(Words("round", "rnd"), new UniOperationCommandParameter(UniOperandType.ROUND));
            AddWords(Words("plus", "+"), new AddCommandParameter(BiOperandType.ADD));
            AddWords(Words("minus", "-"), new AddCommandParameter(BiOperandType.SUBTACT));
            AddWords(Words("multiply", "*"), new MultiplyCommandParameter(BiOperandType.MULTIPLY));
            AddWords(Words("divide", "/"), new MultiplyCommandParameter(BiOperandType.DIVIDE));
            AddWords(Words("mod", "%"), new MultiplyCommandParameter(BiOperandType.MOD));
            AddWords(Words("dot", "."), new MultiplyCommandParameter(BiOperandType.DOT));

            //Unit Words
            AddWords(Words("second", "seconds"), new UnitCommandParameter(UnitType.SECONDS));
            AddWords(Words("tick", "ticks"), new UnitCommandParameter(UnitType.TICKS));
            AddWords(Words("degree", "degrees"), new UnitCommandParameter(UnitType.DEGREES));
            AddWords(Words("meter", "meters"), new UnitCommandParameter(UnitType.METERS));
            AddWords(Words("rpm"), new UnitCommandParameter(UnitType.RPM));

            //Control Types
            AddWords(Words("start", "resume"), new ControlCommandParameter(ControlType.START));
            AddWords(Words("restart", "reset", "reboot"), new ControlCommandParameter(ControlType.RESTART));
            AddWords(Words("repeat", "loop", "rerun", "replay"), new ControlCommandParameter(ControlType.REPEAT));
            AddWords(Words("stop", "halt", "exit"), new ControlCommandParameter(ControlType.STOP));
            AddWords(Words("pause"), new ControlCommandParameter(ControlType.PAUSE));

            //Blocks
            AddBlockWords(Words("piston"), BlockType.PISTON);
            AddBlockWords(Words("light"), BlockType.LIGHT);
            AddBlockWords(Words("rotor", "hinge"), BlockType.ROTOR);
            AddBlockWords(Words("program"), BlockType.PROGRAM);
            AddBlockWords(Words("timer"), BlockType.TIMER);
            AddBlockWords(Words("projector"), BlockType.PROJECTOR);
            AddBlockWords(Words("merge"), Words(), BlockType.MERGE);
            AddBlockWords(Words("connector"), BlockType.CONNECTOR);
            AddBlockWords(Words("welder"), BlockType.WELDER);
            AddBlockWords(Words("grinder"), BlockType.GRINDER);
            AddBlockWords(Words("door", "hangar", "bay", "gate"), BlockType.DOOR);
            AddBlockWords(Words("display", "screen", "lcd"), BlockType.DISPLAY);
            AddBlockWords(Words("speaker", "alarm", "siren"), BlockType.SOUND);
            AddBlockWords(Words("camera"), BlockType.CAMERA);
            AddBlockWords(Words("sensor"), BlockType.SENSOR);
            AddBlockWords(Words("beacon"), BlockType.BEACON);
            AddBlockWords(Words("antenna"), BlockType.ANTENNA);
            AddBlockWords(Words("ship", "rover", "cockpit", "seat"), BlockType.COCKPIT);
            AddBlockWords(Words("drone", "remote", "robot"), BlockType.REMOTE);
            AddBlockWords(Words("thruster"), BlockType.THRUSTER);
            AddBlockWords(Words("airvent", "vent"), BlockType.AIRVENT);
            AddBlockWords(Words("gun", "rocket", "missile", "launcher"), BlockType.GUN);
            AddBlockWords(Words("turret"), BlockType.TURRET);
            AddBlockWords(Words("reactor"), BlockType.REACTOR);
            AddBlockWords(Words("generator"), BlockType.GENERATOR);
            AddBlockWords(Words("tank"), BlockType.TANK);
            AddBlockWords(Words("gear"), BlockType.GEAR);
            AddBlockWords(Words("battery"), Words("batteries"), BlockType.BATTERY);
            AddBlockWords(Words("chute", "parachutes"), BlockType.PARACHUTE);
            AddBlockWords(Words("wheel"), Words("wheels", "suspension"), BlockType.SUSPENSION);
            AddBlockWords(Words("detector"), BlockType.DETECTOR);
            AddBlockWords(Words("drill"), BlockType.DRILL);
            AddBlockWords(Words("engine"), BlockType.ENGINE);
            AddBlockWords(Words("sorter"), BlockType.SORTER);
            AddBlockWords(Words("gyro", "gyroscopes"), BlockType.GYROSCOPE);
            AddBlockWords(Words("gravitygenerator"), BlockType.GRAVITY_GENERATOR);
            AddBlockWords(Words("gravitysphere"), BlockType.GRAVITY_SPHERE);

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

        static String[] Words(params String[] words) {
            return words;
        }

        //Assume group words are just blockWords with "s" added to the end
        static void AddBlockWords(String[] blockWords, BlockType blockType) => AddBlockWords(blockWords, blockWords.Select(b => b + "s").ToArray(), blockType);

        static void AddBlockWords(String[] blockWords, String[] groupWords, BlockType blockType) {
            AddWords(blockWords, new BlockTypeCommandParameter(blockType));
            AddWords(groupWords, new BlockTypeCommandParameter(blockType), new GroupCommandParameter());
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
            double numericValue;

            if (token.isExplicitString) {
                commandParameters.Add(new ExplicitStringCommandParameter(token.original));
            } else if (token.isString) {
                List<Token> subTokens = ParseTokens(t);
                List<CommandParameter> subtokenParams = ParseCommandParameters(subTokens);
                commandParameters.Add(new StringCommandParameter(token.original, subtokenParams.ToArray()));
            } else if (propertyWords.ContainsKey(t)) {
                commandParameters.AddList(propertyWords[t]);
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
