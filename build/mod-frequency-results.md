# Bridge update interval and missing-mod behavior

Verified 2026-10-20. The current bridge protocol is `EasyCommands.Bridge.v2`.
Replace the mod package and bridge together; the older v1 callable has a different
signature and does not carry the requested interval.

## Registration and invocation

The mod registers one hidden terminal-property definition for the programmable-block
type. Existing and newly created PBs expose that definition; the mod does not attach a
separate engine to every PB or scan newly placed blocks. The getter receives the target
PB and returns a delegate bound to it. The bridge gets/caches that callable and invokes
it synchronously. Runtime allocation and close subscriptions begin only on a successful
invocation. This API uses a getter; there is no setter registered.

Registration is attempted in BeforeStart, with the update callback retained as a fallback
if terminal controls are not ready. Normal package discovery/startup ordering remains a
separate validation gate; the server test bootstrap registers a component after session load.

## User-controlled interval

The bridge now exposes the same editable setting as the full standalone script:

```csharp
public UpdateFrequency updateFrequency = UpdateFrequency.Update10;
```

The constructor requests that interval. Each call passes it to the engine; the engine's
normal state logic returns either that interval while active or None when complete/stopped.
The bridge applies the returned frequency. Incremental parsing uses the configured interval
too, so Update100 deliberately takes longer in real time to parse the same program.
None means externally driven/manual execution, including during incremental parsing.

Actual scheduled invocation counts over 600 simulation updates were exactly 600, 60 and 6
for Update1, Update10 and Update100. Every observed inter-call interval was exactly 1, 10
or 100 updates, respectively, and the loaded engine's setting matched. None made no automatic
calls before or after manual execution, and explicit Run remained functional.

## Missing mod and recovery

A missing or incompatible API now sets Runtime.UpdateFrequency to None, emits a clear
message and explicitly rejects any submitted argument. There is no Update100 polling.
After enabling the matching mod, the user can Run again to repeat discovery; cached step
remains null until discovery succeeds. The missing-mod test observed no additional Main
calls during 180 updates and verified that explicit Run retried lookup.

The old polling fallback was a precaution for late API registration, not a requirement for
power/functional recovery. Once connected, normal power restoration resumes the native PB
schedule and uses its cached delegate. If the game recreates the script instance, the
constructor/first call perform normal registration again. Blueprint/grinding-repair lifecycle
and normal package loading have not been independently certified by these tests; there is
no mod-side background job that runs an unpowered PB.

## Evidence

- Combined compatibility, real power/destruction lifecycle and interval checks: 19/19 passed,
  `.build/mod-poc/runs/24ed0a66220748a8b15ffd482e2efe45/`.
- Focused cadence checks: 4/4 passed, `.build/mod-poc/runs/a3197acf2b7a46b296e8a579158ed396/`.
- Missing-mod no-polling/manual-retry control: passed,
  `.build/mod-poc/missing-frequency-controls/4e6dfb3ec9a14a2baab770d1a61aa57e/`.
- The final v2 package is `.build/mod-poc/staged/63d619d9127f4a879a13116ba468b222/`.
- The game's real Mod-target compile passed all 67 source files without diagnostics;
  the PB bridge compiled and executed under the PB whitelist in the live tests.
- `scheduling.json` retains raw observed intervals. Requests, results, source manifests,
  compiler diagnostics and native power/destruction traces remain in the respective runs.

The existing normal-loading/multiplayer/access/budget release gates remain unchanged.
