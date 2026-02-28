using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Commands.LogSleep;
using Speculo.Contracts.Events;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests.Features.Events.Commands;

public class LogSleepCommandHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly IEventBus _eventBusMock;
    private readonly LogSleepCommandHandler _handler;

    public LogSleepCommandHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _eventBusMock = Substitute.For<IEventBus>();
        _handler = new LogSleepCommandHandler(_eventStoreMock, _userProviderMock, _eventBusMock);
    }

    [Fact]
    public async Task Handle_ShouldSaveEventAndReturnId_WhenCommandIsValid()
    {
        //arrange
        var command = new LogSleepCommand(Hours: 8, Quality: 6);
        var userId = Guid.NewGuid();
        var expectedEventId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);
        _eventStoreMock.SaveAsync(Arg.Any<SleepLoggedEvent>()).Returns(expectedEventId);

        //act
        var result = await _handler.Handle(command, CancellationToken.None);

        //assert
        result.Should().Be(expectedEventId);
        await _eventStoreMock.Received(1).SaveAsync(Arg.Is<SleepLoggedEvent>(e =>
            e.UserId == userId &&
            e.Hours == 8 &&
            e.Quality == 6
        ));

        await _eventBusMock.Received(1).PublishAsync(
            Arg.Is<SleepLoggedIntegrationEvent>(e =>
                e.UserId == userId &&
                e.Hours == 8 &&
                e.Quality == 6),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        //arrange
        var command = new LogSleepCommand(Hours: 8, Quality: 6);
        _userProviderMock.UserId.Returns((Guid?)null);

        //act and assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );
    }
}

