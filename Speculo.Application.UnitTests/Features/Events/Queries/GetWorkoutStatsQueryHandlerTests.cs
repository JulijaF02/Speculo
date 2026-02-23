using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Queries.GetWorkoutStats;
using Speculo.Domain.Common;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests.Features.Events.Queries;

public class GetWorkoutStatsQueryHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly GetWorkoutStatsQueryHandler _handler;

    public GetWorkoutStatsQueryHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _handler = new GetWorkoutStatsQueryHandler(_eventStoreMock, _userProviderMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectStats_WhenWorkoutEventsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var events = new List<IDomainEvent>
        {
            new WorkoutLoggedEvent(UserId: userId, Type: "Gym",  Minutes: 60, Score: 8),
            new WorkoutLoggedEvent(UserId: userId, Type: "Gym",  Minutes: 45, Score: 7),
            new WorkoutLoggedEvent(UserId: userId, Type: "Run",  Minutes: 30, Score: 9),
        };

        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>()).Returns(events);

        // Act
        var result = await _handler.Handle(new GetWorkoutStatsQuery(Days: 30), CancellationToken.None);

        // Assert
        result.TotalWorkouts.Should().Be(3);
        result.TotalMinutes.Should().Be(135);
        result.AverageMinutes.Should().BeApproximately(45.0, 0.01);
        result.MostCommonType.Should().Be("Gym"); // Gym appears twice, Run once
    }

    [Fact]
    public async Task Handle_ShouldReturnDefaultStats_WhenNoWorkoutEventsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        _eventStoreMock
            .GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent>());

        // Act
        var result = await _handler.Handle(new GetWorkoutStatsQuery(Days: 30), CancellationToken.None);

        // Assert
        result.TotalWorkouts.Should().Be(0);
        result.MostCommonType.Should().Be("N/A");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        _userProviderMock.UserId.Returns((Guid?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(new GetWorkoutStatsQuery(Days: 30), CancellationToken.None)
        );
    }
}
