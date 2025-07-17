using System.Collections.Concurrent;
using WorkflowEngine.Models;

namespace WorkflowEngine.Storage;

public static class InMemoryStore
{
    public static ConcurrentDictionary<string, WorkflowDefinition> Definitions { get; } = new();

    public static ConcurrentDictionary<string, WorkflowInstance> Instances { get; } = new();
}
