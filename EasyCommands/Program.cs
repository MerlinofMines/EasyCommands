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
        private String[] ignoreWords = { "the", "and", "less", "than", "greater", "more", "turn", "rotate", "set", "until", "is", "block", "tell", "to", "from" };
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

            //Register Block Handlers
            BlockHandlerRegistry.RegisterBlockHandler(BlockType.LIGHT, new BaseBlockHandler<IMyLightingBlock>());
            BlockHandlerRegistry.RegisterBlockHandler(BlockType.MERGE, new BaseBlockHandler<IMyShipMergeBlock>());
            BlockHandlerRegistry.RegisterBlockHandler(BlockType.PROJECTOR, new BaseBlockHandler<IMyProjector>());
            BlockHandlerRegistry.RegisterBlockHandler(BlockType.TIMER, new BaseBlockHandler<IMyTimerBlock>());
            BlockHandlerRegistry.RegisterBlockHandler(BlockType.CONNECTOR, new ConnectorBlockHandler());
            BlockHandlerRegistry.RegisterBlockHandler(BlockType.WELDER, new BaseBlockHandler<IMyShipWelder>());
            BlockHandlerRegistry.RegisterBlockHandler(BlockType.GRINDER, new BaseBlockHandler<IMyShipGrinder>());
            BlockHandlerRegistry.RegisterBlockHandler(BlockType.PISTON, new PistonBlockHandler());
            BlockHandlerRegistry.RegisterBlockHandler(BlockType.ROTOR, new RotorBlockHandler());
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
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
            Echo("Pre Prossessed Parameters:");
            parameters.ForEach(param => Echo("Type: " + param.GetType()));

            ParameterProcessorRegistry.process(parameters);

            Echo("Post Prossessed Parameters:");
            parameters.ForEach(param => Echo("Type: " + param.GetType()));

            return CommandParserRegistry.ParseCommand(this, parameters);
        }
    }
}
