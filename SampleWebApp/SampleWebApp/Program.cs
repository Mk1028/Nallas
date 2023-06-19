using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;
using Microsoft.OpenApi.Models;
using Azure.Storage.Queues;
using System.Text.Json;
using SampleWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<JiraTaskDb>(opt => opt.UseInMemoryDatabase("JiraTask"));
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

//Get Requests
app.MapGet("/jiratasks", async (JiraTaskDb db) =>
	await db.JiraTasks.ToListAsync());

app.MapGet("/jiratasks/todo", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.Status == JiraStatuses.ToDo).ToListAsync());

app.MapGet("/jiratasks/inprogress", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.Status == JiraStatuses.InProgress).ToListAsync());

app.MapGet("/jiratasks/codereview", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.Status == JiraStatuses.CodeReview).ToListAsync());

app.MapGet("/jiratasks/testing", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.Status == JiraStatuses.Testing).ToListAsync());

app.MapGet("/jiratasks/done", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.Status == JiraStatuses.Done).ToListAsync());

app.MapGet("/jiratasks/person1", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.AssignedTo == Assignees.Person1).ToListAsync());

app.MapGet("/jiratasks/person2", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.AssignedTo == Assignees.Person2).ToListAsync());

app.MapGet("/jiratasks/person3", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.AssignedTo == Assignees.Person3).ToListAsync());

app.MapGet("/jiratasks/person4", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.AssignedTo == Assignees.Person4).ToListAsync());

app.MapGet("/jiratasks/person5", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.AssignedTo == Assignees.Person5).ToListAsync());

app.MapGet("/jiratasks/{id}", async (int id, JiraTaskDb db) =>
	await db.JiraTasks.FindAsync(id)
		is JiraTask jiratask
			? Results.Ok(jiratask)
			: Results.NotFound());

// Post Requests
app.MapPost("/jiratasks", async (JiraTask inputJiraTask, JiraTaskDb db) =>
{
	/*// Sending message to the queue in Azure
	string connectionString = "DefaultEndpointsProtocol=https;AccountName=samplewebapp;AccountKey=VQKO7WrFjCDHeYm2kZCtgOBFIPTLCMqFwLua7gRdWyAfE/cW4C7A8CVPkwrhzUfrT6wlCJ2NLNHx+AStD91ymA==;EndpointSuffix=core.windows.net";
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

	db.JiraTasks.Add(inputJiraTask);
	await db.SaveChangesAsync();

	return Results.Created($"/jiratasks/{inputJiraTask.Id}", inputJiraTask);
});

// Put Requests
app.MapPut("/jiratasks/{id}", async (int id, JiraTask inputJiraTask, JiraTaskDb db) =>
{
	var validator = new JiraTaskValidator();
	ValidationResult result = await validator.ValidateAsync(inputJiraTask);

	if (!result.IsValid)
	{
		return Results.BadRequest(result.Errors);
	}

	var jiraTask = await db.JiraTasks.FindAsync(id);

	if (jiraTask is null)
	{
		return Results.NotFound();
	}

	jiraTask.Name = inputJiraTask.Name;
	jiraTask.Status = inputJiraTask.Status;
	jiraTask.AssignedTo = inputJiraTask.AssignedTo;
	jiraTask.Description = inputJiraTask.Description;

	await db.SaveChangesAsync();

	return Results.Created($"/jiratasks/{inputJiraTask.Id}", inputJiraTask);
});


//Delete Requests
app.MapDelete("/jiratasks/{id}", async (int id, JiraTaskDb db) =>
{
	if (await db.JiraTasks.FindAsync(id) is JiraTask jiraTask)
	{
		db.JiraTasks.Remove(jiraTask);
		await db.SaveChangesAsync();
		return Results.Ok(jiraTask);
	}

	return Results.NotFound();
});

app.UseStaticFiles();
app.Run();
