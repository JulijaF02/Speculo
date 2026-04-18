using FluentValidation;

namespace Speculo.Application.Features.Events.Commands.LogWorkout;

public class LogWorkoutCommandValidator : AbstractValidator<LogWorkoutCommand>
{
    public LogWorkoutCommandValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Workout type is required.")
            .MaximumLength(50)
            .WithMessage("Workout type cannot exceed 50 characters.");

        RuleFor(x => x.Minutes)
            .InclusiveBetween(1, 600)
            .WithMessage("Minutes must be between 1 and 600.");

        RuleFor(x => x.Score)
            .InclusiveBetween(1, 10)
            .WithMessage("Score must be between 1 and 10.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}
