using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript {
    using Sandbox.ModAPI.Ingame;
    partial class Program {
        public class AiBlockHandler<T> : FunctionalBlockHandler<T> where T : class, IMyFunctionalBlock {
            public AiBlockHandler() {
                AddPropertyHandler(Property.AUTO, TerminalPropertyHandler("ActivateBehavior", true));
            }
        }

        public class FlightMovementBlockHandler : AiBlockHandler<IMyFlightMovementBlock> {
            public FlightMovementBlockHandler() {
                AddNumericHandler(Property.VELOCITY, b => b.SpeedLimit, (b, v) => b.SpeedLimit = v, 1);
                AddBooleanHandler(Property.PRECISION, b => b.PrecisionMode, (b, v) => b.PrecisionMode = v);
                AddBooleanHandler(Property.AVOID, b => b.CollisionAvoidance, (b, v) => b.CollisionAvoidance = v);
                AddPropertyHandler(Property.ALTITUDE, TerminalPropertyHandler("MinHeightAboveTerrain", 1f));
                AddPropertyHandler(NewList(Property.STRENGTH, Property.POSITION), BooleanHandler(b => b.AlignToPGravity, (b, v) => b.AlignToPGravity = v));
            }
        }

        public class PathRecorderBlockHandler : AiBlockHandler<IMyPathRecorderBlock> {
            public PathRecorderBlockHandler() {
                AddBooleanHandler(Property.REPEAT, b => Recorder(b).RepeatPath, (b, v) => Recorder(b).RepeatPath = v);
                AddBooleanHandler(Property.MEDIA, b => Recorder(b).IsPlaying, (b, v) => {
                    if (v) Recorder(b).Play(); else Recorder(b).StopPlay();
                });
            }

            IMyPathRecorderComponent Recorder(IMyPathRecorderBlock block) => RequireComponent<IMyPathRecorderComponent>(block);
        }

        public class DefensiveCombatBlockHandler : AiBlockHandler<IMyDefensiveCombatBlock> {
            public DefensiveCombatBlockHandler() {
                AddBooleanHandler(Property.FLEE, b => b.IsFleeing, (b, v) => { if (v) b.Flee(); });
                AddBooleanHandler(Property.TARGET, b => b.SearchEnemyComponent?.FoundEnemyId.HasValue ?? false);
                AddPropertyHandler(NewList(Property.TARGET, Property.POSITION), VectorHandler(b => b.FleeCoordinates ?? Vector3D.Zero, (b, v) => b.CustomFleeCoordinates = v));
                AddPropertyHandler(NewList(Property.TARGET, Property.OVERRIDE), BooleanHandler(b => b.CustomFleeCoordinates.HasValue, (b, v) => { if (!v) b.CustomFleeCoordinates = null; }));
            }
        }

        public class OffensiveCombatBlockHandler : AiBlockHandler<IMyOffensiveCombatBlock> {
            public OffensiveCombatBlockHandler() {
                AddBooleanHandler(Property.TARGET, b => b.SearchEnemyComponent?.FoundEnemyId.HasValue ?? false);
            }
        }
    }
}
