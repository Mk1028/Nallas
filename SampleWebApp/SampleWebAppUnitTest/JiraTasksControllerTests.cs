using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
				Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
				var okResult = (OkObjectResult)result.Result;
				Assert.IsInstanceOfType(okResult.Value, typeof(List<JiraTask>));
				var returnedTasks = (List<JiraTask>)okResult.Value; 
				Assert.AreEqual(true, returnedTasks.Contains(tasks[0]));
				Assert.AreEqual(true, returnedTasks.Contains(tasks[1]));
			}
		}

		[TestMethod]
		public async Task GetJiraTask_WithValidId_ReturnsOkResultWithTask()
		{
			var task = new JiraTask { Id = 3, Name = "Task 3", Status = JiraStatuses.ToDo, AssignedTo = Assignees.A1 };

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
				Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
				var okResult = (OkObjectResult)result.Result;
				Assert.IsInstanceOfType(okResult.Value, typeof(JiraTask));
				var returnedTask = (JiraTask)okResult.Value;
				Assert.AreEqual(task.Id, returnedTask.Id);
				Assert.AreEqual(task.Name, returnedTask.Name);
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

				var result = await controller.GetJiraTask(11);

				// Assert
				Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
			}
		}

		[TestMethod]
		public async Task CreateJiraTask_WithValidTask_ReturnsCreatedAtActionResult()
		{
			var task = new JiraTask { Id = 4, Name = "Task 4", Status = JiraStatuses.ToDo, AssignedTo = Assignees.A1 };

			var dbOptions = new DbContextOptionsBuilder<JiraTaskDb>()
				.UseInMemoryDatabase(databaseName: "TestDb")
				.Options;

			using (var dbContext = new JiraTaskDb(dbOptions))
			{
				var controller = new JiraTasksController(dbContext);

				var result = await controller.CreateJiraTask(task);

				// Assert
				Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
				var createdAtActionResult = (CreatedAtActionResult)result.Result;
				Assert.IsInstanceOfType(createdAtActionResult.Value, typeof(JiraTask));
				var createdTask = (JiraTask)createdAtActionResult.Value;
				Assert.AreEqual(task.Name, createdTask.Name);
			}
		}

		[TestMethod]
		public async Task UpdateJiraTask_WithValidIdAndTask_ReturnsNoContentResult()
		{
			var task = new JiraTask { Id = 5, Name = "Task 5", Status = JiraStatuses.ToDo, AssignedTo = Assignees.A1 };
			var updatedTask = new JiraTask { Id = 5, Name = "Updated Task 5", Status = JiraStatuses.InProgress, AssignedTo = Assignees.A2 };

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
				Assert.IsInstanceOfType(result, typeof(NoContentResult));
				var taskFromDb = dbContext.JiraTasks.Find(task.Id);
				Assert.AreEqual(updatedTask.Name, taskFromDb.Name);
				Assert.AreEqual(updatedTask.Status, taskFromDb.Status);
				Assert.AreEqual(updatedTask.AssignedTo, taskFromDb.AssignedTo);
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

				var result = await controller.UpdateJiraTask(12, new JiraTask { Id = 13, Name = "Updated Task", AssignedTo = Assignees.A1 });

				// Assert
				Assert.IsInstanceOfType(result, typeof(NotFoundResult));
			}
		}

		[TestMethod]
		public async Task DeleteJiraTask_WithValidId_ReturnsOkResult()
		{
			var task = new JiraTask { Id = 6, Name = "Task 6", Status = JiraStatuses.ToDo, AssignedTo = Assignees.A1 };

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
				Assert.IsInstanceOfType(result, typeof(NoContentResult));
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

				var result = await controller.DeleteJiraTask(10);

				// Assert
				Assert.IsInstanceOfType(result, typeof(NotFoundResult));
			}
		}
	}
}
