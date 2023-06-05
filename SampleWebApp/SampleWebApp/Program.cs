using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<JiraTaskDb>(opt => opt.UseInMemoryDatabase("JiraTask"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddCors();
var app = builder.Build();

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

app.MapGet("/jiratasks/a1", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.AssignedTo == Assignees.A1).ToListAsync());

app.MapGet("/jiratasks/a2", async (JiraTaskDb db) =>
	await db.JiraTasks.Where(t => t.AssignedTo == Assignees.A2).ToListAsync());

app.MapGet("/jiratasks/{id}", async (int id, JiraTaskDb db) =>
	await db.JiraTasks.FindAsync(id)
		is JiraTask jiratask
			? Results.Ok(jiratask)
			: Results.NotFound());

//Post Requests
app.MapPost("/jiratasks", async (JiraTask inputJiraTask, JiraTaskDb db) =>
{
	db.JiraTasks.Add(inputJiraTask);
	await db.SaveChangesAsync();

	return Results.Created($"/jiratasks/{inputJiraTask.Id}", inputJiraTask);
});

//Put Requests
app.MapPut("/jiratasks/{id}", async (int id, JiraTask inputJiraTask, JiraTaskDb db) =>
{
	var jiraTask = await db.JiraTasks.FindAsync(id);

	if (jiraTask is null) return Results.NotFound();

	jiraTask.Name = inputJiraTask.Name;
	jiraTask.Status = inputJiraTask.Status;

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