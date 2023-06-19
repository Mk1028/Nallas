using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunction
{
    public class QTrigger
    {
        private readonly ILogger _logger;

        public QTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QTrigger>();
        }

		[Function("QTrigger")]
		public void Run([QueueTrigger("myqueue-items", Connection = "connStrName")] string myQueueItem)
		{
			_logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

			JiraTask jiraTask = JsonConvert.DeserializeObject<JiraTask>(myQueueItem);
			HttpClient httpClient = new HttpClient();
			using (var content = new StringContent(JsonConvert.SerializeObject(jiraTask), System.Text.Encoding.UTF8, "application/json"))
			{
				HttpResponseMessage result = httpClient.PostAsync("https://localhost:7218/jiratasks", content).Result;
				if (result.StatusCode == System.Net.HttpStatusCode.Created)
				{
					_logger.LogInformation($"Successfully posted the jiraTask from Queue");
				}
				string returnValue = result.Content.ReadAsStringAsync().Result;
				_logger.LogInformation($"Return value of the post request: {returnValue}");
			}
		}
    }
}
