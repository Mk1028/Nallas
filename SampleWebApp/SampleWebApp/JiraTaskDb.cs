/*using Microsoft.EntityFrameworkCore;

public class JiraTaskDb : DbContext
{
	public JiraTaskDb(DbContextOptions<JiraTaskDb> options)
		: base(options) { }

	public DbSet<JiraTask> JiraTasks => Set<JiraTask>();
}*/

using Microsoft.Azure.Cosmos;
using System.Net;

public class JiraTaskDb
{
	private readonly CosmosClient _cosmosClient;
	private readonly Database _database;
	private readonly Container _container;

	public JiraTaskDb()
	{
		string connectionString = "";
		string databaseName = "jira";
		string containerName = "jiracontainer";
		_cosmosClient = new CosmosClient(connectionString);
		_database = _cosmosClient.GetDatabase(databaseName);
		_container = _database.GetContainer(containerName);
	}

	public async Task<IEnumerable<JiraTask>> GetJiraTasksAsync()
	{
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
		try
		{
			var response = await _container.ReadItemAsync<JiraTask>(id, new PartitionKey(id));
			return response.Resource;
		}
		catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
		{
			// Item with the specified id was not found
			return null;
		}
	}

	public async Task AddJiraTaskAsync(JiraTask task)
	{
		await _container.CreateItemAsync(task);
	}

	public async Task UpdateJiraTaskAsync(JiraTask task)
	{
		await _container.UpsertItemAsync(task);
	}

	public async Task DeleteJiraTaskAsync(string id)
	{
		await _container.DeleteItemAsync<JiraTask>(id, new PartitionKey(id));
	}
}
