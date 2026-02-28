using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Commands.LogWorkout;
using Speculo.Contracts.Events;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests.Features.Events.Commands;

public class LogWorkoutCommandHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly IEventBus _eventBusMock;
    private readonly LogWorkoutCommandHandler _handler;

    public LogWorkoutCommandHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _eventBusMock = Substitute.For<IEventBus>();
        _handler = new LogWorkoutCommandHandler(_eventStoreMock, _userProviderMock, _eventBusMock);
    }

    [Fact]
    public async Task Handle_ShouldSaveEventAndReturnId_WhenCommandIsValid()
    {
        //arrange
        var command = new LogWorkoutCommand(Type: "Running", Minutes: 30, Score: 5);
        var userId = Guid.NewGuid();
        var expectedEventId = Guid.NewGuid();

        _userProviderMock.UserId.Returns(userId);
        _eventStoreMock.SaveAsync(Arg.Any<WorkoutLoggedEvent>()).Returns(expectedEventId);

        //Act
        var result = await _handler.Handle(command, CancellationToken.None);

        //assert
        result.Should().Be(expectedEventId);
        await _eventStoreMock.Received(1).SaveAsync(Arg.Is<WorkoutLoggedEvent>(e =>
          e.UserId == userId &&
          e.Type == "Running" &&
          e.Minutes == 30 &&
          e.Score == 5
      ));

        await _eventBusMock.Received(1).PublishAsync(
            Arg.Is<WorkoutLoggedIntegrationEvent>(e =>
                e.UserId == userId &&
                e.WorkoutType == "Running" &&
                e.Minutes == 30),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        var command = new LogWorkoutCommand(Type: "Yoga", Minutes: 30, Score: 10);
        _userProviderMock.UserId.Returns((Guid?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );
    }
}

