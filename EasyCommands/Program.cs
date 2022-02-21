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
using System.Runtime.CompilerServices;

#pragma warning disable ProhibitedMemberRule // Prohibited Type Or Member
[assembly: InternalsVisibleTo("EasyCommands.Tests")]
#pragma warning restore ProhibitedMemberRule // Prohibited Type Or Member
namespace IngameScript {
    public partial class Program : MyGridProgram {
        #region mdk preserve
        public UpdateFrequency updateFrequency = UpdateFrequency.Update1;
        public LogLevel logLevel = LogLevel.INFO;
        public int commandParseAmount = 1;
        public int commandParameterParseAmount = 100;
        public int maxAsyncThreads = 50;
        public int maxQueuedThreads = 50;
        public int maxItemTransfers = 10;
        #endregion

        public static Program PROGRAM;
        public ProgramState state = ProgramState.STOPPED;
        public Dictionary<String, FunctionDefinition> functions = NewDictionary<string, FunctionDefinition>();
        public Thread currentThread;
        public Random randomGenerator = new Random();

        List<Thread> threadQueue = NewList<Thread>();
        List<Thread> asyncThreadQueue = NewList<Thread>();
        Dictionary<String, IVariable> globalVariables = NewDictionary<String, IVariable>();

        Dictionary<ProgramState, KeyValuePair<String, bool>> programStateMap = NewDictionary(
            KeyValuePair(ProgramState.RUNNING, KeyValuePair("Running", true)),
            KeyValuePair(ProgramState.PAUSED, KeyValuePair("Paused", false)),
            KeyValuePair(ProgramState.STOPPED, KeyValuePair("Stopped", false)),
            KeyValuePair(ProgramState.COMPLETE, KeyValuePair("Complete", false))
        );

        String defaultFunction;
        String customData = null;
        List<String> commandStrings = NewList<String>();

        public void ClearAllState() {
            BroadCastListenerAction(l => true, listener => {
                IGC.DisableBroadcastListener(listener);
                while (listener.HasPendingMessage) listener.AcceptMessage();
            });

            asyncThreadQueue.Clear();
            threadQueue.Clear();
            globalVariables = NewDictionary(
                KeyValuePair("pi", GetStaticVariable(Math.PI)),
                KeyValuePair("e", GetStaticVariable(Math.E)),
                KeyValuePair("empty", GetStaticVariable(NewKeyedList())),
                KeyValuePair("x", GetStaticVariable(Vector(1 ,0, 0))),
                KeyValuePair("y", GetStaticVariable(Vector(0, 1, 0))),
                KeyValuePair("z", GetStaticVariable(Vector(0, 0, 1))),
                KeyValuePair("r", GetStaticVariable(Vector(1 ,0, 0))),
                KeyValuePair("g", GetStaticVariable(Vector(0, 1, 0))),
                KeyValuePair("b", GetStaticVariable(Vector(0, 0, 1)))
            );
        }

        public void BroadCastListenerAction(Func<IMyBroadcastListener, bool> filter, Action<IMyBroadcastListener> action) {
            var listeners = NewList<IMyBroadcastListener>();
            IGC.GetBroadcastListeners(listeners);
            foreach(IMyBroadcastListener listener in listeners.Where(filter)) action(listener);
        }

        public Thread GetCurrentThread() => currentThread;

        public void QueueThread(Thread thread) {
            threadQueue.Add(thread);
            if (threadQueue.Count > maxQueuedThreads) throw new Exception("Stack Overflow Exception! Cannot have more than " + maxQueuedThreads + " queued commands");
        }

        public void QueueAsyncThread(Thread thread) {
            asyncThreadQueue.Add(thread);
            if (asyncThreadQueue.Count > maxAsyncThreads) throw new Exception("Stack Overflow Exception! Cannot have more than " + maxAsyncThreads + "concurrent async commands");
        }

        public void SetGlobalVariable(String variableName, IVariable variable) {
            globalVariables[variableName] = variable;
        }

        public IVariable GetVariable(String variableName) {
            Thread currentThread = GetCurrentThread();
            if(currentThread.threadVariables.ContainsKey(variableName)) {
                return currentThread.threadVariables[variableName];
            } else if (globalVariables.ContainsKey(variableName)) {
                return globalVariables[variableName];
            } else {
                throw new Exception("No Variable Exists for name: " + variableName);
            }
        }

        public Program() {
            PROGRAM = this;
            InitializeParsers();
            InitializeProcessors();
            InitializeOperators();
            InitializeItems();
            Runtime.UpdateFrequency = updateFrequency;
        }

        static void Print(String str) { PROGRAM.Echo(str); }
        static void Info(String str) { if (PROGRAM.logLevel != LogLevel.SCRIPT_ONLY) Print(str); }

        public void Main(string argument) {
            try {
                if (!ParseCommands()) {
                    Runtime.UpdateFrequency = updateFrequency;
                    return;
                }
            } catch (Exception e) {
                Print("Exception Occurred During Parsing:");
                Print(e.Message);
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }

            var messages = NewList<MyIGCMessage>();
            BroadCastListenerAction(listener => listener.HasPendingMessage, listener => messages.Add(listener.AcceptMessage()));

            try {
                if (messages.Count > 0) {
                    var messageCommands = messages.Select(message => new Thread(ParseCommand((String)message.Data), "Message", message.Tag));
                    threadQueue.InsertRange(0, messageCommands);
                }
                if (!String.IsNullOrEmpty(argument)) { threadQueue.Insert(0, new Thread(ParseCommand(argument), "Request", argument)); }

                RunThreads();

                //Update State
                var stateTuple = programStateMap[state];
                Info(stateTuple.Key);
                Runtime.UpdateFrequency = stateTuple.Value ? updateFrequency : UpdateFrequency.None;
            } catch(Exception e) {
                Print("Exception Occurred:");
                Print(e.Message);
                Runtime.UpdateFrequency = UpdateFrequency.None;
                return;
            }
        }

        void RunThreads() {
            try {
                // invalidate block & group cache
                blockCache.Clear();
                groupCache.Clear();
                selectorCache.Clear();

                //If no current commands, we've been asked to restart.  start at the top.
                if (threadQueue.Count == 0 && asyncThreadQueue.Count == 0) {
                    FunctionDefinition defaultFunctionDefinition = functions[defaultFunction];
                    threadQueue.Add(new Thread(defaultFunctionDefinition.function.Clone(), "Main", defaultFunctionDefinition.functionName));
                }

                Info("Queued Threads: " + threadQueue.Count());
                Info("Async Threads: " + asyncThreadQueue.Count());
                //Run first command in the queue.  Could be from a message, program run request, or request to start the main program.
                if (threadQueue.Count > 0 ) {
                    currentThread = threadQueue[0];
                    Info(currentThread.GetName());
                    if(currentThread.Command.Execute()) {
                        threadQueue.RemoveAt(0);
                    }
                }

                //Process 1 iteration of all async commands, removing from queue if processed.
                int asyncThreadQueueIndex = 0;

                while (asyncThreadQueueIndex < asyncThreadQueue.Count) {
                    currentThread = asyncThreadQueue[asyncThreadQueueIndex];
                    Info(currentThread.GetName());
                    if (currentThread.Command.Execute()) {
                        asyncThreadQueue.RemoveAt(asyncThreadQueueIndex);
                    } else {
                        asyncThreadQueueIndex++;
                    }
                }

                if(threadQueue.Count == 0 && asyncThreadQueue.Count == 0) {
                    state = ProgramState.COMPLETE;
                } else {
                    state = ProgramState.RUNNING;
                }
            //InterruptException is thrown by control commands to interrupt execution (stop, pause, restart).
            //The command itself has set the correct state of the command queues, we just need to set the program state.
            } catch(InterruptException interrupt) {
                state = interrupt.ProgramState;
            }
        }

        public class Thread {
            public Command Command { get; set; }
            public Dictionary<String, IVariable> threadVariables = NewDictionary<string, IVariable>();
            String prefix;
            String name;

            public T GetCurrentCommand<T>(Func<Command, bool> filter) where T : class =>
                Command.SearchCurrentCommand(command => command is T && filter(command)) as T;

            public Thread(Command command, string p, string n) {
                Command = command;
                prefix = p;
                name = n;
            }

            public String GetName() => "[" + prefix + "] " + name;
            public void SetName(String s) => name = s;
        }

        public class FunctionDefinition {
            public String functionName;
            public MultiActionCommand function = null;
            public List<String> parameterNames;

            public FunctionDefinition(string function, List<string> parameters) {
                functionName = function;
                parameterNames = parameters;
            }
        }
    }
}
