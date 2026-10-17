# AI E2E scenarios

Run `./build/Test-InGame.ps1 -Suite ai` for binding checks on the stationary rig.
Run `./build/Test-InGame.ps1 -Suite ai-motion` for physical flight checks.
Supply `-ServerRoot` when using an existing full dedicated-server installation.
Use `-SkipBuild` only after building the current harness and EC Release package.

The motion suite spawns a separate disposable dynamic grid with a charged battery,
fueled reactor, gyro, six ion-thrust directions, Flight, Recorder, Defensive and
the test programmable block. A separately owned powered grid is placed 1 km away
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

These cover navigation through empty space. Obstacle avoidance, moving reference
beacons, docking and actual weapons engagements require additional fixtures.

The 2026-10-17 final run passed all three scenarios. See
[measurement and results](../build/ai-support-measurement.md) for distances,
package identity and retained artifacts.
