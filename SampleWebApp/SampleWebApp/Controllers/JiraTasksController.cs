using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

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

