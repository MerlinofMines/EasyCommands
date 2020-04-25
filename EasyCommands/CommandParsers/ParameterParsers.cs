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
        String[] ignoreWords = { "the", "than", "turn", "turned", "rotate", "set", "is", "block", "tell", "to", "from", "then", "of", "either" };
        String[] groupWords = { "blocks", "group" };
        String[] activateWords = { "move", "go", "on", "start", "begin" };
        String[] deactivateWords = { "stop", "off", "terminate", "exit", "cancel", "end" };
        String[] reverseWords = { "reverse", "switch direction", "turn around" };
        String[] increaseWords = { "increase", "raise", "extend", "expand", "forward", "forwards", "up" };
        String[] decreaseWords = { "decrease", "lower", "retract", "reduce", "backward", "backwards", "down" };
        String[] clockwiseWords = { "clockwise", "clock" };
        String[] counterclockwiseWords = { "counter", "counterclock", "counterclockwise" };

        String[] relativeWords = { "by" };
        String[] increaseRelativeWords = { "add" };
        String[] decreaseRelativeWords = { "subtact" };
        String[] speedWords = { "speed", "velocity", "rate", "pace" };
        String[] waitWords = { "wait", "hold", "pause" };
        String[] connectWords = { "connect", "join", "attach", "connected", "joined", "attached" };
        String[] disconnectWords = { "disconnect", "separate", "detach", "disconnected", "separated", "detached" };

        String[] lockWords = { "lock", "freeze" };
        String[] unlockWords = { "unlock", "unfreeze" };

        String[] ifWords = { "if" };
        String[] elseWords = { "else", "otherwise" };
        String[] unlessWords = { "unless" };
        String[] whileWords = { "while" };
        String[] untilWords = { "until" };
        String[] whenWords = { "when" };
        String[] asyncWords = { "async" };

        String[] lessWords = { "less", "<", "below" };
        String[] lessEqualWords = { "<=" };
        String[] equalWords = { "equal", "equals", "=", "==" };
        String[] greaterEqualWords = { ">=" };
        String[] greaterWords = { "greater", ">", "above", "more" };

        String[] anyWords = { "any" };
        String[] allWords = { "all" };
        String[] noneWords = { "none" };

        String[] andWords = { "and", "&", "&&", "but", "yet" };
        String[] orWords = { "or", "|", "||" };
        String[] notWords = { "not", "!", "isn't", "isnt" };
        String[] openParenthesisWords = { "(" };
        String[] closeParenthesisWords = { ")" };

        String[] restartWords = { "restart", "reset", "reboot" };

        String[] runWords = { "run", "execute" };
        String[] runningWords = { "running", "executing" };

        String[] completeWords = { "done", "complete", "finished", "built" };
        String[] progressWords = { "progress", "completion" };

        String[] loopWords = { "loop", "iterate", "repeat", "rerun", "replay" };

        Dictionary<String, UnitType> unitTypeWords = new Dictionary<String, UnitType>()
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

        Dictionary<String, BlockType> blockTypeGroupWords = new Dictionary<String, BlockType>() {
            { "pistons", BlockType.PISTON },
            { "lights", BlockType.LIGHT },
            { "rotors", BlockType.ROTOR },
            { "programs", BlockType.PROGRAM },
            { "timers", BlockType.TIMER },
            { "projectors", BlockType.PROJECTOR },
            { "connectors", BlockType.CONNECTOR },
            { "welders", BlockType.WELDER },
            { "grinders", BlockType.GRINDER }
        };

        Dictionary<String, BlockType> blockTypeWords = new Dictionary<String, BlockType>() {
            { "piston", BlockType.PISTON },
            { "light", BlockType.LIGHT },
            { "rotor", BlockType.ROTOR },
            { "program", BlockType.PROGRAM },
            { "timer", BlockType.TIMER },
            { "projector", BlockType.PROJECTOR },
            { "merge", BlockType.MERGE },
            { "connector", BlockType.CONNECTOR },
            { "welder", BlockType.WELDER },
            { "grinder", BlockType.GRINDER }
        };

        //Internal (Don't touch!)
        private Dictionary<String, List<CommandParameter>> propertyWords = new Dictionary<string, List<CommandParameter>>();

        public void initParsers()
        {
            addWords(groupWords, new GroupCommandParameter());
            addWords(activateWords, new BooleanCommandParameter(true));
            addWords(deactivateWords, new BooleanCommandParameter(false));
            addWords(increaseWords, new DirectionCommandParameter(DirectionType.UP));
            addWords(increaseRelativeWords, new RelativeCommandParameter(), new DirectionCommandParameter(DirectionType.UP));
            addWords(decreaseWords, new DirectionCommandParameter(DirectionType.DOWN));
            addWords(decreaseRelativeWords, new RelativeCommandParameter(), new DirectionCommandParameter(DirectionType.DOWN));
            addWords(clockwiseWords, new DirectionCommandParameter(DirectionType.CLOCKWISE));
            addWords(counterclockwiseWords, new DirectionCommandParameter(DirectionType.COUNTERCLOCKWISE));
            addWords(reverseWords, new ReverseCommandParameter());
            addWords(relativeWords, new RelativeCommandParameter());
            addWords(speedWords, new NumericPropertyCommandParameter(NumericPropertyType.VELOCITY));
            addWords(connectWords, new BooleanPropertyCommandParameter(BooleanPropertyType.CONNECTED));
            addWords(disconnectWords, new BooleanPropertyCommandParameter(BooleanPropertyType.CONNECTED), new BooleanCommandParameter(false));
            addWords(lockWords, new BooleanPropertyCommandParameter(BooleanPropertyType.LOCKED));
            addWords(unlockWords, new BooleanPropertyCommandParameter(BooleanPropertyType.LOCKED), new BooleanCommandParameter(false));
            addWords(waitWords, new WaitCommandParameter());
            addWords(ifWords, new IfCommandParameter(false, false, false));
            addWords(unlessWords, new IfCommandParameter(true, false, false));
            addWords(whileWords, new IfCommandParameter(false, true, false));
            addWords(untilWords, new IfCommandParameter(true, true, false));
            addWords(whenWords, new IfCommandParameter(true, true, true));
            addWords(asyncWords, new AsyncCommandParameter());
            addWords(elseWords, new ElseCommandParameter());
            addWords(lessWords, new ComparisonCommandParameter(ComparisonType.LESS));
            addWords(lessEqualWords, new ComparisonCommandParameter(ComparisonType.LESS_OR_EQUAL));
            addWords(equalWords, new ComparisonCommandParameter(ComparisonType.EQUAL));
            addWords(greaterEqualWords, new ComparisonCommandParameter(ComparisonType.GREATER_OR_EQUAL));
            addWords(greaterWords, new ComparisonCommandParameter(ComparisonType.GREATER));
            addWords(anyWords, new AggregationModeCommandParameter(AggregationMode.ANY));
            addWords(allWords, new AggregationModeCommandParameter(AggregationMode.ALL));
            addWords(noneWords, new AggregationModeCommandParameter(AggregationMode.NONE));
            addWords(andWords, new AndCommandParameter());
            addWords(orWords, new OrCommandParameter());
            addWords(notWords, new NotCommandParameter());
            addWords(openParenthesisWords, new OpenParenthesisCommandParameter());
            addWords(closeParenthesisWords, new CloseParenthesisCommandParameter());
            addWords(restartWords, new RestartCommandParameter());
            addWords(runWords, new StringPropertyCommandParameter(StringPropertyType.RUN));
            addWords(runningWords, new BooleanPropertyCommandParameter(BooleanPropertyType.RUNNING));
            addWords(completeWords, new BooleanPropertyCommandParameter(BooleanPropertyType.COMPLETE));
            addWords(progressWords, new NumericPropertyCommandParameter(NumericPropertyType.PROGRESS));
            addWords(loopWords, new LoopCommandParameter());
        }

        void addWords(String[] words, params CommandParameter[] commands)
        {
            foreach (String word in words) propertyWords.Add(word, commands.ToList());
        }

        List<CommandParameter> parseCommandParameters(List<String> tokens)
        {
            Print("Command: " + String.Join(" | ", tokens));

            List<CommandParameter> commandParameters = new List<CommandParameter>();
            foreach (var token in tokens)
            {
                if (ignoreWords.Contains(token)) continue;

                if (propertyWords.ContainsKey(token))
                {
                    commandParameters.AddList(propertyWords[token]);
                    continue;
                }

                BlockType blockType;
                if (blockTypeGroupWords.TryGetValue(token, out blockType))
                {
                    commandParameters.Add(new BlockTypeCommandParameter(blockType));
                    commandParameters.Add(new GroupCommandParameter());
                    continue;
                }
                else if (blockTypeWords.TryGetValue(token, out blockType))
                {
                    commandParameters.Add(new BlockTypeCommandParameter(blockType));
                    continue;
                }

                UnitType unitType;
                if (unitTypeWords.TryGetValue(token, out unitType))
                {
                    commandParameters.Add(new UnitCommandParameter(unitType));
                    continue;
                }

                double numericValue;
                if (Double.TryParse(token, out numericValue))
                {
                    commandParameters.Add(new NumericCommandParameter((float)numericValue));
                    continue;
                }

                //If nothing else matches, must be a string
                List<String> subTokens = parseTokens(token);

                if (subTokens.Count > 1) commandParameters.Add(new StringCommandParameter(token, parseCommandParameters(subTokens).ToArray()));
                else commandParameters.Add(new StringCommandParameter(token));
            }
            return commandParameters;
        }
    }
}
