using MongoDB.Driver;
using Speculo.Analytics.Models;

namespace Speculo.Analytics.Configuration;

public static class MongoDbIndexes
{
    public static async Task EnsureIndexesAsync(IMongoDatabase database)
    {
        var projections = database.GetCollection<DashboardProjection>("dashboard_projections");
        var processedEvents = database.GetCollection<ProcessedEvent>("processed_events");

        // Index on UserId for fast dashboard lookups
        await projections.Indexes.CreateOneAsync(
            new CreateIndexModel<DashboardProjection>(
                Builders<DashboardProjection>.IndexKeys.Ascending(p => p.UserId),
                new CreateIndexOptions { Unique = true }));

        // Index on EventId for fast idempotency checks
        await processedEvents.Indexes.CreateOneAsync(
            new CreateIndexModel<ProcessedEvent>(
                Builders<ProcessedEvent>.IndexKeys.Ascending(e => e.EventId),
                new CreateIndexOptions { Unique = true }));

        // TTL index to auto-expire old processed events after 7 days
        await processedEvents.Indexes.CreateOneAsync(
            new CreateIndexModel<ProcessedEvent>(
                Builders<ProcessedEvent>.IndexKeys.Ascending(e => e.ProcessedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(7) }));
    }
}
