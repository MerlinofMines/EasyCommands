﻿using Sandbox.Game.EntityComponents;
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
        private Dictionary<String, List<CommandParameter>> propertyWords = new Dictionary<string, List<CommandParameter>>();

        public void InitializeParsers() {
            //Ignored words that have no command parameters
            AddWords(Words("the", "than", "turned", "block", "to", "from", "then", "of", "either", "for"));

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
            AddWords(Words("on", "begin", "true"), new BooleanCommandParameter(true));
            AddWords(Words("off", "terminate", "cancel", "end", "false"), new BooleanCommandParameter(false));

            //Property Words
            AddWords(Words("height", "length"), new PropertyCommandParameter(Property.HEIGHT));
            AddWords(Words("angle"), new PropertyCommandParameter(Property.ANGLE));
            AddWords(Words("speed", "velocity", "rate", "pace"), new PropertyCommandParameter(Property.VELOCITY));
            AddWords(Words("connect", "join", "attach", "connected", "joined", "attached", "dock", "docked"), new PropertyCommandParameter(Property.CONNECTED));
            AddWords(Words("disconnect", "separate", "detach", "disconnected", "separated", "detached", "undock", "undocked"), new PropertyCommandParameter(Property.CONNECTED), new BooleanCommandParameter(false));
            AddWords(Words("lock", "locked", "freeze"), new PropertyCommandParameter(Property.LOCKED));
            AddWords(Words("unlock", "unlocked", "unfreeze"), new PropertyCommandParameter(Property.LOCKED), new BooleanCommandParameter(false));
            AddWords(Words("run", "execute"), new PropertyCommandParameter(Property.RUN));
            AddWords(Words("running", "executing"), new PropertyCommandParameter(Property.RUNNING));
            AddWords(Words("stopped", "terminated"), new PropertyCommandParameter(Property.STOPPED));
            AddWords(Words("paused", "halted"), new PropertyCommandParameter(Property.PAUSED));
            AddWords(Words("done", "complete", "finished", "built"), new PropertyCommandParameter(Property.COMPLETE));
            AddWords(Words("progress", "completion"), new PropertyCommandParameter(Property.RATIO));
            AddWords(Words("open", "opened"), new PropertyCommandParameter(Property.OPEN), new BooleanCommandParameter(true));
            AddWords(Words("close", "closed", "shut"), new PropertyCommandParameter(Property.OPEN), new BooleanCommandParameter(false));
            AddWords(Words("fontsize", "size"), new PropertyCommandParameter(Property.FONT_SIZE));
            AddWords(Words("text", "message"), new PropertyCommandParameter(Property.TEXT));
            AddWords(Words("color"), new PropertyCommandParameter(Property.COLOR));
            AddWords(Words("power"), new PropertyCommandParameter(Property.POWER));
            AddWords(Words("music", "song"), new PropertyCommandParameter(Property.SOUND));
            AddWords(Words("volume"), new PropertyCommandParameter(Property.VOLUME));
            AddWords(Words("range", "distance", "limit", "radius"), new PropertyCommandParameter(Property.RANGE));
            AddWords(Words("blinkinterval", "blinkInterval", "interval"), new PropertyCommandParameter(Property.BLINK_INTERVAL));
            AddWords(Words("blinklength", "blinkLength"), new PropertyCommandParameter(Property.BLINK_LENGTH));
            AddWords(Words("blinkoffset", "blinkOffset"), new PropertyCommandParameter(Property.BLINK_OFFSET));
            AddWords(Words("intensity"), new PropertyCommandParameter(Property.INTENSITY));
            AddWords(Words("falloff"), new PropertyCommandParameter(Property.FALLOFF));
            AddWords(Words("times", "iterations"), new IteratorCommandParameter());
            AddWords(Words("trigger", "triggered", "trip", "tripped", "deploy", "deployed", "shoot", "shooting", "shot"), new PropertyCommandParameter(Property.TRIGGER));
            AddWords(Words("produce", "pressurize", "pressurized", "supply", "generate", "discharge", "discharging"), new PropertyCommandParameter(Property.PRODUCE));
            AddWords(Words("consume", "stockpile", "depressurize", "depressurized", "gather", "intake", "recharge", "recharging"), new PropertyCommandParameter(Property.PRODUCE), new BooleanCommandParameter(false));
            AddWords(Words("ratio", "percentage", "percent"), new PropertyCommandParameter(Property.RATIO));
            AddWords(Words("input", "pilot", "user"), new PropertyCommandParameter(Property.MOVE_INPUT));
            AddWords(Words("roll", "rollInput"), new PropertyCommandParameter(Property.ROLL_INPUT));
            AddWords(Words("auto", "refill", "drain", "draining"), new PropertyCommandParameter(Property.AUTO));
            AddWords(Words("override", "overrides", "overridden"), new PropertyCommandParameter(Property.OVERRIDE));
            AddWords(Words("direction"), new PropertyCommandParameter(Property.DIRECTION));
            AddWords(Words("coordinates", "position", "location"), new PropertyCommandParameter(Property.POSITION));
            AddWords(Words("target", "destination", "waypoint"), new PropertyCommandParameter(Property.TARGET));
            AddWords(Words("targetvelocity"), new PropertyCommandParameter(Property.TARGET_VELOCITY));
            AddWords(Words("strength", "force", "gravity"), new PropertyCommandParameter(Property.STRENGTH));

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
            AddWords(Words("not", "!", "isn't", "isnt", "aren't", "arent"), new NotCommandParameter());
            AddWords(Words("absolute", "abs"), new UniOperationCommandParameter(UniOperand.ABS));
            AddWords(Words("sqrt"), new UniOperationCommandParameter(UniOperand.SQRT));
            AddWords(Words("sin"), new UniOperationCommandParameter(UniOperand.SIN));
            AddWords(Words("cosine", "cos"), new UniOperationCommandParameter(UniOperand.COS));
            AddWords(Words("tangent", "tan"), new UniOperationCommandParameter(UniOperand.TAN));
            AddWords(Words("arcsin", "asin"), new UniOperationCommandParameter(UniOperand.ASIN));
            AddWords(Words("arcos", "acos"), new UniOperationCommandParameter(UniOperand.ACOS));
            AddWords(Words("arctan", "atan"), new UniOperationCommandParameter(UniOperand.ATAN));
            AddWords(Words("round", "rnd"), new UniOperationCommandParameter(UniOperand.ROUND));
            AddWords(Words("plus", "+"), new AddCommandParameter(BiOperand.ADD));
            AddWords(Words("minus", "-"), new AddCommandParameter(BiOperand.SUBTACT));
            AddWords(Words("multiply", "*"), new MultiplyCommandParameter(BiOperand.MULTIPLY));
            AddWords(Words("divide", "/"), new MultiplyCommandParameter(BiOperand.DIVIDE));
            AddWords(Words("mod", "%"), new MultiplyCommandParameter(BiOperand.MOD));
            AddWords(Words("dot", "."), new MultiplyCommandParameter(BiOperand.DOT));

            //Unit Words
            AddWords(Words("second", "seconds"), new UnitCommandParameter(Unit.SECONDS));
            AddWords(Words("tick", "ticks"), new UnitCommandParameter(Unit.TICKS));
            AddWords(Words("degree", "degrees"), new UnitCommandParameter(Unit.DEGREES));
            AddWords(Words("meter", "meters"), new UnitCommandParameter(Unit.METERS));
            AddWords(Words("rpm"), new UnitCommandParameter(Unit.RPM));

            //Control Types
            AddWords(Words("start", "resume"), new ControlCommandParameter(Control.START));
            AddWords(Words("restart", "reset", "reboot"), new ControlCommandParameter(Control.RESTART));
            AddWords(Words("repeat", "loop", "rerun", "replay"), new ControlCommandParameter(Control.REPEAT));
            AddWords(Words("stop", "halt", "exit"), new ControlCommandParameter(Control.STOP));
            AddWords(Words("pause"), new ControlCommandParameter(Control.PAUSE));

            //Blocks
            AddBlockWords(Words("piston"), Block.PISTON);
            AddBlockWords(Words("light"), Block.LIGHT);
            AddBlockWords(Words("rotor", "hinge"), Block.ROTOR);
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
            AddBlockWords(Words("engine"), Block.ENGINE);
            AddBlockWords(Words("sorter"), Block.SORTER);
            AddBlockWords(Words("gyro", "gyroscopes"), Block.GYROSCOPE);
            AddBlockWords(Words("gravitygenerator"), Block.GRAVITY_GENERATOR);
            AddBlockWords(Words("gravitysphere"), Block.GRAVITY_SPHERE);

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
            } else if (t.StartsWith("[") && t.EndsWith("]")) { //Variable References used as Selectors
                commandParameters.Add(new VariableSelectorCommandParameter(new InMemoryVariable(token.original.Substring(1, token.original.Length - 2))));
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
                    .SelectMany(ParseParenthesis)
                    .Select(t => new Token(t, false, false))  // Split the item
                    : new Token[] { new Token(element, true, false) })  // Keep the entire item
                .SelectMany(element => element)
                .ToArray();
        }

        String[] ParseParenthesis(String command) {
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
