using FluentValidation;

namespace Speculo.Application.Features.Events.Commands.LogMoney;

public class LogMoneyCommandValidator : AbstractValidator<LogMoneyCommand>
{
    public LogMoneyCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required.")
            .MaximumLength(50)
            .WithMessage("Category cannot exceed 50 characters.");

        RuleFor(x => x.Merchant)
            .MaximumLength(100)
            .WithMessage("Merchant cannot exceed 100 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}
