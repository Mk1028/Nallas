using Newtonsoft.Json;

public enum JiraStatuses
{
	ToDo = 1,
	InProgress = 2,
	CodeReview = 3,
	Testing = 4,
	Done = 5
}

public enum Assignees
{
	Person1 = 1,
	Person2 = 2,
	Person3 = 3,
	Person4 = 4,
	Person5 = 5
}

public class JiraTask
{
	[JsonProperty(PropertyName = "id")]
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public JiraStatuses Status { get; set; }
	public required Assignees AssignedTo { get; set; }
	public string Description { get; set; }

	//public required string Email {get;set;}

	public JiraTask()
	{
		Id = Guid.NewGuid();
		Status = JiraStatuses.ToDo; // Set default value to ToDo
		Description = "I belong to JiraTask model in SampleWebApp project";
	}
}
