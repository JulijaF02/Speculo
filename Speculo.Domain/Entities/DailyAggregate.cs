using System;

namespace Speculo.Domain.Entities;

public class DailyAggregate
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly Date { get; set; }

    // Money
    public decimal TotalSpent { get; set; }

    // Time & Metrics
    public double TotalWorkHours { get; set; }
    public double TotalSleep { get; set; }
    public double AverageMood { get; set; }
    public int MoodEntryCount { get; set; }
}