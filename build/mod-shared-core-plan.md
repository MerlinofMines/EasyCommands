# One EasyCommands engine, two deployment targets

## Recommendation and measured scope

Maintain the standalone programmable-block script and the source mod in this repository,
compiling the same interpreter sources into both artifacts. Keep the existing language,
block handlers, variables, cooperative threads, IGC behavior and incremental parsing shared.
Use small target-specific host files for the game boundary. Do not maintain an independently
edited copy of the interpreter in the mod directory.

The current PoC already demonstrates source sharing: 64 interpreter files are staged from
`EasyCommands/`, and only three receive small content changes. The other 61 are copied
unchanged (line-ending/encoding normalization is not a semantic edit). No production
interpreter files were modified for the PoC. Two new mod-host files and the small PB bridge
provide the surrounding runtime. This is a file count, not a measured percentage of code lines.

| Existing source | Current PoC adaptation | Permanent shared-source seam |
| --- | --- | --- |
| `Program.cs` | Different base class/constructor; save and restore active `PROGRAM` | Target-specific base/constructor partial files; common initialization and execution with scoped context |
| `CommandParsers/ParameterProcessors.cs` | Remove its repeated `MyGridProgram` base declaration | Remove the redundant base declaration; select inheritance only in the host entry file |
| `BlockHandlers/ProgrammableBlockHandlers.cs` | Replace two ingame `TryRun` calls with the mod API | Shared handler delegates invocation to one target-specific host method |

The tokenizer, parameter parsing, command graph, execution queues, variable semantics and
all other handlers currently run without algorithm changes. Packaging does not prove every
handler works; validation must cover runtime use, API behavior and ownership as well.

## A low-impact source layout

Keep the existing `EasyCommands` project and MDK distribution path working. There is no need
to rename/reorganize all the nested types and partial `Program` modules to start the migration.

```text
EasyCommands/
  EasyCommands.csproj              # existing standalone PB + MDK packaging
  Program.cs                      # shared state, configuration, initialization and Main
  CommandParsers/                  # shared
  Commands/                       # shared
  Common/                         # shared
  BlockHandlers/                  # shared, including the PB handler
  Hosting/PbProgram.cs             # MyGridProgram inheritance, constructor, PB Run adapter
EasyCommands.Mod/
  EasyCommands.Mod.csproj          # explicitly links the shared source set
  Hosting/ModProgram.cs            # mod facade inheritance, constructor, mod Run adapter
  Hosting/ModHostContext.cs        # borrowed PB services and runtime/output facade
  ModSession.cs                   # property registration, per-PB state, lifecycle, server checks
EasyCommands.Bridge/
  script.cs.txt                   # separately shipped tiny PB editor body
```

The shared host split and dual-target build are now implemented. Packaging uses the mod project's evaluated Compile items, with byte-identical source copies. Public-release validation remains incomplete. The PB build must include only
its host entry file; the mod build must exclude that file and include its own. Use explicit
MSBuild include/exclude rules with a common source-item definition where useful. The mod
package builder copies the selected sources into `Data/Scripts/EasyCommands/`; it must not
rely on project links existing on the player's machine. Avoid scattering target conditionals
through parser and handler code. Keep any necessary target selection at the host/build boundary.

Sharing here means **sharing source**, not distributing one common application DLL. MDK still
produces the self-contained PB script; the game compiles the source-mod package separately.
Both builds retain the game's supported language/API constraints.

## Host behavior to isolate

- PB identity and Custom Data access through the current ingame interface types.
- The invoking PB's terminal-system scope and vanilla IGC instance. Continue borrowing these
  synchronously from the bridge; clear invocation references on exit.
- Elapsed simulation time, requested update frequency and output/Echo.
- Requests to run another PB, including disabled/busy/unpowered targets and nested calls.
- Mod-only registration, per-entity runtime management, unload and block-removal cleanup.

The PB Run seam needs particular care: ingame `TryRun` returns a result while mod `Run` is
void, and the existing handler ignores `TryRun`'s return value. The PoC's helper throws for
some rejected targets. Normalize this deliberately to the existing command semantics and
test rejection cases; do not treat compilation success as proof of equivalent behavior.

The existing engine relies on a static active `PROGRAM`. Separate full-PB assemblies masked
the sharing issue; one mod assembly requires scoped save/restore and a same-instance entry
guard. The PoC tests nested calls and instance isolation with that adaptation. Keep execution
on the simulation thread. A subsequent cleanup can pass explicit instance context to helper
objects; that touches more of the core and should be separate from the initial host split.
Do not claim the entire core is already independent of its host or safe for background threads.

## Parsing and execution policy

Preserve incremental parsing across calls, including Custom Data edits, with independent
state and bounded parsing work for each PB. The user explicitly accepts parsing latency to
protect other programs. Keep current parser defaults in the first shared release. Do not
introduce eager parsing of every PB during a session callback.

The first mod version should retain PB-driven scheduling and normal power/enable behavior.
A mod-owned scheduler, a new physical block, persistence of active execution, or a different
IGC transport are separate features. Add explicit mod execution budgets before a public
release; moving work outside PB instrumentation must not allow unbounded simulation stalls.

## Build, test and release workflow

1. Establish parity on the existing staged PoC, retaining exact source manifests and results.
2. Replace text replacements in `stage.py` with the host partials and source selection above.
   Keep the standalone project compiling and the existing PB test factory's parameterless
   constructor/base-class contract intact.
3. Build both targets for every shared-engine change. Check the PB whitelist, packed-source
   size and real Mod-target compilation, rather than ordinary C# compilation alone.
4. Run language/unit tests and shared scenario inputs against both hosts where applicable.
   Adapt the test factory for a mod host rather than importing its reflection-based PB
   initialization into the shipped mod. Run matching in-game scenarios through the full
   PB and bridge paths and compare outcomes.
5. Gate lifecycle and access differences explicitly: missing mod, Run arguments, timer/sensor/
   toolbar activation, rejected PB calls, Custom Data edits, recompile, power loss, block
   removal, save/reload, ownership changes, dedicated server and client join/reconnect.
6. Keep concurrent regression cases: ten-PB host parity, fifty mod-backed PBs, exact IGC
   counts, nested calls, unique instance state and reloads while other programs execute.
7. Ship two clearly labeled artifacts from the same engine revision: standalone PB script;
   source mod plus versioned PB bridge. Put build/revision and bridge-protocol information
   in diagnostics. The standalone artifact continues to run without a mod.

A single engine bug fix should normally be one source change, two builds and shared tests.
Mod lifecycle/authorization and PB packaging remain target-specific maintenance. Features
requiring APIs unavailable to a PB must be marked mod-only and capability-checked rather
than silently changing the shared language's behavior. The standalone 100,000-character
ceiling remains a constraint on shared feature growth even though the mod engine is free
of that limit. The separate Custom Data limit is also unchanged.

## Validation status

The [initial PoC](mod-poc-results.md) and [concurrent tests](mod-stress-results.md) establish
real sandbox compilation, the synchronous PB/mod boundary and substantial language/runtime
parity. The [broader non-motion suites](mod-broad-results.md) have passing evidence for 26
distinct scenarios, with one corrected crop test assumption. Normal mod-package loading and complete
multiplayer/trigger/lifecycle coverage remain release gates.
