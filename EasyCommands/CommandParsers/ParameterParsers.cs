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
        Dictionary<String, List<ICommandParameter>> propertyWords = NewDictionary<string, List<ICommandParameter>>();

        string[] firstPassTokens = new[] { "(", ")", "[", "]", ",", "*", "/", "!", "^", "..", "%", ">=", "<=", "==", "&&", "||", "@", "$", "->", "++", "+=", "--", "-=", "::"};
        string[] secondPassTokens = new[] { "<", ">", "=", "&", "|", "-", "+", "?", ":" };
        string[] thirdPassTokens = new[] { "." };

        Dictionary<UniOperand, String> uniOperandToString = NewDictionary<UniOperand, String>(
            KeyValuePair(UniOperand.CAST, "cast"),
            KeyValuePair(UniOperand.ROUND, "round"),
            KeyValuePair(UniOperand.REVERSE, "negate")
        );
        Dictionary<BiOperand, String> biOperandToString = NewDictionary<BiOperand, String>(
            KeyValuePair(BiOperand.CAST, "cast"),
            KeyValuePair(BiOperand.ROUND, "round"),
            KeyValuePair(BiOperand.COMPARE, "compare")
        );
        Dictionary<Return, String> returnToString = NewDictionary(
            KeyValuePair(Return.BOOLEAN, "boolean"),
            KeyValuePair(Return.NUMERIC, "number"),
            KeyValuePair(Return.STRING, "string"),
            KeyValuePair(Return.VECTOR, "vector"),
            KeyValuePair(Return.COLOR, "color"),
            KeyValuePair(Return.LIST, "list")
        );

        static Dictionary<string, Converter> castMap = NewDictionary(
            CastFunction("bool", p => CastBoolean(p)),
            CastFunction("boolean", p => CastBoolean(p)),
            CastFunction("string", CastString),
            CastFunction("number", p => CastNumber(p)),
            CastFunction("vector", p => CastVector(p)),
            CastFunction("color", p => CastColor(p)),
            CastFunction("list", CastList)
        );

        static Dictionary<string, Color> colors = NewDictionary(
            KeyValuePair("red", Color.Red),
            KeyValuePair("blue", Color.Blue),
            KeyValuePair("green", Color.Green),
            KeyValuePair("orange", Color.Orange),
            KeyValuePair("yellow", Color.Yellow),
            KeyValuePair("white", Color.White),
            KeyValuePair("black", Color.Black)
        );

        public void InitializeParsers() {
            //Ignored words that have no command parameters
            AddWords(Words("the", "than", "turned", "block", "panel", "chamber", "drive", "to", "from", "then", "of", "either", "for", "in", "do", "does", "second", "seconds", "be", "being", "digits", "digit"), new IgnoreCommandParameter());

            //Selector Related Words
            AddWords(Words("blocks", "group", "panels", "chambers", "drives"), new GroupCommandParameter());
            AddWords(Words("my", "self", "this"), new SelfCommandParameter());
            AddWords(Words("$"), new VariableSelectorCommandParameter());

            //Direction Words
            AddDirectionWords(Words("up", "upward", "upwards", "upper"), Direction.UP);
            AddDirectionWords(Words("down", "downward", "downwards", "lower"), Direction.DOWN);
            AddDirectionWords(Words("left", "lefthand"), Direction.LEFT);
            AddDirectionWords(Words("right", "righthand"), Direction.RIGHT);
            AddDirectionWords(Words("forward", "forwards", "front"), Direction.FORWARD);
            AddDirectionWords(Words("backward", "backwards", "back"), Direction.BACKWARD);
            AddDirectionWords(Words("clockwise", "clock"), Direction.CLOCKWISE);
            AddDirectionWords(Words("counterclockwise", "counter", "counterclock"), Direction.COUNTERCLOCKWISE);

            //Action Words
            AddWords(Words("bind", "tie", "link"), new AssignmentCommandParameter(true));
            AddWords(Words("move", "go", "tell", "turn", "rotate", "set", "assign", "allocate", "designate", "apply"), new AssignmentCommandParameter());
            AddWords(Words("reverse", "reversed"), new ReverseCommandParameter());
            AddWords(Words("raise", "extend"), new AssignmentCommandParameter(), new DirectionCommandParameter(Direction.UP));
            AddWords(Words("retract"), new AssignmentCommandParameter(), new DirectionCommandParameter(Direction.DOWN));
            AddWords(Words("increase", "increment"), new IncreaseCommandParameter());
            AddWords(Words("decrease", "decrement", "reduce"), new IncreaseCommandParameter(false));
            AddWords(Words("++", "+="), new IncrementCommandParameter());
            AddWords(Words("--", "-="), new IncrementCommandParameter(false));
            AddWords(Words("global"), new GlobalCommandParameter());
            AddWords(Words("by"), new RelativeCommandParameter());

            //Value Words
            AddWords(Words("on", "begin", "true", "start", "started", "resume", "resumed"), new BooleanCommandParameter(true));
            AddWords(Words("off", "terminate", "cancel", "end", "false", "stopped", "halt", "halted"), new BooleanCommandParameter(false));

            //Property Words
            AddPropertyWords(AllWords(PluralWords("height", "length", "level", "size", "period", "scale")), Property.LEVEL);
            AddPropertyWords(PluralWords("angle"), Property.ANGLE);
            AddPropertyWords(AllWords(PluralWords("speed", "rate", "pace"), Words("velocity", "velocities")), Property.VELOCITY);
            AddPropertyWords(Words("connect", "attach", "connected", "attached", "dock", "docked", "docking"), Property.CONNECTED);
            AddPropertyWords(Words("disconnect", "detach", "disconnected", "detached", "undock", "undocked"), Property.CONNECTED, false);
            AddPropertyWords(Words("lock", "locked", "freeze", "frozen", "brake", "braking", "handbrake", "permanent"), Property.LOCKED);
            AddPropertyWords(Words("unlock", "unlocked", "unfreeze"), Property.LOCKED, false);
            AddPropertyWords(Words("run", "running", "execute", "executing", "script"), Property.RUN);
            AddPropertyWords(Words("use", "used", "occupy", "occupied", "control", "controlled"), Property.USE);
            AddPropertyWords(Words("unused", "unoccupied", "vacant", "available"), Property.USE, false);
            AddPropertyWords(Words("done", "ready", "complete", "finished", "built", "finish", "pressurized", "depressurized"), Property.COMPLETE);
            AddPropertyWords(Words("clear", "wipe", "erase"), Property.COMPLETE, false);
            AddPropertyWords(Words("open", "opened"), Property.OPEN);
            AddPropertyWords(Words("close", "closed", "shut"), Property.OPEN, false);
            AddPropertyWords(PluralWords("font"), Property.FONT);
            AddPropertyWords(PluralWords("text", "message", "argument"), Property.TEXT);
            AddPropertyWords(AllWords(Words("colors"), PluralWords("foreground")), Property.COLOR);
            AddAmbiguousWords(Words("color"), new PropertyCommandParameter(Property.COLOR));
            AddPropertyWords(PluralWords("background"), Property.BACKGROUND);
            AddPropertyWords(Words("power", "powered"), Property.POWER);
            AddPropertyWords(Words("enable", "enabled", "arm", "armed"), Property.ENABLE);
            AddPropertyWords(Words("disable", "disabled", "disarm", "disarmed"), Property.ENABLE, false);
            AddPropertyWords(Words("music", "sound", "song", "track", "image", "play", "playing", "unsilence"), Property.MEDIA);
            AddPropertyWords(Words("silence", "silent", "quiet"), Property.MEDIA, false);
            AddPropertyWords(Words("sounds", "songs", "images", "tracks"), Property.MEDIA_LIST);
            AddPropertyWords(AllWords(PluralWords("volume", "output"), Words("intensity", "intensities")), Property.VOLUME);
            AddPropertyWords(AllWords(PluralWords("range", "distance", "limit", "delay"), Words("radius", "radii", "capacity", "capacities")), Property.RANGE);
            AddPropertyWords(PluralWords("interval"), Property.INTERVAL);
            AddPropertyWords(PluralWords("offset", "padding"), Property.OFFSET);
            AddPropertyWords(PluralWords("falloff"), Property.FALLOFF);
            AddPropertyWords(Words("trigger", "triggered", "detect", "detected", "trip", "tripped", "deploy", "deployed", "shoot", "shooting", "shot", "detonate", "fire", "firing"), Property.TRIGGER);
            AddPropertyWords(Words("pressure", "pressurize", "pressurizing", "supply", "supplying", "generate", "generating", "discharge", "discharging", "broadcast", "broadcasting", "assemble", "assembling"), Property.SUPPLY);
            AddPropertyWords(Words("stockpile", "stockpiling", "depressurize", "depressurizing", "gather", "gathering", "intake", "recharge", "recharging", "consume", "consuming", "collect", "collecting", "disassemble", "disassembling"), Property.SUPPLY, false);
            AddPropertyWords(AllWords(PluralWords("ratio", "percentage", "percent", "completion"), Words("progress", "progresses")), Property.RATIO);
            AddPropertyWords(PluralWords("input", "pilot", "user"), Property.INPUT);
            AddPropertyWords(PluralWords("roll", "rollInput", "rotation"), Property.ROLL_INPUT);
            AddPropertyWords(Words("auto", "autopilot", "refill", "drain", "draining", "cooperate", "cooperating"), Property.AUTO);
            AddPropertyWords(AllWords(PluralWords("override", "dampener"), Words("overridden")), Property.OVERRIDE);
            AddPropertyWords(PluralWords("direction"), Property.DIRECTION);
            AddPropertyWords(PluralWords("position", "location", "alignment"), Property.POSITION);
            AddPropertyWords(Words("target", "destination", "waypoint", "coords", "coordinates"), Property.TARGET);
            AddPropertyWords(Words("waypoints", "destinations"), Property.WAYPOINTS);
            AddPropertyWords(Words("targetvelocity"), Property.TARGET_VELOCITY);
            AddPropertyWords(AllWords(PluralWords("strength", "force", "torque"), Words("gravity", "gravities")), Property.STRENGTH);
            AddPropertyWords(Words("naturalgravity", "naturalgravities", "planetgravity", "planetgravities"), Property.NATURAL_GRAVITY);
            AddPropertyWords(Words("artificialgravity", "artificialgravities"), Property.ARTIFICIAL_GRAVITY);
            AddPropertyWords(PluralWords("countdown"), Property.COUNTDOWN);
            AddPropertyWords(Words("name", "label"), Property.NAME);
            AddPropertyWords(Words("names", "labels"), Property.NAMES);
            AddPropertyWords(Words("show", "showing"), Property.SHOW);
            AddPropertyWords(Words("hide", "hiding"), Property.SHOW, false);
            AddPropertyWords(Words("properties", "attributes"), Property.PROPERTIES);
            AddPropertyWords(Words("actions"), Property.ACTIONS);
            AddPropertyWords(Words("types", "blueprints"), Property.TYPES);
            AddPropertyWords(PluralWords("altitude", "elevation"), Property.ALTITUDE);
            AddPropertyWords(Words("weight", "mass"), Property.WEIGHT);
            AddPropertyWords(Words("data", "customdata"), Property.DATA);
            AddPropertyWords(Words("info", "details", "detailedinfo"), Property.INFO);

            //ValueProperty Words
            AddWords(PluralWords("amount"), new ValuePropertyCommandParameter(ValueProperty.AMOUNT));
            AddWords(Words("property", "attribute"), new ValuePropertyCommandParameter(ValueProperty.PROPERTY));
            AddWords(Words("action"), new ValuePropertyCommandParameter(ValueProperty.ACTION));
            AddWords(Words("produce", "producing", "create", "creating", "build", "building", "make", "making"), new ValuePropertyCommandParameter(ValueProperty.CREATE));
            AddWords(Words("destroy", "destroying", "recycle", "recycling"), new ValuePropertyCommandParameter(ValueProperty.DESTROY));

            //Special Command Words
            AddWords(Words("times", "iterations"), new RepeatCommandParameter());
            AddWords(Words("wait", "hold"), new WaitCommandParameter());
            AddWords(Words("call", "gosub"), new FunctionCommandParameter(false));
            AddWords(Words("goto"), new FunctionCommandParameter(true));
            AddWords(Words("listen", "channel", "register", "subscribe"), new ListenCommandParameter(true));
            AddWords(Words("forget", "dismiss", "ignore", "deregister", "unsubscribe"), new ListenCommandParameter(false));
            AddWords(Words("send"), new SendCommandParameter());
            AddWords(Words("print", "log", "echo", "write"), new PrintCommandParameter());
            AddWords(Words("queue", "schedule"), new QueueCommandParameter(false));
            AddWords(Words("async", "parallel"), new QueueCommandParameter(true));
            AddWords(Words("transfer", "give"), new TransferCommandParameter(true));
            AddWords(Words("take"), new TransferCommandParameter(false));
            AddWords(Words("->"), new KeyedVariableCommandParameter());
            AddWords(Words("?"), new TernaryConditionIndicatorParameter());
            AddWords(Words("::"), new TernaryConditionSeparatorParameter());
            AddWords(Words(":"), new ColonSeparatorParameter());
            AddWords(Words("each", "every"), new IteratorCommandParameter());

            //Conditional Words
            AddWords(Words("if"), new IfCommandParameter(false, false, false));
            AddWords(Words("unless"), new IfCommandParameter(true, false, false));
            AddWords(Words("while"), new IfCommandParameter(false, true, false));
            AddWords(Words("until"), new IfCommandParameter(true, true, false));
            AddWords(Words("when"), new IfCommandParameter(true, true, true));
            AddWords(Words("else", "otherwise"), new ElseCommandParameter());
            AddWords(Words("that", "which", "whose"), new ThatCommandParameter());

            //Comparison Words
            AddWords(Words("less", "<", "below"), new ComparisonCommandParameter((a, b) => a.Compare(b) < 0));
            AddWords(Words("<="), new ComparisonCommandParameter((a, b) => a.Compare(b) <= 0));
            AddWords(Words("is", "are", "equal", "equals", "=", "=="), new ComparisonCommandParameter((a, b) => a.Compare(b) == 0));
            AddWords(Words(">="), new ComparisonCommandParameter((a, b) => a.Compare(b) >= 0));
            AddWords(Words("greater", ">", "above", "more"), new ComparisonCommandParameter((a, b) => a.Compare(b) > 0));
            AddWords(Words("contain", "contains"), new ComparisonCommandParameter((a, b) => CastBoolean(PROGRAM.PerformOperation(BiOperand.CONTAINS, a, b))));

            //Aggregation Words
            AddWords(Words("any"), new AggregationModeCommandParameter(AggregationMode.ANY));
            AddWords(Words("all"), new AggregationModeCommandParameter(AggregationMode.ALL));
            AddWords(Words("none"), new AggregationModeCommandParameter(AggregationMode.NONE));
            AddWords(Words("average", "avg"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => SumAggregator(blocks, primitiveSupplier).Divide(ResolvePrimitive(Math.Max(1, blocks.Count())))));
            AddWords(Words("minimum", "min"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => blocks.Select(b => primitiveSupplier(b)).DefaultIfEmpty(ResolvePrimitive(0)).Aggregate((a, b) => (a.Compare(b) < 0 ? a : b))));
            AddWords(Words("maximum", "max"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => blocks.Select(b => primitiveSupplier(b)).DefaultIfEmpty(ResolvePrimitive(0)).Aggregate((a, b) => (a.Compare(b) > 0 ? a : b))));
            AddWords(Words("count"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => ResolvePrimitive(blocks.Count())));
            AddAmbiguousWords(Words("number"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => ResolvePrimitive(blocks.Count())));
            AddWords(Words("sum", "total"), new PropertyAggregationCommandParameter(SumAggregator));
            AddWords(Words("collection"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => ResolvePrimitive(NewKeyedList(blocks.Select(b => new StaticVariable(primitiveSupplier(b)))))));
            AddAmbiguousWords(Words("list"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => ResolvePrimitive(NewKeyedList(blocks.Select(b => new StaticVariable(primitiveSupplier(b)))))));

            //Operations Words
            AddWords(Words("("), new OpenParenthesisCommandParameter());
            AddWords(Words(")"), new CloseParenthesisCommandParameter());
            AddWords(Words("and", "&", "&&", "but", "yet"), new AndCommandParameter());
            AddWords(Words("or", "|", "||"), new OrCommandParameter());
            AddWords(Words("not", "!", "stop"), new NotCommandParameter());
            AddWords(Words("@"), new IndexCommandParameter());

            AddRightUniOperationWords(Words("absolute", "abs"), UniOperand.ABS);
            AddRightUniOperationWords(Words("sqrt"), UniOperand.SQRT);
            AddRightUniOperationWords(Words("sin"), UniOperand.SIN);
            AddRightUniOperationWords(Words("cosine", "cos"), UniOperand.COS);
            AddRightUniOperationWords(Words("tangent", "tan"), UniOperand.TAN);
            AddRightUniOperationWords(Words("arcsin", "asin"), UniOperand.ASIN);
            AddRightUniOperationWords(Words("arccos", "acos"), UniOperand.ACOS);
            AddRightUniOperationWords(Words("arctan", "atan"), UniOperand.ATAN);
            AddRightUniOperationWords(Words("sort", "sorted"), UniOperand.SORT);
            AddRightUniOperationWords(Words("ln"), UniOperand.LN);
            AddRightUniOperationWords(Words("rand", "random", "randomize"), UniOperand.RANDOM);
            AddRightUniOperationWords(Words("shuffle", "shuffled"), UniOperand.SHUFFLE);
            AddRightUniOperationWords(Words("sign", "quantize"), UniOperand.SIGN);

            AddLeftUniOperationWords(Words("tick", "ticks"), UniOperand.TICKS);
            AddLeftUniOperationWords(Words("keys", "indexes"), UniOperand.KEYS);
            AddLeftUniOperationWords(Words("values"), UniOperand.VALUES);
            AddLeftUniOperationWords(Words("type"), UniOperand.TYPE);

            //Tier 0 Operations
            AddWords(Words("dot", "."), new BiOperandCommandParameter(BiOperand.DOT, 0));

            //Tier 1 Operations
            AddBiOperationWords(Words("pow", "^", "xor"), BiOperand.EXPONENT, 1);

            //Tier 2 Operations
            AddBiOperationWords(Words("multiply", "*"), BiOperand.MULTIPLY, 2);
            AddBiOperationWords(Words("divide", "/"), BiOperand.DIVIDE, 2);
            AddBiOperationWords(Words("mod", "%"), BiOperand.MOD, 2);
            AddBiOperationWords(Words("split", "separate", "separated"), BiOperand.SPLIT, 2);
            AddBiOperationWords(Words("join", "joined"), BiOperand.JOIN, 2);

            //Tier 3 Operations
            AddBiOperationWords(Words("plus", "+"), BiOperand.ADD, 3);
            AddBiOperationWords(Words("minus"), BiOperand.SUBTRACT, 3);

            //Tier 4 Operations
            AddBiOperationWords(Words(".."), BiOperand.RANGE, 4);

            AddWords(Words("-"), new MinusCommandParameter());
            AddWords(Words("round", "rnd", "rounded"), new RoundCommandParameter());
            AddWords(Words("as", "cast", "resolve", "resolved"), new CastCommandParameter());

            //List Words
            AddWords(Words("["), new OpenBracketCommandParameter());
            AddWords(Words("]"), new CloseBracketCommandParameter());
            AddWords(Words(","), new ListSeparatorCommandParameter());

            //Control Types
            AddControlWords(Words("restart", "reset", "reboot"), thread => {
                PROGRAM.ClearAllState();
                throw new InterruptException(ProgramState.RUNNING);
            });
            AddControlWords(Words("repeat", "loop", "rerun", "replay"), thread => {
                thread.Command = thread.Command.Clone();
                return false;
            });
            AddControlWords(Words("exit"), thread => {
                PROGRAM.ClearAllState();
                throw new InterruptException(ProgramState.STOPPED);
            });
            AddControlWords(Words("pause"), thread => {
                if (PROGRAM.state != ProgramState.PAUSED) throw new InterruptException(ProgramState.PAUSED);
                return true;
             });
            AddControlWords(Words("break"), thread => {
                GetInterrupableCommand("break").Break();
                return false;
            });
            AddControlWords(Words("continue"), thread => {
                GetInterrupableCommand("continue").Continue();
                return false;
            });
            AddControlWords(Words("return"), thread => {
                FunctionCommand currentFunction = thread.GetCurrentCommand<FunctionCommand>(command => true);
                if (currentFunction == null) throw new Exception("Invalid use of return command");
                currentFunction.function = new NullCommand();
                return false;
            });

            //Blocks
            AddBlockWords(Words("piston"), Block.PISTON);
            AddBlockWords(Words("light", "spotlight"), Block.LIGHT);
            AddBlockWords(Words("rotor"), Block.ROTOR);
            AddBlockWords(Words("hinge"), Block.HINGE);
            AddBlockWords(Words("program", "programmable"), Block.PROGRAM);
            AddBlockWords(Words("timer"), Block.TIMER);
            AddBlockWords(Words("projector"), Block.PROJECTOR);
            AddBlockWords(Words("merge"), Words(), Block.MERGE);
            AddBlockWords(Words("connector"), Block.CONNECTOR);
            AddBlockWords(Words("welder"), Block.WELDER);
            AddBlockWords(Words("grinder"), Block.GRINDER);
            AddBlockWords(Words("door", "hangar", "bay", "gate"), Block.DOOR);
            AddBlockWords(PluralWords("display", "screen", "lcd"), Words(), Block.DISPLAY);
            AddBlockWords(Words("speaker", "alarm", "siren"), Block.SOUND);
            AddBlockWords(Words("camera"), Block.CAMERA);
            AddBlockWords(Words("sensor"), Block.SENSOR);
            AddBlockWords(Words("beacon"), Block.BEACON);
            AddBlockWords(Words("antenna"), Block.ANTENNA);
            AddBlockWords(Words("ship", "rover", "cockpit", "seat", "station"), Block.COCKPIT);
            AddBlockWords(Words("cryo"), Block.CRYO_CHAMBER);
            AddBlockWords(Words("drone", "remote", "robot"), Block.REMOTE);
            AddBlockWords(Words("thruster"), Block.THRUSTER);
            AddBlockWords(Words("airvent", "vent"), Block.AIRVENT);
            AddBlockWords(Words("gun", "railgun", "cannon", "autocannon", "rocket", "missile", "launcher"), Block.GUN);
            AddBlockWords(Words("turret"), Block.TURRET);
            AddBlockWords(Words("generator"), Block.GENERATOR);
            AddBlockWords(Words("tank"), Block.TANK);
            AddBlockWords(Words("magnet", "gear"), Block.MAGNET);
            AddBlockWords(Words("battery"), Words("batteries"), Block.BATTERY);
            AddBlockWords(Words("chute", "parachute"), Block.PARACHUTE);
            AddBlockWords(Words("wheel"), Words("wheels", "suspension"), Block.SUSPENSION);
            AddBlockWords(Words("detector"), Block.DETECTOR);
            AddBlockWords(Words("drill"), Block.DRILL);
            AddBlockWords(Words("engine"), Block.ENGINE);
            AddBlockWords(Words("turbine"), Block.TURBINE);
            AddBlockWords(Words("reactor"), Block.REACTOR);
            AddBlockWords(Words("solar"), Block.SOLAR_PANEL);
            AddBlockWords(Words("sorter"), Block.SORTER);
            AddBlockWords(Words("gyro", "gyroscope"), Block.GYROSCOPE);
            AddBlockWords(Words("gravitygenerator"), Block.GRAVITY_GENERATOR);
            AddBlockWords(Words("gravitysphere"), Block.GRAVITY_SPHERE);
            AddBlockWords(Words("cargo", "container", "inventory", "inventories"), Words("cargos", "containers"), Block.CARGO);
            AddBlockWords(Words("warhead", "bomb"), Block.WARHEAD);
            AddBlockWords(Words("assembler"), Block.ASSEMBLER);
            AddBlockWords(Words("collector"), Block.COLLECTOR);
            AddBlockWords(Words("ejector"), Block.EJECTOR);
            AddBlockWords(Words("decoy"), Block.DECOY);
            AddBlockWords(Words("jump", "jumpdrive"), Block.JUMPDRIVE);
            AddBlockWords(Words("laser", "laserantenna"), Block.LASER_ANTENNA);
            AddBlockWords(Words("terminal"), Block.TERMINAL);
            AddBlockWords(Words("refinery"), Words("refineries"), Block.REFINERY);
            AddBlockWords(Words("heatvent"), Block.HEAT_VENT);
        }

        String[] Words(params String[] words) => words;

        String[] AllWords(params String[][] words) => words.Aggregate((a, b) => a.Concat(b).ToArray());

        String[] PluralWords(params String[] words) => words.Concat(words.Select(w => w + "s")).ToArray();

        void AddControlWords(String[] words, ControlFunction function) {
            AddWords(words, new CommandReferenceParameter(new ControlCommand { controlFunction = function }));
        }

        void AddPropertyWords(String[] words, Property property, bool nonNegative = true) {
            if (!nonNegative) AddWords(words, new PropertyCommandParameter(property), new BooleanCommandParameter(false));
            else AddWords(words, new PropertyCommandParameter(property));
        }

        void AddDirectionWords(String[] words, Direction direction) {
            AddWords(words, new DirectionCommandParameter(direction));
        }

        void AddRightUniOperationWords(String[] words, UniOperand operand) {
            AddWords(words, new UniOperationCommandParameter(operand));
            uniOperandToString[operand] = words[0];
        }

        void AddLeftUniOperationWords(String[] words, UniOperand operand) {
            AddWords(words, new LeftUniOperationCommandParameter(operand));
            uniOperandToString[operand] = words[0];
        }

        void AddBiOperationWords(String[] words, BiOperand operand, int tier) {
            AddWords(words, new BiOperandCommandParameter(operand, tier));
            biOperandToString[operand] = words[0];
        }

        //Assume group words are just blockWords with "s" added to the end
        void AddBlockWords(String[] blockWords, Block blockType) => AddBlockWords(blockWords, blockWords.Select(b => b + "s").ToArray(), blockType);

        void AddBlockWords(String[] blockWords, String[] groupWords, Block blockType) {
            AddWords(blockWords, new BlockTypeCommandParameter(blockType));
            AddWords(groupWords, new BlockTypeCommandParameter(blockType), new GroupCommandParameter());
        }

        void AddAmbiguousWords(String[] words, params ICommandParameter[] commandParameters) {
            foreach (String word in words)
                AddWords(Words(word), new AmbiguousStringCommandParameter(word, false, new AmbiguousCommandParameter(commandParameters)));
        }

        void AddWords(String[] words, params ICommandParameter[] commandParameters) {
            foreach (String word in words) propertyWords.Add(word, commandParameters.ToList());
        }

        List<ICommandParameter> ParseCommandParameters(List<Token> tokens) => tokens.SelectMany(t => ParseCommandParameters(t)).ToList();

        List<ICommandParameter> ParseCommandParameters(Token token) {
            var commandParameters = NewList<ICommandParameter>();
            String t = token.token;
            if (token.isExplicitString) {
                commandParameters.Add(new VariableCommandParameter(GetStaticVariable(token.original)));
            } else if (token.isString) {
                List<Token> subTokens = Tokenize(t);
                List<ICommandParameter> subtokenParams = ParseCommandParameters(subTokens);
                commandParameters.Add(new AmbiguousStringCommandParameter(token.original, false, subtokenParams.ToArray()));
            } else if (propertyWords.ContainsKey(t)) {
                commandParameters.AddList(propertyWords[t]);
            } else { //If no property matches, must be a string
                commandParameters.Add(new AmbiguousStringCommandParameter(token.original, true));
            }

            commandParameters[0].Token = token.original;
            return commandParameters;
        }

        delegate IEnumerable<String> Pass(String s);
        public List<Token> Tokenize(String commandString) {
            Pass thirdPass = v => SeperatorPass(v, thirdPassTokens);
            Pass secondPass = v => SeperatorPass(v, secondPassTokens, w => PrimitivePass(w, thirdPass));
            Pass firstPass = v => SeperatorPass(v, firstPassTokens, w => PrimitivePass(w, secondPass));

            return (String.IsNullOrWhiteSpace(commandString) || commandString.Trim().StartsWith("#"))
            ? NewList<Token>()
            : TokenizeEnclosed(commandString, "`\'\"",
                u => u.Replace(" : ", " :: ")
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(v => firstPass(v))
                    .Select(v => new Token(v, false, false))
                    .ToArray())
            .ToList();
        }

        Token[] TokenizeEnclosed(String token, string characters, Func<String, Token[]> parseSubTokens) =>
            characters.Length == 0 ? parseSubTokens(token) :
                token.Trim().Split(characters[0])
                .SelectMany((element, index) => index % 2 == 0  // If even index
                    ? TokenizeEnclosed(element, characters.Remove(0,1), parseSubTokens)  // Split the item
                    : new Token[] { new Token(element, true, characters.Length > 1) })  // Keep the entire item
                .ToArray();

        IEnumerable<String> PrimitivePass(String token, Pass nextPass = null) {
            Primitive ignored;
            return ParsePrimitive(token, out ignored) || nextPass == null ? new[] { token } : nextPass(token);
        }

        IEnumerable<String> SeperatorPass(String command, string[] separators, Pass nextPass = null) {
            var newCommand = command;
            foreach (var s in separators) newCommand = newCommand.Replace(s, " " + s + " ");
            return newCommand
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany(token => separators.Contains(token) || nextPass == null ? new[] { token } : nextPass(token));
        }

        public static bool ParsePrimitive(String token, out Primitive primitive) {
            primitive = null;
            bool boolean;
            var vector = GetVector(token);
            Double numeric;
            var color = GetColor(token);
            if (bool.TryParse(token, out boolean)) primitive = ResolvePrimitive(boolean);
            if (Double.TryParse(token, out numeric)) primitive = ResolvePrimitive(numeric);
            if (vector.HasValue) primitive = ResolvePrimitive(vector.Value);
            if (color.HasValue) primitive = ResolvePrimitive(color.Value);
            return primitive != null;
        }

        public class Token {
            public String token;
            public String original;
            public bool isString;
            public bool isExplicitString;

            public Token(string tokenParameter, bool isStringParameter, bool isExplicit) {
                isString = isStringParameter;
                isExplicitString = isExplicit;
                token = tokenParameter.ToLower();
                original = tokenParameter;
            }

            public override string ToString() => token;
        }
    }
}
