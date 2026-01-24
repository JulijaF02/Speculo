using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Commands.LogMood;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests;

[TestFixture]
public class LogMoodCommandHandlerTests
{
    private IEventStore _eventStoreMock = null!;
    private ICurrentUserProvider _userProviderMock = null!;
    private LogMoodCommandHandler _handler = null!;

    [SetUp]
    public void Setup()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();

        _handler = new LogMoodCommandHandler(_eventStoreMock, _userProviderMock);
    }

    [Test]
    public async Task Handle_ShouldSaveEventAndReturnId_WhenCommandIsValid()
    {
        // Arrange
        var command = new LogMoodCommand(Score: 8, Notes: "Feeling great!");
        var userId = Guid.NewGuid();
        var expectedEventId = Guid.NewGuid();

        _userProviderMock.UserId.Returns(userId);
        _eventStoreMock.SaveAsync(Arg.Any<MoodLoggedEvent>()).Returns(expectedEventId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(expectedEventId);

        await _eventStoreMock.Received(1).SaveAsync(Arg.Is<MoodLoggedEvent>(e =>
            e.UserId == userId &&
            e.Score == 8 &&
            e.Notes == "Feeling great!"
        ));
    }

    [Test]
    public void Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        // Arrange
        var command = new LogMoodCommand(Score: 5, Notes: "Neutral");
        _userProviderMock.UserId.Returns((Guid?)null);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );
    }
}
