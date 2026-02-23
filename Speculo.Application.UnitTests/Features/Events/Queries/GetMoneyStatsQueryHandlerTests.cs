using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Queries.GetMoneyStats;
using Speculo.Domain.Common;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests.Features.Events.Queries;

public class GetMoneyStatsQueryHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly GetMoneyStatsQueryHandler _handler;

    public GetMoneyStatsQueryHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _handler = new GetMoneyStatsQueryHandler(_eventStoreMock, _userProviderMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectStats_WhenMoneyEventsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var events = new List<IDomainEvent>
        {
            new MoneyLoggedEvent(UserId: userId, Amount: 3000m, Type: TransactionType.Income,  Category: "Salary"),
            new MoneyLoggedEvent(UserId: userId, Amount: 800m,  Type: TransactionType.Expense, Category: "Rent"),
            new MoneyLoggedEvent(UserId: userId, Amount: 200m,  Type: TransactionType.Expense, Category: "Food"),
        };

        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>(), Arg.Any<DateTimeOffset?>()).Returns(events);

        // Act
        var result = await _handler.Handle(new GetMoneyStatsQuery(Days: 30), CancellationToken.None);

        // Assert
        result.TotalIncome.Should().Be(3000m);
        result.TotalExpenses.Should().Be(1000m);
        result.NetSavings.Should().Be(2000m);
        result.TotalTransactions.Should().Be(3);
    }

    [Fact]
    public async Task Handle_ShouldReturnDefaultStats_WhenNoMoneyEventsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        _eventStoreMock
            .GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent>());

        // Act
        var result = await _handler.Handle(new GetMoneyStatsQuery(Days: 30), CancellationToken.None);

        // Assert
        result.TotalIncome.Should().Be(0);
        result.TotalExpenses.Should().Be(0);
        result.NetSavings.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        _userProviderMock.UserId.Returns((Guid?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(new GetMoneyStatsQuery(Days: 30), CancellationToken.None)
        );
    }
}
