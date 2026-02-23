using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Queries.GetSleepStats;
using Speculo.Domain.Common;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests.Features.Events.Queries;

public class GetSleepStatsQueryHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly GetSleepStatsQueryHandler _handler;

    public GetSleepStatsQueryHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _handler = new GetSleepStatsQueryHandler(_eventStoreMock, _userProviderMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectStats_WhenSleepEventsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var events = new List<IDomainEvent>
        {
            new SleepLoggedEvent(UserId: userId, Hours: 7.5m, Quality: 8),
            new SleepLoggedEvent(UserId: userId, Hours: 6.0m, Quality: 5),
            new SleepLoggedEvent(UserId: userId, Hours: 8.0m, Quality: 9),
        };

        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>()).Returns(events);

        // Act
        var result = await _handler.Handle(new GetSleepStatsQuery(Days: 30), CancellationToken.None);

        // Assert
        result.TotalLogs.Should().Be(3);
        result.BestQuality.Should().Be(9);
        result.WorstQuality.Should().Be(5);
        result.AverageHours.Should().BeApproximately(7.17, 0.01);
    }

    [Fact]
    public async Task Handle_ShouldReturnDefaultStats_WhenNoSleepEventsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        _eventStoreMock
            .GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent>());

        // Act
        var result = await _handler.Handle(new GetSleepStatsQuery(Days: 30), CancellationToken.None);

        // Assert
        result.TotalLogs.Should().Be(0);
        result.AverageHours.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        _userProviderMock.UserId.Returns((Guid?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(new GetSleepStatsQuery(Days: 30), CancellationToken.None)
        );
    }
}
