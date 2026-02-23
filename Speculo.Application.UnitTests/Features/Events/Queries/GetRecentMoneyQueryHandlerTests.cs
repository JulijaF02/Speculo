using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Queries.GetRecentMoney;
using Speculo.Domain.Common;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests.Features.Events.Queries;

public class GetRecentMoneyQueryHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly GetRecentMoneyQueryHandler _handler;

    public GetRecentMoneyQueryHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _handler = new GetRecentMoneyQueryHandler(_eventStoreMock, _userProviderMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnMoneyLogs_WhenEventsExist()
    {
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var moneyEvent = new MoneyLoggedEvent(
            UserId: userId,
            Amount: -25.50m,
            Type: TransactionType.Expense,
            Category: "Food",
            Merchant: "Maxi",
            Notes: "Groceries"
        );

        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent> { moneyEvent });

        var result = await _handler.Handle(new GetRecentMoneyQuery(), CancellationToken.None);

        var resultList = result.ToList();
        resultList.Should().HaveCount(1);
        resultList[0].Amount.Should().Be(-25.50m);
        resultList[0].Type.Should().Be(TransactionType.Expense);
        resultList[0].Category.Should().Be("Food");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoMoneyEventsExist()
    {
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var moodEvent = new MoodLoggedEvent(UserId: userId, Score: 7, Notes: "Good");
        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent> { moodEvent });

        var result = await _handler.Handle(new GetRecentMoneyQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        _userProviderMock.UserId.Returns((Guid?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(new GetRecentMoneyQuery(), CancellationToken.None)
        );
    }
}
