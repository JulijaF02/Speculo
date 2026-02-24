using FluentValidation;

namespace Speculo.Application.Features.Events.Commands.LogSleep;

public class LogSleepCommandValidator : AbstractValidator<LogSleepCommand>
{
    public LogSleepCommandValidator()
    {
        RuleFor(x => x.Hours)
            .InclusiveBetween(0, 24)
            .WithMessage("Hours must be between 0 and 24.");

        RuleFor(x => x.Quality)
            .InclusiveBetween(1, 10)
            .WithMessage("Quality must be between 1 and 10.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}
