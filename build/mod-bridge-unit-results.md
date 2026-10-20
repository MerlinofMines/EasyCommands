# Bridge unit validation

The test project generates a `MyGridProgram` wrapper around the exact contents of
`EasyCommands.Bridge/script.cs.txt`. It compiles that source directly; there is no
separate bridge implementation in the tests. Input changes regenerate the wrapper,
which stays under ignored `obj/` output.

All 25 bridge cases and one mod-output transport case passed. Coverage includes constructor scheduling, service and
argument forwarding, elapsed time, requested and returned update frequencies, output,
cached delegates, logging levels, exact Echo boundaries and long output, initial/reset flags, missing/null/incompatible APIs, explicit retry,
lookup/invocation failures, and independent bridge instances.

The full suite passed 2,089 tests with zero failures. Retained local results:
`.build/test-results/bridge-v3-unit.trx` and `.build/test-results/bridge-v3-full.trx`.
The generator's input dependency was also verified by regenerating after changing the
bridge timestamp and checking that the shipped source remained unchanged.

Run the focused checks from the repository root:

```powershell
dotnet test EasyCommands.Tests/EasyCommands.Tests.csproj -c Release -p:Platform=x64 -p:MdkBuildConfiguration=NoPack --filter FullyQualifiedName~EasyCommands.Tests.Bridge
```

These are mocked PB contract tests. Actual scheduler cadence, power recovery and
warhead cleanup remain covered by the disposable game-server tests documented in
`mod-frequency-results.md` and `mod-lifecycle-results.md`.
