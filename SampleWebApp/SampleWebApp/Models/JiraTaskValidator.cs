using FluentValidation;

public class JiraTaskValidator : AbstractValidator<JiraTask>
{
	public JiraTaskValidator()
	{
		RuleFor(task => task.Name)
			.Cascade(CascadeMode.StopOnFirstFailure)
			.NotEmpty().WithMessage("name is required")
			.Length(4, 200).WithMessage("name should be between 4 to 200 characters")
			.Must(m => m?.ToLower().Contains("task") == true).WithMessage("name should contain word task");

		RuleFor(task => task.AssignedTo)
			.IsInEnum().WithMessage("Invalid assignee");

		RuleFor(task => task.Status)
			.Cascade(CascadeMode.StopOnFirstFailure)
			.IsInEnum().WithMessage("Invalid status")
			.NotEqual(JiraStatuses.Done).WithMessage("Status cannot be Done");
		//RuleFor(task => task.Email).EmailAddress().WithMessage("Invalid Format");
		//RuleFor(task => task.Description).MinimumLength(100).WithMessage("Description should alteast be 100 characters");
	}
}
