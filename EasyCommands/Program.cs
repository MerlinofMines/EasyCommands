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
            foreach (var word in groupWords) { propertyWords.Add(word, new List<CommandParameter>() { new GroupCommandParameter() }); }
            foreach (var word in activateWords) { propertyWords.Add(word, new List<CommandParameter>() { new BooleanCommandParameter(true) }); }
            foreach (var word in deactivateWords) { propertyWords.Add(word, new List<CommandParameter>() { new BooleanCommandParameter(false) }); }
            foreach (var word in increaseWords) { propertyWords.Add(word, new List<CommandParameter>() { new DirectionCommandParameter(DirectionType.UP) }); }
            foreach (var word in decreaseWords) { propertyWords.Add(word, new List<CommandParameter>() { new DirectionCommandParameter(DirectionType.DOWN) }); }
            foreach (var word in clockwiseWords) { propertyWords.Add(word, new List<CommandParameter>() { new DirectionCommandParameter(DirectionType.CLOCKWISE) }); }
            foreach (var word in counterclockwiseWords) { propertyWords.Add(word, new List<CommandParameter>() { new DirectionCommandParameter(DirectionType.COUNTERCLOCKWISE) }); }
            foreach (var word in reverseWords) { propertyWords.Add(word, new List<CommandParameter>() { new ReverseCommandParameter() }); }
            foreach (var word in relativeWords) { propertyWords.Add(word, new List<CommandParameter>() { new RelativeCommandParameter() }); }
            foreach (var word in speedWords) { propertyWords.Add(word, new List<CommandParameter>() { new NumericPropertyCommandParameter(NumericPropertyType.SPEED) }); }
            foreach (var word in connectWords) { propertyWords.Add(word, new List<CommandParameter>() { new BooleanPropertyCommandParameter(BooleanPropertyType.CONNECTED) }); }
            foreach (var word in disconnectWords) { propertyWords.Add(word, new List<CommandParameter>() { new BooleanPropertyCommandParameter(BooleanPropertyType.CONNECTED), new BooleanCommandParameter(false) }); }
            foreach (var word in lockWords) { propertyWords.Add(word, new List<CommandParameter>() { new BooleanPropertyCommandParameter(BooleanPropertyType.LOCKED) }); }
            foreach (var word in unlockWords) { propertyWords.Add(word, new List<CommandParameter>() { new BooleanPropertyCommandParameter(BooleanPropertyType.LOCKED), new BooleanCommandParameter(false) }); }
            foreach (var word in waitWords) { propertyWords.Add(word, new List<CommandParameter>() { new WaitCommandParameter()}); }
            foreach (var word in ifWords) { propertyWords.Add(word, new List<CommandParameter>() { new IfCommandParameter(false, false, false) }); }
            foreach (var word in unlessWords) { propertyWords.Add(word, new List<CommandParameter>() { new IfCommandParameter(true, false, false) }); }
            foreach (var word in whileWords) { propertyWords.Add(word, new List<CommandParameter>() { new IfCommandParameter(false, true, false) }); }
            foreach (var word in untilWords) { propertyWords.Add(word, new List<CommandParameter>() { new IfCommandParameter(true, true, false) }); }
            foreach (var word in whenWords) { propertyWords.Add(word, new List<CommandParameter>() { new IfCommandParameter(true, true, true) }); }
            foreach (var word in asyncWords) { propertyWords.Add(word, new List<CommandParameter>() { new AsyncCommandParameter() }); }
            foreach (var word in elseWords) { propertyWords.Add(word, new List<CommandParameter>() { new ElseCommandParameter() }); }
            foreach (var word in lessWords) { propertyWords.Add(word, new List<CommandParameter>() { new ComparisonCommandParameter(ComparisonType.LESS) }); }
            foreach (var word in lessEqualWords) { propertyWords.Add(word, new List<CommandParameter>() { new ComparisonCommandParameter(ComparisonType.LESS_OR_EQUAL) }); }
            foreach (var word in equalWords) { propertyWords.Add(word, new List<CommandParameter>() { new ComparisonCommandParameter(ComparisonType.EQUAL) }); }
            foreach (var word in greaterEqualWords) { propertyWords.Add(word, new List<CommandParameter>() { new ComparisonCommandParameter(ComparisonType.GREATER_OR_EQUAL) }); }
            foreach (var word in greaterWords) { propertyWords.Add(word, new List<CommandParameter>() { new ComparisonCommandParameter(ComparisonType.GREATER) }); }
            foreach (var word in anyWords) { propertyWords.Add(word, new List<CommandParameter>() { new AggregationModeCommandParameter(AggregationMode.ANY) }); }
            foreach (var word in allWords) { propertyWords.Add(word, new List<CommandParameter>() { new AggregationModeCommandParameter(AggregationMode.ALL) }); }
            foreach (var word in noneWords) { propertyWords.Add(word, new List<CommandParameter>() { new AggregationModeCommandParameter(AggregationMode.NONE) }); }
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
