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
                AddBooleanHandler(PropertyType.TRIGGER, (b) => b.IsActive);
                AddVectorHandler(PropertyType.TARGET, (b) => {
                    MyDetectedEntityInfo lastDetectedEntity = b.LastDetectedEntity;
                    Vector3D hitPosition = Vector3D.Zero;
                    if (!lastDetectedEntity.IsEmpty()) hitPosition = lastDetectedEntity.HitPosition.Value;
                    return hitPosition;
                });
                AddVectorHandler(PropertyType.TARGET_VELOCITY, (b) => {
                    MyDetectedEntityInfo lastDetectedEntity = b.LastDetectedEntity;
                    Vector3D hitPosition = Vector3D.Zero;
                    if (!lastDetectedEntity.IsEmpty()) hitPosition = lastDetectedEntity.Velocity;
                    return hitPosition;
                });

                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.TRIGGER;
                defaultPropertiesByPrimitive[PrimitiveType.VECTOR] = PropertyType.TARGET;
            }
        }
    }
}
