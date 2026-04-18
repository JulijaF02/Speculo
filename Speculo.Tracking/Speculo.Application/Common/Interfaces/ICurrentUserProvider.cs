namespace Speculo.Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    Guid? UserId { get; }
}