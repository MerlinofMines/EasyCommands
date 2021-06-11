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
        //Internal (Don't touch!)
        Dictionary<String, List<CommandParameter>> propertyWords = new Dictionary<string, List<CommandParameter>>();

        string[] separateTokens = new[] { "(", ")", "[", "]", ",", "+", "*", "/", "!", "^", ".." };

        public void InitializeParsers() {
            //Ignored words that have no command parameters
            AddWords(Words("the", "than", "turned", "block", "to", "from", "then", "of", "either", "for", "in"), new IgnoreCommandParameter());

            //Selector Related Words
            AddWords(Words("blocks", "group"), new GroupCommandParameter());
            AddWords(Words("my", "self", "this"), new SelfCommandParameter());

            //Direction Words
            AddWords(Words("up", "upward", "upwards", "upper"), new DirectionCommandParameter(Direction.UP));
            AddWords(Words("down", "downward", "downwards", "lower"), new DirectionCommandParameter(Direction.DOWN));
            AddWords(Words("left", "lefthand"), new DirectionCommandParameter(Direction.LEFT));
            AddWords(Words("right", "righthand"), new DirectionCommandParameter(Direction.RIGHT));
            AddWords(Words("forward", "forwards", "front"), new DirectionCommandParameter(Direction.FORWARD));
            AddWords(Words("backward", "backwards", "back"), new DirectionCommandParameter(Direction.BACKWARD));
            AddWords(Words("clockwise", "clock"), new DirectionCommandParameter(Direction.CLOCKWISE));
            AddWords(Words("counter", "counterclock", "counterclockwise"), new DirectionCommandParameter(Direction.COUNTERCLOCKWISE));

            //Action Words
            AddWords(Words("move", "go", "tell", "turn", "rotate", "set"), new ActionCommandParameter());
            AddWords(Words("increase", "raise", "extend", "expand"), new ActionCommandParameter(), new DirectionCommandParameter(Direction.UP));
            AddWords(Words("add"), new ActionCommandParameter(), new RelativeCommandParameter(), new DirectionCommandParameter(Direction.UP));
            AddWords(Words("subtact"), new ActionCommandParameter(), new RelativeCommandParameter(), new DirectionCommandParameter(Direction.DOWN));
            AddWords(Words("decrease", "retract", "reduce"), new ActionCommandParameter(), new DirectionCommandParameter(Direction.DOWN));
            AddWords(Words("reverse"), new ReverseCommandParameter());
            AddWords(Words("by"), new RelativeCommandParameter());

            //Value Words
            AddWords(Words("on", "begin", "true", "start", "started", "resume", "resumed"), new BooleanCommandParameter(true));
            AddWords(Words("off", "terminate", "cancel", "end", "false", "stopped", "halt", "halted"), new BooleanCommandParameter(false));

            //Property Words
            AddWords(Words("height", "length"), new PropertyCommandParameter(Property.HEIGHT));
            AddWords(Words("angle"), new PropertyCommandParameter(Property.ANGLE));
            AddWords(Words("speed", "velocity", "rate", "pace"), new PropertyCommandParameter(Property.VELOCITY));
            AddWords(Words("connect", "join", "attach", "connected", "joined", "attached", "dock", "docked"), new PropertyCommandParameter(Property.CONNECTED));
            AddWords(Words("disconnect", "separate", "detach", "disconnected", "separated", "detached", "undock", "undocked"), new PropertyCommandParameter(Property.CONNECTED), new BooleanCommandParameter(false));
            AddWords(Words("lock", "locked", "freeze"), new PropertyCommandParameter(Property.LOCKED));
            AddWords(Words("unlock", "unlocked", "unfreeze"), new PropertyCommandParameter(Property.LOCKED), new BooleanCommandParameter(false));
            AddWords(Words("run", "running", "execute", "executing"), new PropertyCommandParameter(Property.RUN));
            AddWords(Words("done", "ready", "complete", "finished", "built", "finish"), new PropertyCommandParameter(Property.COMPLETE));
            AddWords(Words("progress", "completion"), new PropertyCommandParameter(Property.RATIO));
            AddWords(Words("open", "opened"), new PropertyCommandParameter(Property.OPEN), new BooleanCommandParameter(true));
            AddWords(Words("close", "closed", "shut"), new PropertyCommandParameter(Property.OPEN), new BooleanCommandParameter(false));
            AddWords(Words("fontsize", "size"), new PropertyCommandParameter(Property.FONT_SIZE));
            AddWords(Words("text", "message"), new PropertyCommandParameter(Property.TEXT));
            AddWords(Words("color"), new PropertyCommandParameter(Property.COLOR));
            AddWords(Words("power", "powered", "arm", "armed"), new PropertyCommandParameter(Property.POWER));
            AddWords(Words("music", "song"), new PropertyCommandParameter(Property.SONG));
            AddWords(Words("silent", "silence"), new PropertyCommandParameter(Property.SILENCE));
            AddWords(Words("volume", "level", "intensity", "output"), new PropertyCommandParameter(Property.VOLUME));
            AddWords(Words("range", "distance", "limit", "radius", "capacity", "delay"), new PropertyCommandParameter(Property.RANGE));
            AddWords(Words("blinkinterval", "blinkInterval", "interval"), new PropertyCommandParameter(Property.BLINK_INTERVAL));
            AddWords(Words("blinklength", "blinkLength"), new PropertyCommandParameter(Property.BLINK_LENGTH));
            AddWords(Words("blinkoffset", "blinkOffset"), new PropertyCommandParameter(Property.BLINK_OFFSET));
            AddWords(Words("falloff"), new PropertyCommandParameter(Property.FALLOFF));
            AddWords(Words("times", "iterations"), new IteratorCommandParameter());
            AddWords(Words("trigger", "triggered", "trip", "tripped", "deploy", "deployed", "shoot", "shooting", "shot", "detonate"), new PropertyCommandParameter(Property.TRIGGER));
            AddWords(Words("pressurize", "pressurized", "supply", "supplying", "generate", "discharge", "discharging"), new PropertyCommandParameter(Property.SUPPLY));
            AddWords(Words("stockpile", "depressurize", "depressurized", "gather", "intake", "recharge", "recharging", "consume", "consuming", "collect", "collecting"), new PropertyCommandParameter(Property.SUPPLY), new BooleanCommandParameter(false));
            AddWords(Words("assemble", "assembling", "produce", "producing", "create", "creating", "build", "building"), new ValuePropertyCommandParameter(ValueProperty.CREATE));
            AddWords(Words("disassemble", "disassembling", "destroy", "destroying", "recycle", "recycling"), new ValuePropertyCommandParameter(ValueProperty.DESTROY));
            AddWords(Words("ratio", "percentage", "percent"), new PropertyCommandParameter(Property.RATIO));
            AddWords(Words("input", "pilot", "user"), new PropertyCommandParameter(Property.MOVE_INPUT));
            AddWords(Words("roll", "rollInput"), new PropertyCommandParameter(Property.ROLL_INPUT));
            AddWords(Words("auto", "refill", "drain", "draining"), new PropertyCommandParameter(Property.AUTO));
            AddWords(Words("override", "overrides", "overridden"), new PropertyCommandParameter(Property.OVERRIDE));
            AddWords(Words("direction"), new PropertyCommandParameter(Property.DIRECTION));
            AddWords(Words("coordinates", "position", "location"), new PropertyCommandParameter(Property.POSITION));
            AddWords(Words("target", "destination", "waypoint"), new PropertyCommandParameter(Property.TARGET));
            AddWords(Words("targetvelocity"), new PropertyCommandParameter(Property.TARGET_VELOCITY));
            AddWords(Words("strength", "force", "gravity", "torque"), new PropertyCommandParameter(Property.STRENGTH));
            AddWords(Words("countdown"), new PropertyCommandParameter(Property.COUNTDOWN));

            //ValueProperty Words
            AddWords(Words("amount"), new ValuePropertyCommandParameter(ValueProperty.AMOUNT));

            //Special Command Words
            AddWords(Words("wait", "hold"), new WaitCommandParameter());
            AddWords(Words("call", "gosub"), new FunctionCommandParameter(Function.GOSUB));
            AddWords(Words("goto"), new FunctionCommandParameter(Function.GOTO));
            AddWords(Words("listen", "channel"), new ListenCommandParameter());
            AddWords(Words("send", "broadcast"), new SendCommandParameter());
            AddWords(Words("assign", "allocate", "designate"), new AssignmentCommandParameter(false));
            AddWords(Words("global"), new GlobalCommandParameter());
            AddWords(Words("bind", "tie", "link"), new AssignmentCommandParameter(true));
            AddWords(Words("print", "log", "echo", "write"), new PrintCommandParameter());
            AddWords(Words("queue", "schedule"), new QueueCommandParameter(false));
            AddWords(Words("async", "background", "parallel"), new QueueCommandParameter(true));
            AddWords(Words("transfer", "give"), new TransferCommandParameter(true));
            AddWords(Words("take"), new TransferCommandParameter(false));

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
            AddWords(Words("less", "<", "below"), new ComparisonCommandParameter(Comparison.LESS));
            AddWords(Words("<="), new ComparisonCommandParameter(Comparison.LESS_OR_EQUAL));
            AddWords(Words("is", "are", "equal", "equals", "=", "=="), new ComparisonCommandParameter(Comparison.EQUAL));
            AddWords(Words(">="), new ComparisonCommandParameter(Comparison.GREATER_OR_EQUAL));
            AddWords(Words("greater", ">", "above", "more"), new ComparisonCommandParameter(Comparison.GREATER));

            //Aggregation Words
            AddWords(Words("any"), new AggregationModeCommandParameter(AggregationMode.ANY));
            AddWords(Words("all", "every", "each"), new AggregationModeCommandParameter(AggregationMode.ALL));
            AddWords(Words("none"), new AggregationModeCommandParameter(AggregationMode.NONE));
            AddWords(Words("average", "avg"), new PropertyAggregationCommandParameter(PropertyAggregate.AVG));
            AddWords(Words("minimum", "min"), new PropertyAggregationCommandParameter(PropertyAggregate.MIN));
            AddWords(Words("maximum", "max"), new PropertyAggregationCommandParameter(PropertyAggregate.MAX));
            AddWords(Words("count", "number"), new PropertyAggregationCommandParameter(PropertyAggregate.COUNT));
            AddWords(Words("sum", "total"), new PropertyAggregationCommandParameter(PropertyAggregate.SUM));

            //Operations Words
            AddWords(Words("("), new OpenParenthesisCommandParameter());
            AddWords(Words(")"), new CloseParenthesisCommandParameter());
            AddWords(Words("and", "&", "&&", "but", "yet"), new AndCommandParameter());
            AddWords(Words("or", "|", "||"), new OrCommandParameter());
            AddWords(Words("not", "!", "isnt", "arent", "stop"), new NotCommandParameter());
            AddWords(Words("absolute", "abs"), new UniOperationCommandParameter(UniOperand.ABS));
            AddWords(Words("sqrt"), new UniOperationCommandParameter(UniOperand.SQRT));
            AddWords(Words("sin"), new UniOperationCommandParameter(UniOperand.SIN));
            AddWords(Words("cosine", "cos"), new UniOperationCommandParameter(UniOperand.COS));
            AddWords(Words("tangent", "tan"), new UniOperationCommandParameter(UniOperand.TAN));
            AddWords(Words("arcsin", "asin"), new UniOperationCommandParameter(UniOperand.ASIN));
            AddWords(Words("arcos", "acos"), new UniOperationCommandParameter(UniOperand.ACOS));
            AddWords(Words("arctan", "atan"), new UniOperationCommandParameter(UniOperand.ATAN));
            AddWords(Words("round", "rnd"), new UniOperationCommandParameter(UniOperand.ROUND));
            AddWords(Words("multiply", "*"), new BiOperandTier1Operand(BiOperand.MULTIPLY));
            AddWords(Words("divide", "/"), new BiOperandTier1Operand(BiOperand.DIVIDE));
            AddWords(Words("mod", "%"), new BiOperandTier1Operand(BiOperand.MOD));
            AddWords(Words("dot", "."), new BiOperandTier1Operand(BiOperand.DOT));
            AddWords(Words("pow", "exp", "^"), new BiOperandTier1Operand(BiOperand.EXPONENT));
            AddWords(Words("plus", "+"), new BiOperandTier2Operand(BiOperand.ADD));
            AddWords(Words("minus", "-"), new BiOperandTier2Operand(BiOperand.SUBTACT));
            AddWords(Words(".."), new BiOperandTier3Operand(BiOperand.RANGE));

            //List Words
            AddWords(Words("["), new OpenBracketCommandParameter());
            AddWords(Words("]"), new CloseBracketCommandParameter());
            AddWords(Words(","), new ListSeparatorCommandParameter());

            //Unit Words
            AddWords(Words("second", "seconds"), new UnitCommandParameter(Unit.SECONDS));
            AddWords(Words("tick", "ticks"), new UnitCommandParameter(Unit.TICKS));
            AddWords(Words("degree", "degrees"), new UnitCommandParameter(Unit.DEGREES));
            AddWords(Words("meter", "meters"), new UnitCommandParameter(Unit.METERS));
            AddWords(Words("rpm"), new UnitCommandParameter(Unit.RPM));

            //Control Types
            AddWords(Words("restart", "reset", "reboot"), new ControlCommandParameter(Control.RESTART));
            AddWords(Words("repeat", "loop", "rerun", "replay"), new ControlCommandParameter(Control.REPEAT));
            AddWords(Words("exit"), new ControlCommandParameter(Control.STOP));
            AddWords(Words("pause"), new ControlCommandParameter(Control.PAUSE));

            //Blocks
            AddBlockWords(Words("piston"), Block.PISTON);
            AddBlockWords(Words("light"), Block.LIGHT);
            AddBlockWords(Words("rotor"), Block.ROTOR);
            AddBlockWords(Words("hinge"), Block.HINGE);
            AddBlockWords(Words("program"), Block.PROGRAM);
            AddBlockWords(Words("timer"), Block.TIMER);
            AddBlockWords(Words("projector"), Block.PROJECTOR);
            AddBlockWords(Words("merge"), Words(), Block.MERGE);
            AddBlockWords(Words("connector"), Block.CONNECTOR);
            AddBlockWords(Words("welder"), Block.WELDER);
            AddBlockWords(Words("grinder"), Block.GRINDER);
            AddBlockWords(Words("door", "hangar", "bay", "gate"), Block.DOOR);
            AddBlockWords(Words("display", "screen", "lcd"), Block.DISPLAY);
            AddBlockWords(Words("speaker", "alarm", "siren"), Block.SOUND);
            AddBlockWords(Words("camera"), Block.CAMERA);
            AddBlockWords(Words("sensor"), Block.SENSOR);
            AddBlockWords(Words("beacon"), Block.BEACON);
            AddBlockWords(Words("antenna"), Block.ANTENNA);
            AddBlockWords(Words("ship", "rover", "cockpit", "seat"), Block.COCKPIT);
            AddBlockWords(Words("drone", "remote", "robot"), Block.REMOTE);
            AddBlockWords(Words("thruster"), Block.THRUSTER);
            AddBlockWords(Words("airvent", "vent"), Block.AIRVENT);
            AddBlockWords(Words("gun", "rocket", "missile", "launcher"), Block.GUN);
            AddBlockWords(Words("turret"), Block.TURRET);
            AddBlockWords(Words("reactor"), Block.REACTOR);
            AddBlockWords(Words("generator"), Block.GENERATOR);
            AddBlockWords(Words("tank"), Block.TANK);
            AddBlockWords(Words("gear"), Block.GEAR);
            AddBlockWords(Words("battery"), Words("batteries"), Block.BATTERY);
            AddBlockWords(Words("chute", "parachutes"), Block.PARACHUTE);
            AddBlockWords(Words("wheel"), Words("wheels", "suspension"), Block.SUSPENSION);
            AddBlockWords(Words("detector"), Block.DETECTOR);
            AddBlockWords(Words("drill"), Block.DRILL);
            AddBlockWords(Words("engine", "turbine"), Block.ENGINE);
            AddBlockWords(Words("sorter"), Block.SORTER);
            AddBlockWords(Words("gyro", "gyroscopes"), Block.GYROSCOPE);
            AddBlockWords(Words("gravitygenerator"), Block.GRAVITY_GENERATOR);
            AddBlockWords(Words("gravitysphere"), Block.GRAVITY_SPHERE);
            AddBlockWords(Words("cargo", "container", "inventory"), Words("containers", "inventories"), Block.CARGO);
            AddBlockWords(Words("warhead", "bomb"), Block.WARHEAD);
            AddBlockWords(Words("assembler"), Block.ASSEMBLER);
            AddBlockWords(Words("collector"), Block.COLLECTOR);
            AddBlockWords(Words("ejector"), Block.EJECTOR);
            AddBlockWords(Words("decoy"), Block.DECOY);
            AddBlockWords(Words("jumpdrive"), Block.JUMPDRIVE);
            AddBlockWords(Words("laser", "laserAntenna"), Block.LASER_ANTENNA);

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
        }

        Dictionary<Type, Func<CommandParameter, object>> commandParameterStrings = new Dictionary<Type, Func<CommandParameter, object>>();

        void RegisterToString<T>(Func<T, object> toString) where T : CommandParameter {
            commandParameterStrings[typeof(T)] = (p) => toString((T)p);
        }

        string CommandParameterToString(CommandParameter parameter) {
            Func<CommandParameter, object> func;
            if (!commandParameterStrings.TryGetValue(parameter.GetType(), out func)) {
                func = (p) => p.Token;
            }
            return func(parameter).ToString();
        }

        String[] Words(params String[] words) {
            return words;
        }

        //Assume group words are just blockWords with "s" added to the end
        void AddBlockWords(String[] blockWords, Block blockType) => AddBlockWords(blockWords, blockWords.Select(b => b + "s").ToArray(), blockType);

        void AddBlockWords(String[] blockWords, String[] groupWords, Block blockType) {
            AddWords(blockWords, new BlockTypeCommandParameter(blockType));
            AddWords(groupWords, new BlockTypeCommandParameter(blockType), new GroupCommandParameter());
        }

        void AddWords(String[] words, params CommandParameter[] commands) {
            foreach (String word in words) propertyWords.Add(word, commands.ToList());
        }

        List<CommandParameter> ParseCommandParameters(List<Token> tokens) {
            Trace("Command: " + String.Join(" | ", tokens));
            var parameters = new List<CommandParameter>();

            foreach (var token in tokens) {
                var newParameters = ParseCommandParameters(token);
                newParameters.ForEach(p => p.Token = token.original);
                parameters.AddRange(newParameters);
            }

            return parameters;
        }

        List<CommandParameter> ParseCommandParameters(Token token) {
            List<CommandParameter> commandParameters = new List<CommandParameter>();
            String t = token.token;
            double numericValue;

            if (token.isExplicitString) {
                commandParameters.Add(new ExplicitStringCommandParameter(token.original));
            } else if (token.isString) {
                List<Token> subTokens = ParseTokens(t);
                List<CommandParameter> subtokenParams = ParseCommandParameters(subTokens);
                commandParameters.Add(new StringCommandParameter(token.original, false, subtokenParams.ToArray()));
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
            } else if (t.StartsWith("$")) { //Variable References used as Selectors
                commandParameters.Add(new VariableSelectorCommandParameter(new InMemoryVariable(token.original.Substring(1, token.original.Length - 1))));
            } else { //If nothing else matches, must be a string
                commandParameters.Add(new StringCommandParameter(token.original, true));
            }

            return commandParameters;
        }

        //Taken shamelessly from https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        public List<Token> ParseTokens(String commandString) {
            List<Token> singleQuoteTokens = commandString.Trim().Split('\'')
            .SelectMany((element, index) => index % 2 == 0  // If even index
                ? ParseDoubleQuotes(element)  // Split the item
                : new Token[] { new Token(element, true, true) })  // Keep the entire item
            .ToList();

            return singleQuoteTokens;
        }

        Token[] ParseDoubleQuotes(String commandString) {
            return commandString.Trim().Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(ParseSeparateTokens)
                    .Select(t => new Token(t, false, false))  // Split the item
                    : new Token[] { new Token(element, true, false) })  // Keep the entire item
                .SelectMany(element => element)
                .ToArray();
        }

        String[] ParseSeparateTokens(String command) {
            var newCommand = command;
            separateTokens.ForEach(s => newCommand = newCommand.Replace(s, " " + s + " "));
            List<char> commandArray = newCommand.ToCharArray().ToList();

            //- has to be handled specially, as " -3" should be left alone but "n-1" should be split as "n - 1"
            // This is done by separating out any "-" that is not at the beginning of a string
            for(int i = 1; i < commandArray.Count() - 1; i++) {
                if(commandArray[i] == '-' && commandArray[i-1] != ' ') {
                    if (commandArray[i + 1] != ' ') commandArray.Insert(i + 1, ' ');
                    commandArray.Insert(i, ' ');
                }
            }

            return new string(commandArray.ToArray()).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
