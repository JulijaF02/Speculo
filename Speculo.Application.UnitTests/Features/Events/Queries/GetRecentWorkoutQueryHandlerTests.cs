using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Queries.GetRecentWorkouts;
using Speculo.Domain.Common;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests.Features.Events.Queries;

public class GetRecentWorkoutQueryHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly GetRecentWorkoutQueryHandler _handler;

    public GetRecentWorkoutQueryHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _handler = new GetRecentWorkoutQueryHandler(_eventStoreMock, _userProviderMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnWorkoutLogs_WhenEventsExist()
    {
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var workoutEvent = new WorkoutLoggedEvent(UserId: userId, Type: "Running", Minutes: 45, Score: 8);
        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent> { workoutEvent });

        var result = await _handler.Handle(new GetRecentWorkoutQuery(), CancellationToken.None);

        var resultList = result.ToList();
        resultList.Should().HaveCount(1);
        resultList[0].Type.Should().Be("Running");
        resultList[0].Minutes.Should().Be(45);
        resultList[0].Score.Should().Be(8);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoWorkoutEventsExist()
    {
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var sleepEvent = new SleepLoggedEvent(UserId: userId, Hours: 7.5m, Quality: 4);
        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent> { sleepEvent });

        var result = await _handler.Handle(new GetRecentWorkoutQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        _userProviderMock.UserId.Returns((Guid?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(new GetRecentWorkoutQuery(), CancellationToken.None)
        );
    }
}
