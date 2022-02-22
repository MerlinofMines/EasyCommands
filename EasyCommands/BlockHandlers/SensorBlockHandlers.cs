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
                AddBooleanHandler(Property.TRIGGER, b => b.IsActive);
                AddBooleanHandler(Property.MEDIA, b => b.PlayProximitySound, (b, v) => b.PlayProximitySound = v);
                AddVectorHandler(Property.TARGET, b => {
                    var lastDetectedEntity = b.LastDetectedEntity;
                    return lastDetectedEntity.IsEmpty() ? Vector3D.Zero : GetPosition(lastDetectedEntity);
                });
                AddVectorHandler(Property.TARGET_VELOCITY, b => {
                    var lastDetectedEntity = b.LastDetectedEntity;
                    return lastDetectedEntity.IsEmpty() ? Vector3.Zero : lastDetectedEntity.Velocity;
                });

                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
            }
        }
    }
}
