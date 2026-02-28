using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Speculo.Analytics.Models;
public class ProcessedEvent
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid EventId { get; set; }

    public string EventType { get; set; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; set; }
}
