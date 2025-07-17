namespace WorkflowEngine.Models;

public class WorkflowDefinition
{
    public string Id { get; set; } = default!;
    public List<string> States { get; set; } = new();
    public string InitialState { get; set; } = default!;
    public Dictionary<string, Dictionary<string, string>> Transitions { get; set; } = new();
}
