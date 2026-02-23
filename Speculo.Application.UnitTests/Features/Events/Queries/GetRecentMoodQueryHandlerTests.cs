using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Queries.GetRecentMoods;
using Speculo.Domain.Common;
using Speculo.Domain.Events;
namespace Speculo.Application.UnitTests.Features.Events.Queries;

public class GetRecentMoodQueryHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly GetRecentMoodQueryHandler _handler;

    public GetRecentMoodQueryHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _handler = new GetRecentMoodQueryHandler(_eventStoreMock, _userProviderMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnMoodLogs_WhenEventsExsist()
    {
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);
        var moodEvent = new MoodLoggedEvent(UserId: userId, Score: 5, Notes: "Good");
        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>()).Returns(new List<IDomainEvent> { moodEvent });

        var result = await _handler.Handle(new GetRecentMoodQuery(), CancellationToken.None);

        var resultList = result.ToList();
        resultList.Should().HaveCount(1);
        resultList[0].Score.Should().Be(5);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoMoodEventsExsist()
    {
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);
        var workoutEvent = new WorkoutLoggedEvent(UserId: userId, Type: "Gym", Minutes: 30, Score: 8);
        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>()).Returns(new List<IDomainEvent> { workoutEvent });

        var result = await _handler.Handle(new GetRecentMoodQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldThrowUnautorized_WhenUserIsNotFound()
    {
        _userProviderMock.UserId.Returns((Guid?)null);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(new GetRecentMoodQuery(), CancellationToken.None)
        );

    }
}