using FluentValidation.Results;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v2/[controller]")]
[Produces("application/json")]
public class JiraTasksV2Controller : ControllerBase
{
	private readonly JiraTaskDb _db;
	private readonly TelemetryClient _telemetryClient;

	public JiraTasksV2Controller(JiraTaskDb db)
	{
		_db = db;
		_telemetryClient = _db._telemetryClient;
	}

	[HttpGet]
	public async Task<ActionResult<List<JiraTask>>> GetJiraTasks()
	{
		DateTimeOffset startTime = DateTimeOffset.UtcNow;

		_telemetryClient.TrackTrace("GetJiraTasks method called", SeverityLevel.Information);

		var tasks = (await _db.GetJiraTasksAsync()).ToList();

		TimeSpan duration = DateTimeOffset.UtcNow - startTime;

		_telemetryClient.TrackRequest("Get Jira Tasks", startTime, duration, "200", true);

		return Ok(tasks);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<JiraTask>> GetJiraTask(Guid id)
	{
		DateTimeOffset startTime = DateTimeOffset.UtcNow;

		_telemetryClient.TrackTrace("GetJiraTask method called", SeverityLevel.Information, new Dictionary<string, string> { { "ID", id.ToString() } });

		var task = await _db.GetJiraTaskByIdAsync(id.ToString());

		TimeSpan duration = DateTimeOffset.UtcNow - startTime;

		_telemetryClient.TrackRequest("Get Jira Task By Id", startTime, duration, "200", true);

		if (task == null)
			return NotFound();

		return Ok(task);
	}

	[HttpPost]
	public async Task<ActionResult<JiraTask>> CreateJiraTask(JiraTask inputJiraTask)
	{ 
		DateTimeOffset startTime = DateTimeOffset.UtcNow;

		var properties = new Dictionary<string, string>
		{
				{ "ID", inputJiraTask.Id.ToString() },
				{ "Name", inputJiraTask.Name }
			};
		_telemetryClient.TrackTrace("CreateJiraTask method called", SeverityLevel.Information, properties);

		var validator = new JiraTaskValidator();
		ValidationResult result = await validator.ValidateAsync(inputJiraTask);

		if (!result.IsValid)
		{
			return BadRequest(result.Errors);
		}

		await _db.AddJiraTaskAsync(inputJiraTask);

		TimeSpan duration = DateTimeOffset.UtcNow - startTime;

		_telemetryClient.TrackRequest("Create Jira Task", startTime, duration, "200", true);

		return CreatedAtAction(nameof(GetJiraTask), new { id = inputJiraTask.Id }, inputJiraTask);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateJiraTask(Guid id, JiraTask inputJiraTask)
	{
		DateTimeOffset startTime = DateTimeOffset.UtcNow;

		_telemetryClient.TrackTrace("UpdateJiraTask method called", SeverityLevel.Information, new Dictionary<string, string> { { "ID", id.ToString() } });

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

		TimeSpan duration = DateTimeOffset.UtcNow - startTime;

		_telemetryClient.TrackRequest("Update Jira Task", startTime, duration, "200", true);

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteJiraTask(Guid id)
	{
		DateTimeOffset startTime = DateTimeOffset.UtcNow;

		_telemetryClient.TrackTrace("DeleteJiraTask method called", SeverityLevel.Information, new Dictionary<string, string> { { "ID", id.ToString() } });

		var jiraTask = await _db.GetJiraTaskByIdAsync(id.ToString());

		if (jiraTask == null)
			return NotFound();

		await _db.DeleteJiraTaskAsync(id.ToString());

		TimeSpan duration = DateTimeOffset.UtcNow - startTime;

		_telemetryClient.TrackRequest("Delete Jira Task", startTime, duration, "200", true);

		return NoContent();
	}
}

