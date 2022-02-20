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
        public class ProjectorBlockHandler : FunctionalBlockHandler<IMyProjector> {
            public ProjectorBlockHandler() {
                AddBooleanHandler(Property.COMPLETE, b => b.RemainingBlocks == 0);
                AddNumericHandler(Property.RATIO, b => 1 - b.RemainingBlocks / (float)b.TotalBlocks);
                AddBooleanHandler(Property.SHOW, b => b.IsProjecting, (b, v) => b.ShowOnlyBuildable = !v);
                AddPropertyHandler(TerminalPropertyHandler("KeepProjection", ""), Property.LOCKED);
                AddPropertyHandler(TerminalPropertyHandler("Scale", 0.1f), Property.LEVEL);

                AddVectorHandler(Property.ROLL_INPUT, b => GetRotation(b), (b, v) => b.ProjectionRotation = Vector(Clamp(v.X), Clamp(v.Y), Clamp(v.Z)));
                AddPropertyHandler(NumericHandler(b => GetRotation(b).X, (b, v) => SetRotation(b, Vector(0, 1, 1), Vector(v, 0, 0))),
                    Property.ROLL_INPUT, Property.UP);
                AddPropertyHandler(NumericHandler(b => -GetRotation(b).X, (b, v) => SetRotation(b, Vector(0, 1, 1), Vector(-v, 0, 0))),
                    Property.ROLL_INPUT, Property.DOWN);
                AddPropertyHandler(NumericHandler(b => GetRotation(b).Y, (b, v) => SetRotation(b, Vector(1, 0, 1), Vector(0, v, 0))),
                    Property.ROLL_INPUT, Property.RIGHT);
                AddPropertyHandler(NumericHandler(b => -GetRotation(b).Y, (b, v) => SetRotation(b, Vector(1, 0, 1), Vector(0, -v, 0))),
                    Property.ROLL_INPUT, Property.LEFT);
                AddPropertyHandler(NumericHandler(b => GetRotation(b).Z, (b, v) => SetRotation(b, Vector(1, 1, 0), Vector(0, 0, v))),
                    Property.ROLL_INPUT, Property.CLOCKWISE);
                AddPropertyHandler(NumericHandler(b => -GetRotation(b).Z, (b, v) => SetRotation(b, Vector(1, 1, 0), Vector(0, 0, -v))),
                    Property.ROLL_INPUT, Property.COUNTERCLOCKWISE);

                AddVectorHandler(Property.OFFSET, b => GetOffset(b), (b, v) => b.ProjectionOffset = new Vector3I(v));
                AddPropertyHandler(NumericHandler(b => GetOffset(b).X, (b, v) => SetOffset(b, Vector(0, 1, 1), Vector(v, 0, 0))),
                    Property.OFFSET, Property.RIGHT);
                AddPropertyHandler(NumericHandler(b => -GetOffset(b).X, (b, v) => SetOffset(b, Vector(0, 1, 1), Vector(-v, 0, 0))),
                    Property.OFFSET, Property.LEFT);
                AddPropertyHandler(NumericHandler(b => GetOffset(b).Y, (b, v) => SetOffset(b, Vector(1, 0, 1), Vector(0, v, 0))),
                    Property.OFFSET, Property.UP);
                AddPropertyHandler(NumericHandler(b => -GetOffset(b).Y, (b, v) => SetOffset(b, Vector(1, 0, 1), Vector(0, -v, 0))),
                    Property.OFFSET, Property.DOWN);
                AddPropertyHandler(NumericHandler(b => GetOffset(b).Z, (b, v) => SetOffset(b, Vector(1, 1, 0), Vector(0, 0, v))),
                    Property.OFFSET, Property.FORWARD);
                AddPropertyHandler(NumericHandler(b => -GetOffset(b).Z, (b, v) => SetOffset(b, Vector(1, 1, 0), Vector(0, 0, -v))),
                    Property.OFFSET, Property.BACKWARD);
            }

            void SetRotation(IMyProjector projector, Vector3I clearVector, Vector3I newOffset) => projector.ProjectionRotation = GetRotation(projector) * clearVector + new Vector3I(Clamp(newOffset.X), Clamp(newOffset.Y), Clamp(newOffset.Z));
            Vector3I GetRotation(IMyProjector projector) => projector.ProjectionRotation;

            void SetOffset(IMyProjector projector, Vector3I clearVector, Vector3I newOffset) => projector.ProjectionOffset = GetOffset(projector) * clearVector + newOffset;
            Vector3I GetOffset(IMyProjector projector) => projector.ProjectionOffset;

            Vector3I Vector(float i, float j, float k) => new Vector3I(i, j, k);
            float Clamp(double value) => (float)(value > 2 ? 2 : value < -2 ? -2 : value);
        }
    }
}
