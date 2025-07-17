using WorkflowEngine.Models;

namespace WorkflowEngine.Services;

public class WorkflowService
{
    public Dictionary<string, WorkflowDefinition> Definitions { get; set; } = new();
    public Dictionary<string, WorkflowInstance> Instances { get; set; } = new();

    public bool AddDefinition(WorkflowDefinition def)
    {
        if (string.IsNullOrWhiteSpace(def.Id) || !def.States.Contains(def.InitialState))
            return false;

        Definitions[def.Id] = def;
        return true;
    }

    public WorkflowInstance? StartInstance(string defId)
    {
        if (!Definitions.TryGetValue(defId, out var def))
            return null;

        var instance = new WorkflowInstance
        {
            DefinitionId = defId,
            CurrentState = def.InitialState
        };

        Instances[instance.Id] = instance;
        return instance;
    }

    public string? ExecuteAction(string instanceId, string action)
    {
        if (!Instances.TryGetValue(instanceId, out var inst))
            return "Instance not found.";

        if (!Definitions.TryGetValue(inst.DefinitionId, out var def))
            return "Definition not found.";

        if (!def.Transitions.TryGetValue(inst.CurrentState, out var possibleActions))
            return $"No transitions from current state: {inst.CurrentState}";

        if (!possibleActions.TryGetValue(action, out var newState))
            return $"Action '{action}' not allowed from state '{inst.CurrentState}'.";

        inst.CurrentState = newState;
        return null;
    }
}
