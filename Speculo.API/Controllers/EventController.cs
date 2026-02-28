using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Speculo.Application.Features.Events.Commands.LogMoney;
using Speculo.Application.Features.Events.Commands.LogMood;
using Speculo.Application.Features.Events.Commands.LogSleep;
using Speculo.Application.Features.Events.Commands.LogWorkout;

namespace Speculo.API.Controllers;

/// <summary>
/// Write-side API for CQRS — logs life events to the event store and publishes to Kafka.
/// Read queries are served by the Analytics Service.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("standard")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
public class EventController(ISender sender) : ControllerBase
{
    /// <summary>Log a mood entry (score 1-10).</summary>
    [HttpPost("mood")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogMood([FromBody] LogMoodCommand command)
    {
        var eventId = await sender.Send(command);
        return Ok(eventId);
    }

    /// <summary>Log a workout session.</summary>
    [HttpPost("workout")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogWorkout([FromBody] LogWorkoutCommand command)
    {
        var eventId = await sender.Send(command);
        return Ok(eventId);
    }

    /// <summary>Log a sleep entry.</summary>
    [HttpPost("sleep")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogSleep([FromBody] LogSleepCommand command)
    {
        var eventId = await sender.Send(command);
        return Ok(eventId);
    }

    /// <summary>Log a financial transaction (income or expense).</summary>
    [HttpPost("money")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogMoney([FromBody] LogMoneyCommand command)
    {
        var eventId = await sender.Send(command);
        return Ok(eventId);
    }
}
