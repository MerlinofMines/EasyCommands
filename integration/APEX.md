# Apex Survival E2E tests

These scenarios ship with the Apex handlers in PR #281 and run locally on Windows:

    ./build/Test-InGame.ps1 -Suite apex

Run an individual check with existing builds:

    ./build/Test-InGame.ps1 -Suite apex -Scenario seed-alias-transfer -SkipBuild

The seven scenarios cover Farm Plot controls, algae output, Food Processor controls,
exact seed transfers with fruit as a decoy, Irrigation System controls, empty crop
state, and the kelp-crisp recipe blueprint queue. The harness creates the required
powered fixture automatically; no player connection is needed.

GitHub Actions validates both smoke and Apex scenario syntax as part of unit tests.
It does not install or launch a dedicated server for E2E execution.
See README.md for custom saves, separate script artifacts, diagnostics, and timeouts.
