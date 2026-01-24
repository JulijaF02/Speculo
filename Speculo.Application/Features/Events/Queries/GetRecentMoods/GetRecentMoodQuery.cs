using MediatR;
namespace Speculo.Application.Features.Events.Queries.GetRecentMoods;

public record GetRecentMoodQuery : IRequest<IEnumerable<MoodLogDto>>;