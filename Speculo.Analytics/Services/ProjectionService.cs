using MongoDB.Driver;
using Speculo.Analytics.Models;

namespace Speculo.Analytics.Services;

public class ProjectionService
{
    private readonly IMongoCollection<DashboardProjection> _projections;
    private readonly IMongoCollection<ProcessedEvent> _processedEvents;
    private readonly ILogger<ProjectionService> _logger;

    public ProjectionService(IMongoDatabase database, ILogger<ProjectionService> logger)
    {
        _projections = database.GetCollection<DashboardProjection>("dashboard_projections");
        _processedEvents = database.GetCollection<ProcessedEvent>("processed_events");
        _logger = logger;
    }
    public async Task<bool> IsEventProcessedAsync(Guid eventId)
    {
        var exists = await _processedEvents
            .Find(e => e.EventId == eventId)
            .AnyAsync();
        return exists;
    }
    public async Task MarkEventProcessedAsync(Guid eventId, string eventType)
    {
        await _processedEvents.InsertOneAsync(new ProcessedEvent
        {
            EventId = eventId,
            EventType = eventType,
            ProcessedAt = DateTimeOffset.UtcNow
        });
    }

    public async Task ApplyMoodLoggedAsync(Guid userId, int score)
    {
        var filter = Builders<DashboardProjection>.Filter.Eq(p => p.UserId, userId);
        var projection = await _projections.Find(filter).FirstOrDefaultAsync();

        if (projection == null)
        {
            await _projections.InsertOneAsync(new DashboardProjection
            {
                UserId = userId,
                TotalMoodEntries = 1,
                AverageMoodScore = score,
                LatestMoodScore = score,
                LastUpdated = DateTimeOffset.UtcNow
            });
        }
        else
        {
            // Running average: ((old_avg * count) + new_value) / (count + 1)
            var newAvg = ((projection.AverageMoodScore * projection.TotalMoodEntries) + score)
                         / (projection.TotalMoodEntries + 1);

            var update = Builders<DashboardProjection>.Update
                .Inc(p => p.TotalMoodEntries, 1)
                .Set(p => p.AverageMoodScore, newAvg)
                .Set(p => p.LatestMoodScore, score)
                .Set(p => p.LastUpdated, DateTimeOffset.UtcNow);

            await _projections.UpdateOneAsync(filter, update);
        }

        _logger.LogInformation("Applied MoodLogged for user {UserId}, score: {Score}", userId, score);
    }

    public async Task ApplySleepLoggedAsync(Guid userId, decimal hours, int quality)
    {
        var filter = Builders<DashboardProjection>.Filter.Eq(p => p.UserId, userId);
        var projection = await _projections.Find(filter).FirstOrDefaultAsync();

        if (projection == null)
        {
            await _projections.InsertOneAsync(new DashboardProjection
            {
                UserId = userId,
                TotalSleepEntries = 1,
                AverageSleepHours = (double)hours,
                AverageSleepQuality = quality,
                LastUpdated = DateTimeOffset.UtcNow
            });
        }
        else
        {
            var newAvgHours = ((projection.AverageSleepHours * projection.TotalSleepEntries) + (double)hours)
                              / (projection.TotalSleepEntries + 1);
            var newAvgQuality = ((projection.AverageSleepQuality * projection.TotalSleepEntries) + quality)
                                / (projection.TotalSleepEntries + 1);

            var update = Builders<DashboardProjection>.Update
                .Inc(p => p.TotalSleepEntries, 1)
                .Set(p => p.AverageSleepHours, newAvgHours)
                .Set(p => p.AverageSleepQuality, newAvgQuality)
                .Set(p => p.LastUpdated, DateTimeOffset.UtcNow);

            await _projections.UpdateOneAsync(filter, update);
        }

        _logger.LogInformation("Applied SleepLogged for user {UserId}", userId);
    }

    public async Task ApplyMoneyLoggedAsync(Guid userId, decimal amount, string transactionType)
    {
        var filter = Builders<DashboardProjection>.Filter.Eq(p => p.UserId, userId);
        var projection = await _projections.Find(filter).FirstOrDefaultAsync();

        if (projection == null)
        {
            var doc = new DashboardProjection
            {
                UserId = userId,
                TotalTransactions = 1,
                LastUpdated = DateTimeOffset.UtcNow
            };

            if (transactionType == "Income")
                doc.TotalIncome = amount;
            else
                doc.TotalExpenses = amount;

            await _projections.InsertOneAsync(doc);
        }
        else
        {
            var updateBuilder = Builders<DashboardProjection>.Update
                .Inc(p => p.TotalTransactions, 1)
                .Set(p => p.LastUpdated, DateTimeOffset.UtcNow);

            if (transactionType == "Income")
                updateBuilder = updateBuilder.Inc(p => p.TotalIncome, amount);
            else
                updateBuilder = updateBuilder.Inc(p => p.TotalExpenses, amount);

            await _projections.UpdateOneAsync(filter, updateBuilder);
        }

        _logger.LogInformation("Applied MoneyLogged for user {UserId}, type: {Type}, amount: {Amount}",
            userId, transactionType, amount);
    }

    public async Task ApplyWorkoutLoggedAsync(Guid userId, int minutes, int score)
    {
        var filter = Builders<DashboardProjection>.Filter.Eq(p => p.UserId, userId);
        var projection = await _projections.Find(filter).FirstOrDefaultAsync();

        if (projection == null)
        {
            await _projections.InsertOneAsync(new DashboardProjection
            {
                UserId = userId,
                TotalWorkouts = 1,
                TotalWorkoutMinutes = minutes,
                AverageWorkoutScore = score,
                LastUpdated = DateTimeOffset.UtcNow
            });
        }
        else
        {
            var newAvgScore = ((projection.AverageWorkoutScore * projection.TotalWorkouts) + score)
                              / (projection.TotalWorkouts + 1);

            var update = Builders<DashboardProjection>.Update
                .Inc(p => p.TotalWorkouts, 1)
                .Inc(p => p.TotalWorkoutMinutes, minutes)
                .Set(p => p.AverageWorkoutScore, newAvgScore)
                .Set(p => p.LastUpdated, DateTimeOffset.UtcNow);

            await _projections.UpdateOneAsync(filter, update);
        }

        _logger.LogInformation("Applied WorkoutLogged for user {UserId}", userId);
    }

    public async Task<DashboardProjection?> GetDashboardAsync(Guid userId)
    {
        return await _projections
            .Find(p => p.UserId == userId)
            .FirstOrDefaultAsync();
    }
}
