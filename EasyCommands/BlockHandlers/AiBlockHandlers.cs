using Sandbox.ModAPI.Ingame;

namespace IngameScript {
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
            }
        }

        public class PathRecorderBlockHandler : AiBlockHandler<IMyPathRecorderBlock> {
            public PathRecorderBlockHandler() {
                AddBooleanHandler(Property.MEDIA, b => Recorder(b).IsPlaying, (b, v) => {
                    if (v) Recorder(b).Play(); else Recorder(b).StopPlay();
                });
            }

            IMyPathRecorderComponent Recorder(IMyPathRecorderBlock block) => RequireComponent<IMyPathRecorderComponent>(block);
        }

        public class DefensiveCombatBlockHandler : AiBlockHandler<IMyDefensiveCombatBlock> {
            public DefensiveCombatBlockHandler() {
                AddBooleanHandler(Property.FLEE, b => b.IsFleeing, (b, v) => { if (v) b.Flee(); });
            }
        }
    }
}
