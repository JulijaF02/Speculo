using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Speculo.Application.Features.Events.Commands.LogMood;
using Speculo.Application.Features.Events.Commands.LogSleep;
using Speculo.Application.Features.Events.Commands.LogWorkout;
using Speculo.Application.Features.Events.Queries.GetRecentMoods;
using Speculo.Application.Features.Events.Queries.GetRecentSleep;
using Speculo.Application.Features.Events.Queries.GetRecentWorkouts;

namespace Speculo.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventController(ISender sender) : ControllerBase
{
    [HttpPost("mood")]
    public async Task<IActionResult> LogMood([FromBody] LogMoodCommand command)
    {
        var eventId = await sender.Send(command);

        return Ok(eventId);
    }

    [HttpGet("recentMoods")]
    public async Task<IActionResult> GetRecentMood()
    {
        var query = new GetRecentMoodQuery();
        var result = await sender.Send(query);
        return Ok(result);
    }

    [HttpPost("workout")]
    public async Task<IActionResult> LogWorkout([FromBody] LogWorkoutCommand command)
    {
        var eventId = await sender.Send(command);

        return Ok(eventId);
    }

    [HttpGet("recentWorkout")]
    public async Task<IActionResult> GetRecentWorkout()
    {
        var query = new GetRecentWorkoutQuery();
        var result = await sender.Send(query);
        return Ok(result);
    }

    [HttpPost("sleep")]
    public async Task<IActionResult> LogSleep([FromBody] LogSleepCommand command)
    {
        var eventId = await sender.Send(command);

        return Ok(eventId);
    }

    [HttpGet("recentSleep")]
    public async Task<IActionResult> GetRecentSleep()
    {
        var query = new GetRecentSleepQuery();
        var result = await sender.Send(query);
        return Ok(result);
    }

}

