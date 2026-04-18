using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Contracts.Events;
using Speculo.Domain.Events;

namespace Speculo.Application.Features.Events.Commands.LogMoney;

public class LogMoneyCommandHandler(
    IEventStore eventStore,
    ICurrentUserProvider currentUserProvider,
    IEventBus eventBus)
    : IRequestHandler<LogMoneyCommand, Guid>
{
    public async Task<Guid> Handle(LogMoneyCommand request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId ?? throw new UnauthorizedAccessException();

        // 1. Save domain event to PostgreSQL
        var moneyEvent = new MoneyLoggedEvent(
            UserId: userId,
            Amount: request.Amount,
            Type: request.Type,
            Category: request.Category,
            Merchant: request.Merchant,
            Notes: request.Notes
        );

        var eventId = await eventStore.SaveAsync(moneyEvent, ct);

        // 2. Publish integration event to Kafka
        var integrationEvent = new MoneyLoggedIntegrationEvent(
            UserId: userId,
            Amount: request.Amount,
            TransactionType: request.Type.ToString(),
            Category: request.Category,
            Merchant: request.Merchant,
            Notes: request.Notes,
            LoggedAt: moneyEvent.OccurredOn
        );

        await eventBus.PublishAsync(integrationEvent, ct);

        return eventId;
    }
}

