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
        private String[] activateWords = { "move", "go", "on", "start", "begin"};
        private String[] deactivateWords = { "stop", "off", "terminate", "exit", "cancel", "end"};
        private String[] reverseWords = { "reverse", "switch direction", "turn around" };
        private String[] increaseWords = { "increase", "raise", "extend", "expand", "clockwise", "clock", "forward", "forwards", "up"};
        private String[] decreaseWords = { "decrease", "lower", "retract", "reduce", "counter", "counterclock", "counterclockwise", "backward", "backwards", "down"};
        private String[] relativeWords = { "by" };
        private String[] increaseRelativeWords = { "add" };
        private String[] decreaseRelativeWords = { "subtact" };
        private String[] delayWords = {"delay", "after" };
        private String[] velocityWords = {"speed", "velocity", "rate", "pace" };
        private String[] waitWords = { "wait", "hold", "pause" };
        private String[] connectWords = { "connect", "join", "attach" };
        private String[] disconnectWords = { "disconnect", "separate", "detach" };

        private String[] lockWords = { "lock", "freeze" };
        private String[] unlockWords = { "unlock", "unfreeze" };

        private Dictionary<String, BlockType> blockTypeGroupWords = new Dictionary<String, BlockType>() {
            { "pistons", BlockType.PISTON },
            { "lights", BlockType.LIGHT },
            { "rotors", BlockType.ROTOR },
            { "programs", BlockType.PROGRAM },
            { "timers", BlockType.TIMER },
            { "projectors", BlockType.PROJECTOR },
            { "connectors", BlockType.CONNECTOR }
        };

        private Dictionary<String, BlockType> blockTypeWords = new Dictionary<String, BlockType>() {
            { "piston", BlockType.PISTON },
            { "light", BlockType.LIGHT },
            { "rotor", BlockType.ROTOR },
            { "program", BlockType.PROGRAM },
            { "timer", BlockType.TIMER },
            { "projector", BlockType.PROJECTOR },
            { "merge", BlockType.MERGE },
            { "connector", BlockType.CONNECTOR }
        };

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
        private Dictionary<String, CommandParameterType> tokenDictionary;

        List<Command> runningCommands;

        public Program()
        {
            tokenDictionary = new Dictionary<string, CommandParameterType>();

            foreach (var word in groupWords) {tokenDictionary.Add(word, CommandParameterType.GROUP);}
            foreach (var word in activateWords) { tokenDictionary.Add(word, CommandParameterType.ACTIVATE); }
            foreach (var word in deactivateWords) { tokenDictionary.Add(word, CommandParameterType.DEACTIVATE); }
            foreach (var word in reverseWords) { tokenDictionary.Add(word, CommandParameterType.REVERSE); }
            foreach (var word in increaseWords) { tokenDictionary.Add(word, CommandParameterType.INCREMENT); }
            foreach (var word in decreaseWords) { tokenDictionary.Add(word, CommandParameterType.DECREMENT); }
            foreach (var word in relativeWords) { tokenDictionary.Add(word, CommandParameterType.RELATIVE); }
            foreach (var word in delayWords) { tokenDictionary.Add(word, CommandParameterType.DELAY); }
            foreach (var word in velocityWords) { tokenDictionary.Add(word, CommandParameterType.VELOCITY); }
            foreach (var word in waitWords) { tokenDictionary.Add(word, CommandParameterType.WAIT); }
            foreach (var word in connectWords) { tokenDictionary.Add(word, CommandParameterType.CONNECT); }
            foreach (var word in disconnectWords) { tokenDictionary.Add(word, CommandParameterType.DISCONNECT); }
            foreach (var word in lockWords) { tokenDictionary.Add(word, CommandParameterType.LOCK); }
            foreach (var word in unlockWords) { tokenDictionary.Add(word, CommandParameterType.UNLOCK); }

            runningCommands = new List<Command>();
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
            if(runningCommands.Count == 0) return true;

            if(runningCommands[0].Execute(this))
            {
                runningCommands.RemoveAt(0);
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

        //Taken shamelessly from https://stackoverflow.com/questions/14655023/split-a-string-that-has-white-spaces-unless-they-are-enclosed-within-quotes
        private List<String> parseTokens(String commandString)
        {
            return commandString.Split('"')
                .Select((element, index) => index % 2 == 0  // If even index
                                    ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                                    : new string[] { element })  // Keep the entire item
                .SelectMany(element => element)
                .Select(element => element.ToLower())
                .ToList();
        }

        private List<CommandParameter> parseCommandParameters(List<String> tokens)
        {
            Echo("Command: " + String.Join(" | ", tokens));

            List<CommandParameter> commandParameters = new List<CommandParameter>();
            List<SelectorCommandParameter> selectors = new List<SelectorCommandParameter>();
            foreach (var token in tokens)
            {
                if (ignoreWords.Contains(token)) continue;

                BlockType blockType;
                if (blockTypeGroupWords.TryGetValue(token, out blockType))
                {
                    commandParameters.Add(new GroupCommandParameter());
                    commandParameters.Add(new BlockTypeCommandParameter(blockType));
                    continue;
                }

                if (blockTypeWords.TryGetValue(token, out blockType))
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

                if(increaseRelativeWords.Contains(token))
                {
                    commandParameters.Add(new RelativeCommandParameter());
                    commandParameters.Add(new IncrementCommandParameter(true));
                    continue;
                }

                if (decreaseRelativeWords.Contains(token))
                {
                    commandParameters.Add(new RelativeCommandParameter());
                    commandParameters.Add(new IncrementCommandParameter(false));
                    continue;
                }

                CommandParameterType commandParameterType;
                if (tokenDictionary.TryGetValue(token, out commandParameterType))
                {
                    switch (commandParameterType)
                    {
                        case CommandParameterType.GROUP:
                            commandParameters.Add(new GroupCommandParameter());
                            break;
                        case CommandParameterType.REVERSE:
                            commandParameters.Add(new ReverseCommandParameter());
                            break;
                        case CommandParameterType.DELAY:
                            commandParameters.Add(new DelayCommandParameter());
                            break;
                        case CommandParameterType.VELOCITY:
                            commandParameters.Add(new VelocityCommandParameter());
                            break;
                        case CommandParameterType.RELATIVE:
                            commandParameters.Add(new RelativeCommandParameter());
                            break;
                        case CommandParameterType.WAIT:
                            commandParameters.Add(new WaitCommandParameter());
                            break;
                        case CommandParameterType.ACTIVATE:
                            commandParameters.Add(new ActivationCommandParameter(true));
                            break;
                        case CommandParameterType.DEACTIVATE:
                            commandParameters.Add(new ActivationCommandParameter(false));
                            break;
                        case CommandParameterType.INCREMENT:
                            commandParameters.Add(new IncrementCommandParameter(true));
                            break;
                        case CommandParameterType.DECREMENT:
                            commandParameters.Add(new IncrementCommandParameter(false));
                            break;
                        case CommandParameterType.CONNECT:
                            commandParameters.Add(new ConnectCommandParameter(true));
                            break;
                        case CommandParameterType.DISCONNECT:
                            commandParameters.Add(new ConnectCommandParameter(false));
                            break;
                        case CommandParameterType.LOCK:
                            commandParameters.Add(new LockCommandParameter(true));
                            break;
                        case CommandParameterType.UNLOCK:
                            commandParameters.Add(new LockCommandParameter(false));
                            break;
                        default:
                            throw new Exception("Unsupported Command Parameter Type: " + commandParameterType);
                    }
                    continue;
                }

                double numericValue;
                if (Double.TryParse(token, out numericValue)) {
                    commandParameters.Add(new NumericCommandParameter((float)numericValue));
                    continue;
                }

                SelectorCommandParameter selector = new SelectorCommandParameter(token);
                //If nothing else matches, assume this is the name of the selector
                commandParameters.Add(selector);
                selectors.Add(selector);
            }

            //TODO: This may not hold once we have commands with multiple BlockType subcommands (If x is <Condition> then y do <Action>)
            if (commandParameters.Exists(command => command is BlockTypeCommandParameter)) return commandParameters;

            foreach (SelectorCommandParameter selector in selectors)
            {
                //Parse Selectors to try to find Block Type
                List<String> subTokens = parseTokens(selector.GetSelector());

                if (subTokens.Count < 2) continue;

                List<CommandParameter> tokenCommandParameters = parseCommandParameters(subTokens);

                foreach (CommandParameter subToken in tokenCommandParameters)
                {
                    if (subToken is BlockTypeCommandParameter)
                    {
                        commandParameters.Add(subToken);
                    }
                    else if (subToken is GroupCommandParameter)
                    {
                        commandParameters.Add(subToken);
                    }
                }
            }

            return commandParameters;
        }

        private Command parseCommand(List<CommandParameter> parameters)
        {
            Echo("Parsing Command");
            if (parameters.Exists(param => param is WaitCommandParameter))
            {
                return new WaitCommand(this, parameters);
            }

            CommandParameter blockTypeParameter = parameters.Find(param => param is BlockTypeCommandParameter);
            
            if (blockTypeParameter != null)
            {
                BlockType blockType = ((BlockTypeCommandParameter) blockTypeParameter).GetBlockType();
                switch (blockType)
                {
                    case BlockType.PISTON:
                        return new PistonCommand(this, parameters);
                    case BlockType.ROTOR:
                        return new RotorCommand(this, parameters);
                    case BlockType.LIGHT:
                        return new LightCommand(this, parameters);
                    case BlockType.PROGRAM:
                        return new ProgramCommand(this, parameters);
                    case BlockType.TIMER:
                        return new TimerBlockCommand(this, parameters);
                    case BlockType.PROJECTOR:
                        return new ProjectorCommand(this, parameters);
                    case BlockType.MERGE:
                        return new MergeBlockCommand(this, parameters);
                    case BlockType.CONNECTOR:
                        return new ConnectorCommand(this, parameters);
                    default:
                        throw new Exception("Unsupported Block Type Command: " + blockType);
                }
            }

            return new NullCommand(new List<CommandParameter>());
        }








    }
}
