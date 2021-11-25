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

        string[] separateTokensFirstPass = new[] { "(", ")", "[", "]", ",", "+", "*", "/", "!", "^", "..", "%", ">=", "<=", "==", "&&", "||", "@", "$", "->"};
        string[] separateTokensSecondPass = new[] { "<", ">", "=", "&", "|", ".", "-" };

        public void InitializeParsers() {
            //Ignored words that have no command parameters
            AddWords(Words("the", "than", "turned", "block", "panel", "to", "from", "then", "of", "either", "for", "in"), new IgnoreCommandParameter());

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
            AddWords(Words("increase", "raise", "extend", "expand"), new AssignmentCommandParameter(), new DirectionCommandParameter(Direction.UP));
            AddWords(Words("add"), new AssignmentCommandParameter(), new RelativeCommandParameter(), new DirectionCommandParameter(Direction.UP));
            AddWords(Words("subtact"), new AssignmentCommandParameter(), new RelativeCommandParameter(), new DirectionCommandParameter(Direction.DOWN));
            AddWords(Words("decrease", "retract", "reduce"), new AssignmentCommandParameter(), new DirectionCommandParameter(Direction.DOWN));
            AddWords(Words("reverse"), new ReverseCommandParameter());
            AddWords(Words("by"), new RelativeCommandParameter());
            AddWords(Words("global"), new GlobalCommandParameter());

            //Value Words
            AddWords(Words("on", "begin", "true", "start", "started", "resume", "resumed"), new BooleanCommandParameter(true));
            AddWords(Words("off", "terminate", "cancel", "end", "false", "stopped", "halt", "halted"), new BooleanCommandParameter(false));

            //Property Words
            AddPropertyWords(Words("height", "length", "level"), Property.LEVEL);
            AddPropertyWords(Words("angle"), Property.ANGLE);
            AddPropertyWords(Words("speed", "velocity", "rate", "pace"), Property.VELOCITY);
            AddPropertyWords(Words("connect", "join", "attach", "connected", "joined", "attached", "dock", "docked", "docking"), Property.CONNECTED);
            AddPropertyWords(Words("disconnect", "separate", "detach", "disconnected", "separated", "detached", "undock", "undocked"), Property.CONNECTED, false);
            AddPropertyWords(Words("lock", "locked", "freeze", "brake", "handbrake"), Property.LOCKED);
            AddPropertyWords(Words("unlock", "unlocked", "unfreeze"), Property.LOCKED, false);
            AddPropertyWords(Words("run", "running", "execute", "executing"), Property.RUN);
            AddPropertyWords(Words("done", "ready", "complete", "finished", "built", "finish"), Property.COMPLETE);
            AddPropertyWords(Words("open", "opened"), Property.OPEN);
            AddPropertyWords(Words("close", "closed", "shut"), Property.OPEN, false);
            AddPropertyWords(Words("fontsize", "size"), Property.FONT_SIZE);
            AddPropertyWords(Words("text", "message"), Property.TEXT);
            AddPropertyWords(Words("color"), Property.COLOR);
            AddPropertyWords(Words("power", "powered"), Property.POWER);
            AddPropertyWords(Words("enable", "enabled", "arm", "armed"), Property.ENABLE);
            AddPropertyWords(Words("disable", "disabled", "disarm", "disarmed"), Property.ENABLE, false);
            AddPropertyWords(Words("music", "song"), Property.SONG);
            AddPropertyWords(Words("silent", "silence"), Property.SILENCE);
            AddPropertyWords(Words("volume", "intensity", "output"), Property.VOLUME);
            AddPropertyWords(Words("range", "distance", "limit", "radius", "capacity", "delay"), Property.RANGE);
            AddPropertyWords(Words("blinkinterval", "interval"), Property.BLINK_INTERVAL);
            AddPropertyWords(Words("blinklength"), Property.BLINK_LENGTH);
            AddPropertyWords(Words("blinkoffset", "offset"), Property.OFFSET);
            AddPropertyWords(Words("falloff"), Property.FALLOFF);
            AddPropertyWords(Words("trigger", "triggered", "trip", "tripped", "deploy", "deployed", "shoot", "shooting", "shot", "detonate"), Property.TRIGGER);
            AddPropertyWords(Words("pressure", "pressurize", "pressurized", "supply", "supplying", "generate", "discharge", "discharging", "broadcast", "broadcasting"), Property.SUPPLY);
            AddPropertyWords(Words("stockpile", "depressurize", "depressurized", "gather", "intake", "recharge", "recharging", "consume", "consuming", "collect", "collecting"), Property.SUPPLY, false);
            AddPropertyWords(Words("ratio", "percentage", "percent", "progress", "completion"), Property.RATIO);
            AddPropertyWords(Words("input", "pilot", "user"), Property.INPUT);
            AddPropertyWords(Words("roll", "rollInput"), Property.ROLL_INPUT);
            AddPropertyWords(Words("auto", "refill", "drain", "draining"), Property.AUTO);
            AddPropertyWords(Words("override", "overrides", "overridden"), Property.OVERRIDE);
            AddPropertyWords(Words("direction"), Property.DIRECTION);
            AddPropertyWords(Words("coordinates", "position", "location"), Property.POSITION);
            AddPropertyWords(Words("target", "destination", "waypoint"), Property.TARGET);
            AddPropertyWords(Words("waypoints"), Property.WAYPOINTS);
            AddPropertyWords(Words("targetvelocity"), Property.TARGET_VELOCITY);
            AddPropertyWords(Words("strength", "force", "gravity", "torque"), Property.STRENGTH);
            AddPropertyWords(Words("countdown"), Property.COUNTDOWN);
            AddPropertyWords(Words("name", "label"), Property.NAME);
            AddPropertyWords(Words("show", "showing"), Property.SHOW);
            AddPropertyWords(Words("hide", "hiding"), Property.SHOW, false);
            AddPropertyWords(Words("properties", "attributes"), Property.PROPERTIES);
            AddPropertyWords(Words("actions"), Property.ACTIONS);

            //ValueProperty Words
            AddWords(Words("amount"), new ValuePropertyCommandParameter(ValueProperty.AMOUNT));
            AddWords(Words("property", "attribute"), new ValuePropertyCommandParameter(ValueProperty.PROPERTY));
            AddWords(Words("action"), new ValuePropertyCommandParameter(ValueProperty.ACTION));
            AddWords(Words("assemble", "assembling", "produce", "producing", "create", "creating", "build", "building"), new ValuePropertyCommandParameter(ValueProperty.CREATE));
            AddWords(Words("disassemble", "disassembling", "destroy", "destroying", "recycle", "recycling"), new ValuePropertyCommandParameter(ValueProperty.DESTROY));

            //Special Command Words
            AddWords(Words("times", "iterations"), new IteratorCommandParameter());
            AddWords(Words("wait", "hold"), new WaitCommandParameter());
            AddWords(Words("call", "gosub"), new FunctionCommandParameter(Function.GOSUB));
            AddWords(Words("goto"), new FunctionCommandParameter(Function.GOTO));
            AddWords(Words("listen", "channel"), new ListenCommandParameter());
            AddWords(Words("send"), new SendCommandParameter());
            AddWords(Words("print", "log", "echo", "write"), new PrintCommandParameter());
            AddWords(Words("queue", "schedule"), new QueueCommandParameter(false));
            AddWords(Words("async", "background", "parallel"), new QueueCommandParameter(true));
            AddWords(Words("transfer", "give"), new TransferCommandParameter(true));
            AddWords(Words("take"), new TransferCommandParameter(false));
            AddWords(Words("->"), new KeyedVariableCommandParameter());

            //Conditional Words
            AddWords(Words("if"), new IfCommandParameter(false, false, false));
            AddWords(Words("unless"), new IfCommandParameter(true, false, false));
            AddWords(Words("while"), new IfCommandParameter(false, true, false));
            AddWords(Words("until"), new IfCommandParameter(true, true, false));
            AddWords(Words("when"), new IfCommandParameter(true, true, true));
            AddWords(Words("else", "otherwise"), new ElseCommandParameter());
            AddWords(Words("that", "which", "whose"), new ThatCommandParameter());

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
            AddWords(Words("keys", "indexes"), new LeftUniOperationCommandParameter(UniOperand.KEYS));
            AddWords(Words("values"), new LeftUniOperationCommandParameter(UniOperand.VALUES));
            AddWords(Words("multiply", "*"), new BiOperandTier1Operand(BiOperand.MULTIPLY));
            AddWords(Words("divide", "/"), new BiOperandTier1Operand(BiOperand.DIVIDE));
            AddWords(Words("mod", "%"), new BiOperandTier1Operand(BiOperand.MOD));
            AddWords(Words("dot", "."), new BiOperandTier1Operand(BiOperand.DOT));
            AddWords(Words("as", "cast"), new BiOperandTier1Operand(BiOperand.CAST));
            AddWords(Words("pow", "exp", "^"), new BiOperandTier1Operand(BiOperand.EXPONENT));
            AddWords(Words("plus", "+"), new BiOperandTier2Operand(BiOperand.ADD));
            AddWords(Words("minus", "-"), new BiOperandTier2Operand(BiOperand.SUBTACT));
            AddWords(Words(".."), new BiOperandTier3Operand(BiOperand.RANGE));
            AddWords(Words("@"), new IndexCommandParameter());

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
            AddBlockWords(Words("cargo", "container", "inventory"), Words("containers", "inventories"), Block.CARGO);
            AddBlockWords(Words("warhead", "bomb"), Block.WARHEAD);
            AddBlockWords(Words("assembler"), Block.ASSEMBLER);
            AddBlockWords(Words("collector"), Block.COLLECTOR);
            AddBlockWords(Words("ejector"), Block.EJECTOR);
            AddBlockWords(Words("decoy"), Block.DECOY);
            AddBlockWords(Words("jumpdrive"), Block.JUMPDRIVE);
            AddBlockWords(Words("laser", "laserAntenna"), Block.LASER_ANTENNA);
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
            RegisterToString<FunctionDefinitionCommandParameter>(p => "Function[type=" + p.functionType + ",name=" + p.functionDefinition.functionName + "]");
            RegisterToString<ConditionCommandParameter>(p => "[Condition]");
            RegisterToString<BlockConditionCommandParameter>(p => "[BlockCondition]");
            RegisterToString<CommandReferenceParameter>(p => "[Command]");
            RegisterToString<IterationCommandParameter>(p => "[Iteration]");
            RegisterToString<SelectorCommandParameter>(p => "[Selector]");
            RegisterToString<ThatCommandParameter>(p => "That");
            RegisterToString<AssignmentCommandParameter>(p => "[Action]");
            RegisterToString<PropertyCommandParameter>(p => "Property[id=" + p.value.propertyType +"]");
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

        void AddPropertyWords(String[] words, Property property, bool nonNegative = true) {
            if (!nonNegative) AddWords(words, new PropertyCommandParameter(property), new BooleanCommandParameter(false));
            else AddWords(words, new PropertyCommandParameter(property));
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

        //Taken shamelessly from https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        public List<Token> ParseTokens(String commandString) {
            if (String.IsNullOrWhiteSpace(commandString) || commandString.Trim().StartsWith("#")) return NewList<Token>();

            List<Token> singleQuoteTokens = commandString.Trim().Split('\'')
            .SelectMany((element, index) => index % 2 == 0  // If even index
                ? ParseDoubleQuotes(element)  // Split the item
                : new Token[] { new Token(element, true, true) })  // Keep the entire item
            .ToList();

            return singleQuoteTokens;
        }

        Token[] ParseDoubleQuotes(String commandString) =>
            commandString.Trim().Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(ParseSeparateTokens)
                    .Select(t => new Token(t, false, false))  // Split the item
                    : new Token[] { new Token(element, true, false) })  // Keep the entire item
                .SelectMany(element => element)
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
