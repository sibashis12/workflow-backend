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

    public string? ExecuteAction(string instanceId, string actionId)
    {
        if (!Instances.TryGetValue(instanceId, out var inst))
            return "Instance not found.";

        if (!Definitions.TryGetValue(inst.DefinitionId, out var def))
            return "Definition not found.";
        var action = def.Actions.FirstOrDefault(a =>
            a.Id == actionId &&
            a.Enabled &&
            a.FromStates.Contains(inst.CurrentState));

        if (action == null)
            return $"Action '{actionId}' not allowed from state '{inst.CurrentState}'.";

        inst.CurrentState = action.ToState;
        return null;
    }
}
