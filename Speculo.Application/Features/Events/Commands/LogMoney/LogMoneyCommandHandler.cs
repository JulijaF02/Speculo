using MediatR;
using Speculo.Application.Common.Interfaces;
using Speculo.Domain.Events;
namespace Speculo.Application.Features.Events.Commands.LogMoney;

public class LogMoneyCommandHandler(IEventStore eventStore, ICurrentUserProvider currentUserProvider)
: IRequestHandler<LogMoneyCommand, Guid>
{
    public async Task<Guid> Handle(LogMoneyCommand request, CancellationToken ct)
    {
        var userId = currentUserProvider.UserId ?? throw new UnauthorizedAccessException();
        var moneyEvent = new MoneyLoggedEvent
        (
            UserId: userId,
            Amount: request.Amount,
            Type: request.Type,
            Category: request.Category,
            Merchant: request.Merchant,
            Notes: request.Notes

        );
        var eventId = await eventStore.SaveAsync(moneyEvent, ct);
        return eventId;

    }
}

