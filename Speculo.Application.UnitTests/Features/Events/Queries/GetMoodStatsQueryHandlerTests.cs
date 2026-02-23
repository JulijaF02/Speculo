using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Queries.GetMoodStats;
using Speculo.Domain.Common;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests.Features.Events.Queries;

public class GetMoodStatsQueryHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly GetMoodStatsQueryHandler _handler;

    public GetMoodStatsQueryHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _handler = new GetMoodStatsQueryHandler(_eventStoreMock, _userProviderMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectStats_WhenMoodEventsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var events = new List<IDomainEvent>
        {
            new MoodLoggedEvent(UserId: userId, Score: 8),
            new MoodLoggedEvent(UserId: userId, Score: 4),
            new MoodLoggedEvent(UserId: userId, Score: 6),
        };

        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>()).Returns(events);

        // Act
        var result = await _handler.Handle(new GetMoodStatsQuery(Days: 30), CancellationToken.None);

        // Assert
        result.TotalLogs.Should().Be(3);
        result.BestScore.Should().Be(8);
        result.WorstScore.Should().Be(4);
        result.AverageScore.Should().BeApproximately(6.0, precision: 0.01);
        result.Days.Should().Be(30);
    }

    [Fact]
    public async Task Handle_ShouldReturnDefaultStats_WhenNoMoodEventsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        _eventStoreMock
            .GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent>());

        // Act
        var result = await _handler.Handle(new GetMoodStatsQuery(Days: 30), CancellationToken.None);

        // Assert
        result.TotalLogs.Should().Be(0);
        result.AverageScore.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        // Arrange
        _userProviderMock.UserId.Returns((Guid?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(new GetMoodStatsQuery(Days: 30), CancellationToken.None)
        );
    }
}
