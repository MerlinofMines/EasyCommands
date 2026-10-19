# Connector target and production status E2E tests

Run the exact packed Release script on a disposable dedicated server:

```powershell
./build/Test-InGame.ps1 -Suite connector-production
```

Use `-ServerRoot '<full dedicated-server installation>'` to reuse an installation.
The fixture is added only for this suite. It contains a powered refinery, a pair
of facing connectors on separate grids, and an isolated connector with no partner.

The connector scenario checks an empty target for the isolated connector, the
partner name while ready to connect, and the same name after connecting,
unlocking, and reconnecting through EasyCommands. The game's `OtherConnector`
can identify a partner before the connection is locked; `target` reports that
name whenever available. An empty result means no target is reported, rather
than merely that the connector is unlocked.

The refinery scenario starts disabled with iron ore, checks false `running`,
enables production and checks true `running`, verifies that iron ingots increase,
then disables production and checks false `running` again. This also exercises
the game's retained `IsProducing` flag after disabling a production block.

`measurements.jsonl` retains connector status and raw partner names, plus refinery
working, enabled, and producing state every second and at scenario completion.
The usual `metadata.json` records the exact script hash and Steam server build.

## Validation recorded 2026-10-19

Both scenarios passed on Steam server build **24675709**:

| Scenario | Result | Simulation ticks |
| --- | --- | ---: |
| No-target, connectable target, lock/unlock/reconnect | Pass | 1,081 |
| Disabled, producing iron, disabled again | Pass | 872 |

Local artifacts: `.build/integration/97f38ddce65a4b84b4b37b91a408a1a5/`.
The raw API confirms a named `OtherConnector` in the connectable state and a
retained `IsProducing` flag on the disabled refinery. These are handled by
reporting the available target name and requiring both `IsWorking` and
`IsProducing` for production status.

Exact package SHA-256:
`8CF385DCCECCD5D609359D5A550476B7A0245B0458FFF6658D57313DCD474FA0`.
The packed script compiles with C# 6 and passes the MDK game API checks without
warnings or errors. All **2,047 unit tests** passed, including this suite's syntax
check and connector/refinery regressions.

| Release package | Characters | Remaining below 100,000 |
| --- | ---: | ---: |
| Baseline | 94,389 | 5,611 |
| Connector target and shared production status | 94,489 | 5,511 |
| Increase | **100** | |

Counts use `.NET File.ReadAllText(scriptPath).Length`, rather than file bytes.
