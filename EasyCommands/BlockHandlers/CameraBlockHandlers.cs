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
        public class CameraBlockHandler : FunctionalBlockHandler<IMyCameraBlock> {
            public CameraBlockHandler() {
                AddBooleanHandler(PropertyType.TRIGGER, (b) => CastVector(GetPropertyValue(b, PropertyType.TARGET)).GetVectorValue() != Vector3D.Zero, (b, v) => b.EnableRaycast = v);
                AddNumericHandler(PropertyType.RANGE, GetRange, (b, v) => SetCustomProperty(b, "Range", "" + v), 100);
                AddVectorHandler(PropertyType.TARGET_VELOCITY, (b) => { Vector3D velocity; GetVector(GetCustomProperty(b, "Velocity"), out velocity); return velocity; });
                //TODO: Use setter to scan specific vector?
                AddVectorHandler(PropertyType.TARGET, (b) => {
                    var range = (double)GetRange(b);
                    b.EnableRaycast = true;
                    if (b.CanScan(range)) {
                        MyDetectedEntityInfo detectedEntity = b.Raycast(range);
                        if (!detectedEntity.IsEmpty()) {
                            SetCustomProperty(b, "Target", VectorToString(GetPosition(detectedEntity)));
                            SetCustomProperty(b, "Velocity", VectorToString(detectedEntity.Velocity));
                        } else {
                            DeleteCustomProperty(b, "Target");
                        }
                    }
                    Vector3D hitPosition;
                    if (!GetVector(GetCustomProperty(b, "Target") ?? "", out hitPosition)) hitPosition = Vector3D.Zero;
                    return hitPosition;
                });
                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.TRIGGER;
                defaultPropertiesByPrimitive[PrimitiveType.VECTOR] = PropertyType.TARGET;
                defaultPropertiesByDirection[DirectionType.UP] = PropertyType.RANGE;
                defaultDirection = DirectionType.UP;
            }
            public float GetRange(IMyCameraBlock b) { return float.Parse(GetCustomProperty(b, "Range") ?? "1000"); }
        }
    }
}
