using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using SampleWebApp;
using SampleWebApp.Controllers;
using SampleWebApp.Models;

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
			new JiraTask { Id = 1, Name = "Task 1", Status = JiraStatuses.ToDo, AssignedTo = Assignees.A1 },
			new JiraTask { Id = 2, Name = "Task 2", Status = JiraStatuses.InProgress, AssignedTo = Assignees.A2 }
		};

			var dbOptions = new DbContextOptionsBuilder<JiraTaskDb>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;

			using (var dbContext = new JiraTaskDb(dbOptions))
			{
				dbContext.JiraTasks.AddRange(tasks);
				dbContext.SaveChanges();

				var controller = new JiraTasksController(dbContext);

				var result = await controller.GetJiraTasks();

				// Assert
				var okResult = Assert.IsType<OkObjectResult>(result.Result);
				var returnedTasks = Assert.IsType<List<JiraTask>>(okResult.Value);
				Assert.Equal(tasks.Count, returnedTasks.Count);
			}
		}

		[TestMethod]
		public async Task GetJiraTask_WithValidId_ReturnsOkResultWithTask()
		{
			var task = new JiraTask { Id = 1, Name = "Task 1", Status = JiraStatuses.ToDo, AssignedTo = Assignees.A1 };

			var dbOptions = new DbContextOptionsBuilder<JiraTaskDb>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;

			using (var dbContext = new JiraTaskDb(dbOptions))
			{
				dbContext.JiraTasks.Add(task);
				dbContext.SaveChanges();

				var controller = new JiraTasksController(dbContext);

				var result = await controller.GetJiraTask(task.Id);

				// Assert
				var okResult = Assert.IsType<OkObjectResult>(result.Result);
				var returnedTask = Assert.IsType<JiraTask>(okResult.Value);
				Assert.Equal(task.Id, returnedTask.Id);
				Assert.Equal(task.Name, returnedTask.Name);
			}
		}

		[TestMethod]
		public async Task GetJiraTask_WithInvalidId_ReturnsNotFoundResult()
		{
			var dbOptions = new DbContextOptionsBuilder<JiraTaskDb>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;

			using (var dbContext = new JiraTaskDb(dbOptions))
			{
				var controller = new JiraTasksController(dbContext);

				var result = await controller.GetJiraTask(1);

				// Assert
				Assert.IsType<NotFoundResult>(result.Result);
			}
		}

		[TestMethod]
		public async Task CreateJiraTask_WithValidTask_ReturnsCreatedAtActionResult()
		{
			var task = new JiraTask { Name = "Task 1", Status = JiraStatuses.ToDo, AssignedTo = Assignees.A1 };

			var dbOptions = new DbContextOptionsBuilder<JiraTaskDb>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;

			using (var dbContext = new JiraTaskDb(dbOptions))
			{
				var controller = new JiraTasksController(dbContext);

				var result = await controller.CreateJiraTask(task);

				// Assert
				var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
				var createdTask = Assert.IsType<JiraTask>(createdAtActionResult.Value);
				Assert.Equal(task.Id, createdTask.Id);
				Assert.Equal(task.Name, createdTask.Name);
			}
		}

		[TestMethod]
		public async Task UpdateJiraTask_WithValidIdAndTask_ReturnsNoContentResult()
		{
			var task = new JiraTask { Id = 1, Name = "Task 1", Status = JiraStatuses.ToDo, AssignedTo = Assignees.A1 };
			var updatedTask = new JiraTask { Id = 1, Name = "Updated Task 1", Status = JiraStatuses.InProgress, AssignedTo = Assignees.A2 };

			var dbOptions = new DbContextOptionsBuilder<JiraTaskDb>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;

			using (var dbContext = new JiraTaskDb(dbOptions))
			{
				dbContext.JiraTasks.Add(task);
				dbContext.SaveChanges();

				var controller = new JiraTasksController(dbContext);

				var result = await controller.UpdateJiraTask(task.Id, updatedTask);

				// Assert
				Assert.IsType<NoContentResult>(result);
				var taskFromDb = dbContext.JiraTasks.Find(task.Id);
				Assert.Equal(updatedTask.Name, taskFromDb.Name);
				Assert.Equal(updatedTask.Status, taskFromDb.Status);
				Assert.Equal(updatedTask.AssignedTo, taskFromDb.AssignedTo);
			}
		}

		[TestMethod]
		public async Task UpdateJiraTask_WithInvalidId_ReturnsNotFoundResult()
		{
			var dbOptions = new DbContextOptionsBuilder<JiraTaskDb>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;

			using (var dbContext = new JiraTaskDb(dbOptions))
			{
				var controller = new JiraTasksController(dbContext);

				var result = await controller.UpdateJiraTask(1, new JiraTask { Id = 1, Name = "Updated Task", AssignedTo = Assignees.A1 });

				// Assert
				Assert.IsType<NotFoundResult>(result);
			}
		}

		[TestMethod]
		public async Task DeleteJiraTask_WithValidId_ReturnsOkResult()
		{
			var task = new JiraTask { Id = 1, Name = "Task 1", Status = JiraStatuses.ToDo, AssignedTo = Assignees.A1 };

			var dbOptions = new DbContextOptionsBuilder<JiraTaskDb>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;

			using (var dbContext = new JiraTaskDb(dbOptions))
			{
				dbContext.JiraTasks.Add(task);
				dbContext.SaveChanges();

				var controller = new JiraTasksController(dbContext);

				var result = await controller.DeleteJiraTask(task.Id);

				// Assert
				var okResult = Assert.IsType<OkObjectResult>(result);
				var deletedTask = Assert.IsType<JiraTask>(okResult.Value);
				Assert.Equal(task.Id, deletedTask.Id);
			}
		}

		[TestMethod]
		public async Task DeleteJiraTask_WithInvalidId_ReturnsNotFoundResult()
		{
			var dbOptions = new DbContextOptionsBuilder<JiraTaskDb>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;

			using (var dbContext = new JiraTaskDb(dbOptions))
			{
				var controller = new JiraTasksController(dbContext);

				var result = await controller.DeleteJiraTask(1);

				// Assert
				Assert.IsType<NotFoundResult>(result);
			}
		}
	}
}

