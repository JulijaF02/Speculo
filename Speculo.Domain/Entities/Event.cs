using System;

namespace Speculo.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty; 
    public string Payload { get; set; } = string.Empty; 
    public DateTimeOffset Timestamp { get; set; }
}