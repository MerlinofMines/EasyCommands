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
        Dictionary<String, List<CommandParameter>> propertyWords = NewDictionary<string, List<CommandParameter>>();

        string[] separateTokensFirstPass = new[] { "(", ")", "[", "]", ",", "*", "/", "!", "^", "..", "%", ">=", "<=", "==", "&&", "||", "@", "$", "->", "++", "+=", "--", "-=" };
        string[] separateTokensSecondPass = new[] { "<", ">", "=", "&", "|", ".", "-", "+", "?", ":" };

        Dictionary<UniOperand, String> uniOperandToString = NewDictionary<UniOperand, String>();
        Dictionary<BiOperand, String> biOperandToString = NewDictionary<BiOperand, String>();
        Dictionary<Return, String> returnToString = NewDictionary(
            KeyValuePair(Return.BOOLEAN, "boolean"),
            KeyValuePair(Return.NUMERIC, "number"),
            KeyValuePair(Return.STRING, "string"),
            KeyValuePair(Return.VECTOR, "vector"),
            KeyValuePair(Return.COLOR, "color"),
            KeyValuePair(Return.LIST, "list")
        );

        public void InitializeParsers() {
            //Ignored words that have no command parameters
            AddWords(Words("the", "than", "turned", "block", "panel", "to", "from", "then", "of", "either", "for", "in", "do", "does", "second", "seconds"), new IgnoreCommandParameter());

            //Selector Related Words
            AddWords(Words("blocks", "group", "panels"), new GroupCommandParameter());
            AddWords(Words("my", "self", "this"), new SelfCommandParameter());
            AddWords(Words("$"), new VariableSelectorCommandParameter());

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
            AddWords(Words("bind", "tie", "link"), new AssignmentCommandParameter(true));
            AddWords(Words("move", "go", "tell", "turn", "rotate", "set", "assign", "allocate", "designate", "apply"), new AssignmentCommandParameter());
            AddWords(Words("reverse", "reversed"), new ReverseCommandParameter());
            AddWords(Words("raise", "extend"), new AssignmentCommandParameter(), new DirectionCommandParameter(Direction.UP));
            AddWords(Words("retract"), new AssignmentCommandParameter(), new DirectionCommandParameter(Direction.DOWN));
            AddWords(Words("increase", "increment", "add", "by", "++", "+="), new IncrementCommandParameter());
            AddWords(Words("decrease", "decrement", "reduce", "subtract", "--", "-="), new IncrementCommandParameter(false));
            AddWords(Words("global"), new GlobalCommandParameter());

            //Value Words
            AddWords(Words("on", "begin", "true", "start", "started", "resume", "resumed"), new BooleanCommandParameter(true));
            AddWords(Words("off", "terminate", "cancel", "end", "false", "stopped", "halt", "halted"), new BooleanCommandParameter(false));

            //Property Words
            AddPropertyWords(PluralWords("height", "length", "level"), Property.LEVEL);
            AddPropertyWords(PluralWords("angle"), Property.ANGLE);
            AddPropertyWords(AllWords(PluralWords("speed", "rate", "pace"), Words("velocity", "velocities")), Property.VELOCITY);
            AddPropertyWords(Words("connect", "join", "attach", "connected", "joined", "attached", "dock", "docked", "docking"), Property.CONNECTED);
            AddPropertyWords(Words("disconnect", "separate", "detach", "disconnected", "separated", "detached", "undock", "undocked"), Property.CONNECTED, false);
            AddPropertyWords(Words("lock", "locked", "freeze", "brake", "handbrake", "permanent"), Property.LOCKED);
            AddPropertyWords(Words("unlock", "unlocked", "unfreeze"), Property.LOCKED, false);
            AddPropertyWords(Words("run", "running", "execute", "executing"), Property.RUN);
            AddPropertyWords(Words("done", "ready", "complete", "finished", "built", "finish", "pressurized", "depressurized"), Property.COMPLETE);
            AddPropertyWords(Words("open", "opened"), Property.OPEN);
            AddPropertyWords(Words("close", "closed", "shut"), Property.OPEN, false);
            AddPropertyWords(PluralWords("fontsize", "size"), Property.FONT_SIZE);
            AddPropertyWords(PluralWords("text", "message"), Property.TEXT);
            AddPropertyWords(PluralWords("color"), Property.COLOR);
            AddPropertyWords(Words("power", "powered"), Property.POWER);
            AddPropertyWords(Words("enable", "enabled", "arm", "armed"), Property.ENABLE);
            AddPropertyWords(Words("disable", "disabled", "disarm", "disarmed"), Property.ENABLE, false);
            AddPropertyWords(Words("music", "song"), Property.SONG);
            AddPropertyWords(Words("silent", "silence"), Property.SILENCE);
            AddPropertyWords(AllWords(PluralWords("volume", "output"), Words("intensity", "intensities")), Property.VOLUME);
            AddPropertyWords(AllWords(PluralWords("range", "distance", "limit", "delay"), Words("radius", "radii", "capacity", "capacities")), Property.RANGE);
            AddPropertyWords(PluralWords("blinkinterval", "interval"), Property.BLINK_INTERVAL);
            AddPropertyWords(PluralWords("blinklength"), Property.BLINK_LENGTH);
            AddPropertyWords(PluralWords("blinkoffset", "offset"), Property.OFFSET);
            AddPropertyWords(PluralWords("falloff"), Property.FALLOFF);
            AddPropertyWords(Words("trigger", "triggered", "detect", "detected", "trip", "tripped", "deploy", "deployed", "shoot", "shooting", "shot", "detonate"), Property.TRIGGER);
            AddPropertyWords(Words("pressure", "pressurize", "pressurizing", "supply", "supplying", "generate", "generating", "discharge", "discharging", "broadcast", "broadcasting", "assemble", "assembling"), Property.SUPPLY);
            AddPropertyWords(Words("stockpile", "depressurize", "depressurizing", "gather", "gathering", "intake", "recharge", "recharging", "consume", "consuming", "collect", "collecting", "disassemble", "disassembling"), Property.SUPPLY, false);
            AddPropertyWords(AllWords(PluralWords("ratio", "percentage", "percent", "completion"), Words("progress", "progresses")), Property.RATIO);
            AddPropertyWords(PluralWords("input", "pilot", "user"), Property.INPUT);
            AddPropertyWords(PluralWords("roll", "rollInput"), Property.ROLL_INPUT);
            AddPropertyWords(Words("auto", "refill", "drain", "draining", "cooperate", "cooperating"), Property.AUTO);
            AddPropertyWords(AllWords(PluralWords("override"), Words("overridden")), Property.OVERRIDE);
            AddPropertyWords(PluralWords("direction"), Property.DIRECTION);
            AddPropertyWords(PluralWords("position", "location"), Property.POSITION);
            AddPropertyWords(Words("target", "destination", "waypoint", "coords", "coordinates"), Property.TARGET);
            AddPropertyWords(Words("waypoints", "destinations"), Property.WAYPOINTS);
            AddPropertyWords(Words("targetvelocity"), Property.TARGET_VELOCITY);
            AddPropertyWords(AllWords(PluralWords("strength", "force", "torque"), Words("gravity", "gravities")), Property.STRENGTH);
            AddPropertyWords(PluralWords("countdown"), Property.COUNTDOWN);
            AddPropertyWords(Words("name", "label"), Property.NAME);
            AddPropertyWords(Words("names", "labels"), Property.NAMES);
            AddPropertyWords(Words("show", "showing"), Property.SHOW);
            AddPropertyWords(Words("hide", "hiding"), Property.SHOW, false);
            AddPropertyWords(Words("properties", "attributes"), Property.PROPERTIES);
            AddPropertyWords(Words("actions"), Property.ACTIONS);

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
            AddWords(Words("listen", "channel"), new ListenCommandParameter());
            AddWords(Words("send"), new SendCommandParameter());
            AddWords(Words("print", "log", "echo", "write"), new PrintCommandParameter());
            AddWords(Words("queue", "schedule"), new QueueCommandParameter(false));
            AddWords(Words("async", "background", "parallel"), new QueueCommandParameter(true));
            AddWords(Words("transfer", "give"), new TransferCommandParameter(true));
            AddWords(Words("take"), new TransferCommandParameter(false));
            AddWords(Words("->"), new KeyedVariableCommandParameter());
            AddWords(Words("?"), new TernaryConditionIndicatorParameter());
            AddWords(Words(":"), new TernaryConditionSeparatorParameter());
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
            AddWords(Words("count", "number"), new PropertyAggregationCommandParameter((blocks, primitiveSupplier) => ResolvePrimitive(blocks.Count())));
            AddWords(Words("sum", "total"), new PropertyAggregationCommandParameter(SumAggregator));

            //Operations Words
            AddWords(Words("("), new OpenParenthesisCommandParameter());
            AddWords(Words(")"), new CloseParenthesisCommandParameter());
            AddWords(Words("and", "&", "&&", "but", "yet"), new AndCommandParameter());
            AddWords(Words("or", "|", "||"), new OrCommandParameter());
            AddWords(Words("not", "!", "isnt", "arent", "stop"), new NotCommandParameter());
            AddWords(Words("@"), new IndexCommandParameter());

            AddRightUniOperationWords(Words("absolute", "abs"), UniOperand.ABS);
            AddRightUniOperationWords(Words("sqrt"), UniOperand.SQRT);
            AddRightUniOperationWords(Words("sin"), UniOperand.SIN);
            AddRightUniOperationWords(Words("cosine", "cos"), UniOperand.COS);
            AddRightUniOperationWords(Words("tangent", "tan"), UniOperand.TAN);
            AddRightUniOperationWords(Words("arcsin", "asin"), UniOperand.ASIN);
            AddRightUniOperationWords(Words("arccos", "acos"), UniOperand.ACOS);
            AddRightUniOperationWords(Words("arctan", "atan"), UniOperand.ATAN);
            AddRightUniOperationWords(Words("round", "rnd"), UniOperand.ROUND);
            AddRightUniOperationWords(Words("sorted"), UniOperand.SORT);

            AddLeftUniOperationWords(Words("tick", "ticks"), UniOperand.TICKS);
            AddLeftUniOperationWords(Words("keys", "indexes"), UniOperand.KEYS);
            AddLeftUniOperationWords(Words("values"), UniOperand.VALUES);

            AddTier1OperationWords(Words("multiply", "*"), BiOperand.MULTIPLY);
            AddTier1OperationWords(Words("divide", "/"), BiOperand.DIVIDE);
            AddTier1OperationWords(Words("mod", "%"), BiOperand.MOD);
            AddTier1OperationWords(Words("dot", "."), BiOperand.DOT);
            AddTier1OperationWords(Words("pow", "exp", "^"), BiOperand.EXPONENT);

            AddTier2OperationWords(Words("plus", "+"), BiOperand.ADD);
            AddTier2OperationWords(Words("minus", "-"), BiOperand.SUBTRACT);

            AddTier3OperationWords(Words("as", "cast"), BiOperand.CAST);
            AddTier3OperationWords(Words(".."), BiOperand.RANGE);

            //List Words
            AddWords(Words("["), new OpenBracketCommandParameter());
            AddWords(Words("]"), new CloseBracketCommandParameter());
            AddWords(Words(","), new ListSeparatorCommandParameter());

            //Control Types
            AddWords(Words("restart", "reset", "reboot"), new ControlCommandParameter(Control.RESTART));
            AddWords(Words("repeat", "loop", "rerun", "replay"), new ControlCommandParameter(Control.REPEAT));
            AddWords(Words("exit"), new ControlCommandParameter(Control.STOP));
            AddWords(Words("pause"), new ControlCommandParameter(Control.PAUSE));

            //Blocks
            AddBlockWords(Words("piston"), Block.PISTON);
            AddBlockWords(Words("light", "spotlight"), Block.LIGHT);
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
            AddBlockWords(PluralWords("display", "screen", "lcd"), Words(""), Block.DISPLAY);
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
            AddBlockWords(Words("generator"), Block.GENERATOR);
            AddBlockWords(Words("tank"), Block.TANK);
            AddBlockWords(Words("gear"), Block.GEAR);
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
            AddBlockWords(Words("gyro", "gyroscopes"), Block.GYROSCOPE);
            AddBlockWords(Words("gravitygenerator"), Block.GRAVITY_GENERATOR);
            AddBlockWords(Words("gravitysphere"), Block.GRAVITY_SPHERE);
            AddBlockWords(Words("cargo", "container", "inventory", "inventories"), Words("containers"), Block.CARGO);
            AddBlockWords(Words("warhead", "bomb"), Block.WARHEAD);
            AddBlockWords(Words("assembler"), Block.ASSEMBLER);
            AddBlockWords(Words("collector"), Block.COLLECTOR);
            AddBlockWords(Words("ejector"), Block.EJECTOR);
            AddBlockWords(Words("decoy"), Block.DECOY);
            AddBlockWords(Words("jumpdrive"), Block.JUMPDRIVE);
            AddBlockWords(Words("laser", "laserantenna"), Block.LASER_ANTENNA);
            AddBlockWords(Words("terminal"), Block.TERMINAL);
            AddBlockWords(Words("refinery"), Words("refineries"), Block.REFINERY);

            //Register Special CommandParameter Output Values
            RegisterToString<GroupCommandParameter>(p => "group");
            RegisterToString<AmbiguiousStringCommandParameter>(p => "\"" + p.value + "\"");
            RegisterToString<StringCommandParameter>(p => "'" + p.value + "'");
            RegisterToString<VariableAssignmentCommandParameter>(p => "Assign[name=" + p.variableName + ",global=" + p.isGlobal + ",ref=" + p.useReference + "]");
            RegisterToString<VariableCommandParameter>(p => "[Variable]");
            RegisterToString<VariableSelectorCommandParameter>(p => "[VariableSelector]");
            RegisterToString<IndexSelectorCommandParameter>(p => "[IndexSelector]");
            RegisterToString<FunctionDefinitionCommandParameter>(p => "Function[switch=" + p.switchExecution + ",name=" + p.functionDefinition.functionName + "]");
            RegisterToString<ConditionCommandParameter>(p => "[Condition]");
            RegisterToString<BlockConditionCommandParameter>(p => "[BlockCondition]");
            RegisterToString<CommandReferenceParameter>(p => "[Command]");
            RegisterToString<RepetitionCommandParameter>(p => "[Repeat]");
            RegisterToString<SelectorCommandParameter>(p => "[Selector]");
            RegisterToString<ThatCommandParameter>(p => "That");
            RegisterToString<AssignmentCommandParameter>(p => "[Action]");
            RegisterToString<PropertyCommandParameter>(p => "Property[id=" + p.value.propertyType + "]");
        }

        Dictionary<Type, Func<CommandParameter, object>> commandParameterStrings = NewDictionary<Type, Func<CommandParameter, object>>();

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

        String[] Words(params String[] words) => words;

        String[] AllWords(params String[][] words) => words.Aggregate((a, b) => a.Concat(b).ToArray());

        String[] PluralWords(params String[] words) => words.Concat(words.Select(w => w + "s")).ToArray();

        void AddPropertyWords(String[] words, Property property, bool nonNegative = true) {
            if (!nonNegative) AddWords(words, new PropertyCommandParameter(property), new BooleanCommandParameter(false));
            else AddWords(words, new PropertyCommandParameter(property));
        }

        void AddRightUniOperationWords(String[] words, UniOperand operand) {
            AddWords(words, new UniOperationCommandParameter(operand));
            uniOperandToString[operand] = words[0];
        }

        void AddLeftUniOperationWords(String[] words, UniOperand operand) {
            AddWords(words, new LeftUniOperationCommandParameter(operand));
            uniOperandToString[operand] = words[0];
        }

        void AddTier1OperationWords(String[] words, BiOperand operand) {
            AddWords(words, new BiOperandTier1Operand(operand));
            biOperandToString[operand] = words[0];
        }

        void AddTier2OperationWords(String[] words, BiOperand operand) {
            AddWords(words, new BiOperandTier2Operand(operand));
            biOperandToString[operand] = words[0];
        }

        void AddTier3OperationWords(String[] words, BiOperand operand) {
            AddWords(words, new BiOperandTier3Operand(operand));
            biOperandToString[operand] = words[0];
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
            var parameters = NewList<CommandParameter>();

            foreach (var token in tokens) {
                var newParameters = ParseCommandParameters(token);
                newParameters.ForEach(p => p.Token = token.original);
                parameters.AddRange(newParameters);
            }

            return parameters;
        }

        List<CommandParameter> ParseCommandParameters(Token token) {
            var commandParameters = NewList<CommandParameter>();
            String t = token.token;
            if (token.isExplicitString) {
                commandParameters.Add(new StringCommandParameter(token.original, true));
            } else if (token.isString) {
                List<Token> subTokens = ParseTokens(t);
                List<CommandParameter> subtokenParams = ParseCommandParameters(subTokens);
                commandParameters.Add(new AmbiguiousStringCommandParameter(token.original, false, subtokenParams.ToArray()));
            } else if (propertyWords.ContainsKey(t)) {
                commandParameters.AddList(propertyWords[t]);
            } else { //If no property matches, must be a string
                commandParameters.Add(new AmbiguiousStringCommandParameter(token.original, true));
            }

            return commandParameters;
        }

        public List<Token> ParseTokens(String commandString) => (String.IsNullOrWhiteSpace(commandString) || commandString.Trim().StartsWith("#")) ? NewList<Token>() :
            ParseSurrounded(commandString, "`\'\"",
                u => u.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(ParseSeparateTokens)
                    .Select(v => new Token(v, false, false))
                    .ToArray())
            .ToList();

        Token[] ParseSurrounded(String token, string characters, Func<String, Token[]> parseSubTokens) =>
            characters.Length == 0 ? parseSubTokens(token) :
                token.Trim().Split(characters[0])
                .SelectMany((element, index) => index % 2 == 0  // If even index
                    ? ParseSurrounded(element, characters.Remove(0,1), parseSubTokens)  // Split the item
                    : new Token[] { new Token(element, true, characters.Length > 1) })  // Keep the entire item
                .ToArray();

        String[] ParseSeparateTokens(String command) {
            var newCommand = command;
            foreach (var token in separateTokensFirstPass) newCommand = newCommand.Replace(token, " " + token + " ");
            newCommand = string.Join(" ", newCommand.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).SelectMany(token => {
                Primitive ignored;
                if (separateTokensFirstPass.Contains(token) || ParsePrimitive(token, out ignored)) return new[] { token };
                foreach (var s in separateTokensSecondPass) token = token.Replace(s, " " + s + " ");
                return token.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }));

            List<char> commandArray = newCommand.ToCharArray().ToList();

            return new string(commandArray.ToArray()).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool ParsePrimitive(String token, out Primitive primitive) {
            primitive = null;
            Vector3D vector;
            Double numeric;
            Color color;
            if (Double.TryParse(token, out numeric)) primitive = ResolvePrimitive((float)numeric);
            if (GetVector(token, out vector)) primitive = ResolvePrimitive(vector);
            if (GetColor(token, out color)) primitive = ResolvePrimitive(color);
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
