using WorkflowEngine.Models;
using WorkflowEngine.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Initialize the workflow service
var service = new WorkflowService();

// Endpoint to add a new workflow definition
app.MapPost("/definitions", (WorkflowDefinition def) =>
{
    return service.AddDefinition(def)
        ? Results.Ok("Definition added successfully.")
        : Results.BadRequest("Invalid workflow definition.");
});

// Endpoint to get a workflow definition by ID
app.MapGet("/definitions/{id}", (string id) =>
{
    return service.Definitions.TryGetValue(id, out var def)
        ? Results.Ok(def)
        : Results.NotFound("Definition not found.");
});

// Endpoint to start a new instance of a workflow
app.MapPost("/instances/{definitionId}", (string definitionId) =>
{
    var instance = service.StartInstance(definitionId);
    return instance is not null
        ? Results.Ok(instance)
        : Results.BadRequest("Workflow definition not found.");
});

// Endpoint to execute an action on a workflow instance
app.MapPost("/instances/{instanceId}/actions/{actionId}", (string instanceId, string actionId) =>
{
    var error = service.ExecuteAction(instanceId, actionId);
    return error is null
        ? Results.Ok("Action executed successfully.")
        : Results.BadRequest(error);
});

// Endpoint to get a workflow instance by ID
app.MapGet("/instances/{id}", (string id) =>
{
    return service.Instances.TryGetValue(id, out var inst)
        ? Results.Ok(inst)
        : Results.NotFound("Instance not found.");
});

// Endpoint to get available actions for a workflow instance
app.MapGet("/instances/{id}/available-actions", (string id) =>
{
    if (!service.Instances.TryGetValue(id, out var inst))
        return Results.NotFound("Instance not found.");

    if (!service.Definitions.TryGetValue(inst.DefinitionId, out var def))
        return Results.NotFound("Definition not found.");

    var actions = def.Actions
        .Where(a => a.Enabled && a.FromStates.Contains(inst.CurrentState))
        .Select(a => new
        {
            a.Id,
            a.Name,
            a.Description,
            a.ToState
        });

    return Results.Ok(actions);
});

app.MapControllers();

app.Run();
