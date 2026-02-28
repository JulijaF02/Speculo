using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Speculo.Application.Features.Events.Commands.LogMoney;
using Speculo.Application.Features.Events.Commands.LogMood;
using Speculo.Application.Features.Events.Commands.LogSleep;
using Speculo.Application.Features.Events.Commands.LogWorkout;

namespace Speculo.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("standard")]
public class EventController(ISender sender) : ControllerBase
{
    [HttpPost("mood")]
    public async Task<IActionResult> LogMood([FromBody] LogMoodCommand command)
    {
        var eventId = await sender.Send(command);
        return Ok(eventId);
    }

    [HttpPost("workout")]
    public async Task<IActionResult> LogWorkout([FromBody] LogWorkoutCommand command)
    {
        var eventId = await sender.Send(command);
        return Ok(eventId);
    }

    [HttpPost("sleep")]
    public async Task<IActionResult> LogSleep([FromBody] LogSleepCommand command)
    {
        var eventId = await sender.Send(command);
        return Ok(eventId);
    }

    [HttpPost("money")]
    public async Task<IActionResult> LogMoney([FromBody] LogMoneyCommand command)
    {
        var eventId = await sender.Send(command);
        return Ok(eventId);
    }
}
