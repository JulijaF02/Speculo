using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speculo.Analytics.Services;
using StackExchange.Redis;

namespace Speculo.Analytics.Controllers;

/// <summary>
/// Read-side API for CQRS — serves pre-computed projections from MongoDB.
/// Separated from Tracking Service to allow independent scaling of reads and writes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatsController : ControllerBase
{
    private readonly ProjectionService _projectionService;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<StatsController> _logger;

    public StatsController(
        ProjectionService projectionService,
        IConnectionMultiplexer redis,
        ILogger<StatsController> logger)
    {
        _projectionService = projectionService;
        _redis = redis;
        _logger = logger;
    }

    /// <summary>
    /// Returns the full dashboard projection for the authenticated user.
    /// Uses cache-aside pattern: Redis → MongoDB fallback, with event-driven invalidation.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = GetUserId();
        if (userId == null)
            return Unauthorized();

        var cacheKey = $"dashboard:{userId}";
        var db = _redis.GetDatabase();

        // Try Redis cache first
        var cached = await db.StringGetAsync(cacheKey);
        if (cached.HasValue)
        {
            _logger.LogDebug("Cache HIT for user {UserId}", userId);
            return Content(cached!, "application/json");
        }

        _logger.LogDebug("Cache MISS for user {UserId}", userId);

        // Fetch from MongoDB on cache miss
        var dashboard = await _projectionService.GetDashboardAsync(userId.Value);
        if (dashboard == null)
        {
            return Ok(new
            {
                message = "No data yet. Start logging your mood, sleep, money, or workouts!",
                userId = userId
            });
        }

        // Cache with 5-minute TTL (safety net, usually invalidated sooner by events)
        var json = JsonSerializer.Serialize(dashboard);
        await db.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(5));

        return Ok(dashboard);
    }

    /// <summary>
    /// Health check — verifies Redis connectivity.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        try
        {
            var db = _redis.GetDatabase();
            await db.PingAsync();
            return Ok(new { status = "healthy", service = "analytics" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new { status = "unhealthy", error = ex.Message });
        }
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst("sub") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim != null && Guid.TryParse(claim.Value, out var userId))
            return userId;
        return null;
    }
}
