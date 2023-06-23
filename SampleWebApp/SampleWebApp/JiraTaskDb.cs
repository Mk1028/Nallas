/*using Microsoft.EntityFrameworkCore;

public class JiraTaskDb : DbContext
{
	public JiraTaskDb(DbContextOptions<JiraTaskDb> options)
		: base(options) { }

	public DbSet<JiraTask> JiraTasks => Set<JiraTask>();
}*/

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Cosmos;
using System.Net;

public class JiraTaskDb
{
	private readonly CosmosClient _cosmosClient;
	private readonly Database _database;
	private readonly Container _container;
	public readonly TelemetryClient _telemetryClient;

	public JiraTaskDb()
	{
		string connectionString = "";
		string databaseName = "jira";
		string containerName = "jiracontainer";
		_cosmosClient = new CosmosClient(connectionString);
		_database = _cosmosClient.GetDatabase(databaseName);
		_container = _database.GetContainer(containerName);

		TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();

		configuration.ConnectionString = "";
		_telemetryClient = new TelemetryClient(configuration);
	}

	public async Task<IEnumerable<JiraTask>> GetJiraTasksAsync()
	{
		_telemetryClient.TrackTrace("GetJiraTasksAsync method called", SeverityLevel.Information);

		var query = _container.GetItemQueryIterator<JiraTask>();
		var results = new List<JiraTask>();

		while (query.HasMoreResults)
		{
			var response = await query.ReadNextAsync();
			results.AddRange(response.Resource);
		}

		return results;
	}

	public async Task<JiraTask?> GetJiraTaskByIdAsync(string id)
	{
		_telemetryClient.TrackTrace("GetJiraTaskByIdAsync method called", SeverityLevel.Information);

		try
		{
			var response = await _container.ReadItemAsync<JiraTask>(id, new PartitionKey(id));
			return response.Resource;
		}
		catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
		{
			var property = new Dictionary<string, string>
			{
				{ "ID", id }
			};
			// Item with the specified id was not found
			_telemetryClient.TrackException(ex, property);
			return null;
		}
	}

	public async Task AddJiraTaskAsync(JiraTask task)
	{
		_telemetryClient.TrackTrace("AddJiraTaskAsync method called", SeverityLevel.Information);
		await _container.CreateItemAsync(task);
	}

	public async Task UpdateJiraTaskAsync(JiraTask task)
	{
		_telemetryClient.TrackTrace("UpdateJiraTaskAsync method called", SeverityLevel.Information);

		await _container.UpsertItemAsync(task);
	}

	public async Task DeleteJiraTaskAsync(string id)
	{
		_telemetryClient.TrackTrace("DeleteJiraTaskAsync method called", SeverityLevel.Information);

		await _container.DeleteItemAsync<JiraTask>(id, new PartitionKey(id));
	}
}
