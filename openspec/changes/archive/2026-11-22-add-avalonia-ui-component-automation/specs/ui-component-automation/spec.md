# Spec Delta

## Purpose

Provides isolated, repeatable Avalonia UI Component Scenarios that agents can inspect, operate, and capture during local development of page-level Views and reusable Controls without starting the full application.

## ADDED Requirements

### Requirement: Explicit scenario catalog
The system SHALL expose stable identifiers and UI Component names for explicitly registered UI Component Scenarios and SHALL reject unregistered scenario identifiers.

#### Scenario: List available scenarios
- **WHEN** an agent requests the scenario catalog
- **THEN** the system returns every registered scenario with its stable identifier and associated UI Component name

#### Scenario: Reject an unknown scenario
- **WHEN** an agent requests a scenario identifier that is not registered
- **THEN** the system returns a scenario-not-found error and leaves the current session unchanged

### Requirement: Page and reusable component roots
The system SHALL host both page-level Views and reusable Controls as scenario roots through the same inspection, interaction, and capture contract.

#### Scenario: Open a reusable Control scenario
- **GIVEN** a registered scenario has a reusable Control as its root
- **WHEN** an agent opens that scenario
- **THEN** the system renders the Control independently and makes the standard component operations available to the agent

### Requirement: Isolated scenario state
The system SHALL construct each UI Component Scenario with independent presentation state and substituted external services, without starting the full application or accessing real player data.

#### Scenario: Open a scenario independently
- **WHEN** an agent opens a registered scenario
- **THEN** the system loads its real UI Component and presentation state without running application bootstrap, opening the game browser, or calling real player services

#### Scenario: Prevent cross-scenario state sharing
- **WHEN** an agent opens one scenario after another
- **THEN** mutable state from the first scenario is not visible in the second scenario

### Requirement: Single active session
The system SHALL maintain at most one active UI Component Automation Session per agent client and SHALL replace the existing session when another scenario is opened successfully.

#### Scenario: Replace the active scenario
- **GIVEN** a scenario is active
- **WHEN** an agent opens a different scenario successfully
- **THEN** the system disposes the previous session and makes the new scenario the only active session

#### Scenario: Operations require an active scenario
- **WHEN** an agent requests a UI operation while no scenario is active
- **THEN** the system returns a no-active-scenario error without starting an implicit scenario

### Requirement: Deterministic session reset and closure
The system SHALL reset an active scenario to a newly constructed initial state and SHALL dispose session resources when the session is closed.

#### Scenario: Reset after interaction
- **GIVEN** an agent has changed an active scenario through interaction
- **WHEN** the agent resets the scenario successfully
- **THEN** the system replaces the UI Component and its state with a fresh instance of the same scenario

#### Scenario: Close the active scenario
- **GIVEN** a scenario is active
- **WHEN** the agent closes the scenario
- **THEN** the system releases its UI Component, presentation state, and owned services and reports no active scenario

### Requirement: Serialized UI operations
The system SHALL execute operations on an active UI Component in request order and SHALL complete required layout processing before reporting an operation as complete.

#### Scenario: Concurrent agent requests
- **GIVEN** a scenario is active
- **WHEN** multiple UI operations arrive concurrently
- **THEN** the system serializes their execution and returns results in an order consistent with the operations performed

### Requirement: Scenario inspection
The system SHALL return structured control information and scenario-defined read-only presentation state derived from the real scenario objects.

#### Scenario: Inspect a rendered scenario
- **GIVEN** a scenario has completed layout
- **WHEN** an agent inspects its controls
- **THEN** the system returns stable identifiers, control types, display text, visibility, enabled state, and layout information sufficient to identify actionable controls

#### Scenario: Inspect non-visual presentation state
- **GIVEN** an active scenario has presentation state that is not directly displayed by its UI Component
- **WHEN** an agent inspects the scenario
- **THEN** the system returns the scenario's declared read-only state snapshot derived from its real presentation objects

### Requirement: Semantic control interaction
The system SHALL click controls and enter text using unique `AutomationId` or `Name` identifiers and SHALL reject missing, disabled, invisible, or ambiguous targets.

#### Scenario: Click a uniquely identified control
- **GIVEN** a scenario contains an enabled control with a unique stable identifier
- **WHEN** an agent clicks that identifier
- **THEN** the system invokes the control's real interaction behavior and makes the resulting state available for inspection

#### Scenario: Reject an ambiguous target
- **GIVEN** more than one control matches an interaction identifier
- **WHEN** an agent attempts to operate that identifier
- **THEN** the system returns an ambiguous-control error and does not operate any matching control

#### Scenario: Enter text into a control
- **GIVEN** a scenario contains an enabled text input with a unique stable identifier
- **WHEN** an agent enters text into that identifier
- **THEN** the system updates the real input control and its bound presentation state

### Requirement: Direct screenshot capture
The system SHALL return an agent-readable image of the active UI Component after layout, together with structured capture metadata.

#### Scenario: Capture the active UI Component
- **GIVEN** a scenario is active and has completed layout
- **WHEN** an agent requests a screenshot
- **THEN** the system returns the rendered image, its dimensions, and the active scenario identifier

### Requirement: Failure diagnosis with preserved state
The system SHALL return structured operation errors and preserve the previous active scenario after recoverable operation, opening, or reset failures so the agent can inspect or capture the failure state.

#### Scenario: Inspect after a failed operation
- **GIVEN** a scenario is active
- **WHEN** an operation fails because its target cannot be found or operated
- **THEN** the system returns the failure category and keeps the same scenario available for inspection and screenshot capture

#### Scenario: Preserve the previous scenario after opening failure
- **GIVEN** a scenario is active
- **WHEN** constructing a different scenario fails
- **THEN** the system returns the construction error and preserves the previous active scenario without exposing a partial replacement

#### Scenario: Preserve the previous scenario after reset failure
- **GIVEN** a scenario is active
- **WHEN** constructing its replacement during reset fails
- **THEN** the system returns the construction error and preserves the previous active scenario without exposing a partial replacement

### Requirement: Initial real UI Component coverage
The system SHALL provide representative scenarios for `FleetTab` and `Materials` that exercise their real presentation behavior.

#### Scenario: Select a fleet in FleetTab
- **GIVEN** the `FleetTab` scenario has multiple fleets in its presentation state and displays an unselected fleet tab
- **WHEN** an agent clicks that fleet tab
- **THEN** inspection reports the displayed fleet as selected through the real selection state and the updated View can be captured

#### Scenario: Render Materials with resolved presentation state
- **WHEN** an agent opens and inspects the representative `Materials` scenario
- **THEN** the View obtains its presentation state through its normal dependency-injection path and inspection reports the supplied material values for visual capture
