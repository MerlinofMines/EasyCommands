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
                AddBooleanHandler(Property.TRIGGER, (b) => CastVector(GetPropertyValue(b, new PropertySupplier(Property.TARGET+""))) != Vector3D.Zero, (b, v) => b.EnableRaycast = v);
                AddNumericHandler(Property.RANGE, GetRange, (b, v) => SetCustomProperty(b, "Range", "" + v), 100);
                AddVectorHandler(Property.TARGET_VELOCITY, (b) => GetVector(GetCustomProperty(b, "Velocity")).GetValueOrDefault());
                //TODO: Use setter to scan specific vector?
                AddVectorHandler(Property.TARGET, (b) => {
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
                    return GetVector(GetCustomProperty(b, "Target") ?? "").GetValueOrDefault();
                });
                defaultPropertiesByPrimitive[Return.VECTOR] = Property.TARGET;
                defaultPropertiesByDirection[Direction.UP] = Property.RANGE;
            }
            public float GetRange(IMyCameraBlock b) => float.Parse(GetCustomProperty(b, "Range") ?? "1000");
        }
    }
}
