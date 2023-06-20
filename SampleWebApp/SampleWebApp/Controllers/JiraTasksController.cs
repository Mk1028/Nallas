using Azure.Storage.Queues;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class JiraTasksController : ControllerBase
{
	private readonly JiraTaskDb _db;

	public JiraTasksController(JiraTaskDb db)
	{
		_db = db;
	}

	[HttpGet]
	public async Task<ActionResult<List<JiraTask>>> GetJiraTasks()
	{
		var tasks = (await _db.GetJiraTasksAsync()).ToList();
		return Ok(tasks);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<JiraTask>> GetJiraTask(Guid id)
	{
		var task = await _db.GetJiraTaskByIdAsync(id.ToString());

		if (task == null)
			return NotFound();

		return Ok(task);
	}

	[HttpPost]
	public async Task<ActionResult<JiraTask>> CreateJiraTask(JiraTask inputJiraTask)
	{
		var validator = new JiraTaskValidator();
		ValidationResult result = await validator.ValidateAsync(inputJiraTask);

		if (!result.IsValid)
		{
			return BadRequest(result.Errors);
		}

		await _db.AddJiraTaskAsync(inputJiraTask);

		return CreatedAtAction(nameof(GetJiraTask), new { id = inputJiraTask.Id }, inputJiraTask);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateJiraTask(Guid id, JiraTask inputJiraTask)
	{
		var jiraTask = await _db.GetJiraTaskByIdAsync(id.ToString());

		if (jiraTask == null)
			return NotFound();

		var validator = new JiraTaskValidator();
		ValidationResult result = await validator.ValidateAsync(inputJiraTask);

		if (!result.IsValid)
		{
			return BadRequest(result.Errors);
		}

		/*jiraTask.Name = inputJiraTask.Name;
		jiraTask.Status = inputJiraTask.Status;
		jiraTask.AssignedTo = inputJiraTask.AssignedTo;
		jiraTask.Description = inputJiraTask.Description;*/
		await _db.UpdateJiraTaskAsync(inputJiraTask);

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteJiraTask(Guid id)
	{
		var jiraTask = await _db.GetJiraTaskByIdAsync(id.ToString());

		if (jiraTask == null)
			return NotFound();

		await _db.DeleteJiraTaskAsync(id.ToString());

		return NoContent();
	}
}

