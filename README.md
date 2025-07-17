# Workflow Engine (Minimal Backend Service)

A minimal .NET 8 backend for managing state-machine-based workflows.

## Features
- Define workflows with configurable states and actions.
- Start workflow instances from definitions.
- Execute actions with validation (state transitions).
- Inspect current state and history of instances.

## API Endpoints

### Definitions
- `POST /workflow/definition`: Add a new workflow.
- `GET /workflow/definitions`: List all definitions.
- `GET /workflow/definitions/{id}`: Get a definition by ID.

### Instances
- `POST /workflow/instance`: Start a new instance.
- `GET /workflow/instances`: List all instances.
- `GET /workflow/instances/{id}`: Get a specific instance.

### Actions
- `POST /workflow/instance/{id}/action`: Execute an action on an instance.

## Models

Each workflow consists of:
- **States**: With `id`, `isInitial`, `isFinal`, `enabled`.
- **Actions**: With `id`, `name`, `fromStates[]`, `toState`, `enabled`.

## Folder Structure
/Controllers
    WorkflowController.cs
/Models
    State.cs
    Action.cs
    WorkflowDefinition.cs
    WorkflowInstance.cs
/Services
    WorkflowService.cs
    WorkflowStorage.cs
Program.cs
README.md


## Getting Started
Run the API:
```bash

dotnet run
