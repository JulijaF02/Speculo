using MongoDB.Driver;
using Speculo.Analytics.Models;

namespace Speculo.Analytics.Configuration;

public static class MongoDbIndexes
{
    public static async Task EnsureIndexesAsync(IMongoDatabase database)
    {
        var projections = database.GetCollection<DashboardProjection>("dashboard_projections");
        var processedEvents = database.GetCollection<ProcessedEvent>("processed_events");

        // UserId is already the _id field (BsonId), so its unique by default — no extra index needed


        // TTL index to auto-expire old processed events after 7 days
        await processedEvents.Indexes.CreateOneAsync(
            new CreateIndexModel<ProcessedEvent>(
                Builders<ProcessedEvent>.IndexKeys.Ascending(e => e.ProcessedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(7) }));
    }
}
