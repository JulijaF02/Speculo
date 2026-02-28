using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Commands.LogMoney;
using Speculo.Contracts.Events;
using Speculo.Domain.Events;
using Xunit;

namespace Speculo.Application.UnitTests.Features.Events.Commands;

public class LogMoneyCommandHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _currentUserProviderMock;
    private readonly IEventBus _eventBusMock;
    private readonly LogMoneyCommandHandler _handler;

    public LogMoneyCommandHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _currentUserProviderMock = Substitute.For<ICurrentUserProvider>();
        _eventBusMock = Substitute.For<IEventBus>();
        _handler = new LogMoneyCommandHandler(_eventStoreMock, _currentUserProviderMock, _eventBusMock);
    }

    [Fact]
    public async Task Handle_ShouldSaveExpenseEvent_WhenCommandIsValid()
    {
        // Arrange
        var command = new LogMoneyCommand
        (
            Amount: -50.00m,
            Type: TransactionType.Expense,
            Category: "Food",
            Merchant: "Starbucks",
            Notes: "Morning coffee"
        );

        var userId = Guid.NewGuid();
        var expectedEventId = Guid.NewGuid();

        _currentUserProviderMock.UserId.Returns(userId);
        _eventStoreMock.SaveAsync(Arg.Any<MoneyLoggedEvent>()).Returns(expectedEventId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(expectedEventId);

        await _eventStoreMock.Received(1).SaveAsync(Arg.Is<MoneyLoggedEvent>(e =>
            e.UserId == userId &&
            e.Amount == -50.00m &&
            e.Type == TransactionType.Expense &&
            e.Category == "Food"
        ));

        await _eventBusMock.Received(1).PublishAsync(
            Arg.Is<MoneyLoggedIntegrationEvent>(e =>
                e.UserId == userId &&
                e.Amount == -50.00m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        var command = new LogMoneyCommand
        (
            Amount: -50.00m,
            Type: TransactionType.Expense,
            Category: "Food",
            Merchant: "Starbucks",
            Notes: "Morning coffee"
        );

        _currentUserProviderMock.UserId.Returns((Guid?)null);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(command, CancellationToken.None)
        );

    }

    [Fact]
    public async Task Handle_ShouldSaveIncomeEvent_WhenCommandIsValid()
    {
        var command = new LogMoneyCommand
        (
            Amount: 50.00m,
            Type: TransactionType.Income,
            Category: "Bonus",
            Merchant: "Starbucks",
            Notes: "Morning coffee"
        );
        var userId = Guid.NewGuid();
        var expectedEventId = Guid.NewGuid();

        _currentUserProviderMock.UserId.Returns(userId);
        _eventStoreMock.SaveAsync(Arg.Any<MoneyLoggedEvent>()).Returns(expectedEventId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(expectedEventId);

        await _eventStoreMock.Received(1).SaveAsync(Arg.Is<MoneyLoggedEvent>(e =>
          e.UserId == userId &&
          e.Amount == 50.00m &&
          e.Type == TransactionType.Income &&
          e.Category == "Bonus"
      ));

        await _eventBusMock.Received(1).PublishAsync(
            Arg.Is<MoneyLoggedIntegrationEvent>(e =>
                e.UserId == userId &&
                e.Amount == 50.00m),
            Arg.Any<CancellationToken>());
    }

}

