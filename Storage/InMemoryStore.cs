using System.Collections.Concurrent;
using WorkflowEngine.Models;

// This is a simple in-memory store. In production, this could be replaced with a real DB.

namespace WorkflowEngine.Storage;

public static class InMemoryStore
{
    public static ConcurrentDictionary<string, WorkflowDefinition> Definitions { get; } = new();

    public static ConcurrentDictionary<string, WorkflowInstance> Instances { get; } = new();
}
