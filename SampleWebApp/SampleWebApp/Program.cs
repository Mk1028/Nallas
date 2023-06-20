using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;
using Microsoft.OpenApi.Models;
using Azure.Storage.Queues;
using System.Text.Json;
using SampleWebApp.Services;
using Microsoft.Azure.Cosmos;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<JiraTaskDb>(opt => opt.UseInMemoryDatabase("JiraTask"));
// Register JiraTaskDb as a singleton
builder.Services.AddSingleton<JiraTaskDb>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddCors();

// Configure Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHostedService<ReadMessageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "JiraTasks API V1");
		c.RoutePrefix = string.Empty;
	});
}

// Enable CORS
app.UseCors(builder => builder
	.AllowAnyOrigin()
	.AllowAnyMethod()
	.AllowAnyHeader());

// Get Requests
app.MapGet("/jiratasks", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync()).ToList());

app.MapGet("/jiratasks/todo", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync())
	.Where(t => t.Status == JiraStatuses.ToDo)
	.ToList());

app.MapGet("/jiratasks/inprogress", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync())
	.Where(t => t.Status == JiraStatuses.InProgress)
	.ToList());

app.MapGet("/jiratasks/codereview", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync()).Where(t => t.Status == JiraStatuses.CodeReview).ToList());

app.MapGet("/jiratasks/testing", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync()).Where(t => t.Status == JiraStatuses.Testing).ToList());

app.MapGet("/jiratasks/done", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync()).Where(t => t.Status == JiraStatuses.Done).ToList());

app.MapGet("/jiratasks/person1", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync()).Where(t => t.AssignedTo == Assignees.Person1).ToList());

app.MapGet("/jiratasks/person2", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync()).Where(t => t.AssignedTo == Assignees.Person2).ToList());

app.MapGet("/jiratasks/person3", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync()).Where(t => t.AssignedTo == Assignees.Person3).ToList());

app.MapGet("/jiratasks/person4", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync()).Where(t => t.AssignedTo == Assignees.Person4).ToList());

app.MapGet("/jiratasks/person5", async (JiraTaskDb db) =>
	(await db.GetJiraTasksAsync()).Where(t => t.AssignedTo == Assignees.Person5).ToList());

app.MapGet("/jiratasks/{id}", async (Guid id, JiraTaskDb db) =>
{
	var jiraTask = await db.GetJiraTaskByIdAsync(id.ToString());

	if (jiraTask != null)
	{
		return Results.Ok(jiraTask);
	}
	else
	{
		return Results.NotFound();
	}
});


// Post Requests
app.MapPost("/jiratasks", async (JiraTask inputJiraTask, JiraTaskDb db) =>
{
	/*// Sending message to the queue in Azure
	string connectionString = "";
	string queueName = "myqueue-items";
	var queueClient = new QueueClient(connectionString, queueName);

	string jsonData_JiraTask = JsonSerializer.Serialize(inputJiraTask);

	await queueClient.SendMessageAsync(jsonData_JiraTask);*/

	var validator = new JiraTaskValidator();
	ValidationResult result = await validator.ValidateAsync(inputJiraTask);

	if (!result.IsValid)
	{
		return Results.BadRequest(result.Errors);
	}

	await db.AddJiraTaskAsync(inputJiraTask);

	return Results.Created($"/jiratasks/{inputJiraTask.Id}", inputJiraTask);
});

// Put Requests
app.MapPut("/jiratasks/{id}", async (Guid id, JiraTask inputJiraTask, JiraTaskDb db) =>
{
	var validator = new JiraTaskValidator();
	ValidationResult result = await validator.ValidateAsync(inputJiraTask);

	if (!result.IsValid)
	{
		return Results.BadRequest(result.Errors);
	}

	var jiraTask = await db.GetJiraTaskByIdAsync(id.ToString());

	if (jiraTask is null)
	{
		return Results.NotFound();
	}

	/*jiraTask.Name = inputJiraTask.Name;
	jiraTask.Status = inputJiraTask.Status;
	jiraTask.AssignedTo = inputJiraTask.AssignedTo;
	jiraTask.Description = inputJiraTask.Description;*/

	await db.UpdateJiraTaskAsync(inputJiraTask);

	return Results.Created($"/jiratasks/{inputJiraTask.Id}", inputJiraTask);
});


//Delete Requests
app.MapDelete("/jiratasks/{id}", async (Guid id, JiraTaskDb db) =>
{
	if (await db.GetJiraTaskByIdAsync(id.ToString()) is JiraTask jiraTask)
	{
		await db.DeleteJiraTaskAsync(id.ToString());
		return Results.Ok(jiraTask);
	}

	return Results.NotFound();
});

app.UseStaticFiles();
app.Run();
