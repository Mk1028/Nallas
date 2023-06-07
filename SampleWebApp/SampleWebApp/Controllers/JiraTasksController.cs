using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
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
		var tasks = await _db.JiraTasks.ToListAsync();
		return Ok(tasks);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<JiraTask>> GetJiraTask(int id)
	{
		var task = await _db.JiraTasks.FindAsync(id);

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

		_db.JiraTasks.Add(inputJiraTask);
		await _db.SaveChangesAsync();

		return CreatedAtAction(nameof(GetJiraTask), new { id = inputJiraTask.Id }, inputJiraTask);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateJiraTask(int id, JiraTask inputJiraTask)
	{
		var jiraTask = await _db.JiraTasks.FindAsync(id);

		if (jiraTask == null)
			return NotFound();

		jiraTask.Name = inputJiraTask.Name;
		jiraTask.Status = inputJiraTask.Status;
		jiraTask.AssignedTo = inputJiraTask.AssignedTo;

		var validator = new JiraTaskValidator();
		ValidationResult result = await validator.ValidateAsync(jiraTask);

		if (!result.IsValid)
		{
			return BadRequest(result.Errors);
		}

		await _db.SaveChangesAsync();

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteJiraTask(int id)
	{
		var jiraTask = await _db.JiraTasks.FindAsync(id);

		if (jiraTask == null)
			return NotFound();

		_db.JiraTasks.Remove(jiraTask);
		await _db.SaveChangesAsync();

		return NoContent();
	}
}

