using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Speculo.Application.Features.Events.Commands.LogMoney;
using Speculo.Application.Features.Events.Commands.LogMood;
using Speculo.Application.Features.Events.Commands.LogSleep;
using Speculo.Application.Features.Events.Commands.LogWorkout;
using Speculo.Application.Features.Events.Queries.GetRecentMoney;
using Speculo.Application.Features.Events.Queries.GetRecentMoods;
using Speculo.Application.Features.Events.Queries.GetRecentSleep;
using Speculo.Application.Features.Events.Queries.GetRecentWorkouts;
using Speculo.Application.Features.Events.Queries.GetMoodStats;
using Speculo.Application.Features.Events.Queries.GetWorkoutStats;
using Speculo.Application.Features.Events.Queries.GetSleepStats;
using Speculo.Application.Features.Events.Queries.GetMoneyStats;
using Microsoft.AspNetCore.RateLimiting;

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

    [HttpPost("money")]
    public async Task<IActionResult> LogMoney([FromBody] LogMoneyCommand command)
    {
        var eventId = await sender.Send(command);

        return Ok(eventId);
    }

    [HttpGet("recentMoney")]
    public async Task<IActionResult> GetRecentMoney()
    {
        var query = new GetRecentMoneyQuery();
        var result = await sender.Send(query);
        return Ok(result);
    }

    [HttpGet("moodStats")]
    public async Task<ActionResult> GetMoodStats([FromQuery] int days = 30)
    {
        var query = new GetMoodStatsQuery(days);
        var result = await sender.Send(query);
        return Ok(result);
    }

    [HttpGet("workoutStats")]
    public async Task<IActionResult> GetWorkoutStats([FromQuery] int days = 30)
    {
        var query = new GetWorkoutStatsQuery(days);
        var result = await sender.Send(query);
        return Ok(result);
    }

    [HttpGet("sleepStats")]
    public async Task<IActionResult> GetSleepStats([FromQuery] int days = 30)
    {
        var query = new GetSleepStatsQuery(days);
        var result = await sender.Send(query);
        return Ok(result);
    }

    [HttpGet("moneyStats")]
    public async Task<IActionResult> GetMoneyStats([FromQuery] int days = 30)
    {
        var query = new GetMoneyStatsQuery(days);
        var result = await sender.Send(query);
        return Ok(result);
    }
}

