# Minified character headroom

Measured on 2026-10-15 against `origin/master` at `fabb742`, using the existing
MDK2 Release configuration and the installed game assemblies in
`C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64`.
The game-reference configuration and project dependencies were not changed.

| Version | Characters | Remaining under 100,000 |
| --- | ---: | ---: |
| Fresh baseline | 93,002 | 6,998 |
| All four reductions | 92,183 | 7,817 |

The combined saving is **819 characters**, increasing available headroom by
about **12%**. Counts use `File.ReadAllText(scriptPath).Length`, rather than file
bytes, matching the existing packed-script check.

## Changes and extension points

- `Synonyms("up upward upwards upper")` replaces separate string literals in
  parser registrations. `Words(...)` still preserves multiword item phrases such
  as `"scrap metal"`. Plural registrations continue to append `s` to each word.
- `SetDefaultProperty` groups a primitive default and its directions, or several
  direction defaults. Assignments still override inherited defaults. The public
  dictionaries remain available for specialized handlers.
- `AddPropertyAliases` registers independent properties against one handler.
  `AddPropertyHandler(List<Property>, ...)` still registers one compound property;
  explicit compounds such as `build complete` remain separate registrations.
- `SelectorParameter` and `SelectorName` share repeated selector construction.
  Parser rule ordering, required/optional parameters, and ambiguity branching
  remain explicit in the rule table.

Before the minimal `Synonyms` simplification, each change was disabled in a temporary source copy while retaining the
other three changes, then repackaged with the same settings:

| Change disabled | Characters without it | Marginal saving from the change |
| --- | ---: | ---: |
| Compact synonyms | 92,875 | 642 |
| Grouped defaults | 92,255 | 22 |
| Property aliases | 92,302 | 69 |
| Selector factories | 92,274 | 41 |

These earlier marginal savings do not include the subsequent 50-character saving from simplifying `Synonyms`, and do not sum exactly because MDK
assigns short identifiers across the whole script.

## Validation

- Release source build and packaging: zero warnings or errors.
- Exact packaged script: compiled with C# 6, local game references, and the MDK
  game API analyzer, with zero warnings or errors.
- All 1,776 tests passed, including three new registration regressions covering
  inherited/directional defaults and independent versus compound aliases.
  Tests used the repository test sources in a temporary offline project with
  cached MSTest/Moq dependencies and the installed Visual Studio VSTest runner.
  No test-project configuration changes were required.
- All 255 literal synonym lists were compared with the baseline source and
  retained identical words and order.
- `git diff --check` passed.

The final packaged artifact SHA-256 was
`B32CFC3E888489770321433A23E90F0D128C817F620207F8FA8EA2630E3E643B`.

See [the build instructions](README.md) for normal Release packaging and the
packed-script check. The generated measurement copies and test artifacts are
under the ignored `.build/minification` directory. Validation does not include
execution inside a running game.
