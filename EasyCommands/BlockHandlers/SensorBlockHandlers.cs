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

namespace IngameScript {
    partial class Program {
        public class SensorBlockHandler : FunctionalBlockHandler<IMySensorBlock> {
            public SensorBlockHandler() {
                AddBooleanHandler(Property.TRIGGER, (b) => b.IsActive);
                AddBooleanHandler(Property.SILENCE, b => !b.PlayProximitySound, (b, v) => b.PlayProximitySound = !v);
                AddVectorHandler(Property.TARGET, (b) => {
                    MyDetectedEntityInfo lastDetectedEntity = b.LastDetectedEntity;
                    Vector3D position = Vector3D.Zero;
                    if (!lastDetectedEntity.IsEmpty()) position = GetPosition(lastDetectedEntity);
                    return position;
                });
                AddVectorHandler(Property.TARGET_VELOCITY, (b) => {
                    MyDetectedEntityInfo lastDetectedEntity = b.LastDetectedEntity;
                    Vector3D hitPosition = Vector3D.Zero;
                    if (!lastDetectedEntity.IsEmpty()) hitPosition = lastDetectedEntity.Velocity;
                    return hitPosition;
                });

                defaultPropertiesByPrimitive[Return.BOOLEAN] = Property.TRIGGER;
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
            }
        }
    }
}
