# Proposal

## Why

Agents can modify ING's Avalonia Views and reusable Controls but cannot independently load them, exercise their interactions, or inspect their rendered result. Reusing the full application is not an effective development loop because its startup also runs bootstrap services and opens the game browser, while the repository currently has no Avalonia UI automation infrastructure.

## What Changes

- Add a local UI Component Automation capability that opens explicitly registered UI Component Scenarios without starting the full application or accessing real player services.
- Expose agent-facing tools to list and open scenarios, inspect components, click and enter text, capture screenshots, reset state, and close the active scenario.
- Support both page-level Views and reusable Controls as scenario roots through the same hosting and interaction contract.
- Provide isolated, repeatable scenario state with one active UI Component Automation Session per client, serialized UI operations, stable control identification, structured errors, and preserved failure state for diagnosis.
- Return structured control information and scenario-defined read-only presentation state so agents can verify behavior that is not directly visible in the rendered component.
- Return screenshots directly as agent-readable images with structured metadata.
- Cover `FleetTab` and `Materials` as the initial scenarios, including real command interaction and Views that resolve their own ViewModels through dependency injection.
- Add focused automated tests for scenario lifecycle, reusable-control hosting, and core interactions without introducing pixel-based screenshot baselines.
- Exclude full-application workflows, native WebView automation, HTTP or multi-client hosting, hot reload, arbitrary script execution, and cross-platform visual baselines from this change.

## Capabilities

### New Capabilities

- `ui-component-automation`: Isolated, scenario-driven Avalonia UI Component hosting and agent-facing interaction for local development verification.

### Modified Capabilities

None.

## Impact

- Adds an independent `Sakuno.ING.Shell.UIComponentAutomation` executable and its test project under `src/Shell/`, with dependencies on existing Shell Views, Shell Common, and ViewModels; production projects do not depend on the automation executable.
- Adds centrally managed Avalonia headless and MCP server dependencies and a project-level MCP launch declaration.
- Adds explicit scenario definitions and minimal stable automation identifiers to the initial UI Components where required.
- Updates the solution and development configuration without changing the existing desktop launcher, application bootstrap, or production runtime behavior.
- Uses the established UI Component Automation terminology in `GLOSSARY.md`.
