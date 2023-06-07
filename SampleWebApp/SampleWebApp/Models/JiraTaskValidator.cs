using FluentValidation;

public class JiraTaskValidator : AbstractValidator<JiraTask>
{
	public JiraTaskValidator()
	{
		RuleFor(task => task.Name)
			.NotEmpty().WithMessage("name is required")
			.Length(4, 30).WithMessage("name should be between 4 to 30 characters")
			.Must(m => m?.ToLower().Contains("task") == true).WithMessage("name should contain word task");

		RuleFor(task => task.AssignedTo)
			.IsInEnum().WithMessage("Invalid assignee");

		RuleFor(task => task.Status)
			.IsInEnum().WithMessage("Invalid status")
			.NotEqual(JiraStatuses.Done).WithMessage("Status cannot be Done");
	}
}
