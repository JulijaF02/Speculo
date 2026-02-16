using MediatR;
namespace Speculo.Application.Features.Events.Queries.GetRecentMoney;

public record GetRecentMoneyQuery : IRequest<IEnumerable<MoneyLogDto>>;
