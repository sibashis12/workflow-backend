using Microsoft.AspNetCore.Mvc;
using WorkflowEngine.Models;
using WorkflowEngine.Storage;
using System.Linq;

namespace WorkflowEngine.Controllers;

[ApiController]
[Route("workflow")]
public class WorkflowController : ControllerBase
{
    // 1. Define workflow
    [HttpPost("define")]
    public IActionResult DefineWorkflow([FromBody] WorkflowDefinition definition)
    {
        if (definition == null || string.IsNullOrWhiteSpace(definition.Id))
            return BadRequest("Definition must have a non-empty Id");

        if (string.IsNullOrWhiteSpace(definition.InitialState))
            return BadRequest("InitialState must be provided");

        if (!definition.States.Any(s => s.Id == definition.InitialState))
            return BadRequest("InitialState must match the Id of one of the defined states.");

        if (definition.States.Count(s => s.IsInitial) != 1)
            return BadRequest("Workflow must contain exactly one initial state.");

        if (!InMemoryStore.Definitions.TryAdd(definition.Id, definition))
            return Conflict($"Workflow definition with Id '{definition.Id}' already exists");

        return Ok(definition);
    }

    // 2. Create an instance
    [HttpPost("instance")]
    public IActionResult CreateInstance([FromBody] CreateInstanceRequest request)
    {
        if (!InMemoryStore.Definitions.TryGetValue(request.DefinitionId, out var definition))
            return NotFound($"Workflow definition '{request.DefinitionId}' not found");

        var instance = new WorkflowInstance
        {
            DefinitionId = definition.Id,
            CurrentState = definition.InitialState
        };

        InMemoryStore.Instances[instance.Id] = instance;
        return Ok(instance);
    }

    // 3. List instances
    [HttpGet("instances")]
    public IActionResult GetInstances()
    {
        return Ok(InMemoryStore.Instances.Values);
    }

    // 4. List definitions
    [HttpGet("definitions")]
    public IActionResult GetDefinitions()
    {
        var definitions = InMemoryStore.Definitions;
        return Ok(definitions);
    }

    // 5. Execute action
    [HttpPost("execute")]
    public IActionResult ExecuteAction([FromBody] ExecutionRequest request)
    {
        if (!InMemoryStore.Instances.TryGetValue(request.InstanceId, out var instance))
            return NotFound($"Workflow instance '{request.InstanceId}' not found");

        if (!InMemoryStore.Definitions.TryGetValue(instance.DefinitionId, out var definition))
            return NotFound($"Workflow definition '{instance.DefinitionId}' not found");

        var action = definition.Actions.FirstOrDefault(a =>
            a.Id == request.ActionId &&
            a.Enabled &&
            a.FromStates.Contains(instance.CurrentState));

        if (action == null)
            return BadRequest($"Action '{request.ActionId}' cannot be performed from state '{instance.CurrentState}' or does not exist");

        instance.CurrentState = action.ToState;
        return Ok(instance);
    }

    // 6. Get workflow definition by Id
    [HttpGet("definition/{id}")]
    public IActionResult GetWorkflowDefinition(string id)
    {
        if (InMemoryStore.Definitions.TryGetValue(id, out var definition))
        {
            return Ok(definition);
        }
        return NotFound($"Workflow definition '{id}' not found");
    }
}
