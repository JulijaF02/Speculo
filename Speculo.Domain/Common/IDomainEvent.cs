using System;

namespace Speculo.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn {get;}
}