
using MediatR;
using Speculo.Domain.Events;
namespace Speculo.Application.Features.Events.Commands.LogMoney;

public record LogMoneyCommand(decimal Amount, TransactionType Type, string Category, string? Merchant, string? Notes) : IRequest<Guid>;
