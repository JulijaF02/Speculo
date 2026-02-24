using FluentValidation;

namespace Speculo.Application.Features.Events.Commands.LogMood;

public class LogMoodCommandValidator : AbstractValidator<LogMoodCommand>
{
    public LogMoodCommandValidator()
    {
        RuleFor(x => x.Score)
            .InclusiveBetween(1, 10)
            .WithMessage("Score must be between 1 and 10.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}