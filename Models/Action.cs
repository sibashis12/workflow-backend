namespace WorkflowEngine.Models;

public class Action
{
    public required string Id { get; set; }
    public required List<string> FromStates { get; set; }
    public required string ToState { get; set; }
    public bool Enabled { get; set; }
}
