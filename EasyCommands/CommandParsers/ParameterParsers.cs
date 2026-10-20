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
    using Sandbox.ModAPI.Ingame;
    using SpaceEngineers.Game.ModAPI.Ingame;
    using VRage.Game.ModAPI.Ingame.Utilities;
    using VRage.Game.ModAPI.Ingame;
    partial class Program {
        //Internal (Don't touch!)
        Dictionary<String, List<ICommandParameter>> propertyWords = NewDictionary<string, List<ICommandParameter>>();

        string[] firstPassTokens = new[] { "(", ")", "[", "]", ",", "*", "/", "!", "^", "..", "%", ">=", "<=", "==", "&&", "||", "@", "$", "->", "++", "+=", "--", "-=", "::"};
        string[] secondPassTokens = new[] { "<", ">", "=", "&", "|", "-", "+", "?", ":" };
        string[] thirdPassTokens = new[] { "." };

        static Dictionary<UniOperand, String> uniOperandToString = NewDictionary(
            KeyValuePair(UniOperand.CAST, "cast"),
            KeyValuePair(UniOperand.ROUND, "round"),
            KeyValuePair(UniOperand.REVERSE, "negate")
        );
        static Dictionary<BiOperand, String> biOperandToString = NewDictionary(
            KeyValuePair(BiOperand.CAST, "cast"),
            KeyValuePair(BiOperand.ROUND, "round"),
            KeyValuePair(BiOperand.COMPARE, "compare")
        );
        static Dictionary<Return, String> returnToString = NewDictionary(
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

        delegate IEnumerable<Token> Pass(string s);
        delegate Pass Generator(Pass p);

        Pass drop = s => Empty<Token>();
        Pass take = s => Once(new Token(s, false, false));
        Pass tokenize;

        public void InitializeParsers() {
            // build tokenizer stack
            tokenize = Generate(
                take,
                ConditionalPass(s => !string.IsNullOrWhiteSpace(s) && !s.Trim().StartsWith("#")),
                EnclosedPass('`'),
                EnclosedPass('\''),
                EnclosedPass('"', false),
                ReplacePass(" : ", " :: "),
                SymbolPass(firstPassTokens),
                PrimitivePass(),
                SymbolPass(secondPassTokens),
                PrimitivePass(),
                SymbolPass(thirdPassTokens)
            );

            //Ignored words that have no command parameters
            AddWords(Synonyms("in the than turned block panel chamber drive of either for do does second seconds be being digits digit"), new IgnoreCommandParameter());
            AddWords(Synonyms("to from then"), new CommandSeparatorCommandParameter());

            //Selector Related Words
            AddWords(Synonyms("blocks group panels chambers drives"), new GroupCommandParameter());
            AddWords(Synonyms("my self this"), new SelfCommandParameter());
            AddWords(Synonyms("$"), new VariableSelectorCommandParameter());

            //Direction Words
            AddDirectionWords(Synonyms("up upward upwards upper"), Direction.UP);
            AddDirectionWords(Synonyms("down downward downwards lower"), Direction.DOWN);
            AddDirectionWords(Synonyms("left lefthand"), Direction.LEFT);
            AddDirectionWords(Synonyms("right righthand"), Direction.RIGHT);
            AddDirectionWords(Synonyms("forward forwards front"), Direction.FORWARD);
            AddDirectionWords(Synonyms("backward backwards back"), Direction.BACKWARD);
            AddDirectionWords(Synonyms("clockwise clock"), Direction.CLOCKWISE);
            AddDirectionWords(Synonyms("counterclockwise counter counterclock"), Direction.COUNTERCLOCKWISE);

            //Action Words
            AddWords(Synonyms("bind tie link"), new AssignmentCommandParameter(true));
            AddWords(Synonyms("move go tell turn rotate set assign allocate designate apply"), new AssignmentCommandParameter());
            AddWords(Synonyms("reverse reversed"), new ReverseCommandParameter());
            AddWords(Synonyms("raise extend"), new AssignmentCommandParameter(), new DirectionCommandParameter(Direction.UP));
            AddWords(Synonyms("retract"), new AssignmentCommandParameter(), new DirectionCommandParameter(Direction.DOWN));
            AddWords(Synonyms("increase increment"), new IncreaseCommandParameter());
            AddWords(Synonyms("decrease decrement reduce"), new IncreaseCommandParameter(false));
            AddWords(Synonyms("++ +="), new IncrementCommandParameter());
            AddWords(Synonyms("-- -="), new IncrementCommandParameter(false));
            AddWords(Synonyms("global"), new GlobalCommandParameter());
            AddWords(Synonyms("by"), new RelativeCommandParameter());

            //Value Words
            AddWords(Synonyms("on begin true start started resume resumed"), new VariableCommandParameter(GetStaticVariable(true)));
            AddWords(Synonyms("off terminate cancel end false stopped halt halted"), new VariableCommandParameter(GetStaticVariable(false)));

            AddPropertyWords(Synonyms("crop"), Property.CROP);
            AddPropertyWords(Synonyms("alive"), Property.ALIVE);
            AddPropertyWords(Synonyms("dead"), Property.ALIVE, false);

            //Property Words
            AddPropertyWords(Words("repeat path", "loop path"), Property.REPEAT);
            AddPropertyWords(Synonyms("behavior behaviour"), Property.AUTO);
            AddPropertyWords(Synonyms("precision"), Property.PRECISION);
            AddPropertyWords(Synonyms("avoid avoidance"), Property.AVOID);
            AddPropertyWords(Synonyms("flee fleeing"), Property.FLEE);
            AddPropertyWords(AllWords(PluralWords("height length level size period scale")), Property.LEVEL);
            AddPropertyWords(PluralWords("angle azimuth"), Property.ANGLE);
            AddPropertyWords(AllWords(PluralWords("speed rate pace"), Synonyms("velocity velocities")), Property.VELOCITY);
            AddPropertyWords(Synonyms("connect attach connected attached dock docked docking"), Property.CONNECTED);
            AddPropertyWords(Synonyms("disconnect detach disconnected detached undock undocked"), Property.CONNECTED, false);
            AddPropertyWords(Synonyms("lock locked locking freeze frozen brake braking handbrake permanent static"), Property.LOCKED);
            AddPropertyWords(Synonyms("unlock unlocked unfreeze"), Property.LOCKED, false);
            AddPropertyWords(Synonyms("run running execute executing script"), Property.RUN);
            AddPropertyWords(Synonyms("use used occupy occupied control controlled"), Property.USE);
            AddPropertyWords(Synonyms("unused unoccupied vacant available"), Property.USE, false);
            AddPropertyWords(Synonyms("done complete finished finish pressurized depressurized"), Property.COMPLETE);
            AddPropertyWords(Synonyms("clear wipe erase"), Property.COMPLETE, false);
            AddPropertyWords(Synonyms("open opened"), Property.OPEN);
            AddPropertyWords(Synonyms("close closed shut"), Property.OPEN, false);
            AddPropertyWords(PluralWords("font"), Property.FONT);
            AddPropertyWords(PluralWords("text message argument"), Property.TEXT);
            AddPropertyWords(AllWords(Synonyms("colors"), PluralWords("foreground")), Property.COLOR);
            AddAmbiguousWords(Synonyms("color"), new PropertyCommandParameter(Property.COLOR));
            AddPropertyWords(PluralWords("background"), Property.BACKGROUND);
            AddPropertyWords(Synonyms("power powered"), Property.POWER);
            AddPropertyWords(Synonyms("enable enabled arm armed"), Property.ENABLE);
            AddPropertyWords(Synonyms("disable disabled disarm disarmed"), Property.ENABLE, false);
            AddPropertyWords(Synonyms("music sound song track image play playing unsilence"), Property.MEDIA);
            AddPropertyWords(Synonyms("silence silent quiet"), Property.MEDIA, false);
            AddPropertyWords(Synonyms("sounds songs images tracks"), Property.MEDIA_LIST);
            AddPropertyWords(AllWords(PluralWords("volume output"), Synonyms("intensity intensities")), Property.VOLUME);
            AddPropertyWords(AllWords(PluralWords("range distance limit delay"), Synonyms("capacity capacities")), Property.RANGE);
            AddPropertyWords(Synonyms("radius radii"), Property.RADIUS);
            AddPropertyWords(PluralWords("interval"), Property.INTERVAL);
            AddPropertyWords(PluralWords("offset padding"), Property.OFFSET);
            AddPropertyWords(PluralWords("falloff"), Property.FALLOFF);
            AddPropertyWords(Synonyms("trigger triggered detect detected trip tripped deploy deployed shoot shooting shot detonate fire firing"), Property.TRIGGER);
            AddPropertyWords(Synonyms("pressure pressurize pressurizing supply supplying generate generating discharge discharging broadcast broadcasting assemble assembling"), Property.SUPPLY);
            AddPropertyWords(Synonyms("stockpile stockpiling depressurize depressurizing gather gathering intake recharge recharging consume consuming collect collecting disassemble disassembling"), Property.SUPPLY, false);
            AddPropertyWords(AllWords(PluralWords("ratio percentage percent completion"), Synonyms("progress progresses")), Property.RATIO);
            AddPropertyWords(PluralWords("input pilot user"), Property.INPUT);
            AddPropertyWords(PluralWords("roll rollInput rotation"), Property.ROLL_INPUT);
            AddPropertyWords(Synonyms("auto autopilot refill drain draining cooperate cooperating"), Property.AUTO);
            AddPropertyWords(AllWords(PluralWords("override dampener"), Synonyms("overridden")), Property.OVERRIDE);
            AddPropertyWords(PluralWords("direction"), Property.DIRECTION);
            AddPropertyWords(PluralWords("position location alignment"), Property.POSITION);
            AddPropertyWords(Synonyms("target targeting destination waypoint coords coordinates"), Property.TARGET);
            AddPropertyWords(Synonyms("waypoints destinations"), Property.WAYPOINTS);
            AddPropertyWords(AllWords(PluralWords("strength force torque"), Synonyms("gravity gravities")), Property.STRENGTH);
            AddPropertyWords(Synonyms("natural planet"), Property.NATURAL);
            AddPropertyWords(Synonyms("artificial fake"), Property.ARTIFICIAL);
            AddPropertyWords(PluralWords("countdown"), Property.COUNTDOWN);
            AddPropertyWords(PluralWords("name label"), Property.NAME);
            AddPropertyWords(Synonyms("show showing"), Property.SHOW);
            AddPropertyWords(Synonyms("hide hiding"), Property.SHOW, false);
            AddPropertyWords(Synonyms("properties attributes"), Property.PROPERTIES);
            AddPropertyWords(Synonyms("actions"), Property.ACTIONS);
            AddPropertyWords(Synonyms("types blueprints"), Property.TYPES);
            AddPropertyWords(PluralWords("altitude elevation"), Property.ALTITUDE);
            AddPropertyWords(Synonyms("weight mass"), Property.WEIGHT);
            AddPropertyWords(Synonyms("data customdata"), Property.DATA);
            AddPropertyWords(Synonyms("info details detailedinfo"), Property.INFO);
            AddPropertyWords(Synonyms("invert inverted inverting"), Property.INVERT);
            AddPropertyWords(Synonyms("steer steering"), Property.STEER);
            AddPropertyWords(Synonyms("able ready"), Property.ABLE);
            AddPropertyWords(Synonyms("unable"), Property.ABLE, false);
            AddPropertyWords(Synonyms("build building built"), Property.BUILD);
            AddPropertyWords(Synonyms("damage damaged"), Property.DAMAGE);

            //ValueProperty Words
            AddWords(PluralWords("amount"), new ValuePropertyCommandParameter(Property.AMOUNT));
            AddWords(Synonyms("property attribute"), new ValuePropertyCommandParameter(Property.PROPERTY));
            AddWords(Synonyms("action"), new ValuePropertyCommandParameter(Property.ACTION));
            AddWords(Synonyms("produce producing create creating make making"), new ValuePropertyCommandParameter(Property.CREATE));
            AddWords(Synonyms("destroy destroying recycle recycling"), new ValuePropertyCommandParameter(Property.DESTROY));

            //Special Command Words
            AddWords(Synonyms("times iterations"), new RepeatCommandParameter());
            AddWords(Synonyms("wait hold"), new WaitCommandParameter());
            AddWords(Synonyms("call gosub"), new FunctionCommandParameter(false));
            AddWords(Synonyms("goto"), new FunctionCommandParameter(true));
            AddWords(Synonyms("listen channel register subscribe"), new ListenCommandParameter(true));
            AddWords(Synonyms("forget dismiss ignore deregister unsubscribe"), new ListenCommandParameter(false));
            AddWords(Synonyms("send"), new SendCommandParameter());
            AddWords(Synonyms("print log echo write"), new PrintCommandParameter());
            AddWords(Synonyms("queue schedule"), new QueueCommandParameter(false));
            AddAmbiguousWords(Synonyms("async parallel"), new QueueCommandParameter(true));
            AddWords(Synonyms("await blocking"), new AwaitCommandParameter());
            AddWords(Synonyms("transfer give"), new TransferCommandParameter(true));
            AddWords(Synonyms("take"), new TransferCommandParameter(false));
            AddWords(Synonyms("->"), new KeyedVariableCommandParameter());
            AddWords(Synonyms("?"), new TernaryConditionIndicatorParameter());
            AddWords(Synonyms("::"), new TernaryConditionSeparatorParameter());
            AddWords(Synonyms(":"), new ColonSeparatorParameter());
            AddWords(Synonyms("each every"), new IteratorCommandParameter());

            //Conditional Words
            AddWords(Synonyms("if"), new IfCommandParameter(false, false, false));
            AddWords(Synonyms("unless"), new IfCommandParameter(true, false, false));
            AddWords(Synonyms("while"), new IfCommandParameter(false, true, false));
            AddWords(Synonyms("until"), new IfCommandParameter(true, true, false));
            AddWords(Synonyms("when"), new IfCommandParameter(true, true, true));
            AddWords(Synonyms("else otherwise"), new ElseCommandParameter());
            AddWords(Synonyms("that which whose"), new ThatCommandParameter());

            //Comparison Words
            AddWords(Synonyms("less < below"), new ComparisonCommandParameter((a, b) => a.CompareTo(b) < 0));
            AddWords(Synonyms("<="), new ComparisonCommandParameter((a, b) => a.CompareTo(b) <= 0));
            AddWords(Synonyms("is are equal equals = =="), new ComparisonCommandParameter((a, b) => a.CompareTo(b) == 0));
            AddWords(Synonyms(">="), new ComparisonCommandParameter((a, b) => a.CompareTo(b) >= 0));
            AddWords(Synonyms("greater > above more"), new ComparisonCommandParameter((a, b) => a.CompareTo(b) > 0));
            AddWords(Synonyms("contain contains"), new ComparisonCommandParameter((a, b) => CastBoolean(PerformOperation(BiOperand.CONTAINS, a, b))));

            //Aggregation Words
            AddWords(Synonyms("any"), new AggregateConditionCommandParameter((count, matches) => matches > 0));
            AddWords(Synonyms("all"), new AggregateConditionCommandParameter(AllCondition));
            AddWords(Synonyms("none"), new AggregateConditionCommandParameter(NoneCondition));
            AddWords(Synonyms("average avg"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => SumAggregator(blocks, primitiveSupplier).Divide(ResolvePrimitive(Math.Max(1, blocks.Count())))));
            AddWords(Synonyms("minimum min"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => blocks.Select(primitiveSupplier).Min() ?? ResolvePrimitive(0)));
            AddWords(Synonyms("maximum max"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => blocks.Select(primitiveSupplier).Max() ?? ResolvePrimitive(0)));
            AddWords(Synonyms("count"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => ResolvePrimitive(blocks.Count())));
            AddAmbiguousWords(Synonyms("number"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => ResolvePrimitive(blocks.Count())));
            AddWords(Synonyms("sum total"), new PropertyAggregationCommandParameter(SumAggregator));
            AddWords(Synonyms("collection"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => ResolvePrimitive(NewKeyedList(blocks.Select(b => new StaticVariable(primitiveSupplier(b)))))));
            AddAmbiguousWords(Synonyms("list"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => ResolvePrimitive(NewKeyedList(blocks.Select(b => new StaticVariable(primitiveSupplier(b)))))));

            //Operations Words
            AddWords(Synonyms("("), new OpenParenthesisCommandParameter());
            AddWords(Synonyms(")"), new CloseParenthesisCommandParameter());
            AddWords(Synonyms("and & && but yet"), new AndCommandParameter());
            AddWords(Synonyms("or | ||"), new OrCommandParameter());
            AddWords(Synonyms("not ! stop"), new NotCommandParameter());
            AddWords(Synonyms("@"), new IndexCommandParameter());

            AddRightUniOperationWords(Synonyms("absolute abs"), UniOperand.ABS);
            AddRightUniOperationWords(Synonyms("sqrt"), UniOperand.SQRT);
            AddRightUniOperationWords(Synonyms("sin"), UniOperand.SIN);
            AddRightUniOperationWords(Synonyms("cosine cos"), UniOperand.COS);
            AddRightUniOperationWords(Synonyms("tangent tan"), UniOperand.TAN);
            AddRightUniOperationWords(Synonyms("arcsin asin"), UniOperand.ASIN);
            AddRightUniOperationWords(Synonyms("arccos acos"), UniOperand.ACOS);
            AddRightUniOperationWords(Synonyms("arctan atan"), UniOperand.ATAN);
            AddRightUniOperationWords(Synonyms("sort sorted"), UniOperand.SORT);
            AddRightUniOperationWords(Synonyms("ln"), UniOperand.LN);
            AddRightUniOperationWords(Synonyms("rand random randomize"), UniOperand.RANDOM);
            AddRightUniOperationWords(Synonyms("shuffle shuffled"), UniOperand.SHUFFLE);
            AddRightUniOperationWords(Synonyms("sign quantize"), UniOperand.SIGN);

            AddLeftUniOperationWords(Synonyms("tick ticks"), UniOperand.TICKS);
            AddLeftUniOperationWords(Synonyms("keys indexes"), UniOperand.KEYS);
            AddLeftUniOperationWords(Synonyms("values"), UniOperand.VALUES);
            AddLeftUniOperationWords(Synonyms("type"), UniOperand.TYPE);

            //Tier 0 Operations
            AddWords(Synonyms("dot ."), new BiOperandCommandParameter(BiOperand.DOT, 0));

            //Tier 1 Operations
            AddBiOperationWords(Synonyms("pow ^ xor"), BiOperand.EXPONENT, 1);

            //Tier 2 Operations
            AddBiOperationWords(Synonyms("multiply *"), BiOperand.MULTIPLY, 2);
            AddBiOperationWords(Synonyms("divide /"), BiOperand.DIVIDE, 2);
            AddBiOperationWords(Synonyms("mod %"), BiOperand.MOD, 2);
            AddBiOperationWords(Synonyms("split separate separated"), BiOperand.SPLIT, 2);
            AddBiOperationWords(Synonyms("join joined"), BiOperand.JOIN, 2);

            //Tier 3 Operations
            AddBiOperationWords(Synonyms("plus +"), BiOperand.ADD, 3);
            AddBiOperationWords(Synonyms("minus"), BiOperand.SUBTRACT, 3);

            //Tier 4 Operations
            AddBiOperationWords(Synonyms(".."), BiOperand.RANGE, 4);

            AddWords(Synonyms("-"), new MinusCommandParameter());
            AddWords(Synonyms("round rnd rounded"), new RoundCommandParameter());
            AddWords(Synonyms("as cast resolve resolved"), new CastCommandParameter());

            //List Words
            AddWords(Synonyms("["), new OpenBracketCommandParameter());
            AddWords(Synonyms("]"), new CloseBracketCommandParameter());
            AddWords(Synonyms(","), new ListSeparatorCommandParameter());

            //Control Types
            AddControlWords(Synonyms("restart reset reboot"), thread => {
                PROGRAM.ClearAllState();
                throw new InterruptException(ProgramState.RUNNING);
            });
            AddControlWords(Synonyms("repeat loop rerun replay"), thread => {
                thread.Command = thread.Command.Clone();
                return false;
            });
            AddControlWords(Synonyms("exit"), thread => {
                PROGRAM.ClearAllState();
                throw new InterruptException(ProgramState.STOPPED);
            });
            AddControlWords(Synonyms("pause"), thread => {
                if (PROGRAM.state != ProgramState.PAUSED) throw new InterruptException(ProgramState.PAUSED);
                return true;
             });
            AddControlWords(Synonyms("break"), thread => {
                GetInterrupableCommand("break").Break();
                return false;
            });
            AddControlWords(Synonyms("continue"), thread => {
                GetInterrupableCommand("continue").Continue();
                return false;
            });
            AddControlWords(Synonyms("return"), thread => {
                FunctionCommand currentFunction = thread.GetCurrentCommand<FunctionCommand>();
                Command nullCommand = new NullCommand();
                if (currentFunction == null) thread.Command = nullCommand;
                else currentFunction.function = nullCommand;
                return false;
            });

            AddBlockWords(Words("farmplot", "farm plot"), Block.FARM_PLOT);
            AddBlockWords(Words("algaefarm", "algae farm"), Block.ALGAE_FARM);
            AddBlockWords(Words("oxygenfarm", "oxygen farm"), Block.OXYGEN_FARM);
            AddBlockWords(Words("foodprocessor", "food processor"), Block.FOOD_PROCESSOR);
            AddBlockWords(Words("irrigation", "irrigation system"), Block.IRRIGATION);

            //Blocks
            AddBlockWords(Words("aiflight", "ai flight"), Block.AI_FLIGHT);
            AddBlockWords(Words("aibasic", "ai basic"), Block.AI_BASIC);
            AddBlockWords(Words("airecorder", "ai recorder"), Block.AI_RECORDER);
            AddBlockWords(Words("aidefensive", "ai defensive"), Block.AI_DEFENSIVE);
            AddBlockWords(Words("aioffensive", "ai offensive"), Block.AI_OFFENSIVE);
            AddBlockWords(Synonyms("piston"), Block.PISTON);
            AddBlockWords(Synonyms("light spotlight"), Block.LIGHT);
            AddBlockWords(Synonyms("rotor"), Block.ROTOR);
            AddBlockWords(Synonyms("hinge"), Block.HINGE);
            AddBlockWords(Synonyms("program programmable"), Block.PROGRAM);
            AddBlockWords(Synonyms("timer"), Block.TIMER);
            AddBlockWords(Synonyms("projector"), Block.PROJECTOR);
            AddBlockWords(Synonyms("merge"), Words(), Block.MERGE);
            AddBlockWords(Synonyms("connector"), Block.CONNECTOR);
            AddBlockWords(Synonyms("welder"), Block.WELDER);
            AddBlockWords(Synonyms("grinder"), Block.GRINDER);
            AddBlockWords(Synonyms("door hangar bay gate"), Block.DOOR);
            AddBlockWords(Words(), Words(), Block.DISPLAY, PluralWords("display screen lcd"));
            AddBlockWords(Synonyms("speaker alarm siren"), Block.SOUND);
            AddBlockWords(Synonyms("camera"), Block.CAMERA);
            AddBlockWords(Synonyms("sensor"), Block.SENSOR);
            AddBlockWords(Synonyms("beacon"), Block.BEACON);
            AddBlockWords(Synonyms("antenna"), Block.ANTENNA);
            AddBlockWords(Synonyms("ship rover cockpit seat station helm"), Block.COCKPIT);
            AddBlockWords(Synonyms("cryo"), Block.CRYO_CHAMBER);
            AddBlockWords(Synonyms("drone remote robot"), Block.REMOTE);
            AddBlockWords(Synonyms("thruster"), Block.THRUSTER);
            AddBlockWords(Synonyms("airvent vent"), Block.AIRVENT);
            AddBlockWords(Synonyms("gun railgun cannon autocannon rocket missile launcher"), Block.GUN);
            AddBlockWords(Synonyms("turret"), Block.TURRET);
            AddBlockWords(Words("generator", "oxygen generator", "hydrogen generator"), Block.GENERATOR);
            AddBlockWords(Words("tank", "oxygen tank", "hydrogen tank"), Block.TANK);
            AddBlockWords(Synonyms("magnet"), Synonyms("magnets gears"), Block.MAGNET, Synonyms("gear"));
            AddBlockWords(Synonyms("battery"), Synonyms("batteries"), Block.BATTERY);
            AddBlockWords(Synonyms("chute parachute"), Block.PARACHUTE);
            AddBlockWords(Synonyms("wheel"), Synonyms("wheels"), Block.SUSPENSION, Synonyms("suspension"));
            AddBlockWords(Synonyms("detector"), Block.DETECTOR);
            AddBlockWords(Synonyms("drill"), Block.DRILL);
            AddBlockWords(Synonyms("engine"), Block.ENGINE);
            AddBlockWords(Synonyms("turbine"), Block.TURBINE);
            AddBlockWords(Synonyms("reactor"), Block.REACTOR);
            AddBlockWords(Synonyms("solar"), Block.SOLAR_PANEL);
            AddBlockWords(Synonyms("sorter"), Block.SORTER);
            AddBlockWords(Synonyms("gyro gyroscope"), Block.GYROSCOPE);
            AddBlockWords(Synonyms("gravitygenerator"), Block.GRAVITY_GENERATOR);
            AddBlockWords(Synonyms("gravitysphere"), Block.GRAVITY_SPHERE);
            AddBlockWords(Synonyms("container"), Synonyms("cargos containers"), Block.CARGO, Synonyms("cargo inventory inventories"));
            AddBlockWords(Synonyms("warhead bomb"), Block.WARHEAD);
            AddBlockWords(Synonyms("assembler"), Block.ASSEMBLER);
            AddBlockWords(Synonyms("collector"), Block.COLLECTOR);
            AddBlockWords(Synonyms("ejector"), Block.EJECTOR);
            AddBlockWords(Synonyms("decoy"), Block.DECOY);
            AddBlockWords(Synonyms("jump jumpdrive"), Block.JUMPDRIVE);
            AddBlockWords(Synonyms("laser laserantenna"), Block.LASER_ANTENNA);
            AddBlockWords(Synonyms("terminal"), Block.TERMINAL);
            AddBlockWords(Synonyms("refinery"), Synonyms("refineries"), Block.REFINERY);
            AddBlockWords(Synonyms("heatvent"), Block.HEAT_VENT);
            AddBlockWords(Synonyms("searchlight"), Block.SEARCHLIGHT);
            AddBlockWords(Synonyms("turretcontroller"), Block.TURRET_CONTROLLER);
            AddBlockWords(Words(), Words(), Block.GRID, PluralWords("grid"));
            AddBlockWords(PluralWords("thread"), Words(), Block.THREAD);

            AddAliasWords(Synonyms("can"), "is able");
            AddAliasWords(Synonyms("cannot"), "is not able");
        }

        // Keep item phrases intact; only parser synonyms are separated by spaces.
        String[] Words(params String[] words) => words;

        String[] Synonyms(string words) => words.Split(' ');

        String[] AllWords(params String[][] words) => words.Aggregate((a, b) => a.Concat(b).ToArray());

        String[] PluralWords(string words) => AllWords(Synonyms(words), Synonyms(words).Select(w => w + "s").ToArray());

        void AddControlWords(String[] words, ControlFunction function) {
            AddWords(words, new CommandReferenceParameter(new ControlCommand { controlFunction = function }));
        }

        void AddPropertyWords(String[] words, Property property, bool nonNegative = true) =>
            AddWords(words, new PropertyCommandParameter(property, !nonNegative));

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

        void AddBlockWords(String[] blockWords, String[] groupWords, Block blockType, String[] ambiguousWords = null) {
            AddWords(blockWords, new SelectorTypeCommandParameter(new SelectorType { blockType = blockType, isGroup = false}));
            AddWords(groupWords, new SelectorTypeCommandParameter(new SelectorType { blockType = blockType, isGroup = true }));
            AddWords(ambiguousWords ?? new String[0], new SelectorTypeCommandParameter(new SelectorType { blockType = blockType }));
        }

        void AddAmbiguousWords(String[] words, params ICommandParameter[] commandParameters) {
            foreach (String word in words)
                AddWords(Words(word), new AmbiguousStringCommandParameter(word, false, new AmbiguousCommandParameter(commandParameters)));
        }

        void AddWords(String[] words, params ICommandParameter[] commandParameters) {
            foreach (String word in words) propertyWords.Add(word, commandParameters.ToList());
        }

        void AddAliasWords(String[] words, string aliasWords) {
            AddWords(words, PROGRAM.ParseCommandParameters(PROGRAM.Tokenize(aliasWords)).ToArray());
        }

        public List<Token> Tokenize(String commandString) => tokenize(commandString).ToList();

        List<ICommandParameter> ParseCommandParameters(List<Token> tokens) {
            var parameters = NewList<ICommandParameter>();
            for (int i = 0; i < tokens.Count; i++) {
                var token = tokens[i];
                // Match registered two-word selectors without crossing quoted-string boundaries.
                if (!token.isString && i + 1 < tokens.Count && !tokens[i + 1].isString &&
                    propertyWords.ContainsKey(token.token + " " + tokens[i + 1].token))
                    token = new Token(token.original + " " + tokens[++i].original, false, false);
                parameters.AddList(ParseCommandParameters(token));
            }
            return parameters;
        }

        List<ICommandParameter> ParseCommandParameters(Token token) {
            var commandParameters = NewList<ICommandParameter>();
            if (token.isExplicitString)
                commandParameters.Add(new VariableCommandParameter(GetStaticVariable(token.original)));
            else if (token.isString)
                commandParameters.Add(new AmbiguousStringCommandParameter(token.original, false, ParseCommandParameters(Tokenize(token.token)).ToArray()));
            else if (propertyWords.ContainsKey(token.token))
                commandParameters.AddList(propertyWords[token.token]);
            else //If no property matches, must be a string
                commandParameters.Add(new AmbiguousStringCommandParameter(token.original, true));

            commandParameters[0].Token = token.original;
            return commandParameters;
        }

        Pass Generate(Pass end, params Generator[] generators) =>
            Generate(end, generators.Select(g => g));
        Pass Generate(Pass end, IEnumerable<Generator> generators) =>
            generators.Reverse().Aggregate(end ?? drop, (p, g) => g(p));

        Generator ConditionalPass(Func<string, bool> pred) =>
            pass => str => pred(str) ? pass(str) : drop(str);
        Generator EnclosedPass(char encl, bool expl = true) =>
            pass => str => str.Split(encl).SelectMany((e, i) => i % 2 != 0 ? Once(new Token(e, true, expl)) : pass(e));
        Generator ReplacePass(string orig, string repl) =>
            pass => str => pass(str.Replace(orig, repl));
        Generator SymbolPass(string[] symbols) =>
            pass => Generate(
                str => str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(tok => symbols.Contains(tok) ? take(tok) : pass(tok)),
                symbols.Select(sym => ReplacePass(sym, $" {sym} ")));
        Generator PrimitivePass() =>
            pass => str => {
                Primitive ignored;
                return ParsePrimitive(str, out ignored) ? take(str) : pass(str);
            };

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
            public String token, original;
            public bool isString, isExplicitString;

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
