namespace WorkflowEngine.Models;

public class ExecutionRequest
{
    public string InstanceId { get; set; } = default!;
    public string ActionId { get; set; } = default!;
}
