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
                AddPropertyHandler(Property.LOCKED, TerminalPropertyHandler("KeepProjection", ""));
                AddPropertyHandler(Property.LEVEL, TerminalPropertyHandler("Scale", 0.1f));

                AddDirectionHandlers(Property.ROLL_INPUT, Direction.NONE,
                    TypeHandler(VectorHandler(b => GetRotation(b), (b, v) => b.ProjectionRotation = Vector(Clamp(v.X), Clamp(v.Y), Clamp(v.Z))), Direction.NONE),
                    TypeHandler(NumericHandler(b => GetRotation(b).X, (b, v) => SetRotation(b, Vector(0, 1, 1), Vector(v, 0, 0))), Direction.UP),
                    TypeHandler(NumericHandler(b => -GetRotation(b).X, (b, v) => SetRotation(b, Vector(0, 1, 1), Vector(-v, 0, 0))), Direction.DOWN),
                    TypeHandler(NumericHandler(b => GetRotation(b).Y, (b, v) => SetRotation(b, Vector(1, 0, 1), Vector(0, v, 0))), Direction.RIGHT),
                    TypeHandler(NumericHandler(b => -GetRotation(b).Y, (b, v) => SetRotation(b, Vector(1, 0, 1), Vector(0, -v, 0))), Direction.LEFT),
                    TypeHandler(NumericHandler(b => GetRotation(b).Z, (b, v) => SetRotation(b, Vector(1, 1, 0), Vector(0, 0, v))), Direction.CLOCKWISE),
                    TypeHandler(NumericHandler(b => -GetRotation(b).Z, (b, v) => SetRotation(b, Vector(1, 1, 0), Vector(0, 0, -v))), Direction.COUNTERCLOCKWISE));

                AddDirectionHandlers(Property.OFFSET, Direction.NONE,
                    TypeHandler(VectorHandler(b => GetOffset(b), (b,v) => b.ProjectionOffset = new Vector3I(v)), Direction.NONE),
                    TypeHandler(NumericHandler(b => GetOffset(b).X, (b, v) => SetOffset(b, Vector(0, 1, 1),  Vector(v, 0, 0))), Direction.RIGHT),
                    TypeHandler(NumericHandler(b => -GetOffset(b).X, (b,v) => SetOffset(b, Vector(0, 1, 1), Vector(-v, 0, 0))), Direction.LEFT),
                    TypeHandler(NumericHandler(b => GetOffset(b).Y, (b, v) => SetOffset(b, Vector(1, 0, 1), Vector(0, v, 0))), Direction.UP),
                    TypeHandler(NumericHandler(b => -GetOffset(b).Y, (b, v) => SetOffset(b, Vector(1, 0, 1), Vector(0, -v, 0))), Direction.DOWN),
                    TypeHandler(NumericHandler(b => GetOffset(b).Z, (b, v) => SetOffset(b, Vector(1, 1, 0), Vector(0, 0, v))), Direction.FORWARD),
                    TypeHandler(NumericHandler(b => -GetOffset(b).Z, (b, v) => SetOffset(b, Vector(1, 1, 0),  Vector(0, 0, -v))), Direction.BACKWARD));
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
