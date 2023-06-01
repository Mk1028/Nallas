public enum JiraStatuses
{
	ToDo,
	InProgress,
	CodeReview,
	Testing,
	Done
}

public enum Assignees
{
	A1,
	A2
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
