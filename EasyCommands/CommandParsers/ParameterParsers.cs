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

namespace IngameScript
{
    partial class Program
    {
        //Configuration.  Keep all words lowercase
        static String[] ignoreWords = { "the", "than", "turn", "turned", "rotate", "set", "is", "block", "tell", "to", "from", "then", "of", "either", "are" };
        static String[] groupWords = { "blocks", "group" };
        static String[] activateWords = { "move", "go", "on", "begin" };
        static String[] deactivateWords = { "off", "terminate", "exit", "cancel", "end" };
        static String[] reverseWords = { "reverse", "switch direction", "turn around" };
        static String[] increaseWords = { "increase", "raise", "extend", "expand", "forward", "forwards", "up" };
        static String[] decreaseWords = { "decrease", "lower", "retract", "reduce", "backward", "backwards", "down" };
        static String[] clockwiseWords = { "clockwise", "clock" };
        static String[] counterclockwiseWords = { "counter", "counterclock", "counterclockwise" };

        static String[] relativeWords = { "by" };
        static String[] increaseRelativeWords = { "add" };
        static String[] decreaseRelativeWords = { "subtact" };
        static String[] heightWords = { "height", "length" };
        static String[] angleWords = { "angle" };
        static String[] speedWords = { "speed", "velocity", "rate", "pace" };
        static String[] waitWords = { "wait", "hold" };
        static String[] connectWords = { "connect", "join", "attach", "connected", "joined", "attached" };
        static String[] disconnectWords = { "disconnect", "separate", "detach", "disconnected", "separated", "detached" };

        static String[] lockWords = { "lock", "freeze" };
        static String[] unlockWords = { "unlock", "unfreeze" };

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

        static String[] openWords = { "open", "opened"};
        static String[] closeWords = { "close", "closed", "shut" };

        static String[] gosubKeywords = { "call", "gosub" };
        static String[] gotoKeywords = { "goto" };

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
            { "hangers", BlockType.DOOR },
            { "bays", BlockType.DOOR },
            { "gates", BlockType.DOOR },
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
            { "hanger", BlockType.DOOR },
            { "bay", BlockType.DOOR },
            { "gate", BlockType.DOOR },
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

        //Internal (Don't touch!)
        private static Dictionary<String, List<CommandParameter>> propertyWords = new Dictionary<string, List<CommandParameter>>();

        public static void initParsers()
        {
            AddWords(groupWords, new GroupCommandParameter());
            AddWords(activateWords, new BooleanCommandParameter(true));
            AddWords(deactivateWords, new BooleanCommandParameter(false));
            AddWords(increaseWords, new DirectionCommandParameter(DirectionType.UP));
            AddWords(increaseRelativeWords, new RelativeCommandParameter(), new DirectionCommandParameter(DirectionType.UP));
            AddWords(decreaseWords, new DirectionCommandParameter(DirectionType.DOWN));
            AddWords(decreaseRelativeWords, new RelativeCommandParameter(), new DirectionCommandParameter(DirectionType.DOWN));
            AddWords(clockwiseWords, new DirectionCommandParameter(DirectionType.CLOCKWISE));
            AddWords(counterclockwiseWords, new DirectionCommandParameter(DirectionType.COUNTERCLOCKWISE));
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
            AddWords(progressWords, new NumericPropertyCommandParameter(NumericPropertyType.PROGRESS));
            AddWords(openWords, new BooleanPropertyCommandParameter(BooleanPropertyType.OPEN), new BooleanCommandParameter(true));
            AddWords(closeWords, new BooleanPropertyCommandParameter(BooleanPropertyType.OPEN), new BooleanCommandParameter(false));
            AddWords(gosubKeywords, new FunctionCommandParameter(FunctionType.GOSUB));
            AddWords(gotoKeywords, new FunctionCommandParameter(FunctionType.GOTO));
        }

        static void AddWords(String[] words, params CommandParameter[] commands)
        {
            foreach (String word in words) propertyWords.Add(word, commands.ToList());
        }

        static List<CommandParameter> ParseCommandParameters(List<Token> tokens)
        {
            Print("Command: " + String.Join(" | ", tokens));

            List<CommandParameter> commandParameters = new List<CommandParameter>();
            foreach (var token in tokens)
            {
                String t = token.token;

                if (token.isString) {
                    List<Token> subTokens = ParseTokens(t);
                    List<CommandParameter> subtokenParams = ParseCommandParameters(subTokens);
                    commandParameters.Add(new StringCommandParameter(t, subtokenParams.ToArray()));
                    continue;
                }

                if (ignoreWords.Contains(t)) continue;

                if (propertyWords.ContainsKey(t))
                {
                    commandParameters.AddList(propertyWords[t]);
                    continue;
                }

                ControlType controlType;
                if (controlTypeWords.TryGetValue(t, out controlType))
                {
                    commandParameters.Add(new ControlCommandParameter(controlType));
                    continue;
                }

                BlockType blockType;
                if (blockTypeGroupWords.TryGetValue(t, out blockType))
                {
                    commandParameters.Add(new BlockTypeCommandParameter(blockType));
                    commandParameters.Add(new GroupCommandParameter());
                    continue;
                }
                else if (blockTypeWords.TryGetValue(t, out blockType))
                {
                    commandParameters.Add(new BlockTypeCommandParameter(blockType));
                    continue;
                }

                UnitType unitType;
                if (unitTypeWords.TryGetValue(t, out unitType))
                {
                    commandParameters.Add(new UnitCommandParameter(unitType));
                    continue;
                }

                double numericValue;
                if (Double.TryParse(t, out numericValue))
                {
                    commandParameters.Add(new NumericCommandParameter((float)numericValue));
                    continue;
                }

                //If nothing else matches, must be a string
                commandParameters.Add(new StringCommandParameter(t));
            }
            return commandParameters;
        }

        //Taken shamelessly from https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        public static List<Token> ParseTokens(String commandString)
        {
            return commandString.Trim().Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(t => new Token(t, false))  // Split the item
                                    : new Token[] { new Token(element, true) })  // Keep the entire item
                .SelectMany(element => element)
                .ToList();
        }

        public class Token
        {
            public String token;
            public bool isString;

            public Token(string token, bool isString)
            {
                this.isString = isString;
                this.token = token.ToLower();
            }

            public override string ToString() { return token; }
        }
    }
}
