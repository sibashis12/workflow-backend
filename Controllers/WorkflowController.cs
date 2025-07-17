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
        if (string.IsNullOrWhiteSpace(definition.Id))
            return BadRequest("Definition must have a non-empty Id");

        if (string.IsNullOrWhiteSpace(definition.InitialState) || !definition.States.Contains(definition.InitialState))
            return BadRequest("InitialState must be one of the defined states");

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

    // 3. List instances -> TO DO
    [HttpGet("instances")]
    public IActionResult GetInstances()
    {
        return Ok(InMemoryStore.Instances.Values);
    }

    //4. Execute Instances
    [HttpPost("execute")]
    public IActionResult ExecuteAction([FromBody] ExecutionRequest request)
    {
        if (!InMemoryStore.Instances.TryGetValue(request.InstanceId, out var instance))
            return NotFound($"Workflow instance '{request.InstanceId}' not found");

        if (!InMemoryStore.Definitions.TryGetValue(instance.DefinitionId, out var definition))
            return NotFound($"Workflow definition '{instance.DefinitionId}' not found");

        if (!definition.Transitions.TryGetValue(request.ActionId, out var transitionMap))
            return BadRequest($"Action '{request.ActionId}' not defined in workflow '{definition.Id}'");

        if (!transitionMap.TryGetValue(instance.CurrentState, out var newState))
            return BadRequest($"Action '{request.ActionId}' cannot be performed from state '{instance.CurrentState}'");

        instance.CurrentState = newState;
        return Ok(instance);
    }

    // 5. Get workflow definition by Id
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
