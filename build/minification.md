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

## Visibility redaction (2026-10-18)

Measured against `master` at `478c291`, using the existing MDK2 full-minification
configuration and local dedicated-server assemblies (Steam build 24675709):

| Package | Characters | Remaining under 100,000 |
| --- | ---: | ---: |
| MDK output | 98,176 | 1,824 |
| After visibility redaction | 95,985 | 4,015 |

Removing 313 redundant `public` modifiers saves **2,191 characters**. Release
builds now run `Redact-ScriptVisibility.ps1` automatically after MDK packaging.
PowerShell 7 supplies the Roslyn parser. Only direct `Program` declarations are
eligible; constructors, game entry points, editable configuration, overrides,
virtual/extern methods, operators, and nested member accessibility are preserved.
The script verifies that all other syntax tokens are identical before writing.
Source assembly accessibility is unchanged, and Debug builds skip the pass.

Validation:

- Release and Debug builds passed with zero warnings/errors.
- All 1,829 unit tests passed.
- Visibility regressions passed for host APIs, nested members, literals/comments,
  preprocessor trivia, idempotence, and rejection of invalid input.
- The exact packed script passed standalone C# 6 compilation and MDK API analysis.
- The exact package was compiled and executed by the real programmable block in
  three E2E scenarios: farm selector, seed transfer, and recipe blueprint queueing.
  All passed (10, 11, and 10 simulation ticks respectively).
- `git diff --check` passed.

The E2E run is retained locally under
`.build/integration/54dfc20377f34d7ea372b914f7b4c60b/`. Its deployed source and
metadata identify package SHA-256
`6C74FFAF8F637559917477A0450D783068FC406D6A878932E54B889A65529AE8`.
