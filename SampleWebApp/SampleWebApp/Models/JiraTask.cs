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
	A1 = 1,
	A2 = 2
}

public class JiraTask
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public JiraStatuses Status { get; set; }
	public required Assignees AssignedTo { get; set; }
	public JiraTask()
	{
		Status = JiraStatuses.ToDo; // Set default value to ToDo
	}
}
