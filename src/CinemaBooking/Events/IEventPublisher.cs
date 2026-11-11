namespace CinemaBooking.Events;

/// <summary>Used by the write side to announce what has happened.</summary>
public interface IEventPublisher
{
    void Publish<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
}
