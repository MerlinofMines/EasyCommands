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
        //============ DEBUG ==========
        //To debug, comment out below line and uncomment line below it to remove automatic running.  Then, run the command on your programming block manually
        //to see output which will tell you where the script is getting stuck.  Usually will be an initialization issue, or it could be that the room cannot be pressurized.
        private static UpdateFrequency UPDATE_FREQUENCY = UpdateFrequency.Update10;
        //private static UpdateFrequency UPDATE_FREQUENCY = UpdateFrequency.None; //Uncomment to debug.

        //============ Configurable Options =============
        private static string AIRLOCK_GROUP = "Airlock"; //Default Block Group to look for
        private static string EXTERNAL_DOOR_TAG = "exterior"; //Default tag to indicate that a door is an "exterior" door
        private static string INTERIOR_DOOR_TAG = "interior"; //Default tag to indicate that a door is an "interior" door
        private static string[] pressurizeCommands = {"in", "true", "pressurize", "enter"}; //Commands you can use to say "please pressurize";
        private static string[] depressurizeCommands = {"out", "false", "depressurize", "exit"}; //Commands you can use to say "please depressurize";

        //These thresholds determine whether we consider air vents to be "pressurized/depressurized".  Because of small changes in detected pressurization we can't 
        //simply use 0 and 1 (also because doubles, and...well...doubles).
        private static double PRESSURIZED_THRESHOLD = 0.999;
        private static double DEPRESSURIZED_THRESHOLD = 0.001;

        //These thresholds determine whether we consider air vents to be "pressurized/depressurized".  Because of small changes in detected pressurization we can't 
        //simply use 0 and 1 (also because doubles, and...well...doubles).
        private static double OPEN_THRESHOLD = 0.999;
        private static double CLOSED_THRESHOLD = 0.0001;

        //============= DONT TOUCH ===========
        //The below is internal state of the program.  Touch it and all bets are off.

        //This internal dictionary keeps track of the current active airlock groups that we are currently processing.
        //boolean represents whether we are pressurizing (true) or depressurizing (false).  Values are removed once we have completed
        //the requested operation.  Note that this allows us to interrupt "closing" or "opening" the doors by replacing the entry in the map
        //for the same key.  Last one wins is appropriate here.
        private Dictionary<string, bool> activeAirLockGroups = new Dictionary<string, bool>();
        private static bool PRESSURIZE = true;

        public void Main(string argument)
        {
            string blockGroup = AIRLOCK_GROUP;

            if (!string.IsNullOrWhiteSpace(argument))
            {
                addNewProcess(argument.Trim());
            }

            List<string> completedProcesses = new List<string>();

            foreach (KeyValuePair<string, bool> airlockGroup in activeAirLockGroups)
            {
                bool completed = process(airlockGroup.Key, airlockGroup.Value);

                if (completed)
                {
                    Echo("Process Completed: " + airlockGroup.Key);
                    completedProcesses.Add(airlockGroup.Key);
                }
            }

            foreach(string completedProcess in completedProcesses)
            {
                activeAirLockGroups.Remove(completedProcess);
            }

            if (activeAirLockGroups.Count > 0)
            {
                Runtime.UpdateFrequency = UPDATE_FREQUENCY;
            } else
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
        }

        //Attempts to add a new process to list of requested processes.
        private void addNewProcess(string argument)
        {
            string requestedAction = argument;
            string blockGroupName = AIRLOCK_GROUP;

            int blockGroupIndex = argument.IndexOf(' ');

            if (blockGroupIndex > 0)
            {
                requestedAction = argument.Substring(0, blockGroupIndex);
                blockGroupName = argument.Substring(blockGroupIndex).Trim();
            }

            if (pressurizeCommands.Contains(requestedAction))
            {
                activeAirLockGroups[blockGroupName] = PRESSURIZE;
            }
            else if (depressurizeCommands.Contains(requestedAction))
            {
                activeAirLockGroups[blockGroupName] = !PRESSURIZE;
            }
            else
            {
                Echo("Unknown Action: " + requestedAction);
                Echo("Accepted Values are: " + pressurizeCommands + " | " + depressurizeCommands);
            }
        }

        private bool process(string airlockGroup, bool doPressurize)
        {
            //We do this every time so that you don't have to restart the script every time you change the airlock configuration.
            //Could be moved to constructor..but meh.
            List<IMyAirtightDoorBase> exteriorAirlockDoors = new List<IMyAirtightDoorBase>();
            List<IMyAirtightDoorBase> interiorAirlockDoors = new List<IMyAirtightDoorBase>();
            List<IMyAirVent> airVents = new List<IMyAirVent>();

            bool initialized = initialize(exteriorAirlockDoors, interiorAirlockDoors, airVents, airlockGroup);

            if (!initialized)
            {
                Echo("Unable to Initialize Airlock properly.  Terminating processing");
                return true; //Return true to indicate that we've finished processing.  Don't really have "error codes".  Could throw an exception and then swallow it,
                             //but meh.
            }

            if (doPressurize) {
                if (isPressurized(airVents))
                {
                    if (!areAllOpen(interiorAirlockDoors))
                    {
                        openAll(interiorAirlockDoors);
                    } else
                    {
                        return true; //Pressurized and interior doors opened.  We're done.
                    }
                } else //Not Pressurized
                {
                    //Safe to Pressurize
                    if (areAllClosed(interiorAirlockDoors) && areAllClosed(exteriorAirlockDoors))
                    {
                        pressurize(airVents);
                    } else //Close doors before pressurizing
                    {
                        closeAll(interiorAirlockDoors);
                        closeAll(exteriorAirlockDoors);
                    }
                }
            } else
            {
                if (isDepressurized(airVents))
                {
                    if (!areAllOpen(exteriorAirlockDoors))
                    {
                        openAll(exteriorAirlockDoors);
                    }
                    else
                    {
                        return true; //Depressurized and exterior doors opened.  We're done.
                    }
                }
                else //Not Depressurized
                {
                    //Safe to Depressurize
                    if (areAllClosed(interiorAirlockDoors) && areAllClosed(exteriorAirlockDoors))
                    {
                        depressurize(airVents);
                    }
                    else //Close doors before pressurizing
                    {
                        closeAll(interiorAirlockDoors);
                        closeAll(exteriorAirlockDoors);
                    }
                }
            }
            return false;
        }

        private void openAll(List<IMyAirtightDoorBase> airlockDoors)
        {
            foreach (IMyAirtightDoorBase airlockDoor in airlockDoors)
            {
                Echo("Opening Door: " + airlockDoor.CustomName);
                airlockDoor.OpenDoor();
            }
        }

        private void closeAll(List<IMyAirtightDoorBase> airlockDoors)
        {
            foreach(IMyAirtightDoorBase airlockDoor in airlockDoors)
            {
                Echo("Closing Door: " + airlockDoor.CustomName);
                airlockDoor.CloseDoor();
            }
        }

        private bool areAllClosed(List<IMyAirtightDoorBase> airlockDoors)
        {
            foreach (IMyAirtightDoorBase airlockDoor in airlockDoors)
            {
                if (airlockDoor.OpenRatio > CLOSED_THRESHOLD)
                {
                    return false;
                }
            }
            return true;
        }

        private bool areAllOpen(List<IMyAirtightDoorBase> airlockDoors)
        {
            foreach (IMyAirtightDoorBase airlockDoor in airlockDoors)
            {
                if (airlockDoor.OpenRatio < OPEN_THRESHOLD)
                {
                    return false;
                }
            }
            return true;
        }

        private bool isPressurized(List<IMyAirVent> airVents)
        {
            foreach (IMyAirVent vent in airVents)
            {
                Echo("Pressurization Level: " + vent.GetOxygenLevel());
                if (vent.GetOxygenLevel() < PRESSURIZED_THRESHOLD)
                {
                    Echo("Not Pressurized");
                    return false;
                }
            }
            return true;
        }

        private bool isDepressurized(List<IMyAirVent> airVents)
        {
            foreach (IMyAirVent vent in airVents)
            {
                if (vent.GetOxygenLevel() > DEPRESSURIZED_THRESHOLD)
                {
                    return false;
                }
            }
            return true;
        }

        private void pressurize(List<IMyAirVent> airVents)
        {
            foreach(IMyAirVent vent in airVents)
            {
                Echo("Pressurizing " + vent.CustomName);
                vent.Depressurize = false;
            }
        }

        private void depressurize(List<IMyAirVent> airVents)
        {
            foreach (IMyAirVent vent in airVents)
            {
                Echo("Depressurizing " + vent.CustomName);
                vent.Depressurize = true;
            }
        }

        //Initialized the input list of doors and vents by loading them from the given block group.
        private bool initialize(List<IMyAirtightDoorBase> exteriorAirlockDoors, List<IMyAirtightDoorBase> interiorAirlockDoors, List<IMyAirVent> airVents, string blockGroup)
        {
            IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName(blockGroup);

            if (group == null)
            {
                Echo("Unable to find requested block group: " + blockGroup + ", ignoring.");
                return false;
            }

            List<IMyAirtightDoorBase> airtightDoors = new List<IMyAirtightDoorBase>();
            group.GetBlocksOfType<IMyAirtightDoorBase>(airtightDoors);
            group.GetBlocksOfType<IMyAirVent>(airVents);

            foreach (IMyAirtightDoorBase door in airtightDoors)
            {
                if (door.CustomName.ToLower().Contains(EXTERNAL_DOOR_TAG)) {
                    exteriorAirlockDoors.Add(door);
                }
                else if (door.CustomName.ToLower().Contains(INTERIOR_DOOR_TAG))
                {
                    interiorAirlockDoors.Add(door);
                }
                else
                {
                    Echo("Unknown door: " + door.CustomName + ", ignoring");
                }
            }

            if (exteriorAirlockDoors.Count == 0)
            {
                Echo("No Exterior Doors found for block group " + blockGroup + ", ignoring.");
                return false;
            }

            if (interiorAirlockDoors.Count == 0)
            {
                Echo("No Interior Doors found for block group " + blockGroup + ", ignoring.");
                return false;
            }

            if (airVents.Count == 0)
            {
                Echo("No Air Vents found for block group " + blockGroup + ", ignoring.");
                return false;
            }

            return true;
        }
    }
}