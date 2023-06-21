using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace SampleWebAppUnitTests
{
	[TestClass]
	public class JiraTasksControllerTests
	{
		[TestMethod]
		public async Task GetJiraTasks_ReturnsOkResultWithListOfTasks()
		{
			var tasks = new List<JiraTask>
			{
				new JiraTask { Name = "Task 1", Status = JiraStatuses.ToDo, AssignedTo = Assignees.Person1 },
				new JiraTask { Name = "Task 2", Status = JiraStatuses.InProgress, AssignedTo = Assignees.Person2 }
			};

			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			// Add tasks to the Cosmos DB container
			foreach (var task in tasks)
			{
				await controller.CreateJiraTask(task);
			}

			var result = await controller.GetJiraTasks();

			// Assert
			Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
			var okResult = (OkObjectResult)result.Result;
			Assert.IsInstanceOfType(okResult.Value, typeof(List<JiraTask>));
			var returnedTasks = (List<JiraTask>)okResult.Value;
			Assert.AreEqual(true, returnedTasks.Any(t => t.Id == tasks[0].Id));
			Assert.AreEqual(true, returnedTasks.Any(t => t.Id == tasks[1].Id));

			await controller.DeleteJiraTask(tasks[0].Id);
			await controller.DeleteJiraTask(tasks[1].Id);
		}


		[TestMethod]
		public async Task GetJiraTask_WithValidId_ReturnsOkResultWithTask()
		{
			var task = new JiraTask { Name = "Task 3", Status = JiraStatuses.ToDo, AssignedTo = Assignees.Person1 };

			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			// Add task to the Cosmos DB container
			await controller.CreateJiraTask(task);

			var result = await controller.GetJiraTask(task.Id);

			// Assert
			Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
				var okResult = (OkObjectResult)result.Result;
				Assert.IsInstanceOfType(okResult.Value, typeof(JiraTask));
				var returnedTask = (JiraTask)okResult.Value;
				Assert.AreEqual(task.Id, returnedTask.Id);
				Assert.AreEqual(task.Name, returnedTask.Name);

			await controller.DeleteJiraTask(task.Id);
		}

		[TestMethod]
		public async Task GetJiraTask_WithInvalidId_ReturnsNotFoundResult()
		{
			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			var result = await controller.GetJiraTask(Guid.NewGuid());

			// Assert
			Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
		}

		[TestMethod]
		public async Task CreateJiraTask_WithValidTask_ReturnsCreatedAtActionResult()
		{
			var task = new JiraTask { Name = "Task 4", Status = JiraStatuses.ToDo, AssignedTo = Assignees.Person1 };

			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			var result = await controller.CreateJiraTask(task);

			// Assert
			Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
			var createdAtActionResult = (CreatedAtActionResult)result.Result;
			Assert.IsInstanceOfType(createdAtActionResult.Value, typeof(JiraTask));
			var createdTask = (JiraTask)createdAtActionResult.Value;
			Assert.AreEqual(task.Name, createdTask.Name);

			await controller.DeleteJiraTask(task.Id);
		}

		[TestMethod]
		public async Task CreateJiraTask_WithInvalidValidTask_ReturnsValidationErrors()
		{
			var invalidTask = new JiraTask { Name = "testing", Status = JiraStatuses.Done, AssignedTo = Assignees.Person1 };

			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			var result = await controller.CreateJiraTask(invalidTask);

			// Assert
			Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
			var badRequestObjectResult = (BadRequestObjectResult)result.Result;
			var errorMessages = (List<FluentValidation.Results.ValidationFailure>)badRequestObjectResult.Value;

			Assert.AreEqual(true, errorMessages[0].ErrorMessage.Contains("name should contain word task"));
			Assert.AreEqual(true, errorMessages[1].ErrorMessage.Contains("Status cannot be Done"));
		}

		[TestMethod]
		public async Task UpdateJiraTask_WithValidIdAndTask_ReturnsNoContentResult()
		{
			var task = new JiraTask { Name = "Task 5", Status = JiraStatuses.ToDo, AssignedTo = Assignees.Person1 };
			var updatedInvalidTask = new JiraTask { Id = task.Id, Name = "new", Status = JiraStatuses.InProgress, AssignedTo = Assignees.Person2 };

			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			// Add task to the Cosmos DB container
			await controller.CreateJiraTask(task);

			var result = await controller.UpdateJiraTask(task.Id, updatedInvalidTask);

			// Assert
			Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
			var badRequestObjectResult = (BadRequestObjectResult)result;
			var errorMessages = (List<FluentValidation.Results.ValidationFailure>)badRequestObjectResult.Value;

			Assert.AreEqual(true, errorMessages[0].ErrorMessage.Contains("name should be between 4 to 200 characters"));

			await controller.DeleteJiraTask(task.Id);
		}

		[TestMethod]
		public async Task UpdateJiraTask_WithInvalidValidTask_ReturnsValidationErrors()
		{
			var task = new JiraTask { Name = "Task 15", Status = JiraStatuses.ToDo, AssignedTo = Assignees.Person1 };
			var updatedTask = new JiraTask { Id = task.Id, Name = "Updated Task 15", Status = JiraStatuses.InProgress, AssignedTo = Assignees.Person2 };

			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			// Add task to the Cosmos DB container
			await controller.CreateJiraTask(task);

			var result = await controller.UpdateJiraTask(task.Id, updatedTask);

			// Assert
			Assert.IsInstanceOfType(result, typeof(NoContentResult));
			var actionResult = await controller.GetJiraTask(task.Id);
			var taskFromDb = (JiraTask)((OkObjectResult)actionResult.Result).Value;
			Assert.AreEqual(updatedTask.Name, taskFromDb.Name);
			Assert.AreEqual(updatedTask.Status, taskFromDb.Status);
			Assert.AreEqual(updatedTask.AssignedTo, taskFromDb.AssignedTo);

			await controller.DeleteJiraTask(task.Id);
		}

		[TestMethod]
		public async Task UpdateJiraTask_WithInvalidId_ReturnsNotFoundResult()
		{
			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			var result = await controller.UpdateJiraTask(Guid.NewGuid(), new JiraTask { Id = Guid.NewGuid(), Name = "Updated Task", AssignedTo = Assignees.Person1 });

			// Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundResult));
		}

		[TestMethod]
		public async Task DeleteJiraTask_WithValidId_ReturnsOkResult()
		{
			var task = new JiraTask { Name = "Task 6", Status = JiraStatuses.ToDo, AssignedTo = Assignees.Person1 };

			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			// Add task to the Cosmos DB container
			await controller.CreateJiraTask(task);

			var result = await controller.DeleteJiraTask(task.Id);

			// Assert
			Assert.IsInstanceOfType(result, typeof(NoContentResult));
		}

		[TestMethod]
		public async Task DeleteJiraTask_WithInvalidId_ReturnsNotFoundResult()
		{
			var jiraTaskDb = new JiraTaskDb(); // Create an instance of JiraTaskDb

			var controller = new JiraTasksController(jiraTaskDb); // Pass JiraTaskDb instance to the controller

			var result = await controller.DeleteJiraTask(Guid.NewGuid());

			// Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundResult));
		}
	}
}
