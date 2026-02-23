using FluentAssertions;
using NSubstitute;
using Speculo.Application.Common.Interfaces;
using Speculo.Application.Features.Events.Queries.GetRecentSleep;
using Speculo.Domain.Common;
using Speculo.Domain.Events;

namespace Speculo.Application.UnitTests.Features.Events.Queries;

public class GetRecentSleepQueryHandlerTests
{
    private readonly IEventStore _eventStoreMock;
    private readonly ICurrentUserProvider _userProviderMock;
    private readonly GetRecentSleepQueryHandler _handler;

    public GetRecentSleepQueryHandlerTests()
    {
        _eventStoreMock = Substitute.For<IEventStore>();
        _userProviderMock = Substitute.For<ICurrentUserProvider>();
        _handler = new GetRecentSleepQueryHandler(_eventStoreMock, _userProviderMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnSleepLogs_WhenEventsExist()
    {
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var sleepEvent = new SleepLoggedEvent(UserId: userId, Hours: 7.5m, Quality: 4);
        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent> { sleepEvent });

        var result = await _handler.Handle(new GetRecentSleepQuery(), CancellationToken.None);

        var resultList = result.ToList();
        resultList.Should().HaveCount(1);
        resultList[0].Hours.Should().Be(7.5m);
        resultList[0].Quality.Should().Be(4);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenNoSleepEventsExist()
    {
        var userId = Guid.NewGuid();
        _userProviderMock.UserId.Returns(userId);

        var moneyEvent = new MoneyLoggedEvent(UserId: userId, Amount: 100m, Type: TransactionType.Income, Category: "Salary");
        _eventStoreMock.GetEventsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<IDomainEvent> { moneyEvent });

        var result = await _handler.Handle(new GetRecentSleepQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotFound()
    {
        _userProviderMock.UserId.Returns((Guid?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(new GetRecentSleepQuery(), CancellationToken.None)
        );
    }
}
