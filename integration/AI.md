# AI E2E scenarios

Run `./build/Test-InGame.ps1 -Suite ai` for binding checks on the stationary rig.
Run `./build/Test-InGame.ps1 -Suite ai-motion` for physical flight checks.
Supply `-ServerRoot` when using an existing full dedicated-server installation.
Use `-SkipBuild` only after building the current harness and EC Release package.

The motion suite spawns a separate disposable dynamic grid with a charged battery,
fueled reactor, six ion-thrust directions, Flight, Recorder, Defensive, Offensive and
the test programmable block. Six real gyros provide turn authority for repeated
reversals. A separately owned powered grid is placed 1 km away
as a detectable target. No method forces a detected target or injects flee state.

The harness configures a 150 m waypoint and retreat destination through ModAPI.
The exact packed EC script runs all behavior, speed, precision, avoidance,
playback and flee commands through PB-accessible APIs.

- **Route arrival:** start playback and allow 60 seconds of flight.
- **Stop/resume:** start playback, stop after 6 seconds, wait 12 seconds, restart,
  then allow 60 seconds for arrival.
- **Threat retreat:** activate movement and defense, allow 12 seconds for natural
  detection, request flee, then allow 60 seconds to retreat. Automatic flee is
  disabled so it cannot substitute for the EC request.

The harness independently checks displacement over 100 m, final distance below
12 m from the configured destination, and final speed below 1 m/s. Recorder cases
also require observed playback; stop/resume requires observed stopping. The
defensive case requires observed target acquisition and positive flee state.
`motion.jsonl` records position, speed, destination distance, playback/flee state,
target ID, and Flight/Defensive detail text every 30 simulation ticks.
The compact rig disables exhaust damage; all six thrust directions must remain
functional with nonzero effective thrust. The stop/resume check must observe
speed below 0.5 m/s during the explicit pause, before playback resumes.

The original scenarios cover navigation through empty space. The extended suites
below add terrain, obstacle and target-loss fixtures. Moving reference beacons,
docking and actual weapons engagements remain outside this coverage.

## Extended behavior scenarios

- `-Suite ai-motion -Scenario ai-repeat-route-lifecycle`: a 60 m outbound/return
  route runs with repeat enabled for 140 seconds. EC then disables repeat and
  allows the current sequence to finish. At least two visits to each end, final
  playback/repeat off, return within 20 m and speed below 1 m/s are required.
- `-Suite ai-planet`: generates a real 6 km Moon, starts the drone banked about
  36 degrees, and commands minimum altitude 200 m followed by 100 m. Gravity
  alignment is enabled by EC with terminal pitch/roll limits set to zero. The rig
  has a second fueled reactor and seven ion thrusters in each vertical direction.
  The test requires natural gravity, clearance above 180 m throughout the
  20–60 second high-floor window, final clearance of 85–130 m after lowering the
  floor, final alignment within 5 degrees, clearance above 30 m throughout, and healthy
  thrust directions. The rig stays static only during compilation and converts
  normally to a ship before executing EC; no motion/orientation is forced in flight.
- `-Suite ai-obstacle`: an armor cube blocks a 300 m route. EC enables collision
  avoidance. The AI must approach and stop with playback off before the armor,
  remain collision-free, and retain over 25 m PB-to-obstacle clearance. At 45 s
  the harness removes the obstruction; at 50 s EC restarts playback. Arrival and
  a final speed below 1 m/s are required. This checks safe obstruction handling
  and recovery; the game did not find a detour around this solid-grid fixture.
- `-Suite ai-motion -Scenario ai-target-loss`: both combat blocks must acquire
  the target. The harness then removes that grid, and both EC target flags must
  clear naturally.

The waypoint helper supplies valid orientation matrices. The game's AddWaypoint
RPC marks even coordinate-only waypoints as having a recorded rotation; zero-axis
matrices produced invalid test routes. Planetary waypoints explicitly use GPS
semantics without forced rotation so recorded poses cannot satisfy the gravity
alignment assertion independently of the EC control.

The original three motion scenarios passed on 2026-10-17. See
[measurement and results](../build/ai-support-measurement.md) for distances,
package identity and retained artifacts.

## Additional controls

`-Suite ai -Scenario ai-extended-settings` checks minimum altitude, planetary
gravity alignment, route-repeat toggle/readback, retreat override/clear and
false target detection on the stationary rig. Planetary alignment is a setting
roundtrip on this stationary rig; `ai-planet` separately exercises natural gravity
and terrain clearance on the generated Moon.

`-Suite ai-motion -Scenario ai-detection-and-retarget` checks natural target
acquisition on Offensive, switches to Defensive and checks its target flag, then
sets the retreat destination through EC before requesting flee. The harness
initially supplies a deliberately wrong destination in the opposite direction;
the independent arrival checks require the EC-selected point. The powered target
is separately owned, with neutral/enemy targeting enabled in both combat blocks.
