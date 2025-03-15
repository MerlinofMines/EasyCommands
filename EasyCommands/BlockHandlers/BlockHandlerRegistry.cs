using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public List<IMyTerminalBlock> blockCache = NewList<IMyTerminalBlock>();
        public Cache<Block, List<Object>> selectorCache = new Cache<Block, List<Object>>();
        public Cache<Block, List<Object>> groupCache = new Cache<Block, List<Object>>();
        public Cache<Type, ITerminalAction> actionCache = new Cache<Type, ITerminalAction>();
        public Cache<Type, ITerminalProperty> propertyCache = new Cache<Type, ITerminalProperty>();

        public static class BlockHandlerRegistry {
            static Dictionary<Block, IBlockHandler> blockHandlers = new Dictionary<Block, IBlockHandler> {
                { Block.AIRVENT, new AirVentBlockHandler()},
                { Block.ANTENNA, new AntennaBlockHandler()},
                { Block.ASSEMBLER, new AssemblerBlockHandler()},
                { Block.BATTERY, new BatteryBlockHandler()},
                { Block.BEACON, new BeaconBlockHandler()},
                { Block.CAMERA, new CameraBlockHandler() },
                { Block.CARGO, new CargoHandler() },
                { Block.COCKPIT, new CockpitBlockHandler<IMyCockpit>() },
                { Block.COLLECTOR, new FunctionalBlockHandler<IMyCollector>() },
                { Block.CONNECTOR, new ConnectorBlockHandler() },
                { Block.CRYO_CHAMBER, new CockpitBlockHandler<IMyCryoChamber>() },
                { Block.DECOY, new FunctionalBlockHandler<IMyDecoy>() },
                { Block.DETECTOR, new OreDetectorHandler() },
                { Block.DISPLAY, new TextSurfaceHandler() },
                { Block.DOOR, new DoorBlockHandler() },
                { Block.DRILL, new FunctionalBlockHandler<IMyShipDrill>() },
                { Block.EJECTOR, new EjectorBlockHandler() },
                { Block.ENGINE, new EngineBlockHandler<IMyPowerProducer>("Engine") },
                { Block.GENERATOR, new GasGeneratorHandler()},
                { Block.GRAVITY_GENERATOR, new GravityGeneratorBlockHandler() },
                { Block.GRAVITY_SPHERE, new SphericalGravityGeneratorBlockHandler() },
                { Block.GRID, new GridBlockHandler() },
                { Block.GRINDER, new FunctionalBlockHandler<IMyShipGrinder>() },
                { Block.GUN, new GunBlockHandler<IMyUserControllableGun>() },
                { Block.GYROSCOPE, new GyroscopeBlockHandler<IMyGyro>() },
                { Block.HEAT_VENT, new HeatVentBlockHandler() },
                { Block.HINGE, new RotorBlockHandler(IsSubType("Hinge")) },
                { Block.JUMPDRIVE, new JumpDriveBlockHandler() },
                { Block.LASER_ANTENNA, new LaserAntennaBlockHandler() },
                { Block.LIGHT, new LightBlockHandler() },
                { Block.MAGNET, new LandingGearHandler() },
                { Block.MERGE, new MergeBlockHandler() },
                { Block.PARACHUTE, new ParachuteBlockHandler() },
                { Block.PROGRAM, new ProgramBlockHandler() },
                { Block.PISTON, new PistonBlockHandler() },
                { Block.PROJECTOR, new ProjectorBlockHandler() },
                { Block.REACTOR, new EngineBlockHandler<IMyReactor>() },
                { Block.REMOTE, new RemoteControlBlockHandler()},
                { Block.REFINERY, new ProductionBlockHandler<IMyRefinery>() },
                { Block.ROTOR, new RotorBlockHandler(b => !IsSubType("Hinge")(b)) },
                { Block.SOLAR_PANEL, new EngineBlockHandler<IMySolarPanel>() },
                { Block.SORTER, new SorterBlockerHandler() },
                { Block.SOUND, new SoundBlockHandler() },
                { Block.SEARCHLIGHT, new SearchlightHandler() },
                { Block.SENSOR, new SensorBlockHandler() },
                { Block.SUSPENSION, new WheelSuspensionBlockHandler() },
                { Block.TANK, new GasTankBlockHandler() },
                { Block.TERMINAL, new TerminalBlockHandler<IMyTerminalBlock>() },
                { Block.TIMER, new TimerBlockHandler() },
                { Block.THREAD, new ThreadBlockHandler() },
                { Block.THRUSTER, new ThrusterBlockHandler()},
                { Block.TURBINE, new EngineBlockHandler<IMyPowerProducer>("WindTurbine") },
                { Block.TURRET, new TurretBlockHandler<IMyLargeTurretBase>()},
                { Block.TURRET_CONTROLLER, new TurretControlBlockHandler()},
                { Block.WARHEAD, new WarheadBlockHandler() },
                { Block.WELDER, new FunctionalBlockHandler<IMyShipWelder>() }
            };

            public static IBlockHandler GetBlockHandler(Block blockType) {
                if (!blockHandlers.ContainsKey(blockType)) throw new RuntimeException("Unsupported Block Type: " + blockType);
                return blockHandlers[blockType];
            }

            public static List<Object> GetSelf(Block? blockType) =>
                blockType == null || blockType == Block.DISPLAY || blockType == Block.GRID
                    ? blockHandlers[blockType ?? Block.PROGRAM].SelectBlocks(NewList(PROGRAM.Me))
                    : null;

            public static List<Object> GetBlocks(Block blockType, string selector = null) {
                if (blockType == Block.THREAD)
                    return blockHandlers[blockType].SelectBlocks(GetActiveThreads(), t => selector?.Equals(t.customName ?? t.name) ?? true)
                        .Select(t => ((Thread)t).originalThread)
                        .Distinct()
                        .OrderBy(t => t == PROGRAM.currentThread)
                        .OfType<Object>()
                        .ToList();

                if (PROGRAM.blockCache.Count == 0)
                    PROGRAM.GridTerminalSystem.GetBlocks(PROGRAM.blockCache);

                return PROGRAM.selectorCache.GetOrCreate(blockType, selector, s =>
                    blockHandlers[blockType].SelectBlocks(PROGRAM.blockCache, b => s?.Equals(b.CustomName) ?? true));
            }

            public static List<Object> GetBlocksInGroup(Block blockType, String groupName) =>
                PROGRAM.groupCache.GetOrCreate(blockType, groupName, s => {
                    var blocks = NewList<IMyTerminalBlock>();
                    PROGRAM.GridTerminalSystem.GetBlockGroupWithName(s)?.GetBlocks(blocks);
                    return blockHandlers[blockType].SelectBlocks(blocks);
                });

            static List<Thread> GetActiveThreads() {
                var asyncThreads = PROGRAM.asyncThreadQueue.Select(t => t.WithName("async")).ToList();
                var queuedThreads = PROGRAM.threadQueue.Skip(1).Select(t => t.WithName("queued")).ToList();
                var currentThread = PROGRAM.currentThread.WithName("current");
                var programCurrentThread = PROGRAM.currentThread;
                var childrenThreads = PROGRAM.asyncThreadQueue.Where(t => PROGRAM.currentThread == t.parentThread).Select(t => t.WithName("child")).ToList();

                return Combine(
                    Once(currentThread),
                    PROGRAM.asyncThreadQueue,
                    PROGRAM.threadQueue,
                    asyncThreads,
                    queuedThreads,
                    childrenThreads
                ).ToList();
            }
        }
    }
}
