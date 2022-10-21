namespace Beersender.Domain.Events;

public interface IEvent
{
    public Guid Aggregate_id { get; }
}