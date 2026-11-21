# ING

Shared language for ING's KanColle browser and its development workflows.

## Language

**UI Component**:
A page-level View or reusable Control with independently verifiable presentation and interaction.
_Avoid_: Screen, widget

**UI Component Automation**:
Development verification scoped to an individual UI Component and its presentation state, without requiring the full application workflow or its external services.
_Avoid_: End-to-end automation, full-app automation

**UI Component Scenario**:
A named, representative state of a specific UI Component.
_Avoid_: Test fixture, arbitrary component state

**UI Component Automation Host**:
An isolated development environment for UI Component Scenarios, independent of the full application.
_Avoid_: Application launcher, full application host

**UI Component Automation Session**:
A disposable interaction with one UI Component Scenario and its independent state.
_Avoid_: Full application session, shared UI state
