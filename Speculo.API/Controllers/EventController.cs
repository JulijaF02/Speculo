using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speculo.Application.Features.Events.Commands.LogMood;

namespace Speculo.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
// Step 1: Inject the Mailman (ISender) using a Primary Constructor
public class EventController(ISender sender) : ControllerBase
{
    // Step 2: Define the POST "Door" for Moods
    [HttpPost("mood")]
    public async Task<IActionResult> LogMood([FromBody] LogMoodCommand command)
    {
        // Step 3: Send the command and return the Event ID
        var eventId = await sender.Send(command);

        return Ok(eventId);
    }
}