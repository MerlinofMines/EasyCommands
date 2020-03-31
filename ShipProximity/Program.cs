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
        private double LASER_SCAN_DISTANCE = 10000;

        private MyDetectedEntityInfo lastDetectedInfo;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
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

            IMyCameraBlock camera = getCamera();

            if (camera == null) return;

            camera.EnableRaycast = true;

            if (!camera.CanScan(LASER_SCAN_DISTANCE))
            {
                return;
            }

            Echo("Scanning");
            MyDetectedEntityInfo detectedEntityInfo = camera.Raycast(LASER_SCAN_DISTANCE);

            if (detectedEntityInfo.IsEmpty())
            {
                Echo("No Target");
            } else
            {
                lastDetectedInfo = detectedEntityInfo;
            }



            if (lastDetectedInfo.IsEmpty()) return;


            double closestDistance = calculateClosestDistance();
            Echo("Closest Distance: " + closestDistance);

            IMyTextPanel lcd = getLCD();

            if (lcd == null) return;

            Echo("Found LCD");

            lcd.WriteText("Distance: " + Math.Round(closestDistance / 10) / 100 + "km");

        }

        private double calculateClosestDistance()
        {
            Vector3D distanceVector = Me.GetPosition() - lastDetectedInfo.Position;
            Vector3D detectedInfoVelocity = lastDetectedInfo.Velocity;

            if (lastDetectedInfo.Velocity.Length() < 0.0001) return distanceVector.Length();

            //Angle between 2 vectors
            double vang = Math.Acos(Vector3D.Normalize(distanceVector).Dot(Vector3D.Normalize(detectedInfoVelocity)));

            Echo("Angle: " + vang);

            double closestDistance = Math.Sin(vang) * distanceVector.Length();

            //TODO: Do Math
            //            return (lastDetectedInfo.Position - Me.GetPosition()).Length();
            return closestDistance;
 
       }
                                                                                                   
        private IMyTextPanel getLCD()
        {
            List<IMyTextPanel> panels = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels);

            if (panels.Count == 0) return null;

            return panels.Find(panel => panel.CustomName.Contains("LCD"));

        }

        private IMyCameraBlock getCamera()
        {
            List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(cameras);

            if (cameras.Count == 0)
            {
                Echo("No Camera Found.  Aborting.");
                return null;
            }

            if (cameras.Count > 1)
            {
                Echo("Multiple Cameras Found. Aborting");
                return null;
            }

            return cameras[0];
        }
    }
}
