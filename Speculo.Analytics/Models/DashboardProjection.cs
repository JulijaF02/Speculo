using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Speculo.Analytics.Models;

public class DashboardProjection
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    // Mood tracking
    public int TotalMoodEntries { get; set; }
    public double AverageMoodScore { get; set; }
    public int LatestMoodScore { get; set; }

    // Sleep tracking
    public int TotalSleepEntries { get; set; }
    public double AverageSleepHours { get; set; }
    public double AverageSleepQuality { get; set; }

    // Money tracking
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public int TotalTransactions { get; set; }

    // Workout tracking
    public int TotalWorkouts { get; set; }
    public int TotalWorkoutMinutes { get; set; }
    public double AverageWorkoutScore { get; set; }

    // Metadata
    public DateTimeOffset LastUpdated { get; set; }
}
