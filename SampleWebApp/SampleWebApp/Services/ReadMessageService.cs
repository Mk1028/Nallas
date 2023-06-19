using Azure.Storage.Queues;
using System.Text.Json;

namespace SampleWebApp.Services;

public class ReadMessageService : BackgroundService
{
	public ILogger<JiraTask> logger { get; }
    public ReadMessageService(ILogger<JiraTask> logger)
    {
	    this.logger = logger;
    }

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while(!stoppingToken.IsCancellationRequested)
		{
			this.logger.LogInformation("Reading Message from Azure Queue Storage - Start");

			string connectionString = "";
			string queueName = "myqueue-items";
			var queueClient = new QueueClient(connectionString, queueName);

			var message = await queueClient.ReceiveMessageAsync();

			if(message != null && message.Value != null)
			{
				var messageBody = message.Value.Body;

			 	var jiraTask = JsonSerializer.Deserialize<JiraTask>(messageBody);

				// Deleting message from queue  
				await queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
				this.logger.LogInformation("Delete Message from Azure Queue with the MessageId::"+message.Value.MessageId);
			}

			this.logger.LogInformation("Reading Message from Azure Queue Storage - End");
			await Task.Delay(TimeSpan.FromSeconds(5));
		}
	}
}
