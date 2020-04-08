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
    partial class Program : MyGridProgram 
    {
        //Debug
        private static UpdateFrequency UPDATE_FREQUENCY = UpdateFrequency.Update1;

        //Configuration.  Keep all words lowercase
        private String[] ignoreWords = { "the", "than", "turn", "turned", "rotate", "set", "is", "block", "tell", "to", "from", "then", "of" };
        private String[] groupWords = { "blocks", "group" };
        private String[] activateWords = { "move", "go", "on", "start", "begin" };
        private String[] deactivateWords = { "stop", "off", "terminate", "exit", "cancel", "end" };
        private String[] reverseWords = { "reverse", "switch direction", "turn around" };
        private String[] increaseWords = { "increase", "raise", "extend", "expand", "forward", "forwards", "up" };
        private String[] decreaseWords = { "decrease", "lower", "retract", "reduce", "backward", "backwards", "down" };
        private String[] clockwiseWords = { "clockwise", "clock" };
        private String[] counterclockwiseWords = { "counter", "counterclock", "counterclockwise" };

        private String[] relativeWords = { "by" };
        private String[] increaseRelativeWords = { "add" };
        private String[] decreaseRelativeWords = { "subtact" };
        private String[] speedWords = { "speed", "velocity", "rate", "pace" };
        private String[] waitWords = { "wait", "hold", "pause" };
        private String[] connectWords = { "connect", "join", "attach" };
        private String[] disconnectWords = { "disconnect", "separate", "detach" };

        private String[] lockWords = { "lock", "freeze" };
        private String[] unlockWords = { "unlock", "unfreeze" };

        private String[] ifWords = { "if" };
        private String[] elseWords = { "else", "otherwise" };
        private String[] unlessWords = { "unless" };
        private String[] whileWords = { "while" };
        private String[] untilWords = { "until" };
        private String[] whenWords = { "when" };
        private String[] asyncWords = { "async" };

        private String[] lessWords = { "less", "<", "below" };
        private String[] lessEqualWords = { "<=" };
        private String[] equalWords = { "equal", "equals", "=", "==" };
        private String[] greaterEqualWords = { ">=" };
        private String[] greaterWords = { "greater", ">", "above", "more" };

        private String[] anyWords = { "any" };
        private String[] allWords = { "all" };
        private String[] noneWords = { "none" };

        private Dictionary<String, UnitType> unitTypeWords = new Dictionary<String, UnitType>()
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

        //Internal (Don't touch!)
        private Dictionary<String, List<CommandParameter>> propertyWords = new Dictionary<string, List<CommandParameter>>();

        List<Command> runningCommands = new List<Command>();

        public Program()
        {
            addWords(groupWords, new GroupCommandParameter());
            addWords(activateWords, new BooleanCommandParameter(true));
            addWords(deactivateWords, new BooleanCommandParameter(false));
            addWords(increaseWords, new DirectionCommandParameter(DirectionType.UP));
            addWords(decreaseWords, new DirectionCommandParameter(DirectionType.DOWN));
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
        }

        public void addWords(String[] words, params CommandParameter[] commands)
        {
            foreach (String word in words) propertyWords.Add(word, commands.ToList<CommandParameter>());
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (String.IsNullOrEmpty(argument))
            {
                if (execute())
                {
                    Echo("Execution Complete");
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                } else
                {
                    Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                }
            } else if (argument.ToLower() == "restart") //Clear existing commands and start fresh
            {
                Echo("Restarting Commands");
                runningCommands = new List<Command>();
                addCommands();
            }
            else if (argument.ToLower() == "start") //Add more commands to end of running list.  Useful to say "execute me 3 times, please")
            {
                Echo("Starting Commands");
                addCommands();
            }
        }

        private void addCommands()
        {
            Runtime.UpdateFrequency = UPDATE_FREQUENCY;
            List<Command> commands = parseCommands();

            foreach (var command in commands) {
                Echo("Resolved Command: " + command.GetType());
            }

            runningCommands.AddList(commands);

        }

        private bool execute()
        {
            Echo("Executing Commands");
            if (runningCommands.Count == 0) return true;

            int commandIndex = 0;

            while (commandIndex < runningCommands.Count)
            {
                Command nextCommand = runningCommands[commandIndex];

                bool handled = nextCommand.Execute(this);
                if (handled) { runningCommands.RemoveAt(commandIndex); } else {commandIndex++;}
                if (!nextCommand.IsAsync()) break;
                Echo("Command is async, continuing to command at index: " + commandIndex);
            }
            return runningCommands.Count == 0;
        }

        private List<Command> parseCommands()
        {
            String[] commandList = Me.CustomData.Split(new[] { "\r\n", "\r", "\n" },StringSplitOptions.RemoveEmptyEntries);

            return commandList
                .Select(command => parseTokens(command))
                .Select(tokens => parseCommandParameters(tokens))
                .Select(parameters => parseCommand(parameters))
                .ToList();
        }

        private List<CommandParameter> parseCommandParameters(List<String> tokens)
        {
            Echo("Command: " + String.Join(" | ", tokens));

            List<CommandParameter> commandParameters = new List<CommandParameter>();
            foreach (var token in tokens)
            {
                if (ignoreWords.Contains(token)) continue;

                if (new BlockTypeParser().process(token, commandParameters)) continue;

                UnitType unitType;
                if (unitTypeWords.TryGetValue(token, out unitType))
                {
                    commandParameters.Add(new UnitCommandParameter(unitType));
                    continue;
                }

                if(increaseRelativeWords.Contains(token))
                {
                    commandParameters.Add(new RelativeCommandParameter());
                    commandParameters.Add(new DirectionCommandParameter(DirectionType.UP));
                    continue;
                }

                if (decreaseRelativeWords.Contains(token))
                {
                    commandParameters.Add(new RelativeCommandParameter());
                    commandParameters.Add(new DirectionCommandParameter(DirectionType.DOWN));
                    continue;
                }

                if (propertyWords.ContainsKey(token))
                {
                    commandParameters.AddList(propertyWords[token]);
                    continue;
                }

                double numericValue;
                if (Double.TryParse(token, out numericValue)) {
                    commandParameters.Add(new NumericCommandParameter((float)numericValue));
                    continue;
                }

                //If nothing else matches, must be a string
                commandParameters.Add(new StringCommandParameter(token));
            }

            return commandParameters;
        }

        private Command parseCommand(List<CommandParameter> parameters)
        {
            Echo("Pre Processed Parameters:");
            parameters.ForEach(param => Echo("Type: " + param.GetType()));

            ParameterProcessorRegistry.process(this, parameters);

            Echo("Post Prossessed Parameters:");
            parameters.ForEach(param => Echo("Type: " + param.GetType()));

            return CommandParserRegistry.ParseCommand(this, parameters);
        }
    }
}
