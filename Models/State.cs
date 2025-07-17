namespace WorkflowEngine.Models;

public class State
{
    public required string Id { get; set; }
    public string Name { get; set; } = default!;
    public bool IsInitial { get; set; }
    public bool IsFinal { get; set; }
    public bool Enabled { get; set; } = true;//TODO-> add Enabled Functionality
    public string? Description { get; set; }
}
